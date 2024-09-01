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
    /// The name of the variable to populate that can be used for sorting
    /// </summary>
    public string VariableName
    {
      get { return variableName; }
      private set { variableName = value; }
    }
    /// <summary>
    /// The type of node to look in. Options are
    /// PART: field at the top of a PART
    /// MODULE: field in a MODULE
    /// MODULE_DATA: field in a data node in a MODULE
    /// RESOURCE: field in a RESOURCE
    /// </summary>
    public string NodeType
    {
      get { return nodeType; }
      private set { nodeType = value; }
    }
    /// <summary>
    /// Name of the node. Eg to find a MODULE name ModuleTacoTruck you would have ModuleTacoTruck here
    /// </summary>
    public string NodeName
    {
      get { return nodeName; }
      private set { nodeName = value; }
    }
    /// <summary>
    /// The type of data node to look in. Could be anything!
    /// </summary>
    public string DataNodeType
    {
      get { return dataNodeType; }
      private set { dataNodeType = value; }
    }
    /// <summary>
    /// Name of the data node. 
    /// </summary>
    public string DataNodeName
    {
      get { return dataNodeName; }
      private set { dataNodeName = value; }
    }
    /// <summary>
    /// The name of the actual field to parse
    /// </summary>
    public string FieldName
    {
      get { return fieldName; }
      private set { fieldName = value; }
    }
    /// <summary>
    /// In case of multiple entries for a part, the combine mode to use
    /// </summary>
    public string CombineMethod
    {
      get { return combineName; }
      private set { combineName = value; }
    }
    /// <summary>
    /// In case the field is a FloatCurve, the key to sample
    /// </summary>
    public string FieldCurveKey
    {
      get { return fieldCurveKey; }
      private set { fieldCurveKey = value; }
    }
    /// <summary>
    /// In case the field is a FloatCurve, the way to sample a curve
    /// </summary>
    public string FieldCurveParse
    {
      get { return fieldCurveParse; }
      private set { fieldCurveParse = value; }
    }

    protected const string NODE_NAME = "name";
    protected const string NODE_FIELD_NAME = "FieldName";
    protected const string NODE_FIELD_KEY_NAME = "FieldCurveKey";
    protected const string NODE_FIELD_CURVE_PARSE_NAME = "FieldCurveParseMethod";
    protected const string NODE_NODE_TYPE_NAME = "NodeType";
    protected const string NODE_NODE_NAME_NAME = "NodeName";
    protected const string NODE_DATA_NODE_TYPE_NAME = "DataNodeType";
    protected const string NODE_DATA_NODE_NAME_NAME = "DataNodeName";
    protected const string NODE_VARIABLE_NAME = "VariableName";
    protected const string NODE_COMBINE_NAME = "CombineMethod";

    private string name = "category";
    private string variableName = "label";
    private string fieldName = "isp";
    private string nodeName = "";
    private string nodeType = "PART";
    private string dataNodeName = "";
    private string dataNodeType = "";
    private string combineName = "MAX";
    private string fieldCurveKey = "";
    private string fieldCurveParse = "";

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
      node.TryGetValue(NODE_DATA_NODE_NAME_NAME, ref dataNodeName);
      node.TryGetValue(NODE_DATA_NODE_TYPE_NAME, ref dataNodeType);
      node.TryGetValue(NODE_COMBINE_NAME, ref combineName);
      node.TryGetValue(NODE_FIELD_KEY_NAME, ref fieldCurveKey);
      node.TryGetValue(NODE_FIELD_CURVE_PARSE_NAME, ref fieldCurveParse);
    }
  }
}
