﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    public static DeckFile player_1_deckfile {
        get; private set;
    }
    public static DeckFile player_2_deckfile {
        get; private set;
    }

    [SerializeField] List<UIDeckDisplay> deck_displays;
    [SerializeField] List<Sprite> class_sprites;

    [SerializeField] UIDeckDisplay main_display;

    [SerializeField] DeckDisplay deck_constructor;
    [SerializeField] GameObject deck_contsructor_panel;

    [SerializeField] Dropdown player_1_deck, player_2_deck;

    int main_deck_index;

    List<DeckFile> decks;

    public void SaveMainDeck() {
        decks[main_deck_index] = deck_constructor.Save();

        BinaryFormatter bf = new BinaryFormatter();

        Directory.CreateDirectory(Application.persistentDataPath + "/Decks/");
        FileStream file = File.Open(Application.persistentDataPath + "/Decks/" + "Deck" + main_deck_index + ".deck", FileMode.OpenOrCreate);

        bf.Serialize(file, decks[main_deck_index]);

        file.Close();

        deck_displays[main_deck_index].SetNewDeck(false);
        deck_displays[main_deck_index].SetArt(GetArt(decks[main_deck_index].deck_class));
        deck_displays[main_deck_index].SetDeckName(decks[main_deck_index].name);
        deck_displays[main_deck_index].SetOnClick(() => SelectDeck(main_deck_index));

        SelectDeck(main_deck_index);

        SetDropdowns();
    }

    public void DeleteMainDeck() {
        decks[main_deck_index] = null;

        File.Delete(Application.persistentDataPath + "/Decks/" + "Deck" + main_deck_index + ".deck");

        deck_displays[main_deck_index].SetNewDeck(true);
        deck_displays[main_deck_index].SetOnClick(() => StartDeckEditor(new DeckFile("Deck" + main_deck_index), main_deck_index));

        SelectDeck(main_deck_index);
        SetDropdowns();
    }

    public void EditMainDeck() {
        StartDeckEditor(decks[main_deck_index], main_deck_index);
    }

    private void Awake() {
        LoadDecks();

        SetDropdowns();
    }

    public void StartGame() {
        player_1_deckfile = null;
        player_2_deckfile = null;

        int p1 = 0, p2 = 0;
        for (int i = 0; i < decks.Count; i++) {
            if (decks[i] == null) {
                continue;
            } else {
                if (player_1_deck.value == p1) {
                    player_1_deckfile = decks[i];
                }
                if (player_2_deck.value == p2) {
                    player_2_deckfile = decks[i];
                }

                p1++;
                p2++;
            }
        }

        SceneManager.LoadScene("CardGameScene");
    }

    void LoadDecks() {
        decks = new List<DeckFile>();
        for (int i = 0; i < deck_displays.Count; i++) {
            DeckFile deck = LoadDeckFile("Deck" + i + ".deck");
            int deck_to_choose = i;
            decks.Add(deck);
            if (deck != null) {
                deck_displays[i].SetNewDeck(false);
                deck_displays[i].SetDeckName(deck.name);
                deck_displays[i].SetArt(GetArt(deck.deck_class));

                deck_displays[i].SetOnClick(() => SelectDeck(deck_to_choose));
            } else {
                deck_displays[i].SetNewDeck(true);
                deck_displays[i].SetOnClick(() => StartDeckEditor(new DeckFile("Deck" + deck_to_choose), deck_to_choose));
            }
        }

        main_display.gameObject.SetActive(false);
    }

    void SetDropdowns() {
        player_1_deck.ClearOptions();
        player_2_deck.ClearOptions();
        player_1_deck.value = 0;
        player_2_deck.value = 0;
        foreach (DeckFile deck in decks) {
            if (deck == null) {
                continue;
            }

            player_1_deck.AddOptions(new List<Dropdown.OptionData>() { new Dropdown.OptionData(deck.name) });
            player_2_deck.AddOptions(new List<Dropdown.OptionData>() { new Dropdown.OptionData(deck.name) });
        }
    }

    DeckFile LoadDeckFile(string filename) {
        if (!File.Exists(Application.persistentDataPath + "/Decks/" + filename)) {
            return null;
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/Decks/" + filename, FileMode.Open);

        DeckFile deck = (DeckFile)bf.Deserialize(file);
      
        file.Close();

        return deck;
    }

    void StartDeckEditor(DeckFile deckfile, int index) {
        deck_contsructor_panel.SetActive(true);
        deck_constructor.Load(deckfile);
        main_deck_index = index;
    }

    Sprite GetArt(Player.Class deck_class) {
        return class_sprites[(int)deck_class - 1];
    }

    void SelectDeck(int index) {
        if (decks[index] == null) {
            main_display.gameObject.SetActive(false);
            return;
        }
        main_display.gameObject.SetActive(true);

        main_deck_index = index;

        main_display.SetNewDeck(false);
        main_display.SetDeckName(decks[main_deck_index].name);
        main_display.SetArt(GetArt(decks[main_deck_index].deck_class));
    }
}
