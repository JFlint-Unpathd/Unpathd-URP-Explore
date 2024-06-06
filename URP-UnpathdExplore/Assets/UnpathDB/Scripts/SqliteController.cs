using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
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
    private SqliteDataReader m_reader;

    private Dictionary<string, UnpathResource> m_resourceDict = new Dictionary<string, UnpathResource>();
    private static readonly Dictionary<string, double> temporalTagToCoordinateMap;

    private List<string> m_orQueryList = new List<string>();
    private List<string> m_andQueryList = new List<string>();
    private List<string> m_currentQueryList = new List<string>();

 
    //added as a public getter for zoomlogic
    public Dictionary<string, UnpathResource> GetResourceDict() 
    {
        return m_resourceDict;
    }
    //public ZoomController zoomController;

    private ResetRefine resetRefine;

    // Added for storing original positions
    public Dictionary<UnpathResource, Vector3> originalPositions = new Dictionary<UnpathResource, Vector3>();
    public Dictionary<UnpathResource, Vector3> GetOriginalPositions()
    {
        return originalPositions;
    }
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

    public float m_xFactor = 1;
    public float m_yFactor = 2f;

    private Coroutine queryCoroutine;
    

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
        //zoomController = FindObjectOfType<ZoomController>();

        //added for map projection
        mapProjectionController.GetComponent<MapProjection>();
        resetRefine = FindObjectOfType<ResetRefine>();


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

    // // ORIGINAL BY BRUCE
    // public SqliteDataReader ExecuteCommand( string selections, string tableNames, string matches ) {
    //     if (m_reader != null)
    //     {
    //         m_reader.Close();
    //         m_reader.Dispose();
    //     }
    //     m_command.CommandText = string.Format( SelectFromDBTemplate, selections, tableNames, matches );
    //     Debug.Log( m_command.CommandText );

    //     //original
    //     //return m_command.ExecuteReader();

    //     //changed to this to allow for the m_reader to be a variable and accesible
    //     //and be quit in the stopQ method
    //     m_reader = m_command.ExecuteReader();
    //     return m_reader;
    // }

    public SqliteDataReader ExecuteCommand(string selections, string tableNames, string matches) 
    {
        if (m_reader != null)
        {
            m_reader.Close();
            m_reader.Dispose();
        }
        m_command.CommandText = string.Format(SelectFromDBTemplate, selections, tableNames, matches) + " LIMIT 1000";
        Debug.Log(m_command.CommandText);

        m_reader = m_command.ExecuteReader();
        return m_reader;
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

    public void RunQuery() 
    {
        //added by M
        if( m_currentQueryList.Count == 0 ) {
            Debug.Log( "Selection list is empty. Add query terms before running the query." );
            return;
        }
        StringBuilder builder = new StringBuilder();
        for( int i = 0, len = m_currentQueryList.Count; i < len; i++ ) {
            builder.Append( m_currentQueryList[i] );
        }
        // Original by Bruce
        string selections = "*";
        string tableNames = "resource";
            

        if (queryCoroutine != null)
        {
            StopCoroutine(queryCoroutine);
        }

        mapProjectionController.ProjectMap();
        SqliteDataReader reader = ExecuteCommand( selections, tableNames, builder.ToString() );
        //StartCoroutine( CreateAll( reader ) );

        
        queryCoroutine = StartCoroutine(CreateAll( reader ));
    
    }

    private IEnumerator CreateAll( SqliteDataReader reader ) 
    {
        const int loadPerFrame = 10;  // Change this - or add as public variable to mess around with in Editor
        int count = 0;

        while( reader.Read() ) {


        string title = reader.GetString( reader.GetOrdinal( "title" ) ); // this could be optimized to just use the bare integer, once the table layout has been finalised.
        string id = reader.GetString( reader.GetOrdinal( "ids" ) );
        string desc = reader.GetString( reader.GetOrdinal( "description" ) );
        int ordinal = reader.GetOrdinal( "placename" );
        string placename = (reader.GetValue( ordinal ) is System.DBNull) ? "" : reader.GetString( ordinal );
        int latOrdinal = reader.GetOrdinal( "lat" );

        if( reader.GetValue( latOrdinal ).GetType() != typeof( System.DBNull ) ) 
        {
            //Debug.Log( $"lat: {latOrdinal}; type: {reader.GetValue( latOrdinal ).GetType()}" );
            string latString = reader.GetString( latOrdinal );
            string lngString = reader.GetString( reader.GetOrdinal( "lng" ) );
            double lat = 0f;
            double lng = 0f;
            if( double.TryParse( latString, out lat ) && double.TryParse( lngString, out lng ) ) {
            GameObject obj;
            obj = Instantiate( m_ResourcePrefab, new Vector3( (float)lng * m_xFactor, 0f, (float)(lat - 50.0) * m_yFactor ), Quaternion.identity );
            obj.name = title + "__" + id;
            obj.transform.SetParent( m_root.transform );
            UnpathResource res = obj.AddComponent<UnpathResource>();
            res.m_LatLng = new LatLng( lat, lng );
            res.m_Label = title;
            res.m_Title = title;
            res.m_Description = desc;
            res.m_Placename = placename;
            //FilterOff();
            //Debug.Log( $"id : {id}" );
            m_resourceDict.Add( id, res );
            ++count;

            // Add the UnpathResource object to the allObjects list for the isolate logic
            allQResults.Add( res );

            // Access the temporal column and set y-coordinate based on tags

            int temporalOrdinal = reader.GetOrdinal("temporal_text");
            if (!reader.IsDBNull(temporalOrdinal)) 
            {
                string temporalTag = reader.GetString(temporalOrdinal);
                float yCoordinate = (float)GetYCoordinateFromTemporalTag(temporalTag);
                obj.transform.position = new Vector3(obj.transform.position.x, yCoordinate, obj.transform.position.z);

                // Store the original position of the resource
                originalPositions[res] = res.transform.position;
            }

            // added for zoom logic
            // interactable that has been instantiated is being added to zoom list when a hover is detected
            // XRSimpleInteractable interactable = obj.GetComponent<XRSimpleInteractable>();
            // if( interactable == null ) {
            //     interactable = obj.AddComponent<XRSimpleInteractable>();
            // }
            // interactable.hoverEntered.AddListener( ( interactor ) => { zoomController.ZoomList( id ); } );
                if( count % loadPerFrame == 0 ) 
                {
                    yield return 0;
                }
            }
        }

        }

        reader.Close();
        reader.Dispose();

        // Notify ResetRefine that processing is done
        if (resetRefine != null)
        {
            resetRefine.OnProcessingDone();
        }
        
        //StaticBatchingUtility.Combine( m_root );
        Debug.Log( $"Object count: {count}" );
    }

    public void StopQuery()
    {
        if(queryCoroutine != null)
        {
            StopCoroutine(queryCoroutine);
            queryCoroutine = null;
        }

        if (m_reader != null)
        {
            m_reader.Close();
            m_reader.Dispose();
            m_reader = null;
        }
    }

    private static readonly List<string> temporalTags = new List<string>
    {
        "NULL", "UNCERTAIN", "UNKNOWN", "PERIOD UNASSIGNED", "PERIOD UNKNOWN", "GENERAL", "MULTIPERIOD",
        "PREHISTORIC", "PREHISTORIC OR ROMAN", "PALAEOLITHIC", "UPPER PALAEOLITHIC", "EARLY MESOLITHIC",
        "MESOLITHIC", "EARLY NEOLITHIC", "NEOLITHIC", "MIDDLE NEOLITHIC", "LATER PREHISTORIC", "BRONZE AGE",
        "MIDDLE BRONZE AGE", "LATE BRONZE AGE", "IRON AGE", "ROMAN", "MEDIAEVAL", "MEDIEVAL", "EARLY MED. OR LATER",
        "VIKING", "EARLY MEDIEVAL", "LATE MEDIEVAL", "9TH CENTURY", "10TH CENTURY", "11TH CENTURY", "12TH CENTURY",
        "13TH CENTURY", "POST MEDIAEVAL", "POST MEDIEVAL", "POST-MEDIAEVAL", "POST-MEDIEVAL", "14TH CENTURY",
        "15TH CENTURY", "16TH CENTURY", "TUDOR", "ELIZABETHAN", "Post-medieval 1540-1901", "Post-medieval 1588-1588",
        "STUART", "17TH CENTURY", "18TH CENTURY", "HANOVERIAN", "GEORGIAN", "Post-medieval 1781-1781",
        "Post-medieval 1782-1782", "Post-medieval 1798-1798", "19TH CENTURY", "Post-medieval 1814-1814",
        "Post-medieval 1815-1815", "Post-medieval 1827-1827", "VICTORIAN", "Post-medieval 1847-1847",
        "Post-medieval 1848-1848", "Post-medieval 1849-1849", "Post-medieval 1850-1850", "Post-medieval 1852-1852",
        "Post-medieval 1854-1854", "Post-medieval 1856-1856", "Post-medieval 1861-1861", "Post-medieval 1864-1864",
        "Post-medieval 1867-1867", "Post-medieval 1876-1876", "Post-medieval 1878-1878", "Post-medieval 1879-1879",
        "Post-medieval 1881-1881", "Post-medieval 1882-1882", "Post-medieval 1884-1884", "Post-medieval 1890-1890",
        "Post-medieval 1891-1891", "Post-medieval 1892-1892", "Post-medieval 1893-1893", "Post-medieval 1897-1897",
        "EARLY 20TH CENTURY", "20TH CENTURY", "MODERN", "EDWARDIAN", "20th century 1900-1900", "20th century 1900-1999",
        "20th century 1905-1905", "20th century 1906-1906", "20th century 1908-1908", "20th century 1910-1910",
        "20th century 1911-1911", "FIRST WORLD WAR", "First World War 1915-1915", "First World War 1916-1916",
        "First World War 1917-1917", "First World War 1918-1918", "20th century 1921-1921", "20th century 1923-1923",
        "20th century 1924-1924"
        // Add other tags as needed
    };

    static SqliteController()
    {
        temporalTagToCoordinateMap = new Dictionary<string, double>();
        int count = temporalTags.Count;
        double step = 4.0 / (count - 1); // Step size to distribute values from 1.0 to 5.0

        for (int i = 0; i < count; i++)
        {
            temporalTagToCoordinateMap[temporalTags[i]] = 1.0 + (i * step);
        }
    }


    public double GetYCoordinateFromTemporalTag(string temporalTag)
    {
        if (temporalTagToCoordinateMap.TryGetValue(temporalTag, out double yCoordinate))
        {
            return yCoordinate;
        }
        return -1.0; // Handle unknown cases
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

    private void NotifyProcessingDone()
    {
        if (resetRefine != null)
        {
            resetRefine.OnProcessingDone();
        }
    }

}
