using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReduceManaCostInHand : UntargetedEffect {

    [SerializeField] int to_reduce;

    public override void Resolve(Card source) {
        foreach (Card c in source.controller.hand.cards) {
            c.mana_cost.ApplyBuff(to_reduce);
        }
    }
}
