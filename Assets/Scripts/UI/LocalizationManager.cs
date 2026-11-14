using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    public enum Language { Polish, English }

    [Header("Settings")]
    public Language currentLanguage = Language.Polish;

    private Dictionary<string, string> texts = new Dictionary<string, string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadLanguage();
    }

    public void SetLanguage(Language lang)
    {
        currentLanguage = lang;
        LoadLanguage();
    }

    public void ToggleLanguage()
    {
        if (currentLanguage == Language.Polish)
            SetLanguage(Language.English);
        else
            SetLanguage(Language.Polish);
    }

    public string Get(string key)
    {
        if (texts.ContainsKey(key))
            return texts[key];

        return $"[{key}]";
    }

    private void LoadLanguage()
    {
        texts.Clear();

        if (currentLanguage == Language.Polish)
        {
            texts["start_mission"] = "URUCHOM MISJĘ";
            texts["clear_all"] = "Wyczyść wszystko";
            texts["select_components"] = "Wybierz komponenty";
            texts["parameters"] = "Parametry misji";
            texts["selected"] = "Wybrane komponenty";
            texts["mass"] = "Masa";
            texts["cost"] = "Koszt";
            texts["power"] = "Moc";
            texts["reliability"] = "Niezawodność";
            texts["science"] = "Dane naukowe";
            texts["growth"] = "Wzrost roślin";
            texts["no_components"] = "Brak wybranych komponentów";
            texts["info_growing_system"] = "Aby wystartować, wybierz system uprawy i lądownik.";
            texts["negative_power"] = "Ujemna moc: stacja nie będzie samowystarczalna.";

            // Categories
            texts["cat_growingsystem"] = "Uprawa";
            texts["cat_power"] = "Energia";
            texts["cat_structure"] = "Struktura";
            texts["cat_communication"] = "Komunikacja";
            texts["cat_science"] = "Nauka";
            texts["cat_all"] = "Wszystkie";

            // Components
            texts["comp_lander_basic"] = "Lądownik Podstawowy";
            texts["comp_hydroponics_basic"] = "Hydroponika Podstawowa";
            texts["comp_hydroponics_advanced"] = "Hydroponika Zaawansowana";
            texts["comp_led_lowpower"] = "Oświetlenie LED Niskomocowe";
            texts["comp_led_highpower"] = "Oświetlenie LED Wysokomocowe";
            texts["comp_atmospherecontrol"] = "Kontrola Atmosfery";
            texts["comp_solarpanels_small"] = "Panele Słoneczne Małe";
            texts["comp_solarpanels_large"] = "Panele Słoneczne Duże";
            texts["comp_batteries"] = "Baterie";
            texts["comp_rtg_small"] = "RTG Mały";
            texts["comp_radiationshield_light"] = "Osłona Radiacyjna Lekka";
            texts["comp_radiationshield_heavy"] = "Osłona Radiacyjna Ciężka";
            texts["comp_redundancysystem"] = "System Redundancji";
            texts["comp_antenna_basic"] = "Antena Podstawowa";
            texts["comp_antenna_highgain"] = "Antena Wysokiego Zysku";
            texts["comp_camera_basic"] = "Kamera Podstawowa";
            texts["comp_camera_hd"] = "Kamera HD";
            texts["comp_sensors_environment"] = "Czujniki Środowiskowe";
            texts["comp_samplesystem"] = "System Pobierania Próbek";

        }
        else
        {
            texts["start_mission"] = "START MISSION";
            texts["clear_all"] = "Clear All";
            texts["select_components"] = "Select Components";
            texts["parameters"] = "Mission Parameters";
            texts["selected"] = "Selected Components";
            texts["mass"] = "Mass";
            texts["cost"] = "Cost";
            texts["power"] = "Power";
            texts["reliability"] = "Reliability";
            texts["science"] = "Science Data";
            texts["growth"] = "Plant Growth";
            texts["no_components"] = "No components selected";
            texts["info_growing_system"] = "To start, select a growing system and a lander.";
            texts["negative_power"] = "Negative power: the station will not be self-sustainable.";

            // Categories
            texts["cat_growingsystem"] = "Growing";
            texts["cat_power"] = "Power";
            texts["cat_structure"] = "Structure";
            texts["cat_communication"] = "Communication";
            texts["cat_science"] = "Science";
            texts["cat_all"] = "All";

            // Components
            texts["comp_lander_basic"] = "Basic Lander";
            texts["comp_hydroponics_basic"] = "Basic Hydroponics";
            texts["comp_hydroponics_advanced"] = "Advanced Hydroponics";
            texts["comp_led_lowpower"] = "Low Power LED";
            texts["comp_led_highpower"] = "High Power LED";
            texts["comp_atmospherecontrol"] = "Atmosphere Control";
            texts["comp_solarpanels_small"] = "Small Solar Panels";
            texts["comp_solarpanels_large"] = "Large Solar Panels";
            texts["comp_batteries"] = "Batteries";
            texts["comp_rtg_small"] = "Small RTG";
            texts["comp_radiationshield_light"] = "Light Radiation Shield";
            texts["comp_radiationshield_heavy"] = "Heavy Radiation Shield";
            texts["comp_redundancysystem"] = "Redundancy System";
            texts["comp_antenna_basic"] = "Basic Antenna";
            texts["comp_antenna_highgain"] = "High Gain Antenna";
            texts["comp_camera_basic"] = "Basic Camera";
            texts["comp_camera_hd"] = "HD Camera";
            texts["comp_sensors_environment"] = "Environmental Sensors";
            texts["comp_samplesystem"] = "Sample Collection System";

        }
    }
}