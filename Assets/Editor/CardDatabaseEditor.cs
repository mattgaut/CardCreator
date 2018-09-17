using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardDatabase))]
public class CardDatabaseEditor : Editor {

    bool deleted_or_created;

    int add_id;
    Card add_card;

    public override void OnInspectorGUI() {
        serializedObject.Update();

        SerializedProperty id_list = serializedObject.FindProperty("ids");
        SerializedProperty card_list = serializedObject.FindProperty("cards");

        EditorGUILayout.LabelField("     IDs   Cards");

        deleted_or_created = false;
        for (int i = id_list.arraySize - 1; i >= 0; i--) {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("X", GUILayout.MaxWidth(20))) {
                card_list.GetArrayElementAtIndex(i).objectReferenceValue = null;
                card_list.DeleteArrayElementAtIndex(i);
                id_list.DeleteArrayElementAtIndex(i);
                

                deleted_or_created = true;
            } else {
                EditorGUILayout.LabelField(id_list.GetArrayElementAtIndex(i).intValue + "", GUILayout.MaxWidth(30));

                GUI.enabled = false;
                EditorGUILayout.ObjectField(card_list.GetArrayElementAtIndex(i), GUIContent.none);
                GUI.enabled = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        GUI.enabled = !(serializedObject.targetObject as CardDatabase).IsIdTaken(add_id) && add_id != 0 && add_card != null;
        if (GUILayout.Button("+", GUILayout.MaxWidth(20))) {
            id_list.InsertArrayElementAtIndex(id_list.arraySize);
            id_list.GetArrayElementAtIndex(id_list.arraySize - 1).intValue = add_id;

            card_list.InsertArrayElementAtIndex(card_list.arraySize);
            card_list.GetArrayElementAtIndex(card_list.arraySize - 1).objectReferenceValue = add_card;

            add_id = 0;
            add_card = null;

            deleted_or_created = true;
        }
        GUI.enabled = true;

        add_id = EditorGUILayout.IntField(add_id, GUILayout.MaxWidth(30));
        add_card = (Card)EditorGUILayout.ObjectField(GUIContent.none, add_card, typeof(Card), false);

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
        if (deleted_or_created) (serializedObject.targetObject as CardDatabase).ReloadDictionary();
    }
}
