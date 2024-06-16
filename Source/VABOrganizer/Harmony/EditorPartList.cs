using KSP.UI.Screens;
using HarmonyLib;

namespace VABOrganizer.HarmonyPatches
{
  [HarmonyPatch(typeof(EditorPartList))]
  internal class PatchEditorPartList
  {
    [HarmonyPostfix]
    [HarmonyPatch("SortingCallback")]
    internal static void PatchSorting(EditorPartList __instance, int button, bool asc)
    {
      /// Add a new sort option for Button 4
      var partSortProperty = __instance.GetType().GetField("currentPartSorting", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      var assemblySortProperty = __instance.GetType().GetField("currentSubassemblySorting", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      if (button == 4)
      {
        partSortProperty.SetValue(__instance, new RUIutils.FuncComparer<AvailablePart>((AvailablePart r1, AvailablePart r2) => RUIutils.SortAscDescPrimarySecondary(asc, r1.MaxSizeBulkheadData().Size.CompareTo(r2.MaxSizeBulkheadData().Size), r1.title.CompareTo(r2.title))));
        assemblySortProperty.SetValue(__instance, new RUIutils.FuncComparer<ShipTemplate>((ShipTemplate r1, ShipTemplate r2) => RUIutils.SortAscDescPrimarySecondary(asc, r1.shipSize.magnitude.CompareTo(r2.shipSize.magnitude), r1.partCount.CompareTo(r2.partCount), r1.shipName.CompareTo(r2.shipName))));
      }
      __instance.Refresh();
    }

    [HarmonyPostfix]
    [HarmonyPatch("UpdatePartIcon")]
    internal static void PatchPartIconUpdate(EditorPartList __instance, EditorPartIcon newIcon, AvailablePart availablePart, bool customCategory = false)
    {
      if (!customCategory)
      {
        SubcategorySorting.AssignIconToCategory(newIcon, availablePart);
      }
    }
    [HarmonyPrefix]
    [HarmonyPatch("RefreshPartList")]
    internal static bool PatchRefreshPartList(EditorPartList __instance)
    {
      SubcategorySorting.Refresh();
      return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch("UpdatePartIcons")]
    internal static void PatchUpdatePartIcons(EditorPartList __instance)
    {
      SubcategorySorting.ForceLayoutRebuild(__instance);
      
    }

    [HarmonyPrefix]
    [HarmonyPatch("Start")]
    internal static bool PatchStart(EditorPartList __instance)
    {
      SubcategorySorting.CreateVABSubcategoryUI();
      return true;
    }
  }
}