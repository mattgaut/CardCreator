﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Creature))]
public class CreatureDisplay : FullCardDisplay {

    [SerializeField] Text attack, health;
    Creature creature;

    protected override void Awake() {
        card = creature = GetComponent<Creature>();
    }

    public override void UpdateDisplay() {
        base.UpdateDisplay();
        attack.text = "" + creature.attack;
        health.text = "" + creature.max_health;
    }
}
