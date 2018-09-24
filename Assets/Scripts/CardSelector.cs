using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelector : MonoBehaviour {

    public bool done_selecting { get; private set; }

    [SerializeField] List<CardSelectorButton> positions;
    [SerializeField] Button confirm_button;

    List<Card> _cards_selected;

    public IEnumerator StartMultiCardSelection(params Card[] cards) {
        _cards_selected = new List<Card>();
        done_selecting = false;

        confirm_button.gameObject.SetActive(true);

        for (int i = 0; i < positions.Count && i < cards.Length; i++) {
            positions[i].gameObject.SetActive(true);
            positions[i].SetCard(cards[i]);
        }
        for (int i = cards.Length; i < positions.Count; i++) {
            positions[i].gameObject.SetActive(false);
        }

        while (!done_selecting) {
            yield return null;
        }
    }

    public IEnumerator StartSingleCardSelection(params Card[] cards) {
        _cards_selected = new List<Card>();
        done_selecting = false;

        confirm_button.gameObject.SetActive(false);

        for (int i = 0; i < positions.Count && i < cards.Length; i++) {
            positions[i].gameObject.SetActive(true);
            positions[i].SetCard(cards[i], SingleSelectCard);
        }
        for (int i = cards.Length; i < positions.Count; i++) {
            positions[i].gameObject.SetActive(false);
        }

        while (!done_selecting) {
            yield return null;
        }
    }

    public List<Card> GetCardsSelected() {
        return new List<Card>(_cards_selected);
    }

    public Card GetCardSelected() {
        return _cards_selected[0];
    }

    void SingleSelectCard(CardSelectorButton selected) {
        _cards_selected = new List<Card>() { selected.to_return };

        FinishSelecting();
    }

    void FinishSelecting() {
        done_selecting = true;

        confirm_button.gameObject.SetActive(false);
        foreach (CardSelectorButton csb in positions) {
            csb.gameObject.SetActive(false);
        }
    }

    private void Awake() {
        confirm_button.onClick.AddListener(FinishSelecting);

        confirm_button.gameObject.SetActive(false);
        foreach (CardSelectorButton csb in positions) {
            csb.gameObject.SetActive(false);
        }
    }
}
