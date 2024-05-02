using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Text;

public class ReRefine : MonoBehaviour
{

    private SqliteController m_databaseController;
    private ResetRefine resetRefineScript;


    // Start is called before the first frame update
    void Start()
    {

        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(selectEntered);
        
        m_databaseController = GameObject.FindWithTag( "DB" ).GetComponent<SqliteController>();
        resetRefineScript = GameObject.FindWithTag("ResetRefine").GetComponent<ResetRefine>();
    }

    private void selectEntered(SelectEnterEventArgs args)
    {
        resetRefineScript.DestroyResultsScene();
        resetRefineScript.CreateInitialScene();
    }

}
