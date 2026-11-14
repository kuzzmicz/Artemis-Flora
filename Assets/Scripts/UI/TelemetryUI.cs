
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TelemetryUI : MonoBehaviour
{
    [Header("Mass")]
    public TextMeshProUGUI massValueText;
    public TextMeshProUGUI massLimitText;
    public Image massBarFill;

    [Header("Cost")]
    public TextMeshProUGUI costValueText;
    public TextMeshProUGUI costLimitText;
    public Image costBarFill;

    [Header("Other Stats")]
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI reliabilityText;
    public TextMeshProUGUI scienceText;
    public TextMeshProUGUI growthText;

    [Header("Settings")]
    [Tooltip("Włącz, jeśli bonus 5% zapisujesz jako 0.05")]
    public bool bonusesStoredAsFractions = true;

    private MissionManager manager;

    private void Start()
    {
        manager = MissionManager.Instance;

        if (manager == null)
        {
            Debug.LogError(
                "TelemetryUI: Nie znaleziono MissionManager!"
            );
            return;
        }

        manager.OnLoadoutChanged += RefreshTelemetry;

        RefreshTelemetry();
    }

    private void OnEnable()
    {
        if (MissionManager.Instance != null)
            RefreshTelemetry();
    }

    private void OnDestroy()
    {
        if (manager != null)
            manager.OnLoadoutChanged -= RefreshTelemetry;
    }

    public void RefreshTelemetry()
    {
        var mission = MissionManager.Instance;

        if (mission == null ||
            mission.currentLoadout == null)
            return;

        var loadout = mission.currentLoadout;

        if (massValueText != null)
            massValueText.text =
                $"{loadout.TotalMass:0.0}";

        if (massLimitText != null)
            massLimitText.text =
                $"/ {mission.maxMass:0.#} kg";

        if (massBarFill != null)
        {
            massBarFill.fillAmount =
                mission.maxMass > 0
                ? Mathf.Clamp01(
                    loadout.TotalMass / mission.maxMass)
                : 0f;
        }

        if (costValueText != null)
            costValueText.text =
                $"{loadout.TotalCost}";

        if (costLimitText != null)
            costLimitText.text =
                $"/ {mission.maxBudget} CR";

        if (costBarFill != null)
        {
            costBarFill.fillAmount =
                mission.maxBudget > 0
                ? Mathf.Clamp01(
                    (float)loadout.TotalCost / mission.maxBudget)
                : 0f;
        }

        float totalPower = 0f;
        float totalReliability = 0f;
        int totalScience = 0;
        float totalGrowth = 0f;

        foreach (MissionComponent component
                 in loadout.selectedComponents)
        {
            if (component == null)
                continue;

            totalPower += component.power;
            totalReliability += component.reliabilityBonus;
            totalScience += component.scienceValue;
            totalGrowth += component.growthBonus;
        }

        float multiplier =
            bonusesStoredAsFractions ? 100f : 1f;

        float reliabilityPercent =
            totalReliability * multiplier;

        float growthPercent =
            totalGrowth * multiplier;

        if (powerText != null)
            powerText.text =
                $"{totalPower:+0.0;-0.0;+0.0} W";

        if (reliabilityText != null)
            reliabilityText.text =
                $"{reliabilityPercent:+0.#;-0.#;+0.#}%";

        if (scienceText != null)
            scienceText.text =
                $"{totalScience:+0;-0;+0} PTS";

        if (growthText != null)
            growthText.text =
                $"{growthPercent:+0.#;-0.#;+0.#}%";
    }
}
