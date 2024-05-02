using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Text;

public class ExecuteQuery : MonoBehaviour
{
    private SqliteController m_databaseController;
    private ResetRefine resetRefineScript;

    [Header("Warning Settings")]
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private AudioClip warningClip;

    // Start is called before the first frame update
    void Start()
    {
        warningPanel.SetActive(false);
        descriptionPanel.SetActive(true);

        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(selectEntered);
        

        m_databaseController = GameObject.FindWithTag( "DB" ).GetComponent<SqliteController>();
        resetRefineScript = GameObject.FindWithTag("ResetRefine").GetComponent<ResetRefine>();
    }

    private void selectEntered(SelectEnterEventArgs args)
    {
        List<string> currentQueryList = m_databaseController.GetCurrentQueryList();

        if(currentQueryList.Count == 0) 
        {
            HandleWarning();
            return;
        }

        StringBuilder builder = new StringBuilder();
        for(int i = 0, len = currentQueryList.Count; i < len; i++) {
            builder.Append(currentQueryList[i]);
        }

        if(builder.Length > 0) {

            Debug.Log("DatabaseController: " + m_databaseController);
            Debug.Log("ResetRefineScript: " + resetRefineScript);
            
            
            resetRefineScript.DestroyInitialScene();
            StartCoroutine(ExecuteQueryAndCreateResultsSceneAfterDelay());
        }
    }

    private IEnumerator ExecuteQueryAndCreateResultsSceneAfterDelay()
    {
        yield return new WaitForSeconds(1); // Wait for 1 second
        m_databaseController.RunQuery();
        resetRefineScript.CreateResultsScene();
    }

    public void HandleWarning() {
        warningPanel.SetActive(true);
        descriptionPanel.SetActive(false);
        AudioManager.instance.PlayClip(warningClip);
        StartCoroutine(DisableWarning());
    }

    private IEnumerator DisableWarning() {
        yield return new WaitWhile(() => AudioManager.instance.IsPlaying());
        yield return new WaitForSecondsRealtime(5);
        warningPanel.SetActive(false);
        descriptionPanel.SetActive(true);
    }


}
