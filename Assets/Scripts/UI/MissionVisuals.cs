using UnityEngine;
using UnityEngine.UI;

public class MissionVisuals : MonoBehaviour
{
    [Header("Icons")]
    public Image landerIcon;
    public Image plantIcon;
    public Image powerIcon;
    public Image antennaIcon;
    public Image extraIcon;

    [Header("Plant States")]
    public Sprite plantHealthy;
    public Sprite plantMedium;
    public Sprite plantBad;
    public Sprite plantDead;

    [Header("Default Sprites")]
    public Sprite defaultLander;
    public Sprite defaultPower;
    public Sprite defaultAntenna;
    public Sprite defaultExtra;

    private MissionManager missionManager;

    private void Start()
    {
        missionManager = MissionManager.Instance;

        if (missionManager != null)
        {
            missionManager.OnLoadoutChanged += UpdateVisuals;
        }

        UpdateVisuals();
    }

    private void OnDestroy()
    {
        if (missionManager != null)
        {
            missionManager.OnLoadoutChanged -= UpdateVisuals;
        }
    }

    private void Update()
    {
        UpdatePlantVisual();
    }

    public void UpdateVisuals()
    {
        if (MissionManager.Instance == null)
            return;

        var loadout = MissionManager.Instance.currentLoadout;

        if (loadout == null)
            return;

        bool hasLander = loadout.selectedComponents.Exists(c =>
            c != null &&
            c.componentType == ComponentType.Lander_Basic
        );

        SetIcon(landerIcon, hasLander, defaultLander);


        bool hasPower = loadout.selectedComponents.Exists(c =>
            c != null &&
            (
                c.componentType == ComponentType.SolarPanels_Small ||
                c.componentType == ComponentType.SolarPanels_Large ||
                c.componentType == ComponentType.Batteries ||
                c.componentType == ComponentType.RTG_Small
            )
        );

        SetIcon(powerIcon, hasPower, defaultPower);

        bool hasAntenna = loadout.selectedComponents.Exists(c =>
            c != null &&
            (
                c.componentType == ComponentType.Antenna_Basic ||
                c.componentType == ComponentType.Antenna_HighGain
            )
        );

        SetIcon(antennaIcon, hasAntenna, defaultAntenna);

        bool hasExtra = loadout.selectedComponents.Exists(c =>
            c != null &&
            (
                c.componentType == ComponentType.RadiationShield_Light ||
                c.componentType == ComponentType.RadiationShield_Heavy ||
                c.componentType == ComponentType.RedundancySystem
            )
        );

        SetIcon(extraIcon, hasExtra, defaultExtra);

        UpdatePlantVisual();
    }

    private void UpdatePlantVisual()
    {
        if (plantIcon == null ||
            EventManager.Instance == null)
            return;

        float growth = EventManager.Instance.currentGrowth;

        Sprite selectedSprite = null;
        Color selectedColor;

        if (growth >= 0.7f)
        {
            selectedSprite = plantHealthy;
            selectedColor = new Color(0.3f, 0.9f, 0.4f);
        }
        else if (growth >= 0.4f)
        {
            selectedSprite = plantMedium;
            selectedColor = new Color(0.9f, 0.85f, 0.3f);
        }
        else if (growth >= 0.15f)
        {
            selectedSprite = plantBad;
            selectedColor = new Color(0.9f, 0.5f, 0.2f);
        }
        else
        {
            selectedSprite = plantDead;
            selectedColor = new Color(0.6f, 0.15f, 0.15f);
        }

        if (selectedSprite != null)
        {
            plantIcon.sprite = selectedSprite;
            plantIcon.color = Color.white;
        }
        else
        {
            plantIcon.color = selectedColor;
        }
    }

    private void SetIcon(
        Image icon,
        bool active,
        Sprite sprite)
    {
        if (icon == null)
            return;

        icon.enabled = active;

        if (active)
        {
            if (sprite != null)
                icon.sprite = sprite;

            icon.color = Color.white;
        }
    }
}