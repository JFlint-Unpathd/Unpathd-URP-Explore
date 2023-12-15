using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnpathResource : MonoBehaviour {
    
    //original giving me errors
    //public string m_Name;
    public string m_Label;
    public string m_Title;
    public string m_Description;
    public string m_Placename;
    public LatLng m_LatLng;
    public List<string> m_ids;
    
    // added by M
    //public TextMeshProUGUI resultTextTMP;
    public bool isSelected { get; set; }
    public bool isHovered { get; set; }

    public TextMeshProUGUI labelText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI placenameText;
    
}
