using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullCardDisplay : CardDisplay {

    [SerializeField] protected Text mana_cost;

    public override void UpdateDisplay() {
        mana_cost.text = "" + card.mana_cost;
    }
}
