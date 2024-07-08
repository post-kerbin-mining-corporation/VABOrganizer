using UnityEngine;
using KSP.Localization;

namespace VABOrganizer
{
  /// <summary>
  /// Represents data for a bulkhead tag
  /// </summary>
  public class BulkheadData
  {
    /// <summary>
    /// The name
    /// </summary>
    public string Name;
    /// <summary>
    /// The localized tag for the label
    /// </summary>
    public string Label;
    /// <summary>
    /// The size in meteres of the bulkhead - used for sorting
    /// </summary>
    public float Size;
    /// <summary>
    /// The color for tags of this bulkhead type
    /// </summary>
    public Color Color;

    public BulkheadData()
    {
      Name = "UNK";
      Label = "";
      Size = 1000;
      Color = new Color(0, 0, 0, 0);
    }
    public BulkheadData(ConfigNode configNode)
    {
      Load(configNode);
    }
    public void Load(ConfigNode configNode)
    {
      configNode.TryGetValue("name", ref Name);
      configNode.TryGetValue("Label", ref Label); ;
      configNode.TryGetValue("Size", ref Size);
      configNode.TryGetValue("Color", ref Color);

      Label = Localizer.Format(Label);
    }
  }
}
