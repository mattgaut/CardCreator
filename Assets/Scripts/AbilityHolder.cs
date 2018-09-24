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

        // Set the source for the triggered abilities
        foreach (StaticAbility ability in static_abilities) {
            ability.SetSource(card);
        }
    }

    public IEnumerable<TriggeredAbility> GetGlobalTriggersActiveInZone(Zone zone) {
        return global_triggered_abilities.Where(a => a.ActiveInZone(zone));
    }

    public IEnumerable<TriggerInstance> GetLocalTriggers(TriggerInfo info) {
        Debug.Log("Local: " + local_triggered_abilities.Count);
        Debug.Log("Info: " + info.type);
        List<TriggeredAbility> triggers = new List<TriggeredAbility>(local_triggered_abilities.Where(a => a.ActiveInZone(card.container.zone) && a.type == info.type && a.TriggersFrom(info)));
        Debug.Log(triggers.Count);
        List<TriggerInstance> trigger_instances = new List<TriggerInstance>();
        foreach (TriggeredAbility ta in triggers) {
            trigger_instances.Add(new TriggerInstance(ta, info));
        }
        return trigger_instances;
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