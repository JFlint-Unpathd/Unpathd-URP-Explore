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
    //public TMP_Text resultsText;

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
        //resultsText.text = "";
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                string clickedTag = clickedObject.tag;

                selectedObjects.Add(clickedObject);
                Debug.Log("Item added with tag: " + clickedTag);

                if (ContainsThreeDifferentTags())
                {
                    Debug.Log("Three different tags added, can execute query!");
                    ExecuteSQLQuery();
                }
            }
        }
    }


    bool ContainsThreeDifferentTags()
    {
        // Create a HashSet to store the unique tags
        HashSet<string> uniqueTags = new HashSet<string>();

        foreach (GameObject obj in selectedObjects)
        {
            uniqueTags.Add(obj.tag);

            if (uniqueTags.Count == 3)
            {
                return true;
            }
        }

        return false;
    }

    void ExecuteSQLQuery()
    {
        results.Clear();
        //resultsText.text = "";

        // Create a list to store the dynamically determined tags
        List<string> tags = new List<string>();

        // Add the dynamically determined tags from selectedObjects
        foreach (GameObject obj in selectedObjects)
        {
            if (!tags.Contains(obj.tag))
            {
                tags.Add(obj.tag);
            }

            // Check if we have collected three tags; if so, break the loop
            if (tags.Count == 3)
            {
                break;
            }
        }


        // Check if we have collected three different tags
        if (tags.Count == 3)
        {
        // Construct the SQL query with the dynamically determined tags
        string query = "SELECT * FROM resource WHERE ";

        //bool responsible for adding OR separator in sql query, if it is the first
        //tag an OR is not neccesary, however afterwards it is
        bool firstTag = true;

        foreach (string tag in tags)
        {
            if (!firstTag)
            {
                query += " OR ";
            }

            query += "title LIKE '%" + tag + "%' OR description LIKE '%" + tag + "%' OR placename LIKE '%" + tag + "%'";

            firstTag = false;
        }
    
        // Add a LIMIT clause to limit the results to 500
        query += " LIMIT 500";

        // Log the query to the console
        Debug.Log("SQL Query: " + query);
        
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

                        //resultsText.text += title + "\t\t" + "\n";
                    }
                }
            }
            connection.Close();
        }
    }

    else
    {
        Debug.Log("Not enough unique tags collected to execute the query.");
    }
}
}
