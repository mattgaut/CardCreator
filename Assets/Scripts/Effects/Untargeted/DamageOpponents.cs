using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOpponents : UntargetedEffect {
    [SerializeField] int damage;
    public override void Resolve(IEntity source) {
        List<Player> players = new List<Player>(GameManager.players);
        players.Remove(source.controller);

        foreach (IDamageable d in players) {
            source.DealDamage(d, damage);
        }
    }
}
