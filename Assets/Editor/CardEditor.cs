using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Card), true)]
public class CardEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
    }
}
