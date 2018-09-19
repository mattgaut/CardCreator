using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedCantAttackPlayersTarget : TargetedEffect, ITimedEffect {

    [SerializeField] TimePoint _time_point;

    Dictionary<IEntity, int> restriction_ids;


    public TimePoint time_point {
        get { return _time_point; }
    }

    public void EndEffect(IEntity source, IEntity target = null) {
        if (target == null) {
            return;
        }
        ICombatant combatant = (target as ICombatant);

        if (combatant != null && restriction_ids.ContainsKey(target)) {
            combatant.RemoveAttackRestriction(restriction_ids[target]);
            restriction_ids.Remove(target);
        }
    }

    protected override void Resolve(IEntity source, IEntity target) {
        ICombatant combatant = (target as ICombatant);

        if (combatant != null) {
            if (restriction_ids.ContainsKey(target)) {
                combatant.RemoveAttackRestriction(restriction_ids[target]);
                restriction_ids.Remove(target);
            }

            int id = combatant.AddAttackRestriction(new AttackRestriction(CantAttackPlayers));
            restriction_ids.Add(target, id);

            GameStateManager.instance.TrackTimedEffect(this, source, target);
        }
    }

    bool CantAttackPlayers(ICombatant target) {
        return target.entity_type != EntityType.player;
    }

    void Awake() {
        restriction_ids = new Dictionary<IEntity,int>();
    }
}
