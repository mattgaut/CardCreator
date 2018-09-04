using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class WeaponController : CardController {
    
    [SerializeField] WeaponFieldDisplay weapon_field_display;
    [SerializeField] Collider field_box;

    Weapon weapon;

    protected override void Awake() {
        base.Awake();
        weapon = GetComponent<Weapon>();
    }

    protected override void UpdateContainer() {
        if (last_container != card.container) {
            last_container = card.container;
            if (last_container != null && last_container.visible) {
                SetFieldDisplay(last_container == card.controller.equip);
            } else {
                HideCard();
            }
        }
    }

    public override void OnClick() {
        if ((!weapon.mods.NeedsTarget() || !GameStateManager.instance.TargetExists(weapon, weapon.mods)) && card.container == card.controller.hand) {
            card.controller.command_manager.AddCommand(new PlayWeaponCommand(weapon));
        }
    }
    public override void OnHoldDrag(GameObject dragged_to, Vector3 position_dragged_to) {
        if (weapon.mods.NeedsTarget() && weapon.mana_cost <= weapon.controller.current_mana && card.container == card.controller.hand) {
            InterfaceManager.DrawTargetingArrow(transform.position, position_dragged_to);
        }
    }
    public override void OnFinishDrag(GameObject dragged_to, Vector3 position_dragged_to) {
        if (card.container == card.controller.hand) {
            IEntity target = dragged_to.GetComponent<IEntity>();
            if (target != null) {
                card.controller.command_manager.AddCommand(new PlayWeaponWithTargetCommand(weapon, target));
            }
        }
    }
    public override void OnEndClick() {
        InterfaceManager.RemoveTargetingArrow();
    }

    void SetFieldDisplay(bool field_display) {
        if (field_display) {
            field_box.enabled = true;
            box.enabled = false;
            weapon_field_display.ShowCard();
            display.HideCard();
            weapon_field_display.UpdateDisplay();
        } else {
            field_box.enabled = false;
            box.enabled = true;
            weapon_field_display.HideCard();
            display.ShowCard();
        }
    }

    protected override void HideCard() {
        base.HideCard();
        field_box.enabled = false;
        weapon_field_display.HideCard();
    }
}
