using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Creature))]
public class CreatureController : CardController {
    Creature creature;
    [SerializeField] CreatureFieldDisplay creature_field_display_prefab;
    [SerializeField] Collider field_box;
    FieldViewer field_viewer;
    HandViewer hand_viewer;

    CreatureFieldDisplay creature_field_display;

    float field_height;

    bool is_targeting;

    bool playing_creatue;
    Coroutine attach_to_mouse;

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
    public override void OnLeftClickDown() {
        if (card.container == card.controller.hand) {
            field_viewer = card.controller.field.GetComponent<FieldViewer>();
            hand_viewer = card.controller.hand.GetComponent<HandViewer>();
            attach_to_mouse = StartCoroutine(AttachToMouse());
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
            playing_creatue = false;
            if (!can_click) {
                card.controller.hand.GetComponent<HandViewer>().ForceUpdate();
                return;
            }
            if (Mathf.Abs(position_dragged_to.z - card.controller.field.transform.position.z) < 1f) {                
                if (creature.mods.NeedsTarget() && GameStateManager.instance.TargetExists(creature, creature.mods)) {
                    StartCoroutine(TargetingCoroutine(FindPositionInField(position_dragged_to)));
                } else {
                    card.controller.command_manager.AddCommand(new PlayCreatureCommand(creature, FindPositionInField(position_dragged_to)));
                }
            } else {
                card.controller.hand.GetComponent<HandViewer>().ForceUpdate();
            }
        }
    }
    public override void OnEndClick() {
        InterfaceManager.RemoveTargetingArrow();
        if (field_viewer != null && !is_targeting) field_viewer.StopMakeRoom();
    }

    public override void OnHoverEnd(bool was_clicked) {
        if (!is_targeting) base.OnHoverEnd(was_clicked);
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
        is_targeting = true;
        card.transform.position = field_viewer.GetPosition(position);
        display.HideCard();
        creature_field_display.ShowCard();

        bool cancelled = false;
        while (!cancelled) {
            yield return null;
            if (Input.GetMouseButtonDown(1)) {
                cancelled = true;
            }
            InterfaceManager.DrawTargetingArrow(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (Input.GetMouseButtonDown(0)) {
                GameObject hover_object = OverObject();
                if (hover_object != null) {
                    IEntity target = hover_object.GetComponent<IEntity>();
                    if (target != null) {
                        card.controller.command_manager.AddCommand(new PlayTagetedCreatureCommand(creature, position, target));
                        break;
                    }
                }
            }
        }
        if (cancelled) {
            display.ShowCard();
            creature_field_display.HideCard();
            hand_viewer.ForceUpdate();
        }
        InterfaceManager.RemoveTargetingArrow();
        is_targeting = false;
        field_viewer.StopMakeRoom();
    }

    IEnumerator AttachToMouse() {
        playing_creatue = true;
        must_drag = true;
        float y = transform.position.y + 1f;
        while (playing_creatue) {
            Vector3 mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse_position.y = y;
            transform.position = mouse_position;
            yield return null;
        }
        must_drag = false;
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
