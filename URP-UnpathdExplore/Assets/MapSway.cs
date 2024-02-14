using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapSway : MonoBehaviour
{
    public float amplitude = 0.1f; //height of sway

    public float frequency = 1f; //frequency of sway

    private Mesh originalMesh;
    private Vector3[] originalVertices;
    private Vector3[] displacedVertices;

    public GameObject bathMap;
    void Start()
    {
        Mesh mesh = bathMap.GetComponent<MeshFilter>().mesh;
        originalMesh = Mesh.Instantiate(mesh) as Mesh;  // Copy of the original static mesh
        originalVertices = originalMesh.vertices;  // Copy of the original vertices
        displacedVertices = new Vector3[originalVertices.Length];
    }
    void FixedUpdate () {
   
        for (int i = 0; i < originalVertices.Length; i++)
        {
            float y = Mathf.Sin((Time.time + originalVertices[i].x + originalVertices[i].z) * frequency) * amplitude;
            displacedVertices[i] = new Vector3(originalVertices[i].x, y, originalVertices[i].z);
        }

        Mesh displacedMesh = bathMap.GetComponent<MeshFilter>().mesh;
        displacedMesh.vertices = displacedVertices;
        displacedMesh.RecalculateNormals();
    }
 
}

