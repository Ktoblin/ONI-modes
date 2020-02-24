using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUNING;
using UnityEngine;

namespace SolidSensors
{
    class SolidConduitElementSensorConfig : IBuildingConfig
    {
        public const string ID = "SolidConduitElementSensor";

        public override BuildingDef CreateBuildingDef()
        {
            string anim = "solid_element_sensor_kanim";
            float[] construction_mass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
            string[] construction_materials = MATERIALS.REFINED_METALS;
            int width = 1;
            int height = 1;
            int hitpoints = 30;
            float construction_time = 30f;
            float melting_point = 1600f;
            BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
            EffectorValues none = NOISE_POLLUTION.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, hitpoints, construction_time, construction_mass, construction_materials, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER0, none, 0.2f);
            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.AlwaysOperational = true;
            SoundEventVolumeCache.instance.AddVolume(anim, "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
            SoundEventVolumeCache.instance.AddVolume(anim, "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);

            List<LogicPorts.Port> list = new List<LogicPorts.Port>();
            list.Add(LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0),
                                                SolidConduitElementSensorPatch.LOGIC_PORT,
                                                SolidConduitElementSensorPatch.LOGIC_PORT_ACTIVE,
                                                SolidConduitElementSensorPatch.LOGIC_PORT_INACTIVE, true, false));
            buildingDef.LogicOutputPorts = list;

            return buildingDef;
        }
        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            var component = go.GetComponent<Constructable>();
            component.requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            List<Tag> list = new List<Tag>();
            list.AddRange(STORAGEFILTERS.NOT_EDIBLE_SOLIDS);
            list.AddRange(STORAGEFILTERS.FOOD);
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = 0f;
            storage.showInUI = true;
            storage.showDescriptor = true;
            storage.storageFilters = list;
            storage.allowItemRemoval = false;
            storage.onlyTransferFromLowerPriority = true;
            go.AddOrGet<TreeFilterable>();
            SolidConduitElementSensor conduitElementSensor = go.AddOrGet<SolidConduitElementSensor>();
        }
    }
}

