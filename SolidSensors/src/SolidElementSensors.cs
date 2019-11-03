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

namespace SolidSensors
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public partial class SolidConduitElementSensor : KMonoBehaviour
    {

        [MyCmpReq] private TreeFilterable treeFilterable;
        [MyCmpAdd] private Storage storage;
        [MyCmpGet] private LogicPorts ports;

        [Serialize] private bool activated = false;

        protected bool wasOn = false;
        protected KBatchedAnimController animController;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            var component = GetComponent<Building>();
            SolidConduit.GetFlowManager().AddConduitUpdater(ConduitUpdate);
            animController = GetComponent<KBatchedAnimController>();
            UpdateLogicCircuit();
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
            int cell = Grid.PosToCell(base.transform.GetPosition());
            List<Tag> acceptedTags = treeFilterable.AcceptedTags;

            SolidConduitFlow conduitFlow = this.GetConduitFlow();
            SolidConduitFlow.ConduitContents contents = conduitFlow.GetContents(cell);
            Pickupable pickupable = conduitFlow.GetPickupable(contents.pickupableHandle);

            activated = false;

            if ((bool)pickupable)
            {
                foreach (var acceptedTag in acceptedTags)
                {
                    if (pickupable.HasTag(acceptedTag))
                    {
                        activated = true;
                        break;
                    }
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
    }
}


