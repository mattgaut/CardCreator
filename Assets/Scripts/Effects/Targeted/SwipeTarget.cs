using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeTarget : TargetedEffect {

    [SerializeField] int main_damage, splash_damage;

    protected override void Resolve(IEntity source, IEntity target) {
        List<ICombatant> to_hit = new List<ICombatant>();

        to_hit.Add(target.controller);
        foreach (ICombatant comb in target.controller.field.cards) {
            to_hit.Add(comb);
        }
        ICombatant main_target = target as ICombatant;
        if (main_target != null) {
            to_hit.Remove(main_target);

            source.DealDamage(main_target, main_damage);
        }

        foreach (ICombatant comb in to_hit) {
            source.DealDamage(comb, splash_damage);
        }
    }
}
