using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class PersistanceManager
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
        Debug.Log("Keep Alive: "+obj.name);
        _elements.Add(obj);
        Object.DontDestroyOnLoad(obj);
    }
}
