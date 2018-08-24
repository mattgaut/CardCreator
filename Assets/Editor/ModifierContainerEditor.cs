using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModifierContainer))]
public class ModifierContainerEditor : Editor {
    Modifier mod_mask;
    public override void OnInspectorGUI() {
        SerializedProperty mod_property = serializedObject.FindProperty("base_mods");
        Modifier mod_mask = SetModMask(mod_property);
        mod_mask = (Modifier)EditorGUILayout.EnumMaskField("Mods: ", mod_mask);

        mod_property.ClearArray();
        string display_mods = "";

        foreach (Modifier mod in System.Enum.GetValues(typeof(Modifier))) {
            if (((int)mod_mask & (1 << ((int)mod - 1))) != 0 && System.Enum.IsDefined(typeof(Modifier), (int)mod)) {
                mod_property.InsertArrayElementAtIndex(0);
                mod_property.GetArrayElementAtIndex(0).intValue = (int)mod;
                display_mods += mod.ToString() + ", ";
            }
        }
        if (mod_property.arraySize > 0) {
            display_mods = display_mods.Substring(0, display_mods.Length - 2);
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.alignment = TextAnchor.UpperCenter;

            GUILayout.Box(display_mods);
        }

        EditorGUI.indentLevel += 1;

        SerializedProperty battlecry_property = serializedObject.FindProperty("_battlecry_info");
        if (HasMod(Modifier.battlecry, (int)mod_mask)) {
            EditorGUILayout.PropertyField(battlecry_property, new GUIContent("Battlecry Effects"), true);
        } else {
            battlecry_property.FindPropertyRelative("untargeted_effects").ClearArray();
            battlecry_property.FindPropertyRelative("untargeted_effects").ClearArray();
        }

        SerializedProperty overload_property = serializedObject.FindProperty("_overload_cost");
        if (HasMod(Modifier.overload, (int)mod_mask)) {
            overload_property.intValue = EditorGUILayout.IntField("Overload Cost: ", overload_property.intValue);
        } else {
            overload_property.intValue = 0;
        }

        SerializedProperty spellpower_amount = serializedObject.FindProperty("_spellpower_amount");
        if (HasMod(Modifier.spellpower, (int)mod_mask)) {
            spellpower_amount.intValue = EditorGUILayout.IntField("Spellpower: ", spellpower_amount.intValue);
        } else {
            spellpower_amount.intValue = 0;
        }
        EditorGUI.indentLevel -= 1;

        serializedObject.ApplyModifiedProperties();
    }

    private Modifier SetModMask(SerializedProperty property) {
        Modifier to_return = 0;
        for (int i = 0; i < property.arraySize; i++) {
            to_return += 1 << (property.GetArrayElementAtIndex(i).intValue - 1);
        }
        return to_return;
    }

    static bool HasMod(Modifier mod, int mod_mask) {
        return (mod_mask & (1 << ((int)mod - 1))) != 0;
    }
}
