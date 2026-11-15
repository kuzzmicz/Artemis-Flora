
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComponentButton : MonoBehaviour
{
    [Header("Component Data")]
    public MissionComponent component;

    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI addButtonText;

    public Image componentIcon;
    public Image cardBackground;
    public Button addButton;

    [Header("Colors")]
    public Color normalColor = new Color(0.13f, 0.22f, 0.30f);
    public Color selectedColor = new Color(0.15f, 0.45f, 0.28f);
    public Color blockedColor = new Color(0.12f, 0.12f, 0.12f);

    private void Awake()
    {
        if (addButton != null)
            addButton.onClick.AddListener(OnClick);
        else
            Debug.LogError("ComponentButton: Brak AddButton!");
    }

    private void OnEnable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnLoadoutChanged += UpdateVisual;

        UpdateVisual();
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= RefreshText;
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnLoadoutChanged -= UpdateVisual;
    }

    public void Setup(MissionComponent comp)
    {
        component = comp;

        if (componentIcon != null)
        {
            componentIcon.sprite = comp.icon;
            componentIcon.enabled = comp.icon != null;
        }

        UpdateVisual();
    }

    private void OnClick()
    {
        if (component == null || MissionManager.Instance == null)
            return;

        var manager = MissionManager.Instance;
        var loadout = manager.currentLoadout;

        if (loadout.HasComponent(component))
            manager.RemoveComponent(component);
        else
            manager.AddComponent(component);

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (component == null || MissionManager.Instance == null)
            return;

        var manager = MissionManager.Instance;
        var loadout = manager.currentLoadout;

        bool isSelected = loadout.HasComponent(component);

        bool overMass = !isSelected &&
            loadout.TotalMass + component.mass > manager.maxMass;

        bool overCost = !isSelected &&
            loadout.TotalCost + component.cost > manager.maxBudget;

        bool canAfford = !overMass && !overCost;

        if (cardBackground != null)
        {
            cardBackground.color = isSelected
                ? selectedColor
                : canAfford ? normalColor : blockedColor;
        }

        if (nameText != null)
            nameText.text = GetLocalizedComponentName(component);

        if (statsText != null)
        {
            statsText.text =
                $"{GetText("mass", "Masa")}: {component.mass:0.#} kg\n" +
                $"{GetText("cost", "Koszt")}: {component.cost} CR\n" +
                $"{GetText("power", "Energia")}: " +
                $"{component.power:+0.#;-0.#;0}";
        }

        if (addButtonText != null)
        {
            if (isSelected)
                addButtonText.text = GetText("selected_short", "USUŃ");
            else if (overMass && overCost)
                addButtonText.text = GetText("no_mass_and_budget", "BRAK LIMITU");
            else if (overMass)
                addButtonText.text = GetText("no_mass", "BRAK MASY");
            else if (overCost)
                addButtonText.text = GetText("no_budget", "BRAK BUDŻETU");
            else
                addButtonText.text = "+ DODAJ";
        }

        if (addButton != null)
            addButton.interactable = isSelected || canAfford;
    }

    public void RefreshText()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged += RefreshText;
        UpdateVisual();
    }

    private string GetText(string key, string fallback)
    {
        if (LocalizationManager.Instance == null)
            return fallback;

        string result = LocalizationManager.Instance.Get(key);

        if (string.IsNullOrEmpty(result) || result.StartsWith("["))
            return fallback;

        return result;
    }

    private string GetLocalizedComponentName(MissionComponent comp)
    {
        if (comp == null)
            return "";

        string key = "comp_" + comp.componentType.ToString().ToLower();

        return GetText(key, comp.displayName);
    }
}
