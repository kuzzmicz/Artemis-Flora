using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StartMissionButton : MonoBehaviour
{
    private Button button;
    public TextMeshProUGUI buttonText;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnEnable()
    {
        UpdateButtonText();
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnLoadoutChanged += UpdateInteractableState;

        UpdateInteractableState();
    }

    private void OnDisable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnLoadoutChanged -= UpdateInteractableState;
    }

    private void Start()
    {
        UpdateButtonText();
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.OnLoadoutChanged -= UpdateInteractableState;
            MissionManager.Instance.OnLoadoutChanged += UpdateInteractableState;
        }
        UpdateInteractableState();
    }

    private void UpdateInteractableState()
    {
        if (MissionManager.Instance == null) return;

        button.interactable = MissionManager.Instance.CanStartMission();
    }

    private void UpdateButtonText()
    {
        if (buttonText != null && LocalizationManager.Instance != null)
        {
            buttonText.text = LocalizationManager.Instance.Get("start_mission");
        }
    }

    private void OnClick()
    {
        if (MissionManager.Instance == null || !MissionManager.Instance.CanStartMission())
            return;

        SceneManager.LoadScene("MissionStage");
    }
}