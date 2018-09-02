﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Weapon))]
public class WeaponDisplay : FullCardDisplay {

    [SerializeField] Text attack, health;
    Weapon weapon;

    protected override void Awake() {
        card = weapon = GetComponent<Weapon>();
    }

    public override void UpdateDisplay() {
        base.UpdateDisplay();
        attack.text = "" + weapon.attack.value;
        health.text = "" + weapon.durability.value;
    }
}