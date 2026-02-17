using UnityEngine;

public class CrimeIcon : MonoBehaviour
{
    private CrimeEvent eventData;
    private Camera mainCamera;
    
    public void Initialize(CrimeEvent evt)
    {
        eventData = evt;
        mainCamera = Camera.main;
        
        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0, 180, 0);
        }
    }
    
    void Update()
    {
        transform.position += Vector3.up * Mathf.Sin(Time.time * 2f) * 0.002f;
        
        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0, 180, 0);
        }
    }
    
    void OnMouseDown()
    {
        if (UIManager.Instance != null && eventData != null)
        {
            UIManager.Instance.ShowDecisionCard(eventData, this.gameObject);
        }
    }
}
