using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticAbilityManager {

    Dictionary<Zone, List<StaticAbility>> affected_zones_to_abilities;

    GameStateManager game_state_manager;

    public StaticAbilityManager(GameStateManager gsm){
        affected_zones_to_abilities = new Dictionary<Zone, List<StaticAbility>>();
        foreach (Zone zone in (Zone[])System.Enum.GetValues(typeof(Zone))) {
            affected_zones_to_abilities[zone] = new List<StaticAbility>();
        }

        game_state_manager = gsm;
    }

    public void Clear() {
        foreach (List<StaticAbility> list in affected_zones_to_abilities.Values) {
            list.Clear();
        }
    }

    public void SubscribeStaticAbility(StaticAbility ability) {
        foreach (Zone zone in ability.GetAffectedZones()) {
            affected_zones_to_abilities[zone].Add(ability);

            // Check Zone For Card that will be affected
            foreach (Player player in GameManager.players) {
                foreach (Card card in player.GetContainer(zone).cards) {
                    if (ability.AppliesTo(card)) {
                        ability.Apply(card);
                    }
                }
            }
        }
    }

    public void UnsubscribeStaticAbility(StaticAbility ability) {
        ability.RemoveAll();
        foreach (Zone zone in ability.GetAffectedZones()) {
            affected_zones_to_abilities[zone].Remove(ability);
        }
    }

    public void RemoveCardFromAbilities(Card card) {
        foreach (StaticAbility ability in affected_zones_to_abilities[card.container.zone]) {
            if (ability.IsAppliedTo(card)) ability.Remove(card);
        }
    }

    public void AddCardToAbilities(Card card) {
        foreach (StaticAbility ability in affected_zones_to_abilities[card.container.zone]) {
            if (ability.IsAppliedTo(card)) ability.Remove(card);
        }
    }
}
