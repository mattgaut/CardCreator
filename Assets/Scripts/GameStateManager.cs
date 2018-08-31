using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerManager))]
public class GameStateManager : MonoBehaviour {

    public static GameStateManager instance {
        get; private set;
    }

    TriggerManager trigger_manager;
    StaticAbilityManager static_ability_manager;
    TimedEffectManager timed_effect_manager;

    List<IStackEffect> stack;

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
        if (!p.hand.full) {
            MoveCard(p.deck.GetNext(), p.hand);
        }
    }

    public void DrawCard(Player p, int amount) {
        for (int i = 0; i < amount; i++) {
            DrawCard(p);
        }
    }

    public void EndTurn(Player p) {
        timed_effect_manager.EndEffects(TimePoint.end_of_turn);
    }

    public void PlayCardFromHand(Card c) {
        if (c.controller.hand.ContainsCard(c) && c.controller.SpendMana(c.mana_cost)) {
            MoveCard(c, c.controller.stack);
            AddToStack(c);
            ResolveStack();
            MoveCard(c, c.controller.graveyard);
        }
    }

    public void PlaySpellFromHand(Spell spell) {
        if (spell.controller.hand.ContainsCard(spell) && spell.controller.SpendMana(spell.mana_cost)) {
            if (spell.mods.HasMod(Modifier.overload)) {
                spell.controller.LockManaCrystals(spell.mods.overload_cost);
            }
            MoveCard(spell, spell.controller.stack);
            AddTriggersToStack(trigger_manager.GetTriggers(new AfterSpellTriggerInfo(spell)));
            AddToStack(spell);
            AddTriggersToStack(trigger_manager.GetTriggers(new BeforeSpellTriggerInfo(spell)));
            ResolveStack();
            MoveCard(spell, spell.controller.graveyard);
        }
    }

    public void PlayCreatureFromHand(Creature c, int position) {
        if (c.mods.HasMod(Modifier.battlecry) && c.mods.battlecry_info.needs_target) {
            if (TargetExists(c, c.mods.battlecry_info)) {
                return;
            }
        }
        if (c.controller.hand.ContainsCard(c) && c.controller.SpendMana(c.mana_cost)) {
            if (c.mods.HasMod(Modifier.overload)) {
                c.controller.LockManaCrystals(c.mods.overload_cost);
            }
            MoveCard(c, c.controller.stack);
            AddToStack(c);
            if (c.mods.HasMod(Modifier.battlecry)) {
                AddToStack(c.mods.battlecry_info);
            }
            ResolveStack();
            MoveCard(c, c.controller.field, position);
        
            c.NoteSummon();
            foreach (TriggeredAbility ta in trigger_manager.GetTriggers(new ETBTriggerInfo(c))) {
                AddToStack(ta);
            }
            ResolveStack();
        }
    }

    public void PlayCreatureWithTargetFromHand(Creature c, int position, IEntity target) {
        if (c.mods.battlecry_info.needs_target) {
            if (!target.CanBeTargeted(c)) {
                return;
            }
            if (!c.mods.battlecry_info.SetTarget(target)) {
                return;
            }
        } else {
            return;
        }

        if (c.controller.hand.ContainsCard(c) && c.controller.SpendMana(c.mana_cost)) {
            if (c.mods.HasMod(Modifier.overload)) {
                c.controller.LockManaCrystals(c.mods.overload_cost);
            }
            MoveCard(c, c.controller.stack);
            AddToStack(c);
            if (c.mods.HasMod(Modifier.battlecry)) {
                AddToStack(c.mods.battlecry_info);
            }
            ResolveStack();
            MoveCard(c, c.controller.field, position);

            c.NoteSummon();
            foreach (TriggeredAbility ta in trigger_manager.GetTriggers(new ETBTriggerInfo(c))) {
                AddToStack(ta);
            }
            ResolveStack();
        }
    }

    public Card CreateToken(CardContainer initial_container, Card card_to_create, int position = -1) {
        if (initial_container.full) {
            return null;
        }
        Card card = Instantiate(card_to_create);

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
            attacker.NoteAttack();
            int dealt = attacker.DealDamage(defender, attacker.attack);
            if (defender.retaliate) {
                defender.DealDamage(attacker, defender.attack);
            }

            AfterAttackTriggerInfo info = new AfterAttackTriggerInfo(attacker, defender, dealt);

            Creature creature = attacker as Creature;
            if (creature != null) {
                AddTriggersToStack(creature.abilities.GetLocalTriggers(info));
            }
            AddTriggersToStack(trigger_manager.GetTriggers(info));
        }
        ResolveStack();
    }

    void AddToStack(IStackEffect stack_effect) {
        stack.Add(stack_effect);
    }

    void ResolveStack() {
        CheckStateBasedEffects();
        while (stack.Count > 0) {
            stack[stack.Count - 1].Resolve();
            stack.RemoveAt(stack.Count - 1);
            CheckStateBasedEffects();
        }
    }

    void CheckStateBasedEffects() {
        bool change_made;
        do {
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
                }
                change_made = true;
            }

            // ToDo: Add Section to check static abilities are still valid
        } while (change_made);
    }

    void AddTriggersToStack(IEnumerable<TriggeredAbility> triggers) {
        foreach (TriggeredAbility trigger in triggers) {
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

    public void TrackTimedEffect(ITimedEffect effect, Card source) {
        timed_effect_manager.AddTimedEffect(effect, source);
    }
    public void TrackTimedEffect(ITimedEffect effect, Card source, IEntity target) {
        timed_effect_manager.AddTimedEffect(effect, source, target);
    }
}
