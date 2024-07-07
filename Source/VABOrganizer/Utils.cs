using UnityEngine;

namespace VABOrganizer
{
  public static class Utils
  {
    public static void Log(string str)
    {
      if (Settings.DebugMode)
      {
        Debug.Log("[VABOrganizer]" + str);
      }
    }
    public static void LogError(string str)
    {
      Debug.LogError("[VABOrganizer]" + str);
    }
    public static void LogWarning(string str)
    {
      Debug.LogWarning("[VABOrganizer]" + str);
    }
  }
}