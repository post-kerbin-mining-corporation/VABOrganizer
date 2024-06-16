using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace VABOrganizer
{
  public static class Utils
  {

    public static void Log(string str)
    {
      Debug.Log("[VABOrganizer]" + str);
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