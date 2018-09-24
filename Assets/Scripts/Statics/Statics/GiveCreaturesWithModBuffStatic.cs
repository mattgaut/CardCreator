using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveCreaturesWithModBuffStatic : GiveCreaturesBuffStatic {

    [SerializeField] Modifier mod_to_check;

    public override bool AppliesTo(IEntity entity) {

        Card card = entity as Card;

        if (card == null || !card.mods.HasMod(mod_to_check)) {
            return false;
        }

        return base.AppliesTo(entity);
    }
}
