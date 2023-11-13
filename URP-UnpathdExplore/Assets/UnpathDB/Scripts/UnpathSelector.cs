using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class UnpathSelector : MonoBehaviour, IPointerClickHandler {

    public string m_Title;
    public string m_QueryTerm;
    private SqliteController m_databaseController;

    //added by M
    private TextMeshProUGUI label_TmpPro;

    private void Start() {
        m_databaseController = GameObject.FindWithTag( "DB" ).GetComponent<SqliteController>();

        //added by M
        UpdateLabelText();

    }

    public void OnPointerClick( PointerEventData eventData ) {
        if( eventData.button == 0 ) {
            m_databaseController.AddToQuery( m_QueryTerm, SqliteController.QueryType.Or );
            Debug.Log( $"Selected: {m_QueryTerm}" );

        } else {
            m_databaseController.RunQuery();
        }
    }

    // added by M

    public void HandleSelection()
    {   
        if (m_databaseController != null) {
            m_databaseController.AddToQuery(m_QueryTerm, SqliteController.QueryType.Or);

            // Reference the GameObject this script is attached to
            GameObject thisGameObject = gameObject;

            Debug.Log($"Selected: {m_QueryTerm} on GameObject: {thisGameObject.name}");
        }
    }

     private void UpdateLabelText() {

        
        label_TmpPro = GetComponentInChildren<TextMeshProUGUI>(true);

        if (label_TmpPro != null) {
            label_TmpPro.text = m_Title;
        } else {
            Debug.LogWarning("TextMeshPro component not found on the child GameObject or its descendants.");
        }
    }


}
