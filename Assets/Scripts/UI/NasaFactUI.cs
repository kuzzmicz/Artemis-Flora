
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NasaFactUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject factPanel;

    public Button openButton;
    public Button closeButton;

    public TextMeshProUGUI factTitle;
    public TextMeshProUGUI factDescription;
    public TextMeshProUGUI factImportance;

    private void Awake()
    {
        if (openButton != null)
            openButton.onClick.AddListener(OpenFact);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseFact);

        if (factPanel != null)
            factPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (openButton != null)
            openButton.onClick.RemoveListener(OpenFact);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseFact);
    }

    public void SetFact(
        string title,
        string description,
        string importance)
    {
        if (factTitle != null)
            factTitle.text = title;

        if (factDescription != null)
            factDescription.text = description;

        if (factImportance != null)
            factImportance.text = importance;

        CloseFact();
    }

    public void OpenFact()
    {
        if (factPanel != null)
        {
            factPanel.SetActive(true);
            factPanel.transform.SetAsLastSibling();
        }
    }

    public void CloseFact()
    {
        if (factPanel != null)
            factPanel.SetActive(false);
    }
}
