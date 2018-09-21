using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GivePlayerTimedAttackBuff : UntargetedEffect, ITimedEffect {
    [SerializeField] TimePoint _time_point;
    [SerializeField] int attack_buff;

    public TimePoint time_point {
        get { return _time_point; }
    }

    public void EndEffect(IEntity source, IEntity target = null) {
        if (target != null) {
            return;
        }
        Player player = source.controller;

        if (player != null) {
            player.attack.RemoveBuff(new StatBuff(source, StatBuff.Type.timed, attack_buff));
        }
    }

    public override void Resolve(IEntity source) {
        Player player = source.controller;

        if (player != null) {
            player.attack.ApplyBuff(new StatBuff(source, StatBuff.Type.timed, attack_buff));
            
            GameStateManager.instance.TrackTimedEffect(this, source);
        }
    }
}
