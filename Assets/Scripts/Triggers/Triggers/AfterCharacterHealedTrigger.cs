using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterCharacterHealedTrigger : TriggeredAbility {

    [SerializeField] bool creatures, players;

    public override TriggerType type {
        get {
            return TriggerType.character_healed;
        }
    }

    public override bool TriggersFrom(TriggerInfo info) {
        if (info.type != type) {
            return false;
        }

        AfterCharacterHealedTriggerInfo new_info = info as AfterCharacterHealedTriggerInfo;
        if (new_info == null) {
            return false;
        }

        if (!players && new_info.healed.entity_type == EntityType.player) {
            return false;
        }
        if (!creatures && new_info.healed.entity_type == EntityType.card) {
            return false;
        }

        return true;
    }
}

public class AfterCharacterHealedTriggerInfo : TriggerInfo {
    public override TriggerType type {
        get {
            return TriggerType.character_healed;
        }
    }

    public int healing { get; private set; }
    public ICombatant healed { get; private set; }

    public AfterCharacterHealedTriggerInfo(ICombatant healed, int healing) {
        this.healed = healed;
        this.healing = healing;
    }
}
