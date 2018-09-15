using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Card {

    [SerializeField] Stat _attack;
    [SerializeField] ResourceStat _durability;
    private bool destroyed;

    public override CardType type {
        get { return CardType.Weapon; }
    }

    public Stat attack {
        get { return _attack; }
    }
    public ResourceStat durability {
        get { return _durability; }
    }
    public bool dead { get { return durability.current_value <= 0 || destroyed; } }
    
    public void NoteSummon() {
        durability.current_value = durability;
        destroyed = false;
    }

    public override void Resolve() {
    }

    public void Destroy() {
        destroyed = true;
    }
}
