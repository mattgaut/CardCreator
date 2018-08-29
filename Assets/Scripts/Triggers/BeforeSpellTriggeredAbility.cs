using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BeforeSpellTriggeredAbility : TriggeredAbility {

    [SerializeField] CompareEntity compare;

    public override TriggerType type {
        get {
            return TriggerType.before_spell_resolves;
        }
    }

    public override bool TriggersFrom(TriggerInfo info) {
        if (info.type != TriggerType.before_spell_resolves) {
            return false;
        }
        BeforeSpellTriggerInfo before_spell_info = (BeforeSpellTriggerInfo)info;

        return compare.CompareTo(before_spell_info.spell_cast, source);
    }
}

public class BeforeSpellTriggerInfo : TriggerInfo {
    public override TriggerType type {
        get {
            return TriggerType.before_spell_resolves;
        }
    }

    public Spell spell_cast { get; private set; }

    public BeforeSpellTriggerInfo(Spell cast) {
        spell_cast = cast;
    }
}