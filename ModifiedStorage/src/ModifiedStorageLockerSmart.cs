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
    [SerializationConfig(MemberSerialization.OptIn)]
    class ModifiedStorageLockerSmart : StorageLocker, IActivationRangeTarget
    {
        [MyCmpGet]
        private LogicPorts ports;
        [MyCmpGet]
        private Operational operational;
        private static readonly EventSystem.IntraObjectHandler<ModifiedStorageLockerSmart> UpdateLogicCircuitCBDelegate = new EventSystem.IntraObjectHandler<ModifiedStorageLockerSmart>((System.Action<ModifiedStorageLockerSmart, object>)((component, data) => component.UpdateLogicCircuitCB(data)));

        [Serialize]
        private int activateValue = 0;
        [Serialize]
        private int deactivateValue = 100;
        [Serialize]
        private bool activated;

        protected override void OnPrefabInit() => this.Initialize(true);

        protected override void OnSpawn()
        {
            base.OnSpawn();
            this.ports = this.gameObject.GetComponent<LogicPorts>();
            this.Subscribe<ModifiedStorageLockerSmart>(-1697596308, ModifiedStorageLockerSmart.UpdateLogicCircuitCBDelegate);
            this.Subscribe<ModifiedStorageLockerSmart>(-592767678, ModifiedStorageLockerSmart.UpdateLogicCircuitCBDelegate);
            this.UpdateLogicAndActiveState();
        }

        private void OnCopySettings(object data)
        {
            GameObject gameObject = (GameObject)data;
            if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
                return;
            ModifiedStorageLockerSmart component = gameObject.GetComponent<ModifiedStorageLockerSmart>();
            if ((UnityEngine.Object)component == (UnityEngine.Object)null)
                return;
            this.UserMaxCapacity = component.UserMaxCapacity;
            activateValue = component.activateValue;
            deactivateValue = component.deactivateValue;
        }

        private void UpdateLogicCircuitCB(object data) => this.UpdateLogicAndActiveState();

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

            int num1 = activated ? 1 : 0;
            bool isOperational = this.operational.IsOperational;
            int num2 = isOperational ? 1 : 0;
            bool on = (num1 & num2) != 0;
            this.ports.SendSignal(FilteredStorage.FULL_PORT_ID, on ? 1 : 0);
            this.filteredStorage.SetLogicMeter(!on & isOperational);
            this.operational.SetActive(isOperational);
        }

        public override float UserMaxCapacity
        {
            get => base.UserMaxCapacity;
            set
            {
                base.UserMaxCapacity = value;
                this.UpdateLogicAndActiveState();
            }
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
        public float PercentFull { get => GetComponent<Storage>().MassStored() / UserMaxCapacity; }

    }

}
