using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Secret : Card {

    [SerializeField] TriggeredAbility trigger;

    public SecretTriggeredAbility secret_trigger {
        get; private set;
    }

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

    protected override void Awake() {
        base.Awake();
        secret_trigger = new SecretTriggeredAbility(this);
    }
}

// Wrapper class for secret's triggered ability that ensures the secret only resolves once
public class SecretTriggeredAbility : ITriggeredAbility {

    Secret secret;

    public TriggerType type {
        get {
            return secret.secret_trigger.type;
        }
    }

    public bool is_global { get { return secret.secret_trigger.is_global; } }

    public bool is_local { get { return secret.secret_trigger.is_local; } }

    public bool on_their_turn { get { return secret.secret_trigger.on_their_turn; } }

    public bool on_your_turn { get { return secret.secret_trigger.on_your_turn; } }

    public Card source { get { return secret.secret_trigger.source; } }

    public SecretTriggeredAbility(Secret secret) {
        this.secret = secret;
    }

    public bool CheckTrigger(TriggerInfo info) {
        return secret.secret_trigger.CheckTrigger(info);
    }

    public bool InZone(Zone z) {
        return secret.secret_trigger.InZone(z);
    }

    public void Resolve() {
        if (secret.container.zone == Zone.secrets) {
            secret.secret_trigger.Resolve();


        }
    }

    public bool TriggersFrom(TriggerInfo info) {
        return secret.secret_trigger.TriggersFrom(info);
    }
}