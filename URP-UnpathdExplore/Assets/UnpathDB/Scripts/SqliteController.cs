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
    private SqliteDataReader m_reader;

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
        if (m_reader != null)
        {
            m_reader.Close();
            m_reader.Dispose();
        }
        m_command.CommandText = string.Format( SelectFromDBTemplate, selections, tableNames, matches );
        Debug.Log( m_command.CommandText );

        //original
        //return m_command.ExecuteReader();

        //changed to this to allow for the m_reader to be a variable and accesible
        //and be quit in the stopQ method
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

    public double GetYCoordinateFromTemporalTag(string temporalTag)
    {
        switch (temporalTag)
        {
            case "NULL":
            case "UNCERTAIN":
            case "UNKNOWN":
            case "PERIOD UNASSIGNED":
            case "PERIOD UNKNOWN":
            case "GENERAL":
            case "MULTIPERIOD":
                return 1.0;
            case "PREHISTORIC":
                return 1.1;
            case "PREHISTORIC OR ROMAN":
                return 1.2;
            case "PALAEOLITHIC":
                return 1.3;
            case "UPPER PALAEOLITHIC":
                return 1.4;
            case "EARLY MESOLITHIC":
                return 1.5;
            case "MESOLITHIC":
                return 1.6;
            case "EARLY NEOLITHIC":
                return 1.7;
            case "NEOLITHIC":
                return 1.8;
            case "MIDDLE NEOLITHIC":
                return 1.9;
            case "LATER PREHISTORIC":
                return 2.0;
            case "BRONZE AGE":
                return 2.1;
            case "MIDDLE BRONZE AGE":
                return 2.2;
            case "LATE BRONZE AGE":
                return 2.3;
            case "IRON AGE":
                return 2.4;
            case "ROMAN":
                return 2.5;
            case "MEDIAEVAL":
                return 2.6;
            case "MEDIEVAL":
                return 2.7;
            case "EARLY MED. OR LATER":
            case "VIKING":
            case "EARLY MEDIEVAL":
                return 2.8;
            case "LATE MEDIEVAL":
                return 2.9;
            case "9TH CENTURY":
                return 3.0;
            case "10TH CENTURY":
                return 3.1;
            case "11TH CENTURY":
                return 3.2;
            case "12TH CENTURY":
                return 3.3;
            case "13TH CENTURY":
                return 3.4;
            case "POST MEDIAEVAL":
            case "POST MEDIEVAL":
            case "POST-MEDIAEVAL":
            case "POST-MEDIEVAL":
                return 3.5;
            case "14TH CENTURY":
                return 3.6;
            case "15TH CENTURY":
                return 3.7;
            case "16TH CENTURY":
                return 3.8;
            case "TUDOR":
                return 3.9;
            case "ELIZABETHAN":
                return 4.0;
            case "Post-medieval 1540-1901":
                return 4.1;
            case "Post-medieval 1588-1588":
                return 4.2;
            case "STUART":
                return 4.3;
            case "17TH CENTURY":
                return 4.4;
            case "18TH CENTURY":
                return 4.5;
            case "HANOVERIAN":
                return 4.6;
            case "GEORGIAN":
                return 4.7;
            case "Post-medieval 1781-1781":
                return 4.8;
            case "Post-medieval 1782-1782":
                return 4.9;
            case "Post-medieval 1798-1798":
                return 5.0;
            case "19TH CENTURY":
                return 5.1;
            case "Post-medieval 1814-1814":
                return 5.2;
            case "Post-medieval 1815-1815":
                return 5.3;
            case "Post-medieval 1827-1827":
                return 5.4;
            case "VICTORIAN":
                return 5.5;
            case "Post-medieval 1847-1847":
                return 5.6;
            case "Post-medieval 1848-1848":
                return 5.7;
            case "Post-medieval 1849-1849":
                return 5.8;
            case "Post-medieval 1850-1850":
                return 5.9;
            case "Post-medieval 1852-1852":
                return 6.0;
            case "Post-medieval 1854-1854":
                return 6.1;
            case "Post-medieval 1856-1856":
                return 6.2;
            case "Post-medieval 1861-1861":
                return 6.3;
            case "Post-medieval 1864-1864":
                return 6.4;
            case "Post-medieval 1867-1867":
                return 6.5;
            case "Post-medieval 1876-1876":
                return 6.6;
            case "Post-medieval 1878-1878":
                return 6.7;
            case "Post-medieval 1879-1879":
                return 6.8;
            case "Post-medieval 1881-1881":
                return 6.9;
            case "Post-medieval 1882-1882":
                return 7.0;
            case "Post-medieval 1884-1884":
                return 7.1;
            case "Post-medieval 1890-1890":
                return 7.2;
            case "Post-medieval 1891-1891":
                return 7.3;
            case "Post-medieval 1892-1892":
                return 7.4;
            case "Post-medieval 1893-1893":
                return 7.5;
            case "Post-medieval 1897-1897":
                return 7.6;
            case "EARLY 20TH CENTURY":
                return 7.7;
            case "20TH CENTURY":
                return 7.8;
            case "MODERN":
                return 7.9;
            case "EDWARDIAN":
                return 8.0;
            case "20th century 1900-1900":
                return 8.1;
            case "20th century 1900-1999":
                return 8.2;
            case "20th century 1905-1905":
                return 8.3;
            case "20th century 1906-1906":
                return 8.4;
            case "20th century 1908-1908":
                return 8.5;
            case "20th century 1910-1910":
                return 8.6;
            case "20th century 1911-1911":
                return 8.7;
            case "FIRST WORLD WAR":
                return 8.8;
            case "First World War 1915-1915":
                return 8.9;
            case "First World War 1916-1916":
                return 9.0;
            case "First World War 1917-1917":
                return 9.1;
            case "First World War 1918-1918":
                return 9.2;
            case "20th century 1921-1921":
                return 9.3;
            case "20th century 1923-1923":
                return 9.4;
            case "20th century 1924-1924":
                return 9.5;
            // Add other cases as needed...
            default:
                return -1.0; // Handle unknown cases
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
