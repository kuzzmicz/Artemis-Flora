/*
/ The mechanics implemented in this file were entirely our own idea.
/ However, the code in this file was written with the support of Google Antygravity AI tool.
*/

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MissionResultsUI : MonoBehaviour
{
    [System.Serializable]
    public class LocalizedSpritePair
    {
        public Image target;
        public Sprite polishSprite;
        public Sprite englishSprite;

        public void Apply(LocalizationManager.Language lang)
        {
            if (target == null) return;
            Sprite chosen = lang == LocalizationManager.Language.English ? englishSprite : polishSprite;
            if (chosen != null)
            {
                target.sprite = chosen;
            }
        }
    }

    [Header("Text Placeholders")]
    public TMP_Text powerText;
    public TMP_Text reliabilityText;
    public TMP_Text missionTimeText;
    public TMP_Text failureReasonText;
    public TMP_Text massText;
    public TMP_Text costText;
    public TMP_Text componentsPrimaryText;
    public TMP_Text componentsSecondaryText;

    [Header("Progress Bars (Mass & Cost)")]
    public Image progressBar1;
    public Image progressBar2;

    [Header("Status Banner")]
    public Image statusBannerImage;
    public Sprite successBannerPL;
    public Sprite successBannerENG;
    public Sprite failureBannerPL;
    public Sprite failureBannerENG;

    [Header("Localized UI Sprites")]
    public List<LocalizedSpritePair> localizedSprites = new List<LocalizedSpritePair>();

    [Header("Buttons")]
    public Button newMissionButton;
    public Button changeConfigButton;
    public Button polishLanguageButton;
    public Button englishLanguageButton;

    // Color palette preserved from MissionResults scene
    private static readonly Color PanelCardColor = new Color(0.055f, 0.082f, 0.122f, 1f);
    private static readonly Color AccentMintColor = new Color(0f, 0.98823535f, 0.63529414f, 1f);
    private static readonly Color SubtleLabelColor = new Color(0.72f, 0.80f, 0.88f, 1f);

    private TMP_FontAsset baseFont;
    private TMP_Text scoreLabelText;
    private TMP_Text scoreValueText;
    private TMP_Text growthLabelText;
    private TMP_Text growthValueText;
    private TMP_Text scienceLabelText;
    private TMP_Text scienceValueText;
    private TMP_Text equipmentListText;

    private void Awake()
    {
        AutoBindMissingReferences();
        CleanupUnusedSceneElements();
        BuildCustomSections();
        BindButtons();
    }

    private void Start()
    {
        RefreshAll();
    }

    private void AutoBindMissingReferences()
    {
        powerText ??= FindComponentByName<TMP_Text>("placeholder_power");
        reliabilityText ??= FindComponentByName<TMP_Text>("placeholder_rel");
        missionTimeText ??= FindComponentByName<TMP_Text>("placeholder_missiontime");
        failureReasonText ??= FindComponentByName<TMP_Text>("placeholder_failure");
        massText ??= FindComponentByName<TMP_Text>("placeholder_mass");
        costText ??= FindComponentByName<TMP_Text>("placeholder_cost");
        componentsPrimaryText ??= FindComponentByName<TMP_Text>("placeholder_component");
        componentsSecondaryText ??= FindComponentByName<TMP_Text>("placeholder_comp2");

        progressBar1 ??= FindComponentByName<Image>("MassFilled") ?? FindComponentByName<Image>("progress_in_bar_1");
        progressBar2 ??= FindComponentByName<Image>("CostFilled") ?? FindComponentByName<Image>("progress_in_bar_2");
        statusBannerImage ??= FindComponentByName<Image>("status_baner");

        newMissionButton ??= FindComponentByName<Button>("new_mission");
        changeConfigButton ??= FindComponentByName<Button>("change_config");
        polishLanguageButton ??= FindComponentByName<Button>("iconPL");
        englishLanguageButton ??= FindComponentByName<Button>("iconENG");

        if (powerText != null)
            baseFont = powerText.font;
    }

    private void CleanupUnusedSceneElements()
    {
        Transform middle = GameObject.Find("middle")?.transform;
        if (failureReasonText != null && middle != null)
        {
            if (failureReasonText.transform.parent != middle)
                failureReasonText.transform.SetParent(middle, false);

            int bannerIdx = statusBannerImage != null ? statusBannerImage.transform.GetSiblingIndex() : 0;
            failureReasonText.transform.SetSiblingIndex(bannerIdx + 1);
            failureReasonText.gameObject.SetActive(true);

            RectTransform rt = failureReasonText.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = Vector2.zero;
            rt.localScale = Vector3.one;

            LayoutElement le = failureReasonText.GetComponent<LayoutElement>() ?? failureReasonText.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 44f;
            le.flexibleWidth = 1f;

            failureReasonText.alignment = TextAlignmentOptions.Center;
            failureReasonText.fontSize = 24f;
            failureReasonText.color = Color.white;
            equipmentListText.textWrappingMode = TextWrappingModes.Normal;
        }

        DisableByName("graph?");
        DisableByName("preview_button");
        DisableByName("failure");
        DisableByName("StatsRow");
        DisableByName("component");
        DisableByName("component_2");
    }

    private void DisableByName(string objectName)
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj != null)
            obj.SetActive(false);
    }

    private void BuildCustomSections()
    {
        BuildMiddleSummarySection();
        BuildRightEquipmentSection();
    }

    private void BuildMiddleSummarySection()
    {
        Transform middle = GameObject.Find("middle")?.transform;
        if (middle == null || middle.Find("ScoreCard") != null) return;

        // Score card
        GameObject scoreCard = CreatePanel("ScoreCard", middle, PanelCardColor, 140f);
        VerticalLayoutGroup scoreLayout = scoreCard.AddComponent<VerticalLayoutGroup>();
        scoreLayout.padding = new RectOffset(24, 24, 16, 16);
        scoreLayout.spacing = 6f;
        scoreLayout.childAlignment = TextAnchor.MiddleCenter;
        scoreLayout.childControlWidth = true;
        scoreLayout.childControlHeight = true;
        scoreLayout.childForceExpandWidth = true;
        scoreLayout.childForceExpandHeight = false;

        scoreLabelText = CreateText("ScoreLabel", scoreCard.transform, 20f, SubtleLabelColor, TextAlignmentOptions.Center, FontStyles.Bold);
        scoreValueText = CreateText("ScoreValue", scoreCard.transform, 52f, AccentMintColor, TextAlignmentOptions.Center, FontStyles.Bold);

        // Two-column row for Plant Growth & Science Data
        GameObject row = new GameObject("KeyStatsRow", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
        row.transform.SetParent(middle, false);
        LayoutElement rowLe = row.GetComponent<LayoutElement>();
        rowLe.preferredHeight = 120f;
        rowLe.flexibleWidth = 1f;

        HorizontalLayoutGroup hlg = row.GetComponent<HorizontalLayoutGroup>();
        hlg.spacing = 16f;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true;
        hlg.childForceExpandHeight = true;

        GameObject growthCard = CreatePanel("GrowthCard", row.transform, PanelCardColor, 120f);
        SetupStatCardLayout(growthCard);
        growthLabelText = CreateText("GrowthLabel", growthCard.transform, 18f, SubtleLabelColor, TextAlignmentOptions.Center, FontStyles.Bold);
        growthValueText = CreateText("GrowthValue", growthCard.transform, 36f, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);

        GameObject scienceCard = CreatePanel("ScienceCard", row.transform, PanelCardColor, 120f);
        SetupStatCardLayout(scienceCard);
        scienceLabelText = CreateText("ScienceLabel", scienceCard.transform, 18f, SubtleLabelColor, TextAlignmentOptions.Center, FontStyles.Bold);
        scienceValueText = CreateText("ScienceValue", scienceCard.transform, 36f, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);

        // Place right after failureReasonText (or statusBanner)
        int insertIndex = failureReasonText != null && failureReasonText.transform.parent == middle
            ? failureReasonText.transform.GetSiblingIndex() + 1
            : 1;
        scoreCard.transform.SetSiblingIndex(insertIndex);
        row.transform.SetSiblingIndex(insertIndex + 1);
    }

    private void SetupStatCardLayout(GameObject card)
    {
        VerticalLayoutGroup vlg = card.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(16, 16, 14, 14);
        vlg.spacing = 6f;
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
    }

    private void BuildRightEquipmentSection()
    {
        Transform right = GameObject.Find("right")?.transform;
        if (right == null || right.Find("EquipmentListCard") != null) return;

        GameObject listCard = CreatePanel("EquipmentListCard", right, PanelCardColor, 420f);
        LayoutElement le = listCard.GetComponent<LayoutElement>();
        le.flexibleHeight = 1f;

        VerticalLayoutGroup vlg = listCard.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(20, 20, 18, 18);
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = true;

        equipmentListText = CreateText("EquipmentListText", listCard.transform, 22f, Color.white, TextAlignmentOptions.TopLeft, FontStyles.Normal);
        equipmentListText.textWrappingMode = TextWrappingModes.Normal;

        Transform raport = right.Find("Raport");
        int insertIndex = raport != null ? raport.GetSiblingIndex() + 1 : 1;
        listCard.transform.SetSiblingIndex(insertIndex);
    }

    private GameObject CreatePanel(string name, Transform parent, Color bgColor, float preferredHeight)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(LayoutElement));
        panel.transform.SetParent(parent, false);
        panel.GetComponent<Image>().color = bgColor;
        LayoutElement le = panel.GetComponent<LayoutElement>();
        le.preferredHeight = preferredHeight;
        le.flexibleWidth = 1f;
        return panel;
    }

    private TMP_Text CreateText(string name, Transform parent, float fontSize, Color color, TextAlignmentOptions alignment, FontStyles style)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        obj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
        if (baseFont != null) tmp.font = baseFont;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = alignment;
        tmp.fontStyle = style;
        return tmp;
    }

    private T FindComponentByName<T>(string objectName) where T : Component
    {
        GameObject obj = GameObject.Find(objectName);
        return obj != null ? obj.GetComponent<T>() : null;
    }

    private void BindButtons()
    {
        if (newMissionButton != null)
        {
            newMissionButton.onClick.RemoveListener(OnNewMissionClicked);
            newMissionButton.onClick.AddListener(OnNewMissionClicked);
        }

        if (changeConfigButton != null)
        {
            changeConfigButton.onClick.RemoveListener(OnChangeConfigClicked);
            changeConfigButton.onClick.AddListener(OnChangeConfigClicked);
        }

        if (polishLanguageButton != null)
        {
            polishLanguageButton.onClick.RemoveListener(OnSelectPolish);
            polishLanguageButton.onClick.AddListener(OnSelectPolish);
        }

        if (englishLanguageButton != null)
        {
            englishLanguageButton.onClick.RemoveListener(OnSelectEnglish);
            englishLanguageButton.onClick.AddListener(OnSelectEnglish);
        }
    }

    public void RefreshAll()
    {
        RefreshStats();
        RefreshLoadout();
        RefreshLocalization();
    }

    private void RefreshStats()
    {
        var ev = EventManager.Instance;
        float growth = ev != null ? Mathf.Clamp01(ev.currentGrowth) : 0f;
        float reliability = ev != null ? Mathf.Clamp01(ev.currentReliability) : 0f;
        int science = ev != null ? Mathf.Max(0, ev.currentScience) : 0;
        float power = ev != null ? Mathf.Max(0f, ev.currentPower) : 0f;
        int days = ev != null ? ev.missionDaysReached : 1;
        int totalDays = ev != null ? ev.missionTotalDays : 14;
        float elapsed = ev != null ? ev.missionElapsedSeconds : 0f;
        bool failed = ev != null && ev.missionFailed;

        if (powerText != null)
            powerText.text = $"{power:0.#} W";

        if (reliabilityText != null)
            reliabilityText.text = $"{Mathf.RoundToInt(reliability * 100f)}%";

        if (missionTimeText != null)
        {
            string daysLabel = GetLocalized("result_days", "dni");
            missionTimeText.text = $"{days}/{totalDays} {daysLabel}";
        }

        int score = CalculateScore(science, growth, reliability, days, !failed);
        if (scoreValueText != null)
            scoreValueText.text = score.ToString("N0");

        if (growthValueText != null)
            growthValueText.text = $"{Mathf.RoundToInt(growth * 100f)}%";

        if (scienceValueText != null)
            scienceValueText.text = $"{science} PTS";
    }

    public int CalculateScore(int science, float growth, float reliability, int daysReached, bool survived)
    {
        int scienceScore = science * 10;
        int growthScore = Mathf.RoundToInt(growth * 100f) * 5;
        int reliabilityScore = Mathf.RoundToInt(reliability * 100f) * 3;
        int daysScore = daysReached * 50;
        int survivalBonus = survived ? 500 : 0;
        return scienceScore + growthScore + reliabilityScore + daysScore + survivalBonus;
    }

    private void RefreshLoadout()
    {
        var mission = MissionManager.Instance;
        var loadout = mission != null ? mission.currentLoadout : null;

        if (massText != null)
        {
            massText.text = mission != null && loadout != null
                ? $"{loadout.TotalMass:0.#} / {mission.maxMass:0.#} kg"
                : "0 / 0 kg";
        }

        if (costText != null)
        {
            costText.text = mission != null && loadout != null
                ? $"{loadout.TotalCost} / {mission.maxBudget} CR"
                : "0 / 0 CR";
        }

        if (progressBar1 != null)
        {
            progressBar1.fillAmount = mission != null && loadout != null && mission.maxMass > 0f
                ? Mathf.Clamp01(loadout.TotalMass / mission.maxMass)
                : 0f;
        }

        if (progressBar2 != null)
        {
            progressBar2.fillAmount = mission != null && loadout != null && mission.maxBudget > 0
                ? Mathf.Clamp01((float)loadout.TotalCost / mission.maxBudget)
                : 0f;
        }

        RefreshComponentLists(loadout);
    }

    private void RefreshComponentLists(MissionLoadout loadout)
    {
        if (loadout == null || loadout.selectedComponents == null || loadout.selectedComponents.Count == 0)
        {
            string emptyText = GetLocalized("no_components", "Brak wybranych komponentów");
            if (equipmentListText != null) equipmentListText.text = emptyText;
            if (componentsPrimaryText != null) componentsPrimaryText.text = emptyText;
            if (componentsSecondaryText != null) componentsSecondaryText.text = string.Empty;
            return;
        }

        List<string> lines = new List<string>();
        foreach (var comp in loadout.selectedComponents)
        {
            if (comp == null) continue;
            string key = "comp_" + comp.componentType.ToString().ToLower();
            string name = GetLocalized(key, comp.displayName);
            lines.Add($"• {name}");
        }

        if (equipmentListText != null)
            equipmentListText.text = string.Join("\n", lines);
    }

    private void RefreshLocalization()
    {
        var loc = LocalizationManager.Instance;
        var lang = loc != null ? loc.currentLanguage : LocalizationManager.Language.Polish;
        var ev = EventManager.Instance;
        bool failed = ev != null && ev.missionFailed;

        if (scoreLabelText != null)
            scoreLabelText.text = GetLocalized("result_score", "WYNIK MISJI");

        if (growthLabelText != null)
            growthLabelText.text = GetLocalized("result_growth", "WZROST ROŚLIN");

        if (scienceLabelText != null)
            scienceLabelText.text = GetLocalized("result_science", "DANE NAUKOWE");

        if (failureReasonText != null)
        {
            string reasonKey = ev != null && !string.IsNullOrEmpty(ev.missionEndReasonKey)
                ? ev.missionEndReasonKey
                : (failed ? "result_reason_unknown" : "result_reason_success");

            failureReasonText.text = GetLocalized(reasonKey, GetReasonFallback(reasonKey));
        }

        if (statusBannerImage != null)
        {
            Sprite banner = failed
                ? (lang == LocalizationManager.Language.English ? failureBannerENG : failureBannerPL)
                : (lang == LocalizationManager.Language.English ? successBannerENG : successBannerPL);

            if (banner != null)
                statusBannerImage.sprite = banner;
        }

        foreach (var pair in localizedSprites)
        {
            pair?.Apply(lang);
        }
    }

    private string GetReasonFallback(string reasonKey)
    {
        switch (reasonKey)
        {
            case "result_reason_success": return "Misja zakończona sukcesem (14/14 dni).";
            case "result_reason_power": return "Porażka: wyczerpano energię (0 W).";
            case "result_reason_growth": return "Porażka: obumarcie uprawy (0% wzrostu).";
            default: return "Misja zakończona.";
        }
    }

    private string GetLocalized(string key, string fallback)
    {
        if (LocalizationManager.Instance == null)
            return fallback;

        string result = LocalizationManager.Instance.Get(key);
        return string.IsNullOrEmpty(result) || result.StartsWith("[") ? fallback : result;
    }

    private void OnSelectPolish()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.SetLanguage(LocalizationManager.Language.Polish);
        RefreshAll();
    }

    private void OnSelectEnglish()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.SetLanguage(LocalizationManager.Language.English);
        RefreshAll();
    }

    public void OnNewMissionClicked()
    {
        MissionManager.Instance?.ClearLoadout();
        EventManager.Instance?.ResetMissionState();
        SceneManager.LoadScene("MissionDesign");
    }

    public void OnChangeConfigClicked()
    {
        EventManager.Instance?.ResetMissionState();
        SceneManager.LoadScene("MissionDesign");
    }
}
