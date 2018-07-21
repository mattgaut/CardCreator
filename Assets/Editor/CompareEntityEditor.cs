using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CompareEntity))]
public class CompareEntityEditor : PropertyDrawer {

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return 0f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, "Entity Comparer");
        if (property.isExpanded) {
            EditorGUI.indentLevel += 1;

            // Friendly or Enemy
            SerializedProperty child_property = property.FindPropertyRelative("friendly");
            child_property.boolValue = EditorGUILayout.Toggle("Friendly: ", child_property.boolValue);
            child_property = property.FindPropertyRelative("enemy");
            child_property.boolValue = EditorGUILayout.Toggle("Enemy: ", child_property.boolValue);

            // Types
            child_property = property.FindPropertyRelative("types");
            EntityType type_mask = (EntityType)SetMask(child_property);
            type_mask = (EntityType)EditorGUILayout.EnumMaskField("Types: ", type_mask);

            child_property.ClearArray();
            foreach (EntityType entity_type in System.Enum.GetValues(typeof(EntityType))) {
                if (InMask((int)type_mask, (int)entity_type) && System.Enum.IsDefined(typeof(EntityType), (int)entity_type)) {
                    child_property.InsertArrayElementAtIndex(0);
                    child_property.GetArrayElementAtIndex(0).intValue = (int)entity_type;
                }
            }

            child_property = property.FindPropertyRelative("card_types");
            CardType card_type_mask = (CardType)SetMask(child_property);
            if (InMask((int)type_mask, (int)EntityType.card)) {
                EditorGUILayout.BeginHorizontal();
                child_property = property.FindPropertyRelative("match_card_type");
                child_property.boolValue = true;
                child_property = property.FindPropertyRelative("card_types");
                card_type_mask = (CardType)EditorGUILayout.EnumMaskField("Card Types: ", card_type_mask);

                child_property.ClearArray();
                foreach (CardType card_type in System.Enum.GetValues(typeof(CardType))) {
                    if (InMask((int)card_type_mask, (int)card_type) && System.Enum.IsDefined(typeof(CardType), (int)card_type)) {
                        child_property.InsertArrayElementAtIndex(0);
                        child_property.GetArrayElementAtIndex(0).intValue = (int)card_type;
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
                if (InMask((int)card_type_mask, (int)CardType.Creature) || InMask((int)type_mask, (int)EntityType.player)) {
                    // Check Health
                    SetHealth(property);
                    // Check Attack
                    SetAttack(property);
                } else {
                    ClearAttack(property);
                    ClearHealth(property);
                }
                if (InMask((int)type_mask, (int)EntityType.card) || InMask((int)type_mask, (int)EntityType.hero_power)) {
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
            to_return += 1 << (property.GetArrayElementAtIndex(i).intValue - 1);
        }
        return to_return;
    }
    static bool InMask(int mask, int check) {
        return (mask & (1 << (check - 1))) != 0;
    }
}
