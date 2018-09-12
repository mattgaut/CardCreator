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

    public Player controller { get { return _controller; } }

    public Stat mana_cost { get { return _mana_cost; } }

    public EntityType entity_type {
        get { return EntityType.hero_power; }
    }

    public HeroPowerTriggeredAbility trigger { get; private set; }

    public bool has_trigger { get { return trigger != null; } }

    public CompareEntity targeting_comparer {
        get { return _targeting_comparer; }
    }

    public bool CanBeTargeted(IEntity source) {
        return true;
    }

    public bool CanTarget(IEntity target) {
        return targeting_comparer.CompareTo(target, this);
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
    }

    void Awake() {
        trigger = new HeroPowerTriggeredAbility(this, triggered_ability);
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
