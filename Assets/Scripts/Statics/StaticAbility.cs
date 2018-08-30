﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StaticAbility : MonoBehaviour {

    [SerializeField] List<Zone> active_zones;

    [SerializeField] List<Zone> affected_zones;
    [SerializeField] bool affects_players;

    HashSet<IEntity> entities_applied_to;

    public Card source { get; private set; }

    public List<Zone> GetActiveZones() {
        return new List<Zone>(active_zones);
    }

    public List<Zone> GetAffectedZones() {
        return new List<Zone>(affected_zones);
    }

    public virtual bool AppliesTo(IEntity entity) {
        return true;
    }

    public bool IsAppliedTo(IEntity entity) {
        return entities_applied_to.Contains(entity);
    }

    public void Apply(IEntity apply_to) {
        if (!entities_applied_to.Contains(apply_to)) {
            ApplyEffects(apply_to);
        }
    }
    public void Remove(IEntity remove_from) {
        if (entities_applied_to.Contains(remove_from)) {
            RemoveEffects(remove_from);
        }
    }

    protected abstract void ApplyEffects(IEntity apply_to);
    protected abstract void RemoveEffects(IEntity remove_from);

    private void Awake() {
        entities_applied_to = new HashSet<IEntity>();    
    }
}