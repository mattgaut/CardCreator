using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardContainer))]
public class HandViewer : MonoBehaviour {

    CardContainer hand;
    int count;
    int position_to_move_right;
    bool scoot_cards;

    [SerializeField] List<Transform> card_positions;

    void Awake() {
        hand = GetComponent<CardContainer>();
        count = 0;
    }

    void Update() {
        if (count != hand.count) {
            UpdateCardViews();
        }
    }

    public void MakeRoom(int i) {
        position_to_move_right = i;
        scoot_cards = true;
    }
    public void StopMakeRoom() {
        scoot_cards = false;
    }

    public void ForceUpdate() {
        UpdateCardViews();
    }

    public Vector3 GetHoverPosition(Card card) {
        int i = hand.CardPosition(card);
        if (i == -1) {
            return card.transform.position;
        }
        return card_positions[i].position + Vector3.forward * 1.2f;
    }

    public Vector3 GetPosition(Card card) {
        int i = hand.CardPosition(card);
        if (i == -1) {
            return card.transform.position;
        }
        return card_positions[i].position;
    }


    void UpdateCardViews() {
        for (int i = 0; i < hand.count; i++) {
            hand[i].transform.position = card_positions[i + (scoot_cards && i >= position_to_move_right ? 1 : 0)].transform.position;
        }
        count = hand.count;
    }
}
