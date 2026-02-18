using UnityEngine;

public class BoroughSetup : MonoBehaviour
{
    [SerializeField] private GameObject westminsterModel;
    [SerializeField] private GameObject theCityModel;
    [SerializeField] private GameObject camdenModel;
    [SerializeField] private GameObject eastEndModel;
    [SerializeField] private GameObject brixtonModel;
    [SerializeField] private GameObject kingstonModel;
    
    void Start()
    {
        if (BoroughManager.Instance != null)
        {
            AssignBoroughModels();
        }
    }
    
    void AssignBoroughModels()
    {
        if (westminsterModel != null)
        {
            Borough westminster = BoroughManager.Instance.GetBorough(BoroughType.Westminster);
            if (westminster != null) westminster.boroughModel = westminsterModel;
        }
        
        if (theCityModel != null)
        {
            Borough theCity = BoroughManager.Instance.GetBorough(BoroughType.TheCity);
            if (theCity != null) theCity.boroughModel = theCityModel;
        }
        
        if (camdenModel != null)
        {
            Borough camden = BoroughManager.Instance.GetBorough(BoroughType.Camden);
            if (camden != null) camden.boroughModel = camdenModel;
        }
        
        if (eastEndModel != null)
        {
            Borough eastEnd = BoroughManager.Instance.GetBorough(BoroughType.EastEnd);
            if (eastEnd != null) eastEnd.boroughModel = eastEndModel;
        }
        
        if (brixtonModel != null)
        {
            Borough brixton = BoroughManager.Instance.GetBorough(BoroughType.Brixton);
            if (brixton != null) brixton.boroughModel = brixtonModel;
        }
        
        if (kingstonModel != null)
        {
            Borough kingston = BoroughManager.Instance.GetBorough(BoroughType.Kingston);
            if (kingston != null) kingston.boroughModel = kingstonModel;
        }
    }
}
