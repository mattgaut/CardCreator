using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTokensToHand : UntargetedEffect {

    [SerializeField] List<Card> to_add;

    public override void Resolve(Card source) {
        foreach (Card card in to_add) {
            GameStateManager.instance.CreateToken(source.controller.hand, card);
        }
    }
}
