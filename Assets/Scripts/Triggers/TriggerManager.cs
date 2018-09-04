using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour {

    Dictionary<TriggerType, List<TriggeredAbility>> subscribed_triggers;

    public void Awake() {
        subscribed_triggers = new Dictionary<TriggerType, List<TriggeredAbility>>();
        foreach (TriggerType t in System.Enum.GetValues(typeof(TriggerType))) {
            subscribed_triggers.Add(t, new List<TriggeredAbility>());
        }
    }

    public void SubscribeTrigger(TriggeredAbility ability) {
        subscribed_triggers[ability.type].Add(ability);
    }

    public void UnsubscribeTrigger(TriggeredAbility ability) {
        subscribed_triggers[ability.type].Remove(ability);
    }

    public List<TriggeredAbility> GetTriggers(TriggerInfo info) {
        List<TriggeredAbility> to_return = new List<TriggeredAbility>();
        foreach (TriggeredAbility trigger in subscribed_triggers[info.type]) {
            if (trigger.source.controller == GameManager.current_player && !trigger.on_your_turn) {
                continue;
            }
            if (trigger.source.controller != GameManager.current_player && trigger.on_their_turn) {
                continue;
            }
            if (trigger.CheckTrigger(info)) {
                to_return.Add(trigger);
            }
        }
        return to_return;
    }
}
