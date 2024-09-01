using System.Collections.Generic;
using UnityEngine;

namespace VABOrganizer
{
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public class AdvancedSortingDataStore : MonoBehaviour
  {

    public Dictionary<string, AvailablePartData> PartData;
    public static List<CustomSortVariable> ConfigVariables = new List<CustomSortVariable>();

    public static AdvancedSortingDataStore Instance;

    void Awake()
    {
      Instance = this;
    }

    void Start()
    {

      ConfigNode[] variableNodes = GameDatabase.Instance.GetConfigNodes(Settings.ORGANIZER_VARIABLE_NODE_NAME);
      Utils.Log($"[AdvancedSortingDataStore]: Loading variable definitions");
      if (variableNodes.Length > 0)
      {
        ConfigVariables = new List<CustomSortVariable>();
        foreach (ConfigNode varNode in variableNodes)
        {
          CustomSortVariable data = new CustomSortVariable(varNode);
          if (!ConfigVariables.Contains(data))
          {
            ConfigVariables.Add(data);
          }
          else
          {
            Utils.LogWarning($"[AdvancedSortingDataStore]: Multiple {Settings.ORGANIZER_VARIABLE_NODE_NAME} with the same name ({data.Name}) found, skipping others");
          }
        }
        Utils.Log($"[AdvancedSortingDataStore]: Loaded {ConfigVariables.Count} sorting variable definitions");
      }

      GameEvents.OnPartLoaderLoaded.Add(OnPartLoaderLoaded);
    }

    void OnDestroy()
    {
      GameEvents.OnPartLoaderLoaded.Remove(OnPartLoaderLoaded);
    }

    public void OnPartLoaderLoaded()
    {
      PartData = new Dictionary<string, AvailablePartData>();
      var watch = System.Diagnostics.Stopwatch.StartNew();
      Utils.Log($"[AdvancedSortingDataStore]: Starting part data parse");
      foreach (AvailablePart p in PartLoader.Instance.loadedParts)
      {
        PartData.Add(p.name, new AvailablePartData(p));
      }
      watch.Stop();
      /// TODO: 7 ms for stock + NFT on Chris' garbage laptop is fine, may need to be optimized later
      Utils.Log($"[AdvancedSortingDataStore]: Parsed config data in {watch.ElapsedMilliseconds} ms");

    }

  }

  public class AvailablePartData
  {
    Dictionary<string, float> dataEntries;
    AvailablePart basePart;

    public float GetData(string index)
    {
      if (dataEntries.TryGetValue(index, out float value))
      {
        return value;
      }
      return 0f;
    }

    public AvailablePartData(AvailablePart sourcePart)
    {
      basePart = sourcePart;
      dataEntries = new Dictionary<string, float>();
      if (basePart.partConfig != null)
      {
        ConfigNode[] resourceNodes = basePart.partConfig.GetNodes("RESOURCE");
        ConfigNode[] moduleNodes = basePart.partConfig.GetNodes("MODULE");

        /// Parse data out of part top level 
        /// Complicated ones
        ProcessPartKeys(basePart.partConfig);

        for (int i = 0; i < AdvancedSortingDataStore.ConfigVariables.Count; i++)
        {
          CustomSortVariable sortVar = AdvancedSortingDataStore.ConfigVariables[i];
          if (sortVar.NodeType == "PART")
          {
            ParseNodeForConfigSortVar(basePart.partConfig, sortVar);
          }
        }

        // Parse data out of module nodes
        for (int i = 0; i < moduleNodes.Length; i++)
        {
          /// Process the hardcoded 'complicated' ones
          ProcessModule_ReactionWheel(moduleNodes[i]);
          ProcessModule_Engine(moduleNodes[i]);
          ProcessModule_RCS(moduleNodes[i]);
          ProcessModule_DataTransmitter(moduleNodes[i]);
          ProcessModule_ScienceExperiment(moduleNodes[i]);

          for (int j = 0; j < AdvancedSortingDataStore.ConfigVariables.Count; j++)
          {
            CustomSortVariable sortVar = AdvancedSortingDataStore.ConfigVariables[j];
            if (sortVar.NodeType.Contains("MODULE") && moduleNodes[i].GetValue("name") == sortVar.NodeName)
            {
              if (sortVar.NodeType == "MODULE_DATA")
              {
                ConfigNode dataNode = new ConfigNode();
                if (moduleNodes[i].TryGetNode(sortVar.DataNodeType, ref dataNode) && dataNode.GetValue("name") == sortVar.DataNodeName)
                {
                  ParseNodeForConfigSortVar(dataNode, sortVar);
                }
              }
              else
              {
                ParseNodeForConfigSortVar(moduleNodes[i], sortVar);                
              }
            }
          }
        }
        // Parse data out of resource nodes        
        for (int i = 0; i < resourceNodes.Length; i++)
        {
          for (int j = 0; j < AdvancedSortingDataStore.ConfigVariables.Count; j++)
          {
            CustomSortVariable sortVar = AdvancedSortingDataStore.ConfigVariables[j];
            if (sortVar.NodeType == "RESOURCE" && resourceNodes[i].GetValue("name") == sortVar.NodeName)
            {
              ParseNodeForConfigSortVar(resourceNodes[i], sortVar);
            }
          }
        }

      }

      void ParseNodeForConfigSortVar(ConfigNode node, CustomSortVariable sortVar)
      {
        if (sortVar.FieldCurveKey != "" || sortVar.FieldCurveParse != "")
        {
          if (node.HasNode(sortVar.FieldName))
          {
            FloatCurve curveField = new FloatCurve();
            curveField.Load(node.GetNode(sortVar.FieldName));
            if (sortVar.FieldCurveParse == "SINGLE")
            {
              float curveKey = 0f;
              if (node.TryGetValue(sortVar.FieldCurveKey, ref curveKey))
              {
                AddSorterKey(sortVar.VariableName, curveField.Evaluate(curveKey), sortVar.CombineMethod);
              }
            }
            if (sortVar.FieldCurveParse == "MAX")
            {
              AddSorterKey(sortVar.VariableName, curveField.Evaluate(curveField.maxTime), sortVar.CombineMethod);
            }
            if (sortVar.FieldCurveParse == "MIN")
            {
              AddSorterKey(sortVar.VariableName, curveField.Evaluate(curveField.minTime), sortVar.CombineMethod);
            }
          }

        }
        else
        {
          float dataValue = 0f;
          if (node.TryGetValue(sortVar.FieldName, ref dataValue))
          {
            AddSorterKey(sortVar.VariableName, dataValue, sortVar.CombineMethod);
          }
        }
      }
      /// <summary>
      /// Add a data key - if it already exists take the higher one
      /// </summary>
      /// <param name="key"></param>
      /// <param name="value"></param>
      void AddSorterKey(string key, float value, string method)
      {
        if (dataEntries.ContainsKey(key))
        {
          if (method == "MAX")
          {
            dataEntries[key] = Mathf.Max(value, dataEntries[key]);
          }
          if (method == "MIN")
          {
            dataEntries[key] = Mathf.Min(value, dataEntries[key]);
          }
          if (method == "REPLACE")
          {
            dataEntries[key] = value;
          }
        }
        else
        {
          dataEntries.Add(key, value);
        }
      }
      /// <summary>
      /// Gets all the data at the part level
      /// </summary>
      /// <param name="node"></param>
      void ProcessPartKeys(ConfigNode node)
      {
      }
      void ProcessModule_ScienceExperiment(ConfigNode node)
      {
        if (node.GetValue("name") == "ModuleScienceExperiment")
        {
          string expID = "";
          if (node.TryGetValue("experimentID", ref expID))
          {
            AddSorterKey("ModuleVar_Experiment", expID.GetHashCode(), "MAX");
            /// At this point we don't have access to experiment data, so we go and fetch it... This is probably not great
            ConfigNode[] experimentNodes = GameDatabase.Instance.GetConfigNodes("EXPERIMENT_DEFINITION");
            for (int i = 0; i < experimentNodes.Length; i++)
            {
              string expName = "";
              float dataValue = 0f;
              float dataScale = 1f;
              if (experimentNodes[i].TryGetValue("id", ref expName))
              {
                if (expName == expID)
                {
                  if (experimentNodes[i].TryGetValue("baseValue", ref dataValue))
                  {
                    /// If available, scale by the part data value
                    node.TryGetValue("scienceValueRatio", ref dataScale);
                    AddSorterKey("ModuleVar_ExperimentValue", dataValue * dataScale, "MAX");
                  }
                }
              }
            }
          }

        }
      }
      void ProcessModule_DataTransmitter(ConfigNode node)
      {
        if (node.GetValue("name") == "ModuleDataTransmitter")
        {
          bool flag = false;
          float num = 0f;
          float packetSize = 0f;
          float packetInterval = 0f;

          if (node.TryGetValue("antennaCombineable", ref flag))
          {
            if (node.TryGetValue("antennaCombinableExponent", ref num))
            {
              AddSorterKey("ModuleVar_AntennaCombinability", num, "MAX");
            }
          }

          if (node.TryGetValue("packetInterval", ref packetInterval) && node.TryGetValue("packetSize", ref packetSize))
          {
            AddSorterKey("ModuleVar_AntennaRate", packetSize / packetInterval, "MAX");
          }
          if (node.TryGetValue("packetResourceCost", ref num))
          {
            AddSorterKey("ModuleVar_AntennaCost", num / packetInterval, "MAX");
          }
        }

      }

      void ProcessModule_ReactionWheel(ConfigNode node)
      {
        if (node.GetValue("name") == "ModuleReactionWheel")
        {
          float totalTorque = 0f;
          int torqueCount = 0;
          float torque1 = 0;
          if (node.TryGetValue("PitchTorque", ref torque1))
          {
            torqueCount++;
            totalTorque += torque1;
          }
          if (node.TryGetValue("YawTorque", ref torque1))
          {
            torqueCount++;
            totalTorque += torque1;
          }
          if (node.TryGetValue("RollTorque", ref torque1))
          {
            torqueCount++;
            totalTorque += torque1;
          }

          AddSorterKey("ModuleVar_ReactionWheelAverageTorque", totalTorque / (float)(torqueCount), "MAX");
        }
      }
      void ProcessModule_RCS(ConfigNode node)
      {
        if (node.GetValue("name") == "ModuleRCSFX")
        {
          float thrust = 0f;
          ConfigNode ispCurveNode = new ConfigNode();
          if (node.TryGetValue("thrusterPower", ref thrust) && node.TryGetNode("atmosphereCurve", ref ispCurveNode))
          {
            FloatCurve ispCurve = new FloatCurve();
            ispCurve.Load(ispCurveNode);

            float mdot = thrust / (ispCurve.Evaluate(0f) * (float)PhysicsGlobals.GravitationalAcceleration);
            float thrustASL = mdot * (ispCurve.Evaluate(1f) * (float)PhysicsGlobals.GravitationalAcceleration);

            AddSorterKey("ModuleVar_RCSThrustVacuum", thrust, "MAX");
            AddSorterKey("ModuleRCS_RCSThrustASL", thrustASL, "MAX");

            AddSorterKey("ModuleVar_RCSIspASL", ispCurve.Evaluate(1f), "MAX");
            AddSorterKey("ModuleVar_RCSIspVacuum", ispCurve.Evaluate(0f), "MAX");
          }
        }
      }
      void ProcessModule_Engine(ConfigNode node)
      {
        if (node.GetValue("name") == "ModuleEnginesFX")
        {
          float thrust = 0f;
          ConfigNode ispCurveNode = new ConfigNode();
          if (node.TryGetValue("maxThrust", ref thrust) && node.TryGetNode("atmosphereCurve", ref ispCurveNode))
          {
            FloatCurve ispCurve = new FloatCurve();
            ispCurve.Load(ispCurveNode);

            float mdot = thrust / (ispCurve.Evaluate(0f) * (float)PhysicsGlobals.GravitationalAcceleration);
            float thrustASL = mdot * (ispCurve.Evaluate(1f) * (float)PhysicsGlobals.GravitationalAcceleration);

            AddSorterKey("ModuleVar_EngineThrustVacuum", thrust, "MAX");
            AddSorterKey("ModuleVar_EngineThrustASL", thrustASL, "MAX");

            AddSorterKey("ModuleVar_EngineTWRASL", thrustASL / basePart.MinimumMass, "MAX");
            AddSorterKey("ModuleVar_EngineTWRVacuum", thrust / basePart.MinimumMass, "MAX");

            AddSorterKey("ModuleVar_EngineIspASL", ispCurve.Evaluate(1f), "MAX");
            AddSorterKey("ModuleVar_EngineIspVacuum", ispCurve.Evaluate(0f), "MAX");
          }

        }
      }
    }
  }
}