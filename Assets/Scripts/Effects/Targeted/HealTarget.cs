using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealTarget : TargetedEffect {
    [SerializeField] int heal_amount;

    protected override void Resolve(IEntity source, IEntity target) {
        IHealth to_heal = target as IHealth;
        if (to_heal != null) {
            to_heal.Heal(source, heal_amount);
        }
    }
}
