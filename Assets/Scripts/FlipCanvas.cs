using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipCanvas : MonoBehaviour {

    public static List<FlipCanvas> canvases;

    [SerializeField] Canvas canvas;

    public void Awake() {
        if (canvases == null) {
            canvases = new List<FlipCanvas>();
        }
        canvases.Add(this);
    }

    public void Flip() {
        Vector3 flip = canvas.transform.localScale;
        flip.y *= -1;
        canvas.transform.localScale = flip; 
    }

    private void OnDestroy() {
        canvases.Remove(this);
    }
}
