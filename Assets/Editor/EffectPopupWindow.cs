using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class EffectPopupWindow : PopupWindowContent {

    Vector2 size;
    string search_string;

    Type effect_type;
    string folder_path;

    Action<Type> on_click;

    public EffectPopupWindow(Vector2 size, string folder_path, Action<Type> action) {
        this.folder_path = folder_path;
        this.size = size;
        on_click = action;
    }

    public override void OnOpen() {
        effect_type = null;
    }
    public override Vector2 GetWindowSize() {
        return size;
    }

    public override void OnGUI(Rect rect) {
        search_string = EditorGUILayout.TextField("Search: ", search_string);
        string[] assets = AssetDatabase.FindAssets(search_string + " t:Script", new string[] { folder_path });
        for (int i = 0; i < assets.Length; i++) {
            string path_name = AssetDatabase.GUIDToAssetPath(assets[i]);
            string[] path_name_split = path_name.Split('.', '/');
            string class_name = path_name_split[path_name_split.Length - 2];
            if (GUILayout.Button(class_name)) {
                effect_type = AssetDatabase.LoadAssetAtPath<MonoScript>(path_name).GetClass();
                on_click(effect_type);
                editorWindow.Close();
            }
        }
    }
}
