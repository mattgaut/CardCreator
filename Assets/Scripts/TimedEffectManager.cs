using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimePoint { end_of_turn, end_of_opponents_turn, beginning_of_your_turn }

public class TimedEffectManager {

    Dictionary<TimePoint, List<int>> effects_waiting_to_end;

    Dictionary<int, IEntity> sources;
    Dictionary<int, IEntity> targets;
    Dictionary<int, ITimedEffect> effects;

    int next_id;

    public TimedEffectManager() {
        sources = new Dictionary<int, IEntity>();
        targets = new Dictionary<int, IEntity>();
        effects = new Dictionary<int, ITimedEffect>();
        effects_waiting_to_end = new Dictionary<TimePoint, List<int>>();
        next_id = 0;
        foreach (TimePoint tp in (TimePoint[])System.Enum.GetValues(typeof(TimePoint))) {
            effects_waiting_to_end.Add(tp, new List<int>());
        }
    }

    public void AddTimedEffect(ITimedEffect te, IEntity source) {
        int id = next_id++;
        effects_waiting_to_end[te.time_point].Add(id);
        sources.Add(id, source);
        effects.Add(id, te);
    }

    public void AddTimedEffect(ITimedEffect te, IEntity source, IEntity target) {
        int id = next_id++;
        effects_waiting_to_end[te.time_point].Add(id);
        sources.Add(id, source);
        targets.Add(id, target);
        effects.Add(id, te);
    }

    public void EndEffects(TimePoint time_point) {
        for (int i = effects_waiting_to_end[time_point].Count - 1; i >= 0; i--) {
            int id = effects_waiting_to_end[time_point][i];
            EndEffect(id);
        }
    }

    public void EndEffects(TimePoint time_point, Player player) {
        for (int i = effects_waiting_to_end[time_point].Count - 1; i >= 0; i--) {
            int id = effects_waiting_to_end[time_point][i];
            ITimedEffect effect = effects[id];
            if (sources[id].controller == player) {
                EndEffect(id);
            }
        }
    }

    void EndEffect(int id) {
        ITimedEffect effect = effects[id];
        if (targets.ContainsKey(id)) {
            effect.EndEffect(sources[id], targets[id]);
            targets.Remove(id);
        } else {
            effect.EndEffect(sources[id]);
        }
        effects_waiting_to_end[effect.time_point].Remove(id);
        sources.Remove(id);
    }
}
