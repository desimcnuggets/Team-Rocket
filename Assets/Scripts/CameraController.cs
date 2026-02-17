using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 60f;
    [SerializeField] private float mouseSensitivity = 0.5f;
    [SerializeField] private float minAngle = -130f;
    [SerializeField] private float maxAngle = 130f;
    [SerializeField] private float lerpSpeed = 0.15f;
    
    private float targetYRotation = 0f;
    private float currentYRotation = 0f;
    private bool isDragging = false;
    
    void Update()
    {
        HandleKeyboardInput();
        HandleMouseInput();
        ApplyRotation();
    }
    
    void HandleKeyboardInput()
    {
        float input = Input.GetAxis("Horizontal");
        targetYRotation += input * rotationSpeed * Time.deltaTime;
        targetYRotation = Mathf.Clamp(targetYRotation, minAngle, maxAngle);
    }
    
    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) isDragging = true;
        if (Input.GetMouseButtonUp(0)) isDragging = false;
        
        if (isDragging)
        {
            float mouseDelta = Input.GetAxis("Mouse X");
            targetYRotation += mouseDelta * mouseSensitivity;
            targetYRotation = Mathf.Clamp(targetYRotation, minAngle, maxAngle);
        }
    }
    
    void ApplyRotation()
    {
        currentYRotation = Mathf.Lerp(currentYRotation, targetYRotation, lerpSpeed);
        transform.rotation = Quaternion.Euler(0, currentYRotation, 0);
    }
    
    public void RotateToBorough(float angle)
    {
        targetYRotation = angle;
        targetYRotation = Mathf.Clamp(targetYRotation, minAngle, maxAngle);
    }
}
