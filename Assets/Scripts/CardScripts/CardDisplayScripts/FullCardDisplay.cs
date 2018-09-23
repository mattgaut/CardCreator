﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullCardDisplay : CardDisplay {

    [SerializeField] protected Text mana_cost, card_name, description;
    [SerializeField] protected Image border;

    public override void UpdateDisplay() {
        mana_cost.text = "" + card.mana_cost.value;
        card_name.text = card.card_name;
        description.text = card.card_text;
    }

    public void SetHighlight(bool highlight) {
        if (highlight) {
            border.color = Color.yellow;
        } else {
            border.color = Color.black;
        }
    }
    
}
