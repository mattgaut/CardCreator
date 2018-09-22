using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDiscardRandomCard : UntargetedEffect {

    [SerializeField] int amount;

    public override void Resolve(IEntity source) {
        GameStateManager.instance.DiscardRandomCard(source.controller, amount);
    }
}
