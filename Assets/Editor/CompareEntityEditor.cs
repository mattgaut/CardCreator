using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(CompareEntity))]
public class CompareEntityEditor : PropertyDrawer {

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return 0f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label.text);
        if (property.isExpanded) {
            EditorGUI.indentLevel += 1;

            // Friendly or Enemy
            SerializedProperty child_property = property.FindPropertyRelative("friendly");
            child_property.boolValue = EditorGUILayout.Toggle("Friendly: ", child_property.boolValue);
            child_property = property.FindPropertyRelative("enemy");
            child_property.boolValue = EditorGUILayout.Toggle("Enemy: ", child_property.boolValue);

            // Types
            EntityType[] type_enum_array = (EntityType[])Enum.GetValues(typeof(EntityType));
            child_property = property.FindPropertyRelative("types");
            EntityType type_mask = (EntityType)SetMask(child_property);
            
            type_mask = (EntityType)EditorGUILayout.EnumMaskField("Types: ", type_mask);

            child_property.ClearArray();
            for (int i = 0;  i < type_enum_array.Length; i++) {
                if (InMask((int)type_mask, i)) {
                    child_property.InsertArrayElementAtIndex(0);
                    child_property.GetArrayElementAtIndex(0).intValue = (int)type_enum_array[i];
                }
            }

            child_property = property.FindPropertyRelative("card_types");
            CardType[] card_type_enum_array = (CardType[])Enum.GetValues(typeof(CardType));
            CardType card_type_mask = (CardType)SetMask(child_property);
            if (InMask((int)type_mask, (Array.IndexOf(type_enum_array, EntityType.card)))) {
                EditorGUILayout.BeginHorizontal();
                child_property = property.FindPropertyRelative("match_card_type");
                child_property.boolValue = true;


                child_property = property.FindPropertyRelative("card_types");
                card_type_mask = (CardType)EditorGUILayout.EnumMaskField("Card Types: ", card_type_mask);

                child_property.ClearArray();
                for (int i = 0; i < card_type_enum_array.Length; i++) {
                    if (InMask((int)card_type_mask, i)) {
                        child_property.InsertArrayElementAtIndex(0);
                        child_property.GetArrayElementAtIndex(0).intValue = (int)card_type_enum_array[i];
                    }
                }
                EditorGUILayout.EndHorizontal();
            } else {
                child_property = property.FindPropertyRelative("match_card_type");
                child_property.boolValue = false;
                child_property = property.FindPropertyRelative("card_types");
                child_property.ClearArray();
            }

            if (type_mask != 0) {
                if (InMask((int)card_type_mask, Array.IndexOf(card_type_enum_array, CardType.Creature)) || InMask((int)type_mask, Array.IndexOf(type_enum_array, EntityType.player))) {
                    // Check Health
                    SetHealth(property);
                    // Check Attack
                    SetAttack(property);
                    // Undamaged
                    child_property = property.FindPropertyRelative("undamaged");
                    child_property.boolValue = EditorGUILayout.Toggle(new GUIContent("Undamaged: "), child_property.boolValue);
                    // Damaged
                    child_property = property.FindPropertyRelative("damaged");
                    child_property.boolValue = EditorGUILayout.Toggle(new GUIContent("Damaged: "), child_property.boolValue);
                } else {
                    ClearAttack(property);
                    ClearHealth(property);

                    property.FindPropertyRelative("damaged").boolValue = false;
                    property.FindPropertyRelative("undamaged").boolValue = false;
                }

                // Creature Types
                child_property = property.FindPropertyRelative("creature_types");
                if (InMask((int)card_type_mask, Array.IndexOf(card_type_enum_array, CardType.Creature))) {
                    Creature.CreatureType[] creature_enum_array = (Creature.CreatureType[])Enum.GetValues(typeof(Creature.CreatureType));
                    Creature.CreatureType creature_type_mask = (Creature.CreatureType)SetMask(child_property);
                    creature_type_mask = (Creature.CreatureType)EditorGUILayout.EnumMaskField("Creature Types: ", creature_type_mask);
                    child_property.ClearArray();
                    for (int i = 0; i < creature_enum_array.Length; i++) {
                        if (InMask((int)creature_type_mask, i)) {
                            child_property.InsertArrayElementAtIndex(0);
                            child_property.GetArrayElementAtIndex(0).intValue = (int)creature_enum_array[i];
                        }
                    }
                } else {
                    child_property.ClearArray();
                }
                if (InMask((int)type_mask, Array.IndexOf(card_type_enum_array, CardType.Creature)) || InMask((int)type_mask, Array.IndexOf(type_enum_array, EntityType.hero_power))) {
                    // Check Cost
                    SetManaCost(property);
                } else {
                    ClearManaCost(property);
                }
            } else {
                ClearManaCost(property);
                ClearAttack(property);
                ClearHealth(property);
            }


            EditorGUI.indentLevel -= 1;
        }
        EditorGUI.EndProperty();
    }

    private void SetManaCost(SerializedProperty property) {
        EditorGUILayout.BeginHorizontal();
        SerializedProperty child_property = property.FindPropertyRelative("check_mana_cost");
        child_property.boolValue = EditorGUILayout.Toggle("Cost: ", child_property.boolValue);
        if (child_property.boolValue) {
            child_property = property.FindPropertyRelative("mana_cost_range");
            child_property.intValue = (int)(Range)EditorGUILayout.EnumPopup((Range)child_property.intValue);
            child_property = property.FindPropertyRelative("mana_cost");
            child_property.intValue = EditorGUILayout.IntField(child_property.intValue);
        }
        EditorGUILayout.EndHorizontal();
    }
    private void SetAttack(SerializedProperty property) {
        EditorGUILayout.BeginHorizontal();
        SerializedProperty child_property = property.FindPropertyRelative("check_attack");
        child_property.boolValue = EditorGUILayout.Toggle("Attack: ", child_property.boolValue);
        if (child_property.boolValue) {
            child_property = property.FindPropertyRelative("attack_range");
            child_property.intValue = (int)(Range)EditorGUILayout.EnumPopup((Range)child_property.intValue);
            child_property = property.FindPropertyRelative("attack");
            child_property.intValue = EditorGUILayout.IntField(child_property.intValue);
        }
        EditorGUILayout.EndHorizontal();
    }
    private void SetHealth(SerializedProperty property) {
        EditorGUILayout.BeginHorizontal();
        SerializedProperty child_property = property.FindPropertyRelative("check_health");
        child_property.boolValue = EditorGUILayout.Toggle("Health: ", child_property.boolValue);
        if (child_property.boolValue) {
            child_property = property.FindPropertyRelative("health_range");
            child_property.intValue = (int)(Range)EditorGUILayout.EnumPopup((Range)child_property.intValue);
            child_property = property.FindPropertyRelative("health");
            child_property.intValue = EditorGUILayout.IntField(child_property.intValue);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void ClearManaCost(SerializedProperty property) {
        SerializedProperty child_property = property.FindPropertyRelative("check_mana_cost");
        child_property.boolValue = false;
        child_property = property.FindPropertyRelative("mana_cost_range");
        child_property.intValue = (int)Range.Equal;
        child_property = property.FindPropertyRelative("mana_cost");
        child_property.intValue = 0;
    }
    private void ClearAttack(SerializedProperty property) {
        SerializedProperty child_property = property.FindPropertyRelative("check_attack");
        child_property.boolValue = false;
        child_property = property.FindPropertyRelative("attack_range");
        child_property.intValue = (int)Range.Equal;
        child_property = property.FindPropertyRelative("attack");
        child_property.intValue = 0;
    }
    private void ClearHealth(SerializedProperty property) {
        SerializedProperty child_property = property.FindPropertyRelative("check_health");
        child_property.boolValue = false;
        child_property = property.FindPropertyRelative("health_range");
        child_property.intValue = (int)Range.Equal;
        child_property = property.FindPropertyRelative("health");
        child_property.intValue = 0;
    }

    private int SetMask(SerializedProperty property) {
        int to_return = 0;
        for (int i = 0; i < property.arraySize; i++) {
            to_return += 1 << (property.GetArrayElementAtIndex(i).enumValueIndex);
        }
        return to_return;
    }
    bool InMask(int mask, int check) {
        return (mask & (1 << (check))) != 0;
    }
    bool InCreatureMask(int mask, int check) {
        return (mask & (1 << (check))) != 0;
    }
}
