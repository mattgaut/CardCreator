using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ETBTriggeredAbility : TriggeredAbility {

    [SerializeField] CompareEntity compare;

    public override TriggerType type {
        get {
            return TriggerType.enter_battlefield;
        }
    }

    public override bool TriggersFrom(TriggerInfo info) {
        ETBTriggerInfo etb_info = info as ETBTriggerInfo;
        if (etb_info != null) {
            return compare.CompareTo(etb_info.entered, source);
        }
        return false;
    }
}

public class ETBTriggerInfo : TriggerInfo {
    public override TriggerType type {
        get { return TriggerType.enter_battlefield; }
    }

    public Creature entered { get; private set; }
    public ETBTriggerInfo(Creature entered) {
        this.entered = entered;
    }
}
