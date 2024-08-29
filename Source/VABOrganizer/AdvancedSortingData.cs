using System;
using System.Collections.Generic;

namespace VABOrganizer
{
  /// <summary>
  ///  Manages the data load for bulkhead and custom sorting
  /// </summary>
  public static class AdvancedSortingData
  {
    public static Dictionary<string, BulkheadData> BulkheadMap = new Dictionary<string, BulkheadData>();
    public static Dictionary<string, List<AdvancedSortType>> SortMap = new Dictionary<string, List<AdvancedSortType>>();

    public static List<AdvancedSortType> GetSortersForCategory(string category)
    {
      if (SortMap.TryGetValue(category, out List<AdvancedSortType> sorters))
      {
        return sorters;
      }
      return null;
    }

    /// <summary>
    /// Load data from config
    /// </summary>
    public static void Load()
    {
      ConfigNode[] bulkheadNodes = GameDatabase.Instance.GetConfigNodes(Settings.ORGANIZER_BULKHEAD_NODE_NAME);
      Utils.Log($"[Advanced Sorting]: Loading bulkhead definitions");
      if (bulkheadNodes.Length > 0)
      {
        BulkheadMap = new Dictionary<string, BulkheadData>();
        foreach (ConfigNode bulkheadNode in bulkheadNodes)
        {
          BulkheadData data = new BulkheadData(bulkheadNode);
          if (!BulkheadMap.ContainsKey(data.Name))
          {
            BulkheadMap.Add(data.Name, data);
          }
          else
          {
            Utils.LogWarning($"[Advanced Sorting]: Multiple BULKHEADDEFINITIONS with the same name ({data.Name}) found, skipping others");
          }
        }
        Utils.Log($"[Advanced Sorting]: Loaded {BulkheadMap.Count} bulkhead definitions");
      }


      ConfigNode[] sortNodes = GameDatabase.Instance.GetConfigNodes(Settings.ORGANIZER_SORTER_NODE_NAME);
      Utils.Log($"[Advanced Sorting]: Loading sorter definitions");
      if (sortNodes.Length > 0)
      {

        List<AdvancedSortType> sortTypes = new List<AdvancedSortType>();
        foreach (ConfigNode sortNode in sortNodes)
        {
          AdvancedSortType data = new AdvancedSortType(sortNode);
          if (!sortTypes.Contains(data))
          {
            sortTypes.Add(data);
          }
          else
          {
            Utils.LogWarning($"[Advanced Sorting]: Multiple ORGANIZERSORTERTYPEs with the same name ({data.Name}) found, skipping others");
          }
        }
        Utils.Log($"[Advanced Sorting]: Loaded {sortTypes.Count} sorting definitions");

        Utils.Log($"[Advanced Sorting]: Parsing sorting definitions into map");
        if (sortTypes.Count > 0)
        {
          SortMap = new Dictionary<string, List<AdvancedSortType>>();

          for (int i = 0; i < sortTypes.Count; i++)
          {
            for (int j = 0; j < sortTypes[i].AssociatedPartCategories.Count; j++)
            {
              if (SortMap.ContainsKey(sortTypes[i].AssociatedPartCategories[j]))
              {
                SortMap[sortTypes[i].AssociatedPartCategories[j]].Add(sortTypes[i]);
              }
              else
              {
                SortMap.Add(sortTypes[i].AssociatedPartCategories[j], new List<AdvancedSortType> { sortTypes[i] });
              }
            }
          }
        }
      }
    }
  }
}