/*
/ The mechanics implemented in this file were entirely our own idea.
/ However, the code in this file was written with the support of Google Antygravity AI tool.
*/

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
        public float fixedHeight;

        public void Apply(LocalizationManager.Language lang)
        {
            if (target == null) return;
            Sprite chosen = lang == LocalizationManager.Language.English ? englishSprite : polishSprite;
            if (chosen == null) return;

            target.sprite = chosen;

            if (fixedHeight > 0f && chosen.rect.height > 0f)
            {
                float width = fixedHeight * (chosen.rect.width / chosen.rect.height);
                target.rectTransform.sizeDelta = new Vector2(width, fixedHeight);

                LayoutElement le = target.GetComponent<LayoutElement>();
                if (le != null && !le.ignoreLayout)
                {
                    le.preferredWidth = width;
                    le.preferredHeight = fixedHeight;
                }
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

    private static readonly Color PanelCardColor = new Color(0.055f, 0.082f, 0.122f, 1f);
    private static readonly Color AccentMintColor = new Color(0f, 0.98823535f, 0.63529414f, 1f);
    private static readonly Color SubtleLabelColor = new Color(0.72f, 0.80f, 0.88f, 1f);

    private readonly Dictionary<string, Transform> nodes = new Dictionary<string, Transform>();

    private TMP_FontAsset baseFont;
    private TMP_Text leftHeaderText;
    private TMP_Text scoreLabelText;
    private TMP_Text scoreValueText;
    private TMP_Text growthLabelText;
    private TMP_Text growthValueText;
    private TMP_Text scienceLabelText;
    private TMP_Text scienceValueText;
    private TMP_Text equipmentListText;
    private LocalizationManager.Language fallbackLanguage = LocalizationManager.Language.Polish;

    private void Awake()
    {
        CacheHierarchyNodes();
        BindSceneReferences();
        CleanupUnusedSceneElements();
        ConfigureLeftColumn();
        BuildCustomSections();
        BindButtons();
    }

    private void OnEnable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged += RefreshAll;
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= RefreshAll;
    }

    private void Start()
    {
        RefreshAll();
    }

    private void CacheHierarchyNodes()
    {
        nodes.Clear();
        foreach (Transform t in transform.root.GetComponentsInChildren<Transform>(true))
        {
            if (!nodes.ContainsKey(t.name))
                nodes[t.name] = t;
        }
    }

    private T FindNode<T>(string objectName) where T : Component
    {
        return nodes.TryGetValue(objectName, out Transform t) ? t.GetComponent<T>() : null;
    }

    private Transform FindTransform(string objectName)
    {
        return nodes.TryGetValue(objectName, out Transform t) ? t : null;
    }

    private void BindSceneReferences()
    {
        powerText ??= FindNode<TMP_Text>("placeholder_power");
        reliabilityText ??= FindNode<TMP_Text>("placeholder_rel");
        missionTimeText ??= FindNode<TMP_Text>("placeholder_missiontime");
        failureReasonText ??= FindNode<TMP_Text>("placeholder_failure");
        massText ??= FindNode<TMP_Text>("placeholder_mass");
        costText ??= FindNode<TMP_Text>("placeholder_cost");
        componentsPrimaryText ??= FindNode<TMP_Text>("placeholder_component");
        componentsSecondaryText ??= FindNode<TMP_Text>("placeholder_comp2");

        progressBar1 ??= FindNode<Image>("MassFilled") ?? FindNode<Image>("progress_in_bar_1");
        progressBar2 ??= FindNode<Image>("CostFilled") ?? FindNode<Image>("progress_in_bar_2");
        statusBannerImage ??= FindNode<Image>("status_baner");

        newMissionButton ??= FindNode<Button>("new_mission");
        changeConfigButton ??= FindNode<Button>("change_config");
        polishLanguageButton ??= FindNode<Button>("iconPL");
        englishLanguageButton ??= FindNode<Button>("iconENG");

        baseFont = (powerText != null ? powerText.font : failureReasonText?.font) ?? TMP_Settings.defaultFontAsset;
    }

    private void CleanupUnusedSceneElements()
    {
        if (failureReasonText != null)
            failureReasonText.gameObject.SetActive(false);

        string[] unused = { "graph?", "preview_button", "failure", "placeholder_failure", "StatsRow", "component", "component_2" };
        foreach (string name in unused)
        {
            Transform t = FindTransform(name);
            if (t != null)
                t.gameObject.SetActive(false);
        }
    }

    private void ConfigureLeftColumn()
    {
        Transform leftHeader = FindTransform("Left_header");
        if (leftHeader != null && leftHeaderText == null)
        {
            Image headerImg = leftHeader.GetComponent<Image>();
            if (headerImg != null)
                headerImg.enabled = false;

            SetLayout(leftHeader, -1f, 26f, 1f);

            Transform existingChild = leftHeader.Find("LeftHeaderText");
            leftHeaderText = existingChild != null
                ? existingChild.GetComponent<TMP_Text>()
                : CreateText("LeftHeaderText", leftHeader, 18f, AccentMintColor, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);

            SetAnchors(leftHeaderText.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        }

        ConfigureInlineLabel("text_mass", 11f);
        ConfigureInlineLabel("text_cost", 11f);
        ConfigureInlineValue(massText);
        ConfigureInlineValue(costText);
        ConfigureProgressBar(progressBar1);
        ConfigureProgressBar(progressBar2);

        ConfigureStatCard("power_bilans", powerText, 84f, false);
        ConfigureStatCard("reliabillity", reliabilityText, 78f, false);
        ConfigureStatCard("mission time", missionTimeText, 124f, true);
    }

    private void ConfigureInlineLabel(string objectName, float height)
    {
        Image img = FindNode<Image>(objectName);
        if (img == null) return;

        img.preserveAspect = true;
        LayoutElement le = img.GetComponent<LayoutElement>();
        if (le != null)
            le.ignoreLayout = true;

        RectTransform rt = img.rectTransform;
        rt.localScale = Vector3.one;
        rt.anchorMin = new Vector2(0f, 0.5f);
        rt.anchorMax = new Vector2(0f, 0.5f);
        rt.pivot = new Vector2(0f, 0.5f);
        rt.anchoredPosition = new Vector2(10f, 0f);

        float width = img.sprite != null && img.sprite.rect.height > 0f
            ? height * (img.sprite.rect.width / img.sprite.rect.height)
            : rt.sizeDelta.x;
        rt.sizeDelta = new Vector2(width, height);
    }

    private void ConfigureInlineValue(TMP_Text label)
    {
        if (label == null) return;

        ApplyTextStyle(label, 14f, Color.white, TextAlignmentOptions.MidlineRight, FontStyles.Bold, TextWrappingModes.NoWrap);

        RectTransform rt = label.rectTransform;
        rt.localScale = Vector3.one;
        rt.anchorMin = new Vector2(1f, 0.5f);
        rt.anchorMax = new Vector2(1f, 0.5f);
        rt.pivot = new Vector2(1f, 0.5f);
        rt.anchoredPosition = new Vector2(-10f, 0f);
        rt.sizeDelta = new Vector2(120f, 24f);
    }

    private void ConfigureProgressBar(Image bar)
    {
        if (bar == null) return;

        bar.type = Image.Type.Filled;
        bar.fillMethod = Image.FillMethod.Horizontal;
        bar.fillOrigin = 0;

        SetAnchors(bar.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
    }

    private void ConfigureStatCard(string cardName, TMP_Text valueText, float preferredHeight, bool valueInBottomRow)
    {
        Image cardImg = FindNode<Image>(cardName);
        if (cardImg != null)
        {
            cardImg.preserveAspect = false;
            SetLayout(cardImg, -1f, preferredHeight, 1f);
        }

        if (valueText == null) return;

        ApplyTextStyle(
            valueText,
            22f,
            Color.white,
            valueInBottomRow ? TextAlignmentOptions.MidlineLeft : TextAlignmentOptions.MidlineRight,
            FontStyles.Bold,
            TextWrappingModes.NoWrap);

        RectTransform rt = valueText.rectTransform;
        rt.localScale = Vector3.one;
        if (valueInBottomRow)
            SetAnchors(rt, Vector2.zero, new Vector2(1f, 0.56f), new Vector2(24f, 10f), new Vector2(-18f, -2f));
        else
            SetAnchors(rt, new Vector2(0.48f, 0f), Vector2.one, Vector2.zero, new Vector2(-20f, 0f));
    }

    private void BuildCustomSections()
    {
        BuildMiddleSummarySection();
        BuildRightEquipmentSection();
    }

    private void BuildMiddleSummarySection()
    {
        Transform middle = FindTransform("middle");
        if (middle == null || middle.Find("ScoreCard") != null) return;

        GameObject scoreCard = CreatePanel("ScoreCard", middle, PanelCardColor, 140f);
        ConfigureVerticalLayout(scoreCard, new RectOffset(24, 24, 16, 16), 6f, TextAnchor.MiddleCenter);
        scoreLabelText = CreateText("ScoreLabel", scoreCard.transform, 20f, SubtleLabelColor, TextAlignmentOptions.Center, FontStyles.Bold);
        scoreValueText = CreateText("ScoreValue", scoreCard.transform, 52f, AccentMintColor, TextAlignmentOptions.Center, FontStyles.Bold);

        GameObject row = new GameObject("KeyStatsRow", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
        row.transform.SetParent(middle, false);
        SetLayout(row, -1f, 120f, 1f);

        HorizontalLayoutGroup hlg = row.GetComponent<HorizontalLayoutGroup>();
        hlg.spacing = 16f;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true;
        hlg.childForceExpandHeight = true;

        GameObject growthCard = CreatePanel("GrowthCard", row.transform, PanelCardColor, 120f);
        ConfigureVerticalLayout(growthCard, new RectOffset(16, 16, 14, 14), 6f, TextAnchor.MiddleCenter);
        growthLabelText = CreateText("GrowthLabel", growthCard.transform, 18f, SubtleLabelColor, TextAlignmentOptions.Center, FontStyles.Bold);
        growthValueText = CreateText("GrowthValue", growthCard.transform, 36f, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);

        GameObject scienceCard = CreatePanel("ScienceCard", row.transform, PanelCardColor, 120f);
        ConfigureVerticalLayout(scienceCard, new RectOffset(16, 16, 14, 14), 6f, TextAnchor.MiddleCenter);
        scienceLabelText = CreateText("ScienceLabel", scienceCard.transform, 18f, SubtleLabelColor, TextAlignmentOptions.Center, FontStyles.Bold);
        scienceValueText = CreateText("ScienceValue", scienceCard.transform, 36f, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);

        int insertIndex = statusBannerImage != null ? statusBannerImage.transform.GetSiblingIndex() + 1 : 1;
        scoreCard.transform.SetSiblingIndex(insertIndex);
        row.transform.SetSiblingIndex(insertIndex + 1);
    }

    private void BuildRightEquipmentSection()
    {
        Transform right = FindTransform("right");
        if (right == null || right.Find("EquipmentListCard") != null) return;

        GameObject listCard = CreatePanel("EquipmentListCard", right, PanelCardColor, 420f, 1f);
        VerticalLayoutGroup vlg = listCard.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(20, 20, 18, 18);
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = true;

        equipmentListText = CreateText("EquipmentListText", listCard.transform, 22f, Color.white, TextAlignmentOptions.TopLeft, FontStyles.Normal);
        equipmentListText.textWrappingMode = TextWrappingModes.Normal;

        Transform raport = right.Find("Raport");
        listCard.transform.SetSiblingIndex(raport != null ? raport.GetSiblingIndex() + 1 : 1);
    }

    private GameObject CreatePanel(string name, Transform parent, Color bgColor, float preferredHeight, float flexibleHeight = -1f)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(LayoutElement));
        panel.transform.SetParent(parent, false);
        panel.GetComponent<Image>().color = bgColor;
        SetLayout(panel, -1f, preferredHeight, 1f, flexibleHeight);
        return panel;
    }

    private void ConfigureVerticalLayout(GameObject target, RectOffset padding, float spacing, TextAnchor alignment)
    {
        VerticalLayoutGroup vlg = target.AddComponent<VerticalLayoutGroup>();
        vlg.padding = padding;
        vlg.spacing = spacing;
        vlg.childAlignment = alignment;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
    }

    private TMP_Text CreateText(string name, Transform parent, float fontSize, Color color, TextAlignmentOptions alignment, FontStyles style)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        obj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
        ApplyTextStyle(tmp, fontSize, color, alignment, style, tmp.textWrappingMode);
        return tmp;
    }

    private void ApplyTextStyle(TMP_Text tmp, float fontSize, Color color, TextAlignmentOptions alignment, FontStyles style, TextWrappingModes wrapping)
    {
        if (baseFont != null) tmp.font = baseFont;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = alignment;
        tmp.fontStyle = style;
        tmp.textWrappingMode = wrapping;
    }

    private static void SetAnchors(RectTransform rt, Vector2 min, Vector2 max, Vector2 offsetMin, Vector2 offsetMax)
    {
        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
        rt.anchoredPosition = (rt.offsetMin + rt.offsetMax) * 0.5f;
    }

    private static void SetLayout(Component target, float preferredWidth, float preferredHeight, float flexibleWidth = -1f, float flexibleHeight = -1f)
    {
        SetLayout(target.gameObject, preferredWidth, preferredHeight, flexibleWidth, flexibleHeight);
    }

    private static void SetLayout(GameObject target, float preferredWidth, float preferredHeight, float flexibleWidth = -1f, float flexibleHeight = -1f)
    {
        LayoutElement le = target.GetComponent<LayoutElement>() ?? target.AddComponent<LayoutElement>();
        le.preferredWidth = preferredWidth;
        le.preferredHeight = preferredHeight;
        le.flexibleWidth = flexibleWidth;
        if (flexibleHeight >= 0f)
            le.flexibleHeight = flexibleHeight;
    }

    private void BindButtons()
    {
        BindButton(newMissionButton, OnNewMissionClicked);
        BindButton(changeConfigButton, OnChangeConfigClicked);
        BindButton(polishLanguageButton, () => SetLanguage(LocalizationManager.Language.Polish));
        BindButton(englishLanguageButton, () => SetLanguage(LocalizationManager.Language.English));
    }

    private static void BindButton(Button button, UnityAction action)
    {
        if (button == null) return;
        button.onClick.RemoveListener(action);
        button.onClick.AddListener(action);
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
        bool failed = ev != null && ev.missionFailed;

        if (powerText != null)
            powerText.text = $"{power:0.#} W";

        if (reliabilityText != null)
            reliabilityText.text = $"{Mathf.RoundToInt(reliability * 100f)}%";

        if (missionTimeText != null)
        {
            string daysLabel = GetLocalized("result_days", GetCurrentLanguage() == LocalizationManager.Language.English ? "days" : "dni");
            missionTimeText.text = $"{days}/{totalDays} {daysLabel}";
        }

        if (scoreValueText != null)
            scoreValueText.text = CalculateScore(science, growth, reliability, days, !failed).ToString("N0");

        if (growthValueText != null)
            growthValueText.text = $"{Mathf.RoundToInt(growth * 100f)}%";

        if (scienceValueText != null)
            scienceValueText.text = $"{science} PTS";
    }

    public int CalculateScore(int science, float growth, float reliability, int daysReached, bool survived)
    {
        return science * 10
            + Mathf.RoundToInt(growth * 100f) * 5
            + Mathf.RoundToInt(reliability * 100f) * 3
            + daysReached * 50
            + (survived ? 500 : 0);
    }

    private void RefreshLoadout()
    {
        var mission = MissionManager.Instance;
        var loadout = mission != null ? mission.currentLoadout : null;
        bool hasData = mission != null && loadout != null;

        if (massText != null)
            massText.text = hasData ? $"{loadout.TotalMass:0.#} / {mission.maxMass:0.#} kg" : "0 / 0 kg";

        if (costText != null)
            costText.text = hasData ? $"{loadout.TotalCost} / {mission.maxBudget} CR" : "0 / 0 CR";

        if (progressBar1 != null)
            progressBar1.fillAmount = hasData && mission.maxMass > 0f ? Mathf.Clamp01(loadout.TotalMass / mission.maxMass) : 0f;

        if (progressBar2 != null)
            progressBar2.fillAmount = hasData && mission.maxBudget > 0 ? Mathf.Clamp01((float)loadout.TotalCost / mission.maxBudget) : 0f;

        RefreshComponentLists(loadout);
    }

    private void RefreshComponentLists(MissionLoadout loadout)
    {
        if (loadout == null || loadout.selectedComponents == null || loadout.selectedComponents.Count == 0)
        {
            string fallbackEmpty = GetCurrentLanguage() == LocalizationManager.Language.English
                ? "No components selected"
                : "Brak wybranych komponentów";
            string emptyText = GetLocalized("no_components", fallbackEmpty);
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
            lines.Add($"• {GetLocalized(key, comp.displayName)}");
        }

        if (equipmentListText != null)
            equipmentListText.text = string.Join("\n", lines);
    }

    private void RefreshLocalization()
    {
        var lang = GetCurrentLanguage();
        var ev = EventManager.Instance;
        bool failed = ev != null && ev.missionFailed;
        bool isEng = lang == LocalizationManager.Language.English;

        if (leftHeaderText != null)
            leftHeaderText.text = GetLocalized("result_telemetry", isEng ? "FINAL TELEMETRY" : "TELEMETRIA KOŃCOWA");

        if (scoreLabelText != null)
            scoreLabelText.text = GetLocalized("result_score", isEng ? "MISSION SCORE" : "WYNIK MISJI");

        if (growthLabelText != null)
            growthLabelText.text = GetLocalized("result_growth", isEng ? "PLANT GROWTH" : "WZROST ROŚLIN");

        if (scienceLabelText != null)
            scienceLabelText.text = GetLocalized("result_science", isEng ? "SCIENCE DATA" : "DANE NAUKOWE");

        if (statusBannerImage != null)
        {
            Sprite banner = failed
                ? (isEng ? failureBannerENG : failureBannerPL)
                : (isEng ? successBannerENG : successBannerPL);

            if (banner != null)
                statusBannerImage.sprite = banner;

            LayoutElement bannerLe = statusBannerImage.GetComponent<LayoutElement>();
            if (bannerLe != null)
                bannerLe.preferredHeight = failed ? 96f : 240f;
        }

        foreach (var pair in localizedSprites)
            pair?.Apply(lang);
    }

    private LocalizationManager.Language GetCurrentLanguage()
    {
        return LocalizationManager.Instance != null
            ? LocalizationManager.Instance.currentLanguage
            : fallbackLanguage;
    }

    private string GetLocalized(string key, string fallback)
    {
        if (LocalizationManager.Instance == null)
            return fallback;

        string result = LocalizationManager.Instance.Get(key);
        return string.IsNullOrEmpty(result) || result.StartsWith("[") ? fallback : result;
    }

    private void SetLanguage(LocalizationManager.Language lang)
    {
        fallbackLanguage = lang;
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.SetLanguage(lang);
        else
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
