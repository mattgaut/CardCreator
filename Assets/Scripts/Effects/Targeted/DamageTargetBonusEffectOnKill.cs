using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTargetBonusEffectOnKill : TargetedEffect {

    [SerializeField] int damage;
    [SerializeField] UntargetedEffect bonus_effect;

    protected override void Resolve(IEntity source, IEntity target) {
        ICombatant comb = target as ICombatant;
        if (comb == null) {
            return;
        }

        source.DealDamage(comb, damage);

        if (comb.dead) {
            bonus_effect.Resolve(source);
        }
    }
}
