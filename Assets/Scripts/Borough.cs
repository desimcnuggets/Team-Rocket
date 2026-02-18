using UnityEngine;

[System.Serializable]
public class Borough
{
    public BoroughType type;
    public string displayName;
    public bool isUnlocked;
    public int unlockDay;
    public float mood;
    public float cameraAngle;
    public GameObject boroughModel;
    public Material fogMaterial;
    
    [Header("Visuals")]
    public GameObject lockedVisual; // The low opacity cube
    public GameObject boroughGround; // The ground object for organization/spawning
}
