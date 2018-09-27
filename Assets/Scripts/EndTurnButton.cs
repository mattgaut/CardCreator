using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnButton : MonoBehaviour, IClickable {
    public bool must_drag {
        get {
            return false;
        }
    }

    public bool can_click {
        get { return true; }
    }

    public void OnClick() {
        GameManager.current_player.command_manager.EndTurn();
    }

    public void OnEndClick() {
    }

    public void OnFinishDrag(GameObject dragged_to, Vector3 position_dragged_to) {
    }

    public void OnHoldDrag(GameObject dragged_to, Vector3 position_dragged_to) {
    }

    public void OnHoverEnd(bool was_clicked) {
    }

    public void OnHoverStart() {
    }

    public void OnLeftClickDown() {
    }
}
