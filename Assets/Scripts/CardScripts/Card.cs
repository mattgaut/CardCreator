using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityHolder))]
public abstract class Card : MonoBehaviour, IStackEffect, ICard, IDamages {

    public enum Class { neutral, druid, hunter, mage, palladin, priest, rouge, shaman, warlock, warrior }

    [SerializeField] Stat _mana_cost;
    [SerializeField] string _card_name, _card_text;
    [SerializeField] Class _card_class;
    [SerializeField] Sprite _art;
    [SerializeField] ModifierContainer _mods;

    public Stat mana_cost {
        get { return _mana_cost; }
    }
    public string card_name {
        get { return _card_name; }
    }
    public Class card_class {
        get { return _card_class; }
    }
    public string card_text {
        get { return _card_text; }
    }
    public Sprite art {
        get { return _art; }
    }

    public ModifierContainer mods {
        get { return _mods; }
    }

    public EntityType entity_type { get { return EntityType.card; } }
    public abstract CardType type {
        get;
    }
    public AbilityHolder abilities {
        get; private set;
    }


    public CardContainer container {
        get; private set;
    }

    public Player controller {
        get { return container.controller; }
    }

    public Card card {
        get { return this; }
    }

    protected void Awake() {
        abilities = GetComponent<AbilityHolder>();
    }

    public abstract void Resolve();

    public void SetContainer(CardContainer container) {
        this.container = container;
    }

    public virtual bool CanBeTargeted(IEntity source) {
        return card.container.visible && !card.container.hidden_to_opponent;
    }

    public virtual int DealDamage(IDamageable target, int damage) {
        int damage_dealt = target.TakeDamage(this, damage);
        if (mods.HasMod(Modifier.lifesteal)) {
            controller.Heal(controller, damage_dealt);
        }
        return damage_dealt;
    }
}

public enum CardType { Spell = 1, Creature }
