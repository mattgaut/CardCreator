using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    static GameManager instance;

    bool game_over;
    [SerializeField] List<Player> _players;
    [SerializeField] GameStateManager gsm;
    [SerializeField] UIManager ui_manager;
    [SerializeField] CardDatabase card_database;
    [SerializeField] CardSelector card_selector;

    [SerializeField] HeroPowers hero_powers;

    Player active_player;

    int current_position;

    Coroutine game_loop;

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
    public static void KillPlayers(List<Player> dead_players) {
        if (dead_players.Count == players.Count) {
            instance.EndInDraw();
        } else {
            foreach (Player p in dead_players) {
                instance._players.Remove(p);
            }
            if (instance._players.Count == 1) {
                instance.EndPlayerWin(players[0]);
            }
        }
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
        LoadPlayers(new List<DeckFile>() { MainMenuController.player_1_deckfile, MainMenuController.player_2_deckfile });

        game_loop = StartCoroutine(GameLoop());
    }

    void LoadPlayers(List<DeckFile> decklists) {
        if (decklists.Count != players.Count) {
            return;
        }
        if (decklists.Contains(null)) {
            return;
        }

        for (int i = 0; i < players.Count; i++) {
            LoadDecklist(players[i], new Decklist(decklists[i]));
            LoadHeroClass(players[i], decklists[i].deck_class);
        }
    }

    void LoadDecklist(Player player, Decklist decklist) {

        foreach (int card_id in decklist.GetIds()) {
            Card card_prefab = card_database.GetCard(card_id);
            if (card_prefab != null) {
                Card card = Instantiate(card_prefab);
                player.deck.AddCard(card);
            }
        }        
    }

    void LoadHeroClass(Player p, Player.Class player_class) {
        p.LoadHeroPowerFromPrefab(hero_powers.GetPrefab(player_class));
    }

    IEnumerator GameLoop() {        
        gsm.BeginGame();

        ui_manager.ShowMulliganScreen(true);
        while (!Input.GetKeyDown(KeyCode.Escape)) {
            yield return null;
        }
        ui_manager.HideTurnScreen();

        yield return gsm.Mulligan(players[0], 3);

        ui_manager.ShowMulliganScreen(false);
        while (!Input.GetKeyDown(KeyCode.Escape)) {
            yield return null;
        }
        Flip();
        ui_manager.HideTurnScreen();

        yield return gsm.Mulligan(players[1], 4);

        ui_manager.ShowTurnScreen(true);
        while (!Input.GetKeyDown(KeyCode.Escape)) {
            yield return null;
        }
        Flip();
        ui_manager.HideTurnScreen();

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
            Flip();
            while (!Input.GetKeyDown(KeyCode.Escape)) {
                yield return null;
            }
            yield return null;
        }
    }

    void EndInDraw() {
        game_over = true;

        ui_manager.ShowEndScreen();

        StopCoroutine(game_loop);
    }

    void EndPlayerWin(Player p) {
        game_over = true;

        ui_manager.ShowEndScreen();

        StopCoroutine(game_loop);
    }

    void Flip() {
        card_selector.Flip();
        ui_manager.FlipCamera();
    }
}

[System.Serializable]
class HeroPowers {

    [SerializeField] HeroPower druid, hunter, rouge, mage, warrior, warlock, palladin, priest, shaman;

    public HeroPower GetPrefab(Player.Class player_class) {
        if (player_class == Player.Class.druid) {
            return druid;
        }
        if (player_class == Player.Class.hunter) {
            return hunter;
        }
        if (player_class == Player.Class.mage) {
            return mage;
        }
        if (player_class == Player.Class.palladin) {
            return palladin;
        }
        if (player_class == Player.Class.priest) {
            return priest;
        }
        if (player_class == Player.Class.rouge) {
            return rouge;
        }
        if (player_class == Player.Class.shaman) {
            return shaman;
        }
        if (player_class == Player.Class.warlock) {
            return warlock;
        }
        if (player_class == Player.Class.warrior) {
            return warrior;
        }
        return null;
    }
}