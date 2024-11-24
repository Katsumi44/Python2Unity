using UnityEngine;

public class SausageDeformer : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] originalVertices;
    private Vector3[] modifiedVertices;
    public Transform[] nodes;
    public float influenceRadius = 2.0f;  // Increased radius for testing
    public float deformationStrength = 1.0f;  // Increased strength for testing

    void Start()
    {
        // Retrieve the mesh and log if it's missing
        mesh = GetComponent<MeshFilter>().mesh;
        if (mesh == null)
        {
            Debug.LogError("MeshFilter is missing or the mesh is null!");
            return;
        }

        // Store the original vertex positions and log the vertex count
        originalVertices = mesh.vertices;
        modifiedVertices = new Vector3[originalVertices.Length];
        Debug.Log($"Original Mesh has {originalVertices.Length} vertices.");
    }

    void Update()
    {
        if (nodes == null || nodes.Length == 0)
        {
            Debug.LogWarning("No nodes assigned to the SausageDeformer.");
            return;
        }

        // Log the node positions for debugging in world space
        foreach (var node in nodes)
        {
            if (node != null)
            {
                Debug.Log($"Node {node.name} position in World Space: {node.position}");
            }
        }

        // Copy the original vertices to modified vertices for deformation
        originalVertices.CopyTo(modifiedVertices, 0);

        // Apply deformation logic based on the nodes
        foreach (var node in nodes)
        {
            if (node == null) continue;

            // Get node's position in world space
            Vector3 worldNodePos = node.position;
            Debug.Log($"Node {node.name} in world space: {worldNodePos}");

            // Deform mesh based on this node's world position
            DeformMesh(worldNodePos);
        }

        // Apply the modified vertices and recalculate normals
        mesh.vertices = modifiedVertices;
        mesh.RecalculateNormals();

        // Debug log mesh updates
        Debug.Log("Mesh vertices updated.");
    }

    private void DeformMesh(Vector3 worldNodePos)
    {
        bool deformationApplied = false;

        for (int i = 0; i < modifiedVertices.Length; i++)
        {
            // Calculate the distance between the current vertex and the node (in world space)
            float distance = Vector3.Distance(modifiedVertices[i], worldNodePos);

            // Log the distance for each vertex (for debugging)
            if (i % 100 == 0) // Only log for every 100th vertex for performance
            {
                Debug.Log($"Vertex {i} at {modifiedVertices[i]} is {distance} units from node {worldNodePos}");
            }

            // Apply deformation if within the influence radius
            if (distance < influenceRadius)
            {
                // Calculate influence factor based on the distance
                float influence = Mathf.Lerp(deformationStrength, 0, distance / influenceRadius);

                // Log the influence applied to the vertex (for debugging)
                if (i % 100 == 0) // Only log for every 100th vertex for performance
                {
                    Debug.Log($"Vertex {i} is affected by node with influence {influence}");
                }

                // Move the vertex towards the node position
                Vector3 direction = (worldNodePos - modifiedVertices[i]).normalized;
                modifiedVertices[i] += direction * influence;  // Removed Time.deltaTime for testing

                // Change the color of the vertex based on deformation (for visualization)
                // You could use MeshColors to apply different colors to deformed vertices, or directly affect vertices
                deformationApplied = true;
            }
        }

        // If no deformation was applied, log that info for debugging
        if (!deformationApplied)
        {
            Debug.LogWarning("No deformation applied to any vertices for this node.");
        }
    }

    void OnDrawGizmos()
    {
        if (nodes == null) return;

        // Draw gizmos to visualize influence radius around each node
        Gizmos.color = Color.blue;
        foreach (var node in nodes)
        {
            if (node != null)
            {
                Gizmos.DrawWireSphere(node.position, influenceRadius);
                Debug.Log($"Drawing Gizmo for node {node.name} at {node.position}");
            }
        }
    }
}
