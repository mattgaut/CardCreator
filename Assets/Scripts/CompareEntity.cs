using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CompareEntity {

    // Comparisons
    [SerializeField] List<EntityType> types;

    [SerializeField] bool friendly, enemy;

    [SerializeField] bool check_attack;
    [SerializeField] Range attack_range;
    [SerializeField] int attack;

    [SerializeField] bool check_health;
    [SerializeField] Range health_range;
    [SerializeField] int health;

    // Card Comparisons
    [SerializeField] bool match_card_type;
    [SerializeField] List<CardType> card_types;

    [SerializeField] bool check_mana_cost;
    [SerializeField] Range mana_cost_range;
    [SerializeField] int mana_cost;


    public bool CompareTo(IEntity entity, IEntity source) {
        if (entity.controller != source.controller && !enemy) return false;
        if (entity.controller == source.controller && !friendly) return false;
        if (!types.Contains(entity.entity_type)) return false;
        if (entity.entity_type == EntityType.card) {
            return CompareToCard(entity as ICard);
        } else if (entity.entity_type == EntityType.player) {
            return CompareToPlayer(entity as IPlayer);
        }
        return false;
    }

    bool CompareToPlayer(IPlayer entity) {
        if (entity == null) {
            return false;   
        }
        if (check_attack && !NumberInRange(entity.player.attack, attack, attack_range)) {
            return false;
        }
        if (check_health && !NumberInRange(entity.player.current_health, health, health_range)) {
            return false;
        }
        return true;
    }

    bool CompareToCard(ICard entity) {
        if (entity == null) {
            return false;
        }
        if (match_card_type && !card_types.Contains(entity.card.type)) {
            return false;
        }
        if (check_mana_cost && !NumberInRange(entity.card.mana_cost, mana_cost, mana_cost_range)) {
            return false;
        }
        Creature creature = entity.card as Creature;
        if (check_attack && (creature == null || !NumberInRange(creature.attack, attack, attack_range))) {
            return false;
        }
        if (check_health && (creature == null || !NumberInRange(creature.current_health, health, health_range))) {
            return false;
        }
        return true;
    }

    bool NumberInRange(int check, int against, Range r) {
        if (r == Range.Equal) {
            return check == against;
        } else if (r == Range.Less) {
            return check < against;
        } else if (r == Range.LessOrEqual) {
            return check <= against;
        } else if (r == Range.Greater) {
            return check > against;
        } else if (r == Range.GreaterOrEqual) {
            return check >= against;
        }
        return false;
    }
}

public enum Range { Equal, LessOrEqual, Less, GreaterOrEqual, Greater }
