using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GiveCertainCreaturesMassBuff : UntargetedEffect {

    [SerializeField] Creature.CreatureType creature_type;
    [SerializeField] bool friendly, enemy;

    [SerializeField] int attack_buff, health_buff;

    public override void Resolve(IEntity source) {
        List<Creature> to_buff = new List<Creature>();

        if (friendly) to_buff.AddRange(source.controller.field.cards.OfType<Creature>().Where((creature) => creature.creature_type == creature_type));

        if (enemy) {
            foreach (Player player in GameManager.OtherPlayers(source.controller)) {
                to_buff.AddRange(player.field.cards.OfType<Creature>().Where((creature) => creature.creature_type == creature_type));
            }
        }
        foreach (Creature creature in to_buff) {
            if (attack_buff != 0) creature.attack.ApplyBuff(new StatBuff(source, BuffType.basic, attack_buff));
            if (health_buff != 0) creature.attack.ApplyBuff(new StatBuff(source, BuffType.basic, health_buff));
        }
    }
}
