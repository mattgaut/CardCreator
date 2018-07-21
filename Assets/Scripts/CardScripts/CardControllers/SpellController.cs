using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Spell))]
public class SpellController : CardController {
    Spell spell;
    protected override void Awake() {
        base.Awake();
        spell = GetComponent<Spell>();
    }

    public override void OnClick() {
        if (!spell.needs_target && card.container == card.controller.hand) {
            card.controller.command_manager.AddCommand(new PlayCardCommand(card));
        }
    }
    public override void OnHoldDrag(GameObject dragged_to, Vector3 position_dragged_to) {
        if (spell.needs_target && spell.mana_cost <= spell.controller.current_mana && card.container == card.controller.hand) {
            InterfaceManager.DrawTargetingArrow(transform.position, position_dragged_to);
        }
    }
    public override void OnFinishDrag(GameObject dragged_to, Vector3 position_dragged_to) {
        if (card.container == card.controller.hand) {
            IEntity target = dragged_to.GetComponent<IEntity>();
            if (target != null) {
                card.controller.command_manager.AddCommand(new PlayTargetedSpellCommand(spell, target));
            }
        }
    }
    public override void OnEndClick() {
        InterfaceManager.RemoveTargetingArrow();
    }

}
