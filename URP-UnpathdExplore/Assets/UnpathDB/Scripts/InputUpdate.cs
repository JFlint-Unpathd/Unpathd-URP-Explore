using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InputUpdate : MonoBehaviour {

    private SqliteController m_databaseController;

    private void Start() {
        m_databaseController = GameObject.FindWithTag( "DB" ).GetComponent<SqliteController>();
    }

    public void OnClick( InputAction.CallbackContext context ) {
        if( !context.performed ) {
            return;
        }
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );

        if( Physics.Raycast( ray, out hit ) ) {
            UnpathSelector clickedObject = hit.collider.GetComponent<UnpathSelector>();
            //m_databaseController.AddToQuery( clickedObject.m_QueryTerm, SqliteController.QueryType.Or );
            m_databaseController.AddToQuery( clickedObject.m_QueryTerm, SqliteController.QueryType.And );
            Debug.Log( $"Selected: {clickedObject.m_QueryTerm}" );
        }
    }

    public void OnSpace( InputAction.CallbackContext context ) {
        if( !context.performed ) {
            return;
        }
        m_databaseController.RunQuery();
    }

    private void Update() {
        OVRInput.Update();
    }

    private void FixedUpdate() {
        OVRInput.FixedUpdate();
    }

    // added by Maria
    public void HandleSelection(UnpathSelector selectedObject) 
    {
        Debug.Log("Does this run?");

        if (selectedObject != null) {

            Debug.Log("Does this run?");
            //m_databaseController.AddToQuery(selectedObject.m_QueryTerm, SqliteController.QueryType.Or);
            m_databaseController.AddToQuery(selectedObject.m_QueryTerm, SqliteController.QueryType.And);
            Debug.Log($"Selected: {selectedObject.m_QueryTerm}");
        }
    }

    public void ExectuteQ() 
    {
        m_databaseController.RunQuery();
    }

    public void ResetQuery()
    {
        m_databaseController.ResetQuery();
    }

}

