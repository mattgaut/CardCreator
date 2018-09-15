using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOpponentsWeapon : UntargetedEffect {
    public override void Resolve(IEntity source) {
        foreach (Player player in GameManager.players) {
            if (player == source.controller) continue;

            if (player.weapon != null) {
                player.weapon.Destroy();
            }
        }
    }
}
