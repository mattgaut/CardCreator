using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseDrawFromTopCards : UntargetedEffect, ICardSelectionEffect {

    [SerializeField] int amount_to_look_at;

    public void FinishResolve(IEntity source, Card selected, List<Card> cards) {
        GameStateManager.instance.TryMoveCardToHand(source.controller, selected);

        foreach (Card c in cards) {
            GameStateManager.instance.DiscardCard(c);
        }
    }

    public override void Resolve(IEntity source) {
        List<Card> cards = new List<Card>(source.controller.deck.TopCards(amount_to_look_at));

        GameStateManager.instance.SelectCard(this, source, cards);
    }
}
