/*
/ The mechanics implemented in this file were entirely our own idea.
/ However, the code in this file was written with the support of Google Antygravity AI tool.
*/

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AppUpgrade : MonoBehaviour
{
    public int availablePoints = 8;
    public int[] upgradeCosts = { 1, 2, 3 };
    public float growthPerUpgrade = 0.08f;
    public float powerPerUpgrade = 10f;
    public float reliabilityPerUpgrade = 0.08f;

    private class UpgradeBar
    {
        public string localizationKey;
        public string fallbackTitle;
        public TMP_Text titleText;
        public int level;
        public Action applyUpgrade;
        public Button[] buttons;
        public TMP_Text[] buttonLabels;
    }

    private UpgradeBar[] bars;
    private GameObject upgradePanel;
    private TMP_Text pointsText;
    private GameObject funFactObject;
    private TMP_FontAsset uiFont;

    private void Awake()
    {
        GetComponent<Button>()?.onClick.AddListener(OnClick);

        Transform monitorPanel = transform.parent != null && transform.parent.name == "MonitorPanel"
            ? transform.parent
            : GameObject.Find("MonitorPanel")?.transform;

        if (monitorPanel == null) return;

        Transform funFactTransform = monitorPanel.Find("FunFactText");
        if (funFactTransform != null)
        {
            funFactObject = funFactTransform.gameObject;
            uiFont = funFactTransform.GetComponent<TMP_Text>()?.font;
        }

        pointsText = monitorPanel.Find("UpgradePointsText")?.GetComponent<TMP_Text>()
            ?? CreateLabel(monitorPanel, "UpgradePointsText", string.Empty, new Vector2(0f, 280f), new Vector2(360f, 90f), 42f, TextAlignmentOptions.Center, FontStyles.Bold);

        CreateUpgradePanel(monitorPanel);
        RefreshUI();
    }

    private void OnEnable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= RefreshUI;
            LocalizationManager.Instance.OnLanguageChanged += RefreshUI;
        }

        RefreshUI();
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= RefreshUI;
    }

    private RectTransform CreateUIObject(string objectName, Transform parent, Vector2 pos, Vector2 size, params Type[] components)
    {
        GameObject obj = new GameObject(objectName, components);
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return rt;
    }

    private TMP_Text CreateLabel(Transform parent, string objectName, string text, Vector2 pos, Vector2 size, float fontSize, TextAlignmentOptions alignment, FontStyles style = FontStyles.Normal)
    {
        TextMeshProUGUI tmp = CreateUIObject(objectName, parent, pos, size, typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        if (uiFont != null) tmp.font = uiFont;
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = alignment;
        tmp.color = Color.white;
        return tmp;
    }

    private void CreateUpgradePanel(Transform monitorPanel)
    {
        Transform existing = monitorPanel.Find("UpgradePanel");
        if (existing != null) Destroy(existing.gameObject);

        upgradePanel = CreateUIObject("UpgradePanel", monitorPanel, new Vector2(0f, -120f), new Vector2(960f, 440f), typeof(RectTransform)).gameObject;

        bars = new[]
        {
            CreateBarRow("Upgrade_Fertilizer", "upgrade_fertilizer", "Nawóz (Wzrost roślin)", 130f, () =>
            {
                if (EventManager.Instance != null)
                    EventManager.Instance.currentGrowth = Mathf.Clamp01(EventManager.Instance.currentGrowth + growthPerUpgrade);
            }),
            CreateBarRow("Upgrade_Reactor", "upgrade_reactor", "Awaryjny reaktor (Moc)", 0f, () =>
            {
                if (EventManager.Instance != null)
                    EventManager.Instance.currentPower += powerPerUpgrade;
            }),
            CreateBarRow("Upgrade_RepairKit", "upgrade_repair_kit", "Zestaw naprawczy (Niezawodność)", -130f, () =>
            {
                if (EventManager.Instance != null)
                    EventManager.Instance.currentReliability = Mathf.Clamp01(EventManager.Instance.currentReliability + reliabilityPerUpgrade);
            })
        };

        upgradePanel.SetActive(false);
    }

    private UpgradeBar CreateBarRow(string rowName, string localizationKey, string fallbackTitle, float yPos, Action applyAction)
    {
        Transform row = CreateUIObject(rowName, upgradePanel.transform, new Vector2(0f, yPos), new Vector2(940f, 100f), typeof(RectTransform));
        TMP_Text rowLabel = CreateLabel(row, "Label", fallbackTitle, new Vector2(-290f, 0f), new Vector2(340f, 80f), 26f, TextAlignmentOptions.MidlineLeft);

        UpgradeBar bar = new UpgradeBar
        {
            localizationKey = localizationKey,
            fallbackTitle = fallbackTitle,
            titleText = rowLabel,
            applyUpgrade = applyAction,
            buttons = new Button[upgradeCosts.Length],
            buttonLabels = new TMP_Text[upgradeCosts.Length]
        };

        for (int i = 0; i < upgradeCosts.Length; i++)
        {
            int tierIndex = i;
            RectTransform btnRt = CreateUIObject($"Tier_{i + 1}", row, new Vector2(-10f + i * 195f, 0f), new Vector2(180f, 70f), typeof(RectTransform), typeof(Image), typeof(Button));
            Button btn = btnRt.GetComponent<Button>();
            btn.targetGraphic = btnRt.GetComponent<Image>();
            btn.onClick.AddListener(() => BuyUpgrade(bar, tierIndex));
            bar.buttonLabels[i] = CreateLabel(btnRt, "Label", $"Poz. {i + 1}\n({upgradeCosts[i]} pkt)", Vector2.zero, new Vector2(170f, 64f), 22f, TextAlignmentOptions.Center);
            bar.buttons[i] = btn;
        }

        return bar;
    }

    private void BuyUpgrade(UpgradeBar bar, int tierIndex)
    {
        if (tierIndex != bar.level || availablePoints < upgradeCosts[tierIndex]) return;

        availablePoints -= upgradeCosts[tierIndex];
        bar.level++;
        bar.applyUpgrade?.Invoke();
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (pointsText != null)
        {
            string pointsFormat = LocalizationManager.GetText("upgrade_points", "PKT: {0}");
            pointsText.text = string.Format(pointsFormat, availablePoints);
        }

        if (bars == null) return;

        string tierFormat = LocalizationManager.GetText("upgrade_tier", "Poz. {0}\n({1} pkt)");

        foreach (UpgradeBar bar in bars)
        {
            if (bar.titleText != null)
                bar.titleText.text = LocalizationManager.GetText(bar.localizationKey, bar.fallbackTitle);

            for (int i = 0; i < bar.buttons.Length; i++)
            {
                if (bar.buttonLabels != null && bar.buttonLabels[i] != null)
                    bar.buttonLabels[i].text = string.Format(tierFormat, i + 1, upgradeCosts[i]);

                bool bought = i < bar.level;
                bool canAfford = i == bar.level && availablePoints >= upgradeCosts[i];

                bar.buttons[i].interactable = canAfford;
                bar.buttons[i].image.color = bought
                    ? new Color(0.28f, 0.72f, 0.34f, 1f)
                    : canAfford
                        ? new Color(0.22f, 0.58f, 0.85f, 1f)
                        : new Color(0.22f, 0.22f, 0.22f, 0.85f);
            }
        }
    }

    public void OnClick()
    {
        LocalizationManager loc = LocalizationManager.EnsureInstance();
        if (loc != null)
        {
            loc.OnLanguageChanged -= RefreshUI;
            loc.OnLanguageChanged += RefreshUI;
        }

        if (funFactObject != null) funFactObject.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(true);
        RefreshUI();
    }
}
