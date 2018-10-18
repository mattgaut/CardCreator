using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipCanvas : MonoBehaviour {

    static List<FlipCanvas> canvases;
    static bool flipped;

    [SerializeField] Canvas canvas;
    [SerializeField] bool flip_x_instead;

    public static void FlipAll() {
        flipped = !flipped;
        foreach (FlipCanvas canvas in canvases) {
            canvas.Flip();
        }
    }

    void Awake() {
        if (canvases == null) {
            canvases = new List<FlipCanvas>();
        }
        canvases.Add(this);
        if (flipped) {
            Flip();
        }
    }

    void Flip() {
        Vector3 flip = canvas.transform.localScale;
        if (flip_x_instead) {
            flip.x *= -1;
        } else {
            flip.y *= -1;
        }
        canvas.transform.localScale = flip;
    }

    private void OnDestroy() {
        canvases.Remove(this);
    }
}
