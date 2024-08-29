using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UniLinq;
using TMPro;
using KSP.UI;
using KSP.UI.Screens;
using KSP.UI.TooltipTypes;
using KSP.Localization;

namespace VABOrganizer
{
  /// <summary>
  /// Creates and manages the advanced sorting stuff
  /// </summary>
  public static class AdvancedSorting
  {
    public static AdvancedSortType CurrentAdvancedSort = null;

    static UIAdvancedSorterWidget SortWidget;
    static UIListSorter uiSorterBase;
    static EditorPartList uiPartList;
    static string cachedCategorySort = "";

    const string BULKHEAD_SORT_KEY = "#LOC_VABOrganizer_BulkheadSortTooltip";
    const string ADVANCED_SORT_KEY = "#LOC_VABOrganizer_AdvancedSortTooltip";

    /// <summary>
    /// Creates the sorting UI
    /// </summary>
    public static void CreateVABSortUI()
    {
      uiSorterBase = GameObject.FindObjectOfType<UIListSorter>();
      uiPartList = GameObject.FindObjectOfType<EditorPartList>();
      GameObject sortByNameButton = uiSorterBase.gameObject.GetChild("StateButtonName");
      if (!uiSorterBase.gameObject.GetChild("StateButtonProfile"))
      {
        CreateSortButton(uiSorterBase, sortByNameButton, "Profile", 4, true, BULKHEAD_SORT_KEY);
        CreateSortButton(uiSorterBase, sortByNameButton, "Custom", 5, false, ADVANCED_SORT_KEY);

      }
      else
      {
        /// maybe throw warning later
      }
      RestyleButtons(uiSorterBase.transform);
      CreateAdvancedButton(uiSorterBase.transform);
    }

    /// <summary>
    /// Create a new sorting button
    /// </summary>
    /// <param name="sorter"></param>
    /// <param name="template"></param>
    /// <param name="sortName"></param>
    /// <param name="buttonIndex"></param>
    /// <param name="startingSorter"></param>
    public static void CreateSortButton(UIListSorter sorter, GameObject template, string sortName, int buttonIndex, bool startingSorter, string tooltipKey)
    {
      GameObject newSortButtonObj = GameObject.Instantiate(template);
      newSortButtonObj.transform.SetParent(sorter.transform, false);

      Button sortingButton = newSortButtonObj.GetComponent<Button>();
      TooltipController_Text tooltip = newSortButtonObj.GetComponent<TooltipController_Text>();
      TextMeshProUGUI sortingText = newSortButtonObj.GetChild("Text").GetComponent<TextMeshProUGUI>();
      UIStateImage sortingStateImage = newSortButtonObj.GetChild("StateImageName").GetComponent<UIStateImage>();

      sortingButton.onClick.RemoveAllListeners();
      sortingButton.onClick = new Button.ButtonClickedEvent();
      sortingButton.onClick.AddListener(() => sorter.ClickButton(buttonIndex));
      sortingText.text = sortName;
      tooltip.textString = Localizer.Format(tooltipKey);
      newSortButtonObj.name = $"StateButton{sortName}";
      sortingStateImage.name = $"StateImage{sortName}";

      Array.Resize(ref sorter.sortingButtonStates, sorter.sortingButtonStates.Length + 1);
      sorter.sortingButtonStates[sorter.sortingButtonStates.Length - 1] = sortingStateImage;

      if (startingSorter)
      {
        sorter.startSortingMode = sortingStateImage;
        sorter.startSortingAsc = true;
      }
    }

    /// <summary>
    /// Create the advanced sort button thing
    /// </summary>
    /// <param name="sorterBase"></param>
    public static void CreateAdvancedButton(Transform sorterBase)
    {
      GameObject advSortObject = GameObject.Instantiate(VABOrganizerAssets.AdvancedSortPrefab);
      advSortObject.transform.SetParent(sorterBase, false);
      SortWidget = advSortObject.GetComponent<UIAdvancedSorterWidget>();
    }

    /// <summary>
    /// Rebuilds the sort button array to use icons instead of text
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
        iconRect.anchorMin = iconRect.anchorMax = new Vector2(0, 1);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.offsetMin = new Vector2(10f, -18f);
        iconRect.offsetMax = new Vector2(31f, 3f);

        switch (child.name)
        {
          case "StateButtonSize":
            icon.sprite = VABOrganizerAssets.GetSprite("organizer-size");
            break;
          case "StateButtonMass":
            icon.sprite = VABOrganizerAssets.GetSprite("organizer-mass");
            break;
          case "StateButtonCost":
            icon.sprite = VABOrganizerAssets.GetSprite("organizer-funds");
            break;
          case "StateButtonName":
            icon.sprite = VABOrganizerAssets.GetSprite("organizer-name");
            break;
          case "StateButtonProfile":
            icon.sprite = VABOrganizerAssets.GetSprite("organizer-profile");
            break;
          case "StateButtonCustom":
            icon.sprite = VABOrganizerAssets.GetSprite("organizer-custom");
            break;
        }
      }
      HorizontalLayoutGroup hlg = sorterBase.gameObject.AddComponent<HorizontalLayoutGroup>();
      ContentSizeFitter fitter = sorterBase.gameObject.AddComponent<ContentSizeFitter>();

      hlg.childControlWidth = true;
      hlg.childForceExpandWidth = false;
      hlg.spacing = 35f;
      fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    /// <summary>
    /// Changes the current advanced sort mode
    /// </summary>
    /// <param name="newType"></param>
    public static void ChangeAdvancedSortMode(AdvancedSortType newType)
    {
      Utils.Log($"[Advanced Sorting]: Set sorting type to  {newType.Name} from {AdvancedSorting.CurrentAdvancedSort}");

      // Get the currently selected modes
      UIStateImage curMode = (UIStateImage)uiSorterBase.GetType().GetField("activeSortingMode", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(uiSorterBase);
      bool curAsc = (bool)uiSorterBase.GetType().GetField("activeSortingAsc", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(uiSorterBase);
      MethodInfo setSortingMethod = uiSorterBase.GetType().GetMethod("SetSortingMode", BindingFlags.NonPublic | BindingFlags.Instance);
      MethodInfo onSortMethod = uiPartList.GetType().GetMethod("SortingCallback", BindingFlags.NonPublic | BindingFlags.Instance);

      // Determine if the current mode is something other than Advanced, if it is we can cheat
      int i = 0;
      for (int num = uiSorterBase.sortingButtonStates.Length; i < num; i++)
      {
        if (curMode != uiSorterBase.sortingButtonStates[5])
        {
          Utils.Log($"[Advanced Sorting] In a non-custom mode, clicking button 5");
          // Just fake click the button
          uiSorterBase.ClickButton(5);
          CurrentAdvancedSort = newType;
          return;
        }
      }
      // If not we have to do more things
      // If the same, flip the order and keep on Advanced
      if (CurrentAdvancedSort == newType)
      {
        bool newAsc = !curAsc;
        Utils.Log($"[Advanced Sorting] In custom mode with same sort, flipping order");
        setSortingMethod.Invoke(uiSorterBase, new object[] { curMode, newAsc });
        onSortMethod.Invoke(uiPartList, new object[] { 5, newAsc });
      }
      else
      {
        Utils.Log($"[Advanced Sorting] In custom mode with different sort, keeping order");
        CurrentAdvancedSort = newType;
        setSortingMethod.Invoke(uiSorterBase, new object[] { curMode, curAsc });
        onSortMethod.Invoke(uiPartList, new object[] { 5, curAsc });
      }
    }

    /// <summary>
    /// Refresh the UI from a generic part list Refresh event
    /// </summary>
    public static void Refresh()
    {
      string currentCategorySort = uiPartList.CategorizerFilters[0].ID;
      Utils.Log($"[Advanced Sorting] Refreshed, new categoryFilter is {currentCategorySort}, from {cachedCategorySort}");

      if (cachedCategorySort != currentCategorySort)
      {
        List<AdvancedSortType> sorters = AdvancedSortingData.GetSortersForCategory(currentCategorySort);
        if (SortWidget != null)
        {
          SortWidget.SetPanelShown(false);
          SortWidget.SetupSorters(sorters);
        }
        // If the new category has no advanced sorters and the selected mode is custom, set the mode to default
        if (sorters == null || (sorters != null && sorters.Count == 0))
        {
          Utils.Log($"[Advanced Sorting] New category has no sorters, setting to default");
          UIStateImage curMode = (UIStateImage)uiSorterBase.GetType().GetField("activeSortingMode", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(uiSorterBase);
          if (curMode == uiSorterBase.sortingButtonStates[5])
          {
            uiSorterBase.ClickButton(4);
          }
        }
        // If the new category has sorters but they don't contain the current one, pick the first one
        if (sorters != null && sorters.Count > 0 && CurrentAdvancedSort != null)
        {
          UIStateImage curMode = (UIStateImage)uiSorterBase.GetType().GetField("activeSortingMode", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(uiSorterBase);
          if (curMode == uiSorterBase.sortingButtonStates[5])
          {
            if (sorters.FirstOrDefault(x => x.Sorter == CurrentAdvancedSort.Sorter) == null)
            {
              Utils.Log($"[Advanced Sorting] New category has no identical sorters, setting first custom as {sorters[0].Label}");
              ChangeAdvancedSortMode(sorters[0]);
            }
          }
        }
        cachedCategorySort = currentCategorySort;
      }
    }
  }

}
