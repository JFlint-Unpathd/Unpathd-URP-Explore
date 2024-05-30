using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(EventTrigger))]
public class ButtonHoverAudio : MonoBehaviour
{
    public AudioClip hoverClip;

    private Coroutine hoverCoroutine;

    void Start()
    {
        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

        // PointerEnter event
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
        pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        pointerEnterEntry.callback.AddListener((eventData) => { StartHover(); });
        trigger.triggers.Add(pointerEnterEntry);

        //PointerExit event
        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
        pointerExitEntry.eventID = EventTriggerType.PointerExit;
        pointerExitEntry.callback.AddListener((eventData) => { StopHover(); });
        trigger.triggers.Add(pointerExitEntry);
    }

    private void StartHover()
    {
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
        }
        hoverCoroutine = StartCoroutine(HoverAction());
    }

    private void StopHover()
    {
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
            hoverCoroutine = null;
        }
    }

    private IEnumerator HoverAction()
    {
         yield return new WaitForSecondsRealtime(1f);
        AudioManager.instance.PlayClip(hoverClip);
        
    }
}
