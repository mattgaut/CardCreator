using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipToken : UntargetedEffect {

    [SerializeField] Weapon to_equip; 

    public override void Resolve(IEntity source) {
        GameStateManager.instance.CreateToken(source.controller.equip, to_equip);
    }
}
