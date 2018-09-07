using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AbilityHolder : MonoBehaviour {

    [SerializeField] List<StaticAbility> static_abilities;

    // Triggers
    [SerializeField] List<TriggeredAbility> triggered_abilities;

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
        AddTriggeredAbilities(triggered_abilities);
    }

    public IEnumerable<TriggeredAbility> GetGlobalTriggersActiveInZone(Zone zone) {
        return global_triggered_abilities.Where(a => a.InZone(zone));
    }

    public IEnumerable<TriggeredAbility> GetLocalTriggers(TriggerInfo info) {
        return local_triggered_abilities.Where(a => a.InZone(card.container.zone) && a.type == info.type && a.TriggersFrom(info));
    }

    public IEnumerable<StaticAbility> GetStaticAbilitiesActiveInZone(Zone zone) {
        return static_abilities.Where(a => a.InZone(zone));
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