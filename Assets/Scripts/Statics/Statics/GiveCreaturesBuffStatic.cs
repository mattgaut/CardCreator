using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveCreaturesBuffStatic : StaticAbility {

    [SerializeField] protected int attack_buff;
    [SerializeField] protected int health_buff;

    [SerializeField] protected bool other;

    public override bool AppliesTo(IEntity entity) {
        if (ReferenceEquals(entity, source) && other) {
            return false;
        }
        return (entity as Creature != null) && base.AppliesTo(entity);
    }

    protected override void ApplyEffects(IEntity apply_to) {
        Creature creature = apply_to as Creature;
        if (creature != null) {
            if (attack_buff != 0) creature.attack.ApplyBuff(new StatBuff(source, StatBuff.Type.aura, attack_buff));
            if (health_buff != 0) creature.health.ApplyBuff(new StatBuff(source, StatBuff.Type.aura, health_buff));
        }
    }

    protected override void RemoveEffects(IEntity remove_from) {
        Creature creature = remove_from as Creature;
        if (creature != null) {           
            if (attack_buff != 0) creature.attack.RemoveBuff(new StatBuff(source, StatBuff.Type.aura, attack_buff));
            if (health_buff != 0) creature.health.RemoveBuff(new StatBuff(source, StatBuff.Type.aura, health_buff));
        }
    }
}
