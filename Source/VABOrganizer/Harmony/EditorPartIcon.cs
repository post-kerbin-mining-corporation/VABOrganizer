using HarmonyLib;
using KSP.UI.Screens;
using System;

namespace VABOrganizer.HarmonyPatches
{
  [HarmonyPatch(typeof(EditorPartIcon))]
  public class PatchEditorPartIcon
  {

    /// <summary>
    /// public void Create(EditorPartList partList, AvailablePart part, StoredPart sPart, float iconSize, float iconOverScale, float iconOverSpin, Callback<EditorPartIcon> placePartCallback, bool btnPlacePartActive, bool skipVariants, PartVariant variant, bool useImageThumbnail, bool inInventory)
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPostfix]
    [HarmonyPatch("Create", new Type[] { typeof(EditorPartList), typeof(AvailablePart), typeof(StoredPart), typeof(float), typeof(float), typeof(float), typeof(Callback<EditorPartIcon>), typeof(bool), typeof(bool), typeof(PartVariant), typeof(bool), typeof(bool) })]
    static void PatchCreate(EditorPartIcon __instance)
    {
      if (!__instance.inInventory)
      {
        BulkheadTags.CreatePartTag(__instance);
      }
    }

  }

}