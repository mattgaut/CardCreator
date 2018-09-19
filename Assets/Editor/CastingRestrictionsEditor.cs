using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CastingRestrictions))]
public class CastingRestrictionsEditor : PropertyDrawer {

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return 0f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label.text);
        if (property.isExpanded) {
            EditorGUI.indentLevel += 1;

            // Require Enemy Minions
            SerializedProperty child_property = property.FindPropertyRelative("require_enemy_minions");
            child_property.boolValue = EditorGUILayout.Toggle(new GUIContent("Enemy Minions?"), child_property.boolValue);
            if (child_property.boolValue) {
                child_property = property.FindPropertyRelative("enemy_minions_required");
                child_property.intValue = EditorGUILayout.IntField(new GUIContent("Number"), child_property.intValue);
            }
            EditorGUI.indentLevel -= 1;
        }
        EditorGUI.EndProperty();
    }
}
