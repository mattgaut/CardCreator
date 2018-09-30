using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionController : MonoBehaviour {

    [SerializeField] UICardDisplay[] card_displays;
    [SerializeField] DeckDisplay deck_display;

    private void Start() {
        foreach (UICardDisplay card_display in card_displays) {
            Card card = card_display.GetCard();
            card_display.AddOnClick(() => deck_display.AddToDeck(card));
        }
    }
}
