using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Stat), true)]
public class StatPropertyDrawer : PropertyDrawer {

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return 0f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label.text);
        if (property.isExpanded) {
            EditorGUI.indentLevel += 1;

            SerializedProperty child_property = property.FindPropertyRelative("_base_value");
            child_property.intValue = EditorGUILayout.IntField(new GUIContent("Base Value"), child_property.intValue);

            child_property = property.FindPropertyRelative("is_zero_minimum");
            child_property.boolValue = EditorGUILayout.Toggle(new GUIContent("Is Zero Min?"), child_property.boolValue);

            EditorGUI.indentLevel -= 1;
        }
        EditorGUI.EndProperty();
    }
}
