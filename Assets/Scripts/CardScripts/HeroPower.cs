using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPower : MonoBehaviour, IEntity, IStackEffect, ITargets {

    [SerializeField] Player _controller;

    [SerializeField] Stat _mana_cost;

    [SerializeField] List<UntargetedEffect> untargeted_effects;
    [SerializeField] List<TargetedEffect> targeted_effects;

    [SerializeField] ModifierContainer mods;

    [SerializeField] TriggeredAbility triggered_ability;

    [SerializeField] CompareEntity _targeting_comparer;

    [SerializeField] string _description, _hero_power_name;
    [SerializeField] Sprite _art;

    public Player controller { get { return _controller; } }

    public Stat mana_cost { get { return _mana_cost; } }

    public string hero_power_name { get { return _hero_power_name; } }
    public string description { get { return _description; } }

    public Sprite art { get { return _art; } }

    public EntityType entity_type {
        get { return EntityType.hero_power; }
    }

    public HeroPowerTriggeredAbility trigger { get; private set; }

    public bool has_trigger { get { return trigger != null; } }

    public bool has_targeted_effects { get { return targeted_effects.Count > 0; } }

    public bool is_useable { get { return untargeted_effects.Count > 0 || targeted_effects.Count > 0; } }

    public CompareEntity targeting_comparer {
        get { return _targeting_comparer; }
    }

    public bool CanBeTargeted(IEntity source) {
        return true;
    }

    public bool CanTarget(IEntity target) {
        return targeting_comparer.CompareTo(target, this);
    }

    public bool SetTarget(IEntity target) {
        if (!targeting_comparer.CompareTo(target, this)) return false;

        foreach (TargetedEffect effect in targeted_effects) {
            effect.SetTarget(target);
        }
        return true;
    }

    public int DealDamage(IDamageable target, int damage) {
        int damage_dealt = target.TakeDamage(this, damage);
        if (mods.HasMod(Modifier.lifesteal)) {
            controller.Heal(this, damage_dealt);
        }
        return damage_dealt;
    }

    public virtual void Resolve() {
        for (int i = 0; i < untargeted_effects.Count; i++) {
            untargeted_effects[i].Resolve(this);
        }
        for (int i = 0; i < targeted_effects.Count; i++) {
            if (targeted_effects[i].has_target) targeted_effects[i].Resolve(this);
        }
    }

    void Awake() {
        if (triggered_ability != null) {
            trigger = new HeroPowerTriggeredAbility(this, triggered_ability);
        }
    }
}

public class HeroPowerTriggeredAbility : ITriggeredAbility {
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

    public HeroPowerTriggeredAbility(HeroPower power, TriggeredAbility to_wrap) {
        this.to_wrap = to_wrap;
        this.to_wrap.SetSource(power);
    }

    public bool ActiveInZone(Zone z) {
        return true;
    }

    public void Resolve() {
        to_wrap.Resolve();
    }

    public bool TriggersFrom(TriggerInfo info) {
        return to_wrap.TriggersFrom(info);
    }
}
