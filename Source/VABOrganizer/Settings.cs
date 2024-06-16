using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VABOrganizer
{
  public class BulkheadData
  {
    public string Name;
    public string Label;
    public float Size;
    public Color Color;

    public BulkheadData()
    {
      Name = "UNK";
      Label = "";
      Size = 1000;
      Color = new Color(0,0,0,0);
    }
    public BulkheadData(ConfigNode configNode)
    {
      Load(configNode);
    }
    public void Load(ConfigNode configNode)
    {
      configNode.TryGetValue("name", ref Name);
      configNode.TryGetValue("Label", ref Label); ;
      configNode.TryGetValue("Size", ref Size);
      configNode.TryGetValue("Color", ref Color);
    }
  }

  /// <summary>
  /// Static class to hold settings and configuration
  /// </summary>
  public static class Settings
  {
    
    // Emit UI debug messages
    public static bool DebugUI = true;

    public static string ORGANIZER_SETTINGS_NODE_NAME = "ORGANIZERSETTINGS";
    public static string ORGANIZER_BULKHEAD_NODE_NAME = "ORGANIZERBULKHEAD";
    public static string ORGANIZER_SUBCATEGORY_NODE_NAME = "ORGANIZERSUBCATEGORY";
    /// <summary>
    /// Load data from configuration
    /// </summary>
    public static void Load()
    {
      ConfigNode settingsNode;
      Utils.Log("[Settings]: Started loading");

      ConfigNode[] settingsNodes = GameDatabase.Instance.GetConfigNodes(ORGANIZER_SETTINGS_NODE_NAME);
      if (settingsNodes.Length > 0)
      {
        settingsNode = settingsNodes[0];
        settingsNode.TryGetValue("DebugUI", ref DebugUI);
      }
      else
      {
        Utils.Log("[Settings]: Couldn't find settings file, using defaults");
      }
      Utils.Log("[Settings]: Finished loading");
    }
  }
}
