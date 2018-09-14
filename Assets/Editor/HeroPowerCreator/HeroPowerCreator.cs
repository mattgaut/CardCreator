using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HeroPowerCreator : EditorWindow {
    [MenuItem("Window/Hero Power Creator")]
    public static void ShowWindow() {
        (GetWindow(typeof(HeroPowerCreator)) as HeroPowerCreator).LoadHeroPower();
    }

    string hero_power_name;
    string hero_power_text;
    int hero_power_cost;

    List<UntargetedEffect> untargeted_effects;
    List<TargetedEffect> targeted_effects;

    bool enable_rich_text;

    Sprite art;

    GameObject loaded_hero_power;
    SerializedObject hero_power_object;
    TriggeredAbility secret_trigger;

    private Rect card_panel;
    private Rect effects_panel;
    private Rect resizer;
    private Rect popup_rect;

    float size_ratio = 0.5f;
    float offset = 5f;

    bool is_resizing;

    GUIStyle resizer_style;

    Editor mod_editor;
    string search_string;

    Dictionary<Effect, bool> effect_foldouts;
    //Dictionary<StaticAbility, bool> statics_foldouts;
    Dictionary<TriggeredAbility, bool> triggered_foldouts;

    void OnGUI() {
        DrawCardPanel();
        DrawEffectsPanel();
        DrawResizeBar();

        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();
    }

    private void OnEnable() {
        resizer_style = new GUIStyle();
        resizer_style.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;

        effect_foldouts = new Dictionary<Effect, bool>();
        //statics_foldouts = new Dictionary<StaticAbility, bool>();
        triggered_foldouts = new Dictionary<TriggeredAbility, bool>();
    }

    private void DrawCardPanel() {
        card_panel = new Rect(offset, 0, position.width * size_ratio - offset * 2, position.height);

        GUILayout.BeginArea(card_panel);
        GUILayout.Label("Hero Power");

        EditorGUIUtility.labelWidth = 120;

        // Name
        hero_power_name = EditorGUILayout.TextField("Name: ", hero_power_name);

        // Description
        if (GUILayout.Button(enable_rich_text ? "Disable Rich Text" : "Enable Rich Text")) {
            enable_rich_text = !enable_rich_text;
        }
        EditorGUILayout.PrefixLabel("Description: ");

        GUIStyle description_style = new GUIStyle(GUI.skin.textArea);
        description_style.alignment = TextAnchor.MiddleCenter;
        description_style.richText = enable_rich_text;
        hero_power_text = EditorGUILayout.TextArea(hero_power_text, description_style, GUILayout.Height(EditorGUIUtility.singleLineHeight * 4));

        // art
        art = (Sprite)EditorGUILayout.ObjectField("Art: ", art, typeof(Sprite), false);

        // Cost
        hero_power_cost = EditorGUILayout.IntField("Cost: ", hero_power_cost);

        // Effects
        EditorGUILayout.PropertyField(hero_power_object.FindProperty("untargeted_effects"), new GUIContent("Untargeted Effects"), true);
        EditorGUILayout.PropertyField(hero_power_object.FindProperty("targeted_effects"), new GUIContent("Targeted Effects"), true);
        if (hero_power_object.FindProperty("targeted_effects").arraySize > 0) {
            EditorGUILayout.PropertyField(hero_power_object.FindProperty("_targeting_comparer"), new GUIContent("Targeting Rules"), true);
        }
        mod_editor.OnInspectorGUI();

        EditorGUILayout.PropertyField(hero_power_object.FindProperty("triggered_ability"), new GUIContent("Triggered Ability"), true);

        // Save
        if (GUILayout.Button("Save To Prefab")) {
            SaveHeroPower();

            string save_path = EditorUtility.SaveFilePanelInProject("Save Hero Power Prefab", hero_power_name + ".prefab", "prefab", "Save Hero Power File to Prefab", "Assets/HeroPowers");
            if (save_path.Length != 0) {
                PrefabUtility.ReplacePrefab(loaded_hero_power, PrefabUtility.CreateEmptyPrefab(save_path));
            }
        }

        GUILayout.EndArea();
    }

    private void DrawEffectsPanel() {
        effects_panel = new Rect(position.width * size_ratio + offset, 0, position.width * (1 - size_ratio) - offset * 2, position.height);

        GUILayout.BeginArea(effects_panel);
        GUILayout.Label("Effects");
        EditorGUILayout.BeginHorizontal();
        bool untargeted = GUILayout.Button("Untargeted Effect");
        bool targeted = GUILayout.Button("Targeted Effect");
        bool _static = GUILayout.Button("Static Effect");
        bool triggered = GUILayout.Button("Triggered Ability");

        // Search
        if (targeted || untargeted || _static || triggered) {
            EffectPopupWindow window = null;

            if (targeted) {
                window = new EffectPopupWindow(new Vector2(effects_panel.width, (300 < effects_panel.height / 2f ? 300 : effects_panel.height / 2f)),
                    "Assets/Scripts/Effects/Targeted",
                    AddComponentToLoadedCard);
            } else if (untargeted) {
                window = new EffectPopupWindow(new Vector2(effects_panel.width, (300 < effects_panel.height / 2f ? 300 : effects_panel.height / 2f)),
                    "Assets/Scripts/Effects/Untargeted",
                    AddComponentToLoadedCard);
            } else if (_static) {
                window = new EffectPopupWindow(new Vector2(effects_panel.width, (300 < effects_panel.height / 2f ? 300 : effects_panel.height / 2f)),
                    "Assets/Scripts/Statics/Statics",
                    AddComponentToLoadedCard);
            } else if (triggered) {
                window = new EffectPopupWindow(new Vector2(effects_panel.width, (300 < effects_panel.height / 2f ? 300 : effects_panel.height / 2f)),
                    "Assets/Scripts/Triggers/Triggers",
                    AddComponentToLoadedCard);
            }

            popup_rect.x = 0;
            PopupWindow.Show(popup_rect, window);
        }
        if (Event.current.type == EventType.Repaint) popup_rect = GUILayoutUtility.GetLastRect();
        EditorGUILayout.EndHorizontal();


        foreach (Effect e in loaded_hero_power.GetComponents<Effect>()) {
            EffectFoldout(e);
        }
        //foreach (StaticAbility sa in loaded_hero_power.GetComponents<StaticAbility>()) {
        //    StaticsFoldout(sa);
        //}
        foreach (TriggeredAbility ta in loaded_hero_power.GetComponents<TriggeredAbility>()) {
            TriggersFoldout(ta);
        }

        GUILayout.EndArea();
    }

    void AddComponentToLoadedCard(System.Type type) {
        loaded_hero_power.AddComponent(type);
    }

    private void DrawResizeBar() {
        resizer = new Rect((position.width * size_ratio) - 5f, 0, 10f, position.height);

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

    //void StaticsFoldout(StaticAbility sa) {
    //    if (!statics_foldouts.ContainsKey(sa)) {
    //        statics_foldouts.Add(sa, false);
    //    }
    //    statics_foldouts[sa] = EditorGUILayout.InspectorTitlebar(statics_foldouts[sa], sa);
    //    if (statics_foldouts[sa]) {
    //        EditorGUI.indentLevel += 1;
    //        Editor editor = Editor.CreateEditor(sa);
    //        editor.OnInspectorGUI();
    //        EditorGUI.indentLevel -= 1;
    //    }
    //}

    void TriggersFoldout(TriggeredAbility ta) {
        if (!triggered_foldouts.ContainsKey(ta)) {
            triggered_foldouts.Add(ta, false);
        }
        triggered_foldouts[ta] = EditorGUILayout.InspectorTitlebar(triggered_foldouts[ta], ta);
        if (triggered_foldouts[ta]) {
            EditorGUI.indentLevel += 1;
            Editor editor = Editor.CreateEditor(ta);
            editor.OnInspectorGUI();
            EditorGUI.indentLevel -= 1;
        }
    }

    void LoadHeroPower() {
        if (loaded_hero_power != null) {
            DestroyImmediate(loaded_hero_power);
        }
        loaded_hero_power = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath("Assets/HeroPowers/HeroPowerTemplate.prefab", typeof(GameObject)));
        mod_editor = Editor.CreateEditor(loaded_hero_power.GetComponent<ModifierContainer>());
        hero_power_object = new SerializedObject(loaded_hero_power.GetComponent<HeroPower>());
    }

    void SaveHeroPower() {
        hero_power_object.FindProperty("_card_name").stringValue = hero_power_name;
        hero_power_object.FindProperty("_card_text").stringValue = hero_power_text;
        hero_power_object.FindProperty("_mana_cost").FindPropertyRelative("_base_value").intValue = hero_power_cost;
        hero_power_object.FindProperty("_art").objectReferenceValue = art;

        hero_power_object.ApplyModifiedProperties();
    }

    private void OnDestroy() {
        DestroyImmediate(loaded_hero_power);
    }
}
