using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace VABOrganizer
{

  public class UIAdvancedSorterWidget : MonoBehaviour
  {
    enum FilterTextState
    {
      None,
      Selected,
      Rollover
    }

    public bool PanelShown { get; private set; }

    Color normalColor = new Color(0.9f, 0.9f, 0.9f);
    Color rolloverColor = new Color(1f, 1f, 1f);
    Color selectedColor = new Color(0.99f, 0.80f, 0.33f);

    [SerializeField]
    GameObject filterTypeRolloutObject;
    [SerializeField]
    Transform filterTypeTransform;
    [SerializeField]
    GameObject filterTypeCarat;
    [SerializeField]
    Button moreButton;
    [SerializeField]
    GameObject buttonTemplateObject;

    List<Button> sortTypeButtons;
    List<Text> sortTypeText;
    List<AdvancedSortType> sorters;

    public void Awake()
    {

      sortTypeButtons = new List<Button>();
      sortTypeText = new List<Text>();

      moreButton.onClick.AddListener(delegate { OnClickButton(); });

      RectTransform moreRect = moreButton.GetComponent<RectTransform>();
      moreRect.anchoredPosition = new Vector2(10, 2);
    }

    public void AssignReferences()
    {
      filterTypeTransform = this.transform.FindDeepChild("TooltipBG");
      filterTypeRolloutObject = filterTypeTransform.gameObject;
      filterTypeCarat = this.transform.FindDeepChild("Carat").gameObject;

      moreButton = Utils.FindChildOfType<Button>("MoreButton", transform);


      buttonTemplateObject = Utils.FindChildOfType<Button>("CustomFilterName", transform).gameObject;

      buttonTemplateObject.SetActive(false);
      SetPanelShown(false);

    }
    public void SetupSorters(List<AdvancedSortType> currentSorters)
    {
      Utils.Log($"[UIAdvancedSorterWidget] Category has no sorters");
      sorters = currentSorters;
      if (sorters != null && sorters.Count > 0)
      {
        Utils.Log($"[UIAdvancedSorterWidget] Setting up buttons for {currentSorters.Count} sorters");
        ClearButtons();

        for (int i = 0; i < sorters.Count; i++)
        {
          GameObject newButtonObj = GameObject.Instantiate(buttonTemplateObject);
          newButtonObj.transform.SetParent(filterTypeTransform, false);
          newButtonObj.SetActive(true);

          Button newButton = newButtonObj.GetComponent<Button>();
          Text newButtonText = newButtonObj.GetComponent<Text>();

          int j = i;

          newButton.onClick.AddListener(() => this.OnClickSorterButton(j));
          newButtonText.text = sorters[i].Label;

          sortTypeButtons.Add(newButton);
          sortTypeText.Add(newButtonText);

          if (AdvancedSorting.CurrentAdvancedSort != null && AdvancedSorting.CurrentAdvancedSort.Name == sorters[i].Name)
          {
            SetTextState(newButtonText, FilterTextState.Selected);
          }
          else
          {
            SetTextState(newButtonText, FilterTextState.None);
          }
        }
      }
      else
      {
        SetPanelShown(false);
        ClearButtons();
      }
    }
    void ClearButtons()
    {
      if (sortTypeButtons != null && sortTypeButtons.Count > 0)
      {
        for (int i = 0; i < sortTypeButtons.Count; i++)
        {
          GameObject.Destroy(sortTypeButtons[i].gameObject);
        }
        sortTypeButtons.Clear();
        sortTypeText.Clear();
      }
    }
    public void OnClickButton()
    {
      SetPanelShown(!PanelShown);
    }
    public void SetPanelShown(bool state)
    {
      if (sorters == null || (sorters != null && sorters.Count == 0))
      {
        PanelShown = false;
      }
      else
      {
        PanelShown = state;
      }
      filterTypeCarat.SetActive(state);
      filterTypeRolloutObject.SetActive(state);
    }
    public void OnClickSorterButton(int sorterClicked)
    {
      AdvancedSorting.ChangeAdvancedSortMode(sorters[sorterClicked]);
      for (int i = 0; i < sortTypeText.Count; i++)
      {
        SetTextState(sortTypeText[i], FilterTextState.None);
      }
      SetTextState(sortTypeText[sorterClicked], FilterTextState.Selected);
    }

    void SetTextState(Text theText, FilterTextState state)
    {
      switch (state)
      {
        case FilterTextState.None:
          theText.color = normalColor;
          break;
        case FilterTextState.Selected:
          theText.color = selectedColor;
          break;
        case FilterTextState.Rollover:
          theText.color = rolloverColor;
          break;
      }
    }
  }
}
