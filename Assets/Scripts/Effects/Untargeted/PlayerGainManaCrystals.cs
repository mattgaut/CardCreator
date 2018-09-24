using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGainManaCrystals : UntargetedEffect {

    [SerializeField] int amount;
    [SerializeField] bool friendly, enemy;
    [SerializeField] bool filled;

    public override void Resolve(IEntity source) {
        if (friendly) {
            source.controller.AddMaxMana(amount, filled);
        }
        if (enemy) {
            foreach (Player p in GameManager.OtherPlayers(source.controller)) {
                p.AddMaxMana(amount, filled);
            }
        }
    }
}
