using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ETBTriggeredAbility))]
public class ETBTriggerEditor : TriggerEditor {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, "ETBTrigger");
        if (property.isExpanded) {
            EditorGUI.indentLevel += 1;

            SerializedProperty child_property = property.FindPropertyRelative("compare");
            EditorGUILayout.PropertyField(child_property, new GUIContent("Matching"));

            BaseTriggerGUI(property);

            EditorGUI.indentLevel -= 1;
        }
        EditorGUI.EndProperty();
    }


}
