using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : Card, ITargets {
    [SerializeField] List<UntargetedEffect> spell_effects;
    [SerializeField] List<TargetedEffect> targeted_effects;
    [SerializeField] CompareEntity _targeting_comparer;

    public override CardType type {
        get {
            return CardType.Spell;
        }
    }
    public bool needs_target {
        get { return NeedsTarget(); }
    }

    public CompareEntity targeting_comparer {
        get {
            return _targeting_comparer;
        }
    }

    public bool CanTarget(IEntity target) {
        return targeting_comparer.CompareTo(target, this);
    }
    public bool SetTarget(IEntity target) {
        if (targeting_comparer.CompareTo(target, this)) {
            foreach (TargetedEffect te in targeted_effects) {
                te.SetTarget(target);
            }
            if (mods.HasMod(Modifier.combo) && mods.combo_info.needs_target) {
                mods.combo_info.SetTargetingComparer(targeting_comparer);
                mods.combo_info.SetTarget(target);
            }
            return true;
        }
        return false;
    }

    public override void Resolve() {
        if (mods.HasMod(Modifier.combo) && controller.combo_active) {
            mods.combo_info.Resolve();
            if (mods.combo_info.replaces_other_effects) {
                return;
            }
        }
        for (int i = 0; i < targeted_effects.Count; i++) {
            if (targeted_effects[i].has_target) targeted_effects[i].Resolve(this);
        }
        for (int i = 0; i < spell_effects.Count; i++) {
            spell_effects[i].Resolve(this);
        }
    }

    public override int DealDamage(IDamageable target, int damage) {
        return base.DealDamage(target, damage + controller.TotalSpellPower());
    }

    bool NeedsTarget() {
        foreach (TargetedEffect te in targeted_effects) {
            if (!te.has_target) {
                return true;
            }
        }
        if (mods.HasMod(Modifier.combo) && controller.combo_active) {
            if (mods.combo_info.needs_target) {
                return true;
            }
        }
        return false;
    }
}
