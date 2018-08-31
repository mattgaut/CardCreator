using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveTimedBuffTarget : TargetedEffect, ITimedEffect {

    [SerializeField] TimePoint _time_point;
    [SerializeField] int attack_buff;
    [SerializeField] int health_buff;


    public TimePoint time_point {
        get { return _time_point; }
    }

    public void EndEffect(Card source, IEntity target = null) {
        if (target == null) {
            return;
        }
        Creature creature = (target as Creature);

        if (creature != null) {
            creature.attack.RemoveBuff(attack_buff);
            creature.health.RemoveBuff(health_buff);
        }
    }

    protected override void Resolve(Card source, IEntity target) {
        Creature creature = (target as Creature);

        if (creature != null) {
            creature.attack.ApplyBuff(attack_buff);
            creature.health.ApplyBuff(health_buff);

            GameStateManager.instance.TrackTimedEffect(this, source, target);
        }
    }
}
