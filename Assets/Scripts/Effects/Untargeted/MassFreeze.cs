using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MassFreeze : UntargetedEffect {

    [SerializeField] bool friendly, enemy;
    [SerializeField] bool affects_players;

    public override void Resolve(IEntity source) {
        if (affects_players) {
            if (friendly) {
                source.controller.Freeze();
            }
            if (enemy) {
                foreach (Player player in GameManager.players) {
                    if (player == source.controller) {
                        continue;
                    }
                    player.Freeze();
                }
            }
        }

        if (friendly) {
            foreach (Creature creature in source.controller.field.cards.OfType<Creature>()) {
                creature.Freeze();
            }
        }
        
        if (enemy) {
            foreach (Player player in GameManager.players) {
                if (player == source.controller) {
                    continue;
                }
                foreach (Creature creature in player.field.cards.OfType<Creature>()) {
                    creature.Freeze();
                }
            }
        }
    }
}
