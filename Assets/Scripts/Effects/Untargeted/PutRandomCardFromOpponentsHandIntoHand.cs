using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutRandomCardFromOpponentsHandIntoHand : UntargetedEffect {

    [SerializeField] int amount_to_copy;

    public override void Resolve(IEntity source) {
        List<Card> possible_cards = new List<Card>();

        foreach (Player player in GameManager.OtherPlayers(source.controller)) {
            possible_cards.AddRange(player.hand.cards);
        }

        for (int i = 0; i < amount_to_copy; i++) {
            if (!source.controller.hand.full) GameStateManager.instance.CreateToken(source.controller.hand, possible_cards[Random.Range(0, possible_cards.Count)]);
        }
    }
}
