using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : Card, ICombatant {

    [SerializeField] int _attack, _max_health;

    [SerializeField] CreatureModifiers _mods;

    public override CardType type { get { return CardType.Creature; } }

    public int attack { get { return _attack; } }
    public int max_health { get { return _max_health; } }
    public int current_health { get; private set; }
    public bool poisioned { get; private set; }
    public bool dead { get { return current_health <= 0 || poisioned; } }

    public CreatureModifiers mods {
        get { return _mods; }
    }

    public bool can_attack { get { return attacks_taken < attacks_per_turn && attack > 0 && (!summoned_this_turn || mods.HasMod(Modifiers.charge) || mods.HasMod(Modifiers.rush)); } }
    public int attacks_taken { get; private set; }
    public int attacks_per_turn { get { return NumberOfAttacksPerTurn(); } }
    public bool retaliate { get { return true; } }
    public bool summoned_this_turn { get; private set; }

    public void NoteAttack() {
        attacks_taken += 1;
        if (mods.HasMod(Modifiers.stealth)) {
            mods.RemoveMod(Modifiers.stealth);
        }
    }
    public void ResetAttacksTaken() {
        attacks_taken = 0;
    }
    public void NoteSummon() {
        summoned_this_turn = true;
        poisioned = false;
    }
    public void NoteBeginTurn() {
        ResetAttacksTaken();
        summoned_this_turn = false;
    }


    public bool CanAttack(ICombatant target) {
        if (!can_attack) {
            return false;
        }
        if (summoned_this_turn && !mods.HasMod(Modifiers.charge)) {
            if (mods.HasMod(Modifiers.rush)) {
                if (target.entity_type == EntityType.player) {
                    return false;
                }
            } else {
                return false;
            }
        }
        return true;
    }

    public bool CanBeAttacked(ICombatant attacker) {
        if (container != controller.field) {
            return false;
        }
        if (mods.HasMod(Modifiers.stealth)) {
            return false;
        }
        if (!mods.HasMod(Modifiers.taunt)) {
            foreach (Creature c in controller.field.cards) {
                if (c != this && c.mods.HasMod(Modifiers.taunt)) {
                    return false;
                }
            }
        }
        return true;
    }

    public override bool CanBeTargeted(IEntity source) {
        return base.CanBeTargeted(source) && LocalCanBeTargeted(source);
    }

    bool LocalCanBeTargeted(IEntity source) {
        if (mods.HasMod(Modifiers.stealth) && source.controller != controller) {
            return false;
        }
        return true;
    }

    public override void Resolve() {
        current_health = max_health;
    }

    public int DealDamage(IDamageable target, int damage) {
        int damage_dealt = target.TakeDamage(this, damage);
        if (mods.HasMod(Modifiers.lifesteal)) {
            GameStateManager.instance.Heal(this, controller, damage_dealt);
        }
        return damage_dealt;
    }

    public int TakeDamage(IDamages source, int damage) {
        if (mods.HasMod(Modifiers.immune)) {
            return 0;
        }
        if (mods.HasMod(Modifiers.divine_shield)) {
            mods.RemoveMod(Modifiers.divine_shield);
            return 0;
        }
        current_health -= damage;
        if (source.entity_type == EntityType.card) {
            if ((source as Card).type == CardType.Creature) {
                if ((source as Creature).mods.HasMod(Modifiers.poisonous)) {
                    if (damage > 0) {
                        poisioned = true;
                    }
                }
            }
        }
        return damage;
    }

    public int Heal(IEntity source, int to_heal) {
        int old = current_health;
        current_health += to_heal;

        if (current_health > max_health) {
            current_health = max_health;
        }

        return current_health - old;
    }

    int NumberOfAttacksPerTurn() {
        if (mods.HasMod(Modifiers.mega_windfury)) {
            return 4;
        }
        if (mods.HasMod(Modifiers.windfury)) {
            return 2;
        }
        return 1;
    }
}
