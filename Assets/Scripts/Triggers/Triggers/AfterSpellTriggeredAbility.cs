using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterSpellTriggeredAbility : TriggeredAbility {

    [SerializeField] CompareEntity compare;

    public override TriggerType type {
        get {
            return TriggerType.after_spell_resolves;
        }
    }

    public override bool TriggersFrom(TriggerInfo info) {
        if (info.type != TriggerType.after_spell_resolves) {
            return false;
        }
        AfterSpellTriggerInfo after_spell_info = (AfterSpellTriggerInfo)info;

        return compare.CompareTo(after_spell_info.spell_cast, source);
    }
}

public class AfterSpellTriggerInfo : TriggerInfo {
    public override TriggerType type {
        get {
            return TriggerType.after_spell_resolves;
        }
    }

    public Card spell_cast { get; private set; }
    
    public AfterSpellTriggerInfo(Spell cast) {
        spell_cast = cast;
    }

    public AfterSpellTriggerInfo(Secret cast) {
        spell_cast = cast;
    }

}
