using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformMinionTarget : TargetedEffect {

    [SerializeField] Creature transform_into;

    protected override void Resolve(IEntity source, IEntity target) {
        Creature creature = target as Creature;
        if (creature == null) {
            return;
        }

        GameStateManager.instance.TransformCreature(creature, transform_into);
    }
}
