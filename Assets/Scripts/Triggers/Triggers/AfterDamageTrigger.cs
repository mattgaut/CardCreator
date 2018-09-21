using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterDamageTrigger : TriggeredAbility {

    [SerializeField] int damage_needed_to_trigger;
    [SerializeField] bool creatures, players;

    public override TriggerType type {
        get {
            return TriggerType.damage_taken;
        }
    }

    public override bool TriggersFrom(TriggerInfo info) {
        if (info.type != type) {
            return false;
        }

        AfterDamageTriggerInfo damage_info = info as AfterDamageTriggerInfo;
        if (damage_info == null) {
            return false;
        }

        if (damage_info.damaged.entity_type == EntityType.player && !players) {
            return false;
        }

        if (damage_info.damaged as Creature != null && !creatures) {
            return false;
        }

        return damage_info.damage >= damage_needed_to_trigger;
    }
}

public class AfterDamageTriggerInfo : TriggerInfo {
    public override TriggerType type {
        get {
            return TriggerType.damage_taken;
        }
    }

    public ICombatant damaged { get; private set; }
    public int damage { get; private set; }

    public AfterDamageTriggerInfo(ICombatant damaged, int damage) {
        this.damage = damage;
        this.damaged = damaged;
    }
}
