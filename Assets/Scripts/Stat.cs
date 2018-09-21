using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat {

    [SerializeField] int _base_value;
    [SerializeField] bool is_zero_minimum;

    [SerializeField] protected List<StatBuff> buffs;
    [SerializeField] protected List<StatBuff> sets_stack;

    public int value {
        get {
            if (is_zero_minimum) {
                return Math.Max(sets_stack.Count > 0 ? sets_stack[sets_stack.Count - 1].buff_value + buff_value : base_value + buff_value, 0);
            } else {
                return sets_stack.Count > 0 ? sets_stack[sets_stack.Count - 1].buff_value + buff_value : base_value + buff_value;
            }
        }
    }
    public int base_value { get { return _base_value; } }

    protected int buff_value;

    public Stat(int base_value) {
        buffs = new List<StatBuff>();
        sets_stack = new List<StatBuff>();
        _base_value = base_value;
    }

    public virtual void ApplyBuff(StatBuff buff) {
        buff_value += buff.buff_value;
        buffs.Add(buff);
    }

    public virtual void RemoveBuff(StatBuff buff) {
        if (buffs.Contains(buff)) {
            buff_value -= buff.buff_value;
            buffs.Remove(buff);
        }
    }

    public virtual void AddSetBuff(StatBuff buff) {
        ClearNonAuraBuffs();
        sets_stack.Add(buff);
    }

    public virtual void RemoveSetBuff(StatBuff buff) {
        sets_stack.Remove(buff);
    }

    public void ClearNonAuraBuffs() {
        for (int i = buffs.Count - 1; i >= 0; i--) {
            if (buffs[i].buff_type != StatBuff.Type.aura) {
                RemoveBuff(buffs[i]);
            }
        }
        for (int i = sets_stack.Count - 1; i >= 0; i--) {
            if (sets_stack[i].buff_type != StatBuff.Type.aura) {
                RemoveSetBuff(buffs[i]);
            }
        }
    }

    public static implicit operator int(Stat s){
        return s.value;
    }
}

[System.Serializable]
public class ResourceStat : Stat {

    public int current_value {
        get { return _current_value; }
        set {
            _current_value = value;
            if (_current_value > this.value) {
                _current_value = this.value;
            }
        }
    }

    int _current_value;

    public ResourceStat(int base_value) : base(base_value) {
        current_value = base_value;
    }

    public override void ApplyBuff(StatBuff buff) {
        base.ApplyBuff(buff);
        if (buff.buff_value > 0) {
            current_value += buff_value;
        } else if (current_value > value) {
            current_value = value;
        }
    }

    public override void RemoveBuff(StatBuff buff) {
        buff_value -= buff.buff_value;
        if (current_value > value) {
            current_value = value;
        }
    }

    public override void AddSetBuff(StatBuff buff) {
        int difference = value - current_value;
        base.AddSetBuff(buff);

        if (current_value > value) {
            current_value = value;
        }
        // If the difference between the new values are larger make the difference equal to the old difference
        if (value - current_value > difference) {
            current_value = value - difference;
        }
    }

    public override void RemoveSetBuff(StatBuff buff) {
        base.RemoveSetBuff(buff);
    }
}

[Serializable]
public class StatBuff : IEqualityComparer<StatBuff>, IEquatable<StatBuff> {
    public enum Type { basic, aura, timed }

    public int buff_value { get; private set; }
    public Type buff_type { get; private set; }
    public IEntity source { get; private set; }

    public StatBuff(IEntity source, Type type, int value) {
        this.source = source;
        buff_type = type;
        buff_value = value;
    }

    public bool Equals(StatBuff x, StatBuff y) {
        return x.buff_value == y.buff_value && x.buff_type == y.buff_type && x.source == y.source;
    }

    public int GetHashCode(StatBuff obj) {
        return obj.GetHashCode();
    }

    public override bool Equals(object obj) {
        StatBuff other = obj as StatBuff;
        if (other == null) {
            return false;
        }
        return Equals(this, other);
    }

    public override int GetHashCode() {
        return buff_value.GetHashCode() * buff_type.GetHashCode();;
    }

    public bool Equals(StatBuff other) {
        return Equals(this, other);
    }
}