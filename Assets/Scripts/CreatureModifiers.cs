using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Modifiers { lifesteal = 1, charge, rush, divine_shield, immune, windfury, mega_windfury, poisonous, stealth, taunt, battlecry, overload }

[RequireComponent(typeof(Creature))]
public class CreatureModifiers : MonoBehaviour {

    [SerializeField] List<Modifiers> base_mods;
    List<Modifiers> buffed_mods;

    [SerializeField] Battlecry _battlecry_info;
    [SerializeField] int _overload_cost;

    public Battlecry battlecry_info {
        get { return _battlecry_info; }
    }
    public int overload_cost {
        get { return _overload_cost; }
    }

    public void Awake() {
        buffed_mods = new List<Modifiers>();
        if (battlecry_info != null) {
            battlecry_info.SetSource(GetComponent<Creature>());
        }
    }

    public bool HasMod(Modifiers mod) {
        return base_mods.Contains(mod) || buffed_mods.Contains(mod);
    }
    public void AddMod(Modifiers mod) {
        buffed_mods.Add(mod);
    }
    public void RemoveMod(Modifiers mod) {
        if (mod == Modifiers.divine_shield || mod == Modifiers.stealth) {
            buffed_mods.RemoveAll(a => a == mod);
            base_mods.RemoveAll(a => a == mod);
        }
        buffed_mods.Remove(mod);
    }
    public void RemoveAllMods() {
        buffed_mods.Clear();
        base_mods.Clear();
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
