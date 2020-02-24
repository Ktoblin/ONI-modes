using Harmony;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModifiedStorage
{
    [SerializationConfig(MemberSerialization.OptIn)]
    class ModifiedStorageLockerSmart : StorageLocker, IActivationRangeTarget, ISim200ms
    {
        private float updateInterval = 0f;
        private float timeSinceLastUpdate = 1000f;

        [MyCmpGet] private LogicPorts ports;
        [MyCmpGet] private Operational operational;
        [MyCmpGet] private Storage storage;

        //[Serialize] private float userMaxCapacity = float.PositiveInfinity;
        [Serialize] private int activateValue = 0;
        [Serialize] private int deactivateValue = 100;
        [Serialize] private bool activated;

        protected override void OnPrefabInit()
        {
            base.Initialize(true);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            ports = gameObject.GetComponent<LogicPorts>();
            Subscribe(-1697596308, new Action<object>(UpdateLogicCircuitCB));
            Subscribe(-592767678, new Action<object>(UpdateLogicCircuitCB));
            Subscribe((int)GameHashes.CopySettings, new Action<object>(OnCopySettings));
            UpdateLogicAndActiveState();
        }

        private void OnCopySettings(object data)
        {
            GameObject gameObject = (GameObject)data;
            if (gameObject == null)
            {
                return;
            }
            ModifiedStorageLockerSmart component = gameObject.GetComponent<ModifiedStorageLockerSmart>();
            if (component == null)
            {
                return;
            }
            UserMaxCapacity = component.UserMaxCapacity;
            activateValue = component.activateValue;
            deactivateValue = component.deactivateValue;
        }

        private void UpdateLogicCircuitCB(object data)
        {
            UpdateLogicAndActiveState();
        }

        private void UpdateLogicAndActiveState()
        {
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

            bool isOperational = operational.IsOperational;
            ports.SendSignal(FilteredStorage.FULL_PORT_ID, (!activated && isOperational) ? 0 : 1);
            filteredStorage.SetLogicMeter(!activated && isOperational);
            operational.SetActive(isOperational, false);
        }

        public override float UserMaxCapacity
        {
            get
            {
                return Mathf.Min(base.UserMaxCapacity, storage.capacityKg);
            }
            set
            {
                base.UserMaxCapacity = value;
                filteredStorage.FilterChanged();
                this.UpdateLogicAndActiveState();
            }
        }

        //public new float AmountStored { get { return storage.MassStored(); } }
        //public new float MinCapacity { get { return 0f; } }
        //public new float MaxCapacity { get { return storage.capacityKg; } }
        //public new bool WholeValues { get { return false; } }
        //public new LocString CapacityUnits { get { return GameUtil.GetCurrentMassUnit(false); } }

        public void Sim200ms(float dt)
        {
            timeSinceLastUpdate += dt;
            if (timeSinceLastUpdate < updateInterval)
            {
                return;
            }
            UpdateLogicCircuitCB(null);
        }

        public static LocString LOGIC_PORT = "Fill Parameters";
        public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when storage is less than <b>Low Threshold</b> filled";
        public static LocString LOGIC_PORT_INACTIVE = "Sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when the storage is more than <b>High Threshold</b> filled, until <b>Low Threshold</b> is reached again";
        public static LocString ACTIVATE_TOOLTIP = "Sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when storage is more than <b>{0}%</b> filled";
        public static LocString DEACTIVATE_TOOLTIP = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when storage is less than <b>{0}%</b> filled";
        public static LocString SIDESCREEN_TITLE = "Logic Activation Parameters";
        public static LocString SIDESCREEN_ACTIVATE = "Low Threshold, %:";
        public static LocString SIDESCREEN_DEACTIVATE = "High Threshold, %:";

        public float ActivateValue { get => (float)deactivateValue; set { deactivateValue = (int)value; UpdateLogicAndActiveState(); } }
        public float DeactivateValue { get => (float)activateValue; set { activateValue = (int)value; UpdateLogicAndActiveState(); } }
        public float MinValue { get => 0f; }
        public float MaxValue { get => 100f; }
        public bool UseWholeNumbers { get => true; }
        public string ActivationRangeTitleText { get => SIDESCREEN_TITLE; }
        public string ActivateSliderLabelText { get => SIDESCREEN_DEACTIVATE; }
        public string DeactivateSliderLabelText { get => SIDESCREEN_ACTIVATE; }
        public string ActivateTooltip { get => ACTIVATE_TOOLTIP; }
        public string DeactivateTooltip { get => DEACTIVATE_TOOLTIP; }
        public float PercentFull { get => storage.MassStored() / UserMaxCapacity; }

    }


    [HarmonyPatch(typeof(GeneratedBuildings))]
    [HarmonyPatch("LoadGeneratedBuildings")]
    public class ModifiedStorageLockerSmartPatch
    {
        public static LocString NAME = new LocString("Modified Smart Storage Bin",
            "STRINGS.BUILDINGS.PREFABS." + ModifiedStorageLockerSmartConfig.ID.ToUpper() + ".NAME");

        public static LocString DESC = new LocString("Smart storage bins allow for the automation of resource organization based on type and mass.\nSend a " + 
                                                    UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when mass of solid materials reached High Threshold." +
                                                    "And send a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when mass of solid materials below Low Threshold.",
            "STRINGS.BUILDINGS.PREFABS." + ModifiedStorageLockerSmartConfig.ID.ToUpper() + ".DESC");

        public static LocString EFFECT = new LocString("Stores the Solid resources of your choosing.\n\nSends a " +
                                                        UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or " +
                                                        UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) +
                                                        " based on the configuration of the Logic Activation Parameters.",
            "STRINGS.BUILDINGS.PREFABS." + ModifiedStorageLockerSmartConfig.ID.ToUpper() + ".EFFECT");

        static void Prefix()
        {
            Strings.Add(NAME.key.String, NAME.text);
            Strings.Add(DESC.key.String, DESC.text);
            Strings.Add(EFFECT.key.String, EFFECT.text);
            ModUtil.AddBuildingToPlanScreen("Base", ModifiedStorageLockerSmartConfig.ID);
        }

        static void Postfix()
        {
            object obj = Activator.CreateInstance(typeof(ModifiedStorageLockerSmartConfig));
            BuildingConfigManager.Instance.RegisterBuilding(obj as IBuildingConfig);
        }
    }

}
