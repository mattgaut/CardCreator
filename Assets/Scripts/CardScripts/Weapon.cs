using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Card {

    [SerializeField] Stat _attack;
    [SerializeField] ResourceStat _durability;

    public override CardType type {
        get { return CardType.Weapon; }
    }

    public Stat attack {
        get { return _attack; }
    }
    public ResourceStat durability {
        get { return _durability; }
    }

    public override void Resolve() {
        durability.current_value = durability;
    }
}
