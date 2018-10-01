using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UICardDisplay : MonoBehaviour {


    [SerializeField] protected Text mana_cost, card_name, description;
    [SerializeField] protected Transform face, creature_type_block;
    [SerializeField] protected Image art;

    [SerializeField] protected Text attack, health, creature_type;

    [SerializeField] Card card;

    [SerializeField] float scale;

    [SerializeField] RectTransform root_transform;
    RectTransform rect_transform;
    Button button;

    private void Awake() {
        if (card != null) {
            SetCard(card);
        }

        rect_transform = GetComponent<RectTransform>();
        button = GetComponent<Button>();    
    }

    private void Update() {
        if (card != null) {
            SetCard(card);
        }
        float scale_to_use = scale;
        float ratio = (rect_transform.rect.height / root_transform.rect.height);
        float width_ratio = (rect_transform.rect.width / root_transform.rect.width);

        if (ratio * 2 < width_ratio * 4) {
            ratio = width_ratio;
            scale_to_use = scale / 2f;
        }
        transform.localScale = Vector3.one * scale_to_use / ratio;
    }

    public void SetOnClick(UnityAction action) {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
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

    public Card GetCard() {
        return card;
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
