using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotControl : MonoBehaviour
{
  private PlayerControl PC;
  public bool _fixed;
  private bool _mouse_active = false;

  private void DoDragInv(BaseEventData eventData)
  {
    if (!_fixed)
    {
      PC.OutInvUpd();
      _mouse_active = true;
    }
    else
    {
      PC.InInvUpd();
      _mouse_active = false;
    }
  }

  public void Awake()
  {
    PC = FindObjectOfType<PlayerControl>();
    EventTrigger _event = gameObject.AddComponent<EventTrigger>();
    EventTrigger.Entry clickEvent = new EventTrigger.Entry()
    {
      eventID = EventTriggerType.PointerClick
    };
    clickEvent.callback.AddListener(DoDragInv);
    _event.triggers.Add(clickEvent);
  }
}
