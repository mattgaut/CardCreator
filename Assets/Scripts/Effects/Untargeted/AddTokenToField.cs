using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTokenToField : UntargetedEffect {

    [SerializeField] List<Creature> to_add;

    public override void Resolve(IEntity source) {
        foreach (Card card in to_add) {
            GameStateManager.instance.CreateToken(source.controller.field, card);
        }
    }
}
