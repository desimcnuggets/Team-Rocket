using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class CrimeIcon : MonoBehaviour
{
    private CrimeEvent eventData;
    private Camera mainCamera;
    
    public void Initialize(CrimeEvent evt)
    {
        eventData = evt;
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && evt.mapIcon != null)
        {
            sr.sprite = evt.mapIcon;
            
            // INCREASE SIZE & ADD COLLIDER
            transform.localScale = Vector3.one * 60f;
            
            BoxCollider col = GetComponent<BoxCollider>();
            if (col == null)
            {
                col = gameObject.AddComponent<BoxCollider>();
            }
            
            col.size = new Vector3(sr.sprite.bounds.size.x, sr.sprite.bounds.size.y, 0.2f);
        }

        mainCamera = Camera.main;
        
        if (mainCamera != null)
        {
            transform.rotation = mainCamera.transform.rotation;
        }
    }
    
    void Update()
    {
        transform.position += Vector3.up * Mathf.Sin(Time.time * 2f) * 0.002f;
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform);
            // Sprites face Z-forward by default, so LookAt points Z to camera.
            // If the sprite appears backward, we might need 180 rotation, but usually LookAt is correct for facing.
            // However, often billboards are inverted to face away from camera look direction.
            // Let's try matching forward to camera forward inverted (to look at camera).
            // Actually, simplest billboard is:
            // transform.rotation = mainCamera.transform.rotation;
            // But if user says "always face", maybe they mean pivot?
            // Let's stick to true billboard (LookAt) but handle up vector to prevent rolling weirdly?
            // transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.back, mainCamera.transform.rotation * Vector3.up);
            // This is "Camera Aligned".
            
            // Let's try simple LookAt then flip if needed.
            // transform.LookAt(mainCamera.transform);
            // transform.Rotate(0, 180, 0); // Standard flip for quad
            
            // Actually, the user might mean "Spherical" (LookAt) vs "Cylindrical" (Y-only).
            // Given "always face", I'll use simple rotation copy which is robust.
            // Wait, previous code was rotation copy. Maybe they want it to LOOK AT the camera?
            
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0, 180, 0); // Flip to face camera front
        }
    }
    
    void OnMouseDown()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsDecisionPanelOpen) return;

        if (UIManager.Instance != null && eventData != null)
        {
            UIManager.Instance.ShowDecisionCard(eventData, this.gameObject);
        }
    }
}
