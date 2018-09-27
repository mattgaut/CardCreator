using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Card), typeof(Collider))]
public abstract class CardController : MonoBehaviour, IClickable {


    [SerializeField] protected Collider box;
    protected CardContainer last_container;
    [SerializeField] protected CardDisplay display_prefab;
    protected CardDisplay display;

    public bool hovering { get; private set; }

    public bool must_drag {
        get; protected set;
    }

    public bool can_click {
        get { return card.controller == GameManager.current_player; }
    }

    public Card card {
        get; protected set;
    }

    public CardDisplay GetDisplayPrefab() {
        return display_prefab;
    }

    protected virtual void Awake() {
        card = GetComponent<Card>();

        display = Instantiate(display_prefab, transform);
        display.SetCard(card);
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
    public virtual void OnLeftClickDown() {

    }

    public virtual void OnHoverStart() {
        if (card.container.zone == Zone.hand && !hovering) {
            hovering = true;
            card.transform.position = card.controller.hand.GetComponent<HandViewer>().GetHoverPosition(card);
        }
    }

    public virtual void OnHoverEnd(bool was_clicked) {
        if (card.container.zone == Zone.hand && hovering) {
            hovering = false;
            card.transform.position = card.controller.hand.GetComponent<HandViewer>().GetPosition(card);
        }
    }
}
