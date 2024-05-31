using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CurvedCanvas : MonoBehaviour
{
    public float curvature = 55f; // Adjust this value for more or less curvature

    void Start()
    {
        RectTransform rt = GetComponent<RectTransform>();

        // Get the world corners of the RectTransform
        Vector3[] worldCorners = new Vector3[4];
        rt.GetWorldCorners(worldCorners);

        // Convert the world corners to local positions
        Vector3[] localCorners = new Vector3[4];
        for (int i = 0; i < worldCorners.Length; i++)
        {
            localCorners[i] = rt.InverseTransformPoint(worldCorners[i]);
        }

        // Create a mesh for the curved canvas
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[localCorners.Length];
        localCorners.CopyTo(vertices, 0);

        // Curve the vertices
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            float angle = (vertex.x / rt.rect.width) * curvature * Mathf.Deg2Rad;
            float radius = rt.rect.width / curvature;
            vertices[i] = new Vector3(Mathf.Sin(angle) * radius, vertex.y, Mathf.Cos(angle) * radius - radius);
        }

        // Assign the vertices to the mesh
        mesh.vertices = vertices;

        // Add the rest of the mesh properties (e.g., triangles, UVs, etc.) here if needed

        // Assign the mesh to a MeshFilter component
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null)
        {
            mf = gameObject.AddComponent<MeshFilter>();
        }
        mf.mesh = mesh;

        // Optionally add a MeshRenderer component to render the mesh
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr == null)
        {
            mr = gameObject.AddComponent<MeshRenderer>();
        }
    }
}
