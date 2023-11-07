using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.Xml.Linq;
using System.IO;
using TMPro;
using System.Reflection.Emit;
using Unity.VisualScripting;

public class DBUnpathExplorer : MonoBehaviour
{
    //object to spawn to represent obj in scene
    public GameObject objectToSpawn;

    public Vector3 origin = Vector3.zero;
    public float radius = 10;

    public TMP_Text resultsText;

    // instance of list that will contain results loaded from database or add new results 
    public List<Results> results = new List<Results>();

    void Start()
    {
        GetResults();
    }

    public void GetResults()
    {   //clear list to make sure list up to date
        results.Clear();

        //clear out database list before displaying new contents
        resultsText.text = "";

        //initialize database
        string dbName = "URI=file:unpath.db";

        //launch connection to database
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM resource WHERE placename = 'England';";

                using (IDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        
                      
                        Debug.Log( "Result: " + reader["title"]);

                        // Finds a position in a sphere with a radius of 10 units.
                        Vector3 randomPosition = origin + Random.insideUnitSphere * radius;
                        Instantiate(objectToSpawn, randomPosition, Quaternion.identity);

                        // this will display as many times as the records are returned
                        // note that to show all the records in a text field i had to use += otherwise only the last record would show up
                        // "\t\t" - space, "\n" - new line
                        resultsText.text += reader["title"] + "\t\t" + "\n";

                    }

                    reader.Close();
                }
            }

            connection.Close();

        }
    }

 

   
}
