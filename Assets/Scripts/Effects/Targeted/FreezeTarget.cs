﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeTarget : TargetedEffect {

    protected override void Resolve(IEntity source, IEntity target) {
        ICombatant combatant = target as ICombatant;
        if (combatant != null) {
            combatant.Freeze();
        }
    }
}
