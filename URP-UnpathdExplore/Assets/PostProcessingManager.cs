using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessingManager : MonoBehaviour
{
    static PostProcessingManager instance;
    void Awake()
    {
        //Debug.Log("Awake called. Current instance: " + instance);
        
        if(instance != null)
        {
            Debug.Log("Instance already exists. Destroying gameObject: "+gameObject.name);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("No existing instance. Setting this as instance: "+gameObject.name);
            instance = this;
            PersistanceClass.DontDestroyOnLoad(gameObject);
        }
    }
}
