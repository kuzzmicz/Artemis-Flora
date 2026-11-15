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
        public int level;
        public Action applyUpgrade;
        public Button[] buttons;
        public Image[] backgrounds;
    }

    private UpgradeBar[] bars;
    private GameObject upgradePanel;
    private TMP_Text pointsText;
    private GameObject funFactObject;
    private TMP_FontAsset uiFont;

    private void Awake()
    {
        GetComponent<Button>()?.onClick.AddListener(OnClick);
        InitializeUI();
        RefreshUI();
    }

    private void InitializeUI()
    {
        Transform monitorPanel = FindMonitorPanel();
        if (monitorPanel == null) return;

        Transform funFactTransform = monitorPanel.Find("FunFactText");
        if (funFactTransform != null)
        {
            funFactObject = funFactTransform.gameObject;
            uiFont = funFactTransform.GetComponent<TMP_Text>()?.font;
        }

        CreatePointsDisplay(monitorPanel);
        CreateUpgradePanel(monitorPanel);
    }

    private Transform FindMonitorPanel()
    {
        Transform current = transform.parent;
        while (current != null)
        {
            if (current.name == "MonitorPanel")
                return current;
            current = current.parent;
        }

        return GameObject.Find("MonitorPanel")?.transform;
    }

    private void CreatePointsDisplay(Transform monitorPanel)
    {
        Transform existing = monitorPanel.Find("UpgradePointsText");
        if (existing != null)
        {
            pointsText = existing.GetComponent<TMP_Text>();
            return;
        }

        GameObject pointsObj = new GameObject("UpgradePointsText", typeof(RectTransform), typeof(TextMeshProUGUI));
        pointsObj.transform.SetParent(monitorPanel, false);

        RectTransform rt = pointsObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0f, 280f);
        rt.sizeDelta = new Vector2(360f, 90f);

        pointsText = pointsObj.GetComponent<TextMeshProUGUI>();
        if (uiFont != null) pointsText.font = uiFont;
        pointsText.fontSize = 42f;
        pointsText.fontStyle = FontStyles.Bold;
        pointsText.alignment = TextAlignmentOptions.Center;
        pointsText.color = Color.white;
    }

    private void CreateUpgradePanel(Transform monitorPanel)
    {
        Transform existing = monitorPanel.Find("UpgradePanel");
        if (existing != null)
            Destroy(existing.gameObject);

        upgradePanel = new GameObject("UpgradePanel", typeof(RectTransform));
        upgradePanel.transform.SetParent(monitorPanel, false);

        RectTransform panelRt = upgradePanel.GetComponent<RectTransform>();
        panelRt.anchorMin = new Vector2(0.5f, 0.5f);
        panelRt.anchorMax = new Vector2(0.5f, 0.5f);
        panelRt.pivot = new Vector2(0.5f, 0.5f);
        panelRt.anchoredPosition = new Vector2(0f, -120f);
        panelRt.sizeDelta = new Vector2(960f, 440f);

        bars = new[]
        {
            CreateBarRow("Nawóz (Plant Growth)", 130f, () =>
            {
                if (EventManager.Instance != null)
                    EventManager.Instance.currentGrowth = Mathf.Clamp01(EventManager.Instance.currentGrowth + growthPerUpgrade);
            }),
            CreateBarRow("Awaryjny reaktor (Power)", 0f, () =>
            {
                if (EventManager.Instance != null)
                    EventManager.Instance.currentPower += powerPerUpgrade;
            }),
            CreateBarRow("Zestaw naprawczy (Reliability)", -130f, () =>
            {
                if (EventManager.Instance != null)
                    EventManager.Instance.currentReliability = Mathf.Clamp01(EventManager.Instance.currentReliability + reliabilityPerUpgrade);
            })
        };

        upgradePanel.SetActive(false);
    }

    private UpgradeBar CreateBarRow(string title, float yPos, Action applyAction)
    {
        GameObject rowObj = new GameObject(title, typeof(RectTransform));
        rowObj.transform.SetParent(upgradePanel.transform, false);

        RectTransform rowRt = rowObj.GetComponent<RectTransform>();
        rowRt.anchorMin = new Vector2(0.5f, 0.5f);
        rowRt.anchorMax = new Vector2(0.5f, 0.5f);
        rowRt.pivot = new Vector2(0.5f, 0.5f);
        rowRt.anchoredPosition = new Vector2(0f, yPos);
        rowRt.sizeDelta = new Vector2(940f, 100f);

        CreateLabel(rowObj.transform, title, new Vector2(-290f, 0f), new Vector2(340f, 80f), 26f, TextAlignmentOptions.MidlineLeft);

        UpgradeBar bar = new UpgradeBar
        {
            level = 0,
            applyUpgrade = applyAction,
            buttons = new Button[upgradeCosts.Length],
            backgrounds = new Image[upgradeCosts.Length]
        };

        for (int i = 0; i < upgradeCosts.Length; i++)
        {
            int tierIndex = i;
            float xPos = -10f + i * 195f;
            CreateTierButton(rowObj.transform, bar, tierIndex, xPos);
        }

        return bar;
    }

    private void CreateTierButton(Transform parent, UpgradeBar bar, int tierIndex, float xPos)
    {
        GameObject btnObj = new GameObject($"Tier_{tierIndex + 1}", typeof(RectTransform), typeof(Image), typeof(Button));
        btnObj.transform.SetParent(parent, false);

        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(xPos, 0f);
        rt.sizeDelta = new Vector2(180f, 70f);

        Image bg = btnObj.GetComponent<Image>();
        Button btn = btnObj.GetComponent<Button>();
        btn.targetGraphic = bg;
        btn.onClick.AddListener(() => BuyUpgrade(bar, tierIndex));

        int cost = upgradeCosts[tierIndex];
        CreateLabel(btnObj.transform, $"Poz. {tierIndex + 1}\n({cost} pkt)", Vector2.zero, new Vector2(170f, 64f), 22f, TextAlignmentOptions.Center);

        bar.buttons[tierIndex] = btn;
        bar.backgrounds[tierIndex] = bg;
    }

    private void CreateLabel(Transform parent, string text, Vector2 pos, Vector2 size, float fontSize, TextAlignmentOptions alignment)
    {
        GameObject textObj = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObj.transform.SetParent(parent, false);

        RectTransform rt = textObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
        if (uiFont != null) tmp.font = uiFont;
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = Color.white;
    }

    private void BuyUpgrade(UpgradeBar bar, int tierIndex)
    {
        if (tierIndex != bar.level) return;

        int cost = upgradeCosts[tierIndex];
        if (availablePoints < cost) return;

        availablePoints -= cost;
        bar.level++;
        bar.applyUpgrade?.Invoke();
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (pointsText != null)
            pointsText.text = $"PKT: {availablePoints}";

        if (bars == null) return;

        foreach (UpgradeBar bar in bars)
        {
            for (int i = 0; i < bar.buttons.Length; i++)
            {
                bool bought = i < bar.level;
                bool isNext = i == bar.level;
                bool canAfford = isNext && availablePoints >= upgradeCosts[i];

                bar.buttons[i].interactable = canAfford;

                if (bought)
                    bar.backgrounds[i].color = new Color(0.28f, 0.72f, 0.34f, 1f);
                else if (canAfford)
                    bar.backgrounds[i].color = new Color(0.22f, 0.58f, 0.85f, 1f);
                else
                    bar.backgrounds[i].color = new Color(0.22f, 0.22f, 0.22f, 0.85f);
            }
        }
    }

    public void OnClick()
    {
        if (funFactObject != null)
            funFactObject.SetActive(false);

        if (upgradePanel != null)
            upgradePanel.SetActive(true);

        RefreshUI();
    }
}
