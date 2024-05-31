using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPanel : MonoBehaviour {

    private static InfoPanel _instance;

    private TMPro.TMP_Text m_title;
    private TMPro.TMP_Text m_desc;
    private TMPro.TMP_Text m_label;
     private Transform m_parent;

    private Transform m_playerTransform;

    private void Awake() {
        _instance = this;
        TMPro.TMP_Text[] texts = GetComponentsInChildren<TMPro.TMP_Text>();
        m_title = texts[0];
        m_desc = texts[1];
        m_label = texts[2];
    }

    private void Start() {
        m_playerTransform = Camera.main.transform;
        GetComponent<Canvas>().worldCamera = Camera.main;
        gameObject.SetActive( false );
    }

    public void SetInfo( string title, string desc, string label ) {
        m_title.text = title;
        m_desc.text = desc;
        m_label.text = label;
    }
    
    // public static bool Show( Transform parent, string title, string desc, string label ) {
    //     // if we're showing the same parent, actually hide... this is prob not the best way to do this, but it makes it simple
    //     if( _instance.transform.parent == parent ) {
    //         Hide();
    //         return false;
    //     }
    //     _instance.SetInfo( title, desc, label );
    //     if( !_instance.isActiveAndEnabled ) {
    //         _instance.gameObject.SetActive( true );
    //     }
    //     _instance.transform.SetParent( parent, false );
    //     return true;
    // }

    public static bool Show( Transform parent, string title, string desc, string label ) {
    // if we're showing the same parent, actually hide... this is prob not the best way to do this, but it makes it simple
    if( _instance.m_parent == parent ) {
        _instance.m_parent = null;
        Hide();
        return false;
    }
    _instance.SetInfo( title, desc, label );
    if( !_instance.isActiveAndEnabled ) {
        _instance.gameObject.SetActive( true );
    }
    _instance.transform.position = parent.position;
    _instance.m_parent = parent;
    return true;
    }

    // public static void Hide() {
    //     _instance.transform.SetParent( null, false );
    //     if( _instance.isActiveAndEnabled ) {
    //         _instance.gameObject.SetActive( false );
    //     }
    // }

    public static void Hide() {
    if( _instance.isActiveAndEnabled ) {
        _instance.gameObject.SetActive( false );
    }
}

    private void Update() {
        transform.LookAt( m_playerTransform );
    }
}
