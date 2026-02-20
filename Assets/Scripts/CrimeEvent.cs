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

    [Header("Secondary Stats (Raid)")]
    public int raidEconomyChange;
    public int raidTrustChange;
    public int raidCoolChange;

    [Header("Secondary Stats (Ignore)")]
    public int ignoreEconomyChange;
    public int ignoreTrustChange;
    public int ignoreCoolChange;
}

public enum StatTier
{
    Low, // < 25
    Med, // 25 - 74
    High // 75 - 100
}
