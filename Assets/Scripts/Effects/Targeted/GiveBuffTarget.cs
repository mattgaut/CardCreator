using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveBuffTarget : TargetedEffect {

    [SerializeField] int health_buff_amount;
    [SerializeField] int attack_buff_amount;

    protected override void Resolve(IEntity source, IEntity target) {
        Creature creature = target as Creature;
        if (creature != null) {
            creature.attack.ApplyBuff(new StatBuff(source, StatBuff.Type.basic, attack_buff_amount));
            creature.health.ApplyBuff(new StatBuff(source, StatBuff.Type.basic, health_buff_amount));
        }
    }
}
