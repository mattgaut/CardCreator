using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterDamageDealtTrigger : TriggeredAbility {

    [SerializeField] int damage_needed_to_trigger;
    [SerializeField] bool creatures, players;

    [SerializeField] List<TargetedEffect> effects_targeting_damaged;

    public override TriggerType type {
        get {
            return TriggerType.damage_dealt;
        }
    }

    public override bool TriggersFrom(TriggerInfo info) {

        if (info.type != type) {
            return false;
        }

        AfterDamageDealtTriggerInfo damage_info = info as AfterDamageDealtTriggerInfo;
        if (damage_info == null) {
            return false;
        }

        if (damage_info.damager.entity_type == EntityType.player && !players) {
            return false;
        }

        if (damage_info.damager as Creature != null && !creatures) {
            return false;
        }

        return damage_info.damage >= damage_needed_to_trigger;
    }

    public override void Resolve(TriggerInfo info) {
        base.Resolve(info);

        if (info.type == TriggerType.damage_dealt) {
            AfterDamageDealtTriggerInfo damage_info = info as AfterDamageDealtTriggerInfo;

            if (damage_info.damaged != null) {
                foreach (TargetedEffect te in effects_targeting_damaged) {
                    te.SetTarget(damage_info.damaged);
                    te.Resolve(source);
                }
            }
        }
    }
}

public class AfterDamageDealtTriggerInfo : TriggerInfo {
    public override TriggerType type {
        get {
            return TriggerType.damage_dealt;
        }
    }

    public IEntity damager { get; private set; }
    public ICombatant damaged { get; private set; }
    public int damage { get; private set; }

    public AfterDamageDealtTriggerInfo(IEntity damager, ICombatant damaged, int damage) {
        this.damage = damage;
        this.damaged = damaged;
        this.damager = damager;
    }
}
