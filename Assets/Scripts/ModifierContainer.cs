using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Modifier { lifesteal = 1, charge, rush, divine_shield, immune, windfury, mega_windfury, poisonous, stealth, taunt, battlecry, overload }

public class ModifierContainer : MonoBehaviour {

    [SerializeField] protected List<Modifier> base_mods;
    protected List<Modifier> buffed_mods;

    [SerializeField] protected int _overload_cost;

    public int overload_cost {
        get { return _overload_cost; }
    }

    public bool HasMod(Modifier mod) {
        return base_mods.Contains(mod) || buffed_mods.Contains(mod);
    }
    public void AddMod(Modifier mod) {
        buffed_mods.Add(mod);
    }
    public void RemoveMod(Modifier mod) {
        if (mod == Modifier.divine_shield || mod == Modifier.stealth) {
            buffed_mods.RemoveAll(a => a == mod);
            base_mods.RemoveAll(a => a == mod);
        }
        buffed_mods.Remove(mod);
    }
    public void RemoveAllMods() {
        buffed_mods.Clear();
        base_mods.Clear();
    }

    protected virtual void OnAwake() {
    }

    private void Awake() {
        buffed_mods = new List<Modifier>();
        OnAwake();
    }
}
