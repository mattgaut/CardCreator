using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnMinionToHandTarget : TargetedEffect {

    protected override void Resolve(IEntity source, IEntity target) {
        Creature creature = target as Creature;

        if (creature == null || creature.container.zone != Zone.field) {
            return;
        }

        GameStateManager.instance.TryReturnCreatureToHand(creature);
    }
}
