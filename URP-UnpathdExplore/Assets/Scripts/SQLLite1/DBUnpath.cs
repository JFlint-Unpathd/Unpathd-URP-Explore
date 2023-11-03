using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using TMPro;

public class DBUnpath : MonoBehaviour
{
    public GameObject objectToSpawn;
    public Vector3 origin = Vector3.zero;
    public float radius = 10;
    public TMP_Text resultsText;

    public List<Results> results = new List<Results>();

    // Define the Results class
    public class Results
    {
        public string title;

        // Add more properties as needed
    }

    void Start()
    {
        GetResults();
    }

    public void GetResults()
    {
        results.Clear();
        resultsText.text = "";

        string dbName = "URI=file:unpath.db";

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT title FROM resource WHERE placename = 'England';";

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string title = reader["title"].ToString();

                        Results result = new Results();
                        result.title = title;
                        results.Add(result);

                        //Debug.Log("Result: " + title);

                        Vector3 randomPosition = origin + Random.insideUnitSphere * radius;
                        GameObject spawnedObject = Instantiate(objectToSpawn, randomPosition, Quaternion.identity);
                        
                        // Find the TMP component in the spawned object
                        TMP_Text textElement = spawnedObject.GetComponentInChildren<TMP_Text>();

                        if (textElement != null)
                        {
                            textElement.text = title; // Set the text to the title
                        }

                    }
                }
            }

            connection.Close();
        }

        // Debug the items added to the results list
        foreach (var result in results)
        {
            Debug.Log("Results List Item: " + result.title);
        }
    }
}
