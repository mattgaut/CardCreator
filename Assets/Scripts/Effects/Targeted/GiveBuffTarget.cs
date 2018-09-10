using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveBuffTarget : TargetedEffect {

    [SerializeField] int health_buff_amount;
    [SerializeField] int attack_buff_amount;

    protected override void Resolve(IEntity source, IEntity target) {
        Creature creature = target as Creature;
        if (creature != null) {
            creature.attack.ApplyBuff(attack_buff_amount);
            creature.health.ApplyBuff(health_buff_amount);
        }
    }
}
