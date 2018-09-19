using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityHolder))]
public abstract class Card : MonoBehaviour, IStackEffect, ICard, IDamages {

    public enum Class { neutral, druid, hunter, mage, palladin, priest, rouge, shaman, warlock, warrior }
    public enum Rarity { basic, common, rare, epic, legendary }

    [SerializeField] Stat _mana_cost;
    [SerializeField] string _card_name, _card_text;
    [SerializeField] int _id;
    [SerializeField] Class _card_class;
    [SerializeField] Sprite _art;
    [SerializeField] Rarity _rarity;
    [SerializeField] ModifierContainer _mods;

    [SerializeField] CastingRestrictions restrictions;

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
    public int id {
        get { return _id; }
    }
    public Sprite art {
        get { return _art; }
    }
    public Rarity rarity {
        get { return _rarity; }
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

    public bool CanPlay() {
        return restrictions.CanPlay(this);
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

    public void SetID(int id) {
        _id = id;
    }

    protected virtual void Awake() {
        abilities = GetComponent<AbilityHolder>();        
    }
}

public enum CardType { Spell = 1, Creature = 2, Weapon = 4 }

[Serializable]
public class CastingRestrictions {
    [SerializeField] bool require_enemy_minions;
    [SerializeField] int enemy_minions_required;

    [SerializeField] bool require_equiped_weapon;

    public bool CanPlay(Card to_cast) {
        if (require_enemy_minions) {
            int count = 0;
            foreach (Player player in GameManager.players) {
                if (player == to_cast.controller) {
                    continue;
                }
                foreach (Card c in player.field.cards) {
                    count++;
                }
            }
            if (count < enemy_minions_required) {
                return false;
            }
        }
        if (require_equiped_weapon && to_cast.controller.weapon == null) {
            return false;
        }
        return true;
    }
}