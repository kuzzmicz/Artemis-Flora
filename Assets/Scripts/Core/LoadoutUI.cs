using TMPro;
using UnityEngine;

public class LoadoutUI : MonoBehaviour
{
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI selectedListText;
    public TextMeshProUGUI infoGrowingSystemText;

    private void OnEnable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnLoadoutChanged += UpdateDisplay;

        UpdateDisplay();
        UpdateInfoText();
    }

    private void OnDisable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnLoadoutChanged -= UpdateDisplay;
    }

    private void Start()
    {
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.OnLoadoutChanged -= UpdateDisplay;
            MissionManager.Instance.OnLoadoutChanged += UpdateDisplay;
        }
        UpdateDisplay();
        UpdateInfoText();
    }

    public void UpdateDisplay()
    {
        if (MissionManager.Instance == null || LocalizationManager.Instance == null) return;

        var loadout = MissionManager.Instance.currentLoadout;
        var loc = LocalizationManager.Instance;

        bool overMass = loadout.TotalMass > MissionManager.Instance.maxMass;
        bool overBudget = loadout.TotalCost > MissionManager.Instance.maxBudget;

        string massColor = overMass ? "<color=red>" : "<color=white>";
        string costColor = overBudget ? "<color=red>" : "<color=white>";

        statsText.text =
            $"{massColor}{loc.Get("mass")}: {loadout.TotalMass:F1} / {MissionManager.Instance.maxMass}</color>\n" +
            $"{costColor}{loc.Get("cost")}: {loadout.TotalCost} / {MissionManager.Instance.maxBudget}</color>\n" +
            $"{loc.Get("power")}: {loadout.TotalPower:F1}\n" +
            $"{loc.Get("reliability")}: {loadout.TotalReliability * 100:F0}%\n" +
            $"{loc.Get("science")}: {loadout.TotalScience}\n" +
            $"{loc.Get("growth")}: {loadout.TotalGrowth * 100:F0}%";

        if (loadout.selectedComponents.Count == 0)
        {
            selectedListText.text = loc.Get("selected") + ":\n(" + loc.Get("no_components") + ")";
        }
        else
        {
            string list = loc.Get("selected") + ":\n";
            foreach (var comp in loadout.selectedComponents)
            {
                list += $"• {comp.displayName}\n";
            }
            selectedListText.text = list;
        }

    }

    private void UpdateInfoText()
    {
        if (infoGrowingSystemText != null && LocalizationManager.Instance != null)
        {
            infoGrowingSystemText.text = LocalizationManager.Instance.Get("info_growing_system");
        }
    }
}