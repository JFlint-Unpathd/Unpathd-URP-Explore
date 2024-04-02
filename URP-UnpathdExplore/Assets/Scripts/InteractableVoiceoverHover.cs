
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class InteractableVoiceoverHover : MonoBehaviour
{
    public AudioClip voiceoverClip;
    private XRBaseInteractable interactable;

    private bool isHovering = false;
    private float hoverTime = 0f;
    private const float requiredHoverTime = .5f;

    void Awake()
    {
        
        interactable = GetComponent<XRBaseInteractable>();
    }


    void Start()
    {
        

       if (interactable == null)
       {
            Debug.LogError("The XRInteractable component is missing.");
            return;
        }

        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.hoverExited.AddListener(OnHoverExited);

        
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        isHovering = true;
        hoverTime = 0f;
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        isHovering = false;
    }

    void Update()
    {
        if (isHovering)
        {
            hoverTime += Time.deltaTime;
            if (hoverTime >= requiredHoverTime)
            {
                AudioManager.instance.PlayClip(voiceoverClip);
                // Reset hoverTime and isHovering to prevent the clip from constantly restarting
                hoverTime = 0f;
                isHovering = false;
            }
        }
    }
}

