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

 
    //added as a public getter for zoomlogic
    public Dictionary<string, UnpathResource> GetResourceDict() 
    {
        return m_resourceDict;
    }
    public ZoomController zoomController;

    //....

    //added for isolate logic
    private List<UnpathResource> allQResults = new List<UnpathResource>();

    // Add a public getter for allQResults
    public List<UnpathResource> GetAllQResults()
    {
        return allQResults;
    }

    // added for Map Projection
    public MapProjection mapProjectionController;

    //added for ExecuteQ Script to allow list to be acessed 
    public List<string> GetCurrentQueryList() 
    {
        return m_currentQueryList;
    }


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

        // added to fill in inspector, kept clearing field, failsafe option
        zoomController = FindObjectOfType<ZoomController>();

        //added for map projection
        mapProjectionController.GetComponent<MapProjection>();
        
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
    public SqliteDataReader ExecuteCommand( string selections, string tableNames, string matches ) {
        m_command.CommandText = string.Format( SelectFromDBTemplate, selections, tableNames, matches );
        Debug.Log( m_command.CommandText );
        return m_command.ExecuteReader();
    }


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

        //added by M
        if (m_currentQueryList.Count == 0) {
        Debug.Log("Selection list is empty. Add query terms before running the query.");
        return;
        }
       
        StringBuilder builder = new StringBuilder();
        for( int i = 0, len = m_currentQueryList.Count; i < len; i++ ) {
            builder.Append( m_currentQueryList[i] );
        }
        
        // Original by Bruce
        string selections = "*";
        string tableNames = "resource";
        int count = 0;
        //SqliteDataReader reader = ExecuteCommand( selections, tableNames, builder.ToString() );

        //maria for join
        // string selections = "*";
        // string tableNames = "resource";
        // string joinTable = "extra";
        // string joinCondition = "resource.PK=extra.PK";
        // int count = 0;

        //using (SqliteDataReader reader = ExecuteCommandWithJoin(selections, tableNames, joinTable, joinCondition, builder.ToString())){

        using (SqliteDataReader reader = ExecuteCommand(selections, tableNames, builder.ToString())){
        
        //added for map projection
        mapProjectionController.ProjectMap();
            
        //Instantiate(birdsEye, new Vector3(3.5f, 5.0f, -6.5f), Quaternion.identity);

        while( reader.Read() ) {
            string title = reader.GetString( reader.GetOrdinal( "title" ) ); // this could be optimized to just use the bare integer, once the table layout has been finalised.
            string id = reader.GetString( reader.GetOrdinal( "ids" ) );
            string desc = reader.GetString( reader.GetOrdinal( "description" ) );
            string placename = reader.GetString( reader.GetOrdinal( "placename" ) );


            int latOrdinal = reader.GetOrdinal( "lat" );
            if( reader.GetValue( latOrdinal ).GetType() != typeof (System.DBNull)){
                
                Debug.Log($"lat: {latOrdinal}; type: {reader.GetValue( latOrdinal ).GetType()}" );
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
                    res.m_Label = title;
                    res.m_Title = title;
                    res.m_Description = desc;
                    res.m_Placename = placename;
                    //FilterOff();
                    Debug.Log($"id : {id}" );
                    m_resourceDict.Add( id, res );
                    ++count;

                    // Add the UnpathResource object to the allObjects list for the isolate logic
                    allQResults.Add(res);
                    
                    // Access the temporal column and set y-coordinate based on tags
                    int temporalOrdinal = reader.GetOrdinal("temporal");
                    if (!reader.IsDBNull(temporalOrdinal)) 
                    {
                        string temporalTag = reader.GetString(temporalOrdinal);
                        float yCoordinate = GetYCoordinateFromTemporalTag(temporalTag);
                        obj.transform.position = new Vector3(obj.transform.position.x, yCoordinate, obj.transform.position.z);
                    }

                    // added for zoom logic
                    // interactable that has been instantiated is being added to zoom list when a hover is detected
                    
                    XRSimpleInteractable interactable = obj.GetComponent<XRSimpleInteractable>();
                    if (interactable == null) 
                    {
                        interactable = obj.AddComponent<XRSimpleInteractable>();
                    }

                    interactable.hoverEntered.AddListener((interactor) => { zoomController.ZoomList(id); });
                    

                    // Find and set the information to the TextMeshProUGUI components dynamically and add info for results
                    TextMeshProUGUI[] textComponents = res.GetComponentsInChildren<TextMeshProUGUI>(true);
                    foreach (TextMeshProUGUI textComponent in textComponents) {
                    if (textComponent.name.Equals("Label")) {
                        textComponent.text = title;
                    } else if (textComponent.name.Equals("Title")) {
                        textComponent.text = "Title: " + title; 
                    } else if (textComponent.name.Equals("Description")) {
                        textComponent.text = "Description: " + desc; 
                    } else if (textComponent.name.Equals("Placename")) {
                        textComponent.text = "Placename: " + placename;
                    }
                    }

                 
                }
            }
        }
    }
        StaticBatchingUtility.Combine( m_root );
        Debug.Log( $"Object count: {count}" );
    }

        //added  for y logic
        private float GetYCoordinateFromTemporalTag(string temporalTag) {
            // Extract the relevant information from the URL
            string[] parts = temporalTag.Split('/');
            string lastPart = parts[parts.Length - 1].ToLower().Replace("%20", " "); // Convert to lowercase and replace %20 with space

            // Check for specific keywords in the last part of the URL
            if (lastPart.Contains("medieval")) {
                return 1f;
            } else if (lastPart.Contains("first world war")) {
                return 2f;
            } else if (lastPart.Contains("second world war")) {
                return 3f;
            } else if (lastPart.Contains("modern")) {
                return 4f;
            } else {
                return 0f; // Default value if the tag is not recognized
            }
        }
    

        public void ClearResourceDictandLists()
        {
            m_resourceDict.Clear();
            DeleteResults();
            allQResults.Clear();
        }
        public void DeleteResults() 
        {
            for( int i = 0, len = allQResults.Count; i < len; i++ ) 
            {
                Destroy(allQResults[i].gameObject);
            }
        }



}
