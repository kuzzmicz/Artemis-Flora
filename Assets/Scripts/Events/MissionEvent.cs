using UnityEngine;

[CreateAssetMenu(fileName = "New Event", menuName = "Mission/Event")]
public class MissionEvent : ScriptableObject
{
    public string eventName;

    [TextArea(3, 5)] public string description;

    [Header("Identification")]
    public EventType eventType;

    [Header("Type of event")]
    public bool isPositive;

    [Header("Choices")]
    public string choiceA;
    public string choiceB;
    public string choiceC;          

    [Header("Effects of Choice A")]
    public float growthChangeA;
    public float reliabilityChangeA;
    public int scienceChangeA;
    public float powerChangeA;

    [Header("Effects of Choice B")]
    public float growthChangeB;
    public float reliabilityChangeB;
    public int scienceChangeB;
    public float powerChangeB;

    [Header("Effects of Choice C")]
    public float growthChangeC;
    public float reliabilityChangeC;
    public int scienceChangeC;
    public float powerChangeC;

    [Header("Weighting")]
    [Tooltip("Bazowa szansa pojawienia się eventu. Im wyższa, tym częstszy.")]
    public float baseWeight = 1f;
}
