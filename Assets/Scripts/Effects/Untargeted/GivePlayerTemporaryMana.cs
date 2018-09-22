using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GivePlayerTemporaryMana : UntargetedEffect {

    [SerializeField] int amount;

    public override void Resolve(IEntity source) {
        source.controller.GainMana(amount, true);
    }
}
