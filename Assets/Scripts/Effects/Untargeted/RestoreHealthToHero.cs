﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestoreHealthToHero : UntargetedEffect {

    [SerializeField] int amount;

    public override void Resolve(Card source) {
        source.controller.Heal(source, amount);
    }
}
