using Database;
using Harmony;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SmartReservoir
{
    [SerializationConfig(MemberSerialization.OptIn)]
    class SmartReservoir : KMonoBehaviour, IActivationRangeTarget, ISim200ms
    {
        private float updateInterval = 0f;
        private float timeSinceLastUpdate = 1000f;
        private MeterController meter;
        private MeterController logicMeter;

        [Serialize] private int activateValue = 0;
        [Serialize] private int deactivateValue = 100;
        [Serialize] private bool activated;
        [MyCmpGet] private LogicPorts logicPorts;
        [MyCmpAdd] private CopyBuildingSettings copyBuildingSettings;
        [MyCmpGet] private Storage storage;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_fill", "meter_OL");
            Subscribe((int)GameHashes.OnStorageChange, OnStorageChange);
            Subscribe((int)GameHashes.CopySettings, new Action<object>(OnCopySettings));
            OnStorageChange(null);
            CreateLogicMeter();
            Subscribe(-536857173, new Action<object>(UpdateLogicCircuit)); // -536857173
        }

        private void CreateLogicMeter()
        {
            logicMeter = new MeterController(GetComponent<KBatchedAnimController>(), "logicmeter_target", "logicmeter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[0]);
        }

        private void OnStorageChange(object data)
        {
            meter.SetPositionPercent(Mathf.Clamp01(storage.MassStored() / storage.capacityKg));
        }

        private void OnCopySettings(object data)
        {
            GameObject gameObject = (GameObject)data;
            SmartReservoir component = gameObject.GetComponent<SmartReservoir>();
            if (component != null)
            {
                ActivateValue = component.ActivateValue;
                DeactivateValue = component.DeactivateValue;
            }
        }

        public void Sim200ms(float dt)
        {
            // update the update timer
            timeSinceLastUpdate += dt;
            if (timeSinceLastUpdate < updateInterval)
            {
                return;
            }
            UpdateLogicCircuit(null);
        }

        public static LocString LOGIC_PORT = "Fill Parameters";
        public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when reservoir is less than <b>Low Threshold</b> filled";
        public static LocString LOGIC_PORT_INACTIVE = "Sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when the reservoir is more than <b>High Threshold</b> filled, until <b>Low Threshold</b> is reached again";
        public static LocString ACTIVATE_TOOLTIP = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when reservoir is less than <b>{0}%</b> filled";
        public static LocString DEACTIVATE_TOOLTIP = "Sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when reservoir is more than <b>{0}%</b> filled";
        public static LocString SIDESCREEN_TITLE = "Logic Activation Parameters";
        public static LocString SIDESCREEN_ACTIVATE = "Low Threshold, %:";
        public static LocString SIDESCREEN_DEACTIVATE = "High Threshold, %:";

        public float ActivateValue { get => (float)deactivateValue; set { deactivateValue = (int)value; UpdateLogicCircuit(null); } }
        public float DeactivateValue { get => (float)activateValue; set { activateValue = (int)value; UpdateLogicCircuit(null); } }
        public float MinValue { get => 0f; }
        public float MaxValue { get => 100f; }
        public bool UseWholeNumbers { get => true; }
        public string ActivationRangeTitleText { get => SIDESCREEN_TITLE; }
        public string ActivateSliderLabelText { get => SIDESCREEN_DEACTIVATE; }
        public string DeactivateSliderLabelText { get => SIDESCREEN_ACTIVATE; }
        public string ActivateTooltip { get => ACTIVATE_TOOLTIP; }
        public string DeactivateTooltip { get => DEACTIVATE_TOOLTIP; }
        public float PercentFull { get => storage.MassStored() / storage.capacityKg; }
        public static readonly HashedString PORT_ID = "ReservoirSmartLogicPort";

        private void UpdateLogicCircuit(object data)
        {
            timeSinceLastUpdate = 0f;
            float num = (float)Mathf.RoundToInt(PercentFull * 100f);
            if (activated)
            {
                if (num >= (float)deactivateValue)
                {
                    activated = false;
                }
            }
            else if (num <= (float)activateValue)
            {
                activated = true;
            }
            logicPorts.SendSignal(PORT_ID, (!activated) ? 0 : 1);
            logicMeter.SetPositionPercent((activated) ? 0f : 1f);
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    public class DbPatch
    {
        public static void Prefix()
        {
            List<string> ls = new List<string>(Techs.TECH_GROUPING["ImprovedLiquidPiping"]) { SmartLiquidReservoirConfig.ID };
            Techs.TECH_GROUPING["ImprovedLiquidPiping"] = ls.ToArray();
            List<string> gs = new List<string>(Techs.TECH_GROUPING["HVAC"]) { SmartGasReservoirConfig.ID };
            Techs.TECH_GROUPING["HVAC"] = gs.ToArray();
        }
    }
}