using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using KSP.UI;
using KSP.UI.Screens;
using TMPro;


namespace VABOrganizer
{

  public class SubcategorySorting 
  {
    public static SubcategorySorting Instance { get; private set; }

    public static List<Subcategory> Subcategories = new List<Subcategory>();


    public static void Load()
    {
      Subcategories = new List<Subcategory>();
      ConfigNode[] subcategoryNodes = GameDatabase.Instance.GetConfigNodes(Settings.ORGANIZER_SUBCATEGORY_NODE_NAME);
      Utils.Log($"[Subcategory Sorting]: Loading bulkhead definitions");
      if (subcategoryNodes.Length > 0)
      {
        foreach (ConfigNode subcatNode in subcategoryNodes)
        {
          Subcategory subcat = new Subcategory(subcatNode);
          Subcategories.Add(subcat);
        }
        Utils.Log($"[Subcategory Sorting]: Loaded {Subcategories.Count} subcategory definitions");
      }
      Subcategory miscCat = new Subcategory();
      miscCat.name = "misc";
      miscCat.label = "Other";
      Subcategories.Add(miscCat);
    }

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
    public static void Refresh()
    {
      foreach (Subcategory sub in Subcategories)
      {
        sub.ClearParts();
      }
    }
    public static void ForceLayoutRebuild(EditorPartList list)
    {
      LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)(list.partGrid.parent));
    }
    public static void CreateVABSubcategoryUI()
    {
      EditorPartList editorList = GameObject.FindObjectOfType<EditorPartList>();
      RectTransform partListRect = (RectTransform)editorList.partGrid.parent;
      GridLayoutGroup templateGrid = editorList.partGrid.GetComponent<GridLayoutGroup>();

      Utils.Log($"Setting up new canvas structure");

      GameObject containerObj = GameObject.Instantiate(templateGrid.gameObject);
      GameObject.DestroyImmediate(containerObj.GetComponent<GridLayoutGroup>());
      
      containerObj.transform.SetParent(partListRect.transform, false);
      containerObj.name = $"PartGrid_Base";

      VerticalLayoutGroup vlg = containerObj.AddComponent<VerticalLayoutGroup>();
      partListRect = containerObj.GetComponent<RectTransform>();

      vlg.childForceExpandHeight = vlg.childForceExpandWidth = true;
      vlg.childControlWidth = vlg.childControlHeight = vlg.childScaleHeight = vlg.childScaleWidth = false;
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
