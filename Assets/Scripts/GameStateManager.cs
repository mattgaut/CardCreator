using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerManager))]
public class GameStateManager : MonoBehaviour {

    public static GameStateManager instance {
        get; private set;
    }

    TriggerManager trigger_manager;

    List<IStackEffect> stack;

    public void Awake() {
        trigger_manager = GetComponent<TriggerManager>();

        stack = new List<IStackEffect>();

        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void BeginTurn(Player p) {
        p.AdvanceLockedMana();
        p.ResetAttacksTaken();
        p.AddOneMaxMana();
        p.FillMana();

        if (p.deck.count > 0) {
            DrawCard(p);
        }
        foreach (Creature c in p.field.cards) {
            c.NoteBeginTurn();
        }
    }

    public void DrawCard(Player p) {
        if (!p.hand.full) {
            p.hand.AddCard(p.deck.GetNext());
        }
    }

    public void EndTurn(Player p) {

    }

    public void PlayCardFromHand(Card c) {
        if (c.controller.hand.ContainsCard(c) && c.controller.SpendMana(c.mana_cost)) {
            MoveCard(c, c.controller.stack);
            AddToStack(c);
            ResolveStack();
            MoveCard(c, c.controller.graveyard);
        }
    }

    public void PlayCreatureFromHand(Creature c, int position) {
        if (c.controller.hand.ContainsCard(c) && c.controller.SpendMana(c.mana_cost)) {
            if (c.mods.HasMod(Modifiers.overload)) {
                c.controller.LockManaCrystals(c.mods.overload_cost);
            }
            MoveCard(c, c.controller.stack);
            AddToStack(c);
            if (c.mods.HasMod(Modifiers.battlecry)) {
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

    public bool CanAttack(ICombatant attacker, ICombatant defender) {
        return attacker.controller != defender.controller && attacker.CanAttack(defender) && defender.CanBeAttacked(attacker);
    }

    public void Attack(ICombatant attacker, ICombatant defender) {
        if (CanAttack(attacker, defender)) {
            attacker.NoteAttack();
            attacker.DealDamage(defender, attacker.attack);
            if (defender.retaliate) {
                defender.DealDamage(attacker, defender.attack);
            }
        }
        ResolveStack();
    }

    public int Heal(IEntity source, IHealth target, int healing) {
        return target.Heal(source, healing);
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
        } while (change_made);
    }

    public void MoveCard(Card c, CardContainer to) {
        foreach (TriggeredAbility ta in c.abilities.GetTriggersActiveInZone(c.container.zone)) {
            trigger_manager.UnsubscribeTrigger(ta);
        }
        CardContainer.MoveCard(c, c.container, to);
        foreach (TriggeredAbility ta in c.abilities.GetTriggersActiveInZone(c.container.zone)) {
            trigger_manager.SubscribeTrigger(ta);
        }
    }
    public void MoveCard(Card c, CardContainer to, int position) {
        foreach (TriggeredAbility ta in c.abilities.GetTriggersActiveInZone(c.container.zone)) {
            trigger_manager.UnsubscribeTrigger(ta);
        }
        CardContainer.MoveCard(c, c.container, to, position);
        foreach (TriggeredAbility ta in c.abilities.GetTriggersActiveInZone(c.container.zone)) {
            trigger_manager.SubscribeTrigger(ta);
        }
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
}
