using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRandomTokenToHand : UntargetedEffect {

    [SerializeField] List<Card> possible_cards;

    public override void Resolve(IEntity source) {
        GameStateManager.instance.CreateToken(source.controller.hand, possible_cards[Random.Range(0, possible_cards.Count)]);
    }
}
