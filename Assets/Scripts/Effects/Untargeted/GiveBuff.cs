using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveBuff : UntargetedEffect {

    [SerializeField] int health_buff_amount, attack_buff_amount;

    public override void Resolve(IEntity source) {
        Creature c = source as Creature;

        if (attack_buff_amount != 0) c.attack.ApplyBuff(attack_buff_amount);
        if (health_buff_amount != 0) c.health.ApplyBuff(health_buff_amount);
    }
}
