using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class OnBeginEdit : MonoBehaviour, ISelectHandler {

    InputField input_field;

    [SerializeField] UnityEvent on_select;

    public void OnSelect(BaseEventData eventData) {
        on_select.Invoke();
    }

    void Start () {
        input_field = GetComponent<InputField>();
	}

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (EventSystem.current.currentSelectedGameObject == input_field.gameObject) {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}
