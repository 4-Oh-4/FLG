using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralVisionCone : MonoBehaviour
{
    [Header("Cone Properties")]
    public float viewRadius = 6f;
    [Range(1f, 360f)] public float viewAngle = 70f;
    [Range(3, 100)] public int meshResolution = 30; // Number of triangles

    private MeshFilter viewMeshFilter;
    private Mesh viewMesh;

    private void Awake()
    {
        viewMeshFilter = GetComponent<MeshFilter>();
        viewMesh = new Mesh();
        viewMesh.name = "Vision Cone Mesh";
        viewMeshFilter.mesh = viewMesh;
    }

    // Using LateUpdate ensures the cone is drawn after all movement has been calculated for the frame.
    void LateUpdate()
    {
        DrawVisionCone();
    }

    private void DrawVisionCone()
    {
        int stepCount = meshResolution;
        float stepAngleSize = viewAngle / stepCount;

        // A list to hold all the points (vertices) of our mesh
        Vector3[] vertices = new Vector3[stepCount + 2];
        // A list to define the triangles that make up the mesh
        int[] triangles = new int[stepCount * 3];

        // The first vertex is always the origin (the enemy's position)
        vertices[0] = Vector3.zero;

        // Loop to create the vertices that form the outer arc of the cone
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = -viewAngle / 2 + (i * stepAngleSize);

            // Calculate the vertex position using trigonometry
            // The default orientation points "up" (along the Y-axis)
            Vector3 vertexPosition = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), 0) * viewRadius;
            vertices[i + 1] = vertexPosition;

            // Create the triangles that form the cone shape
            if (i < stepCount)
            {
                triangles[i * 3] = 0;           // The origin point
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        // Apply the generated vertices and triangles to the mesh
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals(); // Good practice for lighting and some shaders
    }
}