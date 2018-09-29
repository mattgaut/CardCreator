using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UICardDisplay : MonoBehaviour {


    [SerializeField] protected Text mana_cost, card_name, description;
    [SerializeField] protected Transform face, creature_type_block;
    [SerializeField] protected Image art;

    [SerializeField] protected Text attack, health, creature_type;

    [SerializeField] Card card;

    [SerializeField] float scale;

    RectTransform root_transform, rect_transform;

    private void Awake() {
        if (card != null) {
            SetCard(card);
        }

        root_transform = transform.parent.root.GetComponent<RectTransform>();
        rect_transform = GetComponent<RectTransform>();

    }

    private void Update() {
        if (card != null) {
            SetCard(card);
        }
        float ratio = (rect_transform.rect.height / root_transform.rect.height);
        transform.localScale = Vector3.one * scale / ratio;
    }

    public virtual void SetCard(Card card) {
        this.card = card;

        SetDisplay();

        creature_type_block.gameObject.SetActive(false);
        if (card.type == CardType.Creature) {
            SetCreature(card as Creature);
        }
        if (card.type == CardType.Weapon) {
            SetWeapon(card as Weapon);
        }
    }
    void SetWeapon(Weapon card) {
        attack.text = card.attack + "";
        health.text = card.durability + "";        
    }


    void SetCreature(Creature card) {
        attack.text = card.attack.value + "";
        health.text = card.health.value + "";

        if (card.creature_type != Creature.CreatureType.none) {
            creature_type_block.gameObject.SetActive(true);
        }
        creature_type.text = CreatureTypeToString(card.creature_type);
    }

    void SetDisplay() {
        art.sprite = card.art;
        mana_cost.text = "" + card.mana_cost.value;
        card_name.text = card.card_name;
        description.text = card.card_text;
    }

    string CreatureTypeToString(Creature.CreatureType creature_type) {
        switch (creature_type) {
            case Creature.CreatureType.none:
                return "";
            case Creature.CreatureType.mech:
                return "Mech";
            case Creature.CreatureType.dragon:
                return "Dragon";
            case Creature.CreatureType.beast:
                return "Beast";
            case Creature.CreatureType.murloc:
                return "Murloc";
            case Creature.CreatureType.elemental:
                return "Elemental";
            case Creature.CreatureType.totem:
                return "Totem";
            default:
                return "";
        }
    }
}
