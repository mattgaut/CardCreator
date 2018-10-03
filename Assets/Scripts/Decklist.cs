using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Decklist {

    public Player.Class deck_class { get; private set; }
    public int count { get { return card_ids.Count; } }

    List<int> card_ids;

	public Decklist() {
        card_ids = new List<int>();
    }

    public void SetDeckClass(Player.Class _class) {
        deck_class = _class;
    }

    public bool AddCard(int card_id, bool is_legendary) {
        if (card_ids.Count < 30 && card_ids.FindAll((id) => id == card_id).Count < (is_legendary ? 1 : 2)) {
            card_ids.Add(card_id);
            return true;
        }
        return false;
    }

    public bool RemoveCard(int card_id) {
        return card_ids.Remove(card_id);
    }

    public bool ContainsCard(int card_id) {
        return card_ids.Contains(card_id);
    }

    public IEnumerable<int> GetIds() {
        return card_ids;
    }

    public List<int> GetIdList() {
        return card_ids;
    }

    public void Load(DeckFile decklist) {
        card_ids = new List<int>(decklist.cards);
        deck_class = decklist.deck_class;
    }
}


[System.Serializable]
public class DeckFile {

    public int[] cards;
    public Player.Class deck_class;
    public string name;

    public DeckFile(string name, Decklist decklist) {
        cards = decklist.GetIdList().ToArray();
        deck_class = decklist.deck_class;
        this.name = name;
    }

}
