using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VABOrganizer
{
  public class CustomSortVariable
  {
    public string Name
    {
      get { return name; }
      private set { name = value; }
    }
    /// <summary>
    /// 
    /// </summary>
    public string VariableName
    {
      get { return variableName; }
      private set { variableName = value; }
    }
    /// <summary>
    /// 
    /// </summary>
    public string NodeType
    {
      get { return nodeType; }
      private set { nodeType = value; }
    }
    /// <summary>
    /// 
    /// </summary>
    public string NodeName
    {
      get { return nodeName; }
      private set { nodeName = value; }
    }
    /// <summary>
    /// 
    /// </summary>
    public string FieldName
    {
      get { return fieldName; }
      private set { fieldName = value; }
    }


    protected const string NODE_NAME = "name";
    protected const string NODE_FIELD_NAME = "FieldName";
    protected const string NODE_NODE_TYPE_NAME = "NodeType";
    protected const string NODE_NODE_NAME_NAME = "NodeName";
    protected const string NODE_VARIABLE_NAME = "VariableName";

    private string name = "category";
    private string variableName = "label";
    private string fieldName = "isp";
    private string nodeName = "";
    private string nodeType = "PART";

    public CustomSortVariable() { }

    public CustomSortVariable(ConfigNode node)
    {
      Load(node);
    }

    public void Load(ConfigNode node)
    {
      node.TryGetValue(NODE_NAME, ref name);
      node.TryGetValue(NODE_FIELD_NAME, ref fieldName);
      node.TryGetValue(NODE_NODE_NAME_NAME, ref nodeName);
      node.TryGetValue(NODE_NODE_TYPE_NAME, ref nodeType);
      node.TryGetValue(NODE_VARIABLE_NAME, ref variableName);
    }
  }
}
