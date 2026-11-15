using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    [Header("Events")]
    public List<MissionEvent> allEvents = new List<MissionEvent>();

    [Header("Timing")]
    public float minTimeBetweenEvents = 12f;
    public float maxTimeBetweenEvents = 25f;

    [Header("Current Mission Stats")]
    public float currentGrowth = 0.5f;
    public float currentReliability = 0.8f;
    public int currentScience = 0;
    public float currentPower = 0f;

    public bool isMissionOver { get; private set; }
    public bool missionFailed { get; private set; }
    public string missionEndReasonKey { get; private set; } = string.Empty;
    public float missionElapsedSeconds { get; private set; }
    public int missionDaysReached { get; private set; } = 1;
    public int missionTotalDays { get; private set; } = 14;

    private bool eventsRunning = false;
    private Coroutine eventCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;

        if (scene.name == "MissionStage")
        {
            StartMission();
        }
        else
        {
            StopEvents();
        }
    }

    private void Start()
    {
        if (!eventsRunning && !isMissionOver && SceneManager.GetActiveScene().name == "MissionStage")
        {
            StartMission();
        }
    }

    public void StartMission()
    {
        ResetMissionState();

        if (MissionManager.Instance != null && MissionManager.Instance.currentLoadout != null)
        {
            var loadout = MissionManager.Instance.currentLoadout;
            currentGrowth = loadout.TotalGrowth;
            currentReliability = loadout.TotalReliability;
            currentScience = loadout.TotalScience;
            currentPower = Mathf.Max(0f, loadout.TotalPower);
        }

        StartEvents();
    }

    public void ResetMissionState()
    {
        StopEvents();
        isMissionOver = false;
        missionFailed = false;
        missionEndReasonKey = string.Empty;
        missionElapsedSeconds = 0f;
        missionDaysReached = 1;
        missionTotalDays = 14;
    }

    public void UpdateMissionProgress(float elapsedSeconds, int currentDay, int totalDays)
    {
        missionElapsedSeconds = elapsedSeconds;
        missionDaysReached = currentDay;
        missionTotalDays = totalDays;
    }

    public void StartEvents()
    {
        if (eventsRunning) return;
        eventsRunning = true;
        eventCoroutine = StartCoroutine(EventLoop());
    }

    public void StopEvents()
    {
        eventsRunning = false;
        if (eventCoroutine != null)
            StopCoroutine(eventCoroutine);
    }

    private IEnumerator EventLoop()
    {
        while (eventsRunning)
        {
            float waitTime = Random.Range(minTimeBetweenEvents, maxTimeBetweenEvents);
            yield return new WaitForSeconds(waitTime);

            if (!eventsRunning) yield break;

            MissionEvent randomEvent = GetWeightedRandomEvent();
            if (randomEvent != null && EventUI.Instance != null)
            {
                EventUI.Instance.ShowEvent(randomEvent);
            }
        }
    }

    private MissionEvent GetWeightedRandomEvent()
    {
        if (allEvents == null || allEvents.Count == 0) return null;

        float totalWeight = 0f;
        List<float> weights = new List<float>();

        foreach (var ev in allEvents)
        {
            float weight = ev.baseWeight;

        
            if (MissionManager.Instance != null)
            {
                var loadout = MissionManager.Instance.currentLoadout;

                bool hasStrongShield = loadout.selectedComponents.Exists(c =>
                    c.componentType == ComponentType.RadiationShield_Heavy);

                bool hasRedundancy = loadout.selectedComponents.Exists(c =>
                    c.componentType == ComponentType.RedundancySystem);

                if (ev.eventType == EventType.RadiationSpike && hasStrongShield)
                    weight *= 0.4f;

                if (ev.eventType == EventType.PumpFailure && hasRedundancy)
                    weight *= 0.5f;
            }

            weights.Add(weight);
            totalWeight += weight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        for (int i = 0; i < allEvents.Count; i++)
        {
            cumulative += weights[i];
            if (randomValue <= cumulative)
                return allEvents[i];
        }

        return allEvents[allEvents.Count - 1];
    }

    public void ApplyChoice(MissionEvent ev, int choiceIndex)
    {
        if (ev == null) return;

        switch (choiceIndex)
        {
            case 0:
                currentGrowth += ev.growthChangeA;
                currentReliability += ev.reliabilityChangeA;
                currentScience += ev.scienceChangeA;
                currentPower += ev.powerChangeA;
                break;
            case 1:
                currentGrowth += ev.growthChangeB;
                currentReliability += ev.reliabilityChangeB;
                currentScience += ev.scienceChangeB;
                currentPower += ev.powerChangeB;
                break;
            case 2:
                currentGrowth += ev.growthChangeC;
                currentReliability += ev.reliabilityChangeC;
                currentScience += ev.scienceChangeC;
                currentPower += ev.powerChangeC;
                break;
        }

        currentGrowth = Mathf.Clamp01(currentGrowth);
        currentReliability = Mathf.Clamp01(currentReliability);
        currentScience = Mathf.Max(0, currentScience);
        currentPower = Mathf.Max(0f, currentPower);

        if (currentPower <= 0f)
        {
            FailMission("result_reason_power");
        }
        else if (currentGrowth <= 0f)
        {
            FailMission("result_reason_growth");
        }
    }

    public void CompleteMission(string reasonKey = "result_reason_success")
    {
        EndMission(failed: false, reasonKey: reasonKey);
    }

    public void FailMission(string reasonKey = "result_reason_power")
    {
        EndMission(failed: true, reasonKey: reasonKey);
    }

    private void EndMission(bool failed, string reasonKey)
    {
        if (isMissionOver) return;

        isMissionOver = true;
        missionFailed = failed;
        missionEndReasonKey = reasonKey;
        StopEvents();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MissionResults");
    }
}