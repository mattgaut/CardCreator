using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureDisplay : FullCardDisplay {

    [SerializeField] Text attack, health, creature_type;
    Creature creature;

    protected override void Awake() {
        base.Awake();
    }

    public override void UpdateDisplay() {
        base.UpdateDisplay();
        attack.text = "" + creature.attack.value;
        health.text = "" + creature.health.value;
        creature_type.text = CreatureTypeToString(creature.creature_type);
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

    public override void SetCard(Card card) {
        base.SetCard(card);
        creature = card as Creature;
    }
}
