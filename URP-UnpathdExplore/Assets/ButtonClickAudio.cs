using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class ButtonClickAudio : MonoBehaviour
{
    public AudioClip clickClip;

    private void Start()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();

        // PointerClick event
        EventTrigger.Entry pointerClickEntry = new EventTrigger.Entry();
        pointerClickEntry.eventID = EventTriggerType.PointerClick;
        pointerClickEntry.callback.AddListener((eventData) => { OnButtonClick(); });
        trigger.triggers.Add(pointerClickEntry);
    }

    private void OnButtonClick()
    {
        AudioManager.instance.PlayClip(clickClip);
    }
}
