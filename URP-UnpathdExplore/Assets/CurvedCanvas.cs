using UnityEngine;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CurvedCanvas : MonoBehaviour
{
    public float curvature = 10f;

    void Start()
    {
        // Get required components
        Canvas canvas = GetComponent<Canvas>();
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        if (canvas.renderMode != RenderMode.WorldSpace)
        {
            Debug.LogError("Canvas render mode must be set to World Space.");
            return;
        }

        RectTransform rectTransform = canvas.GetComponent<RectTransform>();
        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);

        Vector3[] vertices = new Vector3[worldCorners.Length];
        for (int i = 0; i < worldCorners.Length; i++)
        {
            float angle = Mathf.Lerp(-curvature, curvature, (float)i / (worldCorners.Length - 1));
            vertices[i] = Quaternion.Euler(0, angle, 0) * (worldCorners[i] - rectTransform.position);
        }

        int[] triangles = new int[] { 0, 1, 2, 2, 3, 0 };

        Mesh mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles
        };

        meshFilter.mesh = mesh;
    }
}
