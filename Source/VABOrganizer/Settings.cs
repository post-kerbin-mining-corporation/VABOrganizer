namespace VABOrganizer
{

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
