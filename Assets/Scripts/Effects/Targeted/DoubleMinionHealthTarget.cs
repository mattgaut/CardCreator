using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleMinionHealthTarget : TargetedEffect {

    protected override void Resolve(IEntity source, IEntity target) {
        Creature creature = target as Creature;
        if (creature == null) {
            return;
        }

        creature.health.ApplyBuff(creature.current_health);
    }
}
