using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelector : MonoBehaviour {

    public bool done_selecting { get; private set; }

    [SerializeField] List<CardSelectorButton> positions;
    [SerializeField] Button confirm_button;

    List<int> buttons_selected;

    public IEnumerator StartMultiCardSelection(params Card[] cards) {
        buttons_selected = new List<int>();
        done_selecting = false;

        confirm_button.gameObject.SetActive(true);

        for (int i = 0; i < positions.Count && i < cards.Length; i++) {
            positions[i].gameObject.SetActive(true);
            positions[i].SetCard(cards[i], MultiSelectCard);
        }
        for (int i = cards.Length; i < positions.Count; i++) {
            positions[i].gameObject.SetActive(false);
        }

        while (!done_selecting) {
            yield return null;
        }
    }

    public IEnumerator StartSingleCardSelection(params Card[] cards) {
        buttons_selected = new List<int>();
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
        List<Card> selected = new List<Card>();
        foreach (int i in buttons_selected) {
            selected.Add(positions[i].to_return);
        }
        return selected;
    }

    public void Flip() {
        confirm_button.transform.parent.localPosition *= -1f;
    }

    public List<int> GetPositionsSelected() {
        return new List<int>(buttons_selected);
    }

    public Card GetCardSelected() {
        return positions[buttons_selected[0]].to_return;
    }

    void SingleSelectCard(CardSelectorButton selected) {
        buttons_selected = new List<int>() { positions.IndexOf(selected)};

        FinishSelecting();
    }

    void MultiSelectCard(CardSelectorButton selected) {
        int position = positions.IndexOf(selected);
        if (buttons_selected.Contains(position)) {
            buttons_selected.Remove(position);
        } else {
            buttons_selected.Add(position);
        }
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
