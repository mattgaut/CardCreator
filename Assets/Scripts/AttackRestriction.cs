using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRestriction : MonoBehaviour {

    Func<ICombatant, bool> function;

    public AttackRestriction(Func<ICombatant, bool> function) {
        this.function = function;
    }

    public bool CanAttack(ICombatant combatant) {
        return function(combatant);
    }
}
