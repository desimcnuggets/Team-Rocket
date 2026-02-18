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
            transform.localScale = Vector3.one * 30f;
            
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
        
        if (mainCamera != null)
        {
            transform.rotation = mainCamera.transform.rotation;
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
