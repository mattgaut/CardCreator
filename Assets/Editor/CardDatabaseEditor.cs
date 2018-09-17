using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardDatabase))]
public class CardDatabaseEditor : Editor {

    bool deleted;

    public override void OnInspectorGUI() {
        SerializedProperty id_list = serializedObject.FindProperty("ids");
        SerializedProperty card_list = serializedObject.FindProperty("cards");

        EditorGUILayout.LabelField("     IDs  Cards");

        deleted = false;
        for (int i = id_list.arraySize - 1; i >= 0; i--) {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("X", GUILayout.MaxWidth(20))) {
                card_list.GetArrayElementAtIndex(i).objectReferenceValue = null;
                card_list.DeleteArrayElementAtIndex(i);
                id_list.DeleteArrayElementAtIndex(i);
                

                deleted = true;
            } else {
                EditorGUILayout.LabelField(id_list.GetArrayElementAtIndex(i).intValue + "", GUILayout.MaxWidth(30));

                GUI.enabled = false;
                EditorGUILayout.ObjectField(card_list.GetArrayElementAtIndex(i), GUIContent.none);
                GUI.enabled = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
        if (deleted) (serializedObject.targetObject as CardDatabase).ReloadDictionary();
    }

}
