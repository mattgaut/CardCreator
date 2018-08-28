using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Card))]
public class AbilityHolder : MonoBehaviour {
   
    [SerializeField] List<ETBTriggeredAbility> etb_triggered_abilities;
    [SerializeField] List<AfterAttackTriggeredAbility> after_attack_triggered_abilities;
    [SerializeField] List<BeforeSpellTriggeredAbility> before_spell_triggered_abilities;
    [SerializeField] List<AfterSpellTriggeredAbility> after_spell_triggered_abilities;

    List<TriggeredAbility> global_triggered_abilities;
    List<TriggeredAbility> local_triggered_abilities;

    public Card card {
        get; private set;
    }

    private void Awake() {
        card = GetComponent<Card>();
        global_triggered_abilities = new List<TriggeredAbility>();
        local_triggered_abilities = new List<TriggeredAbility>();

        // Add All trigger types to their proper lists
        AddTriggersToList(etb_triggered_abilities.OfType<TriggeredAbility>());
        AddTriggersToList(after_attack_triggered_abilities.OfType<TriggeredAbility>());
        AddTriggersToList(before_spell_triggered_abilities.OfType<TriggeredAbility>());
        AddTriggersToList(after_spell_triggered_abilities.OfType<TriggeredAbility>());
    }

    public IEnumerable<TriggeredAbility> GetTriggersActiveInZone(Zone zone) {
        return global_triggered_abilities.Where(a => a.InZone(zone));
    }

    public void AddTriggeredAbility(TriggeredAbility ta) {
        global_triggered_abilities.Add(ta);
        ta.SetSource(card);
    }

    void AddTriggersToList(IEnumerable<TriggeredAbility> abilities) {
        foreach (TriggeredAbility ta in abilities) {
            if (ta.is_local) {
                local_triggered_abilities.Add(ta);
            } else {
                global_triggered_abilities.Add(ta);
            }
            ta.SetSource(card);
        }
    }
}