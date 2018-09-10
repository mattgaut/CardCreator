using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageAllCombatants : UntargetedEffect {
    [SerializeField] bool friendly, enemy;
    [SerializeField] int damage;

    public override void Resolve(IEntity source) {
        List<ICombatant> to_affect = new List<ICombatant>();
        if (friendly) {
            to_affect.Add(source.controller);

            to_affect.AddRange(source.controller.field.cards.OfType<ICombatant>());
        }
        if (enemy) {
            foreach (Player p in GameManager.players) {
                if (p == source.controller) continue;

                to_affect.Add(p);

                to_affect.AddRange(p.field.cards.OfType<ICombatant>());
            }
        }

        foreach (ICombatant c in to_affect) {
            source.DealDamage(c, damage);
        }
    }
}
