using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTarget : TargetedEffect {
    [SerializeField] int damage;
    protected override void Resolve(Card source, IEntity target) {
        IDamageable damageable = target as IDamageable;
        if (damageable != null) {
            source.controller.DealDamage(damageable, damage);
        }
    }
}
