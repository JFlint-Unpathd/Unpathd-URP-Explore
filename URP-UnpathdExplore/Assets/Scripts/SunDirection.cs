using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunDirection : MonoBehaviour
{
    void Start()
    {
        Shader.SetGlobalVector("_SunDirection", transform.forward);
    }

}
