using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistanceClass
{
    private static readonly List<GameObject> _elements = new List<GameObject>();

    public static void DestroyAll()
    {
        foreach(GameObject element in _elements){
        Object.Destroy(element);
        }
    }

    public static void DontDestroyOnLoad(GameObject obj)
    {
        _elements.Add(obj);
        Object.DontDestroyOnLoad(obj);
    }
}
