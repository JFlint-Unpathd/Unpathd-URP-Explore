using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using TMPro;

public class DBUnpathE2 : MonoBehaviour
{
    public GameObject objectToSpawn;
    public Vector3 origin = Vector3.zero;
    public float radius = 10;
    public TMP_Text resultsText;

    //list for database results to be collated once sql query executed
    public List<Results> results = new List<Results>();

    //list for collected gameobjects that sql query is refined by
    public List<GameObject> selectedObjects = new List<GameObject>();

    // Define the Results class
    public class Results
    {  
        public string title;
        // Add more properties as needed
    }

    void Start()
    {
        // clears any previous result text
        resultsText.text = "";
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;

                if (clickedObject.CompareTag("england") || clickedObject.CompareTag("victorian") || clickedObject.CompareTag("ship"))
                {
                    selectedObjects.Add(clickedObject);
                    Debug.Log("Item added!");
                }

                // Check if the list contains one object of each tag
                if (ContainsOneObjectOfEachTag("england", "victorian", "ship"))
                {
                    Debug.Log("Item of each type added, can execute query!");
                    ExecuteSQLQuery();
                }
            }
        }
    }

    bool ContainsOneObjectOfEachTag(string england, string victorian, string ship)
    {
        int count1 = 0, count2 = 0, count3 = 0;

        foreach (GameObject obj in selectedObjects)
        {
            if (obj.CompareTag(england))
            {
                count1++;
            }
            else if (obj.CompareTag(victorian))
            {
                count2++;
            }
            else if (obj.CompareTag(ship))
            {
                count3++;
            }
        }
        
        return (count1 > 0 && count2 > 0 && count3 > 0);

        // This method checks if there is at least one object with each of the specified tags.
        // It counts the number of objects with each tag and ensures each count is greater than zero.
        // If all counts are greater than zero, it returns true.

    }

    void ExecuteSQLQuery()
    {
        results.Clear();
        resultsText.text = "";

        // Construct a SQL query to select all columns from resource where any column contains the keywords
        string query = "SELECT * FROM resource WHERE ";

        string[] keywords = new string[] { "ship", "victorian", "england" };

        bool firstKeyword = true;

        foreach (string keyword in keywords)
    {
        if (!firstKeyword)
        {
            query += " OR ";
        }

        query += "title LIKE '%" + keyword + "%' OR description LIKE '%" + keyword + "%' OR placename LIKE '%" + keyword + "%'";

        firstKeyword = false;
        Debug.Log(query);
    }
    
        // Add a LIMIT clause to limit the results to 500
        query += " LIMIT 500";
        
        // Execute the SQL query
        using (var connection = new SqliteConnection("URI=file:unpath.db"))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string title = reader["title"].ToString();

                        Results result = new Results();
                        result.title = title;
                        results.Add(result);

                        Debug.Log("Result: " + title);

                        Vector3 randomPosition = origin + Random.insideUnitSphere * radius;
                        GameObject spawnedObject = Instantiate(objectToSpawn, randomPosition, Quaternion.identity);

                        // Find the TMP component in the spawned object
                        TMP_Text textElement = spawnedObject.GetComponentInChildren<TMP_Text>();

                        if (textElement != null)
                        {
                            textElement.text = title; // Set the text to the title
                        }

                        resultsText.text += title + "\t\t" + "\n";
                    }
                }
            }
            connection.Close();
        }
    }
}
