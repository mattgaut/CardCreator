using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerManager))]
public class GameStateManager : MonoBehaviour {

    public static GameStateManager instance {
        get; private set;
    }

    public bool can_take_command {
        get { return !selecting_card; }
    }
    public bool can_process_command {
        get { return !attack_happening && !resolving_stack && !selecting_card; }
    }

    [SerializeField] CardSelector card_selector;

    TriggerManager trigger_manager;
    StaticAbilityManager static_ability_manager;
    TimedEffectManager timed_effect_manager;

    List<IStackEffect> stack;

    ICardSelectionEffect pending_selection_effect;
    IEntity selection_effect_source;

    bool attack_happening;
    bool resolving_stack;

    bool selecting_card;

    public void Awake() {
        trigger_manager = GetComponent<TriggerManager>();
        static_ability_manager = new StaticAbilityManager(this);
        timed_effect_manager = new TimedEffectManager();

        stack = new List<IStackEffect>();

        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void BeginGame() {
        foreach (Player p in GameManager.players) {
            if (p.hero_power.has_trigger) {
                trigger_manager.SubscribeTrigger(p.hero_power.trigger);
            }
        }
    }

    public void BeginTurn(Player p) {
        p.NoteBeginTurn();

        if (p.deck.count > 0) {
            DrawCard(p);
        }
        foreach (Creature c in p.field.cards) {
            c.NoteBeginTurn();
        }
    }   

    public void DrawCard(Player p) {
        if (!p.hand.full && p.deck.count > 0) {
            MoveCard(p.deck.TopCard(), p.hand);
        }
    }

    public void DrawCard(Player p, int amount) {
        for (int i = 0; i < amount; i++) {
            DrawCard(p);
        }
    }

    public void TryMoveCardToHand(Player p, Card c) {
        if (p.controller.hand.full) {
            BurnCard(c);
        } else {
            MoveCard(c, p.hand);
        }
    }

    public void DiscardCard(Card c) {
        MoveCard(c, c.controller.discard);
    }

    public void DiscardRandomCard(Player p) {
        if (p.hand.count > 0) {
            DiscardCard(p.hand.cards[Random.Range(0, p.hand.count)]);
        }
    }

    public void DiscardRandomCard(Player p, int amount) {
        for (int i = 0; i < amount; i++) {
            DiscardRandomCard(p);
        }
    }

    public void EndTurn(Player p) {
        timed_effect_manager.EndEffects(TimePoint.end_of_turn);

        foreach (Creature c in p.field.cards) {
            c.NoteEndTurn();
        }
    }

    public void UseHeroPower(Player p) {
        if (!TryUseHeroPower(p.hero_power)) {
            return;
        }

        AddToStack(p.hero_power);

        ResolveStack();

        p.NoteUsedHeroPower();
    }

    public void UseTargetedHeroPower(Player p, IEntity target) {
        if (!TrySetTarget(p.hero_power, target)) {
            return;
        }
        if (!TryUseHeroPower(p.hero_power)) {
            return;
        }

        AddToStack(p.hero_power);

        ResolveStack();

        p.NoteUsedHeroPower();
    }

    public void PlaySpellFromHand(Spell spell) {
        if (!TryPlayCard(spell)) {
            return;
        }
        MoveCard(spell, spell.controller.stack);

        AddTriggersToStack(trigger_manager.GetTriggers(new AfterSpellTriggerInfo(spell)));

        AddToStack(spell);

        AddTriggersToStack(trigger_manager.GetTriggers(new BeforeSpellTriggerInfo(spell)));

        ResolveStack();

        spell.controller.NotePlayedCard();

        MoveCard(spell, spell.controller.graveyard);

        ResolveStack();
    }

    public void PlaySecretFromHand(Secret secret) {
        if (!TryPlayCard(secret)) {
            return;
        }

        MoveCard(secret, secret.controller.stack);

        AddTriggersToStack(trigger_manager.GetTriggers(new AfterSpellTriggerInfo(secret)));

        AddToStack(secret);

        AddTriggersToStack(trigger_manager.GetTriggers(new BeforeSpellTriggerInfo(secret)));

        ResolveStack();

        secret.controller.NotePlayedCard();

        MoveCard(secret, secret.controller.secrets);

        trigger_manager.SubscribeTrigger(secret.secret_trigger);

        ResolveStack();
    }

    public void PlayWeaponFromHand(Weapon weapon) {
        if (TargetNecessary(weapon)) {
            return;
        }
        if (!TryPlayCard(weapon)) {
            return;
        }

        ResolveWeapon(weapon);
    }

    public void PlayWeaponWithTargetFromHand(Weapon weapon, IEntity target) {
        // Make sure creature needs a target and can target the intended target
        if (!TrySetTarget(weapon, target)) {
            return;
        }

        if (!TryPlayCard(weapon)) {
            return;
        }

        ResolveWeapon(weapon);
    }

    public void PlayCreatureFromHand(Creature creature, int position) {
        // Make sure creature can be played without target
        if (TargetNecessary(creature)) {
            return;
        }
        
        if (!TryPlayCard(creature)) {
            return;
        }

        ResolveCreature(creature, position);
    }

    public void PlayCreatureWithTargetFromHand(Creature creature, int position, IEntity target) {
        // Make sure creature needs a target and can target the intended target
        if (!TrySetTarget(creature, target)) {
            return;
        }

        if (!TryPlayCard(creature)) {
            return;
        }

        ResolveCreature(creature, position);        
    }

    public void TriggerSecret(Secret triggered) {
        MoveCard(triggered, triggered.controller.graveyard);

        trigger_manager.UnsubscribeTrigger(triggered.secret_trigger);
    }

    public Card CreateToken(CardContainer initial_container, Card card_to_create, int position = -1) {
        if (initial_container.full) {
            return null;
        }
        Card card = Instantiate(card_to_create);

        if (card.type == CardType.Creature && initial_container.zone == Zone.field) {
            (card as Creature).NoteSummon();
            foreach (TriggerInstance ti in trigger_manager.GetTriggers(new WheneverCreatureSummonedInfo(card as Creature))) {
                AddToStack(ti);
            }
        }

        if (position >= 0) {
            initial_container.AddCard(card, position);
        } else {
            initial_container.AddCard(card);
        }
        SubscribeEffects(card);

        return card;
    }

    public bool CanAttack(ICombatant attacker, ICombatant defender) {
        return attacker.controller != defender.controller && attacker.CanAttack(defender) && defender.CanBeAttacked(attacker);
    }

    public void Attack(ICombatant attacker, ICombatant defender) {
        if (CanAttack(attacker, defender)) {
            attack_happening = true;
            attacker.NoteAttack();
            int dealt = attacker.DealDamage(defender, attacker.attack);
            if (defender.retaliate) {
                defender.DealDamage(attacker, defender.attack);
            }
            attack_happening = false;

            AfterAttackTriggerInfo info = new AfterAttackTriggerInfo(attacker, defender, dealt);

            Creature creature = attacker as Creature;
            if (creature != null) {
                AddTriggersToStack(creature.abilities.GetLocalTriggers(info));
            }
            AddTriggersToStack(trigger_manager.GetTriggers(info));
        }
        ResolveStack();
    }

    public void TransformCreature(Creature to_transform, Creature to_copy) {
        int position = to_transform.container.CardPosition(to_transform);
        CardContainer initial_container = to_transform.container;

        RemoveCardFromPlay(to_transform);
        CreateToken(initial_container, to_copy, position);
    }

    public void ProcessDamageEvent(IEntity damager, ICombatant damaged, int damage) {
        bool resolve_after = !resolving_stack && !attack_happening;

        AfterDamageTakenTriggerInfo info = new AfterDamageTakenTriggerInfo(damager, damaged, damage);
        AfterDamageDealtTriggerInfo dealt_info = new AfterDamageDealtTriggerInfo(damager, damaged, damage);

        Creature creature = damager as Creature;
        if (creature != null) {
            AddTriggersToStack(creature.abilities.GetLocalTriggers(dealt_info));
        }
        AddTriggersToStack(trigger_manager.GetTriggers(dealt_info));

        creature = damaged as Creature;
        if (creature != null) {
            AddTriggersToStack(creature.abilities.GetLocalTriggers(info));
        }
        AddTriggersToStack(trigger_manager.GetTriggers(info));

        if (resolve_after) {
            ResolveStack();
        }
    }
    public void ProcessHealEvent(IEntity source, ICombatant healed, int amount) {
        bool resolve_after = !resolving_stack && !attack_happening;

        AfterCharacterHealedTriggerInfo info = new AfterCharacterHealedTriggerInfo(healed, amount);

        Creature creature = healed as Creature;
        if (creature != null) {
            AddTriggersToStack(creature.abilities.GetLocalTriggers(info));
        }
        AddTriggersToStack(trigger_manager.GetTriggers(info));

        if (resolve_after) {
            ResolveStack();
        }
    }

    public void TryReturnCreatureToHand(Creature c) {
        if (c.container.zone != Zone.field) {
            return;
        }

        if (c.controller.hand.full) {
            BurnCard(c);
        } else {
            MoveCard(c, c.controller.hand);
        }
    }

    public void BurnCard(Card c) {
        UnsubscribeEffects(c);

        Destroy(c.gameObject);
    }

    public void SelectCard(ICardSelectionEffect effect, IEntity source, List<Card> choose_from) {
        if (!selecting_card) {
            Debug.Log("Gere");
            selecting_card = true;
            pending_selection_effect = effect;
            selection_effect_source = source;

            StartCoroutine(WaitForCardSelection(choose_from));
        }
    }

    IEnumerator WaitForCardSelection(List<Card> choose_from) {
        yield return card_selector.StartSingleCardSelection(choose_from.ToArray());

        FinishSelectCard(card_selector.GetCardSelected(), choose_from);
    }

    void FinishSelectCard(Card selected, List<Card> cards) {
        if (selecting_card) {
            selecting_card = false;

            cards.Remove(selected);
            pending_selection_effect.FinishResolve(selection_effect_source, selected, cards);
            ResolveStack();
        }
    }

    void AddToStack(IStackEffect stack_effect) {
        stack.Add(stack_effect);
    }

    void ResolveStack() {
        CheckStateBasedEffects();

        resolving_stack = true;
        while (stack.Count > 0) {

            IStackEffect effect = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            effect.Resolve();

            if (selecting_card) {
                return;
            }

            CheckStateBasedEffects();            
        }
        resolving_stack = false;
    }

    void CheckStateBasedEffects() {
        bool change_made;
        do {
            static_ability_manager.UpdateStaticAbilities();

            change_made = false;
            List<Creature> dead_creatures = new List<Creature>();
            foreach (Player p in GameManager.players) {
                foreach (Creature c in p.field.cards) {
                    if (c.dead) {
                        dead_creatures.Add(c);
                    }
                }
            }

            if (dead_creatures.Count > 0) {
                for (int i = 0; i < dead_creatures.Count; i++) {
                    MoveCard(dead_creatures[i], dead_creatures[i].controller.graveyard);

                    // After Creature is move to Graveyard Add Deathrattle effects to stack
                    if (dead_creatures[i].mods.HasMod(Modifier.deathrattle)) {
                        AddToStack(dead_creatures[i].mods.deathrattle_info);
                    }
                }
                change_made = true;
            }

            List<Weapon> dead_weapons = new List<Weapon>();
            foreach (Player p in GameManager.players) {
                foreach (Weapon weapon in p.equip.cards) {
                    if (weapon.dead) {
                        dead_weapons.Add(weapon);
                    }
                }
            }

            if (dead_weapons.Count > 0) {
                for (int i = 0; i < dead_weapons.Count; i++) {
                    MoveCard(dead_weapons[i], dead_weapons[i].controller.graveyard);
                    dead_weapons[i].controller.SetWeapon(null);

                    // After Creature is move to Graveyard Add Deathrattle effects to stack
                    if (dead_weapons[i].mods.HasMod(Modifier.deathrattle)) {
                        AddToStack(dead_weapons[i].mods.deathrattle_info);
                    }
                }
                change_made = true;
            }
        } while (change_made);
    }

    void AddTriggersToStack(IEnumerable<TriggerInstance> triggers) {
        foreach (TriggerInstance trigger in triggers) {
            AddToStack(trigger);
        }
    }

    void UnsubscribeEffects(Card c) {
        foreach (TriggeredAbility ta in c.abilities.GetGlobalTriggersActiveInZone(c.container.zone)) {
            trigger_manager.UnsubscribeTrigger(ta);
        }
        foreach (StaticAbility sa in c.abilities.GetStaticAbilitiesActiveInZone(c.container.zone)) {
            static_ability_manager.UnsubscribeStaticAbility(sa);
        }
        static_ability_manager.RemoveCardFromAbilities(c);
    }
    void SubscribeEffects(Card c) {
        foreach (TriggeredAbility ta in c.abilities.GetGlobalTriggersActiveInZone(c.container.zone)) {
            trigger_manager.SubscribeTrigger(ta);
        }
        foreach (StaticAbility sa in c.abilities.GetStaticAbilitiesActiveInZone(c.container.zone)) {
            static_ability_manager.SubscribeStaticAbility(sa);
        }
        static_ability_manager.AddCardToAbilities(c);
    }

    bool TryPlayCard(Card card) {
        if (!card.CanPlay()) {
            return false;
        }
        if (card.controller.hand.ContainsCard(card) && card.controller.SpendMana(card.mana_cost)) {
            if (card.mods.HasMod(Modifier.overload)) {
                card.controller.LockManaCrystals(card.mods.overload_cost);
            }
            return true;
        }
        return false;
    }

    bool TryUseHeroPower(HeroPower power) {
        if (power.controller.can_use_hero_power && power.controller.SpendMana(power.mana_cost)) {
            return true;
        }
        return false;
    }

    void ResolveCreature(Creature creature, int position) {
        MoveCard(creature, creature.controller.stack);

        AddToStack(creature);

        AddBattlecryAndComboEffectsToStack(creature);

        foreach (TriggerInstance ti in trigger_manager.GetTriggers(new WheneverCreatureMinionPlayedInfo(creature))) {
            AddToStack(ti);
        }
        foreach (TriggerInstance ti in trigger_manager.GetTriggers(new WheneverCreatureSummonedInfo(creature))) {
            AddToStack(ti);
        }

        // Lock a position in field to save space for creature
        creature.controller.field.AddLock();        

        ResolveStack();

        creature.controller.field.RemoveLock();

        MoveCard(creature, creature.controller.field, position);

        creature.NoteSummon();

        ResolveStack();
        creature.controller.NotePlayedCard();
    }

    void ResolveWeapon(Weapon weapon) {

        MoveCard(weapon, weapon.controller.stack);

        AddToStack(weapon);

        AddBattlecryAndComboEffectsToStack(weapon);

        ResolveStack();

        // If A weapon is already equipped move it to the graveyard       
        if (weapon.controller.equip.TopCard() != null) {
            MoveCard(weapon.controller.equip.TopCard(), weapon.controller.graveyard);
            weapon.controller.SetWeapon(null);
        }

        MoveCard(weapon, weapon.controller.equip);
        weapon.NoteSummon();

        weapon.controller.SetWeapon(weapon);

        ResolveStack();
        weapon.controller.NotePlayedCard();
    }

    void AddBattlecryAndComboEffectsToStack(Card card) {
        bool has_combo = card.mods.HasMod(Modifier.combo) && card.controller.combo_active;
        bool has_battlecry = card.mods.HasMod(Modifier.battlecry);
        // If there are battlecry or combo effects Add them to stack 
        if (has_combo && has_battlecry) {
            // Dont add Battlecry if it is replaced
            if (!card.mods.combo_info.replaces_other_effects) {
                AddToStack(card.mods.battlecry_info);
            }
            AddToStack(card.mods.combo_info);
        } else if (has_combo) {
            AddToStack(card.mods.combo_info);
        } else if (has_battlecry) {
            AddToStack(card.mods.battlecry_info);
        }
    }

    bool TargetNecessary(Card card) {
        bool combo_active = card.controller.combo_active && card.mods.HasMod(Modifier.combo);
        if (combo_active && card.mods.combo_info.needs_target) {
            // Check if target exists
            if (TargetExists(card, card.mods.combo_info)) {
                return true;
            }
        }
        // Battlecry only relevant if not replaced by combo
        if (card.mods.HasMod(Modifier.battlecry) && card.mods.battlecry_info.needs_target &&
            (!combo_active || !card.mods.combo_info.replaces_other_effects)) {
            // Check if target Exists
            if (TargetExists(card, card.mods.battlecry_info)) {
                return true;
            }
        }
        return false;
    }

    bool TrySetTarget(Card card, IEntity target) {
        if (!target.CanBeTargeted(card)) {
            return false;
        }
        bool combo_active = card.controller.combo_active && card.mods.HasMod(Modifier.combo);
        if (combo_active && card.mods.combo_info.needs_target) {
            // Check if target is set successfully
            if (!card.mods.combo_info.SetTarget(target)) {
                return false;
            }
        }
        // Battlecry only relevant if not replaced by combo
        if (card.mods.HasMod(Modifier.battlecry) && card.mods.battlecry_info.needs_target &&
            (!combo_active || !card.mods.combo_info.replaces_other_effects)) {
            // Check if target is set Successfully
            if (!card.mods.battlecry_info.SetTarget(target)) {
                return false;
            }
        }

        return true;
    }

    bool TrySetTarget(HeroPower hero_power, IEntity target) {
        if (!target.CanBeTargeted(hero_power)) {
            return false;
        }
        return hero_power.SetTarget(target);
    }

    void RemoveCardFromPlay(Card c) {
        UnsubscribeEffects(c);

        c.container.RemoveCard(c);

        Destroy(c.gameObject);
    }

    public void MoveCard(Card c, CardContainer to) {
        UnsubscribeEffects(c);
        CardContainer.MoveCard(c, c.container, to);
        SubscribeEffects(c);
    }
    public void MoveCard(Card c, CardContainer to, int position) {
        UnsubscribeEffects(c);
        CardContainer.MoveCard(c, c.container, to, position);
        SubscribeEffects(c);
    }
    public bool TargetExists(IEntity source, ITargets targets) {
        foreach (Player p in GameManager.players) {
            if (targets.CanTarget(p)) {
                return true;
            }
            foreach (Card c in p.field.cards) {
                if (targets.CanTarget(c)) {
                    return true;
                }
            }
        }
        return false;
    }

    public void TrackTimedEffect(ITimedEffect effect, IEntity source) {
        timed_effect_manager.AddTimedEffect(effect, source);
    }
    public void TrackTimedEffect(ITimedEffect effect, IEntity source, IEntity target) {
        timed_effect_manager.AddTimedEffect(effect, source, target);
    }
}
