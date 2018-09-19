using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CommandManager))]
public class Player : MonoBehaviour, ICombatant, IPlayer {
    public CommandManager command_manager {
        get; private set;
    }

    public HeroPower hero_power {
        get { return _hero_power; }
    }

    public ResourceStat health {
        get { return _health; }
    }
    public int armor {
        get { return _armor; }
        private set { _armor = value; }
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
    public CardContainer equip { get { return _equip; } }

    public Player controller { get { return this; } }
    public EntityType entity_type { get { return EntityType.player; } }

    public int times_hero_power_used { get; private set; }

    public int hero_power_uses_per_turn { get { return 1; } }

    public bool can_use_hero_power { get { return times_hero_power_used < hero_power_uses_per_turn && current_mana >= hero_power.mana_cost; } }

    public int attacks_taken { get; private set; }

    public int attacks_per_turn { get { return 1; } }

    public bool can_attack { get { return attacks_taken < attacks_per_turn && !frozen && attack > 0; } }

    public bool retaliate { get { return false; } }

    public Stat attack { get { return _attack; } }

    public bool dead { get { return current_health <= 0; } }

    public int cards_played_this_turn { get; private set; }

    public bool combo_active { get { return cards_played_this_turn > 0; } }

    public Player player {
        get { return this; }
    }

    public Weapon weapon {
        get; private set;    
    }

    Stat _attack;

    [SerializeField] ResourceStat _health;
    [SerializeField] int _armor;
    [SerializeField] CardContainer _deck, _hand, _field, _discard, _graveyard, _stack, _secrets, _equip;

    [SerializeField] HeroPower _hero_power;

    Dictionary<Zone, CardContainer> card_containers;

    Dictionary<int, AttackRestriction> attack_restrictions;
    int next_restriction;

    public void Awake() {
        command_manager = GetComponent<CommandManager>();
        current_health = health;

        _attack = new Stat(0);

        attack_restrictions = new Dictionary<int, AttackRestriction>();
        next_restriction = 0;

        card_containers = new Dictionary<Zone, CardContainer>();
        card_containers.Add(Zone.deck, deck);
        card_containers.Add(Zone.hand, hand);
        card_containers.Add(Zone.field, field);
        card_containers.Add(Zone.discard, discard);
        card_containers.Add(Zone.graveyard, graveyard);
        card_containers.Add(Zone.secrets, secrets);
        card_containers.Add(Zone.stack, stack);
        card_containers.Add(Zone.equipment, equip);

        foreach (CardContainer container in card_containers.Values) {
            container.SetController(this); 
        }
    }

    public void NoteBeginTurn() {
        AdvanceLockedMana();
        ResetAttacksTaken();
        AddOneMaxMana();
        FillMana();

        times_hero_power_used = 0;
        cards_played_this_turn = 0;

        frozen = false;
    }

    public void NoteUsedHeroPower() {
        times_hero_power_used++;
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

    public int Heal(IEntity source, int to_heal) {
        if (to_heal <= 0) {
            return 0;
        }
        int old = current_health;
        current_health += to_heal;

        if (current_health > health) {
            current_health = health;
        }

        return current_health - old;
    }

    public int GainArmor(IEntity source, int to_gain) {
        if (to_gain <= 0) {
            return 0;
        }
        int old = armor;
        armor += to_gain;

        return armor - old;
    }

    public void NoteAttack() {
        attacks_taken += 1;
        if (weapon != null) {
            weapon.durability.current_value -= 1;
        }
    }
    public void ResetAttacksTaken() {
        attacks_taken = 0;
    }

    public int AddAttackRestriction(AttackRestriction attack_restriction) {
        attack_restrictions.Add(next_restriction, attack_restriction);
        return next_restriction++;
    }

    public bool RemoveAttackRestriction(int id) {
        return attack_restrictions.Remove(id);
    }

    public bool CanAttack(ICombatant target) {
        foreach (AttackRestriction restriction in attack_restrictions.Values) {
            if (!restriction.CanAttack(target)) {
                return false;
            }
        }
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

    public int TakeDamage(IEntity source, int damage) {
        int damage_to_deal = damage;        
        if (armor < damage_to_deal) {
            damage_to_deal -= armor;
            armor = 0;
            current_health -= damage_to_deal;
        } else {
            armor -= damage;
        }        
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

    public void NotePlayedCard() {
        cards_played_this_turn++;
    }

    public void SetWeapon(Weapon new_weapon) {
        if (weapon != null) {
            attack.RemoveBuff(weapon.attack);
        }
        weapon = new_weapon;
        if (weapon != null) {
            attack.ApplyBuff(weapon.attack);
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            command_manager.EndTurn();
        }
    }
}
