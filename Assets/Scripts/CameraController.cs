using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Top-Down View")]
    [SerializeField] private float cameraHeight = 150f;
    [SerializeField] private Transform lookTarget; // drag any scene object here as the city centre

    void Start()
    {
        ApplyTopDown();
    }

    void ApplyTopDown()
    {
        Vector3 centre = lookTarget != null ? lookTarget.position : Vector3.zero;
        transform.position = new Vector3(centre.x, cameraHeight, centre.z);
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    // Kept for compatibility — does nothing in top-down mode
    public void RotateToBorough(float angle) { }
}

