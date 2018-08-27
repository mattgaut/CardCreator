using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Card))]
public class AbilityHolder : MonoBehaviour {
    [SerializeField] List<TriggeredAbility> global_triggered_abilities;
    [SerializeField] List<TriggeredAbility> local_triggered_abilities;

    public Card card {
        get; private set;
    }

    private void Awake() {
        card = GetComponent<Card>();
        global_triggered_abilities = new List<TriggeredAbility>();
        global_triggered_abilities.AddRange(global_triggered_abilities);
        foreach (TriggeredAbility ta in global_triggered_abilities) {
            ta.SetSource(card);
        }
    }

    public IEnumerable<TriggeredAbility> GetTriggersActiveInZone(Zone zone) {
        return global_triggered_abilities.Where(a => a.InZone(zone));
    }

    public void AddTriggeredAbility(TriggeredAbility ta) {
        global_triggered_abilities.Add(ta);
        ta.SetSource(card);
    }
}

public enum AbilityType { constant, trigger, none }