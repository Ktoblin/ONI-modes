using Database;
using HarmonyLib;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SmartReservoir
{
    [SerializationConfig(MemberSerialization.OptIn)]
    class SmartReservoir : KMonoBehaviour, IActivationRangeTarget, ISim200ms, IUserControlledCapacity
    {
        [MyCmpGet]
        private Storage storage;
        [MyCmpGet]
        private Operational operational;
        [Serialize]
        private int activateValue;
        [Serialize]
        private int deactivateValue = 100;
        [Serialize]
        private bool activated;
        [Serialize]
        private float userMaxCapacity = 100000f;
        [MyCmpGet]
        private LogicPorts logicPorts;
        [MyCmpAdd]
        private CopyBuildingSettings copyBuildingSettings;
        private MeterController logicMeter;
        public static readonly HashedString PORT_ID = (HashedString)"SmartReservoirLogicPort";
        private static readonly EventSystem.IntraObjectHandler<SmartReservoir> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<SmartReservoir>((component, data) => component.OnCopySettings(data));
        private static readonly EventSystem.IntraObjectHandler<SmartReservoir> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<SmartReservoir>((component, data) => component.OnLogicValueChanged(data));
        private static readonly EventSystem.IntraObjectHandler<SmartReservoir> UpdateLogicCircuitDelegate = new EventSystem.IntraObjectHandler<SmartReservoir>((component, data) => component.UpdateLogicCircuit(data));

        public float PercentFull => AmountStored / UserMaxCapacity;

        public float UserMaxCapacity
        {
            get => Mathf.Min(userMaxCapacity, MaxCapacity);
            set
            {
                Debug.Log("Capacity: " + value);
                userMaxCapacity = value;
                storage.capacityKg = userMaxCapacity;
                UpdateLogicCircuit((object)null);
            }
        }

        public float AmountStored => storage.MassStored();
        public float MinCapacity => 0.0f;
        public float MaxCapacity => 100000f;// storage.capacityKg;
        public bool WholeValues => false;
        public LocString CapacityUnits => GameUtil.GetCurrentMassUnit();

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Subscribe(-801688580, OnLogicValueChangedDelegate);
            Subscribe(-592767678, UpdateLogicCircuitDelegate);
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Subscribe(-905833192, OnCopySettingsDelegate);
        }

        public void Sim200ms(float dt) => UpdateLogicCircuit((object)null);

        private void UpdateLogicCircuit(object data)
        {
            storage.capacityKg = userMaxCapacity; // incredible thing but it works
            float num = PercentFull * 100f;
            if (this.activated)
            {
                if ((double)num >= (double)deactivateValue)
                    this.activated = false;
            }
            else if ((double)num <= (double)activateValue)
                this.activated = true;
            bool activated = this.activated;
            logicPorts.SendSignal(PORT_ID, activated ? 1 : 0);
        }

        private void OnLogicValueChanged(object data)
        {
            LogicValueChanged logicValueChanged = (LogicValueChanged)data;
            if (!(logicValueChanged.portID == PORT_ID))
                return;
            SetLogicMeter(LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue));
        }

        private void OnCopySettings(object data)
        {
            SmartReservoir component = ((GameObject)data).GetComponent<SmartReservoir>();
            if (!(component != null))
                return;
            ActivateValue = component.ActivateValue;
            DeactivateValue = component.DeactivateValue;
            userMaxCapacity = component.storage.capacityKg;
        }

        public void SetLogicMeter(bool on)
        {
            if (logicMeter == null)
                return;
            logicMeter.SetPositionPercent(on ? 1f : 0.0f);
        }

        public float ActivateValue
        {
            get => (float)deactivateValue;
            set
            {
                deactivateValue = (int)value;
                UpdateLogicCircuit((object)null);
            }
        }

        public float DeactivateValue
        {
            get => (float)activateValue;
            set
            {
                activateValue = (int)value;
                UpdateLogicCircuit((object)null);
            }
        }

        public float MinValue => 0.0f;
        public float MaxValue => 100f;
        public bool UseWholeNumbers => false;
        public string ActivateTooltip => (string)BUILDINGS.PREFABS.SMARTRESERVOIR.DEACTIVATE_TOOLTIP;
        public string DeactivateTooltip => (string)BUILDINGS.PREFABS.SMARTRESERVOIR.ACTIVATE_TOOLTIP;
        public string ActivationRangeTitleText => (string)BUILDINGS.PREFABS.SMARTRESERVOIR.SIDESCREEN_TITLE;
        public string ActivateSliderLabelText => (string)BUILDINGS.PREFABS.SMARTRESERVOIR.SIDESCREEN_DEACTIVATE;
        public string DeactivateSliderLabelText => (string)BUILDINGS.PREFABS.SMARTRESERVOIR.SIDESCREEN_ACTIVATE;
    }
}