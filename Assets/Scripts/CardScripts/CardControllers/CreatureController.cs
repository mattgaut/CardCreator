using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Creature))]
public class CreatureController : CardController {
    Creature creature;
    [SerializeField] CreatureFieldDisplay creature_field_display_prefab;
    [SerializeField] Collider field_box;
    FieldViewer field_viewer;

    CreatureFieldDisplay creature_field_display;

    float field_height;

    bool selecting_targeting;
    int position_saved;

    protected override void Awake() {
        base.Awake();
        creature = GetComponent<Creature>();

        creature_field_display = Instantiate(creature_field_display_prefab, transform);
        creature_field_display.SetCard(creature);
    }

    protected override void Update() {
        base.Update();
    }

    protected override void UpdateContainer() {
        if (last_container != card.container) {
            last_container = card.container;
            if (last_container != null && last_container.visible) {
                SetFieldDisplay(last_container == card.controller.field);
            } else {
                HideCard();
            }
        }
    }
    public override void OnClick() {
    }
    public override void OnMouseDown() {
        if (card.container == card.controller.hand) {
            field_viewer = card.controller.field.GetComponent<FieldViewer>();
        }
    }
    public override void OnHoldDrag(GameObject dragged_to, Vector3 position_dragged_to) {
        if (card.container == card.controller.field && creature.can_attack) {
            InterfaceManager.DrawTargetingArrow(transform.position, position_dragged_to);
        } else if (card.container == card.controller.hand) {
            if (Mathf.Abs(position_dragged_to.z - card.controller.field.transform.position.z) < 1f) {
                field_viewer.MakeRoom(FindPositionInField(position_dragged_to));
            }
        }
    }
    public override void OnFinishDrag(GameObject dragged_to, Vector3 position_dragged_to) {
        if (card.container == card.controller.field) {
            if (dragged_to != null) {
                ICombatant c = dragged_to.GetComponent<ICombatant>();
                if (c != null) {
                    card.controller.command_manager.AddCommand(new AttackCommand(c, creature));
                }
            }
        } else if (card.container == card.controller.hand) {
            if (Mathf.Abs(position_dragged_to.z - card.controller.field.transform.position.z) < 1f) {
                if (creature.mods.NeedsTarget() && GameStateManager.instance.TargetExists(creature, creature.mods)) {
                    StartCoroutine(TargetingCoroutine(FindPositionInField(position_dragged_to)));
                } else {
                    card.controller.command_manager.AddCommand(new PlayCreatureCommand(creature, FindPositionInField(position_dragged_to)));
                }
            }
        }
    }
    public override void OnEndClick() {
        InterfaceManager.RemoveTargetingArrow();
        if (field_viewer != null) field_viewer.StopMakeRoom();
    }

    void SetFieldDisplay(bool field_display) {
        if (field_display) {
            field_box.enabled = true;
            box.enabled = false;
            creature_field_display.ShowCard();
            display.HideCard();
            creature_field_display.UpdateDisplay();
        } else {
            field_box.enabled = false;
            box.enabled = true;
            creature_field_display.HideCard();
            display.ShowCard();
        }
    }

    int FindPositionInField(Vector3 position) {
        int i = 0;
        for (; i < card.controller.field.count; i++) {
            if (position.x < card.controller.field.cards[i].transform.position.x) {
                break;
            }
        }
        return i;
    }

    protected override void HideCard() {
        base.HideCard();
        field_box.enabled = false;
        creature_field_display.HideCard();
    }

    IEnumerator TargetingCoroutine(int position) {
        while (!Input.GetMouseButtonDown(1)) {
            yield return null;
            InterfaceManager.DrawTargetingArrow(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (Input.GetMouseButtonDown(0)) {
                GameObject hover_object = OverObject();
                if (hover_object != null) {
                    IEntity target = hover_object.GetComponent<IEntity>();
                    if (target != null) {
                        card.controller.command_manager.AddCommand(new PlayTagetedCreatureCommand(creature, position, target));
                    }
                }
                break;
            }
        }
        InterfaceManager.RemoveTargetingArrow();
    }

    GameObject OverObject() {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit floorHit;
        Physics.Raycast(camRay, out floorHit, 20f);
        if (floorHit.collider) {
            return floorHit.collider.gameObject;
        } else {
            return null;
        }
    }
}
