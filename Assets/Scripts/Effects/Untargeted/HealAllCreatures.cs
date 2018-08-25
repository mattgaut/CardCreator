using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealAllCreatures : UntargetedEffect {
    [SerializeField] bool friendly, enemy;
    [SerializeField] int heal;

    public override void Resolve(Card source) {
        List<Creature> creatures_to_affect = new List<Creature>();
        if (friendly) {
            creatures_to_affect.AddRange(source.controller.field.cards.OfType<Creature>());
        }
        if (enemy) {
            foreach (Player p in GameManager.players) {
                if (p == source.controller) continue;

                creatures_to_affect.AddRange(p.field.cards.OfType<Creature>());
            }
        }

        foreach (Creature c in creatures_to_affect) {
            c.Heal(source, heal);
        }
    }
}
