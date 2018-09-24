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
        secret_trigger = new SecretTriggeredAbility(this, trigger);
        trigger.SetSource(this);
    }
}

// Wrapper class for secret's triggered ability that ensures the secret only resolves once
public class SecretTriggeredAbility : ITriggeredAbility {

    Secret secret;
    TriggeredAbility to_wrap;

    public TriggerType type {
        get {
            return to_wrap.type;
        }
    }

    public bool is_global { get { return to_wrap.is_global; } }

    public bool is_local { get { return to_wrap.is_local; } }

    public bool on_their_turn { get { return to_wrap.on_their_turn; } }

    public bool on_your_turn { get { return to_wrap.on_your_turn; } }

    public IEntity source { get { return to_wrap.source; } }

    public SecretTriggeredAbility(Secret secret, TriggeredAbility to_wrap) {
        this.secret = secret;
        this.to_wrap = to_wrap;
        this.to_wrap.SetSource(secret);
    }

    public bool TriggersFrom(TriggerInfo info) {
        return to_wrap.TriggersFrom(info);
    }

    public bool ActiveInZone(Zone z) {
        return to_wrap.ActiveInZone(z);
    }

    public void Resolve(TriggerInfo info) {
        if (secret.container.zone == Zone.secrets) {
            to_wrap.Resolve(info);

            GameStateManager.instance.TriggerSecret(secret);
        }
    }
}