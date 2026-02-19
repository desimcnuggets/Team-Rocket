using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoroughGrouper))]
[CanEditMultipleObjects]
public class BoroughGrouperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);
        if (GUILayout.Button("Group Objects By Proximity", GUILayout.Height(30)))
        {
            foreach (Object target in targets)
            {
                BoroughGrouper grouper = (BoroughGrouper)target;
                grouper.GroupObjects();
            }
        }
    }
}
