using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveBuffTarget : TargetedEffect {

    [SerializeField] int health_buff_amount;
    [SerializeField] int attack_buff_amount;

    protected override void Resolve(IEntity source, IEntity target) {
        Creature creature = target as Creature;
        if (creature != null) {
            creature.attack.ApplyBuff(new StatBuff(source, BuffType.basic, attack_buff_amount));
            creature.health.ApplyBuff(new StatBuff(source, BuffType.basic, health_buff_amount));
        }
    }
}
