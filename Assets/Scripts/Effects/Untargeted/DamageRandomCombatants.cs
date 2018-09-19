using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageRandomCombatants : UntargetedEffect {

    [SerializeField] bool friendly, enemy, players;
    [SerializeField] int number_to_damage;
    [SerializeField] int damage;

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

        for (int i = 0; i < number_to_damage && possible_targets.Count > 0; i++) {
            ICombatant chosen = possible_targets[Random.Range(0, possible_targets.Count)];
            source.DealDamage(chosen, damage);
            possible_targets.Remove(chosen);
        }
    }
}
