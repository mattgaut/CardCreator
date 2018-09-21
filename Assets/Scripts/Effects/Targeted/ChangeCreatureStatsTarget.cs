using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCreatureStatsTarget : TargetedEffect {

    [SerializeField] bool change_attack;
    [SerializeField] int new_attack;
    [SerializeField] bool change_health;
    [SerializeField] int new_health;

    protected override void Resolve(IEntity source, IEntity target) {
        Creature creature = target as Creature;
        if (creature == null) {
            return;
        }

        if (change_attack) creature.attack.AddSetBuff(new StatBuff(source, StatBuff.Type.basic, new_attack));
        if (change_health) creature.attack.AddSetBuff(new StatBuff(source, StatBuff.Type.basic, new_health));
    }
}
