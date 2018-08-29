using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AfterAttackTriggeredAbility))]
public class AfterAttackTriggerEditor : TriggerEditor {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, "After Attack Trigger");
        if (property.isExpanded) {
            EditorGUI.indentLevel += 1;

            SerializedProperty child_property = property.FindPropertyRelative("need_to_kill_attacked");
            child_property.boolValue = EditorGUILayout.Toggle(new GUIContent("On Kill?"), child_property.boolValue);

            child_property = property.FindPropertyRelative("damage_needed");
            child_property.intValue = EditorGUILayout.IntField(new GUIContent("Damage: "), child_property.intValue);

            BaseTriggerGUI(property);

            EditorGUI.indentLevel -= 1;
        }
        EditorGUI.EndProperty();
    }
}
