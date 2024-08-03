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

    internal static Vector2 swatchVABOffsetMin = new Vector2(0, 52);
    internal static Vector2 swatchVABOffsetMax = new Vector2(27, 66);
    internal static Vector2 swatchRDOffsetMin = new Vector2(0, 33);
    internal static Vector2 swatchRDOffsetMax = new Vector2(21, 50);

    internal static Vector2 textVABOffsetMin = new Vector2(3, 48);
    internal static Vector2 textVABOffsetMax = new Vector2(16, 64);
    internal static Vector2 textRDOffsetMin = new Vector2(3, 37);
    internal static Vector2 textRDOffsetMax = new Vector2(12, 50);

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
      vlg.childControlWidth = true;
      vlg.childForceExpandHeight = vlg.childForceExpandWidth = vlg.childScaleHeight = vlg.childScaleWidth = vlg.childControlHeight = false;
      csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

      RectTransform rect = swatch.GetComponent<RectTransform>();
      rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.zero;

      if (RDController.Instance != null)
      {
        rect.offsetMin = swatchRDOffsetMin;
        rect.offsetMax = swatchRDOffsetMax;
      }
      else
      {
        rect.offsetMin = swatchVABOffsetMin;
        rect.offsetMax = swatchVABOffsetMax;
      }

      TextMeshProUGUI textObj = new GameObject("Tag").AddComponent<TextMeshProUGUI>();
      textObj.text = GetText(part);

      textObj.margin = new Vector4(4, 1, 4, 0);
      textObj.font = UISkinManager.TMPFont;
      textObj.overflowMode = TextOverflowModes.Truncate;
      textObj.gameObject.SetLayerRecursive(LayerMask.NameToLayer("UI"));
      textObj.transform.SetParent(rect.transform, false);
      textObj.enableWordWrapping = false;
      

      RectTransform rectText = textObj.GetComponent<RectTransform>();
      rectText.anchorMin = rectText.anchorMax = rectText.pivot = Vector2.zero;

      if (RDController.Instance != null)
      {
        rectText.offsetMin = textRDOffsetMin;
        rectText.offsetMax = textRDOffsetMax;
        textObj.fontSize = Settings.RDLabelFontSize;
      }
      else
      {
        rectText.offsetMin = textVABOffsetMin;
        rectText.offsetMax = textVABOffsetMax;
        textObj.fontSize = Settings.LabelFontSize;
      }

      // If the bulkhead label ends up empty hide things
      if (textObj.text == "")
      {
        textObj.enabled = false;
        swatch.enabled = false;
      }

      // turn off for performance
      LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
      vlg.enabled = false;
      csf.enabled = false;
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
