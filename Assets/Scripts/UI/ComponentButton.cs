using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComponentButton : MonoBehaviour
{
    public MissionComponent component;
    public TextMeshProUGUI buttonText;

    private Button button;
    private Image buttonImage;

    private Color normalColor = new Color(1f, 1f, 1f);
    private Color selectedColor = new Color(0.15f, 0.45f, 0.28f);
    private Color blockedColor = new Color(0.12f, 0.12f, 0.12f, 0.55f);

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        button.onClick.AddListener(OnClick);
    }

    private void OnEnable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnLoadoutChanged += UpdateVisual;

        UpdateVisual();
    }

    private void OnDisable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnLoadoutChanged -= UpdateVisual;
    }

    public void Setup(MissionComponent comp)
    {
        component = comp;
        UpdateVisual();
    }

    private void OnClick()
    {
        if (component == null || MissionManager.Instance == null) return;

        var loadout = MissionManager.Instance.currentLoadout;

        if (loadout.HasComponent(component))
        {
            MissionManager.Instance.RemoveComponent(component);
        }
        else
        {
            MissionManager.Instance.AddComponent(component);
        }

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (component == null || MissionManager.Instance == null || LocalizationManager.Instance == null) return;

        var loc = LocalizationManager.Instance;
        var loadout = MissionManager.Instance.currentLoadout;
        bool isSelected = loadout.HasComponent(component);

        bool overMass = false;
        bool overCost = false;

        if (!isSelected)
        {
            float newMass = loadout.TotalMass + component.mass;
            int newCost = loadout.TotalCost + component.cost;
            overMass = newMass > MissionManager.Instance.maxMass;
            overCost = newCost > MissionManager.Instance.maxBudget;
        }

        bool canAfford = !overMass && !overCost;

        if (isSelected)
            buttonImage.color = selectedColor;
        else if (!canAfford)
            buttonImage.color = blockedColor;
        else
            buttonImage.color = normalColor;

        string prefix = isSelected ? "[" + loc.Get("selected_short") + "] " : "";
        string status = "";

        if (!isSelected)
        {
            if (overMass && overCost)
                status = "  [" + loc.Get("no_mass_and_budget") + "]";
            else if (overMass)
                status = "  [" + loc.Get("no_mass") + "]";
            else if (overCost)
                status = "  [" + loc.Get("no_budget") + "]";
        }

        string localizedName = GetLocalizedComponentName(component);

        if (buttonText != null)
        {
            buttonText.text = $"{prefix}{localizedName}{status}\n<size=70%>{loc.Get("mass")}: {component.mass} kg  |  {loc.Get("cost")}: {component.cost}</size>";
        }
    }

    public void RefreshText()
    {
        UpdateVisual();
    }

    private string GetLocalizedComponentName(MissionComponent comp)
    {
        if (LocalizationManager.Instance == null || comp == null)
            return comp.displayName;

        string key = "comp_" + comp.componentType.ToString().ToLower();
        string localized = LocalizationManager.Instance.Get(key);


        if (localized.StartsWith("["))
            return comp.displayName;

        return localized;
    }
}