using Database;
using HarmonyLib;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModifiedStorage
{
    [AddComponentMenu("KMonoBehaviour/scripts/Refrigerator")]
    [SerializationConfig(MemberSerialization.OptIn)]
    class ModifiedRefrigerator : KMonoBehaviour, IUserControlledCapacity, IActivationRangeTarget
    {
        public static readonly HashedString PORT_ID = (HashedString)"ModRefrigeratorLogicPort";
        [MyCmpGet]
        private Storage storage;
        [MyCmpGet]
        private Operational operational;
        [MyCmpGet]
        private LogicPorts ports;
        private MeterController logicMeter;
        [Serialize]
        private int activateValue = 0;
        [Serialize]
        private int deactivateValue = 100;
        [Serialize]
        private bool activated;
        [Serialize]
        private float userMaxCapacity = float.PositiveInfinity;
        private FilteredStorage filteredStorage;
        private static readonly EventSystem.IntraObjectHandler<ModifiedRefrigerator> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<ModifiedRefrigerator>((System.Action<ModifiedRefrigerator, object>)((component, data) => component.OnCopySettings(data)));
        private static readonly EventSystem.IntraObjectHandler<ModifiedRefrigerator> UpdateLogicCircuitCBDelegate = new EventSystem.IntraObjectHandler<ModifiedRefrigerator>((System.Action<ModifiedRefrigerator, object>)((component, data) => component.UpdateLogicCircuitCB(data)));
        private static readonly EventSystem.IntraObjectHandler<ModifiedRefrigerator> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<ModifiedRefrigerator>((System.Action<ModifiedRefrigerator, object>)((component, data) => component.OnLogicValueChanged(data)));

        //protected override void OnPrefabInit() => this.filteredStorage = new FilteredStorage((KMonoBehaviour)this, (Tag[])null, new Tag[1]
        //{
        //    GameTags.Compostable
        //}, (IUserControlledCapacity)this, true, Db.Get().ChoreTypes.FoodFetch);

        protected override void OnPrefabInit()
        {
            this.filteredStorage = new FilteredStorage(this, new Tag[]
            {
            GameTags.Compostable
            }, this, true, Db.Get().ChoreTypes.FoodFetch);
        }

        protected override void OnSpawn()
        {
            this.GetComponent<KAnimControllerBase>().Play((HashedString)"off");
            this.filteredStorage.FilterChanged();
            this.ports = this.gameObject.GetComponent<LogicPorts>();
            this.UpdateLogicCircuit();
            this.Subscribe<ModifiedRefrigerator>(-905833192, ModifiedRefrigerator.OnCopySettingsDelegate);
            this.Subscribe<ModifiedRefrigerator>(-1697596308, ModifiedRefrigerator.UpdateLogicCircuitCBDelegate);
            this.Subscribe<ModifiedRefrigerator>(-592767678, ModifiedRefrigerator.UpdateLogicCircuitCBDelegate);
            this.Subscribe<ModifiedRefrigerator>(-801688580, ModifiedRefrigerator.OnLogicValueChangedDelegate);
        }

        private void OnLogicValueChanged(object data)
        {
            LogicValueChanged logicValueChanged = (LogicValueChanged)data;
            if (!(logicValueChanged.portID == ModifiedRefrigerator.PORT_ID))
                return;
            this.SetLogicMeter(LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue));
        }

        public void SetLogicMeter(bool on)
        {
            if (this.logicMeter == null)
                return;
            this.logicMeter.SetPositionPercent(on ? 1f : 0.0f);
        }

        protected override void OnCleanUp() => this.filteredStorage.CleanUp();

        public bool IsActive() => operational.IsActive;

        private void OnCopySettings(object data)
        {
            GameObject gameObject = (GameObject)data;
            if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
                return;
            ModifiedRefrigerator component = gameObject.GetComponent<ModifiedRefrigerator>();
            if ((UnityEngine.Object)component == (UnityEngine.Object)null)
                return;
            UserMaxCapacity = component.UserMaxCapacity;
            activateValue = component.activateValue;
            deactivateValue = component.deactivateValue;
        }

        //private void UpdateLogicCircuitCB(object data) => this.UpdateLogicAndActiveState();

        //private void UpdateLogicAndActiveState()
        //{
        //    int num1 = this.filteredStorage.IsFull() ? 1 : 0;
        //    bool isOperational = this.operational.IsOperational;
        //    int num2 = isOperational ? 1 : 0;
        //    bool on = (num1 & num2) != 0;
        //    this.ports.SendSignal(FilteredStorage.FULL_PORT_ID, on ? 1 : 0);
        //    this.filteredStorage.SetLogicMeter(on);
        //    this.operational.SetActive(isOperational);
        //}

        //public override float UserMaxCapacity
        //{
        //    get => base.UserMaxCapacity;
        //    set
        //    {
        //        base.UserMaxCapacity = value;
        //        this.UpdateLogicAndActiveState(); !!!
        //    }
        //}

        public float UserMaxCapacity
        {
            get => Mathf.Min(this.userMaxCapacity, this.storage.capacityKg);
            set
            {
                this.userMaxCapacity = value;
                this.filteredStorage.FilterChanged();
                this.UpdateLogicCircuit();
            }
        }

        public float AmountStored => this.storage.MassStored();

        public float MinCapacity => 0.0f;

        public float MaxCapacity => this.storage.capacityKg;

        public bool WholeValues => false;

        public LocString CapacityUnits => GameUtil.GetCurrentMassUnit();

        private void UpdateLogicCircuitCB(object data) => this.UpdateLogicCircuit();

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

            bool on = activated & operational.IsOperational;
            ports.SendSignal(FilteredStorage.FULL_PORT_ID, on ? 1 : 0);
            filteredStorage.SetLogicMeter(!on & operational.IsOperational);
            operational.SetActive(operational.IsOperational);
        }

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
        public float PercentFull { get => AmountStored / UserMaxCapacity; }

    }

}
