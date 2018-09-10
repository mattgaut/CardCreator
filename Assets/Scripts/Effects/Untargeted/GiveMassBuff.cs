using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GiveMassBuff : UntargetedEffect {

    [SerializeField] int health_buff_amount, attack_buff_amount;
    [SerializeField] bool friendly, enemy;

    public override void Resolve(IEntity source) {
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
            c.attack.ApplyBuff(attack_buff_amount);
            c.health.ApplyBuff(health_buff_amount);
        }
    }
}
