using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveEquipedWeaponAttackBuff : UntargetedEffect {

    [SerializeField] int attack_buff;

    public override void Resolve(IEntity source) {
        if (source.controller.weapon != null) {
            source.controller.weapon.attack.ApplyBuff(new StatBuff(source, StatBuff.Type.basic, attack_buff));
        }
    }
}
