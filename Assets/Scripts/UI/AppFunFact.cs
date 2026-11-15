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

    private void OnEnable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= RefreshCurrentFact;
            LocalizationManager.Instance.OnLanguageChanged += RefreshCurrentFact;
        }
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= RefreshCurrentFact;
        }
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

        LocalizationManager loc = LocalizationManager.EnsureInstance();
        if (loc != null)
        {
            loc.OnLanguageChanged -= RefreshCurrentFact;
            loc.OnLanguageChanged += RefreshCurrentFact;
        }

        currentIndex = (currentIndex + 1) % fun_fact_list.Length;
        RefreshCurrentFact();
    }

    private void RefreshCurrentFact()
    {
        if (currentIndex < 0 || outputText == null || !outputText.gameObject.activeSelf)
        {
            return;
        }

        string key = fun_fact_list[currentIndex];
        outputText.text = LocalizationManager.GetText(key, key);
    }
}