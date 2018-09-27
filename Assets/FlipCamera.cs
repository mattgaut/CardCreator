using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class FlipCamera : MonoBehaviour {

    Camera attached_camera;
    [SerializeField] bool flip_vertical;

    public void ToggleFlip() {
        flip_vertical = !flip_vertical;
    }

    void Awake() {
        attached_camera = GetComponent<Camera>();
    }
    void OnPreCull() {
        attached_camera.ResetWorldToCameraMatrix();
        attached_camera.ResetProjectionMatrix();
        Vector3 scale = new Vector3(1, flip_vertical ? -1 : 1, 1);
        attached_camera.projectionMatrix = attached_camera.projectionMatrix * Matrix4x4.Scale(scale);
    }
    void OnPreRender() {
        GL.invertCulling = flip_vertical;
    }

    void OnPostRender() {
        GL.invertCulling = false;
    }
}