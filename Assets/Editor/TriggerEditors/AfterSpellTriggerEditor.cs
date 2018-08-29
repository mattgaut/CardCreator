using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AfterSpellTriggeredAbility))]
public class AfterSpellTriggerEditor : TriggerEditor {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, "After Spell Trigger");
        if (property.isExpanded) {
            EditorGUI.indentLevel += 1;

            BaseTriggerGUI(property);

            SerializedProperty child_property = property.FindPropertyRelative("compare");
            EditorGUILayout.PropertyField(child_property, new GUIContent("Matching"));

            EditorGUI.indentLevel -= 1;
        }
        EditorGUI.EndProperty();
    }
}
