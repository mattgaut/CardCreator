using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainBuffForEachCreatureOnTheField : UntargetedEffect {

    [SerializeField] int attack_buff, health_buff;
    [SerializeField] bool friendly, enemy;

    public override void Resolve(IEntity source) {
        int count = 0;
        if (friendly) {
            count += source.controller.field.count;
        }
        if (enemy) {
            foreach (Player player in GameManager.OtherPlayers(source.controller)) {
                count += player.field.count;
            }
        }

        Creature creature = source as Creature;

        if (creature) {
            if (attack_buff != 0) {
                creature.attack.ApplyBuff(new StatBuff(source, BuffType.basic, count * attack_buff));
            }
            if (health_buff != 0) {
                creature.health.ApplyBuff(new StatBuff(source, BuffType.basic, count * health_buff));
            }
        }
    }
}
