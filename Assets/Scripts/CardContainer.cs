using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardContainer : MonoBehaviour {

    [SerializeField] Zone _zone;
    [SerializeField] bool has_max;
    [SerializeField] int max_cards;
    [SerializeField] bool _visible, _hidden_to_opponent;
    [SerializeField] List<Card> contained_cards;

    int locked_places;

    public Zone zone {
        get { return _zone; }
    }

    public int count {
        get { return contained_cards.Count; }
    }
    public bool full {
        get { return has_max && count + locked_places >= max_cards; }
    }


    public bool visible {
        get { return _visible; }
    }
    public bool hidden_to_opponent {
        get { return _hidden_to_opponent; }
    }

    public Player controller {
        get; private set;
    }

    public List<Card> cards {
        get { return new List<Card>(contained_cards); }
    }

    private void Awake() {
        foreach (Card c in contained_cards) {
            c.SetContainer(this);
        }
    }

    public Card GetNext() {
        Card c = contained_cards[0];
        contained_cards.RemoveAt(0);
        return c;
    }

    public bool RemoveCard(Card c) {
        if (ContainsCard(c)) {
            c.SetContainer(null);
            return contained_cards.Remove(c);            
        } else {
            return false;
        }
    }

    public bool AddCard(Card c) {
        if (!has_max || max_cards > contained_cards.Count) {
            contained_cards.Add(c);
            c.SetContainer(this);
            return true;
        } else {
            return false;
        }
    }

    public bool AddCard(Card c, int i) {
        if (!has_max || max_cards > contained_cards.Count) {
            contained_cards.Insert(i, c);
            c.SetContainer(this);
            return true;
        } else {
            return false;
        }
    }

    public bool ContainsCard(Card c) {
        return contained_cards.Contains(c);
    }

    public void Shuffle() {
        for (int i = contained_cards.Count - 1; i > 0; i--) {
            int j = Random.Range(0, i);
            Card temp = contained_cards[j];
            contained_cards[j] = contained_cards[i];
            contained_cards[i] = temp;
        }
    }

    public void SetController(Player player) {
        controller = player;
    }

    public static void MoveCard(Card c, CardContainer from, CardContainer to) {
        from.RemoveCard(c);
        to.AddCard(c);
    }
    public static void MoveCard(Card c, CardContainer from, CardContainer to, int position) {
        from.RemoveCard(c);
        to.AddCard(c, position);
    }

    public Card this[int key] {
        get {
            if (key >= 0 && key < count) {
                return contained_cards[key];
            } else {
                return null;
            }  
        }
    }

    // Used to reserve spots in the container
    public bool AddLock() {
        if (full) {
            return false;
        }
        locked_places++;
        return true;
    }
    public bool RemoveLock() {
        if (locked_places== 0) {
            return false;
        }
        locked_places--;
        return true;
    }
}

public enum Zone { field = 1, hand = 2, discard = 4, graveyard = 8, deck = 16, secrets = 32, stack = 64 }
