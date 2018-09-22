using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTargetBonusWithCreature : DamageTarget {

    [SerializeField] int bonus_damage;
    [SerializeField] Creature.CreatureType creature_type;

    protected override void Resolve(IEntity source, IEntity target) {
        bool has_creature = false;

        foreach (Creature creature in source.controller.field.cards) {
            if (creature.creature_type == creature_type) {
                has_creature = true;
                break;
            }
        }

        if (has_creature) {
            IDamageable damageable = target as IDamageable;
            if (damageable != null) {
                source.DealDamage(damageable, damage + bonus_damage);
            }
        } else {
            base.Resolve(source, target);
        }
    }
}
