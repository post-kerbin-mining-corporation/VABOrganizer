using KSP.UI;
using HarmonyLib;

namespace VABOrganizer.HarmonyPatches
{
  [HarmonyPatch(typeof(UIListSorter))]
  internal class PatchUIListSorter
  {
    /// <summary>
    /// Patch the sorter UI to replace it with icons
    /// </summary>
    /// <param name="__instance"></param>
    /// <returns></returns>
    [HarmonyPrefix]
    [HarmonyPatch("Start")]
    internal static bool PatchStart(UIListSorter __instance)
    {
      AdvancedSorting.CreateVABSortUI();
      return true;
    }
  }
}
