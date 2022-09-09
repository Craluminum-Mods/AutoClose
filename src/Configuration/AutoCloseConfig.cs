using System.Collections.Generic;

namespace AutoClose.Configuration
{
  class AutoCloseConfig
  {
    public readonly string Comment = "Delay in milliseconds";

    public Dictionary<string, Dictionary<string, int>> Delays = new()
    {
      { "Vanilla", vanillaBlocks },
      { "MedievalExpansion", medievalExpansionBlocks },
      { "OtherMods", blocksFromOtherMods},
    };

    public static Dictionary<string, int> vanillaBlocks = new()
    {
      { "Door", 1_500 },
      { "Fencegate", 1_500 },
      { "Trapdoor", 2_500 },
    };

    public static Dictionary<string, int> medievalExpansionBlocks = new()
    {
      { "Drawbridge", 14_000 },
      { "Gate", 7_000 },
      { "Portcullis", 7_000 },
    };

    public static Dictionary<string, int> blocksFromOtherMods = new()
    {
      { "SlidingDoor", 1_500 }
    };

    public AutoCloseConfig() { }

    public AutoCloseConfig(AutoCloseConfig previousConfig)
    {
      Comment = previousConfig.Comment;
      Delays = previousConfig.Delays;
    }
  }
}