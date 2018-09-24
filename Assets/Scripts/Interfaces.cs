using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType { card = 1, player, hero_power }
public interface IEntity : IDamages {
    bool CanBeTargeted(IEntity source);
    Player controller { get; }
    EntityType entity_type { get; }
}
public interface ICard : IEntity {
    Card card { get; }
}
public interface IPlayer : IEntity {
    Player player { get; }
}

public interface ITargets {
    CompareEntity targeting_comparer {
        get;
    }
    bool CanTarget(IEntity target);
}

public interface IStackEffect {
    void Resolve();
}

public interface ITimedEffect {
    TimePoint time_point { get; }
    void EndEffect(IEntity source, IEntity target = null);
}

public interface IHealth {
    int current_health {
        get;
    }
    ResourceStat health {
        get;
    }
    int Heal(IEntity source, int to_heal);
}

public interface IDamageable : IHealth {
    int TakeDamage(IEntity source, int damage);
}

public interface IDamages {
    int DealDamage(IDamageable target, int damage);
}

public interface IClickable {
    void OnMouseDown();
    void OnClick();
    void OnHoldDrag(GameObject dragged_to, Vector3 position_dragged_to);
    void OnFinishDrag(GameObject dragged_to, Vector3 position_dragged_to);
    void OnEndClick();
}

public interface ICombatant : IDamageable, IEntity {
    bool CanAttack(ICombatant target);
    bool CanBeAttacked(ICombatant attacker);
    int AddAttackRestriction(AttackRestriction attackRestriction);
    bool RemoveAttackRestriction(int id);
    void Freeze();
    void NoteAttack();

    int attacks_taken {
        get;
    }
    int attacks_per_turn {
        get;
    }

    bool retaliate {
        get; 
    }

    bool dead {
        get;
    }

    bool frozen {
        get;
    }

    Stat attack {
        get;
    }
}

public interface ITriggeredAbility {
    TriggerType type { get; }

    bool is_global { get; }
    bool is_local { get; }
    bool on_their_turn { get; }
    bool on_your_turn { get; }

    IEntity source { get; }

    bool TriggersFrom(TriggerInfo info);

    void Resolve(TriggerInfo info);

    bool ActiveInZone(Zone z);
}

public interface ICardSelectionEffect {
    void FinishResolve(IEntity source, Card selected, List<Card> cards);
}

public enum BuffType { basic, aura, timed }
public interface IBuff {
    BuffType buff_type { get; }
    IEntity source { get; }
}