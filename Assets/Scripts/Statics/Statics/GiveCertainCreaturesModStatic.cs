using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveCertainCreaturesModStatic : StaticAbility {

    [SerializeField] Modifier mod;

    [SerializeField] Creature.CreatureType to_buff;
    [SerializeField] protected bool other;

    public override bool AppliesTo(IEntity entity) {
        if (!other && ReferenceEquals(entity, source)) {
            return false;
        }

        Creature creature = entity as Creature;

        if (creature == null || creature.creature_type != to_buff) {
            return false;
        }

        return base.AppliesTo(entity);
    }

    protected override void ApplyEffects(IEntity apply_to) {
        Creature creature = apply_to as Creature;

        if (creature == null) {
            return;
        }

        creature.mods.ApplyBuff(new ModBuff(source, BuffType.aura, mod));
    }

    protected override void RemoveEffects(IEntity remove_from) {
        Creature creature = remove_from as Creature;

        if (creature == null) {
            return;
        }

        creature.mods.RemoveBuff(new ModBuff(source, BuffType.aura, mod));
    }
}
