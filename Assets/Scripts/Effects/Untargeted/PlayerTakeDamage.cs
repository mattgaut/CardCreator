using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeDamage : UntargetedEffect {

    [SerializeField] int damage;

    public override void Resolve(IEntity source) {
        source.DealDamage(source.controller, damage);
    }
}
