using KSP.UI;
using HarmonyLib;

namespace VABOrganizer.HarmonyPatches
{
  /// <summary>
  /// Patch the sorter UI to generate the AI when everything else gets created
  /// </summary>
  [HarmonyPatch(typeof(UIListSorter))]
  internal class PatchUIListSorter
  {
    [HarmonyPrefix]
    [HarmonyPatch("Start")]
    internal static bool PatchStart(UIListSorter __instance)
    {
      BulkheadSorting.CreateVABSortUI();
      return true;
    }
  }
}
