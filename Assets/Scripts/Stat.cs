using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat {

    [SerializeField] int _base_value;
    [SerializeField] bool is_zero_minimum;

    public int value {
        get {
            if (is_zero_minimum) {
                return System.Math.Max(use_force_value ? force_value : base_value + buff_value, 0);
            } else {
                return use_force_value ? force_value : base_value + buff_value;
            }
        }
    }
    public int base_value { get { return _base_value; } }

    protected int force_value;
    protected bool use_force_value;

    protected int buff_value;

    public Stat(int base_value) {
        _base_value = base_value;
    }

    public virtual void ApplyBuff(int buff_value) {
        this.buff_value += buff_value;
    }

    public virtual void RemoveBuff(int buff_value) {
        this.buff_value -= buff_value;
    }

    public void SetValue(int value_to_set) {
        buff_value = value_to_set - base_value;
    }

    public void ForceValue(int force_value) {
        this.force_value = force_value;
        use_force_value = true;
    }

    public void ClearForceValue() {
        force_value = 0;
        use_force_value = false;
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

    public override void ApplyBuff(int buff_value) {
        base.ApplyBuff(buff_value);
        current_value += buff_value;
    }

    public override void RemoveBuff(int buff_value) {
        this.buff_value -= buff_value;
        if (current_value > value) {
            current_value = value;
        }
    }
}
