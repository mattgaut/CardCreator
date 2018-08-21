using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Creature))]
public class CreatureModifiers : ModifierContainer {

    [SerializeField] Battlecry _battlecry_info;
    public Battlecry battlecry_info {
        get { return _battlecry_info; }
    }

    protected override void OnAwake() {
        if (battlecry_info != null) {
            battlecry_info.SetSource(GetComponent<Creature>());
        }
    }
}

[System.Serializable]
public class Battlecry : IStackEffect, ITargets {
    [SerializeField] List<UntargetedEffect> untargeted_effects;
    [SerializeField] List<TargetedEffect> targeted_effects;
    [SerializeField] CompareEntity _targeting_comparer;
    Card source;

    public CompareEntity targeting_comparer {
        get { return _targeting_comparer; }
    }
    public bool needs_target {
        get { return NeedsTarget(); }
    }

    bool NeedsTarget() {
        foreach (TargetedEffect te in targeted_effects) {
            if (!te.has_target) {
                return true;
            }
        }
        return false;
    }

    public void SetSource(Card source) {
        this.source = source;
    }

    public bool CanTarget(IEntity target) {
        return targeting_comparer.CompareTo(target, source);
    }
    public bool SetTarget(IEntity target) {
        if (targeting_comparer.CompareTo(target, source)) {
            foreach (TargetedEffect te in targeted_effects) {
                te.SetTarget(target);
            }
            return true;
        }
        return false;
    }

    public void Resolve() {
        for (int i = 0; i < targeted_effects.Count; i++) {
            if (targeted_effects[i].has_target) targeted_effects[i].Resolve(source);
        }
        for (int i = 0; i < untargeted_effects.Count; i++) {
            untargeted_effects[i].Resolve(source);
        }
    }
}
