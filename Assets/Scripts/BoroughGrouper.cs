using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoroughGrouper : MonoBehaviour
{
    [Header("Source")]
    public Transform cityRoot; // The parent containing all mixed buildings

    [Header("Borough Centers (Create objects and place them in scene)")]
    public Transform greenwichCenter;
    public Transform westminsterCenter;
    public Transform lambethCenter;
    public Transform hillingdonCenter;
    public Transform kensingtonCenter;
    public Transform camdenCenter;

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
                // Map center to container
                Transform targetContainer = GetContainerForCenter(closest);
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
        Transform closest = null;
        float minDst = float.MaxValue;

        // Helper to keep code clean
        void Check(Transform center)
        {
            if (center == null) return;
            float dst = Vector3.Distance(pos, center.position);
            if (dst < minDst)
            {
                minDst = dst;
                closest = center;
            }
        }

        Check(greenwichCenter);
        Check(westminsterCenter);
        Check(lambethCenter);
        Check(hillingdonCenter);
        Check(kensingtonCenter);
        Check(camdenCenter);

        return closest;
    }

    Transform GetContainerForCenter(Transform center)
    {
        if (center == greenwichCenter) return greenwichContainer;
        if (center == westminsterCenter) return westminsterContainer;
        if (center == lambethCenter) return lambethContainer;
        if (center == hillingdonCenter) return hillingdonContainer;
        if (center == kensingtonCenter) return kensingtonContainer;
        if (center == camdenCenter) return camdenContainer;
        return null;
    }

    Transform CreateContainer(string name)
    {
        GameObject go = new GameObject(name);
        return go.transform;
    }
}


