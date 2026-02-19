using UnityEngine;

public class BoroughSetup : MonoBehaviour
{
    [SerializeField] private GameObject greenwichModel;
    [SerializeField] private GameObject westminsterModel;
    [SerializeField] private GameObject lambethModel;
    [SerializeField] private GameObject hillingdonModel;
    [SerializeField] private GameObject kensingtonModel;
    [SerializeField] private GameObject camdenModel;
    
    void Start()
    {
        if (BoroughManager.Instance != null)
        {
            AssignBoroughModels();
        }
    }
    
    void AssignBoroughModels()
    {
        if (greenwichModel != null)
        {
            Borough greenwich = BoroughManager.Instance.GetBorough(BoroughType.Greenwich);
            if (greenwich != null) greenwich.boroughModel = greenwichModel;
        }
        
        if (westminsterModel != null)
        {
            Borough westminster = BoroughManager.Instance.GetBorough(BoroughType.Westminster);
            if (westminster != null) westminster.boroughModel = westminsterModel;
        }
        
        if (lambethModel != null)
        {
            Borough lambeth = BoroughManager.Instance.GetBorough(BoroughType.Lambeth);
            if (lambeth != null) lambeth.boroughModel = lambethModel;
        }
        
        if (hillingdonModel != null)
        {
            Borough hillingdon = BoroughManager.Instance.GetBorough(BoroughType.Hillingdon);
            if (hillingdon != null) hillingdon.boroughModel = hillingdonModel;
        }
        
        if (kensingtonModel != null)
        {
            Borough kensington = BoroughManager.Instance.GetBorough(BoroughType.Kensington);
            if (kensington != null) kensington.boroughModel = kensingtonModel;
        }
        
        if (camdenModel != null)
        {
            Borough camden = BoroughManager.Instance.GetBorough(BoroughType.Camden);
            if (camden != null) camden.boroughModel = camdenModel;
        }
    }
}
