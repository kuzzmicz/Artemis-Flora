using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class MissionLoadout
{
    public List<MissionComponent> selectedComponents = new List<MissionComponent>();

    public float TotalMass => selectedComponents.Sum(c => c.mass);
    public int TotalCost => selectedComponents.Sum(c => c.cost);
    public float TotalPower => selectedComponents.Sum(c => c.power);
    public float TotalReliability => selectedComponents.Sum(c => c.reliabilityBonus);
    public int TotalScience => selectedComponents.Sum(c => c.scienceValue);
    public float TotalGrowth => selectedComponents.Sum(c => c.growthBonus);

    public void AddComponent(MissionComponent component)
    {
        if (component != null && !selectedComponents.Contains(component))
        {
            selectedComponents.Add(component);
        }
    }

    public void RemoveComponent(MissionComponent component)
    {
        if (component != null)
        {
            selectedComponents.Remove(component);
        }
    }

    public bool HasComponent(MissionComponent component)
    {
        return selectedComponents.Contains(component);
    }

    public void Clear()
    {
        selectedComponents.Clear();
    }
}