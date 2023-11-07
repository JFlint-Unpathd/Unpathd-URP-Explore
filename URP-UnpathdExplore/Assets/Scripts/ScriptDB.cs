using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System;
using System.Data;
//using UnityEditorInternal;

public class ScriptDB : MonoBehaviour
{
    //The name of the DB, in this case "Inventory", it is put here so all the metods can acess it
    private string dbName = "URI=file:Inventory.db";

    void Start()
    {
        //run the method to create the table
        CreateDB();

        //method to add a weapom
        //this will add a record each time prog is run, 
        //so typically it would be called on button press or similar, otherwise duplicate info
        AddWeapon("Silver Sword", 30);

        //method to display the records to console
        DisplayWeapons();
    }

    private void CreateDB()
    {
        //create the db connection
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            //set up an object (called"command") to allow db control
            using (var command = connection.CreateCommand())
            {
            // create a table called weapons if it doesn't already exist
            //it has 2 fields (up to 20 characters) and damage (an integer)
            command.CommandText = "CREATE TABLE IF NOT EXISTS weapons (name VARCHAR(20), damage INT);";
            //this line runs the execution creating the table
            command.ExecuteNonQuery();
            }
            connection.Close();
        }
        
    }

    private void AddWeapon(string weaponName, int weaponDamage)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            //set up an object (called "command") to allow db control
            using (var command = connection.CreateCommand())
            {
                //Write the SQL command to insert a record with the val passed in this method
                //note that i had to concatenate to get the weaponName and weaponDamage values into the statement
                // statement syntax: "INSERT INTO tablename (field1, field2) VALUES ('value1','value2');"
                command.CommandText = " INSERT INTO weapons (name, damage) VALUES ('" + weaponName + "', '" + weaponDamage + "');";
                command.ExecuteNonQuery(); //runs SQL command
            }
            connection.Close();
        }

    }

    
    private void DisplayWeapons()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            //set up an obect (called"command") to allow db control
            using (var command = connection.CreateCommand())
            {
                // select what you want to get (in this case evrything from weapon table)
                //this just sets the parameters of what will be reutned
                command.CommandText = "SELECT * FROM weapons;";

                //iterate through the recordset that was returned from the above statement
                //just type this line in - just change what's inside the while statement

                using (IDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    //show the console what isin field "name" and in field "damage" for each row
                    // for reader ["xxxxx"] - replace the xxxx with the field name you want to show
                    // \t - to add a tab (space) in the reader
                    //this will display as many times as there are records returned
                    Debug.Log("Name: " + reader["name"] + "\tDamage: " + reader["damage"]);

                    reader.Close();
                }
            }

            connection.Close();
        }   
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
