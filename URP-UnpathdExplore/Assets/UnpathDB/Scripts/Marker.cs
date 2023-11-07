using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour {
    public LatLng m_LatLng;
    public List<string> m_spacialNames;

    public void AddSpacialName( string spacialName ) {
        m_spacialNames.Add( spacialName );
    }
}
