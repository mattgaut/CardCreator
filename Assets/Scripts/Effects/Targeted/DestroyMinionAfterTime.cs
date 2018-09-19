using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMinionAfterTime : TargetedEffect, ITimedEffect {
    [SerializeField] TimePoint _time_point;

    public TimePoint time_point {
        get { return _time_point; }
    }

    public void EndEffect(IEntity source, IEntity target = null) {
        if (target as Creature == null) {
            return;
        }
        (target as Creature).Destroy();
    }

    protected override void Resolve(IEntity source, IEntity target) {
        if (target as Creature == null) {
            return;
        }
        GameStateManager.instance.TrackTimedEffect(this, source, target);
    }
}
