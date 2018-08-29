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
        AddTriggeredAbilities(etb_triggered_abilities.OfType<TriggeredAbility>());
        AddTriggeredAbilities(after_attack_triggered_abilities.OfType<TriggeredAbility>());
        AddTriggeredAbilities(before_spell_triggered_abilities.OfType<TriggeredAbility>());
        AddTriggeredAbilities(after_spell_triggered_abilities.OfType<TriggeredAbility>());
    }

    public IEnumerable<TriggeredAbility> GetGlobalTriggersActiveInZone(Zone zone) {
        return global_triggered_abilities.Where(a => a.InZone(zone));
    }

    public IEnumerable<TriggeredAbility> GetLocalTriggers(TriggerInfo info) {
        return local_triggered_abilities.Where(a => a.InZone(card.container.zone) && a.type == info.type && a.TriggersFrom(info));
    }

    public void AddTriggeredAbility(TriggeredAbility ta) {
        if (ta.is_local) {
            local_triggered_abilities.Add(ta);
        } else {
            global_triggered_abilities.Add(ta);
        }
        ta.SetSource(card);
    }

    void AddTriggeredAbilities(IEnumerable<TriggeredAbility> abilities) {
        foreach (TriggeredAbility ta in abilities) {
            AddTriggeredAbility(ta);
        }
    }
}