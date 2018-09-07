using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Secret))]
public class SecretController : CardController {
    Secret secret;
    protected override void Awake() {
        base.Awake();
        secret = GetComponent<Secret>();
    }

    public override void OnClick() {
        if (card.container == card.controller.hand) {
            card.controller.command_manager.AddCommand(new PlaySecretCommand(secret));
        }
    }
    public override void OnHoldDrag(GameObject dragged_to, Vector3 position_dragged_to) {
    }
    public override void OnFinishDrag(GameObject dragged_to, Vector3 position_dragged_to) {
    }
    public override void OnEndClick() {
    }
}
