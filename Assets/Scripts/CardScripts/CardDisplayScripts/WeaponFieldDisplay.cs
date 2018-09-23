using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponFieldDisplay : CardDisplay {
    Weapon weapon;

    [SerializeField] Text attack, durability;

    public override void UpdateDisplay() {
        attack.text = "" + weapon.attack.value;
        durability.text = "" + weapon.durability.current_value;
    }

    public override void SetCard(Card card) {
        base.SetCard(card);
        weapon = card as Weapon;
    }

    protected override void Awake() {
        base.Awake();
        weapon = GetComponent<Weapon>();
    }
}
