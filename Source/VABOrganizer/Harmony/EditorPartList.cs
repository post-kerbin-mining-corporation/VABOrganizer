using System;
using System.Collections.Generic;
using KSP.UI.Screens;
using HarmonyLib;

namespace VABOrganizer.HarmonyPatches
{
  [HarmonyPatch(typeof(EditorPartList))]
  internal class PatchEditorPartList
  {
    // TODO: Reassess these one-shot diagnostics before publishing once the
    // remaining modded-install failures have been identified.
    private static readonly HashSet<string> MissingPartDataWarnings = new HashSet<string>();
    private static bool dataStoreUnavailableWarningLogged;

    /// <summary>
    /// Patch the sorter to add the Bulkhead sorter
    /// </summary>
    /// <param name="__instance"></param>
    /// <param name="button"></param>
    /// <param name="asc"></param>
    /// <returns></returns>
    [HarmonyPrefix]
    [HarmonyPatch("SortingCallback")]
    internal static bool PatchSorting(EditorPartList __instance, int button, bool asc)
    {
      var partSortProperty = __instance.GetType().GetField("currentPartSorting", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      var assemblySortProperty = __instance.GetType().GetField("currentSubassemblySorting", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      // Profile sort
      if (button == 4)
      {
        partSortProperty.SetValue(__instance, new RUIutils.FuncComparer<AvailablePart>((AvailablePart r1, AvailablePart r2) => RUIutils.SortAscDescPrimarySecondary(asc, r1.MaxSizeBulkheadData().Size.CompareTo(r2.MaxSizeBulkheadData().Size), r1.title.CompareTo(r2.title))));
        assemblySortProperty.SetValue(__instance, new RUIutils.FuncComparer<ShipTemplate>((ShipTemplate r1, ShipTemplate r2) => RUIutils.SortAscDescPrimarySecondary(asc, r1.shipSize.magnitude.CompareTo(r2.shipSize.magnitude), r1.partCount.CompareTo(r2.partCount), r1.shipName.CompareTo(r2.shipName))));
      }
      // Custom sort
      if (button == 5)
      {
        if (AdvancedSorting.CurrentAdvancedSort != null)
        {
          string sortKey = AdvancedSorting.CurrentAdvancedSort.Sorter;
          if (!ReferenceEquals(AdvancedSortingDataStore.Instance, null))
          {
            AdvancedSortingDataStore.Instance.LogSortCoverage(sortKey);
          }
          partSortProperty.SetValue(__instance,
            new RUIutils.FuncComparer<AvailablePart>((AvailablePart r1, AvailablePart r2) =>
            RUIutils.SortAscDescPrimarySecondary(asc, CompareAdvancedSortValues(r1, r2, sortKey), string.Compare(r1.title, r2.title, StringComparison.Ordinal))));
        }
      }
      return true;
    }

    private static int CompareAdvancedSortValues(AvailablePart first, AvailablePart second, string sortKey)
    {
      bool hasFirst = TryGetPartData(first, out AvailablePartData firstData);
      bool hasSecond = TryGetPartData(second, out AvailablePartData secondData);

      if (hasFirst && hasSecond)
      {
        return firstData.GetData(sortKey).CompareTo(secondData.GetData(sortKey));
      }

      return hasFirst == hasSecond ? 0 : hasFirst ? 1 : -1;
    }

    private static bool TryGetPartData(AvailablePart part, out AvailablePartData partData)
    {
      partData = null;
      AdvancedSortingDataStore store = AdvancedSortingDataStore.Instance;
      // Unity objects compare equal to null after destruction even while their
      // managed fields remain reachable. ReferenceEquals avoids misclassifying
      // that state as an unpopulated data store during diagnostics/fallback.
      if (ReferenceEquals(store, null) || store.PartData == null)
      {
        if (!dataStoreUnavailableWarningLogged)
        {
          dataStoreUnavailableWarningLogged = true;
          Utils.LogError("[Advanced Sorting]: Part data store is unavailable during comparison");
        }
        return false;
      }

      if (part == null || string.IsNullOrEmpty(part.name) || !store.PartData.TryGetValue(part.name, out partData))
      {
        string partName = part == null ? "<null>" : part.name ?? "<unnamed>";
        if (MissingPartDataWarnings.Add(partName))
        {
          Utils.LogWarning($"[Advanced Sorting]: No parsed sort data for editor part '{partName}'; using title ordering for comparisons involving it");
        }
        return false;
      }

      return true;
    }
    /// <summary>
    /// Patch the part icon update to assign icons to the right categories
    /// </summary>
    /// <param name="__instance"></param>
    /// <param name="newIcon"></param>
    /// <param name="availablePart"></param>
    /// <param name="customCategory"></param>
    [HarmonyPostfix]
    [HarmonyPatch("UpdatePartIcon")]
    internal static void PatchPartIconUpdate(EditorPartList __instance, EditorPartIcon newIcon, AvailablePart availablePart, bool customCategory = false)
    {
      if (!customCategory)
      {
        SubcategorySorting.AssignIconToCategory(newIcon, availablePart);
      }
    }
    /// <summary>
    /// Patch the part list refresh to also refresh subcategories
    /// </summary>
    /// <param name="__instance"></param>
    /// <returns></returns>
    [HarmonyPrefix]
    [HarmonyPatch("RefreshPartList")]
    internal static bool PatchRefreshPartList(EditorPartList __instance)
    {
      SubcategorySorting.Refresh();
      
      AdvancedSorting.Refresh();
      return true;
    }
    /// <summary>
    /// Patch the part list refresh to also refresh subcategories
    /// </summary>
    /// <param name="__instance"></param>
    /// <returns></returns>
    [HarmonyPrefix]
    [HarmonyPatch("RefreshSearchList")]
    internal static bool PatchRefreshSearchList(EditorPartList __instance)
    {
      SubcategorySorting.Refresh();
      AdvancedSorting.Refresh();
      return true;
    }

    /// <summary>
    /// Patch the icon update to force a layout rebuild because scrollrects and layouts suck
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPostfix]
    [HarmonyPatch("UpdatePartIcons")]
    internal static void PatchUpdatePartIcons(EditorPartList __instance)
    {
      SubcategorySorting.ForceLayoutRebuild(__instance);
    }

    /// <summary>
    /// Patch start to appropriately do the UI changes when the VAB starts
    /// </summary>
    /// <param name="__instance"></param>
    /// <returns></returns>
    [HarmonyPrefix]
    [HarmonyPatch("Start")]
    internal static bool PatchStart(EditorPartList __instance)
    {
      /// We're defaulting the bulkhead sorter to on, so we have to patch it here
      var partSortProperty = __instance.GetType().GetField("currentPartSorting", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      partSortProperty.SetValue(__instance, new RUIutils.FuncComparer<AvailablePart>((AvailablePart r1, AvailablePart r2) => RUIutils.SortAscDescPrimarySecondary(true, r1.MaxSizeBulkheadData().Size.CompareTo(r2.MaxSizeBulkheadData().Size), r1.title.CompareTo(r2.title))));
      SubcategorySorting.CreateVABSubcategoryUI();
      return true;
    }
  }
}
