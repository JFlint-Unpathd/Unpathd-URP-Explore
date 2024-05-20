using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldRunQMethod : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    // public void RunQuery() {


    //     //added by M
    //     if (m_currentQueryList.Count == 0) {
    //     Debug.Log("Selection list is empty. Add query terms before running the query.");
    //     return;
    //     }
       
    //     StringBuilder builder = new StringBuilder();
    //     for( int i = 0, len = m_currentQueryList.Count; i < len; i++ ) {
    //         builder.Append( m_currentQueryList[i] );
    //     }
        
    //     // Original by Bruce
    //     string selections = "*";
    //     string tableNames = "resource";
    //     int count = 0;
        
    //     // Append LIMIT 500 to the query string
    //     builder.Append(" LIMIT 500");

    //     //SqliteDataReader reader = ExecuteCommand( selections, tableNames, builder.ToString() );

    //     //maria for join
    //     // string selections = "*";
    //     // string tableNames = "resource";
    //     // string joinTable = "extra";
    //     // string joinCondition = "resource.PK=extra.PK";
    //     // int count = 0;

    //     //using (SqliteDataReader reader = ExecuteCommandWithJoin(selections, tableNames, joinTable, joinCondition, builder.ToString())){

    //     using (SqliteDataReader reader = ExecuteCommand(selections, tableNames, builder.ToString())){
        
    //     //added for map projection
    //     mapProjectionController.ProjectMap();
            
    //     //Instantiate(birdsEye, new Vector3(3.5f, 5.0f, -6.5f), Quaternion.identity);

    //     while( reader.Read() && count < 500 ) {
    //         string title = reader.GetString( reader.GetOrdinal( "title" ) ); // this could be optimized to just use the bare integer, once the table layout has been finalised.
    //         string id = reader.GetString( reader.GetOrdinal( "ids" ) );
    //         string desc = reader.GetString( reader.GetOrdinal( "description" ) );
    //         string placename = (reader.GetValue( reader.GetOrdinal( "placename" )) is System.DBNull )?"":reader.GetString( reader.GetOrdinal( "placename" ) );


    //         int latOrdinal = reader.GetOrdinal( "lat" );
    //         if( reader.GetValue( latOrdinal ).GetType() != typeof (System.DBNull)){
                
    //             //Debug.Log($"lat: {latOrdinal}; type: {reader.GetValue( latOrdinal ).GetType()}" );
    //             string latString = reader.GetString( latOrdinal );
    //             string lngString = reader.GetString( reader.GetOrdinal( "lng" ) );
    //             double lat = 0f;
    //             double lng = 0f;
    //             if( double.TryParse( latString, out lat ) && double.TryParse( lngString, out lng ) ) {
    //                 GameObject obj;
    //                 //obj = Instantiate( m_ResourcePrefab, new Vector3( (float)lng, 0f, (float)(lat - 50.0) ), Quaternion.identity );
    //                 obj = Instantiate( m_ResourcePrefab, new Vector3( (float)lng * m_xFactor, 0f, (float)(lat - 50.0)*m_yFactor ), Quaternion.identity );
    //                 obj.name = title + "__" + id;
    //                 obj.transform.SetParent( m_root.transform );
    //                 UnpathResource res = obj.AddComponent<UnpathResource>();
    //                 res.m_LatLng = new LatLng( lat, lng );
    //                 res.m_Label = title;
    //                 res.m_Title = title;
    //                 res.m_Description = desc;
    //                 res.m_Placename = placename;
    //                 //FilterOff();
    //                 //Debug.Log($"id : {id}" );
    //                 m_resourceDict.Add( id, res );
    //                 ++count;

    //                 // Add the UnpathResource object to the allObjects list for the isolate logic
    //                 allQResults.Add(res);
                    
    //                 // Access the temporal column and set y-coordinate based on tags
    //                 int temporalOrdinal = reader.GetOrdinal("temporal_text");
    //                 if (!reader.IsDBNull(temporalOrdinal)) 
    //                 {
    //                     string temporalTag = reader.GetString(temporalOrdinal);
            
    //                     float yCoordinate = (float)GetYCoordinateFromTemporalTag(temporalTag);
    //                     //Debug.Log($"Y Coordinate for {title}: {yCoordinate}");

    //                     obj.transform.position = new Vector3(obj.transform.position.x, yCoordinate, obj.transform.position.z);
    //                     //Debug.Log($"Object {title} Position: {obj.transform.position}");
    //                 }

    //                 // added for zoom logic
    //                 // interactable that has been instantiated is being added to zoom list when a hover is detected
                    
    //                 XRSimpleInteractable interactable = obj.GetComponent<XRSimpleInteractable>();
    //                 if (interactable == null) 
    //                 {
    //                     interactable = obj.AddComponent<XRSimpleInteractable>();
    //                 }

    //                 //interactable.hoverEntered.AddListener((interactor) => { zoomController.ZoomList(id); });
                    

    //                 // Find and set the information to the TextMeshProUGUI components dynamically and add info for results
    //                 // TextMeshProUGUI[] textComponents = res.GetComponentsInChildren<TextMeshProUGUI>(true);
    //                 // foreach (TextMeshProUGUI textComponent in textComponents) {
    //                 // if (textComponent.name.Equals("Label")) {
    //                 //     textComponent.text = title;
    //                 // } else if (textComponent.name.Equals("Title")) {
    //                 //     textComponent.text = "Title: " + title; 
    //                 // } else if (textComponent.name.Equals("Description")) {
    //                 //     textComponent.text = "Description: " + desc; 
    //                 // } else if (textComponent.name.Equals("Placename")) {
    //                 //     textComponent.text = "Placename: " + placename;
    //                 // }
    //                 //}

                 
    //             }
    //         }
    //     }
    // }
    //     //disabled for a bit as may take longer
    //     //StaticBatchingUtility.Combine( m_root );

    //     Debug.Log( $"Object count: {count}" );
    // }
}
