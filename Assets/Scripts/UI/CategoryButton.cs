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

    private Color normalColor = new Color(1f, 1f, 1);
    private Color activeColor = new Color(0.2f, 0.45f, 0.7f);

    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        button.onClick.AddListener(OnClick);
    }

    private void Start()
    {
        UpdateText();
    }

    private void OnEnable()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        if (buttonText == null || LocalizationManager.Instance == null) return;

        if (showAll)
        {
            buttonText.text = LocalizationManager.Instance.Get("cat_all");
        }
        else
        {
            string key = "cat_" + category.ToString().ToLower();
            buttonText.text = LocalizationManager.Instance.Get(key);
        }
    }

    private void OnClick()
    {
        if (listUI == null) return;

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
        bool isActive = false;

        if (listUI != null)
        {
            if (showAll)
                isActive = listUI.IsShowingAll();
            else
                isActive = listUI.IsShowingCategory(category);
        }

        if (image != null)
            image.color = isActive ? activeColor : normalColor;
    }
}