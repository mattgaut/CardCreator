using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfTurnTriggeredAbility : TriggeredAbility {
    public override TriggerType type {
        get {
            return TriggerType.end_of_turn;
        }
    }

    public override bool TriggersFrom(TriggerInfo info) {
        if (info.type != TriggerType.end_of_turn) {
            return false;
        }

        EndOfTurnTriggerInfo eot_info = info as EndOfTurnTriggerInfo;

        if (eot_info == null) {
            return false;
        }

        return true;
    }
}

public class EndOfTurnTriggerInfo : TriggerInfo {
    public override TriggerType type {
        get {
            return TriggerType.end_of_turn;
        }
    }

    public Player player { get; private set; }

    public EndOfTurnTriggerInfo(Player player) {
        this.player = player;
    }
}
