using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SolidSensors
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public partial class AdvancedSolidConduitTemperatureSensor : KMonoBehaviour, IThresholdSwitch
    {

        [MyCmpGet] private LogicPorts ports;
        [MyCmpAdd] private CopyBuildingSettings copyBuildingSettings;

        [Serialize] private bool dirty = true;
        [Serialize] private bool activated = false;
        [Serialize] private float lastValue;

        [SerializeField]
        [Serialize]
        protected float threshold;

        [SerializeField]
        [Serialize]
        protected bool activateAboveThreshold = true;

        protected bool wasOn = false;
        protected KBatchedAnimController animController;

        public float rangeMin = 0f;
        public float rangeMax = 373.15f;


        protected override void OnSpawn()
        {
            base.OnSpawn();
            Subscribe((int)GameHashes.CopySettings, new Action<object>(OnCopySettings)); // -905833192
            SolidConduit.GetFlowManager().AddConduitUpdater(ConduitUpdate);
            animController = GetComponent<KBatchedAnimController>();
            OnSwitchToggled();
        }

        protected override void OnCleanUp()
        {
            SolidConduit.GetFlowManager().RemoveConduitUpdater(ConduitUpdate);
            base.OnCleanUp();
        }

        private void OnCopySettings(object data)
        {
            GameObject gameObject = (GameObject)data;
            if (gameObject == null)
            {
                return;
            }
            AdvancedSolidConduitTemperatureSensor component = gameObject.GetComponent<AdvancedSolidConduitTemperatureSensor>();
            if (component == null)
            {
                return;
            }
            Threshold = component.Threshold;
            ActivateAboveThreshold = component.ActivateAboveThreshold;
        }

        private SolidConduitFlow GetConduitFlow()
        {
            return Game.Instance.solidConduitFlow;
        }

        protected void ConduitUpdate(float dt)
        {
            if (CurrentValue == 0f)
            {
                activated = false;
            }
            else
            {
                if (ActivateAboveThreshold)
                {
                    activated = CurrentValue > Threshold;
                }
                else
                {
                    activated = CurrentValue <= Threshold;
                }
            }

            if (wasOn != activated)
            {
                OnSwitchToggled();
            }
        }

        private void OnSwitchToggled()
        {
            UpdateLogicCircuit();
            UpdateVisualState(false);
        }

        private void UpdateLogicCircuit()
        {
            ports.SendSignal(LogicSwitch.PORT_ID, !activated ? 0 : 1);
        }

        protected virtual void UpdateVisualState(bool force = false)
        {
            if (this.wasOn != this.activated || force)
            {
                this.wasOn = this.activated;
                if (this.activated)
                {
                    this.animController.Play(ON_ANIMS, KAnim.PlayMode.Loop);
                }
                else
                {
                    this.animController.Play(OFF_ANIMS, KAnim.PlayMode.Once);
                }
            }
        }

        protected static readonly HashedString[] ON_ANIMS = new HashedString[]
        {
        "on_pre",
        "on"
        };

        protected static readonly HashedString[] OFF_ANIMS = new HashedString[]
        {
        "on_pst",
        "off"
        };

        public float Threshold { get => threshold; set { threshold = value; dirty = true; } }
        public bool ActivateAboveThreshold { get => activateAboveThreshold; set { activateAboveThreshold = value; dirty = true; } }
        public float CurrentValue => GetCurrentValue();
        public float RangeMin => rangeMin;
        public float RangeMax => rangeMax;
        public LocString Title => UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.TITLE;
        public LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE;
        public string AboveToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE_TOOLTIP_ABOVE;
        public string BelowToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE_TOOLTIP_BELOW;
        public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;
        public int IncrementScale => 1;
        public NonLinearSlider.Range[] GetRanges => new NonLinearSlider.Range[]
            {
                new NonLinearSlider.Range(25f, 260f),
                new NonLinearSlider.Range(50f, 400f),
                new NonLinearSlider.Range(12f, 1500f),
                new NonLinearSlider.Range(13f, 10000f)
            };

        public float GetCurrentValue()
        {
            int cell = Grid.PosToCell(base.transform.GetPosition());
            SolidConduitFlow conduitFlow = GetConduitFlow();
            SolidConduitFlow.ConduitContents contents = conduitFlow.GetContents(cell);
            Pickupable pickupable = conduitFlow.GetPickupable(contents.pickupableHandle);

            if (!(bool)pickupable)
                return 0f;

            lastValue = pickupable.PrimaryElement.Temperature;
            return lastValue;
        }

        public float GetRangeMinInputField()
        {
            return GameUtil.GetConvertedTemperature(RangeMin, false);
        }

        public float GetRangeMaxInputField()
        {
            return GameUtil.GetConvertedTemperature(RangeMax, false);
        }

        public LocString ThresholdValueUnits()
        {
            LocString result = null;
            GameUtil.TemperatureUnit temperatureUnit = GameUtil.temperatureUnit;
            if (temperatureUnit != GameUtil.TemperatureUnit.Celsius)
            {
                if (temperatureUnit != GameUtil.TemperatureUnit.Fahrenheit)
                {
                    if (temperatureUnit == GameUtil.TemperatureUnit.Kelvin)
                    {
                        result = UI.UNITSUFFIXES.TEMPERATURE.KELVIN;
                    }
                }
                else
                {
                    result = UI.UNITSUFFIXES.TEMPERATURE.FAHRENHEIT;
                }
            }
            else
            {
                result = UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;
            }
            return result;
        }

        public string Format(float value, bool units)
        {
            return GameUtil.GetFormattedTemperature(value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, units, false);
        }

        public float ProcessedSliderValue(float input)
        {
            return Mathf.Round(input);
        }

        public float ProcessedInputValue(float input)
        {
            return GameUtil.GetTemperatureConvertedToKelvin(input);
        }

    }
}
