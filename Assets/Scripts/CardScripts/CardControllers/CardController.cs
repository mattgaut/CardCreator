using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Card), typeof(Collider))]
public abstract class CardController : MonoBehaviour, IClickable {


    [SerializeField] protected Collider box;
    protected CardContainer last_container;
    [SerializeField] protected CardDisplay display;

    public Card card {
        get; protected set;
    }

    protected virtual void Awake() {
        card = GetComponent<Card>();
    }

    protected virtual void Update() {
        UpdateContainer();
    }

    protected virtual void UpdateContainer() {
        if (last_container != card.container) {
            last_container = card.container;
            if (last_container != null && last_container.visible) {
                ShowCard();
            } else {
                HideCard();
            }
        }
    }

    public abstract void OnClick();

    protected virtual void HideCard() {
        display.HideCard();
        box.enabled = false;
    }

    protected virtual void ShowCard() {
        display.ShowCard();
        box.enabled = true;
    }

    public virtual void OnFinishDrag(GameObject dragged_to, Vector3 position_dragged_to) {    
    }

    public virtual void OnHoldDrag(GameObject dragged_to, Vector3 position_dragged_to) {
    }
    public virtual void OnEndClick() {

    }
    public virtual void OnMouseDown() {

    }
}
