using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour {

    Dictionary<TriggerType, List<ITriggeredAbility>> subscribed_triggers;

    public void Awake() {
        subscribed_triggers = new Dictionary<TriggerType, List<ITriggeredAbility>>();
        foreach (TriggerType t in System.Enum.GetValues(typeof(TriggerType))) {
            subscribed_triggers.Add(t, new List<ITriggeredAbility>());
        }
    }

    public void SubscribeTrigger(ITriggeredAbility ability) {
        subscribed_triggers[ability.type].Add(ability);
    }

    public void UnsubscribeTrigger(ITriggeredAbility ability) {
        subscribed_triggers[ability.type].Remove(ability);
    }

    public List<ITriggeredAbility> GetTriggers(TriggerInfo info) {
        List<ITriggeredAbility> to_return = new List<ITriggeredAbility>();
        foreach (ITriggeredAbility trigger in subscribed_triggers[info.type]) {
            if (trigger.source.controller == GameManager.current_player && trigger.on_your_turn) {
                continue;
            }
            if (trigger.source.controller != GameManager.current_player && trigger.on_their_turn) {
                continue;
            }
            if (trigger.TriggersFrom(info)) {
                to_return.Add(trigger);
            }
        }
        return to_return;
    }
}
