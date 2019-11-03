﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUNING;
using UnityEngine;

namespace SolidSensors
{
    class SolidConduitTemperatureSensorConfig : IBuildingConfig
    {
        public const string ID = "SolidConduitTemperatureSensor";

        public override BuildingDef CreateBuildingDef()
        {
            string anim = "solid_temperature_sensor_kanim";
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
            SoundEventVolumeCache.instance.AddVolume(anim, "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
            SoundEventVolumeCache.instance.AddVolume(anim, "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
            return buildingDef;
        }
        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
            var component = go.GetComponent<Constructable>();
            component.requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
            SolidConduitTemperatureSensor conduitTemperatureSensor = go.AddOrGet<SolidConduitTemperatureSensor>();
            go.AddOrGet<LogicOperationalController>();
            conduitTemperatureSensor.Threshold = 280f;
            conduitTemperatureSensor.ActivateAboveThreshold = true;
            conduitTemperatureSensor.rangeMin = 0f;
            conduitTemperatureSensor.rangeMax = 9999f;
        }

        public static readonly LogicPorts.Port OUTPUT_PORT = LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0),
                                                                SolidConduitTemperatureSensorPatch.LOGIC_PORT,
                                                                SolidConduitTemperatureSensorPatch.LOGIC_PORT_ACTIVE,
                                                                SolidConduitTemperatureSensorPatch.LOGIC_PORT_INACTIVE, true, false);
    }
}
