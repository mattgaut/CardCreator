using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card Database", fileName = "Card Database")]
public class CardDatabase : ScriptableObject {

    [SerializeField] List<int> ids;
    [SerializeField] List<Card> cards;

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
        cards_by_id.Add(id, card);
        card.SetID(id);
    }

    private void OnEnable() {
        cards_by_id = new Dictionary<int, Card>();
        if (ids == null || cards == null) {
            ids = new List<int>();
            cards = new List<Card>();
        }
        for (int i = 0; i < ids.Count && i < cards.Count; i++) {
            cards_by_id.Add(ids[i], cards[i]);
        }    
    }

    private void OnDisable() {
        ids = new List<int>();
        cards = new List<Card>();
        foreach(int id in cards_by_id.Keys) {
            ids.Add(id);
            cards.Add(cards_by_id[id]);
        }
    }
}
