using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HeroPower))]
public class HeroPowerController : MonoBehaviour, IClickable {

    HeroPower hero_power;

    public void OnClick() {
        if (!hero_power.has_targeted_effects && hero_power.is_useable) {
            hero_power.controller.command_manager.AddCommand(new UseHeroPowerCommand(hero_power.controller));
        }
    }

    public void OnEndClick() {
        InterfaceManager.RemoveTargetingArrow();
    }

    public void OnFinishDrag(GameObject dragged_to, Vector3 position_dragged_to) {
        IEntity target = dragged_to.GetComponent<IEntity>();
        if (target != null) {
            hero_power.controller.command_manager.AddCommand(new UseTargetedHeroPowerCommand(hero_power.controller, target));
        }
    }

    public void OnHoldDrag(GameObject dragged_to, Vector3 position_dragged_to) {
        if (hero_power.has_targeted_effects && hero_power.mana_cost <= hero_power.controller.current_mana) {
            InterfaceManager.DrawTargetingArrow(transform.position, position_dragged_to);
        }
    }

    public void OnMouseDown() {
    }

    void Awake() {
        hero_power = GetComponent<HeroPower>();
    }
}
