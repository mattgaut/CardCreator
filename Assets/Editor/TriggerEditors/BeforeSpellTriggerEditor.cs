﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BeforeSpellTriggeredAbility))]
public class BeforeSpellTriggerEditor : TriggerEditor {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, "Before Spell Trigger");
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