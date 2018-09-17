using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CardCreationDisplayWindow : EditorWindow {

    CardCreationWindow creator;
    Texture2D card_frame;
    GUIStyle style;
    public void SetCreationWindow(CardCreationWindow window) {
        creator = window;
        card_frame = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Icons/CreatureTemplate.png", typeof(Texture2D));
    }

    public void LoadFrame(CreationType type) {
        if (type == CreationType.Creature) {
            card_frame = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Icons/CreatureTemplate.png", typeof(Texture2D));
        } else if (type == CreationType.Weapon) {
            card_frame = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Icons/WeaponTemplate.png", typeof(Texture2D));
        } else if (type == CreationType.Secret || type == CreationType.Spell) {
            card_frame = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Icons/SpellTemplate.png", typeof(Texture2D));
        }
    }

    public void OnGUI() {
        if (creator == null) return;

        Rect rect = GUILayoutUtility.GetRect(400, 600);

        // Card Frame


        SetStyle();

        if (creator.card_type == CardType.Creature) {

            if (creator.card_art != null) GUI.DrawTexture(new Rect(94, 30, 230, 300), creator.card_art.texture, ScaleMode.ScaleAndCrop);
            GUI.DrawTexture(new Rect(0, 0, 400, 600), card_frame, ScaleMode.ScaleToFit);

            DrawManaCost(25, 52, 3);
            DrawDescription(70, 370, 275, 150);
            DrawName(70, 305, 275, 30);
            DrawAttack(25, 485, 3);
            DrawHealth(315, 485, 3);
        } else if (creator.card_type == CardType.Weapon) {

            if (creator.card_art != null) GUI.DrawTexture(new Rect(80, 30, 260, 300), creator.card_art.texture, ScaleMode.ScaleAndCrop);
            GUI.DrawTexture(new Rect(0, 0, 400, 600), card_frame, ScaleMode.ScaleToFit);

            DrawManaCost(20, 48, 3);
            DrawDescription(70, 360, 275, 150);
            DrawName(70, 290, 275, 30);
            DrawAttack(25, 485, 3);
            DrawHealth(315, 485, 3);
        } else if (creator.card_type == CardType.Spell) {

            if (creator.card_art != null) GUI.DrawTexture(new Rect(58, 30, 300, 300), creator.card_art.texture, ScaleMode.ScaleAndCrop);
            GUI.DrawTexture(new Rect(0, 0, 400, 600), card_frame, ScaleMode.ScaleToFit);

            DrawManaCost(25, 52, 3);
            DrawDescription(70, 360, 275, 150);
            DrawName(70, 290, 275, 30);
        }
    }

    void DrawManaCost(int x, int y, int offset) {
        // Card Cost
        style.fontSize = 60;

        DrawOffset(x, y, 80, 60, 3, creator.card_health + "");

        EditorGUI.LabelField(new Rect(x, y, 80, 60), "<color=white>" + creator.card_cost + "</color>", style);
    }

    void DrawDescription(int x, int y, int width, int height) {
        style.fontSize = 17;

        DrawOffset(x, y, width, height, 2, creator.card_text + "");

        EditorGUI.LabelField(new Rect(x, y, width, height), "<color=white>" + creator.card_text + "</color>", style);
    }

    void DrawName(int x, int y, int width, int height) {
        style.fontSize = 20;

        DrawOffset(x, y, width, height, 2, creator.card_name + "");

        EditorGUI.LabelField(new Rect(x, y, width, height), "<color=white>" + creator.card_name + "</color>", style);
    }

    void DrawAttack(int x, int y, int offset) {
        style.fontSize = 60;

        DrawOffset(x, y, 80, 60, 3, creator.card_attack + "");

        EditorGUI.LabelField(new Rect(x, y, 80, 60), "<color=white>" + creator.card_attack + "</color>", style);
    }

    void DrawHealth(int x, int y, int offset) {
        style.fontSize = 60;

        DrawOffset(x, y, 80, 60, 3, creator.card_health + "");

        EditorGUI.LabelField(new Rect(x, y, 80, 60), "<color=white>" + creator.card_health + "</color>", style);
    }

    void SetStyle() {
        style = new GUIStyle(EditorStyles.textField);

        style.richText = true;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.background = null;
    }

    void DrawOffset(int x, int y, int width, int height, int offset, string text) {
        EditorGUI.LabelField(new Rect(x + offset, y, width, height), text, style);
        EditorGUI.LabelField(new Rect(x - offset, y, width, height), text, style);
        EditorGUI.LabelField(new Rect(x, y + offset, width, height), text, style);
        EditorGUI.LabelField(new Rect(x, y - offset, width, height), text, style);

        EditorGUI.LabelField(new Rect(x + offset, y + offset, width, height), text, style);
        EditorGUI.LabelField(new Rect(x + offset, y - offset, width, height), text, style);
        EditorGUI.LabelField(new Rect(x - offset, y - offset, width, height), text, style);
        EditorGUI.LabelField(new Rect(x - offset, y + offset, width, height), text, style);
    }
}
