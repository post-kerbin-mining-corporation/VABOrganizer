namespace VABOrganizer
{

  /// <summary>
  /// Static class to hold settings and configuration
  /// </summary>
  public static class Settings
  {
    
    // Emit UI debug messages
    public static bool DebugMode = true;
    public static float LabelFontSize = 9f;

    public static string ORGANIZER_PART_ASSIGNMENT_NAME = "VABORGANIZER";
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
        settingsNode.TryGetValue("DebugMode", ref DebugMode);
        settingsNode.TryGetValue("LabelFontSize", ref LabelFontSize);
      }
      else
      {
        Utils.Log("[Settings]: Couldn't find settings file, using defaults");
      }
      Utils.Log("[Settings]: Finished loading");
    }
  }
}
