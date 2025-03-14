using System.Collections;
using UnityEngine;

public class ManagePanels : MonoBehaviour
{
    public GameObject panel1;
    public GameObject panel2;
    public GameObject panel3;
    public GameObject panel4;

    void Start()
    {
        StartCoroutine(DisablePanelsCoroutine());
    }

    IEnumerator DisablePanelsCoroutine()
    {
        Debug.Log("Disabling ui panels...");
        yield return null;
        panel1.SetActive(false);
        panel2.SetActive(false);
        panel3.SetActive(false);
        panel4.SetActive(false);
    }
}
