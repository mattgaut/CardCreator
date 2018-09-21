using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GiveAdjacentCreaturesBuff : StaticAbility {

    public override bool AppliesTo(IEntity entity) {
        if (source.type != CardType.Creature) {
            return false;
        }

        Card card = entity as Card;
        if (card == null) {
            return false;
        }

        if (card.container != source.container) {
            return false;
        }
        int source_position = source.container.CardPosition(source);
        int card_position = card.container.CardPosition(card);

        if (System.Math.Abs(card_position - source_position) != 1) {
            return false;
        }

        return base.AppliesTo(entity);
    }

}
