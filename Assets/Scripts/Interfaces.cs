using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType { card = 1, player, hero_power }
public interface IEntity {
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
    int TakeDamage(IDamages source, int damage);
}



public interface IDamages : IEntity {
    int DealDamage(IDamageable target, int damage);
}

public interface IClickable {
    void OnMouseDown();
    void OnClick();
    void OnHoldDrag(GameObject dragged_to, Vector3 position_dragged_to);
    void OnFinishDrag(GameObject dragged_to, Vector3 position_dragged_to);
    void OnEndClick();
}

public interface ICombatant : IDamageable, IDamages {
    bool CanAttack(ICombatant target);
    bool CanBeAttacked(ICombatant attacker);
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

