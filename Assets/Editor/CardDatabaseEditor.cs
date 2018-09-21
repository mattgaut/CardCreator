using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardDatabase))]
public class CardDatabaseEditor : Editor {

    bool deleted_or_created;

    int add_id;
    Card add_card;

    string search_string;
    int search_int;
    bool search;
    bool use_int_search;

    public override void OnInspectorGUI() {
        serializedObject.Update();

        SerializedProperty card_list = serializedObject.FindProperty("cards");

        search_string = EditorGUILayout.TextField(search_string);
        if (search_string != null && search_string != "") {
            use_int_search = int.TryParse(search_string, out search_int);
            search = true;
        }  else {
            use_int_search = false;
            search = false;
        }

        EditorGUILayout.LabelField("     IDs   Cards");

        deleted_or_created = false;
        for (int i = card_list.arraySize - 1; i >= 0; i--) {
            SerializedProperty card = card_list.GetArrayElementAtIndex(i);
            if (search) {
                if (use_int_search && !(card.FindPropertyRelative("id").intValue + "").Contains(search_string)) {
                    continue;
                } else if (!use_int_search && !(card.FindPropertyRelative("card").objectReferenceValue as Card).card_name.Contains(search_string)) {
                    continue;
                }
            }
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("X", GUILayout.MaxWidth(20))) {
                card_list.DeleteArrayElementAtIndex(i);              

                deleted_or_created = true;
            } else {
                EditorGUILayout.LabelField(card.FindPropertyRelative("id").intValue + "", GUILayout.MaxWidth(30));

                GUI.enabled = false;
                EditorGUILayout.ObjectField(card.FindPropertyRelative("card"), GUIContent.none);
                GUI.enabled = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        GUI.enabled = !(serializedObject.targetObject as CardDatabase).IsIdTaken(add_id) && add_id != 0 && add_card != null;
        if (GUILayout.Button("+", GUILayout.MaxWidth(20))) {
            card_list.InsertArrayElementAtIndex(card_list.arraySize);
            card_list.GetArrayElementAtIndex(card_list.arraySize - 1).FindPropertyRelative("card").objectReferenceValue = add_card;
            card_list.GetArrayElementAtIndex(card_list.arraySize - 1).FindPropertyRelative("id").intValue = add_id;

            add_card.SetID(add_id);

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
