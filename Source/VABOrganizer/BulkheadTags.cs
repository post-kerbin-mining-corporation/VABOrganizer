using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KSP.UI.Screens;

namespace VABOrganizer
{
  /// <summary>
  /// Manages the tags on part icons for bulkhead sizes
  /// </summary>
  public static class BulkheadTags
  {
    /// <summary>
    /// Creates the colored and labeled bulkhead tag on the part icon
    /// </summary>
    /// <param name="icon"></param>
    public static void CreatePartTag(EditorPartIcon icon)
    {
      AvailablePart part = icon.AvailPart;

      Image swatch = new GameObject("TagSwatch").AddComponent<Image>();
      swatch.sprite = VABOrganizerAssets.Sprites["organizer-carat"];
      swatch.type = Image.Type.Sliced;
      swatch.gameObject.SetLayerRecursive(LayerMask.NameToLayer("UI"));
      swatch.color = GetColor(part);
      swatch.transform.SetParent(icon.gameObject.transform, false);
      ContentSizeFitter csf = swatch.gameObject.AddComponent<ContentSizeFitter>();
      VerticalLayoutGroup vlg = swatch.gameObject.AddComponent<VerticalLayoutGroup>();
      vlg.childControlWidth  = true;
      vlg.childForceExpandHeight = vlg.childForceExpandWidth = vlg.childScaleHeight = vlg.childScaleWidth = vlg.childControlHeight = false;
      csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
      RectTransform rect = swatch.GetComponent<RectTransform>();

      rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.zero;
      rect.offsetMin = new Vector2(0, 52);
      rect.offsetMax = new Vector2(27, 66);

      TextMeshProUGUI textObj = new GameObject("Tag").AddComponent<TextMeshProUGUI>();
      
      textObj.text = GetText(part);
      textObj.fontSize = Settings.LabelFontSize;
      textObj.margin = new Vector4(4, 2, 3, 0);
      textObj.overflowMode = TextOverflowModes.Truncate;
      textObj.gameObject.SetLayerRecursive(LayerMask.NameToLayer("UI"));
      textObj.transform.SetParent(rect.transform, false);
      textObj.enableWordWrapping = false;

      rect = textObj.GetComponent<RectTransform>();
      rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.zero;
      rect.offsetMin = new Vector2(3, 48);
      rect.offsetMax = new Vector2(16, 64);
    }

    /// <summary>
    /// Get the text string for the tag
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    static string GetText(AvailablePart part)
    {
      return $"{part.MaxSizeBulkheadData().Label}";
    }

    /// <summary>
    /// Get the color for the tag
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    static Color GetColor(AvailablePart part)
    {
      Color c = part.MaxSizeBulkheadData().Color;
      c.a = Settings.LabelAlpha;
      return c;
    }
  }
}
