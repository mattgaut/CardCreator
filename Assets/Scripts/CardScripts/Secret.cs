using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Secret : Card {

    [SerializeField] List<UntargetedEffect> on_trigger_effects;
    [SerializeField] TriggeredAbility trigger;

    public override CardType type {
        get {
            return CardType.Spell;
        }
    }

    public override void Resolve() {

    }

    public override int DealDamage(IDamageable target, int damage) {
        return base.DealDamage(target, damage + controller.TotalSpellPower());
    }
}
