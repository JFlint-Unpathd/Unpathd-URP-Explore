using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.Xml.Linq;

public class DBUnpathExplorer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlaceNameList();
    }

    public void PlaceNameList()
    {
        //initialize database
        string dbName = "URI=file:unpath.db";

        //launch connection to database
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT title FROM resource WHERE placename = 'England';";

                using (IDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        Debug.Log("Result: " + reader["title"]);
                    }

                    reader.Close();
                }
            }

            connection.Close();

        }
    }

   
}
