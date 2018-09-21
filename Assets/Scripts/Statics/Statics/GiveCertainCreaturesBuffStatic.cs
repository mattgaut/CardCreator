using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveCertainCreaturesBuffStatic : GiveCreaturesBuffStatic {

    [SerializeField] Creature.CreatureType to_buff;
    public override bool AppliesTo(IEntity entity) {
        if (!other && ReferenceEquals(entity, source)) {
            return false;
        }

        Creature creature = entity as Creature;

        if (creature == null || creature.creature_type != to_buff) {
            return false;
        }

        return base.AppliesTo(entity);
    }
}
