using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Player))]
public class PlayerDisplay : MonoBehaviour {

    Player to_display;

    [SerializeField] Text health, mana, attack;

	// Use this for initialization
	void Awake () {
        to_display = GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
        health.text = "" + to_display.current_health;
        mana.text = "" + to_display.current_mana + " / " + to_display.max_mana;
        if (to_display.attack > 0) {
            attack.enabled = true;
            attack.text = "" + to_display.attack.value;
        } else {
            attack.enabled = false;
        }
    }
}
