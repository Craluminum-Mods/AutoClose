using Vintagestory.API.Common;

namespace AutoClose.Configuration
{
  static class ModConfig
  {
    private const string jsonConfig = "AutoClose.json";
    private static AutoCloseConfig config;

    public static void ReadConfig(ICoreAPI api)
    {
      try
      {
        config = LoadConfig(api);

        if (config == null)
        {
          GenerateConfig(api);
          config = LoadConfig(api);
        }
        else
        {
          GenerateConfig(api, config);
        }
      }
      catch
      {
        GenerateConfig(api);
        config = LoadConfig(api);
      }

      api.World.Config.SetInt("AutoClose_Door_DelayMs", config.Delays["Vanilla"]["Door"]);
      api.World.Config.SetInt("AutoClose_FenceGate_DelayMs", config.Delays["Vanilla"]["Fencegate"]);
      api.World.Config.SetInt("AutoClose_Trapdoor_DelayMs", config.Delays["Vanilla"]["Trapdoor"]);
      api.World.Config.SetInt("AutoClose_SlidingDoor_DelayMs", config.Delays["OtherMods"]["SlidingDoor"]);
      api.World.Config.SetInt("AutoClose_Drawbridge_DelayMs", config.Delays["MedievalExpansion"]["Drawbridge"]);
      api.World.Config.SetInt("AutoClose_Gate_DelayMs", config.Delays["MedievalExpansion"]["Gate"]);
      api.World.Config.SetInt("AutoClose_Portcullis_DelayMs", config.Delays["MedievalExpansion"]["Portcullis"]);
    }

    private static AutoCloseConfig LoadConfig(ICoreAPI api) =>
      api.LoadModConfig<AutoCloseConfig>(jsonConfig);

    private static void GenerateConfig(ICoreAPI api) =>
      api.StoreModConfig(new AutoCloseConfig(), jsonConfig);

    private static void GenerateConfig(ICoreAPI api, AutoCloseConfig previousConfig) =>
      api.StoreModConfig(new AutoCloseConfig(previousConfig), jsonConfig);
  }
}