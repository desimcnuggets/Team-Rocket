using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoroughGrouper : MonoBehaviour
{
    [Header("Source")]
    public Transform cityRoot; // The parent containing all mixed buildings

    [Header("Borough Colliders (Assign the Lock/Area colliders)")]
    public Collider greenwichCollider;
    public Collider westminsterCollider;
    public Collider lambethCollider;
    public Collider hillingdonCollider;
    public Collider kensingtonCollider;
    public Collider camdenCollider;

    [Header("Output Containers (Generated Automatically if Empty)")]
    public Transform greenwichContainer;
    public Transform westminsterContainer;
    public Transform lambethContainer;
    public Transform hillingdonContainer;
    public Transform kensingtonContainer;
    public Transform camdenContainer;

    [ContextMenu("Group Objects By Proximity")]
    public void GroupObjects()
    {
        if (cityRoot == null)
        {
            Debug.LogError("Assign the City Root (parent of all buildings) first!");
            return;
        }

        // Ensure containers exist
        if (greenwichContainer == null) greenwichContainer = CreateContainer("Greenwich_Borough");
        if (westminsterContainer == null) westminsterContainer = CreateContainer("Westminster_Borough");
        if (lambethContainer == null) lambethContainer = CreateContainer("Lambeth_Borough");
        if (hillingdonContainer == null) hillingdonContainer = CreateContainer("Hillingdon_Borough");
        if (kensingtonContainer == null) kensingtonContainer = CreateContainer("Kensington_Borough");
        if (camdenContainer == null) camdenContainer = CreateContainer("Camden_Borough");

        // List of children to process (cache them first so we don't break the loop while reparenting)
        List<Transform> children = new List<Transform>();
        foreach (Transform child in cityRoot)
        {
            children.Add(child);
        }

        int count = 0;
        foreach (Transform child in children)
        {
            Transform closest = FindClosestBorough(child.position);
            if (closest != null)
            {
                // Map collider to container
                Transform targetContainer = GetContainerForCollider(closest);
                if (targetContainer != null)
                {
                    #if UNITY_EDITOR
                    Undo.SetTransformParent(child, targetContainer, "Group Borough Objects");
                    #else
                    child.SetParent(targetContainer);
                    #endif
                    count++;
                }
            }
        }

        Debug.Log($"Grouped {count} objects into boroughs!");
    }

    Transform FindClosestBorough(Vector3 pos)
    {
        // 1. Check if inside bounds (Priority)
        if (IsInside(greenwichCollider, pos)) return greenwichCollider.transform;
        if (IsInside(westminsterCollider, pos)) return westminsterCollider.transform;
        if (IsInside(lambethCollider, pos)) return lambethCollider.transform;
        if (IsInside(hillingdonCollider, pos)) return hillingdonCollider.transform;
        if (IsInside(kensingtonCollider, pos)) return kensingtonCollider.transform;
        if (IsInside(camdenCollider, pos)) return camdenCollider.transform;

        // 2. Fallback to closest distance center
        Transform closest = null;
        float minDst = float.MaxValue;

        Checkdist(greenwichCollider);
        Checkdist(westminsterCollider);
        Checkdist(lambethCollider);
        Checkdist(hillingdonCollider);
        Checkdist(kensingtonCollider);
        Checkdist(camdenCollider);

        void Checkdist(Collider col)
        {
            if (col == null) return;
            // Use closest point on bounds to get a rough distance if outside
            Vector3 closestPoint = col.ClosestPoint(pos);
            float dst = Vector3.Distance(pos, closestPoint);
            
            if (dst < minDst)
            {
                minDst = dst;
                closest = col.transform;
            }
        }

        return closest;
    }

    bool IsInside(Collider col, Vector3 pos)
    {
        if (col == null) return false;
        return col.bounds.Contains(pos);
    }

    Transform GetContainerForCollider(Transform hitTransform)
    {
        // We are comparing the transform of the collider we found
        if (greenwichCollider != null && hitTransform == greenwichCollider.transform) return greenwichContainer;
        if (westminsterCollider != null && hitTransform == westminsterCollider.transform) return westminsterContainer;
        if (lambethCollider != null && hitTransform == lambethCollider.transform) return lambethContainer;
        if (hillingdonCollider != null && hitTransform == hillingdonCollider.transform) return hillingdonContainer;
        if (kensingtonCollider != null && hitTransform == kensingtonCollider.transform) return kensingtonContainer;
        if (camdenCollider != null && hitTransform == camdenCollider.transform) return camdenContainer;
        return null;
    }

    Transform CreateContainer(string name)
    {
        GameObject go = new GameObject(name);
        return go.transform;
    }
}


