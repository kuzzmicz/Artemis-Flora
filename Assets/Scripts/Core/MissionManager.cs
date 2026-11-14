using Unity;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }
    public MissionLoadout currentLoadout = new MissionLoadout();
    public event System.Action OnLoadoutChanged;

    [Header("Limits")]
    public float maxMass = 130f;
    public int maxBudget = 90;

    [Header("Available Components")]
    public MissionComponent[] availableComponents;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool AddComponent(MissionComponent component)
    {
        if (component == null)
        {
            return false;
        }

        if (component.category == ComponentCategory.GrowingSystem)
        {
            var existingGrow = currentLoadout.selectedComponents.Find(c => c.category == ComponentCategory.GrowingSystem);

            if (existingGrow != null)
            {
                currentLoadout.RemoveComponent(existingGrow);
            }
        }

        float newMass = currentLoadout.TotalMass + component.mass;
        int newCost = currentLoadout.TotalCost + component.cost;

        if (newMass > maxMass || newCost > maxBudget)
        {
            Debug.Log($"Nie można dodać {component.displayName} – przekroczony limit");
            return false;
        }

        currentLoadout.AddComponent(component);
        OnLoadoutChanged?.Invoke();
        return true;
    }

    public void RemoveComponent(MissionComponent component)
    {
        currentLoadout.RemoveComponent(component);
        OnLoadoutChanged?.Invoke();
    }

    public void ClearLoadout()
    {
        currentLoadout.Clear();
        OnLoadoutChanged?.Invoke();
    }
}
