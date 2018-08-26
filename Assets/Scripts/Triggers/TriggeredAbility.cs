using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class TriggeredAbility : IStackEffect {


    protected Card source;

    [SerializeField] List<UntargetedEffect> effects;
    [SerializeField] List<Zone> active_zones;

    public abstract TriggerType type { get; }

    public abstract bool TriggersFrom(TriggerInfo info);

    public bool CheckTrigger(TriggerInfo info) {
        return TriggersFrom(info);
    }

    public void Resolve() {
        for (int i = 0; i < effects.Count; i++) {
            effects[i].Resolve(source);
        }
    }

    public void SetSource(Card source) {
        this.source = source;
    }

    public bool InZone(Zone z) {
        return active_zones.Contains(z);
    }
}

public abstract class TriggerInfo {
    public abstract TriggerType type { get; }
}

public enum TriggerType { enter_battlefield, cast_spell, after_spell_resolves }
