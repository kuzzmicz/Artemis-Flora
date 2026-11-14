
using TMPro;
using UnityEngine;

public class LoadoutUI : MonoBehaviour
{
    [Header("UI References")]
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
        if (MissionManager.Instance == null ||
            LocalizationManager.Instance == null)
            return;

        if (selectedListText == null)
            return;

        var loadout = MissionManager.Instance.currentLoadout;
        var loc = LocalizationManager.Instance;

        if (loadout == null)
            return;

        if (loadout.selectedComponents.Count == 0)
        {
            selectedListText.text =
                loc.Get("selected") + ":\n(" +
                loc.Get("no_components") + ")";

            return;
        }

        string list = loc.Get("selected") + ":\n";

        foreach (var comp in loadout.selectedComponents)
        {
            if (comp == null)
                continue;

            list += $"• {comp.displayName}\n";
        }

        selectedListText.text = list;
    }

    private void UpdateInfoText()
    {
        if (infoGrowingSystemText != null &&
            LocalizationManager.Instance != null)
        {
            infoGrowingSystemText.text =
                LocalizationManager.Instance.Get(
                    "info_growing_system"
                );
        }
    }
}
