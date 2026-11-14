using UnityEngine;
using UnityEngine.UI;

public class MonitorToggle : MonoBehaviour
{
    public GameObject monitorPanel;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ToggleMonitor);
        }

        if (monitorPanel != null)
        {
            monitorPanel.SetActive(false);
        }
    }

    public void ToggleMonitor()
    {
        if (monitorPanel != null)
        {
            monitorPanel.SetActive(!monitorPanel.activeSelf);
        }
    }
}
