using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TriangleCounter : MonoBehaviour
{
    [Header("Drag your model here")]
    public GameObject targetModel;

    [ContextMenu("Count Triangles")]
    public void CountTriangles()
    {
        if (targetModel == null)
        {
            Debug.LogWarning("Please assign a model to count.");
            return;
        }

#if UNITY_EDITOR
        // Clear Console
        var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
#endif

        int totalTriangles = 0;

        // Check MeshRenderer + MeshFilter
        MeshFilter[] meshFilters = targetModel.GetComponentsInChildren<MeshFilter>();
        foreach (var mf in meshFilters)
        {
            if (mf.sharedMesh != null)
            {
                totalTriangles += mf.sharedMesh.triangles.Length / 3;
            }
        }

        // Check SkinnedMeshRenderer
        SkinnedMeshRenderer[] skinnedMeshes = targetModel.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var smr in skinnedMeshes)
        {
            if (smr.sharedMesh != null)
            {
                totalTriangles += smr.sharedMesh.triangles.Length / 3;
            }
        }

        Debug.Log($"[TriangleCounter] Total Triangles = {totalTriangles}");
    }
}
