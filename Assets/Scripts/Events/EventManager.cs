using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    private void Start()
    {
       
        if (MissionManager.Instance != null)
        {
            var loadout = MissionManager.Instance.currentLoadout;
            currentGrowth = loadout.TotalGrowth;
            currentReliability = loadout.TotalReliability;
            currentScience = loadout.TotalScience;
            currentPower = loadout.TotalPower;
        }

        StartEvents();
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
    }
}