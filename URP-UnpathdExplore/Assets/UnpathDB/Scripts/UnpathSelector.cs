using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UnpathSelector : MonoBehaviour, IPointerClickHandler {

    public string m_QueryTerm;

    private SqliteController m_databaseController;

    private void Start() {
        m_databaseController = GameObject.FindWithTag( "DB" ).GetComponent<SqliteController>();

    }

    public void OnPointerClick( PointerEventData eventData ) {
        if( eventData.button == 0 ) {
            m_databaseController.AddToQuery( m_QueryTerm, SqliteController.QueryType.Or );
            Debug.Log( $"Selected: {m_QueryTerm}" );

        } else {
            m_databaseController.RunQuery();
        }
    }

}
