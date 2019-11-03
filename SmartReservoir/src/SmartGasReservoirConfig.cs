using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUNING;
using UnityEngine;

namespace SmartReservoir
{
    class SmartGasReservoirConfig : GasReservoirConfig
    {
        public new const string ID = "SmartGasReservoir";
        private const ConduitType CONDUIT_TYPE = ConduitType.Gas;
        private const int WIDTH = 5;
        private const int HEIGHT = 3;

        public override BuildingDef CreateBuildingDef()
        {
            string anim = "gasstorage_kanim";
            int hitpoints = 100;
            float construction_time = 240f;
            float[] construction_mass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
            string[] construction_materials = MATERIALS.ALL_METALS;
            EffectorValues decor = BUILDINGS.DECOR.PENALTY.TIER1;
            EffectorValues pollution = NOISE_POLLUTION.NOISY.TIER0;

            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, WIDTH, HEIGHT, anim, hitpoints, construction_time, 
                                                                          construction_mass, construction_materials, 800f, BuildLocationRule.OnFloor, decor, pollution, 0.2f);
            buildingDef.InputConduitType = ConduitType.Gas;
            buildingDef.OutputConduitType = ConduitType.Gas;
            buildingDef.Floodable = false;
            buildingDef.ViewMode = OverlayModes.GasConduits.ID;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.UtilityInputOffset = new CellOffset(1, 2);
            buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<SmartReservoir>();
            go.AddOrGet<LogicOperationalController>();
            Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
            storage.showDescriptor = true;
            storage.storageFilters = STORAGEFILTERS.GASES;
            storage.capacityKg = Loader.Config.gasStorage;
            storage.SetDefaultStoredItemModifiers(GasReservoirConfig.ReservoirStoredItemModifiers);
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Gas;
            conduitConsumer.ignoreMinMassCheck = true;
            conduitConsumer.forceAlwaysSatisfied = true;
            conduitConsumer.alwaysConsume = true;
            conduitConsumer.capacityKG = storage.capacityKg;
            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Gas;
            conduitDispenser.elementFilter = null;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORTS);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORTS);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGetDef<StorageController.Def>();
            GeneratedBuildings.RegisterLogicPorts(go, null, OUTPUT_PORTS);
        }

        private static readonly LogicPorts.Port[] OUTPUT_PORTS = new LogicPorts.Port[]
        {
            LogicPorts.Port.OutputPort(SmartReservoir.PORT_ID, new CellOffset(1, 0), SmartReservoir.LOGIC_PORT,
                                        SmartReservoir.LOGIC_PORT_ACTIVE, SmartReservoir.LOGIC_PORT_INACTIVE, true, false)
        };
    }
}
