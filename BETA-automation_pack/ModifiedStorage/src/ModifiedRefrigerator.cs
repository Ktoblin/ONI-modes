using Database;
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
    class ModifiedRefrigerator : KMonoBehaviour, IUserControlledCapacity, IEffectDescriptor, IGameObjectEffectDescriptor, IActivationRangeTarget//, ISim200ms
    {
        private FilteredStorage filteredStorage;
        private SimulatedTemperatureAdjuster temperatureAdjuster;
        //private float updateInterval = 0f;
        //private float timeSinceLastUpdate = 1000f;

        [MyCmpGet]
        private Storage storage;
        [MyCmpGet]
        private LogicPorts ports;
        [MyCmpGet]
        private Operational operational;
        [SerializeField]
        public float simulatedInternalTemperature = 277.15f;
        [SerializeField]
        public float simulatedInternalHeatCapacity = 400f;
        [SerializeField]
        public float simulatedThermalConductivity = 1000f;
        [Serialize]
        private float userMaxCapacity = float.PositiveInfinity;
        [Serialize]
        private int activateValue = 0;
        [Serialize]
        private int deactivateValue = 100;
        [Serialize]
        private bool activated;

        protected override void OnPrefabInit()
        {
            filteredStorage = new FilteredStorage(this, null, new Tag[]
            {
            GameTags.Compostable
            }, this, true, Db.Get().ChoreTypes.FoodFetch);
        }

        protected override void OnSpawn()
        {
            operational.SetActive(operational.IsOperational, false);
            GetComponent<KAnimControllerBase>().Play("off", KAnim.PlayMode.Once, 1f, 0f);
            filteredStorage.FilterChanged();
            temperatureAdjuster = new SimulatedTemperatureAdjuster(simulatedInternalTemperature, simulatedInternalHeatCapacity, simulatedThermalConductivity, base.GetComponent<Storage>());
            UpdateLogicCircuit();
            Subscribe((int)GameHashes.OperationalChanged, new Action<object>(OnOperationalChanged));
            Subscribe((int)GameHashes.CopySettings, new Action<object>(OnCopySettings));
            Subscribe(-1697596308, new Action<object>(UpdateLogicCircuitCB));
            Subscribe(-592767678, new Action<object>(UpdateLogicCircuitCB));
        }

        protected override void OnCleanUp()
        {
            filteredStorage.CleanUp();
            temperatureAdjuster.CleanUp();
        }

        private void OnOperationalChanged(object data)
        {
            bool isOperational = operational.IsOperational;
            operational.SetActive(isOperational, false);
        }

        public bool IsActive()
        {
            return operational.IsActive;
        }

        private void OnCopySettings(object data)
        {
            GameObject gameObject = (GameObject)data;
            if (gameObject == null)
            {
                return;
            }
            ModifiedRefrigerator component = gameObject.GetComponent<ModifiedRefrigerator>();
            if (component == null)
            {
                return;
            }
            UserMaxCapacity = component.UserMaxCapacity;
            activateValue = component.activateValue;
            deactivateValue = component.deactivateValue;
        }

        public List<Descriptor> GetDescriptors(BuildingDef def)
        {
            return GetDescriptors(def.BuildingComplete);
        }

        public List<Descriptor> GetDescriptors(GameObject go)
        {
            return SimulatedTemperatureAdjuster.GetDescriptors(simulatedInternalTemperature);
        }

        public float UserMaxCapacity
        {
            get
            {
                return Mathf.Min(userMaxCapacity, storage.capacityKg);
            }
            set
            {
                userMaxCapacity = value;
                filteredStorage.FilterChanged();
                UpdateLogicCircuit();
            }
        }

        public float AmountStored { get { return storage.MassStored(); } }
        public float MinCapacity { get { return 0f; } }
        public float MaxCapacity { get { return storage.capacityKg; } }
        public bool WholeValues { get { return false; } }
        public LocString CapacityUnits { get { return GameUtil.GetCurrentMassUnit(false); } }
        private void UpdateLogicCircuitCB(object data) { UpdateLogicCircuit(); }

        private void UpdateLogicCircuit()
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

            
            bool isOperational = this.operational.IsOperational;
            ports.SendSignal(FilteredStorage.FULL_PORT_ID, (!activated && isOperational) ? 0 : 1);
            filteredStorage.SetLogicMeter(!activated && isOperational);
            operational.SetActive(isOperational, false);
        }

        //public void Sim200ms(float dt)
        //{
        //    timeSinceLastUpdate += dt;
        //    if (timeSinceLastUpdate < updateInterval)
        //    {
        //        return;
        //    }
        //    //foreach (GameObject go in storage.items)
        //    //{
        //    //    KSelectable component = go.GetComponent<KSelectable>();
        //    //    component.SetStatusItem(Db.Get().StatusItemCategories.PreservationTemperature, Db.Get().CreatureStatusItems.Refrigerated, component);
        //    //    Debug.Log("OMG: Is " + component.name.ToString() + " refrigerated? " + component.GetStatusItem(Db.Get().StatusItemCategories.PreservationTemperature).ToString());
        //    //}
        //}

        public static LocString LOGIC_PORT = "Fill Parameters";
        public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when storage is less than <b>Low Threshold</b> filled";
        public static LocString LOGIC_PORT_INACTIVE = "Sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when the storage is more than <b>High Threshold</b> filled, until <b>Low Threshold</b> is reached again";
        public static LocString ACTIVATE_TOOLTIP = "Sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when storage is more than <b>{0}%</b> filled";
        public static LocString DEACTIVATE_TOOLTIP = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when storage is less than <b>{0}%</b> filled";
        public static LocString SIDESCREEN_TITLE = "Logic Activation Parameters";
        public static LocString SIDESCREEN_ACTIVATE = "Low Threshold, %:";
        public static LocString SIDESCREEN_DEACTIVATE = "High Threshold, %:";

        public float ActivateValue { get => (float)deactivateValue; set { deactivateValue = (int)value; UpdateLogicCircuit(); } }
        public float DeactivateValue { get => (float)activateValue; set { activateValue = (int)value; UpdateLogicCircuit(); } }
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
    public class ModifiedRefrigeratorPatch
    {
        public static LocString NAME = new LocString("Modified Refrigerator",
            "STRINGS.BUILDINGS.PREFABS." + ModifiedRefrigeratorConfig.ID.ToUpper() + ".NAME");

        public static LocString DESC = new LocString("Food spoilage can be slowed by ambient conditions as well as by refrigerators.\nSend a " + 
                                                    UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when mass of food reached High Threshold." +
                                                    "And send a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when mass of food below Low Threshold.",
            "STRINGS.BUILDINGS.PREFABS." + ModifiedRefrigeratorConfig.ID.ToUpper() + ".DESC");

        public static LocString EFFECT = new LocString("Stores " + UI.FormatAsLink("Food", "FOOD") + ".\n\nSends a " +
                                                        UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or " +
                                                        UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) +
                                                        " based on the configuration of the Logic Activation Parameters.",
            "STRINGS.BUILDINGS.PREFABS." + ModifiedRefrigeratorConfig.ID.ToUpper() + ".EFFECT");

        static void Prefix()
        {
            Strings.Add(NAME.key.String, NAME.text);
            Strings.Add(DESC.key.String, DESC.text);
            Strings.Add(EFFECT.key.String, EFFECT.text);
            ModUtil.AddBuildingToPlanScreen("Food", ModifiedRefrigeratorConfig.ID);
        }

        static void Postfix()
        {
            object obj = Activator.CreateInstance(typeof(ModifiedRefrigeratorConfig));
            BuildingConfigManager.Instance.RegisterBuilding(obj as IBuildingConfig);
        }
    }
}
