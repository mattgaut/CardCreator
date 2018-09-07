using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterAttackTriggeredAbility : TriggeredAbility {

    [SerializeField] int damage_needed;
    [SerializeField] bool need_to_kill_attacked;

    public override TriggerType type {
        get {
            return TriggerType.after_attack;
        }
    }

    public override bool TriggersFrom(TriggerInfo info) {
        if (info.type != TriggerType.after_attack) {
            return false;
        }
        AfterAttackTriggerInfo attack_info = (AfterAttackTriggerInfo)info;

        if (need_to_kill_attacked && !attack_info.attacked.dead) {
            return false;
        }
        return damage_needed <= attack_info.damage_dealt;
    }
}

public class AfterAttackTriggerInfo : TriggerInfo {
    public override TriggerType type {
        get {
            return TriggerType.after_attack;
        }
    }

    public int damage_dealt { get; private set; }
    public ICombatant attacked { get; private set; }
    public ICombatant attacker { get; private set; }

    public AfterAttackTriggerInfo(ICombatant attacker, ICombatant attacked, int damage_dealt) {
        this.attacked = attacked;
        this.attacker = attacker;
        this.damage_dealt = damage_dealt;
    }
}
