using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    static GameManager instance;

    bool game_over;
    [SerializeField] List<Player> _players;
    [SerializeField] GameStateManager gsm;
    [SerializeField] UIManager ui_manager;
    Player active_player;

    int current_position;

    public static List<Player>  players {
        get { return new List<Player>(instance._players); }
    }
    public static Player current_player {
        get { return instance.active_player; }
    }
    public static List<Player> OtherPlayers(Player player) {
        List<Player> to_return = players;
        to_return.Remove(player);

        return to_return;
    }


	// Use this for initialization
	void Awake () {
		if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
	}

    void Start() {
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop() {        
        gsm.BeginGame();
        while (!game_over) {
            active_player = _players[current_position];
            active_player.GetComponent<Renderer>().material.color = Color.blue;
            gsm.BeginTurn(active_player);
            yield return null;
            ui_manager.HideTurnScreen();
            active_player.command_manager.Clear();
            while (!active_player.command_manager.end_turn || !gsm.can_process_command) {
                if (active_player.command_manager.commands.Count > 0 && gsm.can_process_command) {
                    Command command = active_player.command_manager.PopCommand();
                    if (command.ValidateCommand()) {
                        command.ResolveCommand();
                    } else {
                        command.OnFail();
                    }
                }
                yield return null;
            }
            yield return null;
            active_player.GetComponent<Renderer>().material.color = Color.red;
            gsm.EndTurn(active_player);
            current_position = (current_position + 1) % _players.Count;
            ui_manager.ShowTurnScreen(current_position == 0);
            ui_manager.FlipCamera();
            while (!Input.GetKeyDown(KeyCode.Escape)) {
                yield return null;
            }
            yield return null;
        }
    }
}
