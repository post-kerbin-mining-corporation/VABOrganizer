using System.Collections.Generic;
using UnityEngine;

namespace VABOrganizer
{
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public class PartFilterDatastore : MonoBehaviour
  {

    public static PartFilterDatastore Instance;

    void Awake()
    {
      Instance = this;
    }

    public void Start()
    {
      GameEvents.OnPartLoaderLoaded.Add(OnPartLoaderLoaded);
    }

    public Dictionary<string, AvailablePartData> PartFilterData;

    public void OnPartLoaderLoaded()
    {
      PartFilterData = new Dictionary<string, AvailablePartData>();
      foreach (AvailablePart p in PartLoader.Instance.loadedParts)
      {
        PartFilterData.Add(p.name, new AvailablePartData(p));
      }
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
        // Parse data out of part top level 
        ProcessPartKeys(basePart.partConfig);

        // Parse data out of part modules
        ConfigNode[] moduleNodes = basePart.partConfig.GetNodes("MODULE");
        for (int i = 0; i < moduleNodes.Length; i++)
        {
          ProcessModule_SAS(moduleNodes[i]);
          ProcessModule_Gimbal(moduleNodes[i]);
          ProcessModule_ReactionWheel(moduleNodes[i]);
          ProcessModule_Engine(moduleNodes[i]);
          ProcessModule_RCS(moduleNodes[i]);
          ProcessModule_Command(moduleNodes[i]);
          ProcessModule_DataTransmitter(moduleNodes[i]);
          ProcessModule_SolarPanel(moduleNodes[i]);
          ProcessModule_HeatRadiation(moduleNodes[i]);
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
        AddOrMaxKey("part_crewCapacity", crewData);
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

        AddOrMaxKey("ModuleReactionWheel_AverageTorque", totalTorque / (float)(torqueCount));
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

          AddOrMaxKey("ModuleRCS_ThrustVacuum", thrust);
          AddOrMaxKey("ModuleRCS_ThrustASL", thrustASL);

          AddOrMaxKey("ModuleRCS_IspASL", ispCurve.Evaluate(1f));
          AddOrMaxKey("ModuleRCS_IspVacuum", ispCurve.Evaluate(0f));
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

          AddOrMaxKey("ModuleEngines_ThrustVacuum", thrust);
          AddOrMaxKey("ModuleEngines_ThrustASL", thrustASL);

          AddOrMaxKey("ModuleEngines_TWRASL", thrustASL / basePart.MinimumMass);
          AddOrMaxKey("ModuleEngines_TWRVacuum", thrust / basePart.MinimumMass);

          AddOrMaxKey("ModuleEngines_IspASL", ispCurve.Evaluate(1f));
          AddOrMaxKey("ModuleEngines_IspVacuum", ispCurve.Evaluate(0f));
        }

      }
    }
    void ProcessModule_Gimbal(ConfigNode node)
    {
      if (node.GetValue("name") == "ModuleGimbal")
      {
        float gimbalRange = 0f;
        if (node.TryGetValue("gimbalRange", ref gimbalRange))
        {

          AddOrMaxKey("ModuleGimbal_Range", gimbalRange);
        }
      }
    }
    void ProcessModule_SAS(ConfigNode node)
    {
      if (node.GetValue("name") == "ModuleSAS")
      {
        float sasLevel = 0f;
        if (node.TryGetValue("SASServiceLevel", ref sasLevel))
        {

          AddOrMaxKey("ModuleSAS_ServiceLevel", sasLevel);
        }
      }
    }
    void ProcessModule_Command(ConfigNode node)
    {
      if (node.GetValue("name") == "ModuleCommand")
      {

      }
    }
    void ProcessModule_DataTransmitter(ConfigNode node)
    {
      if (node.GetValue("name") == "ModuleDataTransmitter")
      {
        float antennaPower = 0f;
        if (node.TryGetValue("antennaPower", ref antennaPower))
        {

          AddOrMaxKey("ModuleDataTransmitter_Power", antennaPower);
        }
      }
    }

    void ProcessModule_HeatRadiation(ConfigNode node)
    {
      if (node.GetValue("name") == "ModuleActiveRadiator")
      {
        float antennaPower = 0f;
        if (node.TryGetValue("maxEnergyTransfer", ref antennaPower))
        {

          AddOrMaxKey("ModuleRadiator_RadiatorCapacity", antennaPower);
        }
      }
    }
    void ProcessModule_SolarPanel(ConfigNode node)
    {
      if (node.GetValue("name") == "ModuleDeployableSolarPanel")
      {
        float power = 0f;
        if (node.TryGetValue("chargeRate", ref power))
        {

          AddOrMaxKey("ModuleSolarPanel_ChargeRate", power);
        }
      }
    }

  }
}
