using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityHolder))]
public abstract class Card : MonoBehaviour, IStackEffect, ICard {

    public enum Class { neutral, druid, hunter, mage, palladin, priest, rouge, shaman, warlock, warrior }

    [SerializeField] int _mana_cost;
    [SerializeField] string _card_name, _card_text;
    [SerializeField] Class card_class;
    [SerializeField] Sprite _art;
    [SerializeField] ModifierContainer _mods;

    public int mana_cost {
        get { return _mana_cost; }
    }
    public string card_name {
        get { return _card_name; }
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
}

public enum CardType { Spell = 1, Creature }
