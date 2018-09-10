using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TriggeredAbility : MonoBehaviour, ITriggeredAbility {

    [SerializeField] bool _on_their_turn, _on_your_turn;
    [SerializeField] List<UntargetedEffect> effects;
    [SerializeField] List<Zone> active_zones;
    [SerializeField] bool _is_global;

    public abstract TriggerType type { get; }

    public bool is_global { get { return _is_global; } }
    public bool is_local { get { return !_is_global; } }
    public bool on_their_turn { get { return _on_their_turn; } }
    public bool on_your_turn { get { return _on_your_turn; } }

    public IEntity source { get; protected set; }

    public abstract bool TriggersFrom(TriggerInfo info);

    public void Resolve() {
        for (int i = 0; i < effects.Count; i++) {
            effects[i].Resolve(source);
        }
    }

    public void SetSource(IEntity source) {
        this.source = source;
    }

    public bool ActiveInZone(Zone z) {
        return active_zones.Contains(z);
    }

    protected bool CheckTrigger(TriggerInfo info) {
        return TriggersFrom(info);
    }
}

public abstract class TriggerInfo {
    public abstract TriggerType type { get; }
}

public enum TriggerType { enter_battlefield, before_spell_resolves, after_spell_resolves, after_attack, before_attack, creature_killed, damage_taken }
