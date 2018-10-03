using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIDeckDisplay : MonoBehaviour {

    [SerializeField] Image art;
    [SerializeField] Text deck_name;

    [SerializeField] Button button;

    [SerializeField] GameObject new_deck_object;

    public void SetDeckName(string name) {
        deck_name.text = name;
    }

    public void SetArt(Sprite sprite) {
        art.sprite = sprite;
    }

    public void SetOnClick(UnityAction action) {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    public void SetNewDeck(bool is_new_deck) {
        new_deck_object.SetActive(is_new_deck);
    }
}
