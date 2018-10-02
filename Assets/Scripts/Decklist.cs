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
}
