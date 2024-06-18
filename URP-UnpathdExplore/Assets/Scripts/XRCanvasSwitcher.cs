using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit;
using CurvedUI;

public class XRCanvasSwitcher : MonoBehaviour
{
    public XRUIInputModule xrInputModule;
    public CurvedUIInputModule curvedInputModule;
    public XRRayInteractor xrRayInteractor;
    private CurvedUISettings currentCurvedCanvas;

    void Update()
    {
        // Get the currently hovered object by the XR pointer
        GameObject hoveredObject = GetHoveredObject();

        if (hoveredObject != null)
        {
            // Check if the hovered object has a CurvedUISettings component
            CurvedUISettings curvedCanvas = hoveredObject.GetComponentInParent<CurvedUISettings>();

            if (curvedCanvas != null)
            {
                // Switch to CurvedUIInputModule if not already using it
                if (currentCurvedCanvas != curvedCanvas)
                {
                    SwitchToCurvedUI();
                    currentCurvedCanvas = curvedCanvas;
                }
            }
            else
            {
                // Switch to XRUIInputModule if not already using it
                if (currentCurvedCanvas != null)
                {
                    SwitchToXRUI();
                    currentCurvedCanvas = null;
                }
            }
        }
        else
        {
            // No object is being hovered, switch to XRUIInputModule
            if (currentCurvedCanvas != null)
            {
                SwitchToXRUI();
                currentCurvedCanvas = null;
            }
        }
    }

    GameObject GetHoveredObject()
    {
        // Perform a raycast from the XR pointer to get the currently hovered object
        if (xrRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    void SwitchToCurvedUI()
    {
        curvedInputModule.enabled = true;
        xrInputModule.enabled = false;
    }

    void SwitchToXRUI()
    {
        curvedInputModule.enabled = false;
        xrInputModule.enabled = true;
    }
}
