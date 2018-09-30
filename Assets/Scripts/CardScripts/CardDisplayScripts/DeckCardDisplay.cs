using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DeckCardDisplay : MonoBehaviour {

    [SerializeField] Text card_name, mana_cost;
    [SerializeField] Image art;
    [SerializeField] GameObject count;

    Button button;

    private void Awake() {
        button = GetComponent<Button>();
    }

    public void SetCard(Card c, bool is_two) {
        card_name.text = c.card_name;
        mana_cost.text = c.mana_cost.value + "";

        art.sprite = c.art;

        count.SetActive(is_two);
    }

    public void AddOnClick(UnityAction action) {
        button.onClick.AddListener(action);
    }
}
