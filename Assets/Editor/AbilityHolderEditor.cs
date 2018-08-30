using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(AbilityHolder))]
public class AbilityHolderEditor : Editor {

    TriggerType trigger_type;
    static Dictionary<GameObject, bool> show_triggers;
    SerializedProperty etb_triggers, after_spell_triggers, before_spell_triggers, after_attack_triggers;
    SerializedProperty static_abilities;

    public override void OnInspectorGUI() {
        serializedObject.Update();
        AbilityHolder abilities = (AbilityHolder)target;


        GUILayout.Space(4);

        // Static Abilities
        static_abilities = serializedObject.FindProperty("static_abilities");

        // Triggers 
        etb_triggers = serializedObject.FindProperty("etb_triggered_abilities");
        after_spell_triggers = serializedObject.FindProperty("after_spell_triggered_abilities");
        before_spell_triggers = serializedObject.FindProperty("before_spell_triggered_abilities");
        after_attack_triggers = serializedObject.FindProperty("after_attack_triggered_abilities");

        DisplayStaticAbilities(static_abilities);
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
            foreach (TriggerType type in (TriggerType[])System.Enum.GetValues(typeof(TriggerType))) {
                DisplayTriggers(type);
            }
            EditorGUI.indentLevel -= 2;
        }
        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(4);
    }

    void CreateNewTrigger(AbilityHolder ah) {
        SerializedProperty property = GetPropertyFromTriggerType(trigger_type);
        if (property != null) {
            property.InsertArrayElementAtIndex(property.arraySize);
        }
    }

    SerializedProperty GetPropertyFromTriggerType(TriggerType type) {
        if (type == TriggerType.enter_battlefield) {
            return etb_triggers;
        } else if (type == TriggerType.after_spell_resolves) {
            return after_spell_triggers;
        } else if (type == TriggerType.before_spell_resolves) {
            return before_spell_triggers;
        } else if (type == TriggerType.after_attack) {
            return after_attack_triggers;
        }
        return null;
    }

    string GetNameFromTriggerType(TriggerType type) {
        if (type == TriggerType.enter_battlefield) {
            return "ETB";
        } else if (type == TriggerType.after_spell_resolves) {
            return "After Spell";
        } else if (type == TriggerType.before_spell_resolves) {
            return "Before Spell";
        } else if (type == TriggerType.after_attack) {
            return "After Attack";
        }
        return null;
    }

    void DisplayTriggers(TriggerType type) {
        SerializedProperty triggers = GetPropertyFromTriggerType(type);
        if (triggers == null || triggers.arraySize == 0) {
            return;
        }
        for (int i = 0; i < triggers.arraySize; i++) {
            EditorGUILayout.PropertyField(triggers.GetArrayElementAtIndex(i), new GUIContent(GetNameFromTriggerType(type)), true);
        }
    }

    void DisplayStaticAbilities(SerializedProperty static_abilities) {
        EditorGUILayout.PropertyField(static_abilities, true);
    }
}
