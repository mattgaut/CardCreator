using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionController : MonoBehaviour {

    [SerializeField] UICardDisplay[] card_displays;
    [SerializeField] DeckDisplay deck_display;

    [SerializeField] CardDatabase cards;

    [SerializeField] Button prev_button, next_button;

    SortedList<string, Card> sorted_cards;

    SortedList<string, Card> searched_sorted_cards;

    bool searching;

    int pages, page;

    private void Start() {
        sorted_cards = new SortedList<string, Card>();

        foreach (Card card in cards.GetCards()) {
            if (card.collectible) {
                sorted_cards.Add(card.card_name, card);
            }        
        }
        pages = (sorted_cards.Count / card_displays.Length) + 1;

        DisplayPage(0);
    }

    void DisplayPage(int page) {
        SortedList<string, Card> list = searching ? searched_sorted_cards : sorted_cards;

        this.page = page;
        for (int i = 0; i < 8; i++) {
            if (list.Count <= i + page * (card_displays.Length)) {
                card_displays[i].gameObject.SetActive(false);
            } else {
                card_displays[i].gameObject.SetActive(true);
                Card card = list.Values[i + page * (card_displays.Length)];
                card_displays[i].SetCard(card);
                card_displays[i].SetOnClick(() => deck_display.AddToDeck(card));
            }
        }

        prev_button.interactable = (page != 0);
        next_button.interactable = (page != pages - 1);
    }

    public void NextPage() {
        if (page >= pages - 1) {
            return;
        }
        DisplayPage(page + 1);
    }

    public void PrevPage() {
        if (page <= 0) {
            return;
        }
        DisplayPage(page - 1);
    }

    public void Search(string search) {
        if (search == "") {
            searching = false;
            pages = (sorted_cards.Count / card_displays.Length) + 1;
        } else {
            searching = true;
            searched_sorted_cards = new SortedList<string, Card>();
            foreach (Card c in sorted_cards.Values) {
                if (c.name.ToLower().Contains(search.ToLower()) || c.card_text.ToLower().Contains(search.ToLower())) searched_sorted_cards.Add(c.name, c);
            }
            pages = (searched_sorted_cards.Count / card_displays.Length) + 1;
        }

        DisplayPage(0);
    }
}
