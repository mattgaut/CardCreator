using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [SerializeField] Text player_text;
    [SerializeField] GameObject turn_screen_panel;
    [SerializeField] FlipCamera flip_camera;

    public void ShowTurnScreen(bool is_player_one) {
        player_text.text = is_player_one ? "Player 1's turn." : "Player 2's turn.";
        turn_screen_panel.SetActive(true);
    }

    public void HideTurnScreen() {
        turn_screen_panel.SetActive(false);
    }

    public void FlipCamera() {
        flip_camera.ToggleFlip();
        foreach (FlipCanvas c in FlipCanvas.canvases) {
            if (c != null) c.Flip();
        }
    }
}
