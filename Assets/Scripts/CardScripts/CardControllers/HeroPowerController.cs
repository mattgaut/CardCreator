using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HeroPower))]
public class HeroPowerController : MonoBehaviour, IClickable {

    [SerializeField] Image art;

    HeroPower hero_power;

    void Update() {
        if (art != null) {
            if (hero_power.controller.can_use_hero_power) {
                art.color = Color.white;
            } else {
                art.color = Color.black;
            }
        }
    }

    public bool must_drag { get; private set; }
    public bool can_click {
        get { return hero_power.controller == GameManager.current_player; }
    }

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

    public void OnHoverEnd(bool was_clicked) {

    }

    public void OnHoverStart() {

    }

    public void OnLeftClickDown() {
    }

    void Awake() {
        hero_power = GetComponent<HeroPower>();

        if (art != null)
            art.sprite = hero_power.art;
    }
}
