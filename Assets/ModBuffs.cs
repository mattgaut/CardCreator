using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModBuff : IBuff, IEqualityComparer<ModBuff>, IEquatable<ModBuff> {

    public Modifier buff_value { get; private set; }
    public BuffType buff_type { get; private set; }
    public IEntity source { get; private set; }

    public ModBuff(IEntity source, BuffType type, Modifier value) {
        this.source = source;
        buff_type = type;
        buff_value = value;
    }

    public bool Equals(ModBuff x, ModBuff y) {
        return x.buff_value == y.buff_value && x.buff_type == y.buff_type && x.source == y.source;
    }

    public int GetHashCode(ModBuff obj) {
        return obj.GetHashCode();
    }

    public override bool Equals(object obj) {
        ModBuff other = obj as ModBuff;
        if (other == null) {
            return false;
        }
        return Equals(this, other);
    }

    public override int GetHashCode() {
        return buff_value.GetHashCode() * buff_type.GetHashCode(); ;
    }

    public bool Equals(ModBuff other) {
        return Equals(this, other);
    }
}
