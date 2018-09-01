using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CommandManager))]
public class Player : MonoBehaviour, ICombatant, IPlayer {
    public CommandManager command_manager {
        get; private set;
    }

    public ResourceStat health {
        get { return _health; }
    }
    public int current_health {
        get { return health.current_value; }
        private set { health.current_value = value; }
    }

    public int current_mana {
        get; private set;
    }
    public int current_overloaded_mana {
        get; private set;
    }
    public int next_turn_overloaded_mana {
        get; private set;
    }
    public int max_mana {
        get; private set;
    }
    public bool frozen { get; private set; }

    public CardContainer deck { get { return _deck; } }
    public CardContainer hand { get { return _hand; } }
    public CardContainer secrets { get { return _secrets; } }
    public CardContainer stack { get { return _stack; } }
    public CardContainer field { get { return _field; } }
    public CardContainer discard { get { return _discard; } }
    public CardContainer graveyard { get { return _graveyard; } }

    public Player controller { get { return this; } }
    public EntityType entity_type { get { return EntityType.player; } }

    public int attacks_taken { get; private set; }

    public int attacks_per_turn { get { return 1; } }

    public bool retaliate { get { return false; } }

    public Stat attack { get { return _attack; } }

    public bool dead { get { return current_health <= 0; } }

    public Player player {
        get { return this; }
    }

    Stat _attack;

    [SerializeField] ResourceStat _health;
    [SerializeField] CardContainer _deck, _hand, _field, _discard, _graveyard, _stack, _secrets;

    Dictionary<Zone, CardContainer> card_containers;

    public void Awake() {
        deck.SetController(this);
        hand.SetController(this);
        field.SetController(this);
        discard.SetController(this);
        graveyard.SetController(this);
        stack.SetController(this);
        secrets.SetController(this);

        command_manager = GetComponent<CommandManager>();
        current_health = health;

        _attack = new Stat(0);

        card_containers = new Dictionary<Zone, CardContainer>();
        card_containers.Add(Zone.deck, deck);
        card_containers.Add(Zone.hand, hand);
        card_containers.Add(Zone.field, field);
        card_containers.Add(Zone.discard, discard);
        card_containers.Add(Zone.graveyard, graveyard);
        card_containers.Add(Zone.secrets, secrets);
        card_containers.Add(Zone.stack, stack);
    }

    public void NoteBeginTurn() {
        AdvanceLockedMana();
        ResetAttacksTaken();
        AddOneMaxMana();
        FillMana();

        frozen = false;
    }

    public void AdvanceLockedMana() {
        current_overloaded_mana = next_turn_overloaded_mana;
        next_turn_overloaded_mana = 0;
    }
    public int LockManaCrystals(int number_to_lock) {
        int old = next_turn_overloaded_mana;
        next_turn_overloaded_mana += number_to_lock;
        if (next_turn_overloaded_mana > 10) {
            next_turn_overloaded_mana = 10;
        }
        return next_turn_overloaded_mana - old;
    }
    public void UnlockManaCrystals() {
        next_turn_overloaded_mana = 0;
        int mana_refunded = current_overloaded_mana;
        current_overloaded_mana = 0;
        current_mana += mana_refunded;
    }
    public bool SpendMana(int mana) {
        if (mana <= current_mana) {
            current_mana -= mana;
            return true;
        } else {
            return false;
        }
    }
    public int GainMana(int mana) {
        int old = current_mana;
        current_mana += mana;
        if (current_mana > max_mana - current_overloaded_mana) {
            current_mana = max_mana - current_overloaded_mana;
        }
        return current_mana - old;
    }
    public void AddOneMaxMana(bool fill_new_mana = false) {
        SetMaxMana(max_mana + 1, fill_new_mana);
    }
    public void SetMaxMana(int count, bool fill_new_mana = false) {
        if (count > 0) {
            int old = max_mana;
            max_mana = count;
            if (max_mana > 10) {
                max_mana = 10;
            }
            if (max_mana - old > 0) {
                current_mana += max_mana - old;
            }
        }
    }
    public void FillMana() {
        current_mana = max_mana - current_overloaded_mana;
        if (current_mana < 0) {
            current_mana = 0;
        }
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            command_manager.EndTurn();
        }
    }

    public int TakeDamage(int dmg) {
        current_health -= dmg;
        return dmg;
    }

    public int Heal(IEntity source, int to_heal) {
        int old = current_health;
        current_health += to_heal;

        if (current_health > health) {
            current_health = health;
        }

        return current_health - old;
    }

    public void NoteAttack() {
        attacks_taken += 1;
    }
    public void ResetAttacksTaken() {
        attacks_taken = 0;
    }
    public bool CanAttack(ICombatant target) {
        return attack > 0 && attacks_taken < attacks_per_turn;
    }

    public bool CanBeAttacked(ICombatant attacker) {
        foreach (Creature c in controller.field.cards) {
            if (c.mods.HasMod(Modifier.taunt)) {
                return false;
            }
        }
        return true;
    }
    
    public void Freeze() {
        frozen = true;
    }

    public int TakeDamage(IDamages source, int damage) {
        current_health -= damage;
        return damage;
    }

    public int DealDamage(IDamageable target, int damage) {
        return target.TakeDamage(this, damage);
    }

    public bool CanBeTargeted(IEntity source) {
        return true;
    }

    public CardContainer GetContainer(Zone z) {
        return card_containers[z];
    }

    public int TotalSpellPower() {
        int spell_power = 0;
        foreach (Card card in field.cards) {
            if (card.mods.HasMod(Modifier.spellpower)) {
                spell_power += card.mods.spellpower_amount;
            }
        }
        return spell_power;
    }
}
