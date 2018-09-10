using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveModTarget : TargetedEffect {
    [SerializeField] Modifier mod;
    protected override void Resolve(IEntity source, IEntity target) { 
        if (target.entity_type == EntityType.card) {
            Card target_card = (target as Card);
            target_card.mods.AddMod(mod);
        }
    }
}
