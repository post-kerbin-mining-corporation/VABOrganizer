using HarmonyLib;
using UnityEngine;

namespace VABOrganizer.HarmonyPatches
{
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public class VABOrganizerHarmonyPatcher : MonoBehaviour
  {
    public void Start()
    {
      Utils.Log("[Harmony] Start Patching");
      var harmony = new Harmony("VABOrganizer");
      harmony.PatchAll();
      Utils.Log("[Harmony] Patching complete");
    }
  }
}