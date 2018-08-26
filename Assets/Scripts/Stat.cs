using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat {

    [SerializeField] int _base_value;

    public int value { get { return base_value + buff_value; } }
    public int base_value { get { return use_force_value ? force_value : base_value + buff_value; } }

    int force_value;
    bool use_force_value;

    int buff_value;

    public Stat(int base_value) {
        _base_value = base_value;
    }

    public void ApplyBuff(int buff_value) {
        this.buff_value += buff_value;
    }

    public void RemoveBuff(int buff_value) {
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
