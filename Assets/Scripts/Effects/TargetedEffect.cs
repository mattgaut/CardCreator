using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetedEffect : Effect {

    private IEntity target;

    public bool has_target {
        get  { return target != null; }
    }
    public void SetTarget(IEntity new_target) {
        target = new_target;
    }
    public override void Resolve(IEntity source) {
        if (has_target) {
            Resolve(source, target);
        }
        target = null;
    }
    protected abstract void Resolve(IEntity source, IEntity target);
}
