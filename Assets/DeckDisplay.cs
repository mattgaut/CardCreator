using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckDisplay : MonoBehaviour {

    [SerializeField] DeckCardDisplay card_display_prefab;
    [SerializeField] Transform root;

    Decklist to_display;

    Dictionary<int, DeckCardDisplay> card_displays;

    void Awake() {
        card_displays = new Dictionary<int, DeckCardDisplay>();
        to_display = new Decklist();
    }

    public void AddToDeck(Card to_add) {
        if (to_display.AddCard(to_add.id, to_add.rarity == Card.Rarity.legendary)) {
            if (card_displays.ContainsKey(to_add.id)) {
                card_displays[to_add.id].SetCard(to_add, true);
            } else {
                card_displays.Add(to_add.id, Instantiate(card_display_prefab, root));
                card_displays[to_add.id].SetCard(to_add, false);
                card_displays[to_add.id].AddOnClick(() => RemoveFromDeck(to_add));
            }
        }
    }

    public void RemoveFromDeck(Card to_remove) {
        if (to_display.RemoveCard(to_remove.id)) {
            if (to_display.ContainsCard(to_remove.id)) {
                card_displays[to_remove.id].SetCard(to_remove, false);
            } else {
                Destroy(card_displays[to_remove.id].gameObject);
                card_displays.Remove(to_remove.id);
            }
        }
    }
}
