using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTarget : TargetedEffect {
    protected override void Resolve(Card source, IEntity target) {
        Creature creature = target as Creature;
        if (creature != null) {
            creature.Destroy();
        }
    }
}
