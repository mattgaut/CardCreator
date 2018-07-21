using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spell))]
public class SpellEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
    }
}
