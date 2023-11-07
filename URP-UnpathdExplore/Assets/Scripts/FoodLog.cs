using System.Data;
using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class FoodLog : MonoBehaviour
{
    public TMP_InputField foodInput;
    public TMP_InputField mealInput;
    public TMP_Text foodList;

    //the name of the Db, in this case Foodlog
    private string dbName = "URI=file:FoodLog.db";

    // Start is called before the first frame update
    void Start()
    {
        //run method to create table
        CreateDB();

        //display existing food list
        DisplayFood();
        
    }

    // create table if does not exist already
    public void CreateDB()
    {
        //create the DB connection
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            //set up an object (called "command") to allow db control
            using (var command = connection.CreateCommand())
            {
                //create a table called fooditems with 2 fields: foodname and meal
                command.CommandText = "CREATE TABLE IF NOT EXISTS fooditems (foodname VARCHAR(30), meal VARCHAR(20));";
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public void AddFood()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            //set up an object (called "command") to allow db control
            using (var command = connection.CreateCommand())
            {
                // write the sql command to insert a record - values are pulled from UI fields
                // note that i had to concatenate to get the foodname and meal val into the statemebt
                //statement syntax: " INSERT INTO ftablename (field1, field2);"
                command.CommandText = " INSERT INTO fooditems (foodname, meal) VALUES ('" + foodInput.text + "','" + mealInput.text + "');";
                command.ExecuteNonQuery(); //this runs sql command
            }

            connection.Close();
        }

        //run the display food method
        DisplayFood();
    }


     public void DisplayFood()
    {
        //clear out list before displaying new contents
        foodList.text = "";

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            //set up an obj (called "command") to allow db control
            using (var command = connection.CreateCommand())
            {
                //select what you want to retrieve from database (in this case everything from the table)
                command.CommandText = "SELECT * FROM fooditems ORDER BY meal;";       

                //iterate through the recordset that was returned from the statement above
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    // for reader ["xxxxx"] - replace xxx with the field name you want to show
                    // this will display as many times as the records are returned
                    // note that to show all the records in a text field i had to use += otherwise only the last record would show up

                    foodList.text += reader["foodname"] + "\t\t" + reader["meal"] + "\n";

                    reader.Close();
                }         
            }

            connection.Close();
        }
        
        
    }
}
 