using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;


public class SqliteController : MonoBehaviour {
    // Resources:
    // https://www.mono-project.com/docs/database-access/providers/sqlite/

    // Mono DLL in MonoBleedingEdge/lib/mono/unity

    public GameObject m_ResourcePrefab;
    private GameObject m_root;

    private const string DBName = @"unpath.db";

    private string SelectAllDBTemplate = "SELECT * FROM resource";
    private string SelectFromDBTemplate = @"SELECT {0} FROM {1} WHERE {2}";
    private string SelectFromWithJoinDBTemplate = @"SELECT {0} FROM {1} JOIN {2} ON {3}";
   

    private SqliteConnection m_connection;
    private SqliteCommand m_command;

    private Dictionary<string, UnpathResource> m_resourceDict = new Dictionary<string, UnpathResource>();

    private List<string> m_orQueryList = new List<string>();
    private List<string> m_andQueryList = new List<string>();
    private List<string> m_currentQueryList = new List<string>();

    //added by M 
    // The list of currently selected UnpathResource objects for zoom logic
    private List<UnpathResource> selectionList = new List<UnpathResource>();
    public Material selectionColor;
    public Material originalMaterial;
    
    public GameObject zoomObject;
    private bool isActivated = false;
    //... 

    public enum QueryType {
        None,
        And,
        Or
    }

    private void Start() {
        string dbPath = Path.Combine( Application.persistentDataPath, "data", DBName );
        Debug.Log( $"DB path: {dbPath}" );

        m_root = new GameObject( "root" );

        // We want to copy our database ot the persistent data dir, if necessary
        if( !File.Exists( dbPath ) ) {
            StartCoroutine( CopyFromStreamingAssetsToPersistentData() );
        } else {
            InitDB( dbPath );
        }
        
        //added by M for zoom logic
        XRGrabInteractable grabInteractable = zoomObject.GetComponent<XRGrabInteractable>();

        grabInteractable.onHoverEntered.AddListener(HandleHoverEnter);
        grabInteractable.onHoverExited.AddListener(HandleHoverExit);
        grabInteractable.onSelectEntered.AddListener(HandleSelectEnter);
        grabInteractable.onSelectExited.AddListener(HandleSelectExit);
    }

    private void OnDestroy() {
        m_command?.Dispose();
        m_connection?.Dispose();
    }

    //added by Maria
    public SqliteDataReader ExecuteCommandWithJoin( string selections, string tableNames, string joinTable, string joinCondition, string matches ) {
    string commandText = $"SELECT {selections} FROM {tableNames} LEFT JOIN {joinTable} ON {joinCondition} WHERE {matches}";
    m_command.CommandText = commandText;
    Debug.Log( m_command.CommandText );
    return m_command.ExecuteReader();
    }

    // ORIGINAL BY BRUCE
    // public SqliteDataReader ExecuteCommand( string selections, string tableNames, string matches ) {
    //     m_command.CommandText = string.Format( SelectFromDBTemplate, selections, tableNames, matches );
    //     Debug.Log( m_command.CommandText );
    //     return m_command.ExecuteReader();
    // }


    private void InitDB( string dbPath ) {
        m_connection = new SqliteConnection( "URI=file:" + dbPath );
        m_connection.Open();
        m_command = m_connection.CreateCommand();

        //InstanciateAllAtLatLng();
    }

    

    /// <summary>
    /// The actual database file needs to be read from disk, but streaming it as an object every time is a pain so copy it to persistent data folder.
    /// </summary>
    /// <returns></returns>
    IEnumerator CopyFromStreamingAssetsToPersistentData() {
        string filePath = Path.Combine( Application.streamingAssetsPath, DBName );
        byte[] result;
        if( filePath.Contains( "://" ) || filePath.Contains( ":///" ) ) {
            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get( filePath );
            yield return www.SendWebRequest();
            result = www.downloadHandler.data;
        } else {
            result = File.ReadAllBytes( filePath );
        }
        Directory.CreateDirectory( Path.Combine( Application.persistentDataPath, "data" ) );
        string dbPath = Path.Combine( Application.persistentDataPath, "data", DBName );
        File.WriteAllBytes( dbPath, result );
        InitDB( dbPath );
    }

    /// <summary>
    /// Add single query term to list based on QueryType. If type is None, reset the query list. If list already contains query, remove it.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="type"></param>
    public void AddToQuery( string query, QueryType type ) {
        if( m_currentQueryList.Count == 0 ) {
            m_currentQueryList.Add( query ); // No prefix for first item
            return;
        }
        if( m_currentQueryList.Contains( query ) ) {
            m_currentQueryList.RemoveAt( 0 );// edge case of the first NOT having AND/OR prefix
            return;
        }
        if( type == QueryType.Or ) {
            string q = $"OR {query} ";
            if( m_currentQueryList.Contains( q ) ) {
                RemoveFromQuery( q );
            } else {
                m_currentQueryList.Add( q );
            }

        } else if( type == QueryType.And ) {
            string q = $"AND {query} ";
            if( m_currentQueryList.Contains( q ) ) {
                RemoveFromQuery( q );
            } else {
                m_currentQueryList.Add( q );
            }
        } else {
            ResetQuery();
        }
    }

    /// <summary>
    /// Remove a single term from our list, making sure to fix the AND/OR prefixes
    /// </summary>
    /// <param name="query"></param>
    public void RemoveFromQuery( string query ) {
        for( int i = 0, len = m_currentQueryList.Count; i < len; i++ ) {
            if( m_currentQueryList[i].Contains( query ) ) {
                m_currentQueryList.RemoveAt( i );
                if( i == 0 ) {
                    m_currentQueryList[0] = m_currentQueryList[0].Remove( 0, m_currentQueryList.IndexOf( " " ) + 1 );// Remove the AND/OR from the start of the string
                }
                return;
            }
        }
    }

    public void ResetQuery() {
        m_currentQueryList.Clear();
    }

    public void RunQuery() {
       
        StringBuilder builder = new StringBuilder();
        for( int i = 0, len = m_currentQueryList.Count; i < len; i++ ) {
            builder.Append( m_currentQueryList[i] );
        }
        
        // Original by Bruce
        // string selections = "*";
        // string tableNames = "resource";
        // int count = 0;
        // SqliteDataReader reader = ExecuteCommand( selections, tableNames, builder.ToString() );

        string selections = "*";
        string tableNames = "resource";
        string joinTable = "extra";
        string joinCondition = "resource.PK=extra.PK";
        int count = 0;

        SqliteDataReader reader = ExecuteCommandWithJoin( selections, tableNames, joinTable, joinCondition, builder.ToString() );
        while( reader.Read() ) {
            string title = reader.GetString( reader.GetOrdinal( "title" ) ); // this could be optimized to just use the bare integer, once the table layout has been finalised.
            string id = reader.GetString( reader.GetOrdinal( "ids" ) );
            string desc = reader.GetString( reader.GetOrdinal( "description" ) );
            string placename = reader.GetString( reader.GetOrdinal( "placename" ) );


            int latOrdinal = reader.GetOrdinal( "lat" );
            if( reader.GetFieldType( latOrdinal ) == typeof( string ) ) {
                string latString = reader.GetString( latOrdinal );
                string lngString = reader.GetString( reader.GetOrdinal( "lng" ) );
                double lat = 0f;
                double lng = 0f;
                if( double.TryParse( latString, out lat ) && double.TryParse( lngString, out lng ) ) {
                    GameObject obj;
                    obj = Instantiate( m_ResourcePrefab, new Vector3( (float)lng, 0f, (float)(lat - 50.0) ), Quaternion.identity );
                    obj.name = title + "__" + id;
                    obj.transform.SetParent( m_root.transform );
                    UnpathResource res = obj.AddComponent<UnpathResource>();
                    res.m_LatLng = new LatLng( lat, lng );

                     

                    //added by M for Zoom logic
                    //Get the XRSimpleInteractable component and set up the OnSelect event.
                    XRSimpleInteractable interactable = obj.GetComponent<XRSimpleInteractable>();
                    interactable.onHoverEntered.AddListener((interactor) => {
                        OnResourceSelected(id);
                    });

                    //added by M
                    // Find the TextMeshProUGUI component in child objects
                    TextMeshProUGUI textMeshPro = obj.GetComponentInChildren<TextMeshProUGUI>(true);
                    if (textMeshPro != null)
                    {
                        textMeshPro.text = $"Title: {title}";
                        //textMeshPro.text = $"Title: {title}, ID: {id}, Description: {desc}, Place Name: {placename}";
                    }

                    m_resourceDict.Add( id, res );
                    ++count;
                }
            }
        }
        StaticBatchingUtility.Combine( m_root );
        Debug.Log( $"Object count: {count}" );
    }
    
    // added by M for zoom logic
        private void OnResourceSelected(string selectedId) {
            UnpathResource selectedResource = m_resourceDict[selectedId];
            if (selectedResource.IsSelected) {

            //Debug.Log("Already Selected");
            zoomObject.SetActive(true);

                // // If the resource is already selected, deselect it.
                // selectedResource.IsSelected = false;
                // selectionList.Remove(selectedResource);

                // // Change the material back to the original material.
                // selectedResource.GetComponent<Renderer>().material = originalMaterial;
            } 

            else {
                // Otherwise, select the resource.
                selectedResource.IsSelected = true;
                selectionList.Add(selectedResource);

                // Change the material to the selectionColor.
                selectedResource.GetComponent<Renderer>().material = selectionColor;
            }
        }

        
        
        private void HandleHoverEnter(XRBaseInteractor interactor)
        {
            // Toggle the activation status each time the interactable is selected
            isActivated = !isActivated;
            ToggleActivation(isActivated);
        }

        private void HandleHoverExit(XRBaseInteractor interactor)
        {
            // Add any necessary behavior when the interactable is deselected
        }

        private void HandleSelectEnter(XRBaseInteractor interactor) {
        // Toggle the activation status each time the interactable is selected
        isActivated = !isActivated;
        ToggleActivation(isActivated);
        if (isActivated) {
            ClearSelection();
        }
        }

        private void HandleSelectExit(XRBaseInteractor interactor) {
        // Add any necessary behavior when the interactable is deselected
        // if (isActivated) {
        //     ClearSelection();
        // }
        }


        public void ToggleActivation(bool activate) 
        {
            foreach (var resource in m_resourceDict.Values) 
            {
                if (!selectionList.Contains(resource)) 
                {
                    resource.gameObject.SetActive(activate);
                }
            }
        }
        

        public void ClearSelection() {
        foreach (var selectedResource in selectionList) {
            selectedResource.IsSelected = false;
            selectedResource.GetComponent<Renderer>().material = originalMaterial;
        }
        selectionList.Clear();
        }

}
