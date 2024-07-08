namespace VABOrganizer
{

  /// <summary>
  /// Static class to hold settings and configuration
  /// </summary>
  public static class Settings
  {
    /// <summary>
    /// If true, Log messages will be printed to log
    /// </summary>
    public static bool DebugMode = true;
    /// <summary>
    /// Size of the font on tags
    /// </summary>
    public static float LabelFontSize = 9f;
    public static float LabelAlpha = 0.4f;

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
        settingsNode.TryGetValue("LabelAlpha", ref LabelAlpha);
      }
      else
      {
        Utils.Log("[Settings]: Couldn't find settings file, using defaults");
      }
      Utils.Log("[Settings]: Finished loading");
    }
  }
}
