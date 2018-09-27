using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IClickable))]
public class ClickHandler : MonoBehaviour {

    IClickable clickable;

    public bool mouse_over {
        get; private set;
    }

    private void Awake() {
        clickable = GetComponent<IClickable>();
    }

    private void OnMouseEnter() {
        mouse_over = true;
        StartCoroutine(ClickListener());
        clickable.OnHoverStart();
    }

    private void OnMouseExit() {
        mouse_over = false;
        clickable.OnHoverEnd(false);
    }

    IEnumerator ClickListener() {
        while (mouse_over) {
            if (clickable.can_click && Input.GetMouseButtonDown(0)) {
                clickable.OnLeftClickDown();
                bool mouse_left = false;
                while (clickable.can_click && !Input.GetMouseButtonUp(0)) {
                    mouse_left = !mouse_over || mouse_left || clickable.must_drag;
                    if (mouse_left) clickable.OnHoldDrag(OverObject(), OverPosition());
                    yield return null;
                }
                if (!mouse_left) {
                    clickable.OnClick();
                } else {
                    clickable.OnFinishDrag(OverObject(), OverPosition());
                }
                clickable.OnEndClick();
            }
            yield return null;
        }
    }

    GameObject OverObject() {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit floorHit;
        Physics.Raycast(camRay, out floorHit, 20f);
        if (floorHit.collider) {
            return floorHit.collider.gameObject;
        } else {
            return null;
        }
    }

    Vector3 OverPosition() {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit floorHit;
        Physics.Raycast(camRay, out floorHit, 20f,  1 << LayerMask.NameToLayer("Plane"));
        return floorHit.point;
    }
}
