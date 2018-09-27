using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour, IClickable {

    Player player;

    public bool must_drag {
        get; private set;
    }
    public bool can_click {
        get { return player == GameManager.current_player; }
    }

    void Awake() {
        player = GetComponent<Player>();
    }

    public void OnClick() {
    }

    public void OnEndClick() {
        InterfaceManager.RemoveTargetingArrow();
    }

    public void OnFinishDrag(GameObject dragged_to, Vector3 position_dragged_to) {
        if (player.can_attack) {
            ICombatant target;
            if ((target = dragged_to.GetComponent<ICombatant>()) == null) {
                return;
            }
            player.command_manager.AddCommand(new AttackCommand(target, player));
        }
    }

    public void OnHoldDrag(GameObject dragged_to, Vector3 position_dragged_to) {
        if (player.can_attack) {
            InterfaceManager.DrawTargetingArrow(transform.position, position_dragged_to);
        }
    }

    public void OnLeftClickDown() {
        
    }

    public void OnHoverStart() {

    }

    public void OnHoverEnd(bool was_clicked) {

    }
}
