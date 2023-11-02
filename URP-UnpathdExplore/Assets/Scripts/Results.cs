using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Results
{
    public int Id {  get; set; }
    public string Label { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Placename { get; set; }

    //constructor called whenever a new highscore is created
    public Results(int id, string label, string title, string description, string placename)
    {
        this.Id = id;
        this.Label = label;
        this.Title = title;
        this.Description = description;
        this.Placename = placename;

    }


}
