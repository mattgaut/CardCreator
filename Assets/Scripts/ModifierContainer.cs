using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Modifier { lifesteal = 1, charge, rush, divine_shield, immune, windfury, mega_windfury, poisonous, stealth, taunt, battlecry, overload, spellpower, deathrattle, combo }

[RequireComponent(typeof(Card))]
public class ModifierContainer : MonoBehaviour, ITargets {

    [SerializeField] protected List<Modifier> base_mods;
    protected List<Modifier> unavailable_mods;
    protected List<Modifier> buffed_mods;

    [SerializeField] Battlecry _battlecry_info;
    [SerializeField] Combo _combo_info;
    [SerializeField] Deathrattle _deathrattle_info;
    [SerializeField] protected int _overload_cost;
    [SerializeField] protected int _spellpower_amount;

    [SerializeField] CompareEntity _targeting_comparer;

    Card source;

    public Battlecry battlecry_info {
        get { return _battlecry_info; }
    }
    public Combo combo_info {
        get { return _combo_info; }
    }
    public Deathrattle deathrattle_info {
        get { return _deathrattle_info; }
    }
    public int overload_cost {
        get { return _overload_cost; }
    }
    public int spellpower_amount {
        get { return _spellpower_amount; }
    }

    public CompareEntity targeting_comparer {
        get {
            return _targeting_comparer;
        }
    }

    public bool HasMod(Modifier mod) {
        return base_mods.Contains(mod) || buffed_mods.Contains(mod);
    }
    public void AddMod(Modifier mod) {
        if (unavailable_mods.Contains(mod)) {
            return;
        }
        buffed_mods.Add(mod);
    }
    public void RemoveMod(Modifier mod) {
        if (mod == Modifier.divine_shield || mod == Modifier.stealth) {
            buffed_mods.RemoveAll(a => a == mod);
            base_mods.RemoveAll(a => a == mod);
        }
        buffed_mods.Remove(mod);
    }
    public void RemoveAllMods() {
        buffed_mods.Clear();
        base_mods.Clear();
    }

    public bool NeedsTarget() {
        if (HasMod(Modifier.battlecry) && battlecry_info.needs_target) {
            return true;
        }

        if (HasMod(Modifier.combo) && combo_info.needs_target && source.controller.combo_active) {
            return true;
        }
        return false;
    }

    public bool CanTarget(IEntity target) {
        return targeting_comparer.CompareTo(target, source);
    }

    private void Awake() {
        source = GetComponent<Card>();
        buffed_mods = new List<Modifier>();
        unavailable_mods = new List<Modifier>();
        foreach (Modifier mod in unavailable_mods) {
            RemoveMod(mod);
        }
        if (battlecry_info != null) {
            battlecry_info.SetSource(source);
            battlecry_info.SetTargetingComparer(targeting_comparer);
        }
        if (combo_info != null) {
            combo_info.SetSource(source);
            combo_info.SetTargetingComparer(targeting_comparer);
        }
    }
}

[System.Serializable]
public class Battlecry : IStackEffect, ITargets {
    [SerializeField] List<UntargetedEffect> untargeted_effects;
    [SerializeField] List<TargetedEffect> targeted_effects;
    CompareEntity _targeting_comparer;
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

    public void SetTargetingComparer(CompareEntity compare) {
        _targeting_comparer = compare;
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

[System.Serializable]
public class Deathrattle : IStackEffect {
    [SerializeField] List<UntargetedEffect> untargeted_effects;
    Card source;

    public void SetSource(Card source) {
        this.source = source;
    }

    public void Resolve() {
        for (int i = 0; i < untargeted_effects.Count; i++) {
            untargeted_effects[i].Resolve(source);
        }
    }
}

[System.Serializable]
public class Combo : Battlecry {
    [SerializeField] bool _replaces_other_effects;

    public bool replaces_other_effects {
        get { return _replaces_other_effects; }
    }
}