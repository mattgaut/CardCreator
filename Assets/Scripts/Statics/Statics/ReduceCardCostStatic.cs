using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReduceCardCostStatic : StaticAbility {

    [SerializeField] int cost_to_reduce;
    [SerializeField] CompareEntity type_to_reduce;

    protected override void ApplyEffects(IEntity apply_to) {
        Card card = apply_to as Card;
        if (card != null) {
            card.mana_cost.ApplyBuff(-cost_to_reduce);
        }
    }

    protected override void RemoveEffects(IEntity remove_from) {
        Card card = remove_from as Card;
        if (card != null) {
            card.mana_cost.RemoveBuff(-cost_to_reduce);
        }
    }
}
