﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum CreationType { Creature, Spell, Weapon, Secret }

public class CardCreationWindow : EditorWindow {

    [SerializeField] CardDatabase database;

    [MenuItem("Window/Card Creator")]
    public static void ShowWindow() {
        CardCreationWindow ccw = (CardCreationWindow)GetWindow(typeof(CardCreationWindow));
        CardCreationDisplayWindow ccdw = (GetWindow(typeof(CardCreationDisplayWindow)) as CardCreationDisplayWindow);
        ccw.ccdw = ccdw;
        ccdw.SetCreationWindow(ccw);
        ccw.LoadCardType(CreationType.Creature);
    }

    public CardType card_type { get; private set; }
    public CreationType creation_type { get; private set; }

    public string card_name { get; private set; }
    public Card.Class card_class { get; private set; }
    public Card.Rarity card_rarity { get; private set; }
    public Creature.CreatureType card_creature_type { get; private set; }
    public string card_text { get; private set; }
    public int card_cost { get; private set; }
    public int card_attack { get; private set; }
    public int card_health { get; private set; }
    public int card_id { get; private set; }

    public bool enable_rich_text { get; private set; }
    public Sprite card_art { get; private set; }

    public AbilityHolder abilities { get; private set; }

    public CardCreationDisplayWindow ccdw;

    GameObject loaded_card;
    SerializedObject card_object;
    TriggeredAbility secret_trigger;

    private Rect card_panel;
    private Rect effects_panel;
    private Rect resizer;
    private Rect popup_rect;

    float size_ratio = 0.5f;
    float offset = 5f;

    bool is_resizing = false;

    GUIStyle resizer_style;

    Editor ability_holder_editor;
    Editor mod_editor;

    Dictionary<Effect, bool> effect_foldouts;
    Dictionary<StaticAbility, bool> statics_foldouts;
    Dictionary<TriggeredAbility, bool> triggered_foldouts;


    private void OnEnable() {
        resizer_style = new GUIStyle();
        resizer_style.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;
        effect_foldouts = new Dictionary<Effect, bool>();
        statics_foldouts = new Dictionary<StaticAbility, bool>();
        triggered_foldouts = new Dictionary<TriggeredAbility, bool>();    
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
        // Card Creature Type
        card_creature_type = (Creature.CreatureType)EditorGUILayout.EnumPopup("Creature Type: ", card_creature_type);

        // Card Statline
        card_attack = EditorGUILayout.IntField("Attack: ", card_attack);
        card_health = EditorGUILayout.IntField("Health: ", card_health);
    }

    void MakeSpell() {
        EditorGUILayout.PropertyField(card_object.FindProperty("spell_effects"), true);
        EditorGUILayout.PropertyField(card_object.FindProperty("targeted_effects"), true);
        if (card_object.FindProperty("targeted_effects").arraySize > 0) EditorGUILayout.PropertyField(card_object.FindProperty("_targeting_comparer"), true);
    }

    void MakeWeapon() {
        // Card Statline
        card_attack = EditorGUILayout.IntField("Attack: ", card_attack);
        card_health = EditorGUILayout.IntField("Durability: ", card_health);
    }

    void MakeSecret() {
        secret_trigger = (TriggeredAbility)EditorGUILayout.ObjectField("Secret Trigger", secret_trigger, typeof(TriggeredAbility), true);
    }

    void LoadCardType(CreationType type) {
        Reset();
        card_id = database.GetNextId();
        if (loaded_card != null) {
            DestroyImmediate(loaded_card);
        }
        creation_type = type;
        if (type == CreationType.Creature) {
            loaded_card = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Cards/CreatureTemplate.prefab", typeof(GameObject)));
            card_object = new SerializedObject(loaded_card.GetComponent<Creature>());
        } else if (type == CreationType.Spell) {
            loaded_card = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Cards/SpellTemplate.prefab", typeof(GameObject)));
            card_object = new SerializedObject(loaded_card.GetComponent<Spell>());
        } else if (type == CreationType.Secret) {
            loaded_card = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Cards/SecretTemplate.prefab", typeof(GameObject)));
            card_object = new SerializedObject(loaded_card.GetComponent<Secret>());
        } else if (type == CreationType.Weapon) {
            loaded_card = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Cards/WeaponTemplate.prefab", typeof(GameObject)));
            card_object = new SerializedObject(loaded_card.GetComponent<Weapon>());
        }
        if (loaded_card != null) {
            ability_holder_editor = Editor.CreateEditor(loaded_card.GetComponent<AbilityHolder>());
            mod_editor = Editor.CreateEditor(loaded_card.GetComponent<ModifierContainer>());
            card_type = loaded_card.GetComponent<Card>().type;
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
        CreationType new_type = (CreationType)EditorGUILayout.EnumPopup("Card Type: ", creation_type);
        if (new_type != creation_type) {
            creation_type = new_type;
            ccdw.LoadFrame(creation_type);
            LoadCardType(creation_type);
        }

        // Card Name
        card_name = EditorGUILayout.TextField("Name: ", card_name);

        // Card ID
        card_id = EditorGUILayout.IntField("ID: ", card_id);
        if (database.IsIdTaken(card_id) || card_id == 0) {
            Color color = EditorStyles.label.normal.textColor;
            EditorStyles.label.normal.textColor = Color.red;
            EditorGUILayout.LabelField(card_id == 0 ? "ID invalid" : "ID is already in use.");
            EditorStyles.label.normal.textColor = color;
        }

        // Card Class
        card_class = (Card.Class)EditorGUILayout.EnumPopup("Class: ", card_class);

        // Card Rarity
        card_rarity = (Card.Rarity)EditorGUILayout.EnumPopup("Rarity: ", card_rarity);

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

        // Card Casting Restriction
        EditorGUILayout.PropertyField(card_object.FindProperty("restrictions"), true);

        if (creation_type == CreationType.Creature) {
            MakeCreatue();
        } else if (creation_type == CreationType.Spell) {
            MakeSpell();
        } else if (creation_type == CreationType.Secret) {
            MakeSecret();
        } else if (creation_type == CreationType.Weapon) {
            MakeWeapon();
        }

        if (creation_type != CreationType.Secret) {
            DisplayAbilities();
        }
        DisplayMods();

        // Save Card

        GUI.enabled = !database.IsIdTaken(card_id) && card_id != 0;
        if (GUILayout.Button("Save To Prefab")) {
            SaveCard();

            string save_path = EditorUtility.SaveFilePanelInProject("Save Card Prefab", card_name + ".prefab", "prefab", "Save Card File to Prefab", "Assets/Cards/" + CreationTypeToString(creation_type));
            if (save_path.Length != 0) {
                GameObject new_card = PrefabUtility.ReplacePrefab(loaded_card, PrefabUtility.CreateEmptyPrefab(save_path));

                database.AddCard(new_card.GetComponent<Card>(), card_id);
                EditorUtility.SetDirty(database);

                LoadCardType(creation_type);
            }
        }
        GUI.enabled = true;

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


        foreach (Effect e in loaded_card.GetComponents<Effect>()) {
            EffectFoldout(e);
        }
        foreach (StaticAbility sa in loaded_card.GetComponents<StaticAbility>()) {
            StaticsFoldout(sa);
        }
        foreach (TriggeredAbility ta in loaded_card.GetComponents<TriggeredAbility>()) {
            TriggersFoldout(ta);
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
        card_object.FindProperty("_card_class").enumValueIndex = (int)card_class;
        card_object.FindProperty("_card_text").stringValue = card_text;
        card_object.FindProperty("_mana_cost").FindPropertyRelative("_base_value").intValue = card_cost;
        card_object.FindProperty("_art").objectReferenceValue = card_art;
        card_object.FindProperty("_rarity").enumValueIndex = (int)card_rarity;
        card_object.FindProperty("_id").intValue = card_id;
        if (creation_type == CreationType.Creature) {
            card_object.FindProperty("_creature_type").intValue = (int)card_creature_type;
            card_object.FindProperty("_attack").FindPropertyRelative("_base_value").intValue = card_attack;
            card_object.FindProperty("_health").FindPropertyRelative("_base_value").intValue = card_health;
        } else if (creation_type == CreationType.Weapon) {
            card_object.FindProperty("_attack").FindPropertyRelative("_base_value").intValue = card_attack;
            card_object.FindProperty("_durability").FindPropertyRelative("_base_value").intValue = card_health;
        } else if (creation_type == CreationType.Secret) {
            card_object.FindProperty("trigger").objectReferenceValue = secret_trigger;
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

    void StaticsFoldout(StaticAbility sa) {
        if (!statics_foldouts.ContainsKey(sa)) {
            statics_foldouts.Add(sa, false);
        }
        statics_foldouts[sa] = EditorGUILayout.InspectorTitlebar(statics_foldouts[sa], sa);
        if (statics_foldouts[sa]) {
            EditorGUI.indentLevel += 1;
            Editor editor = Editor.CreateEditor(sa);
            editor.OnInspectorGUI();
            EditorGUI.indentLevel -= 1;
        }
    }

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

    void Reset() {
        card_art = null;
        card_attack = 0;
        card_class = Card.Class.neutral;
        card_cost = 0;
        card_creature_type = Creature.CreatureType.none;
        card_health = 0;
        card_name = "";
        card_rarity = Card.Rarity.basic;
        card_text = "";
    }

    string CreationTypeToString(CreationType ct) {
        if (creation_type == CreationType.Creature) {
            return "Minion";
        } else if (creation_type == CreationType.Spell) {
            return "Spell";
        } else if (creation_type == CreationType.Secret) {
            return "Secret";
        } else if (creation_type == CreationType.Weapon) {
            return "Weapon";
        }
        return "";
    }

    private void OnDestroy() {
        ccdw.Close();
        DestroyImmediate(loaded_card);
    }
}
