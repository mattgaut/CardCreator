using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Creature))]
public class CreatureFieldDisplay : CardDisplay {

    [SerializeField] Text attack, health;
    [SerializeField] Image border;
    [SerializeField] GameObject taunt, divine_shield, immune, poisonous, charge, rush, stealth, windfury, mega_windfury, lifesteal;
    Creature creature;

    HashSet<Modifier> current_mods;

    protected override void Awake() {
        base.Awake();
        current_mods = new HashSet<Modifier>();
        creature = GetComponent<Creature>();
    }

    public override void UpdateDisplay() {
        if (card.container != null) border.color = creature.can_attack && creature.controller == GameManager.current_player ? Color.yellow : Color.black;
        attack.text = "" + creature.attack.value;
        health.text = "" + creature.current_health;
        UpdateModDisplay();
    }

    void UpdateModDisplay() {
        foreach (Modifier mod in System.Enum.GetValues(typeof(Modifier))) {
            if (creature.mods.HasMod(mod)) {
                if (!current_mods.Contains(mod)) {
                    DisplayMod(mod);
                }
            } else {
                if (current_mods.Contains(mod)) {
                    ClearMod(mod);
                }
            }
        }
    }

    void ClearMod(Modifier mod) {
        current_mods.Remove(mod);
        SetMod(mod, false);
    }

    void DisplayMod(Modifier mod) {
        current_mods.Add(mod);
        SetMod(mod, true);
    }

    void SetMod(Modifier mod, bool enabled) {
        switch (mod) {
            case Modifier.charge:
                charge.SetActive(enabled);
                break;
            case Modifier.divine_shield:
                divine_shield.SetActive(enabled);
                break;
            case Modifier.immune:
                immune.SetActive(enabled);
                break;
            case Modifier.lifesteal:
                lifesteal.SetActive(enabled);
                break;
            case Modifier.mega_windfury:
                mega_windfury.SetActive(enabled);
                break;
            case Modifier.poisonous:
                poisonous.SetActive(enabled);
                break;
            case Modifier.rush:
                rush.SetActive(enabled);
                break;
            case Modifier.stealth:
                stealth.SetActive(enabled);
                break;
            case Modifier.taunt:
                taunt.SetActive(enabled);
                break;
            case Modifier.windfury:
                windfury.SetActive(enabled);
                break;
        }
    }
}
