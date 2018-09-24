using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveAdjacentCreaturesAttackStatic : GiveAdjacentCreaturesBuff {
    [SerializeField] int attack_buff;

    protected override void ApplyEffects(IEntity apply_to) {
        Creature creature = apply_to as Creature;
        if (creature == null) {
            return;
        }
        creature.attack.ApplyBuff(new StatBuff(source, BuffType.aura, attack_buff));
    }

    protected override void RemoveEffects(IEntity remove_from) {
        Creature creature = remove_from as Creature;
        if (creature == null) {
            return;
        }
        creature.attack.RemoveBuff(new StatBuff(source, BuffType.aura, attack_buff));
    }
}
