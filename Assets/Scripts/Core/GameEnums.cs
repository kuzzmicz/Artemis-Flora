public enum ComponentType
{
    None = 0,

    Lander_Basic,
    RadiationShield_Light,
    RadiationShield_Heavy,
    RedundancySystem,

    Hydroponics_Basic,
    Hydroponics_Advanced,
    LED_LowPower,
    LED_HighPower,
    AtmosphereControl,

    SolarPanels_Small,
    SolarPanels_Large,
    Batteries,
    RTG_Small,

    Antenna_Basic,
    Antenna_HighGain,

    Camera_Basic,
    Camera_HD,
    Sensors_Environment,
    SampleSystem
}

public enum EventType
{
    None = 0,
    PowerShortage,
    PumpFailure,
    Fire,
    PlantDisease,
    RadiationSpike,
    WaterProblem,
    ThermalStress,
    DustStorm,
    Mutation,
    GoodGrowth,
    SensorFailure,
    CommunicationGlitch,
    FilterClog,
    DataOverflow,
    MicroorganismWater
}
}