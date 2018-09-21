using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StaticAbility : MonoBehaviour {

    [SerializeField] List<Zone> active_zones;

    [SerializeField] List<Zone> affected_zones;
    [SerializeField] bool affects_players;

    [SerializeField] bool friendly, enemy;

    HashSet<IEntity> entities_applied_to;
    HashSet<IEntity> possible_entities;

    public Card source { get; private set; }

    public List<Zone> GetActiveZones() {
        return new List<Zone>(active_zones);
    }

    public List<Zone> GetAffectedZones() {
        return new List<Zone>(affected_zones);
    }

    public bool InZone(Zone z) {
        return active_zones.Contains(z);
    }

    public virtual bool AppliesTo(IEntity entity) {
        if (!affects_players && entity.entity_type == EntityType.player) {
            return false;
        }
        if (!friendly && entity.controller == source.controller) {
            return false;
        }
        if (!enemy && entity.controller != source.controller) {
            return false;
        }
        return true;
    }

    public bool IsAppliedTo(IEntity entity) {
        return entities_applied_to.Contains(entity);
    }

    public void AddPossible(IEntity entity) {
        if (!possible_entities.Contains(entity)) {
            possible_entities.Add(entity);
            Apply(entity);
        }
    }
    public void RemovePossible(IEntity entity) {
        if (possible_entities.Contains(entity)) {
            possible_entities.Remove(entity);
            Remove(entity);
        }
    }

    public void UpdateEffects() {
        foreach (IEntity possible_entity in possible_entities) {
            if (AppliesTo(possible_entity)) {
                Apply(possible_entity);
            } else {
                Remove(possible_entity);
            }
        }
    }

    public void RemoveAll() {
        List<IEntity> to_remove = new List<IEntity>(possible_entities);
        foreach (IEntity entity in to_remove) {
            RemovePossible(entity);
        }
    }

    public void SetSource(Card source) {
        this.source = source;
    }

    protected abstract void ApplyEffects(IEntity apply_to);
    protected abstract void RemoveEffects(IEntity remove_from);

    void Apply(IEntity apply_to) {
        if (!entities_applied_to.Contains(apply_to)) {
            entities_applied_to.Add(apply_to);
            ApplyEffects(apply_to);
        }
    }
    void Remove(IEntity remove_from) {
        if (entities_applied_to.Contains(remove_from)) {
            entities_applied_to.Remove(remove_from);
            RemoveEffects(remove_from);
        }
    }


    private void Awake() {
        entities_applied_to = new HashSet<IEntity>();
        possible_entities = new HashSet<IEntity>();
    }
}
