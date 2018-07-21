using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TriggeredAbility))]
public class TriggerEditor : PropertyDrawer {

    protected void BaseTriggerGUI(SerializedProperty property) {
        // Subscribed Zones        
        property.FindPropertyRelative("subscried_zones");

        SerializedProperty zones_property = property.FindPropertyRelative("subscribed_zones");
        Zone zoneMask = SetZoneMask(zones_property);
        zoneMask = (Zone)EditorGUILayout.EnumMaskField("Zones: ", zoneMask);

        zones_property.ClearArray();
        foreach (Zone zone in System.Enum.GetValues(typeof(Zone))) {
            if ((zoneMask & zone) != 0 && System.Enum.IsDefined(typeof(Zone), (int)zone)) {
                zones_property.InsertArrayElementAtIndex(0);
                zones_property.GetArrayElementAtIndex(0).intValue = (int)zone;
            }
        }

        // Effect List
        SerializedProperty effects_property = property.FindPropertyRelative("effects");
        EditorGUILayout.LabelField("Effects: ");

        EditorGUI.indentLevel += 1;
        effects_property.arraySize = EditorGUILayout.DelayedIntField("Size: ", effects_property.arraySize);
        for (int i = 0; i < effects_property.arraySize; i++) {
            EditorGUILayout.PropertyField(effects_property.GetArrayElementAtIndex(i), GUIContent.none);
        }
        EditorGUI.indentLevel -= 1;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) { return 0f; }

    Zone SetZoneMask(SerializedProperty zone_list) {
        Zone zone = 0;
        for (int i = 0; i < zone_list.arraySize; i++) {
            zone += zone_list.GetArrayElementAtIndex(i).intValue;
        }
        return zone;
    }
}
