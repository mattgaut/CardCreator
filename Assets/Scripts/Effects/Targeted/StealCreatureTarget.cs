using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealCreatureTarget : TargetedEffect {

    protected override void Resolve(IEntity source, IEntity target) {
        Creature creature = target as Creature;
        if (creature == null) {
            return;
        }

        if (creature.controller != source.controller) {
            GameStateManager.instance.MoveCard(creature, source.controller.field);
            creature.NoteControlChange();
        }
    }
}
