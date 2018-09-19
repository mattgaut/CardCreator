using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card Database", fileName = "Card Database")]
public class CardDatabase : ScriptableObject {

    [SerializeField] List<CardInfo> cards;

    Dictionary<int, Card> cards_by_id;

    public int GetNextId() {
        int next_id = 1;
        while (cards_by_id.ContainsKey(next_id)) {
            next_id++;
        }
        return next_id;
    }

    public bool IsIdTaken(int id) {
        return cards_by_id.ContainsKey(id);
    }

    public void AddCard(Card card, int id) {
        if (cards_by_id.ContainsKey(id)) {
            return;
        }

        cards_by_id.Add(id, card);
        card.SetID(id);

        cards.Add(new CardInfo(card, id));
        cards.Sort((a, b) => a.id - b.id);
    }

    public void ReloadDictionary() {
        cards_by_id = new Dictionary<int, Card>();
        if (cards == null) {
            cards = new List<CardInfo>();
        }
        for (int i = 0; i < cards.Count; i++) {
            if (cards[i] == null || cards[i].card == null) {
                continue;
            }
            cards_by_id.Add(cards[i].id, cards[i].card);
        }
        cards.Sort((a, b) => a.id - b.id);
    }

    private void OnEnable() {
        ReloadDictionary();
    }

    [System.Serializable]
    public class CardInfo {
        public int id;
        public Card card;

        public CardInfo(Card card, int id) {
            this.card = card;
            this.id = id;
        }
    }
}
