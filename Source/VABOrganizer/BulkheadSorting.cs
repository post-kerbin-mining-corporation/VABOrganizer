using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KSP.UI;
using KSP.UI.TooltipTypes;
using KSP.Localization;

namespace VABOrganizer
{
  /// <summary>
  /// Manages the sorting by bulkhead
  /// </summary>
  public static class BulkheadSorting
  {
    public static Dictionary<string, BulkheadData> BulkheadMap = new Dictionary<string, BulkheadData>();

    const string BULKHEAD_SORT_KEY = "#LOC_VABOrganizer_BulkheadSortTooltip";

    /// <summary>
    /// Load data from config
    /// </summary>
    public static void Load()
    {
      ConfigNode[] bulkheadNodes = GameDatabase.Instance.GetConfigNodes(Settings.ORGANIZER_BULKHEAD_NODE_NAME);
      Utils.Log($"[Bulkhead Sorting]: Loading bulkhead definitions");
      if (bulkheadNodes.Length > 0)
      {
        BulkheadMap = new Dictionary<string, BulkheadData>();
        foreach (ConfigNode bulkheadNode in bulkheadNodes)
        {
          BulkheadData data = new BulkheadData(bulkheadNode);
          if (!BulkheadMap.ContainsKey(data.Name))
          {
            BulkheadMap.Add(data.Name, data);
          }
          else
          {
            Utils.LogWarning($"[Bulkhead Sorting]: Multiple BULKHEADDEFINITIONS with the same name ({data.Name}) found, skipping others");
          }
        }
        Utils.Log($"[Bulkhead Sorting]: Loaded {BulkheadMap.Count} bulkhead definitions");
      }
    }

    /// <summary>
    /// Creates the sorting UI
    /// </summary>
    public static void CreateVABSortUI()
    {
      UIListSorter sorterBase = GameObject.FindObjectOfType<UIListSorter>();
      GameObject sortByNameButton = sorterBase.gameObject.GetChild("StateButtonName");
      if (!sorterBase.gameObject.GetChild("StateButtonProfile"))
      {
        GameObject sortByProfileButton = GameObject.Instantiate(sortByNameButton);
        sortByProfileButton.transform.SetParent(sorterBase.transform, false);

        Button sortingButton = sortByProfileButton.GetComponent<Button>();
        TooltipController_Text tooltip = sortByProfileButton.GetComponent<TooltipController_Text>();
        TextMeshProUGUI sortingText = sortByProfileButton.GetChild("Text").GetComponent<TextMeshProUGUI>();
        UIStateImage sortingStateImage = sortByProfileButton.GetChild("StateImageName").GetComponent<UIStateImage>();
        sortingButton.onClick.RemoveAllListeners();
        sortingButton.onClick = new Button.ButtonClickedEvent();
        sortingButton.onClick.AddListener(() => sorterBase.ClickButton(4));
        sortingText.text = "Profile";
        tooltip.textString = Localizer.Format(BULKHEAD_SORT_KEY);
        sortByProfileButton.name = "StateButtonProfile";
        sortingStateImage.name = "StateImageProfile";

        Array.Resize(ref sorterBase.sortingButtonStates, sorterBase.sortingButtonStates.Length + 1);
        sorterBase.sortingButtonStates[4] = sortingStateImage;
        sorterBase.startSortingMode = sortingStateImage;
        sorterBase.startSortingAsc = true;
      }
      else
      {
        /// maybe throw warning later
      }
      RestyleButtons(sorterBase.transform);
    }

    /// <summary>
    /// Rebuilds the sort button array to use icons instead of text and adds a bulkhead one
    /// </summary>
    /// <param name="sorterBase"></param>
    public static void RestyleButtons(Transform sorterBase)
    {
      List<Transform> targetButtons = new List<Transform>();
      foreach (Transform child in sorterBase)
      {
        targetButtons.Add(child);
      }
      foreach (Transform child in targetButtons)
      {
        RectTransform textObj = (RectTransform)child.gameObject.GetChild("Text").transform;
        RectTransform stateIconRect = (RectTransform)child.gameObject.GetChild("Text").transform;
        textObj.gameObject.SetActive(false);

        Image icon = new GameObject("Icon").AddComponent<Image>();
        RectTransform iconRect = icon.GetComponent<RectTransform>();
        
        iconRect.transform.SetParent(child, false);
        iconRect.anchorMin = iconRect.anchorMax = new Vector2 (0,1);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.offsetMin = new Vector2(10f, -18f);
        iconRect.offsetMax = new Vector2(31f, 3f);

        switch (child.name)
        {
          case "StateButtonSize":
            icon.sprite = VABOrganizerAssets.Sprites["organizer-size"];
            break;
          case "StateButtonMass":
            icon.sprite = VABOrganizerAssets.Sprites["organizer-mass"];
            break;
          case "StateButtonCost":
            icon.sprite = VABOrganizerAssets.Sprites["organizer-funds"];
            break;
          case "StateButtonName":
            icon.sprite = VABOrganizerAssets.Sprites["organizer-name"];
            break;
          case "StateButtonProfile":
            icon.sprite = VABOrganizerAssets.Sprites["organizer-profile"];
            break;
        }
      }
      HorizontalLayoutGroup hlg = sorterBase.gameObject.AddComponent<HorizontalLayoutGroup>();
      ContentSizeFitter fitter = sorterBase.gameObject.AddComponent<ContentSizeFitter>();
     
      hlg.childControlWidth = true;
      hlg.childForceExpandWidth = false;
      hlg.spacing = 45;
      fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
    }
  }
}
