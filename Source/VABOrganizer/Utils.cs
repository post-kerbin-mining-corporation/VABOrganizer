using System.Collections.Generic;
using UnityEngine;
using System;

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

    /// <summary>
    /// Get a reference in a child of a type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static T FindChildOfType<T>(string name, Transform parent)
    {
      T result = default(T);
      try
      {
        result = parent.FindDeepChild(name).GetComponent<T>();
      }
      catch (NullReferenceException e)
      {
        Debug.LogError($"Couldn't find {name} in children of {parent.name}");
      }
      return result;
    }
  }
  public static class TransformDeepChildExtension
  {
    //Breadth-first search
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
      Queue<Transform> queue = new Queue<Transform>();
      queue.Enqueue(aParent);
      while (queue.Count > 0)
      {
        var c = queue.Dequeue();
        if (c.name == aName)
          return c;
        foreach (Transform t in c)
          queue.Enqueue(t);
      }
      return null;
    }
  }
}
