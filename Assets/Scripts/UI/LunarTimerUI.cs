using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LunarTimerUI : MonoBehaviour
{
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI totalDaysText;
    public Image progressBar;

    public float missionDurationSeconds = 140f;
    public int totalLunarDays = 14;

    private float elapsedTime;

    void Start()
    {
        elapsedTime = 0f;

        if (totalDaysText != null)
            totalDaysText.text = $"/ {totalLunarDays} Days";

        RefreshUI();
    }

    void Update()
    {
        elapsedTime = Mathf.Min(elapsedTime + Time.deltaTime, missionDurationSeconds);
        RefreshUI();
    }

    void RefreshUI()
    {
        float progress = missionDurationSeconds > 0f
            ? Mathf.Clamp01(elapsedTime / missionDurationSeconds)
            : 1f;

        int currentDay = Mathf.Clamp(Mathf.FloorToInt(progress * totalLunarDays) + 1, 1, totalLunarDays);

        if (dayText != null)
            dayText.text = currentDay.ToString();

        if (progressBar != null)
            progressBar.fillAmount = progress;
    }
}
