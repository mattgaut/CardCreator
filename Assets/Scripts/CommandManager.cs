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
    protected Creature to_play;
    protected int position;
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

public class PlayTagetedCreatureCommand : PlayCreatureCommand {
    IEntity target;
    public PlayTagetedCreatureCommand(Creature c, int position, IEntity target) : base(c, position) {
        this.target = target;
    }

    public override void ResolveCommand() {
        GameStateManager.instance.PlayCreatureWithTargetFromHand(to_play, position, target);
    }

    public override bool ValidateCommand() {
        return base.ValidateCommand() && to_play.mods.CanTarget(target) && target.CanBeTargeted(to_play);
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

public class PlayWeaponCommand : Command {
    protected Weapon weapon;

    public PlayWeaponCommand(Weapon weapon) {
        this.weapon = weapon;
    }

    public override void ResolveCommand() {
        GameStateManager.instance.PlayWeaponFromHand(weapon);
    }

    public override bool ValidateCommand() {
        return weapon.controller.current_mana >= weapon.mana_cost;
    }
}

public class PlaySecretCommand : Command {
    protected Secret secret;

    public PlaySecretCommand(Secret secret) {
        this.secret = secret;
    }

    public override void ResolveCommand() {
        GameStateManager.instance.PlaySecretFromHand(secret);
    }

    public override bool ValidateCommand() {
        return secret.controller.current_mana >= secret.mana_cost;
    }
}

public class PlayWeaponWithTargetCommand : PlayWeaponCommand {
    IEntity target;

    public PlayWeaponWithTargetCommand(Weapon weapon, IEntity target) : base(weapon) {
        this.target = target;
    }

    public override void ResolveCommand() {
        GameStateManager.instance.PlayWeaponWithTargetFromHand(weapon, target);
    }

    public override bool ValidateCommand() {
        return base.ValidateCommand() && weapon.mods.CanTarget(target) && target.CanBeTargeted(weapon);
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

public class UseHeroPowerCommand : Command {
    protected Player to_use;

    public UseHeroPowerCommand(Player to_use) {
        this.to_use = to_use;
    }

    public override void ResolveCommand() {
        GameStateManager.instance.UseHeroPower(to_use);
    }

    public override bool ValidateCommand() {
        return to_use.can_use_hero_power;
    }
}

public class UseTargetedHeroPowerCommand : UseHeroPowerCommand {
    IEntity target;

    public UseTargetedHeroPowerCommand(Player to_use, IEntity target) : base(to_use) {
        this.target = target;
    }

    public override void ResolveCommand() {
        GameStateManager.instance.UseTargetedHeroPower(to_use, target);
    }

    public override bool ValidateCommand() {
        return base.ValidateCommand() && to_use.hero_power.CanTarget(target) && target.CanBeTargeted(to_use.hero_power);
    }
}