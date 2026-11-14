using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class updateStats : MonoBehaviour
{
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI reliabilityText;
    public TextMeshProUGUI scienceText;
    public TextMeshProUGUI growthText;

    public Image powerProgressBar;
    public Image reliabilityProgressBar;
    public Image scienceProgressBar;
    public Image growthProgressBar;

    public float maxPower = 100f;
    public float maxScience = 200f;

    void Start()
    {
        if (EventManager.Instance != null && MissionManager.Instance == null)
        {
            EventManager.Instance.currentGrowth = 0.50f;
            EventManager.Instance.currentReliability = 0.50f;
            EventManager.Instance.currentScience = 110;
            EventManager.Instance.currentPower = 0.50f;
        }

        RefreshUI();
    }

    void Update()
    {
        RefreshUI();
    }

    void RefreshUI()
    {
        if (EventManager.Instance == null) return;

        float growth = Mathf.Clamp01(EventManager.Instance.currentGrowth);
        float reliability = Mathf.Clamp01(EventManager.Instance.currentReliability);
        int science = Mathf.Max(0, EventManager.Instance.currentScience);
        float power = EventManager.Instance.currentPower;

        UpdateStat(growthProgressBar, growthText, growth, $"{Mathf.RoundToInt(growth * 100f)}%");
        UpdateStat(reliabilityProgressBar, reliabilityText, reliability, $"{Mathf.RoundToInt(reliability * 100f)}%");
        UpdateStat(scienceProgressBar, scienceText, science / maxScience, science.ToString());
        UpdateStat(powerProgressBar, powerText, power / maxPower, $"{Mathf.RoundToInt(Mathf.Clamp01(power / maxPower) * 100f)}%");
    }

    void UpdateStat(Image bar, TextMeshProUGUI label, float fill, string text)
    {
        if (bar != null)
            bar.fillAmount = Mathf.Clamp01(fill);

        if (label != null)
            label.text = text;
    }
}
