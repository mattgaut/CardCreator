using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheneverMinionSummonedTrigger : TriggeredAbility {
    public override TriggerType type {
        get {
            return TriggerType.after_creature_summoned;
        }
    }

    [SerializeField] bool match_creature_type;
    [SerializeField] Creature.CreatureType creature_type;

    public override bool TriggersFrom(TriggerInfo info) {
        if (info.type != type) {
            return false;
        }

        WheneverCreatureSummonedInfo new_info = info as WheneverCreatureSummonedInfo;
        if (match_creature_type && creature_type != new_info.creature.creature_type) {
            return false;
        }

        return true;
    }
}

public class WheneverCreatureSummonedInfo : TriggerInfo {
    public override TriggerType type {
        get {
            return TriggerType.after_creature_summoned;
        }
    }

    public Creature creature { get; private set; }

    public WheneverCreatureSummonedInfo(Creature creature) {
        this.creature = creature;
    }
}
