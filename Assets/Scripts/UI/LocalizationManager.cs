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

    private void AddEventTexts(
        EventType type,
        string name,
        string description,
        string choiceA,
        string choiceB,
        string choiceC)
    {
        string key = "event_" + type.ToString().ToLowerInvariant();

        texts[key + "_name"] = name;
        texts[key + "_description"] = description;
        texts[key + "_a"] = choiceA;
        texts[key + "_b"] = choiceB;
        texts[key + "_c"] = choiceC;
    }

    public string GetEventText(
        EventType type,
        string field,
        string fallback = "")
    {
        string key = "event_" +
            type.ToString().ToLowerInvariant() +
            "_" + field;

        return texts.TryGetValue(key, out string value)
            ? value
            : fallback;
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

            // Fun facts
            texts["fact_rocket_equation"] = "Ponad 85% masy startowej rakiety to samo paliwo! Dodanie zaledwie 1 kg ładunku naukowego wymaga wykładniczego zwiększenia ilości paliwa, aby uciec ze studni grawitacyjnej Ziemi.";
            texts["fact_light_speed_delay"] = "Fale radiowe pędzą z prędkością światła, lecz sygnał na Marsa leci od 4 do 24 minut w jedną stronę. Niemożliwe jest sterowanie w czasie rzeczywistym, dlatego sondy muszą podejmować decyzje autonomicznie.";
            texts["fact_gravity_assist"] = "Sonda może drastycznie zwiększyć prędkość bez zużywania cennego paliwa, przelatując blisko planety i przejmując znikomą część jej pędu orbitalnego wokół Słońca.";
            texts["fact_solar_inverse_square"] = "Natężenie światła słonecznego maleje z kwadratem odległości od Słońca. W rejonie Jowisza wynosi zaledwie około 3,7% wartości ziemskiej, wymuszając wybór gigantycznych paneli lub baterii jądrowych.";
            texts["fact_thermal_vacuum"] = "W kosmosie strona oświetlona Słońcem nagrzewa się do ponad +120°C, a zacieniona wychładza do -150°C. Inżynierowie stosują wielowarstwową izolację (złotą folię) i radiatory, chroniąc instrumenty przed zniszczeniem.";
            texts["fact_cubesat_revolution"] = "Zbudowane z modułów 10x10x10 cm satelity udowodniły, że tanie platformy mogą latać w głęboki kosmos - jak bliźniacze sondy MarCO, które transmitowały dane telemetryczne podczas lądowania misji InSight na Marsie w 2018 roku.";
            texts["fact_radiation_hardening"] = "Promieniowanie kosmiczne wywołuje przekłamania bitów. Komputery pokładowe wykorzystują wzmocnione radiacyjnie układy o taktowaniu poniżej 200 MHz, przedkładając niezawodność nad czystą wydajność obliczeniową.";
            texts["fact_seven_minutes_of_terror"] = "Wejście w atmosferę z prędkością ponad 20 000 km/h generuje tarcie o temperaturze ponad 2000°C. Przez opóźnienia i blokadę łączności radiowej proces wejścia, opadania i lądowania (EDL) musi być całkowicie zautomatyzowany.";

            // Feedbacks
            texts["fb_powershortage_0"] = "Ograniczono oświetlenie. Rośliny rosną wolniej, ale system przetrwa.";
            texts["fb_powershortage_1"] = "Wyłączono instrumenty naukowe. Utracono część danych.";
            texts["fb_powershortage_2"] = "Zużyto rezerwę energii. Ryzyko całkowitego blackoutu wzrosło.";
            texts["fb_radiationspike_0"] = "Aktywowano maksymalną ochronę. Rośliny są bezpieczniejsze.";
            texts["fb_radiationspike_1"] = "Zbierano dane o promieniowaniu. Rośliny ucierpiały.";
            texts["fb_radiationspike_2"] = "Przeniesiono rośliny w bardziej chronioną strefę.";
            texts["fb_plantdisease_0"] = "Odizolowano chore rośliny. Zdrowe osobniki są bezpieczne.";
            texts["fb_plantdisease_1"] = "Zastosowano środki ochronne na całą uprawę.";
            texts["fb_plantdisease_2"] = "Zebrano dane o chorobie. Rośliny ucierpiały.";
            texts["fb_fire_0"] = "Aktywowano system gaśniczy. Zużyto dużo mocy.";
            texts["fb_fire_1"] = "Odłączono zasilanie zagrożonej sekcji.";
            texts["fb_fire_2"] = "Ewakuowano priorytetowe systemy. Wzrost roślin osłabł.";
            texts["fb_pumpfailure_0"] = "Przełączono na pompę zapasową. Zużyto dodatkową moc.";
            texts["fb_pumpfailure_1"] = "Uruchomiono tryb awaryjny. Podlewanie zostało ograniczone.";
            texts["fb_pumpfailure_2"] = "Próbujesz samodzielnie naprawić pompę.";
            texts["fb_waterproblem_0"] = "Uruchomiono system zapasowy. Zużyto dodatkową moc.";
            texts["fb_waterproblem_1"] = "Ograniczono podlewanie. Rośliny odczuwają stres.";
            texts["fb_waterproblem_2"] = "Kontynuowano bez interwencji. Ryzyko awarii wzrosło.";
            texts["fb_thermalstress_0"] = "Stabilizowano temperaturę kosztem energii.";
            texts["fb_thermalstress_1"] = "Zaakceptowano wahania. Zebrano dodatkowe dane naukowe.";
            texts["fb_thermalstress_2"] = "Ograniczono aktywność systemów uprawy.";
            texts["fb_goodgrowth_0"] = "Skupiono się na maksymalizacji plonu.";
            texts["fb_goodgrowth_1"] = "Priorytetem stały się dodatkowe pomiary naukowe.";
            texts["fb_goodgrowth_2"] = "Utrzymano stabilne, bezpieczne warunki.";
            texts["fb_communicationglitch_0"] = "Przełączono na tryb autonomiczny.";
            texts["fb_communicationglitch_1"] = "Wzmocniono sygnał kosztem energii.";
            texts["fb_communicationglitch_2"] = "Wstrzymano część eksperymentów.";
            texts["fb_filterclog_0"] = "Uruchomiono czyszczenie awaryjne. Zużyto dodatkową moc.";
            texts["fb_filterclog_1"] = "Ograniczono podlewanie. Rośliny odczuwają stres.";
            texts["fb_filterclog_2"] = "Kontynuowano bez interwencji. Ryzyko awarii wzrosło.";
            texts["fb_dataoverflow_0"] = "Priorytetem stały się dane o roślinach.";
            texts["fb_dataoverflow_1"] = "Wyłączono część instrumentów naukowych.";
            texts["fb_dataoverflow_2"] = "Zwiększono zużycie mocy na przetwarzanie danych.";
            texts["fb_microorganismwater_0"] = "Zignorowano skażenie. Mikroorganizm zainfekował system uprawy!";
            texts["fb_microorganismwater_1"] = "Odłączono dopływ wody. Uprawa uniknęła skażenia, lecz rośliny ucierpiały z braku nawadniania.";
            texts["fb_microorganismwater_2"] = "Rozpoczęto ręczną rekonfigurację rurociągu.";
            texts["fb_microorganismwater_success"] = "Sukces! Obieg wody został prawidłowo poprowadzony z ominięciem mikroorganizmu.";
            texts["fb_microorganismwater_fail"] = "Porażka! Mikroorganizm przedostał się do uprawy lub obieg został rozszczelniony.";
            texts["fb_default"] = "Podjęto decyzję operacyjną.";
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

            // Fun facts
            texts["fact_rocket_equation"] = "Over 85% of a rocket's launch mass is propellant! Adding just 1 kg of scientific payload requires an exponential amount of extra fuel to escape Earth's gravity well.";
            texts["fact_light_speed_delay"] = "Radio waves travel at light speed, but signals to Mars take 4 to 24 minutes one-way. Because real-time piloting is impossible, deep space probes must be engineered for autonomous fault protection.";
            texts["fact_gravity_assist"] = "Spacecraft can gain massive velocity without burning propellant by flying past a planet, stealing an imperceptible fraction of that planet's orbital momentum around the Sun.";
            texts["fact_solar_inverse_square"] = "Sunlight intensity drops with the square of distance from the Sun. At Jupiter, solar irradiance is only around 3.7% of Earth's, forcing designers to choose between massive solar wings or nuclear RTG generators.";
            texts["fact_thermal_vacuum"] = "In space, direct sunlight heats spacecraft surfaces to over +120°C, while shadowed sides plunge to -150°C. Engineers use reflective multi-layer insulation (gold foil) and heat radiators to avoid instrument failure.";
            texts["fact_cubesat_revolution"] = "Built from standard 10x10x10 cm units, CubeSats have proven that low-cost, rideshare platforms can venture into deep space, like NASA's twin MarCO probes that relayed telemetry during the 2018 InSight Mars landing.";
            texts["fact_radiation_hardening"] = "Galactic cosmic rays frequently cause bit flips. Space mission computers favor heavily shielded, radiation-hardened processors (often running below 200 MHz) rather than modern high-speed chips that risk sudden reboots.";
            texts["fact_seven_minutes_of_terror"] = "Entering a planetary atmosphere at over 20,000 km/h produces scorching friction over 2,000°C. With long radio communication blackouts and delays, the entire Entry, Descent, and Landing (EDL) process must be 100% automated.";

            // Feedbacks
            texts["fb_powershortage_0"] = "Lighting reduced. Plants grow slower, but the system will survive.";
            texts["fb_powershortage_1"] = "Science instruments disabled. Some data was lost.";
            texts["fb_powershortage_2"] = "Energy reserves used. Risk of total blackout increased.";
            texts["fb_radiationspike_0"] = "Maximum protection activated. Plants are safer.";
            texts["fb_radiationspike_1"] = "Radiation data collected. Plants suffered damage.";
            texts["fb_radiationspike_2"] = "Plants moved to a more protected area.";
            texts["fb_plantdisease_0"] = "Diseased plants isolated. Healthy ones are safe.";
            texts["fb_plantdisease_1"] = "Protective measures applied to the entire crop.";
            texts["fb_plantdisease_2"] = "Disease data collected. Plants suffered.";
            texts["fb_fire_0"] = "Fire suppression system activated. High power usage.";
            texts["fb_fire_1"] = "Power cut to the affected section.";
            texts["fb_fire_2"] = "Priority systems evacuated. Plant growth weakened.";
            texts["fb_fire_success"] = "The fire was quickly extinguished.";
            texts["fb_fire_fail"] = "Failed to fully extinguish the fire.";
            texts["fb_pump_success"] = "Repair successful. The pump is working.";
            texts["fb_pump_fail"] = "Repair failed. Plants suffered.";
            texts["fb_pumpfailure_0"] = "Switched to backup pump. Additional power used.";
            texts["fb_pumpfailure_1"] = "Emergency mode activated. Watering reduced.";
            texts["fb_pumpfailure_2"] = "Attempting manual pump repair.";
            texts["fb_waterproblem_0"] = "Backup system activated. Additional power used.";
            texts["fb_waterproblem_1"] = "Watering reduced. Plants are under stress.";
            texts["fb_waterproblem_2"] = "No intervention. Risk of failure increased.";
            texts["fb_thermalstress_0"] = "Temperature stabilized at the cost of energy.";
            texts["fb_thermalstress_1"] = "Fluctuations accepted. Additional science data collected.";
            texts["fb_thermalstress_2"] = "Growing system activity reduced.";
            texts["fb_goodgrowth_0"] = "Focused on maximizing yield.";
            texts["fb_goodgrowth_1"] = "Priority given to additional scientific measurements.";
            texts["fb_goodgrowth_2"] = "Stable and safe conditions maintained.";
            texts["fb_communicationglitch_0"] = "Switched to autonomous mode.";
            texts["fb_communicationglitch_1"] = "Signal boosted at the cost of energy.";
            texts["fb_communicationglitch_2"] = "Some experiments paused.";
            texts["fb_filterclog_0"] = "Emergency cleaning activated. Additional power used.";
            texts["fb_filterclog_1"] = "Watering reduced. Plants are under stress.";
            texts["fb_filterclog_2"] = "No intervention. Risk of failure increased.";
            texts["fb_dataoverflow_0"] = "Plant data prioritized.";
            texts["fb_dataoverflow_1"] = "Some science instruments disabled.";
            texts["fb_dataoverflow_2"] = "Increased power usage for data processing.";
            texts["fb_microorganismwater_0"] = "Contamination ignored. The microorganism infected the hydroponic system!";
            texts["fb_microorganismwater_1"] = "Water supply disconnected. Crop protected from infection, but plant growth suffered due to lack of water.";
            texts["fb_microorganismwater_2"] = "Manual reconfiguration of the water pipeline initiated.";
            texts["fb_microorganismwater_success"] = "Success! The water circuit was safely routed, bypassing the microorganism.";
            texts["fb_microorganismwater_fail"] = "Failure! The microorganism contaminated the crop or the circuit leaked.";
            texts["fb_default"] = "Operational decision made.";
        }
    }
}