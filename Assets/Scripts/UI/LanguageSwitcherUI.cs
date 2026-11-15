
using UnityEngine;
using UnityEngine.UI;

public class LanguageSwitcherUI : MonoBehaviour
{
    [Header("Language Buttons")]
    public Button polishButton;
    public Button englishButton;

    [Header("Colors")]
    public Color activeColor =
        new Color(0.20f, 0.33f, 0.44f, 1f);

    public Color inactiveColor =
        new Color(0.04f, 0.11f, 0.18f, 1f);

    private LocalizationManager localization;

    private void Start()
    {
        localization = LocalizationManager.Instance;

        if (localization == null)
        {
            Debug.LogError(
                "LanguageSwitcherUI: Brak LocalizationManager!"
            );
            return;
        }

        if (polishButton == null || englishButton == null)
        {
            Debug.LogError(
                "LanguageSwitcherUI: Nie przypisano przycisków!"
            );
            return;
        }

        polishButton.onClick.AddListener(SetPolish);
        englishButton.onClick.AddListener(SetEnglish);

        localization.OnLanguageChanged += UpdateVisual;

        UpdateVisual();
    }

    private void SetPolish()
    {
        Debug.Log("KLIKNIĘTO PL!");

        if (localization == null) return;

        localization.SetLanguage(
            LocalizationManager.Language.Polish
        );

        UpdateVisual();
    }

    private void SetEnglish()
    {
        var loc = LocalizationManager.Instance;

        if (loc == null)
        {
            Debug.LogError("BRAK LocalizationManager!");
            return;
        }

        loc.SetLanguage(LocalizationManager.Language.English);

        Debug.Log("Aktualny język: " + loc.currentLanguage);
        Debug.Log("Test tłumaczenia: " + loc.Get("select_components"));

        UpdateVisual();
    }


    private void UpdateVisual()
    {
        if (localization == null) return;

        bool isPolish =
            localization.currentLanguage ==
            LocalizationManager.Language.Polish;

        if (polishButton != null && polishButton.image != null)
            polishButton.image.color =
                isPolish ? activeColor : inactiveColor;

        if (englishButton != null && englishButton.image != null)
            englishButton.image.color =
                isPolish ? inactiveColor : activeColor;
    }

    private void OnDestroy()
    {
        if (polishButton != null)
            polishButton.onClick.RemoveListener(SetPolish);

        if (englishButton != null)
            englishButton.onClick.RemoveListener(SetEnglish);

        if (localization != null)
            localization.OnLanguageChanged -= UpdateVisual;
    }
}
