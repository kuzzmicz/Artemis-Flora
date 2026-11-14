using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventUI : MonoBehaviour
{
    public static EventUI Instance { get; private set; }

    [Header("Panel")]
    public GameObject eventPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    [Header("Choices")]
    public Button choiceAButton;
    public Button choiceBButton;
    public Button choiceCButton;

    public TextMeshProUGUI choiceAText;
    public TextMeshProUGUI choiceBText;
    public TextMeshProUGUI choiceCText;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;

    private MissionEvent currentEvent;
    private Coroutine feedbackCoroutine;

    private void Awake()
    {
        Instance = this;

        if (eventPanel != null)
            eventPanel.SetActive(false);
    }

    public void ShowEvent(MissionEvent ev)
    {
        if (ev == null) return;

        currentEvent = ev;

        if (LocalizationManager.Instance != null)
        {
            string baseKey = "event_" + ev.eventType.ToString().ToLower();

            titleText.text = LocalizationManager.Instance.Get(baseKey + "_name");
            descriptionText.text = LocalizationManager.Instance.Get(baseKey + "_desc");

            choiceAText.text = LocalizationManager.Instance.Get(baseKey + "_a");
            choiceBText.text = LocalizationManager.Instance.Get(baseKey + "_b");

            bool hasChoiceC = !string.IsNullOrEmpty(ev.choiceC);
            choiceCButton.gameObject.SetActive(hasChoiceC);

            if (hasChoiceC)
                choiceCText.text = LocalizationManager.Instance.Get(baseKey + "_c");
        }
        else
        {
            titleText.text = ev.eventName;
            descriptionText.text = ev.description;
            choiceAText.text = ev.choiceA;
            choiceBText.text = ev.choiceB;
            choiceCText.text = ev.choiceC;
            choiceCButton.gameObject.SetActive(!string.IsNullOrEmpty(ev.choiceC));
        }

        eventPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnChoiceA() => ResolveChoice(0);
    public void OnChoiceB() => ResolveChoice(1);
    public void OnChoiceC() => ResolveChoice(2);

    private void ResolveChoice(int index)
    {
        if (currentEvent == null || EventManager.Instance == null)
        {
            CloseEvent();
            return;
        }

        EventManager.Instance.ApplyChoice(currentEvent, index);
        ShowFeedback(GetFeedbackMessage(currentEvent, index));
        CloseEvent();
    }

    private string GetFeedbackMessage(MissionEvent ev, int choice)
    {
        if (LocalizationManager.Instance == null)
            return "Podjęto decyzję operacyjną.";

        string key = $"fb_{ev.eventType.ToString().ToLower()}_{choice}";
        string result = LocalizationManager.Instance.Get(key);

        if (result.StartsWith("["))
            return LocalizationManager.Instance.Get("fb_default");

        return result;
    }

    public void ShowFeedback(string message)
    {
        if (feedbackText == null || string.IsNullOrEmpty(message)) return;

        if (feedbackCoroutine != null)
            StopCoroutine(feedbackCoroutine);

        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);

        Color c = feedbackText.color;
        c.a = 1f;
        feedbackText.color = c;

        feedbackCoroutine = StartCoroutine(FadeOutFeedback(3.5f, 1f));
    }

    private IEnumerator FadeOutFeedback(float visibleTime, float fadeDuration)
    {
        yield return new WaitForSecondsRealtime(visibleTime);

        float elapsed = 0f;
        Color startColor = feedbackText.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeDuration;

            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            feedbackText.color = c;

            yield return null;
        }

        feedbackText.gameObject.SetActive(false);
    }

    private void CloseEvent()
    {
        if (eventPanel != null)
            eventPanel.SetActive(false);

        Time.timeScale = 1f;
        currentEvent = null;
    }
}