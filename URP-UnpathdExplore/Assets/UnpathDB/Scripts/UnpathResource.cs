using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnpathResource : MonoBehaviour {

    public string m_Name;
    public string m_Title;
    public string m_Description;
    public LatLng m_LatLng;
    public List<string> m_ids;
    
    // added by M
    public TextMeshProUGUI resultTextTMP;
    public bool IsSelected { get; set; }
    
}
