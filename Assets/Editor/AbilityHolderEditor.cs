using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(AbilityHolder))]
public class AbilityHolderEditor : Editor {

    TriggerType trigger_type;
    static Dictionary<GameObject, bool> show_triggers;
    SerializedProperty etb_triggers_property;

    public override void OnInspectorGUI() {
        serializedObject.Update();
        AbilityHolder abilities = (AbilityHolder)target;


        GUILayout.Space(4);

        // Triggers 
        etb_triggers_property = serializedObject.FindProperty("etb_triggered_abilities");

        if (show_triggers == null) {
            show_triggers = new Dictionary<GameObject, bool>();
        }
        if (!show_triggers.ContainsKey(abilities.gameObject)) {
            show_triggers.Add(abilities.gameObject, false);
        }
        show_triggers[abilities.gameObject] = EditorGUILayout.Foldout(show_triggers[abilities.gameObject], "Triggers");
        if (show_triggers[abilities.gameObject]) {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add New Trigger")) {
                CreateNewTrigger(abilities);
            }
            trigger_type = (TriggerType)EditorGUILayout.EnumPopup(trigger_type);
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel += 2;
            for (int i = 0; i < etb_triggers_property.arraySize; i++) {
                EditorGUILayout.PropertyField(etb_triggers_property.GetArrayElementAtIndex(i), new GUIContent("ETB Trigger"), true);
            }

            EditorGUI.indentLevel -= 2;
        }
        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(4);
    }

    void CreateNewTrigger(AbilityHolder ah) {
        if (trigger_type == TriggerType.enter_battlefield) {
            etb_triggers_property.InsertArrayElementAtIndex(etb_triggers_property.arraySize);
        }
    }
}
