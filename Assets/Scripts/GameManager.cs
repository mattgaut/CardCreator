﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    static GameManager instance;

    bool game_over;
    [SerializeField] List<Player> _players;
    [SerializeField] GameStateManager gsm;
    Player active_player;

    int current_position;

    public static List<Player>  players {
        get { return new List<Player>(instance._players); }
    }
    public static Player current_player {
        get { return instance.active_player; }
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
            active_player.command_manager.Clear();
            while (!active_player.command_manager.end_turn) {
                if (active_player.command_manager.commands.Count > 0) {
                    active_player.command_manager.PopCommand().ResolveCommand();
                }
                yield return null;
            }
            active_player.GetComponent<Renderer>().material.color = Color.red;
            gsm.EndTurn(active_player);
            yield return null;
            current_position = (current_position + 1) % _players.Count;
        }
    }
}
