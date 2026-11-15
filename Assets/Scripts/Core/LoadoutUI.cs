
using TMPro;
using UnityEngine;

public class LoadoutUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI selectedListText;
    public TextMeshProUGUI infoGrowingSystemText;

    private MissionManager missionManager;
    private LocalizationManager localization;

    private void OnEnable()
    {
        Subscribe();
        RefreshAll();
    }

    private void Start()
    {
        Subscribe();
        RefreshAll();
    }

    private void Subscribe()
    {
        if (missionManager != MissionManager.Instance)
        {
            if (missionManager != null)
                missionManager.OnLoadoutChanged -= UpdateDisplay;

            missionManager = MissionManager.Instance;

            if (missionManager != null)
                missionManager.OnLoadoutChanged += UpdateDisplay;
        }

        if (localization != LocalizationManager.Instance)
        {
            if (localization != null)
                localization.OnLanguageChanged -= RefreshAll;

            localization = LocalizationManager.Instance;

            if (localization != null)
                localization.OnLanguageChanged += RefreshAll;
        }
    }

    private void OnDisable()
    {
        if (missionManager != null)
            missionManager.OnLoadoutChanged -= UpdateDisplay;

        if (localization != null)
            localization.OnLanguageChanged -= RefreshAll;

        missionManager = null;
        localization = null;
    }

    private void RefreshAll()
    {
        UpdateDisplay();
        UpdateInfoText();
    }

    public void UpdateDisplay()
    {
        var manager = MissionManager.Instance;
        var loc = LocalizationManager.Instance;

        if (manager == null ||
            loc == null ||
            selectedListText == null ||
            manager.currentLoadout == null)
            return;

        var loadout = manager.currentLoadout;

        if (loadout.selectedComponents.Count == 0)
        {
            selectedListText.text = loc.Get("no_components");
            return;
        }

        string list = "";

        foreach (var comp in loadout.selectedComponents)
        {
            if (comp == null)
                continue;

          
            string key = "comp_" +
                comp.componentType.ToString().ToLowerInvariant();

            string name = loc.Get(key);

           
            if (name == "[" + key + "]")
                name = comp.displayName;

            list += "• " + name + "\n";
        }

        selectedListText.text = list.TrimEnd();
    }

    private void UpdateInfoText()
    {
        if (infoGrowingSystemText == null ||
            LocalizationManager.Instance == null)
            return;

        infoGrowingSystemText.text =
            LocalizationManager.Instance.Get(
                "info_growing_system"
            );
    }
}
