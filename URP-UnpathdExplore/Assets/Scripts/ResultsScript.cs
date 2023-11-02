using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsScript : MonoBehaviour
{
    public GameObject idLabel;
    public GameObject resTitle;
    public GameObject resPlace;

    public void SetResult(string idLabel, string resTitle, string resPlace)
    {
        this.idLabel.GetComponent<TMP_Text>().text = idLabel;
        this.resTitle.GetComponent<TMP_Text>().text = resTitle;
        this.resPlace.GetComponent<TMP_Text>().text = resPlace;

    }
}
