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
    
    [Header("Mood & Mechanics")]
    public float timeAbove75 = 0f;
    public float timeBelow25 = 0f;
    public float permanentMoodDamage = 0f;
    public float extraSpawnMultiplier = 0f;
    public float baseWeight = 1.0f;
    
    [Header("Visuals")]
    public GameObject lockedVisual; // The low opacity cube
    public GameObject boroughGround; // The ground object for organization/spawning
}
