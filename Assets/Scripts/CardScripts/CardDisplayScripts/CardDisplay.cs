using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Card))]
public abstract class CardDisplay : MonoBehaviour {

    [SerializeField] protected Canvas face;
    [SerializeField] Image art;
    protected Card card;

    protected virtual void Awake() {
        card = GetComponent<Card>();
    }
    
    protected virtual void Update() {
        UpdateDisplay();
    }

    private void Start() {
        UpdateDisplay();
        art.sprite = card.art;
    }

    public abstract void UpdateDisplay();

    public void HideCard() {
        face.enabled = false;
    }

    public void ShowCard() {
        face.enabled = true;
    }
}
