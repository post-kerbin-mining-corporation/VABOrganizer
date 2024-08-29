using System;
using System.Collections.Generic;
using KSP.Localization;

namespace VABOrganizer
{
  public class AdvancedSortType
  {
    public string Name
    {
      get { return name; }
      private set { name = value; }
    }
    /// <summary>
    /// Localized sorter name
    /// </summary>
    public string Label
    {
      get { return label; }
      private set { label = value; }
    }
    /// <summary>
    /// The sorter to use
    /// </summary>
    public string Sorter
    {
      get { return sorter; }
      private set { sorter = value; }
    }
    /// <summary>
    /// The VAB PartCategories where this sorter will be used
    /// </summary>
    public List<string> AssociatedPartCategories
    {
      get { return partCategories; }
      private set { partCategories = value; }
    }

    protected const string NODE_NAME = "name";
    protected const string NODE_LABEL = "Label";
    protected const string NODE_SORTER = "Sorter";
    protected const string NODE_ASSOCIATED_CATEGORY = "VisibleCategories";
    protected const string NODE_CATEGORY = "category";

    private string name = "category";
    private string label = "label";
    private string sorter = "isp";
    private List<string> partCategories;

    public AdvancedSortType() { }

    public AdvancedSortType(ConfigNode node)
    {
      Load(node);
    }

    public void Load(ConfigNode node)
    {
      node.TryGetValue(NODE_NAME, ref name);
      node.TryGetValue(NODE_LABEL, ref label);
      node.TryGetValue(NODE_SORTER, ref sorter);

      partCategories = new List<string>();
      ConfigNode categoryNode = new ConfigNode();
      if (node.TryGetNode(NODE_ASSOCIATED_CATEGORY, ref categoryNode))
      {
        string[] categories = categoryNode.GetValues(NODE_CATEGORY);
        foreach (string stringCat in categories)
        {
            partCategories.Add(stringCat);
        }
      }

      label = Localizer.Format(label);
    }
  }
}
