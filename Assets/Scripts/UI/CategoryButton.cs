
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CategoryButton : MonoBehaviour
{
    public ComponentCategory category;
    public bool showAll = false;
    public ComponentListUI listUI;
    public TextMeshProUGUI buttonText;

    private Button button;
    private Image image;

    private LocalizationManager localization;

    private Color normalColor =
        new Color(0.15f, 0.28f, 0.36f);

    private Color activeColor =
        new Color(0f, 0.87f, 0.65f);

    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        if (button != null)
            button.onClick.AddListener(OnClick);
    }

    private void Start()
    {
        localization = LocalizationManager.Instance;

        if (localization != null)
        {
            // Reagujemy na zmianę języka!
            localization.OnLanguageChanged += UpdateText;
        }

        UpdateText();
        UpdateVisual();
    }

    private void OnEnable()
    {
        UpdateText();
        UpdateVisual();
    }

    private void OnDestroy()
    {
        if (localization != null)
            localization.OnLanguageChanged -= UpdateText;

        if (button != null)
            button.onClick.RemoveListener(OnClick);
    }

    public void Setup(
        ComponentCategory newCategory,
        ComponentListUI targetList,
        bool all = false)
    {
        category = newCategory;
        listUI = targetList;
        showAll = all;

        UpdateText();
        UpdateVisual();
    }

    private void UpdateText()
    {
        if (buttonText == null ||
            LocalizationManager.Instance == null)
            return;

        var loc = LocalizationManager.Instance;

        string key;

        if (showAll)
        {
            key = "cat_all";
        }
        else
        {
            key = "cat_" +
                category.ToString().ToLowerInvariant();
        }

        buttonText.text = loc.Get(key);
    }

    private void OnClick()
    {
        if (listUI == null)
            return;

        if (showAll)
            listUI.ShowAll();
        else
            listUI.ShowCategory(category);

        foreach (var btn in FindObjectsOfType<CategoryButton>())
        {
            btn.UpdateVisual();
        }
    }

    public void UpdateVisual()
    {
        if (listUI == null)
            return;

        bool isActive = showAll
            ? listUI.IsShowingAll()
            : listUI.IsShowingCategory(category);

        if (image != null)
            image.color = isActive
                ? activeColor
                : normalColor;
    }
}
