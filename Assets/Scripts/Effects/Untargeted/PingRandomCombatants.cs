using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PingRandomCombatants : UntargetedEffect {

    [SerializeField] bool friendly, enemy, players;
    [SerializeField] int number_of_pings;

    public override void Resolve(IEntity source) {
        List<ICombatant> possible_targets = new List<ICombatant>();

        if (friendly) {
            if (players) {
                possible_targets.Add(source.controller);
            }
            possible_targets.AddRange(source.controller.field.cards.OfType<ICombatant>());
        }
        if (enemy) {
            foreach (Player enemy in GameManager.players) {
                if (enemy == source.controller) {
                    continue;
                }
                if (players) {
                    possible_targets.Add(enemy);
                }
                possible_targets.AddRange(enemy.field.cards.OfType<ICombatant>());
            }
        }

        if (source as Spell != null) {
            int total = number_of_pings + source.controller.TotalSpellPower();
            for (int i = 0; i < total; i++) {
                ICombatant chosen = possible_targets[Random.Range(0, possible_targets.Count)];
                (source as Spell).DealDamageWithoutSpellpower(chosen, 1);
                if (chosen.dead == true) {
                    possible_targets.Remove(chosen);
                }
            }
        } else {
            for (int i = 0; i < number_of_pings; i++) {
                ICombatant chosen = possible_targets[Random.Range(0, possible_targets.Count)];
                source.DealDamage(chosen, 1);
                if (chosen.dead == true) {
                    possible_targets.Remove(chosen);
                }
            }
        }  
    }

}
