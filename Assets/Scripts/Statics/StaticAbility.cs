using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StaticAbility : MonoBehaviour {

    [SerializeField] List<Zone> active_zones;

    [SerializeField] List<Zone> affected_zones;
    [SerializeField] bool affects_players;

    public Card source { get; private set; }

    public virtual bool AppliesTo(IEntity entity) {
        return true;
    }

    public abstract void Apply(IEntity apply_to);
    public abstract void Remove(IEntity remove_from);
}
