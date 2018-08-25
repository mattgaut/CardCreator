﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : UntargetedEffect {
    [SerializeField] int amount;

    public override void Resolve(Card source) {
        GameStateManager.instance.DrawCard(source.controller, amount);
    }
}
