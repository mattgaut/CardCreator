using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveCertainCreaturesBuffStatic : GiveCreaturesBuffStatic {

    [SerializeField] CompareEntity type_to_buff;

    public override bool AppliesTo(IEntity entity) {

        if (!type_to_buff.CompareTo(entity, source)) {
            return false;
        }

        return base.AppliesTo(entity);
    }
}
