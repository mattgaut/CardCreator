using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardContainer))]
public class FieldViewer : MonoBehaviour {

    CardContainer field;
    int count;
    int position_to_move_right;
    bool scoot_cards;

    [SerializeField] List<Transform> card_positions;

    public void Awake() {
        field = GetComponent<CardContainer>();
        count = 0;
    }

    public void Update() {
        if (count != field.count) {
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

    void UpdateCardViews() {
        for (int i = 0; i < field.count; i++) {
            field[i].transform.position = card_positions[i + Shift(i)].transform.position;
        }
    }

    int Shift(int i) {
        return (scoot_cards && field.count < 7 && i >= position_to_move_right ? 1 : 0);
    }
}
