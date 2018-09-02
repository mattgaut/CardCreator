using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour {

    public bool end_turn {
        get; private set;
    }

    public List<Command> commands {
        get; private set;
    }

    private void Awake() {
        commands = new List<Command>();
    }

    public void AddCommand(Command command) {
        if (command.ValidateCommand()) {
            commands.Add(command);
        } else {
            command.DisplayInvalid();
        }        
    }

    public void Clear() {
        commands.Clear();
    }

    public Command PopCommand() {
        Command to_ret = commands[0];
        commands.RemoveAt(0);
        return to_ret;
    }

    public void EndTurn() {
        StartCoroutine(EndTurnRoutine());
    }

    IEnumerator EndTurnRoutine() {
        end_turn = true;
        yield return null;
        end_turn = false;
    }
}

public abstract class Command {
    public Command() {

    }
    public abstract bool ValidateCommand();
    public abstract void ResolveCommand();
    public virtual void DisplayInvalid() { }
}

public class PlaySpellCommand : Command {
    protected Spell to_play;
    public PlaySpellCommand(Spell c) {
        to_play = c;
    }

    public override void ResolveCommand() {
        GameStateManager.instance.PlaySpellFromHand(to_play);
    }

    public override bool ValidateCommand() {
        return to_play.controller.current_mana >= to_play.mana_cost;
    }
}

public class PlayCreatureCommand : Command {
    Creature to_play;
    int position;
    public PlayCreatureCommand(Creature c, int position) {
        to_play = c;
        this.position = position;
    }

    public override void ResolveCommand() {
        GameStateManager.instance.PlayCreatureFromHand(to_play, position);
    }

    public override bool ValidateCommand() {
        return to_play.controller.current_mana >= to_play.mana_cost && !to_play.controller.field.full;
    }
}

public class PlayTagetedCreatureCommand : Command {
    Creature to_play;
    int position;
    IEntity target;
    public PlayTagetedCreatureCommand(Creature c, int position, IEntity target) {
        to_play = c;
        this.position = position;
        this.target = target;
    }

    public override void ResolveCommand() {
        GameStateManager.instance.PlayCreatureWithTargetFromHand(to_play, position, target);
    }

    public override bool ValidateCommand() {
        return to_play.controller.current_mana >= to_play.mana_cost && !to_play.controller.field.full 
            && to_play.mods.battlecry_info.CanTarget(target) && target.CanBeTargeted(to_play);
    }
}

public class PlayTargetedSpellCommand : PlaySpellCommand {
    IEntity target;
    public PlayTargetedSpellCommand(Spell to_play, IEntity target) : base(to_play) {
        this.to_play = to_play;
        this.target = target;
    }

    public override void ResolveCommand() {
        to_play.SetTarget(target);
        GameStateManager.instance.PlaySpellFromHand(to_play);
    }

    public override bool ValidateCommand() {
        return base.ValidateCommand() && to_play.CanTarget(target) && target.CanBeTargeted(to_play);
    }
}

public class AttackCommand : Command {
    ICombatant target, attacker;
    public AttackCommand(ICombatant target, ICombatant attacker) {
        this.target = target;
        this.attacker = attacker;
    }

    public override void ResolveCommand() {
        GameStateManager.instance.Attack(attacker, target);
    }

    public override bool ValidateCommand() {
        return GameStateManager.instance.CanAttack(attacker, target);
    }
}
