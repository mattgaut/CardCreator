using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimePoint { end_of_turn, end_of_opponents_turn, beginning_of_your_turn }

public class TimedEffectManager {

    Dictionary<TimePoint, List<ITimedEffect>> effects_waiting_to_end;
    Dictionary<ITimedEffect, Card> sources;

    GameStateManager game_state_manager;

    public TimedEffectManager(GameStateManager gsm) {
        sources = new Dictionary<ITimedEffect, Card>();
        effects_waiting_to_end = new Dictionary<TimePoint, List<ITimedEffect>>();
        foreach (TimePoint tp in (TimePoint[])System.Enum.GetValues(typeof(TimePoint))) {
            effects_waiting_to_end.Add(tp, new List<ITimedEffect>());
        }
        game_state_manager = gsm;
    }

    public void AddTimedEffect(ITimedEffect te, Card source) {
        effects_waiting_to_end[te.time_point].Add(te);
        sources.Add(te, source);
    }

    public void EndEffects(TimePoint time_point) {
        for (int i = effects_waiting_to_end.Count - 1; i >= 0; i++) {
            ITimedEffect effect = effects_waiting_to_end[time_point][i];
            effect.EndEffect(sources[effect]);
            effects_waiting_to_end[time_point].Remove(effect);
        }
    }

    public void EndEffects(TimePoint time_point, Player player) {
        foreach (ITimedEffect effect in effects_waiting_to_end[time_point]) {
           effect.EndEffect(sources[effect]);
        }
        for (int i = effects_waiting_to_end.Count - 1; i >= 0; i++) {
            ITimedEffect effect = effects_waiting_to_end[time_point][i];
            if (sources[effect].controller == player) {
                effect.EndEffect(sources[effect]);
                effects_waiting_to_end[time_point].Remove(effect);
            }
        }
    }
}
