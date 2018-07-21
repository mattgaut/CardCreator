using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour {

    static InterfaceManager instance;
    [SerializeField] LineRenderer line_renderer;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public static void DrawTargetingArrow(Vector3 source, Vector3 target) {
        source.y = 5;
        target.y = 5;
        if (!instance.line_renderer.enabled) instance.line_renderer.enabled = true;
        instance.line_renderer.SetPositions(new Vector3[] { source, target } );
    }

    public static void RemoveTargetingArrow() {
        instance.line_renderer.enabled = false;
    }
}
