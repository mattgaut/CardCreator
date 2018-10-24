using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Player))]
public class PlayerDisplay : MonoBehaviour {

    Player to_display;

    [SerializeField] Text health, mana, attack, armor, deck;

	// Use this for initialization
	void Awake () {
        to_display = GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
        health.text = "" + to_display.current_health;
        mana.text = "" + to_display.current_mana + " / " + to_display.max_mana;
        deck.text = "" + to_display.deck.count;
        if (to_display.attack > 0) {
            attack.enabled = true;
            attack.text = "" + to_display.attack.value;
        } else {
            attack.enabled = false;
        }
        if (to_display.armor > 0) {
            armor.enabled = true;
            armor.text = "" + to_display.armor;
        } else {
            armor.enabled = false;
        }
    }
}
