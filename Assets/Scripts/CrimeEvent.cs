using UnityEngine;

[CreateAssetMenu(fileName = "CrimeEvent", menuName = "TNE/Crime Event")]
public class CrimeEvent : ScriptableObject
{
    public string eventName;
    public BoroughType borough;
    public CrimeCategory category;
    public int crimeRateChange;
    public int bribeValue;
    public string victimDescription;
    public Sprite victimPortrait;
    public Sprite mapIcon;
    public string tickerIgnore;
    public string tickerRaid;
    public CrimeEvent escalatesTo;
    public float escalationChance;
}
