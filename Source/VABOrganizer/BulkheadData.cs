using UnityEngine;
using KSP.Localization;

namespace VABOrganizer
{
  public class BulkheadData
  {
    public string Name;
    public string Label;
    public float Size;
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
