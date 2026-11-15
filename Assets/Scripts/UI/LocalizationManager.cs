using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    public enum Language { Polish, English }

    [Header("Settings")]
    public Language currentLanguage = Language.Polish;

    public event Action OnLanguageChanged;

    private readonly Dictionary<string, string> texts = new Dictionary<string, string>();

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
        OnLanguageChanged?.Invoke();
    }

    public void ToggleLanguage()
    {
        SetLanguage(currentLanguage == Language.Polish
            ? Language.English
            : Language.Polish);
    }

    public string Get(string key)
    {
        return texts.TryGetValue(key, out string value)
            ? value
            : $"[{key}]";
    }

    public string GetEventText(EventType type, string field, string fallback = "")
    {
        string key = "event_" + type.ToString().ToLowerInvariant() + "_" + field;
        return texts.TryGetValue(key, out string value) ? value : fallback;
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
        texts[key + "_desc"] = description;
        texts[key + "_a"] = choiceA;
        texts[key + "_b"] = choiceB;
        texts[key + "_c"] = choiceC;
    }

    private void LoadLanguage()
    {
        texts.Clear();

        if (currentLanguage == Language.Polish)
        {
            // Główne UI
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
            texts["info_growing_system"] = "Aby wystartować, wybierz system uprawy, lądownik oraz zapewnij dodatnią moc.";
            texts["negative_power"] = "Ujemna moc: stacja nie będzie samowystarczalna.";

            // Kategorie
            texts["cat_growingsystem"] = "Uprawa";
            texts["cat_power"] = "Energia";
            texts["cat_structure"] = "Struktura";
            texts["cat_communication"] = "Komunikacja";
            texts["cat_science"] = "Nauka";
            texts["cat_all"] = "Wszystkie";

            // Komponenty
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

            // Ciekawostki
            texts["fact_rocket_equation"] = "Ponad 85% masy startowej rakiety to samo paliwo! Dodanie zaledwie 1 kg ładunku naukowego wymaga wykładniczego zwiększenia ilości paliwa, aby uciec ze studni grawitacyjnej Ziemi.";
            texts["fact_light_speed_delay"] = "Fale radiowe pędzą z prędkością światła, lecz sygnał na Marsa leci od 4 do 24 minut w jedną stronę. Niemożliwe jest sterowanie w czasie rzeczywistym, dlatego sondy muszą podejmować decyzje autonomicznie.";
            texts["fact_gravity_assist"] = "Sonda może drastycznie zwiększyć prędkość bez zużywania cennego paliwa, przelatując blisko planety i przejmując znikomą część jej pędu orbitalnego wokół Słońca.";
            texts["fact_solar_inverse_square"] = "Natężenie światła słonecznego maleje z kwadratem odległości od Słońca. W rejonie Jowisza wynosi zaledwie około 3,7% wartości ziemskiej, wymuszając wybór gigantycznych paneli lub baterii jądrowych.";
            texts["fact_thermal_vacuum"] = "W kosmosie strona oświetlona Słońcem nagrzewa się do ponad +120°C, a zacieniona wychładza do -150°C. Inżynierowie stosują wielowarstwową izolację (złotą folię) i radiatory, chroniąc instrumenty przed zniszczeniem.";
            texts["fact_cubesat_revolution"] = "Zbudowane z modułów 10x10x10 cm satelity udowodniły, że tanie platformy mogą latać w głęboki kosmos - jak bliźniacze sondy MarCO, które transmitowały dane telemetryczne podczas lądowania misji InSight na Marsie w 2018 roku.";
            texts["fact_radiation_hardening"] = "Promieniowanie kosmiczne wywołuje przekłamania bitów. Komputery pokładowe wykorzystują wzmocnione radiacyjnie układy o taktowaniu poniżej 200 MHz, przedkładając niezawodność nad czystą wydajność obliczeniową.";
            texts["fact_seven_minutes_of_terror"] = "Wejście w atmosferę z prędkością ponad 20 000 km/h generuje tarcie o temperaturze ponad 2000°C. Przez opóźnienia i blokadę łączności radiowej proces wejścia, opadania i lądowania (EDL) musi być całkowicie zautomatyzowany.";

            // Feedback
            texts["fb_powershortage_0"] = "Ograniczono oświetlenie roślin. Pobór energii spadł, ale uprawy rosną wolniej.";
            texts["fb_powershortage_1"] = "Wyłączono część aparatury naukowej. Energia jest dostępna dla systemów krytycznych, lecz utracono dane.";
            texts["fb_powershortage_2"] = "Włączono zasilanie z akumulatorów. Rezerwa energii i margines bezpieczeństwa zmalały.";
            texts["fb_radiationspike_0"] = "Uruchomiono tryb ochronny. Ograniczono zagrożenie kosztem części badań.";
            texts["fb_radiationspike_1"] = "Kontynuowano badania promieniowania. Uzyskano dane, ale uprawy i urządzenia ucierpiały.";
            texts["fb_radiationspike_2"] = "Przeniesiono rośliny do lepiej osłoniętej sekcji.";
            texts["fb_plantdisease_0"] = "Odizolowano chore rośliny. Ograniczono ryzyko rozprzestrzenienia się infekcji.";
            texts["fb_plantdisease_1"] = "Zastosowano kontrolowane leczenie zagrożonej sekcji.";
            texts["fb_plantdisease_2"] = "Pobrano próbki do badań. Choroba spowodowała dalsze straty upraw.";
            texts["fb_fire_0"] = "Rozpoczęto ręczne gaszenie pożaru.";
            texts["fb_fire_1"] = "Odcięto zasilanie zagrożonej sekcji, ograniczając działanie szklarni.";
            texts["fb_fire_2"] = "Odizolowano zagrożony moduł, aby chronić pozostałą część instalacji.";
            texts["fb_fire_success"] = "Sukces! Pożar został ugaszony, a uszkodzenia ograniczone.";
            texts["fb_fire_fail"] = "Nie udało się ugasić pożaru. Ogień uszkodził część systemów.";
            texts["fb_pumpfailure_0"] = "Przełączono na pompę zapasową, wykorzystując rezerwę systemu.";
            texts["fb_pumpfailure_1"] = "Uruchomiono ograniczone nawadnianie awaryjne. Rośliny otrzymują mniej wody.";
            texts["fb_pumpfailure_2"] = "Rozpoczęto ręczną naprawę pompy.";
            texts["fb_pump_success"] = "Sukces! Pompa została naprawiona i obieg wody działa ponownie.";
            texts["fb_pump_fail"] = "Naprawa nieudana. Przerwa w nawadnianiu spowodowała straty.";
            texts["fb_waterproblem_0"] = "Przełączono na rezerwowy obieg wody. Przywrócono większość przepływu.";
            texts["fb_waterproblem_1"] = "Rozdzielono ograniczony dopływ wody pomiędzy sekcje. Rośliny rosną wolniej.";
            texts["fb_waterproblem_2"] = "Pozostawiono obieg pod obserwacją. Niedobór wody pogłębił się.";
            texts["fb_thermalstress_0"] = "Zwiększono chłodzenie. Temperatura spadła kosztem dodatkowej energii.";
            texts["fb_thermalstress_1"] = "Zebrano dane o stresie cieplnym, ale rośliny ucierpiały.";
            texts["fb_thermalstress_2"] = "Ograniczono pracę LED. Temperatura i pobór energii spadły.";
            texts["fb_goodgrowth_0"] = "Zwiększono intensywność uprawy i uzyskano większy przyrost roślin.";
            texts["fb_goodgrowth_1"] = "Wykonano dodatkowe pomiary i zebrano dane naukowe.";
            texts["fb_goodgrowth_2"] = "Utrzymano stabilne warunki, ograniczając ryzyko dla upraw.";
            texts["fb_communicationglitch_0"] = "Przełączono stację na tryb autonomiczny.";
            texts["fb_communicationglitch_1"] = "Zwiększono moc nadajnika kosztem energii.";
            texts["fb_communicationglitch_2"] = "Wstrzymano część eksperymentów do czasu poprawy łączności.";
            texts["fb_filterclog_0"] = "Uruchomiono czyszczenie filtra, zużywając dodatkową energię.";
            texts["fb_filterclog_1"] = "Ograniczono przepływ wody. Rośliny odczuwają stres.";
            texts["fb_filterclog_2"] = "Kontynuowano pracę mimo zatkanego filtra. Ryzyko awarii wzrosło.";
            texts["fb_dataoverflow_0"] = "Nadano priorytet danym dotyczącym roślin.";
            texts["fb_dataoverflow_1"] = "Wyłączono część instrumentów naukowych, ograniczając napływ danych.";
            texts["fb_dataoverflow_2"] = "Zwiększono moc obliczeniową kosztem energii.";
            texts["fb_microorganismwater_0"] = "Zignorowano skażenie. Mikroorganizmy rozprzestrzeniły się w układzie uprawy.";
            texts["fb_microorganismwater_1"] = "Odizolowano skażony obieg. Rośliny ucierpiały z powodu ograniczonego nawadniania.";
            texts["fb_microorganismwater_2"] = "Rozpoczęto ręczne przekierowanie obiegu wody.";
            texts["fb_microorganismwater_success"] = "Sukces! Ominięto skażoną sekcję i przywrócono bezpieczny przepływ.";
            texts["fb_microorganismwater_fail"] = "Porażka! Skażenie objęło kolejną sekcję lub obieg został rozszczelniony.";
            texts["fb_mutation_0"] = "Pozostawiono zmienione rośliny do obserwacji. Zebrano cenne dane.";
            texts["fb_mutation_1"] = "Usunięto zmienione rośliny i zabezpieczono próbki.";
            texts["fb_mutation_2"] = "Przeniesiono rośliny do osłoniętej sekcji, ograniczając dalszą ekspozycję.";
            texts["fb_sensorfailure_0"] = "Włączono bezpieczny tryb awaryjny. Wzrost roślin nieco spowolnił.";
            texts["fb_sensorfailure_1"] = "Przeprowadzono diagnostykę i kalibrację czujników.";
            texts["fb_sensorfailure_2"] = "Kontynuowano pracę na starych odczytach. Ryzyko błędnego sterowania wzrosło.";
            texts["fb_duststorm_0"] = "Uruchomiono czyszczenie paneli z osadzonego pyłu księżycowego.";
            texts["fb_duststorm_1"] = "Ograniczono zużycie energii, kompensując spadek wydajności paneli.";
            texts["fb_duststorm_2"] = "Kontynuowano obserwację paneli. Warstwa pyłu nadal ogranicza ich wydajność.";
            texts["fb_default"] = "Podjęto decyzję operacyjną.";

            // Mission Results
            texts["result_reason_success"] = "Misja zakończona sukcesem (14/14 dni).";
            texts["result_reason_power"] = "Porażka: wyczerpano energię (0 W).";
            texts["result_reason_growth"] = "Porażka: obumarcie uprawy (0% wzrostu).";
            texts["result_reason_unknown"] = "Misja zakończona.";
            texts["result_score"] = "WYNIK MISJI";
            texts["result_growth"] = "WZROST ROŚLIN";
            texts["result_science"] = "DANE NAUKOWE";
            texts["result_days"] = "dni";

            LoadPolishEvents();
        }
        else
        {
            // Main UI
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
            texts["info_growing_system"] = "To start, select a growing system, a lander, and ensure positive power.";
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

            // Event choice feedback
            texts["fb_powershortage_0"] = "Plant lighting reduced. Power demand dropped, but crop growth slowed.";
            texts["fb_powershortage_1"] = "Some scientific instruments were shut down. Essential systems retained power, but data was lost.";
            texts["fb_powershortage_2"] = "Backup batteries activated. Energy reserves and safety margins decreased.";
            texts["fb_radiationspike_0"] = "Protective mode activated. Radiation risk decreased at the cost of some research.";
            texts["fb_radiationspike_1"] = "Radiation studies continued. Valuable data was collected, but crops and equipment suffered.";
            texts["fb_radiationspike_2"] = "Plants moved to a better-shielded section.";
            texts["fb_plantdisease_0"] = "Infected plants isolated. The risk of spreading the disease was reduced.";
            texts["fb_plantdisease_1"] = "Targeted treatment applied to the affected section.";
            texts["fb_plantdisease_2"] = "Samples collected for research. The disease caused additional crop losses.";
            texts["fb_fire_0"] = "Manual fire suppression initiated.";
            texts["fb_fire_1"] = "Power cut to the affected section, restricting greenhouse operations.";
            texts["fb_fire_2"] = "The affected module was isolated to protect the rest of the station.";
            texts["fb_fire_success"] = "Success! The fire was extinguished and damage was limited.";
            texts["fb_fire_fail"] = "Failed to extinguish the fire. Some systems were damaged.";
            texts["fb_pumpfailure_0"] = "Switched to the backup pump, using the system's redundancy reserve.";
            texts["fb_pumpfailure_1"] = "Limited emergency irrigation activated. Plants are receiving less water.";
            texts["fb_pumpfailure_2"] = "Manual pump repair initiated.";
            texts["fb_pump_success"] = "Success! The pump has been repaired and water flow restored.";
            texts["fb_pump_fail"] = "Repair failed. The irrigation outage damaged the crops.";
            texts["fb_waterproblem_0"] = "Switched to the backup water circuit, restoring most of the flow.";
            texts["fb_waterproblem_1"] = "Limited water supply redistributed between sections. Plant growth slowed.";
            texts["fb_waterproblem_2"] = "The circuit was kept under observation. The water shortage worsened.";
            texts["fb_thermalstress_0"] = "Cooling increased. Temperature dropped at the cost of additional power.";
            texts["fb_thermalstress_1"] = "Heat stress data was collected, but the plants suffered.";
            texts["fb_thermalstress_2"] = "LED activity reduced, lowering temperature and power demand.";
            texts["fb_goodgrowth_0"] = "Cultivation intensity increased, boosting plant growth.";
            texts["fb_goodgrowth_1"] = "Additional measurements completed and scientific data collected.";
            texts["fb_goodgrowth_2"] = "Stable growing conditions maintained, reducing crop risks.";
            texts["fb_communicationglitch_0"] = "The station switched to autonomous mode.";
            texts["fb_communicationglitch_1"] = "Transmitter output increased at the cost of power.";
            texts["fb_communicationglitch_2"] = "Some experiments paused until communication improves.";
            texts["fb_filterclog_0"] = "Filter cleaning activated, consuming additional power.";
            texts["fb_filterclog_1"] = "Water flow reduced. Plants are under stress.";
            texts["fb_filterclog_2"] = "Operation continued with a clogged filter. Failure risk increased.";
            texts["fb_dataoverflow_0"] = "Plant research data prioritized.";
            texts["fb_dataoverflow_1"] = "Some scientific instruments shut down to reduce incoming data.";
            texts["fb_dataoverflow_2"] = "Processing power increased at the cost of energy.";
            texts["fb_microorganismwater_0"] = "Contamination ignored. Microorganisms spread through the growing system.";
            texts["fb_microorganismwater_1"] = "The contaminated circuit was isolated. Reduced irrigation damaged the crops.";
            texts["fb_microorganismwater_2"] = "Manual rerouting of the water circuit initiated.";
            texts["fb_microorganismwater_success"] = "Success! The contaminated section was bypassed and safe flow restored.";
            texts["fb_microorganismwater_fail"] = "Failure! Contamination spread to another section or the circuit developed a leak.";
            texts["fb_mutation_0"] = "Affected plants kept under observation. Valuable data was collected.";
            texts["fb_mutation_1"] = "Affected plants removed and samples preserved.";
            texts["fb_mutation_2"] = "Plants moved to a shielded section to reduce further exposure.";
            texts["fb_sensorfailure_0"] = "Safe emergency mode activated. Plant growth slowed slightly.";
            texts["fb_sensorfailure_1"] = "Sensor diagnostics and calibration completed.";
            texts["fb_sensorfailure_2"] = "Operation continued using old readings. Control errors became more likely.";
            texts["fb_duststorm_0"] = "Lunar dust cleaning initiated on the solar panels.";
            texts["fb_duststorm_1"] = "Power consumption reduced to compensate for lower panel output.";
            texts["fb_duststorm_2"] = "Solar panels kept under observation. Dust continued to reduce efficiency.";
            texts["fb_default"] = "Operational decision made.";

            // Mission Results
            texts["result_reason_success"] = "Mission completed successfully (14/14 days).";
            texts["result_reason_power"] = "Failure: power depleted (0 W).";
            texts["result_reason_growth"] = "Failure: crop died (0% growth).";
            texts["result_reason_unknown"] = "Mission ended.";
            texts["result_score"] = "MISSION SCORE";
            texts["result_growth"] = "PLANT GROWTH";
            texts["result_science"] = "SCIENCE DATA";
            texts["result_days"] = "days";

            LoadEnglishEvents();
        }
    }

  
    private void LoadPolishEvents()
    {
        AddEventTexts(EventType.PowerShortage,
            "Krytyczny spadek mocy",
            "System energetyczny wykrył przeciążenie głównej magistrali zasilania. Jednoczesna praca oświetlenia LED, pomp i aparatury naukowej przekroczyła dostępny limit mocy.",
            "Ogranicz oświetlenie roślin",
            "Wyłącz aparaturę naukową",
            "Uruchom rezerwę akumulatorów");

        AddEventTexts(EventType.PumpFailure,
            "Awaria pompy hydroponicznej",
            "Główna pompa hydroponiczna przestała działać. Korzenie roślin nie otrzymują odpowiedniej ilości wody i składników mineralnych. Konieczna jest szybka reakcja.",
            "Przełącz na pompę zapasową",
            "Uruchom ograniczone nawadnianie awaryjne",
            "Spróbuj naprawić pompę ręcznie");

        AddEventTexts(EventType.Fire,
            "Pożar w module",
            "Czujniki wykryły pożar w module szklarniowym. Ogień zagraża instalacjom elektrycznym, wentylacji i uprawom. Musisz natychmiast zareagować.",
            "Ugaś pożar ręcznie",
            "Odetnij zasilanie zagrożonej sekcji",
            "Odizoluj zagrożony moduł");

        AddEventTexts(EventType.PlantDisease,
            "Choroba roślin",
            "W części upraw wykryto przebarwienia liści, zahamowanie wzrostu i oznaki infekcji grzybiczej. Choroba może rozprzestrzenić się na pozostałe rośliny.",
            "Odizoluj chore rośliny",
            "Zastosuj kontrolowane leczenie",
            "Obserwuj i pobieraj próbki");

        AddEventTexts(EventType.RadiationSpike,
            "Alarm radiacyjny",
            "Czujniki wykryły gwałtowny wzrost poziomu promieniowania związany z aktywnością słoneczną. Zagrożone są rośliny oraz wrażliwa elektronika.",
            "Uruchom tryb ochronny",
            "Kontynuuj badania promieniowania",
            "Przenieś rośliny do osłoniętej sekcji");

        AddEventTexts(EventType.WaterProblem,
            "Spadek ciśnienia w systemie nawadniania",
            "W jednym z przewodów hydroponicznych wykryto spadek ciśnienia. Pompa nadal działa, ale część roślin otrzymuje zbyt mało roztworu odżywczego.",
            "Przełącz na rezerwowy obieg wody",
            "Ogranicz i rozdziel nawadnianie",
            "Kontynuuj pracę i monitoruj ciśnienie");

        AddEventTexts(EventType.ThermalStress,
            "Stres termiczny",
            "Temperatura wewnątrz szklarni wzrosła do niebezpiecznego poziomu. Oświetlenie LED i obciążenie układu chłodzenia powodują przegrzewanie upraw.",
            "Zwiększ moc systemu chłodzenia",
            "Obserwuj reakcję roślin na stres cieplny",
            "Ogranicz pracę oświetlenia LED");

        AddEventTexts(EventType.DustStorm,
            "Osadzanie się pyłu księżycowego",
            "Pył księżycowy wzniecony podczas wcześniejszych operacji powierzchniowych osiadł na panelach słonecznych i elementach instalacji. Wydajność zasilania może spaść.",
            "Uruchom procedurę czyszczenia paneli",
            "Ogranicz zużycie energii",
            "Monitoruj wydajność paneli");

        AddEventTexts(EventType.Mutation,
            "Nietypowe zmiany roślin",
            "Część roślin wykazuje nietypowe zmiany w budowie liści i tempie wzrostu. Przyczyną może być stres środowiskowy lub uszkodzenia komórkowe związane z promieniowaniem.",
            "Pozostaw rośliny do badań",
            "Usuń zmienione rośliny i zabezpiecz próbki",
            "Przenieś rośliny do osłoniętej sekcji");

        AddEventTexts(EventType.GoodGrowth,
            "Niespodziewanie dobry wzrost",
            "Rośliny rozwijają się szybciej, niż przewidywały prognozy. Korzystne warunki możesz wykorzystać do zwiększenia plonów albo zebrania dodatkowych danych naukowych.",
            "Zwiększ intensywność uprawy",
            "Przeprowadź dodatkowe pomiary",
            "Utrzymaj stabilne warunki");

        AddEventTexts(EventType.SensorFailure,
            "Awaria czujników środowiskowych",
            "System utracił sygnał z części czujników temperatury, wilgotności i stężenia CO₂. Automatyczne sterowanie szklarnią nie może dokładnie określić warunków uprawy.",
            "Przejdź na bezpieczny tryb awaryjny",
            "Uruchom diagnostykę i kalibrację",
            "Kontynuuj na podstawie ostatnich odczytów");

        AddEventTexts(EventType.CommunicationGlitch,
            "Zakłócenia komunikacji",
            "Stacja utraciła stabilną łączność z centrum kontroli misji. Pakiety danych docierają z opóźnieniem, a część poleceń może nie zostać odebrana.",
            "Przełącz na tryb autonomiczny",
            "Zwiększ moc nadajnika",
            "Wstrzymaj część eksperymentów");

        AddEventTexts(EventType.FilterClog,
            "Zatkany filtr obiegu wody",
            "System wykrył wzrost oporu przepływu w filtrze hydroponicznym. Zanieczyszczenia ograniczają dopływ roztworu odżywczego do roślin.",
            "Uruchom automatyczne czyszczenie filtra",
            "Ogranicz przepływ wody",
            "Kontynuuj pracę i monitoruj filtr");

        AddEventTexts(EventType.DataOverflow,
            "Przepełnienie pamięci danych",
            "System badawczy gromadzi dane szybciej, niż może je przetwarzać i przesyłać. Pamięć pokładowa zbliża się do limitu, a część wyników może zostać utracona.",
            "Nadaj priorytet danym o roślinach",
            "Wyłącz część instrumentów naukowych",
            "Zwiększ moc przetwarzania danych");

        AddEventTexts(EventType.MicroorganismWater,
            "Skażenie mikrobiologiczne wody",
            "W obiegu hydroponicznym wykryto niepożądane mikroorganizmy. Istnieje ryzyko powstawania biofilmu, zatykania filtrów i pogorszenia kondycji korzeni.",
            "Zignoruj ostrzeżenie",
            "Odizoluj skażony obieg",
            "Ręcznie przekieruj obieg wody");
    }

    private void LoadEnglishEvents()
    {
        AddEventTexts(EventType.PowerShortage,
            "Critical Power Shortage",
            "The power management system has detected an overload in the main power bus. LED lighting, pumps, and scientific equipment are drawing more power than the system can safely supply.",
            "Reduce plant lighting",
            "Shut down scientific instruments",
            "Activate backup batteries");

        AddEventTexts(EventType.PumpFailure,
            "Hydroponic Pump Failure",
            "The main hydroponic pump has stopped working. Plant roots are no longer receiving sufficient water and nutrients. Immediate action is required.",
            "Switch to the backup pump",
            "Activate limited emergency irrigation",
            "Attempt manual pump repair");

        AddEventTexts(EventType.Fire,
            "Fire in the Module",
            "Sensors have detected a fire inside the greenhouse module. Electrical systems, ventilation, and crops are at risk. Immediate action is required.",
            "Extinguish the fire manually",
            "Cut power to the affected section",
            "Isolate the affected module");

        AddEventTexts(EventType.PlantDisease,
            "Plant Disease",
            "Some plants show leaf discoloration, reduced growth, and signs of fungal infection. The disease may spread to the remaining crops.",
            "Isolate infected plants",
            "Apply targeted treatment",
            "Observe and collect samples");

        AddEventTexts(EventType.RadiationSpike,
            "Radiation Alert",
            "Sensors have detected a sudden increase in radiation associated with solar activity. Crops and sensitive electronics may be at risk.",
            "Activate protective mode",
            "Continue radiation research",
            "Move plants to a shielded compartment");

        AddEventTexts(EventType.WaterProblem,
            "Irrigation Pressure Drop",
            "Pressure has dropped in one of the hydroponic pipelines. The pump is still running, but some plants are receiving insufficient nutrient solution.",
            "Switch to the backup water circuit",
            "Reduce and redistribute irrigation",
            "Continue operating and monitor pressure");

        AddEventTexts(EventType.ThermalStress,
            "Thermal Stress",
            "The greenhouse temperature has reached dangerous levels. LED lighting and an overloaded cooling system are causing the growing chamber to overheat.",
            "Increase cooling system output",
            "Observe plant responses to heat stress",
            "Reduce LED lighting activity");

        AddEventTexts(EventType.DustStorm,
            "Lunar Dust Accumulation",
            "Lunar dust disturbed during earlier surface operations has settled on solar panels and exposed equipment. Power generation efficiency may decrease.",
            "Activate the panel cleaning procedure",
            "Reduce power consumption",
            "Monitor solar panel performance");

        AddEventTexts(EventType.Mutation,
            "Unusual Plant Changes",
            "Some plants show unusual leaf structures and growth patterns. Possible causes include environmental stress or radiation-related cellular damage.",
            "Keep the plants for research",
            "Remove affected plants and preserve samples",
            "Move plants to a shielded compartment");

        AddEventTexts(EventType.GoodGrowth,
            "Unexpectedly Strong Growth",
            "The plants are growing faster than predicted. These favorable conditions could be used to increase crop yield or collect additional scientific data.",
            "Increase cultivation intensity",
            "Perform additional measurements",
            "Maintain stable conditions");

        AddEventTexts(EventType.SensorFailure,
            "Environmental Sensor Failure",
            "The system has lost signals from several temperature, humidity, and CO₂ sensors. Automated greenhouse control can no longer accurately assess growing conditions.",
            "Switch to safe emergency mode",
            "Run diagnostics and calibration",
            "Continue using the last recorded readings");

        AddEventTexts(EventType.CommunicationGlitch,
            "Communication Disruption",
            "The station has lost stable communication with mission control. Data packets are delayed, and some commands may not reach the station.",
            "Switch to autonomous mode",
            "Increase transmitter power",
            "Pause some experiments");

        AddEventTexts(EventType.FilterClog,
            "Clogged Water Filter",
            "The hydroponic system has detected increased flow resistance in a filter. Contaminants are restricting the delivery of nutrient solution to the plants.",
            "Activate automatic filter cleaning",
            "Reduce water flow",
            "Continue operating and monitor the filter");

        AddEventTexts(EventType.DataOverflow,
            "Data Storage Overflow",
            "Scientific instruments are generating data faster than the system can process or transmit it. Onboard storage is approaching its limit.",
            "Prioritize plant research data",
            "Disable some scientific instruments",
            "Increase data processing power");

        AddEventTexts(EventType.MicroorganismWater,
            "Microbial Water Contamination",
            "Unwanted microorganisms have been detected in the hydroponic water circuit. Biofilm formation, filter clogging, and damage to plant roots are possible.",
            "Ignore the warning",
            "Isolate the contaminated water circuit",
            "Manually reroute the water circuit");
    }
}
