using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponDisplay : FullCardDisplay {

    [SerializeField] Text attack, health;
    Weapon weapon;

    protected override void Awake() {
        base.Awake();
    }

    public override void UpdateDisplay() {
        base.UpdateDisplay();
        attack.text = "" + weapon.attack.value;
        health.text = "" + weapon.durability.value;
    }

    public override void SetCard(Card card) {
        base.SetCard(card);
        weapon = card as Weapon;
    }
}
