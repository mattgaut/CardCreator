using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectorButton : MonoBehaviour {

    [SerializeField] Button button;

    CardDisplay display;

    System.Action<CardSelectorButton> callback;

    public Card to_return { get; private set; }
    public bool selected { get; private set; }

    public void SetCard(Card card, System.Action<CardSelectorButton> on_select_action = null) {
        display = Instantiate(card.GetComponent<CardController>().GetDisplayPrefab());
        display.transform.position = transform.position;
        display.transform.SetParent(button.transform, true);
        display.SetCard(card);

        callback = on_select_action;

        to_return = card;
        selected = false;
    }

    public void ResetSelected() {
        selected = false;

        (display as FullCardDisplay).SetHighlight(selected);
    }

    void Toggle() {
        selected = !selected;

        Debug.Log("Toggle");

        (display as FullCardDisplay).SetHighlight(selected);

        if (callback != null && selected) callback(this);
    }

    private void Awake() {
        button.onClick.AddListener(Toggle);
    }
}
