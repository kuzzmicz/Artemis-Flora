using UnityEngine;

public class ComponentListUI : MonoBehaviour
{
    public Transform contentParent;
    public ComponentButton buttonPrefab;

    private ComponentCategory? currentCategory = null;
    private bool showingAll = false;

    private void Start()
    {
        ShowCategory(ComponentCategory.GrowingSystem);
    }

    public void ShowCategory(ComponentCategory category)
    {
        currentCategory = category;
        showingAll = false;
        GenerateButtons();
    }

    public void ShowAll()
    {
        currentCategory = null;
        showingAll = true;
        GenerateButtons();
    }

    public bool IsShowingCategory(ComponentCategory category)
    {
        return !showingAll && currentCategory == category;
    }

    public bool IsShowingAll()
    {
        return showingAll;
    }

    private void GenerateButtons()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        if (MissionManager.Instance == null || MissionManager.Instance.availableComponents == null)
            return;

        foreach (var comp in MissionManager.Instance.availableComponents)
        {
            if (comp == null) continue;

            if (!showingAll && currentCategory != null && comp.category != currentCategory)
                continue;

            ComponentButton newButton = Instantiate(buttonPrefab, contentParent);
            newButton.Setup(comp);
            newButton.RefreshText();
        }
    }

    public void RefreshAllButtons()
    {
        foreach (Transform child in contentParent)
        {
            ComponentButton btn = child.GetComponent<ComponentButton>();
            if (btn != null)
                btn.RefreshText();
        }
    }
}