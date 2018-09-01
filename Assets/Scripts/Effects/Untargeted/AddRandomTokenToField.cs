using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRandomTokenToField : UntargetedEffect {

    [SerializeField] List<Creature> possible_cards;

    public override void Resolve(Card source) {
        GameStateManager.instance.CreateToken(source.controller.field, possible_cards[Random.Range(0, possible_cards.Count)]);
    }
}