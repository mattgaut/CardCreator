using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullHealTarget : TargetedEffect {
    protected override void Resolve(Card source, IEntity target) {
        IHealth to_heal = target as IHealth;
        if (to_heal != null) {
            to_heal.Heal(source, to_heal.max_health - to_heal.current_health);
        }
    }
}
