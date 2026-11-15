
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    [Header("Localization Key")]
    public string localizationKey;

    private TMP_Text textComponent;
    private LocalizationManager localization;

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        localization = LocalizationManager.Instance;

        if (localization == null)
            return;

        localization.OnLanguageChanged += RefreshText;

        RefreshText();
    }

    private void OnDestroy()
    {
        if (localization != null)
            localization.OnLanguageChanged -= RefreshText;
    }

    public void RefreshText()
    {
        if (localization == null || textComponent == null)
            return;

        string translated =
            localization.Get(localizationKey);

        if (!translated.StartsWith("["))
            textComponent.text = translated;
    }
}
