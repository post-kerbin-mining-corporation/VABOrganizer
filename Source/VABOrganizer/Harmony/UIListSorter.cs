using KSP.UI;
using HarmonyLib;

namespace VABOrganizer.HarmonyPatches
{
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
