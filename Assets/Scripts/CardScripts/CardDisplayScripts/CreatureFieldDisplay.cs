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

    HashSet<Modifiers> current_mods;

    protected override void Awake() {
        current_mods = new HashSet<Modifiers>();
        card = creature = GetComponent<Creature>();
    }

    public void Update() {
        UpdateDisplay();
    }

    public override void UpdateDisplay() {
        border.color = creature.can_attack && creature.controller == GameManager.current_player ? Color.yellow : Color.black;
        attack.text = "" + creature.attack;
        health.text = "" + creature.current_health;
        UpdateModDisplay();
    }

    void UpdateModDisplay() {
        foreach (Modifiers mod in System.Enum.GetValues(typeof(Modifiers))) {
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

    void ClearMod(Modifiers mod) {
        current_mods.Remove(mod);
        SetMod(mod, false);
    }

    void DisplayMod(Modifiers mod) {
        current_mods.Add(mod);
        SetMod(mod, true);
    }

    void SetMod(Modifiers mod, bool enabled) {
        switch (mod) {
            case Modifiers.charge:
                charge.SetActive(enabled);
                break;
            case Modifiers.divine_shield:
                divine_shield.SetActive(enabled);
                break;
            case Modifiers.immune:
                immune.SetActive(enabled);
                break;
            case Modifiers.lifesteal:
                lifesteal.SetActive(enabled);
                break;
            case Modifiers.mega_windfury:
                mega_windfury.SetActive(enabled);
                break;
            case Modifiers.poisonous:
                poisonous.SetActive(enabled);
                break;
            case Modifiers.rush:
                rush.SetActive(enabled);
                break;
            case Modifiers.stealth:
                stealth.SetActive(enabled);
                break;
            case Modifiers.taunt:
                taunt.SetActive(enabled);
                break;
            case Modifiers.windfury:
                windfury.SetActive(enabled);
                break;
        }
    }
}
