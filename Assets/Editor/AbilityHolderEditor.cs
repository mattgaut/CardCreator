using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(AbilityHolder))]
public class AbilityHolderEditor : Editor {

    SerializedProperty static_abilities, triggered_abilities;

    public override void OnInspectorGUI() {
        serializedObject.Update();
        AbilityHolder abilities = (AbilityHolder)target;


        GUILayout.Space(4);

        // Static Abilities
        static_abilities = serializedObject.FindProperty("static_abilities");

        // Triggered Ailities
        triggered_abilities = serializedObject.FindProperty("triggered_abilities");

        EditorGUILayout.PropertyField(static_abilities, true);

        EditorGUILayout.PropertyField(triggered_abilities, true);

        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(4);
    }

    void DisplayStaticAbilities(SerializedProperty static_abilities) {
    }
}
