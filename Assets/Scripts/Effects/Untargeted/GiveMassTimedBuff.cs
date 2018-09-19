using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GiveMassTimedBuff : UntargetedEffect, ITimedEffect {

    [SerializeField] TimePoint lasts_until;

    [SerializeField] bool friendly, enemy;
    [SerializeField] int attack_buff, health_buff;

    public TimePoint time_point {
        get { return lasts_until; }
    }

    public void EndEffect(IEntity source, IEntity target = null) {
        if (target == null) {
            return;
        }

        Creature creature = target as Creature;
        if (creature == null) {
            return;
        }

        if (attack_buff > 0) creature.attack.RemoveBuff(attack_buff);
        if (health_buff > 0) creature.health.RemoveBuff(health_buff);
    }

    public override void Resolve(IEntity source) {
        if (friendly) {
            foreach (Creature creature in source.controller.field.cards.OfType<Creature>()) {
                if (attack_buff > 0) creature.attack.ApplyBuff(attack_buff);
                if (health_buff > 0) creature.health.ApplyBuff(health_buff);
            }
        }
        if (enemy) {
            foreach (Player player in GameManager.players) {
                if (player == source.controller) {
                    continue;
                }
                foreach (Creature creature in player.field.cards.OfType<Creature>()) {
                    if (attack_buff > 0) creature.attack.ApplyBuff(attack_buff);
                    if (health_buff > 0) creature.health.ApplyBuff(health_buff);
                }
            }            
        }
    }
}
