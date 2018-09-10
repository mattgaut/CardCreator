using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPower : MonoBehaviour, IEntity, IStackEffect {

    [SerializeField] Stat _mana_cost;

    [SerializeField] List<UntargetedEffect> untargeted_effects;

    public Player controller { get; private set; }

    public EntityType entity_type {
        get { return EntityType.hero_power; }
    }

    public bool CanBeTargeted(IEntity source) {
        return true;
    }

    public int DealDamage(IDamageable target, int damage) {
        throw new System.NotImplementedException();
    }

    public virtual void Resolve() {
        for (int i = 0; i < untargeted_effects.Count; i++) {
            untargeted_effects[i].Resolve(this);
        }
    }
}
