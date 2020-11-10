using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

using UnityEngine;
using Database;
using TUNING;
using STRINGS;
using KSerialization;
using Klei.AI;

namespace SolidSensors
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public partial class AdvancedSolidConduitDiseaseSensor : KMonoBehaviour, IThresholdSwitch
    {

        [MyCmpGet] private LogicPorts ports;
        [MyCmpAdd] private CopyBuildingSettings copyBuildingSettings;

        [Serialize] private bool activated = false;
        [Serialize] private float lastValue;
        [Serialize] private bool dirty = true;

        [SerializeField]
        [Serialize]
        protected float threshold;

        [SerializeField]
        [Serialize]
        protected bool activateAboveThreshold = true;

        protected bool wasOn = false;
        protected bool empty = true;
        protected KBatchedAnimController animController;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Subscribe((int)GameHashes.CopySettings, new Action<object>(OnCopySettings)); // -905833192
            SolidConduit.GetFlowManager().AddConduitUpdater(ConduitUpdate);
            animController = GetComponent<KBatchedAnimController>();
            OnSwitchToggled();
        }

        private void OnCopySettings(object data)
        {
            GameObject gameObject = (GameObject)data;
            if (gameObject == null)
            {
                return;
            }
            AdvancedSolidConduitDiseaseSensor component = gameObject.GetComponent<AdvancedSolidConduitDiseaseSensor>();
            if (component == null)
            {
                return;
            }
            Threshold = component.Threshold;
            ActivateAboveThreshold = component.ActivateAboveThreshold;
        }

        protected override void OnCleanUp()
        {
            SolidConduit.GetFlowManager().RemoveConduitUpdater(ConduitUpdate);
            base.OnCleanUp();
        }

        private SolidConduitFlow GetConduitFlow()
        {
            return Game.Instance.solidConduitFlow;
        }

        protected void ConduitUpdate(float dt)
        {
            float cv = CurrentValue;

            if (empty)
            {
                activated = false;
            }
            else
            {
                if (ActivateAboveThreshold)
                {
                    activated = cv > Threshold;
                }
                else
                {
                    activated = cv < Threshold;
                }
            }

            if (wasOn != activated)
            {
                OnSwitchToggled();
            }
        }

        private void OnSwitchToggled()
        {
            this.UpdateLogicCircuit();
            this.UpdateVisualState(false);
        }

        private void UpdateLogicCircuit()
        {
            ports.SendSignal(LogicSwitch.PORT_ID, !activated ? 0 : 1);
        }
        #region Visual
        protected virtual void UpdateVisualState(bool force = false)
        {
            if (this.wasOn != this.activated || force)
            {
                this.wasOn = this.activated;
                if (this.activated)
                {
                    animController.Play(ON_ANIMS, KAnim.PlayMode.Loop);
                    int cell = Grid.PosToCell(base.transform.GetPosition());
                    SolidConduitFlow conduitFlow = GetConduitFlow();
                    SolidConduitFlow.ConduitContents contents = conduitFlow.GetContents(cell);
                    Pickupable pickupable = conduitFlow.GetPickupable(contents.pickupableHandle);
                    Color32 c = Color.white;
                    if (pickupable.PrimaryElement.DiseaseIdx != 255)
                    {
                        Disease disease = Db.Get().Diseases[(int)pickupable.PrimaryElement.DiseaseIdx];
                        c = disease.overlayColour;
                    }
                    animController.SetSymbolTint(TINT_SYMBOL, c);
                }
                else
                {
                    animController.Play(OFF_ANIMS, KAnim.PlayMode.Once);
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

        private static readonly HashedString TINT_SYMBOL = "germs";
        #endregion

        #region Desease
        public float Threshold { get => threshold; set { threshold = value; dirty = true; } }
        public bool ActivateAboveThreshold { get => activateAboveThreshold; set { activateAboveThreshold = value; dirty = true; } }
        public float CurrentValue => GetCurrentValue();
        public float RangeMin => 0f;
        public float RangeMax => 100000f;
        public LocString Title => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TITLE;
        public LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE;
        public string AboveToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TOOLTIP_ABOVE;
        public string BelowToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TOOLTIP_BELOW;
        public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;
        public int IncrementScale => 1;
        public NonLinearSlider.Range[] GetRanges => new NonLinearSlider.Range[]
		{
			new NonLinearSlider.Range(100f, RangeMax)
		};

        public float GetCurrentValue()
        {
            int cell = Grid.PosToCell(base.transform.GetPosition());
            SolidConduitFlow conduitFlow = GetConduitFlow();
            SolidConduitFlow.ConduitContents contents = conduitFlow.GetContents(cell);
            Pickupable pickupable = conduitFlow.GetPickupable(contents.pickupableHandle);

            if (!(bool)pickupable)
            {
                empty = true;
                return 0f;
            }

            empty = false;
            lastValue = pickupable.PrimaryElement.DiseaseCount;
            return lastValue;
        }

        public float GetRangeMinInputField()
        {
            return 0f;
        }

        public float GetRangeMaxInputField()
        {
            return 100000f;
        }

        public LocString ThresholdValueUnits()
        {
            return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_UNITS;
        }

        public string Format(float value, bool units)
        {
            return GameUtil.GetFormattedInt((float)((int)value), GameUtil.TimeSlice.None);
        }

        public float ProcessedSliderValue(float input)
        {
            return input;
        }

        public float ProcessedInputValue(float input)
        {
            return input;
        }
        #endregion
    }
}


