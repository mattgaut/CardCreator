using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Card))]
public class AbilityHolder : MonoBehaviour {
    [SerializeField] List<ETBTriggeredAbility> etb_triggered_abilities;

    [SerializeField] List<TriggeredAbility> triggered_abilities;

    public Card card {
        get; private set;
    }

    private void Awake() {
        card = GetComponent<Card>();
        triggered_abilities = new List<TriggeredAbility>();
        triggered_abilities.AddRange(triggered_abilities);
        foreach (TriggeredAbility ta in triggered_abilities) {
            ta.SetSource(card);
        }
    }

    public IEnumerable<TriggeredAbility> GetTriggersActiveInZone(Zone zone) {
        return triggered_abilities.Where(a => a.InZone(zone));
    }

    public void AddTriggeredAbility(TriggeredAbility ta) {
        triggered_abilities.Add(ta);
        ta.SetSource(card);
    }
}

public enum AbilityType { constant, trigger, none }