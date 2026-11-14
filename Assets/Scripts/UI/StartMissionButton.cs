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

        var loadout = MissionManager.Instance.currentLoadout;

        bool hasGrowingSystem = loadout.selectedComponents
            .Exists(c => c.category == ComponentCategory.GrowingSystem);


        bool hasLander = loadout.selectedComponents
      .Exists(c => c.componentType == ComponentType.Lander_Basic);

        button.interactable = hasGrowingSystem && hasLander;
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
        if (MissionManager.Instance == null) return;
        SceneManager.LoadScene("MissionStage");
    }
}