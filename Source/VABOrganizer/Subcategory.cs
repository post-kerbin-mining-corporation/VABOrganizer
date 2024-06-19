using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KSP.UI.Screens;
using KSP.Localization;


namespace VABOrganizer
{
  public class Subcategory
  {
    public string name;
    public string label;
    public float priority = 0f;
    protected GameObject gridObj;
    protected GameObject headerObj;

    protected Image headerIcon;
    protected Transform gridTransform;

    protected bool categoryVisible = false;
    protected bool categoryActive = true;
    protected bool categoryEmpty = true;

    protected List<string> validParts = new List<string>();
    protected List<string> validPaths = new List<string>();
    protected List<string> validRegex = new List<string>();

    public Subcategory() { }

    public Subcategory(ConfigNode node)
    {
      Load(node);
    }

    public void Load(ConfigNode node) 
    {
      validParts = new List<string>();
      validPaths = new List<string>();
      validRegex = new List<string>();
      node.TryGetValue("name", ref name);
      node.TryGetValue("Label", ref label);
      node.TryGetValue("Priority", ref priority);

      label = Localizer.Format(label);

      validParts = node.GetValues("part").ToList();
      validPaths = node.GetValues("path").ToList();
      validRegex = node.GetValues("regex").ToList();
    }

    /// <summary>
    /// Build the UI for this category
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="templatePartGrid"></param>
    public void Build(RectTransform parent, GridLayoutGroup templatePartGrid)
    {
      if (parent.gameObject.GetChild($"PartHeader_{name}") == null)
      {
        BuildHeader(parent);

        gridObj = GameObject.Instantiate(templatePartGrid.gameObject);
        gridObj.transform.SetParent(parent.transform, false);
        gridObj.name = $"PartGrid_{name}";
        gridTransform = gridObj.transform;
      }
      else
      {
        gridTransform = parent.gameObject.GetChild($"PartHeader_{name}").transform;
      }
    }

    /// <summary>
    /// Build the header for this category
    /// </summary>
    /// <param name="parent"></param>
    protected void BuildHeader(RectTransform parent)
    {
      Image headerBackground;
      TextMeshProUGUI headerText;
      Button headerButton;

      headerObj = new GameObject($"PartHeader_{name}");
      headerBackground = headerObj.AddComponent<Image>();
      headerIcon = new GameObject($"PartHeaderIcon_{name}").AddComponent<Image>();
      headerText = new GameObject($"PartHeaderLabel_{name}").AddComponent<TextMeshProUGUI>();
      headerBackground.sprite = VABOrganizerAssets.Sprites["organizer-carat2"];
      headerBackground.type = Image.Type.Sliced;
      headerBackground.transform.SetParent(parent, false);
      headerBackground.color = new Color(1f, 1f, 1f, 1f);

      /// Set up the background/button
      headerButton = headerBackground.gameObject.AddComponent<Button>();
      headerButton.onClick.AddListener(() => OnClick());
      headerButton.targetGraphic = headerBackground;
      headerButton.transition = Selectable.Transition.ColorTint;
      headerButton.colors = new ColorBlock()
      {
        colorMultiplier = 1f,
        fadeDuration = 0.1f,
        highlightedColor = new Color(1f, 1f, 1f, 0.4f),
        normalColor = new Color(1f, 1f, 1f, 0.1f),
        pressedColor = new Color(1f, 1f, 1f, 0.2f)
      };        
      
      RectTransform headerXform = headerBackground.GetComponent<RectTransform>();
      headerXform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20f);

      /// Set up the text
      headerText.transform.SetParent(headerBackground.transform, false);
      RectTransform textXform = headerText.GetComponent<RectTransform>();

      textXform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20f);
      textXform.offsetMin = new Vector2(textXform.offsetMin.x + 15f, textXform.offsetMin.y);
      headerText.fontSize = 12;
      headerText.text = label;
      headerText.fontStyle = FontStyles.Bold;
      headerText.margin = new Vector4(0, 4, 0, 2);

      SetCategoryVisible(false);
      /// Set up the carat icon
      headerIcon.sprite = VABOrganizerAssets.Sprites["organizer-arrow-down"];
      headerIcon.transform.SetParent(headerBackground.transform, false);
      RectTransform headerIconXform = headerIcon.GetComponent<RectTransform>();
      headerIconXform.anchorMin = headerIconXform.anchorMax = headerIconXform.pivot = Vector2.zero;
      headerIconXform.offsetMin = new Vector2(5, 7);
      headerIconXform.offsetMax = new Vector2(13, 14);
    }

    /// <summary>
    /// Sets whether we can see a category at all
    /// </summary>
    /// <param name="shown"></param>
    public void SetCategoryVisible(bool shown)
    {
      categoryVisible = shown;
      if (headerObj)
        headerObj.SetActive(categoryVisible);
      if (gridObj)
        gridObj.SetActive(categoryVisible);

    }
    public bool TryAssignPart(EditorPartIcon icon, AvailablePart part)
    {
      // Put the underscores back 
      bool passedName = validParts.Contains(part.name.Replace('.', '_'));
      bool passedRegex = false;
      bool passedRegexPath = false;

      foreach (string expr in validRegex)
      {
        if (Regex.IsMatch(part.name, expr))
          passedRegex = true;
      }
      foreach (string expr in validPaths)
      {
        if (Regex.IsMatch(part.partUrl, expr))
          passedRegexPath = true;
      }

      if (passedName | passedRegex | passedRegexPath)
      {
        AssignPart(icon);
        return true;
      }
      string parsedCatgeory = "";
      if (part.partConfig.TryGetValue("organizerSubcategory", ref parsedCatgeory))
      {
        if (name == parsedCatgeory)
        {
          AssignPart(icon);
          return true;
        }
      }
      
      return false;
    }
    public void AssignPart(EditorPartIcon icon)
    {
      categoryEmpty = false;
      icon.transform.SetParent(gridTransform, false);
      SetCategoryVisible(true);
    }
    public void ClearParts()
    {
      categoryEmpty = true;
      SetCategoryVisible(false);
    }

    public void OnClick()
    {
      if (categoryVisible)
      {
        categoryActive = !categoryActive;
        gridTransform.gameObject.SetActive(categoryActive);
        if (categoryActive)
        {
          headerIcon.sprite = VABOrganizerAssets.Sprites["organizer-arrow-down"];
        }
        else
        {
          headerIcon.sprite = VABOrganizerAssets.Sprites["organizer-arrow-right"];
        }
      }
    }

  }
}
