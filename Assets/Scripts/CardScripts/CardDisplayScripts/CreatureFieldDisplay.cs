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

    HashSet<CreatureMod> current_mods;

    protected override void Awake() {
        current_mods = new HashSet<CreatureMod>();
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
        foreach (CreatureMod mod in System.Enum.GetValues(typeof(CreatureMod))) {
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

    void ClearMod(CreatureMod mod) {
        current_mods.Remove(mod);
        SetMod(mod, false);
    }

    void DisplayMod(CreatureMod mod) {
        current_mods.Add(mod);
        SetMod(mod, true);
    }

    void SetMod(CreatureMod mod, bool enabled) {
        switch (mod) {
            case CreatureMod.charge:
                charge.SetActive(enabled);
                break;
            case CreatureMod.divine_shield:
                divine_shield.SetActive(enabled);
                break;
            case CreatureMod.immune:
                immune.SetActive(enabled);
                break;
            case CreatureMod.lifesteal:
                lifesteal.SetActive(enabled);
                break;
            case CreatureMod.mega_windfury:
                mega_windfury.SetActive(enabled);
                break;
            case CreatureMod.poisonous:
                poisonous.SetActive(enabled);
                break;
            case CreatureMod.rush:
                rush.SetActive(enabled);
                break;
            case CreatureMod.stealth:
                stealth.SetActive(enabled);
                break;
            case CreatureMod.taunt:
                taunt.SetActive(enabled);
                break;
            case CreatureMod.windfury:
                windfury.SetActive(enabled);
                break;
        }
    }
}
