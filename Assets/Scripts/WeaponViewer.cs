using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardContainer))]
public class WeaponViewer : MonoBehaviour {

    CardContainer weapon_container;

    Card current_weapon;

    [SerializeField] Transform card_position;

    private void Awake() {
        weapon_container = GetComponent<CardContainer>();
    }

    private void Update() {
        if (current_weapon != weapon_container.TopCard()) {
            UpdateCardViews();
        }
    }

    void UpdateCardViews() {
        current_weapon = weapon_container.TopCard();
        if (current_weapon != null) {
            current_weapon.transform.position = card_position.position;
        }
    }
}
