
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LunarTimerUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI totalDaysText;
    public Image progressBar;

    [Header("Mission Settings")]
    public float missionDurationSeconds = 140f;
    public int totalLunarDays = 14;

    private float elapsedTime = 0f;
    private bool isPaused = false;
    private bool missionCompleted = false;

    public bool IsPaused => isPaused;

    private void Start()
    {
        elapsedTime = 0f;
        isPaused = false;
        missionCompleted = false;

        if (totalDaysText != null)
            totalDaysText.text = $"/ {totalLunarDays} Days";

        RefreshUI();
    }

    private void Update()
    {
        if (isPaused || missionCompleted)
            return;

        elapsedTime = Mathf.Min(
            elapsedTime + Time.deltaTime,
            missionDurationSeconds
        );

        int currentDay = RefreshUI();

        if (EventManager.Instance != null)
        {
            EventManager.Instance.UpdateMissionProgress(elapsedTime, currentDay, totalLunarDays);
        }

        if (elapsedTime >= missionDurationSeconds)
        {
            missionCompleted = true;
            if (EventManager.Instance != null)
            {
                EventManager.Instance.CompleteMission();
            }
        }
    }

    public void PauseTimer()
    {
        isPaused = true;
        Debug.Log("LunarTimerUI: Timer zatrzymany");
    }

    public void ResumeTimer()
    {
        isPaused = false;
        Debug.Log("LunarTimerUI: Timer wznowiony");
    }

    private int RefreshUI()
    {
        float progress = missionDurationSeconds > 0f
            ? Mathf.Clamp01(
                elapsedTime / missionDurationSeconds)
            : 1f;

        int currentDay = Mathf.Clamp(
            Mathf.FloorToInt(progress * totalLunarDays) + 1,
            1,
            totalLunarDays
        );

        if (dayText != null)
            dayText.text = currentDay.ToString();

        if (progressBar != null)
            progressBar.fillAmount = progress;

        return currentDay;
    }
}
