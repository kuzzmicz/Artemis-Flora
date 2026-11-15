using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AppFunFact : MonoBehaviour
{
    public TMP_Text outputText;
    public float yOffsetBelowButton = 25f;

    private readonly string[] fun_fact_list =
    {
        "fact_rocket_equation",
        "fact_light_speed_delay",
        "fact_gravity_assist",
        "fact_solar_inverse_square",
        "fact_thermal_vacuum",
        "fact_cubesat_revolution",
        "fact_radiation_hardening",
        "fact_seven_minutes_of_terror"
    };

    private int currentIndex = -1;

    private void Awake()
    {
        GetComponent<Button>()?.onClick.AddListener(OnClick);
        EnsureOutputText();
    }

    private void EnsureOutputText()
    {
        Transform panel = FindMonitorPanel();
        if (panel == null)
        {
            return;
        }

        Transform existing = panel.Find("FunFactText");
        outputText = existing.GetComponent<TMP_Text>();
    }

    private Transform FindMonitorPanel()
    {
        Transform current = transform.parent;
        while (current != null)
        {
            if (current.name == "MonitorPanel")
            {
                return current;
            }
            current = current.parent;
        }

        GameObject monitor = GameObject.Find("MonitorPanel");
        if (monitor != null)
        {
            return monitor.transform;
        }

        return transform.parent != null ? transform.parent : transform;
    }

    public void OnClick()
    {
        if (outputText == null)
        {
            EnsureOutputText();
        }

        if (outputText == null)
        {
            return;
        }

        Transform upgradePanel = FindMonitorPanel()?.Find("UpgradePanel");
        if (upgradePanel != null)
        {
            upgradePanel.gameObject.SetActive(false);
        }

        outputText.gameObject.SetActive(true);

        if (LocalizationManager.Instance == null)
        {
            var existingManager = FindObjectOfType<LocalizationManager>();
            if (existingManager == null)
            {
                new GameObject("LocalizationManager").AddComponent<LocalizationManager>();
            }
        }

        currentIndex = (currentIndex + 1) % fun_fact_list.Length;
        string key = fun_fact_list[currentIndex];
        outputText.text = LocalizationManager.Instance != null
            ? LocalizationManager.Instance.Get(key)
            : key;
    }
}