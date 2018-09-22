using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheneverMinionPlayedTrigger : TriggeredAbility {

    [SerializeField] CompareEntity compare;

    public override TriggerType type {
        get {
            return TriggerType.after_minion_played;
        }
    }

    public override bool TriggersFrom(TriggerInfo info) {
        WheneverCreatureMinionPlayedInfo etb_info = info as WheneverCreatureMinionPlayedInfo;
        if (etb_info != null) {
            return compare.CompareTo(etb_info.entered, source);
        }
        return false;
    }
}

public class WheneverCreatureMinionPlayedInfo : TriggerInfo {
    public override TriggerType type {
        get { return TriggerType.after_minion_played; }
    }

    public Creature entered { get; private set; }
    public WheneverCreatureMinionPlayedInfo(Creature entered) {
        this.entered = entered;
    }
}
