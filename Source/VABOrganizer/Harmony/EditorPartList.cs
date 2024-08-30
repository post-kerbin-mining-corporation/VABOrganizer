using KSP.UI.Screens;
using HarmonyLib;

namespace VABOrganizer.HarmonyPatches
{
  [HarmonyPatch(typeof(EditorPartList))]
  internal class PatchEditorPartList
  {
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
          partSortProperty.SetValue(__instance,
            new RUIutils.FuncComparer<AvailablePart>((AvailablePart r1, AvailablePart r2) =>
            RUIutils.SortAscDescPrimarySecondary(asc, AdvancedSortingDataStore.Instance.PartData[r1.name].GetData(AdvancedSorting.CurrentAdvancedSort.Sorter).CompareTo(AdvancedSortingDataStore.Instance.PartData[r2.name].GetData(AdvancedSorting.CurrentAdvancedSort.Sorter)), r1.title.CompareTo(r2.title))));
        }
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