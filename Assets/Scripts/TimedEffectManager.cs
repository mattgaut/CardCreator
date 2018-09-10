using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimePoint { end_of_turn, end_of_opponents_turn, beginning_of_your_turn }

public class TimedEffectManager {

    Dictionary<TimePoint, List<ITimedEffect>> effects_waiting_to_end;
    Dictionary<ITimedEffect, IEntity> sources;
    Dictionary<ITimedEffect, IEntity> targets;

    public TimedEffectManager() {
        sources = new Dictionary<ITimedEffect, IEntity>();
        targets = new Dictionary<ITimedEffect, IEntity>();
        effects_waiting_to_end = new Dictionary<TimePoint, List<ITimedEffect>>();
        foreach (TimePoint tp in (TimePoint[])System.Enum.GetValues(typeof(TimePoint))) {
            effects_waiting_to_end.Add(tp, new List<ITimedEffect>());
        }
    }

    public void AddTimedEffect(ITimedEffect te, IEntity source) {
        effects_waiting_to_end[te.time_point].Add(te);
        sources.Add(te, source);
    }

    public void AddTimedEffect(ITimedEffect te, IEntity source, IEntity target) {
        effects_waiting_to_end[te.time_point].Add(te);
        sources.Add(te, source);
        targets.Add(te, target);
    }

    public void EndEffects(TimePoint time_point) {
        for (int i = effects_waiting_to_end[time_point].Count - 1; i >= 0; i--) {
            ITimedEffect effect = effects_waiting_to_end[time_point][i];
            EndEffect(effect, time_point);
        }
    }

    public void EndEffects(TimePoint time_point, Player player) {
        for (int i = effects_waiting_to_end[time_point].Count - 1; i >= 0; i--) {
            ITimedEffect effect = effects_waiting_to_end[time_point][i];
            if (sources[effect].controller == player) {
                EndEffect(effect, time_point);
            }
        }
    }

    void EndEffect(ITimedEffect effect, TimePoint time_point) {
        if (targets.ContainsKey(effect)) {
            effect.EndEffect(sources[effect], targets[effect]);
            targets.Remove(effect);
        } else {
            effect.EndEffect(sources[effect]);
        }
        effects_waiting_to_end[time_point].Remove(effect);
        sources.Remove(effect);
    }
}
