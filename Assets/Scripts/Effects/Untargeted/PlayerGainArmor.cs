using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGainArmor : UntargetedEffect {

    [SerializeField] int to_gain;

    public override void Resolve(IEntity source) {
        source.controller.GainArmor(source, to_gain);
    }

}
