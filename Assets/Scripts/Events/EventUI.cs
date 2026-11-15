
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

    [Header("NASA Science Fact")]
    public GameObject nasaFactPanel;
    public Button nasaFactButton;
    public Button nasaFactCloseButton;
    public Button nasaSourceButton;
    public TextMeshProUGUI nasaFactTitleText;
    public TextMeshProUGUI nasaFactDescriptionText;
    public TextMeshProUGUI nasaFactImportanceText;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;

    private MissionEvent currentEvent;
    private Coroutine feedbackCoroutine;
    private bool miniGameInProgress = false;

    private void Awake()
    {
        Instance = this;
        if (nasaFactButton != null)
            nasaFactButton.onClick.AddListener(OpenNasaFact);
        if (nasaFactCloseButton != null)
            nasaFactCloseButton.onClick.AddListener(CloseNasaFact);
        if (nasaSourceButton != null)
            nasaSourceButton.onClick.AddListener(OpenNasaSource);
        if (nasaFactPanel != null)
            nasaFactPanel.SetActive(false);

        if (eventPanel != null)
            eventPanel.SetActive(false);
    }

    public void ShowEvent(MissionEvent ev)
    {
        if (ev == null || miniGameInProgress)
            return;

        currentEvent = ev;
        CloseNasaFact();
        RefreshNasaFact();

        string baseKey =
            "event_" + ev.eventType.ToString().ToLowerInvariant();

        if (titleText != null)
            titleText.text = GetTranslatedText(
                baseKey + "_name", ev.eventName);

        if (descriptionText != null)
            descriptionText.text = GetTranslatedText(
                baseKey + "_desc", ev.description);

        if (choiceAText != null)
            choiceAText.text = GetTranslatedText(
                baseKey + "_a", ev.choiceA);

        if (choiceBText != null)
            choiceBText.text = GetTranslatedText(
                baseKey + "_b", ev.choiceB);

        bool hasChoiceC = !string.IsNullOrEmpty(ev.choiceC);

        if (choiceCButton != null)
            choiceCButton.gameObject.SetActive(hasChoiceC);

        if (hasChoiceC && choiceCText != null)
            choiceCText.text = GetTranslatedText(
                baseKey + "_c", ev.choiceC);

        if (eventPanel != null)
            eventPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    private void RefreshNasaFact()
    {
        if (currentEvent == null) return;

        bool english = LocalizationManager.Instance != null &&
            LocalizationManager.Instance.currentLanguage == LocalizationManager.Language.English;

        string title = english ? currentEvent.nasaFactTitleEN : currentEvent.nasaFactTitlePL;
        string fact = english ? currentEvent.nasaFactEN : currentEvent.nasaFactPL;
        string importance = english ? currentEvent.nasaImportanceEN : currentEvent.nasaImportancePL;

        if (string.IsNullOrWhiteSpace(title)) title = currentEvent.nasaFactTitlePL;
        if (string.IsNullOrWhiteSpace(fact)) fact = currentEvent.nasaFactPL;
        if (string.IsNullOrWhiteSpace(importance)) importance = currentEvent.nasaImportancePL;

        if (nasaFactTitleText != null) nasaFactTitleText.text = title;
        if (nasaFactDescriptionText != null) nasaFactDescriptionText.text = fact;
        if (nasaFactImportanceText != null) nasaFactImportanceText.text = importance;

        if (nasaSourceButton != null)
            nasaSourceButton.gameObject.SetActive(!string.IsNullOrWhiteSpace(currentEvent.nasaSourceURL));

        if (nasaFactButton != null)
            nasaFactButton.gameObject.SetActive(!string.IsNullOrWhiteSpace(fact));
    }

    private void OnEnable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged += RefreshLanguage;
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= RefreshLanguage;
    }

    private void RefreshLanguage()
    {
        if (currentEvent == null) return;
        string key = "event_" + currentEvent.eventType.ToString().ToLowerInvariant();

        if (titleText != null)
            titleText.text = GetTranslatedText(key + "_name", currentEvent.eventName);
        if (descriptionText != null)
            descriptionText.text = GetTranslatedText(key + "_desc", currentEvent.description);
        if (choiceAText != null)
            choiceAText.text = GetTranslatedText(key + "_a", currentEvent.choiceA);
        if (choiceBText != null)
            choiceBText.text = GetTranslatedText(key + "_b", currentEvent.choiceB);
        if (choiceCText != null && choiceCButton != null && choiceCButton.gameObject.activeSelf)
            choiceCText.text = GetTranslatedText(key + "_c", currentEvent.choiceC);

        RefreshNasaFact();
    }

    public void OpenNasaFact()
    {
        if (currentEvent == null || miniGameInProgress || nasaFactPanel == null) return;
        RefreshNasaFact();
        nasaFactPanel.SetActive(true);
        nasaFactPanel.transform.SetAsLastSibling();
    }

    public void CloseNasaFact()
    {
        if (nasaFactPanel != null) nasaFactPanel.SetActive(false);
    }

    public void OpenNasaSource()
    {
        if (currentEvent != null &&
            System.Uri.TryCreate(currentEvent.nasaSourceURL, System.UriKind.Absolute, out var uri) &&
            (uri.Scheme == System.Uri.UriSchemeHttps || uri.Scheme == System.Uri.UriSchemeHttp))
            Application.OpenURL(uri.AbsoluteUri);
    }

    private string GetTranslatedText(
        string key, string fallback)
    {
        LocalizationManager loc = LocalizationManager.Instance;

        if (loc == null)
            return fallback;

        string translated = loc.Get(key);

        if (string.IsNullOrEmpty(translated) ||
            translated.StartsWith("["))
        {
            return fallback;
        }

        return translated;
    }

    public void OnChoiceA() => ResolveChoice(0);
    public void OnChoiceB() => ResolveChoice(1);
    public void OnChoiceC() => ResolveChoice(2);

    private void ResolveChoice(int index)
    {
        if (miniGameInProgress)
            return;

        EventManager manager = EventManager.Instance;

        if (currentEvent == null || manager == null)
        {
            Debug.LogWarning(
                "EventUI: Brak wydarzenia lub EventManager!"
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

        if (currentEvent.eventType == EventType.Fire &&
            index == 0)
        {
            StartFireMiniGame();
            return;
        }
        manager.ApplyChoice(currentEvent, index);

        ShowFeedback(
            GetFeedbackMessage(currentEvent, index)
        );

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
                "EventUI: Brak EventManager!"
            );
            return;
        }

        if (miniGame == null ||
            miniGame.miniGamePanel == null ||
            miniGame.timingSlider == null)
        {
            Debug.LogError(
                "EventUI: MiniGameTiming nie jest skonfigurowany!"
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
                    "EventUI: EventManager został zniszczony " +
                    "podczas minigierki pompy!"
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

    private void StartFireMiniGame()
    {
        if (miniGameInProgress)
            return;

        EventManager manager = EventManager.Instance;
        MiniGameFire miniGame = MiniGameFire.Instance;

        if (manager == null)
        {
            Debug.LogError(
                "EventUI: Nie znaleziono EventManager!"
            );
            return;
        }

        if (miniGame == null)
        {
            Debug.LogError(
                "EventUI: Nie znaleziono MiniGameFire!"
            );
            return;
        }

        if (miniGame.panel == null ||
            miniGame.fireIconPrefab == null ||
            miniGame.fireParent == null)
        {
            Debug.LogError(
                "EventUI: MiniGameFire ma nieprzypisane pola!"
            );
            return;
        }

        miniGameInProgress = true;

        if (eventPanel != null)
            eventPanel.SetActive(false);


        miniGame.StartFireGame(success =>
        {
            if (this == null)
                return;

            miniGameInProgress = false;

            if (manager == null)
            {
                Debug.LogError(
                    "EventUI: EventManager został zniszczony " +
                    "podczas minigierki pożaru!"
                );

                CloseEvent();
                return;
            }

            if (success)
            {
                manager.currentGrowth -= 0.03f;
                manager.currentReliability += 0.08f;

                ShowFeedback(GetTranslatedText(
                    "fb_fire_success",
                    "Pożar został ugaszony!"
                ));

                Debug.Log(
                    "MiniGameFire: Sukces! Statystyki zaktualizowane."
                );
            }
            else
            {
                manager.currentGrowth -= 0.12f;
                manager.currentReliability -= 0.06f;

                ShowFeedback(GetTranslatedText(
                    "fb_fire_fail",
                    "Nie udało się ugasić pożaru!"
                ));

                Debug.Log(
                    "MiniGameFire: Porażka! Statystyki zaktualizowane."
                );
            }

            CloseEvent();
        });
    }


    private string GetFeedbackMessage(
        MissionEvent ev, int choice)
    {
        string key =
            $"fb_{ev.eventType.ToString().ToLowerInvariant()}_{choice}";

        return GetTranslatedText(
            key,
            GetTranslatedText(
                "fb_default",
                "Podjęto decyzję operacyjną."
            )
        );
    }

    public void ShowFeedback(string message)
    {
        if (feedbackText == null ||
            string.IsNullOrEmpty(message))
            return;

        if (feedbackCoroutine != null)
        {
            StopCoroutine(feedbackCoroutine);
            feedbackCoroutine = null;
        }

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

            float t = Mathf.Clamp01(
                elapsed / fadeDuration
            );

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
        CloseNasaFact();
        if (eventPanel != null)
            eventPanel.SetActive(false);

        currentEvent = null;
        miniGameInProgress = false;
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        if (nasaFactButton != null) nasaFactButton.onClick.RemoveListener(OpenNasaFact);
        if (nasaFactCloseButton != null) nasaFactCloseButton.onClick.RemoveListener(CloseNasaFact);
        if (nasaSourceButton != null) nasaSourceButton.onClick.RemoveListener(OpenNasaSource);
        if (Instance == this)
            Instance = null;
    }
}
