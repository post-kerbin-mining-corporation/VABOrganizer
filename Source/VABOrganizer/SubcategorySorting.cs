using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KSP.UI.Screens;
using KSP.Localization;


namespace VABOrganizer
{
  public class SubcategorySorting 
  {
    public static List<Subcategory> Subcategories = new List<Subcategory>();

    /// <summary>
    /// Load the config defined subcategories
    /// </summary>
    public static void Load()
    {
      Subcategories = new List<Subcategory>();
      ConfigNode[] subcategoryNodes = GameDatabase.Instance.GetConfigNodes(Settings.ORGANIZER_SUBCATEGORY_NODE_NAME);
      Utils.Log($"[Subcategory Sorting]: Loading subcategory definitions");
      if (subcategoryNodes.Length > 0)
      {
        foreach (ConfigNode subcatNode in subcategoryNodes)
        {
          Subcategory subcat = new Subcategory(subcatNode);
          Subcategories.Add(subcat);
        }
        Subcategories.Sort((x, y) => x.priority.CompareTo(y.priority));
        Utils.Log($"[Subcategory Sorting]: Loaded {Subcategories.Count} subcategory definitions");
      }
      Subcategory miscCat = new Subcategory();
      miscCat.name = "misc";
      miscCat.label = Localizer.Format("#LOC_VABOrganizer_Subcategory_misc");
      Subcategories.Add(miscCat);
    }

    /// <summary>
    /// Assign an icon to a subcategory
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="availablePart"></param>
    public static void AssignIconToCategory(EditorPartIcon icon, AvailablePart availablePart)
    {
      bool isAssigned = false;
      foreach (Subcategory sub in Subcategories)
      {
        if (sub.TryAssignPart(icon, availablePart))
          isAssigned = true;
      }
      if (!isAssigned)
      {
        Subcategories[Subcategories.Count-1].AssignPart(icon);
      }
      
    }
    /// <summary>
    /// Clear the subcategory part sets when needed
    /// </summary>
    public static void Refresh()
    {
      Debug.Log("[Subcategories] Cleared all parts");
      foreach (Subcategory sub in Subcategories)
      {
        sub.ClearParts();
      }
    }

    public static void ForceLayoutRebuild(EditorPartList list)
    {
      LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)(list.partGrid.parent));
    }
    /// <summary>
    /// Build the subcategory UI by instantiating various new UI grids
    /// </summary>
    public static void CreateVABSubcategoryUI()
    {
      EditorPartList editorList = GameObject.FindObjectOfType<EditorPartList>();
      RectTransform partListRect = (RectTransform)editorList.partGrid.parent;
      GridLayoutGroup templateGrid = editorList.partGrid.GetComponent<GridLayoutGroup>();

      Utils.Log($"[Subcategories] Setting up new canvas structure");

      GameObject containerObj = GameObject.Instantiate(templateGrid.gameObject);
      GameObject.DestroyImmediate(containerObj.GetComponent<GridLayoutGroup>());
      
      containerObj.transform.SetParent(partListRect.transform, false);
      containerObj.name = $"PartGrid_Base";

      VerticalLayoutGroup vlg = containerObj.AddComponent<VerticalLayoutGroup>();
      partListRect = containerObj.GetComponent<RectTransform>();

      vlg.childControlWidth = vlg.childForceExpandHeight = vlg.childForceExpandWidth = true;
      vlg.childControlHeight = vlg.childScaleHeight = vlg.childScaleWidth = false;
      partListRect.anchorMin =  new Vector2(0, 0f);
      partListRect.anchorMax = new Vector2(1, 1f);
      partListRect.pivot = new Vector2(0, 1f);

      foreach (Subcategory sub in Subcategories)
      {
        sub.Build(partListRect, templateGrid);
      }
      templateGrid.transform.SetAsLastSibling();
      UIScrollRectState scroll = partListRect.parent.GetComponent<UIScrollRectState>();
      foreach(UIScrollRectState.PanelState state in scroll.panelList)
      {
        if (state.name == "parts")
        {
          state.rectTransform = containerObj.GetComponent<RectTransform>();
        }
      }
    }
  }
  
}
