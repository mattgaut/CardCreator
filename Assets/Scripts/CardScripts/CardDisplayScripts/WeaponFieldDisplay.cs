using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Weapon))]
public class WeaponFieldDisplay : CardDisplay {
    Weapon weapon;

    [SerializeField] Text attack, durability;

    public override void UpdateDisplay() {
        attack.text = "" + weapon.attack;
        durability.text = "" + weapon.durability.current_value;
    }

    protected override void Awake() {
        base.Awake();
        weapon = GetComponent<Weapon>();
    }
}
