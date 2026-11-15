
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
    private bool miniGameInProgress = false;

    private void Awake()
    {
        Instance = this;

        if (eventPanel != null)
            eventPanel.SetActive(false);
    }

    public void ShowEvent(MissionEvent ev)
    {
        if (ev == null || miniGameInProgress)
            return;

        currentEvent = ev;

        if (LocalizationManager.Instance != null)
        {
            string baseKey =
                "event_" + ev.eventType.ToString().ToLowerInvariant();

            titleText.text =
                GetTranslatedText(baseKey + "_name", ev.eventName);

            descriptionText.text =
                GetTranslatedText(baseKey + "_desc", ev.description);

            choiceAText.text =
                GetTranslatedText(baseKey + "_a", ev.choiceA);

            choiceBText.text =
                GetTranslatedText(baseKey + "_b", ev.choiceB);
        }
        else
        {
            titleText.text = ev.eventName;
            descriptionText.text = ev.description;
            choiceAText.text = ev.choiceA;
            choiceBText.text = ev.choiceB;
        }

        bool hasChoiceC = !string.IsNullOrEmpty(ev.choiceC);

        if (choiceCButton != null)
            choiceCButton.gameObject.SetActive(hasChoiceC);

        if (hasChoiceC && choiceCText != null)
        {
            string key =
                "event_" + ev.eventType.ToString().ToLowerInvariant()
                + "_c";

            choiceCText.text =
                GetTranslatedText(key, ev.choiceC);
        }

        if (eventPanel != null)
            eventPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    private string GetTranslatedText(string key, string fallback)
    {
        var loc = LocalizationManager.Instance;

        if (loc == null)
            return fallback;

        string text = loc.Get(key);

        if (string.IsNullOrEmpty(text) || text.StartsWith("["))
            return fallback;

        return text;
    }

    public void OnChoiceA() => ResolveChoice(0);
    public void OnChoiceB() => ResolveChoice(1);
    public void OnChoiceC() => ResolveChoice(2);

    private void ResolveChoice(int index)
    {
        if (miniGameInProgress)
            return;

        if (currentEvent == null || EventManager.Instance == null)
        {
            Debug.LogWarning(
                "EventUI: Brak aktualnego eventu lub EventManager!"
            );

            CloseEvent();
            return;
        }

        if (currentEvent.eventType == EventType.PumpFailure &&
            index == 2)
        {
            StartPumpMiniGame();
            return;
        }

        EventManager.Instance.ApplyChoice(currentEvent, index);

        ShowFeedback(GetFeedbackMessage(currentEvent, index));

        CloseEvent();
    }

    private void StartPumpMiniGame()
    {
        if (miniGameInProgress)
            return;

        EventManager manager = EventManager.Instance;
        MiniGameTiming miniGame = MiniGameTiming.Instance;

        if (manager == null)
        {
            Debug.LogError(
                "EventUI: Nie znaleziono EventManager!"
            );
            return;
        }

        if (miniGame == null ||
            miniGame.miniGamePanel == null ||
            miniGame.timingSlider == null)
        {
            Debug.LogError(
                "EventUI: MiniGameTiming nie jest poprawnie skonfigurowany!"
            );
            return;
        }

        miniGameInProgress = true;

        if (eventPanel != null)
            eventPanel.SetActive(false);

        miniGame.StartMiniGame(success =>
        {
            if (this == null)
                return;

            miniGameInProgress = false;

            if (manager == null)
            {
                Debug.LogError(
                    "EventUI: EventManager nie istnieje po minigierce! " +
                    "Sprawdź, czy scena nie została zmieniona."
                );

                CloseEvent();
                return;
            }

            if (success)
            {
                manager.currentGrowth -= 0.03f;
                manager.currentReliability += 0.05f;

                ShowFeedback(GetTranslatedText(
                    "fb_pump_success",
                    "Sukces! Pompa została naprawiona."
                ));
            }
            else
            {
                manager.currentGrowth -= 0.15f;
                manager.currentReliability -= 0.08f;

                ShowFeedback(GetTranslatedText(
                    "fb_pump_fail",
                    "Naprawa nieudana. Rośliny ucierpiały."
                ));
            }
            CloseEvent();
        });
    }

    private string GetFeedbackMessage(MissionEvent ev, int choice)
    {
        if (LocalizationManager.Instance == null)
            return "Podjęto decyzję operacyjną.";

        string key =
            $"fb_{ev.eventType.ToString().ToLowerInvariant()}_{choice}";

        string result = LocalizationManager.Instance.Get(key);

        if (result.StartsWith("["))
            return GetTranslatedText(
                "fb_default",
                "Podjęto decyzję operacyjną."
            );

        return result;
    }

    public void ShowFeedback(string message)
    {
        if (feedbackText == null ||
            string.IsNullOrEmpty(message))
            return;

        if (feedbackCoroutine != null)
            StopCoroutine(feedbackCoroutine);

        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);

        Color c = feedbackText.color;
        c.a = 1f;
        feedbackText.color = c;

        feedbackCoroutine =
            StartCoroutine(FadeOutFeedback(3.5f, 1f));
    }

    private IEnumerator FadeOutFeedback(
        float visibleTime,
        float fadeDuration)
    {
        yield return new WaitForSecondsRealtime(visibleTime);

        if (feedbackText == null)
            yield break;

        float elapsed = 0f;
        Color startColor = feedbackText.color;

        while (elapsed < fadeDuration)
        {
            if (feedbackText == null)
                yield break;

            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            feedbackText.color = c;

            yield return null;
        }

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        feedbackCoroutine = null;
    }

    private void CloseEvent()
    {
        if (eventPanel != null)
            eventPanel.SetActive(false);

        currentEvent = null;

        Time.timeScale = 1f;
    }
}
