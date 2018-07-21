using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CardCreationDisplayWindow : EditorWindow {

    CardCreationWindow creator;
    Texture2D card_frame;
    public void SetCreationWindow(CardCreationWindow window) {
        creator = window;
        card_frame = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Icons/BlankCreature.png", typeof(Texture2D));
    }

    public void LoadFrame(CardType type) {
        if (type == CardType.Creature) {
            card_frame = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Icons/BlankCreature.png", typeof(Texture2D));
        } else {

        }
    }

    public void OnGUI() {
        if (creator == null) return;

        if (creator.card_type == CardType.Creature) {
            LoadCreature();
        }
    }

    void LoadCreature() {
        EditorGUILayout.LabelField("Creature");
        Rect rect = GUILayoutUtility.GetRect(400, 600);

        // Card Art
        if (creator.card_art != null) GUI.DrawTexture(new Rect(88, 30, 230, 300), creator.card_art.texture, ScaleMode.ScaleAndCrop);

        // Card Frame
        GUI.DrawTexture(new Rect(0, 0, 400, 600), card_frame, ScaleMode.ScaleToFit);

        // Card Description
        GUIStyle style = new GUIStyle(EditorStyles.textField);
        style.richText = true;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 17;
        style.normal.background = null;

        EditorGUI.LabelField(new Rect(70, 380, 275, 150), creator.card_text, style);

        // Card Name
        style.fontSize = 20;

        EditorGUI.LabelField(new Rect(70, 315, 275, 30), creator.card_name, style);

        // Card Cost
        style.fontSize = 60;

        int x = 15, y = 48, offset = 3;
        EditorGUI.LabelField(new Rect(x + offset, y, 80, 60), creator.card_cost + "", style);
        EditorGUI.LabelField(new Rect(x - offset, y, 80, 60), creator.card_cost + "", style);
        EditorGUI.LabelField(new Rect(x, y + offset, 80, 60), creator.card_cost + "", style);
        EditorGUI.LabelField(new Rect(x, y - offset, 80, 60), creator.card_cost + "", style);

        EditorGUI.LabelField(new Rect(x + offset, y + offset, 80, 60), creator.card_cost + "", style);
        EditorGUI.LabelField(new Rect(x + offset, y - offset, 80, 60), creator.card_cost + "", style);
        EditorGUI.LabelField(new Rect(x - offset, y - offset, 80, 60), creator.card_cost + "", style);
        EditorGUI.LabelField(new Rect(x - offset, y + offset, 80, 60), creator.card_cost + "", style);

        EditorGUI.LabelField(new Rect(x, y, 80, 60), "<color=white>" + creator.card_cost + "</color>", style);

        style.fontSize = 60;


        // Card Attack
        x = 15; y = 495; offset = 3;
        EditorGUI.LabelField(new Rect(x + offset, y, 80, 60), creator.card_attack + "", style);
        EditorGUI.LabelField(new Rect(x - offset, y, 80, 60), creator.card_attack + "", style);
        EditorGUI.LabelField(new Rect(x, y + offset, 80, 60), creator.card_attack + "", style);
        EditorGUI.LabelField(new Rect(x, y - offset, 80, 60), creator.card_attack + "", style);

        EditorGUI.LabelField(new Rect(x + offset, y + offset, 80, 60), creator.card_attack + "", style);
        EditorGUI.LabelField(new Rect(x + offset, y - offset, 80, 60), creator.card_attack + "", style);
        EditorGUI.LabelField(new Rect(x - offset, y - offset, 80, 60), creator.card_attack + "", style);
        EditorGUI.LabelField(new Rect(x - offset, y + offset, 80, 60), creator.card_attack + "", style);

        EditorGUI.LabelField(new Rect(x, y, 80, 60), "<color=white>" + creator.card_attack + "</color>", style);

        // Card Health
        x = 315; y = 495; offset = 3;
        EditorGUI.LabelField(new Rect(x + offset, y, 80, 60), creator.card_health + "", style);
        EditorGUI.LabelField(new Rect(x - offset, y, 80, 60), creator.card_health + "", style);
        EditorGUI.LabelField(new Rect(x, y + offset, 80, 60), creator.card_health + "", style);
        EditorGUI.LabelField(new Rect(x, y - offset, 80, 60), creator.card_health + "", style);

        EditorGUI.LabelField(new Rect(x + offset, y + offset, 80, 60), creator.card_health + "", style);
        EditorGUI.LabelField(new Rect(x + offset, y - offset, 80, 60), creator.card_health + "", style);
        EditorGUI.LabelField(new Rect(x - offset, y - offset, 80, 60), creator.card_health + "", style);
        EditorGUI.LabelField(new Rect(x - offset, y + offset, 80, 60), creator.card_health + "", style);

        EditorGUI.LabelField(new Rect(x, y, 80, 60), "<color=white>" + creator.card_health + "</color>", style);
    }
}
