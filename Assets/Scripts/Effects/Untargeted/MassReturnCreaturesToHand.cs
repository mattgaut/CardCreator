using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MassReturnCreaturesToHand : UntargetedEffect {

    [SerializeField] bool friendly, enemy;

    public override void Resolve(IEntity source) {
        List<Creature> to_bounce = new List<Creature>();

        if (friendly) {
            to_bounce.AddRange(source.controller.field.cards.OfType<Creature>());
        }

        if (enemy) {
            foreach (Player p in GameManager.OtherPlayers(source.controller)) {
                to_bounce.AddRange(p.field.cards.OfType<Creature>());
            }
        }

        foreach (Creature c in to_bounce) {
            GameStateManager.instance.TryReturnCreatureToHand(c);
        }
    }
}
