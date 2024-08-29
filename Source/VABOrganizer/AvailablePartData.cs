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

        // Parse data out of part top level 
        ProcessPartKeys(basePart.partConfig);

        // Parse data out of module nodes
        for (int i = 0; i < moduleNodes.Length; i++)
        {
          /// Process the hardcoded 'complicated' ones
          ProcessModule_ReactionWheel(moduleNodes[i]);
          ProcessModule_Engine(moduleNodes[i]);
          ProcessModule_RCS(moduleNodes[i]);

          for (int j = 0; j < AdvancedSortingDataStore.ConfigVariables.Count; j++)
          {
            CustomSortVariable sortVar = AdvancedSortingDataStore.ConfigVariables[j];
            if (sortVar.NodeType == "MODULE" && moduleNodes[i].GetValue("name") == sortVar.NodeName)
            {
              float dataValue = 0f;
              if (moduleNodes[i].TryGetValue(sortVar.FieldName, ref dataValue))
              {
                AddOrMaxKey(sortVar.VariableName, dataValue);
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
              float dataValue = 0f;
              if (resourceNodes[i].TryGetValue(sortVar.FieldName, ref dataValue))
              {
                AddOrMaxKey(sortVar.VariableName, dataValue);
              }
            }
          }
        }

      }

      /// <summary>
      /// Add a data key - if it already exists take the higher one
      /// </summary>
      /// <param name="key"></param>
      /// <param name="value"></param>
      void AddOrMaxKey(string key, float value)
      {
        if (dataEntries.ContainsKey(key))
        {
          dataEntries[key] = Mathf.Max(value, dataEntries[key]);
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
        int crewData = 0;
        if (node.TryGetValue("CrewCapacity", ref crewData))
        {
          AddOrMaxKey("PartVar_CrewCapacity", crewData);
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

          AddOrMaxKey("ModuleVar_ReactionWheelAverageTorque", totalTorque / (float)(torqueCount));
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

            AddOrMaxKey("ModuleVar_RCSThrustVacuum", thrust);
            AddOrMaxKey("ModuleRCS_RCSThrustASL", thrustASL);

            AddOrMaxKey("ModuleVar_RCSIspASL", ispCurve.Evaluate(1f));
            AddOrMaxKey("ModuleVar_RCSIspVacuum", ispCurve.Evaluate(0f));
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

            AddOrMaxKey("ModuleVar_EngineThrustVacuum", thrust);
            AddOrMaxKey("ModuleVar_EngineThrustASL", thrustASL);

            AddOrMaxKey("ModuleVar_EngineTWRASL", thrustASL / basePart.MinimumMass);
            AddOrMaxKey("ModuleVar_EngineTWRVacuum", thrust / basePart.MinimumMass);

            AddOrMaxKey("ModuleVar_EngineIspASL", ispCurve.Evaluate(1f));
            AddOrMaxKey("ModuleVar_EngineIspVacuum", ispCurve.Evaluate(0f));
          }

        }
      }
    }
  }
}