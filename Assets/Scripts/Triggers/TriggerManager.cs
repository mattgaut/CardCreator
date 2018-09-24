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

    public List<TriggerInstance> GetTriggers(TriggerInfo info) {
        List<TriggerInstance> to_return = new List<TriggerInstance>();
        foreach (ITriggeredAbility trigger in subscribed_triggers[info.type]) {
            if (trigger.source.controller == GameManager.current_player && !trigger.on_your_turn) {
                continue;
            }
            if (trigger.source.controller != GameManager.current_player && !trigger.on_their_turn) {
                continue;
            }
            if (trigger.TriggersFrom(info)) {
                to_return.Add(new TriggerInstance(trigger, info));
            }
        }
        return to_return;
    }
}
