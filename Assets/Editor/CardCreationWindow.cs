
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CardCreationWindow : EditorWindow {

    [MenuItem("Window/Card Creator")]
    public static void ShowWindow() {
        CardCreationWindow ccw = (CardCreationWindow)GetWindow(typeof(CardCreationWindow));
        CardCreationDisplayWindow ccdw = (GetWindow(typeof(CardCreationDisplayWindow)) as CardCreationDisplayWindow);
        ccw.ccdw = ccdw;
        ccdw.SetCreationWindow(ccw);
        ccw.LoadCardType(CardType.Creature);
    }

    public CardType card_type { get; private set; }

    public string card_name { get; private set; }
    public string card_text { get; private set; }
    public int card_cost { get; private set; }
    public int card_attack { get; private set; }
    public int card_health { get; private set; }

    public bool enable_rich_text { get; private set; }
    public Sprite card_art { get; private set; }

    public AbilityHolder abilities { get; private set; }

    public CardCreationDisplayWindow ccdw;

    GameObject loaded_card;
    SerializedObject card_object;

    public Rect windowRect = new Rect(100, 100, 200, 200);

    private Rect card_panel;
    private Rect effects_panel;
    private Rect resizer;
    private Rect popup_rect;

    float size_ratio = 0.5f;
    float offset = 5f;

    bool is_resizing;

    GUIStyle resizer_style;

    Editor ability_holder_editor;
    Editor mod_editor;
    string search_string;

    Dictionary<Effect, bool> effect_foldouts;

    private void OnEnable() {
        resizer_style = new GUIStyle();
        resizer_style.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;
        effect_foldouts = new Dictionary<Effect, bool>();
    }

    void OnGUI() {
        DrawCardPanel();
        DrawEffectsPanel();
        DrawResizeBar();

        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();

        ccdw.Repaint();
    }

    void MakeCreatue() {
        // Card Statline
        card_attack = EditorGUILayout.IntField("Attack: ", card_attack);
        card_health = EditorGUILayout.IntField("Health: ", card_health);
    }

    void MakeSpell() {

    }

    void LoadCardType(CardType type) {
        card_type = type;
        if (type == CardType.Creature) {
            loaded_card = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Cards/CreatureTemplate.prefab", typeof(GameObject)));
            ability_holder_editor = Editor.CreateEditor(loaded_card.GetComponent<AbilityHolder>());
            mod_editor = Editor.CreateEditor(loaded_card.GetComponent<ModifierContainer>());
            card_object = new SerializedObject(loaded_card.GetComponent<Creature>());
        } else if (type == CardType.Spell) {

        }
    }

    void DisplayAbilities() {
        ability_holder_editor.OnInspectorGUI();
    }
    void DisplayMods() {
        mod_editor.OnInspectorGUI();
    }

    private void DrawCardPanel() {
        card_panel = new Rect(offset, 0, position.width * size_ratio - offset * 2, position.height);

        GUILayout.BeginArea(card_panel);
        GUILayout.Label("Card");

        EditorGUIUtility.labelWidth = 120;

        // Card Type
        CardType new_type = (CardType)EditorGUILayout.EnumPopup("Card Type: ", card_type);
        if (new_type != card_type) {
            card_type = new_type;
            ccdw.LoadFrame(new_type);
        }

        // Card Name
        card_name = EditorGUILayout.TextField("Name: ", card_name);

        // Card Description
        if (GUILayout.Button(enable_rich_text ? "Disable Rich Text" : "Enable Rich Text")) {
            enable_rich_text = !enable_rich_text;
        }
        EditorGUILayout.PrefixLabel("Card Description: ");

        GUIStyle description_style = new GUIStyle(GUI.skin.textArea);
        description_style.alignment = TextAnchor.MiddleCenter;
        description_style.richText = enable_rich_text;
        card_text = EditorGUILayout.TextArea(card_text, description_style, GUILayout.Height(EditorGUIUtility.singleLineHeight * 4));

        // Card art
        card_art = (Sprite)EditorGUILayout.ObjectField("Art: ", card_art, typeof(Sprite), false);

        // Card Cost
        card_cost = EditorGUILayout.IntField("Card Cost: ", card_cost);

        if (card_type == CardType.Creature) {
            MakeCreatue();
        } else if (card_type == CardType.Spell) {
            MakeSpell();
        }

        DisplayAbilities();
        DisplayMods();

        // Save Card

        if (GUILayout.Button("Save To Prefab")) {
            SaveCard();

            string save_path = EditorUtility.SaveFilePanelInProject("Save Card Prefab", "Card.prefab", "prefab", "Save Card File to Prefab", "Assets/Cards");
            if (save_path.Length != 0) {
                PrefabUtility.ReplacePrefab(loaded_card, PrefabUtility.CreateEmptyPrefab(save_path));
            }
        }

        GUILayout.EndArea();
    }

    private void DrawEffectsPanel() {
        effects_panel = new Rect(position.width * size_ratio + offset, 0, position.width * (1 - size_ratio) - offset * 2, position.height);

        GUILayout.BeginArea(effects_panel);
        GUILayout.Label("Effects");
        EditorGUILayout.BeginHorizontal();
        bool untargeted = GUILayout.Button("Add Untargeted Effect");
        bool targeted = GUILayout.Button("Add Targeted Effect");

        // Search
        if (targeted || untargeted) {
            popup_rect.x = 0;
            PopupWindow.Show(popup_rect, new EffectPopupWindow(
                new Vector2(effects_panel.width, (300 < effects_panel.height/2f ? 300 : effects_panel.height /2f)), 
                targeted, 
                AddComponentToLoadedCard));
        }
        if (Event.current.type == EventType.Repaint) popup_rect = GUILayoutUtility.GetLastRect();
        EditorGUILayout.EndHorizontal();

        foreach (Effect e in loaded_card.GetComponents<Effect>()) {
            EffectFoldout(e);
        }

        GUILayout.EndArea();
    }

    void AddComponentToLoadedCard(System.Type type) {
        loaded_card.AddComponent(type);
    }

    private void DrawResizeBar() {
        resizer = new Rect((position.width * size_ratio) - 5f, 0 , 10f, position.height);

        GUILayout.BeginArea(new Rect(resizer.position + (Vector2.right * 5f), new Vector2(2, position.height)), resizer_style);
        GUILayout.EndArea();

        EditorGUIUtility.AddCursorRect(resizer, MouseCursor.ResizeHorizontal);
    }

    private void ProcessEvents(Event e) {
        switch (e.type) {
            case EventType.MouseDown:
                if (e.button == 0 && resizer.Contains(e.mousePosition)) {
                    is_resizing = true;
                }
                break;
            case EventType.MouseUp:
                is_resizing = false;
                break;
        }

        Resize(e);
    }

    private void Resize(Event e) {
        if (is_resizing) {
            size_ratio = e.mousePosition.x / position.width;
            if (size_ratio < .1f) {
                size_ratio = .1f;
            } else if (size_ratio > .9f) {
                size_ratio = .9f;
            }
            Repaint();
        }
    }

    void SaveCard() {
        card_object.FindProperty("_card_name").stringValue = card_name;
        card_object.FindProperty("_card_text").stringValue = card_text;
        card_object.FindProperty("_mana_cost").intValue = card_cost;
        card_object.FindProperty("_art").objectReferenceValue = card_art;
        if (card_type == CardType.Creature) {
            card_object.FindProperty("_attack").intValue = card_attack;
            card_object.FindProperty("_max_health").intValue = card_health;
        }

        card_object.ApplyModifiedProperties();
    }

    void EffectFoldout(Effect e) {
        if (!effect_foldouts.ContainsKey(e)) {
            effect_foldouts.Add(e, false);
        }
        effect_foldouts[e] = EditorGUILayout.InspectorTitlebar(effect_foldouts[e], e);
        if (effect_foldouts[e]) {
            EditorGUI.indentLevel += 1;
            Editor editor = Editor.CreateEditor(e);
            editor.OnInspectorGUI();
            EditorGUI.indentLevel -= 1;
        }
    }

    private void OnDestroy() {
        ccdw.Close();
        DestroyImmediate(loaded_card);
    }
}
