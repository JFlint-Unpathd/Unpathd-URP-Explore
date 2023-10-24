using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject[] postProcessingPanels;

    public void TurnOffPanels()
    {
        for (int i = 0; i < postProcessingPanels.Length; i++)
        {
            postProcessingPanels[i].SetActive(false);
        }
    }
}
