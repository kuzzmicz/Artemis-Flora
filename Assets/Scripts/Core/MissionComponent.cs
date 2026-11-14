using UnityEngine;

public enum ComponentCategory
{
    GrowingSystem,
    Power,
    Structure,
    Communication,
    Science
}

[CreateAssetMenu(fileName = "New Component", menuName = "Mission/Component")]
public class MissionComponent : ScriptableObject
{
    [Header("Basic Info")]
    public string displayName;
    public ComponentCategory category;
    public Sprite icon;
    [TextArea(3, 6)] public string description;

    [Header("Identification")]
    public ComponentType componentType;

    [Header("Stats")]
    public float mass;
    public int cost;
    public float power;
    public float reliabilityBonus;
    public int scienceValue;
    public float growthBonus;
}