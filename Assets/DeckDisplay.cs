﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class DeckDisplay : MonoBehaviour {

    [SerializeField] DeckCardDisplay card_display_prefab;
    [SerializeField] RectTransform root;

    [SerializeField] Text count_text;
    [SerializeField] Button save_button;
    [SerializeField] InputField name_field;
    [SerializeField] Dropdown dropdown;

    [SerializeField] CardDatabase database;

    Decklist to_display;

    Dictionary<int, DeckCardDisplay> card_displays;

    float child_height;

    string filename;

    void Awake() {
        card_displays = new Dictionary<int, DeckCardDisplay>();
        to_display = new Decklist();

        child_height = card_display_prefab.GetComponent<RectTransform>().rect.height;
    }

    private void Update() {
        save_button.interactable = to_display.count == 30 && name_field.text != "";
    }

    public void SetFileName(string filename) {
        this.filename = filename;
    }

    public void SetClass(int class_value) {
        if (class_value > 0 && class_value < 10) {
            to_display.SetDeckClass((Player.Class)(class_value + 1));
        }
    }

    public void AddToDeck(Card to_add) {
        if (to_display.AddCard(to_add.id, to_add.rarity == Card.Rarity.legendary)) {
            AddCardButton(to_add);
        }
    }

    public void RemoveFromDeck(Card to_remove) {
        if (to_display.RemoveCard(to_remove.id)) {
            RemoveCardButton(to_remove);
        }
    }

    public void Load(DeckFile deck_file) {
        to_display.Load(deck_file);
        name_field.text = deck_file.name;
        dropdown.value = (int)deck_file.deck_class - 1;

        ReloadCardButtons();
    }

    public DeckFile Save() {
        return new DeckFile(name_field.text, to_display);
    }

    void UpdateCount() {
        count_text.text = "Cards: " + to_display.count + "/30";
    }

    void ReloadCardButtons() {
        foreach (DeckCardDisplay display in card_displays.Values) {
            Destroy(display.gameObject);
        }
        card_displays.Clear();

        foreach (int id in to_display.GetIds()) {
            AddCardButton(database.GetCard(id));
        }
    }

    void AddCardButton(Card to_add) {
        if (card_displays.ContainsKey(to_add.id)) {
            card_displays[to_add.id].SetCard(to_add, true);
        } else {
            card_displays.Add(to_add.id, Instantiate(card_display_prefab, root));
            root.sizeDelta += Vector2.up * child_height;
            card_displays[to_add.id].SetCard(to_add, false);
            card_displays[to_add.id].AddOnClick(() => RemoveFromDeck(to_add));
        }
        UpdateCount();
    }

    void RemoveCardButton(Card to_remove) {
        if (to_display.ContainsCard(to_remove.id)) {
            card_displays[to_remove.id].SetCard(to_remove, false);
        } else {
            Destroy(card_displays[to_remove.id].gameObject);
            card_displays.Remove(to_remove.id);
            root.sizeDelta -= Vector2.up * child_height;
        }
        UpdateCount();
    }
}
