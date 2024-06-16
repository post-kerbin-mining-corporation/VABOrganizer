using HarmonyLib;
using UnityEngine;

namespace VABOrganizer.HarmonyPatches
{
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public class VABOrganizerHarmonyPatcher : MonoBehaviour
  {
    public void Start()
    {
      Utils.Log("Starting Harmony patching...");
      var harmony = new Harmony("VABOrganizer");
      harmony.PatchAll();
      Utils.Log("Harmony patching complete");
    }
  }
}