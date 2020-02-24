using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace SmartReservoir
{
    class SmartLiquidReservoirConfig : LiquidReservoirConfig
    {
        public new const string ID = "SmartLiquidReservoir";
        private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;
        private const int WIDTH = 2;
        private const int HEIGHT = 3;

        public override BuildingDef CreateBuildingDef()
        {
            string anim = "smartliquidreservoir_kanim";
            int hitpoints = 100;
            float construction_time = 240f;
            float[] construction_mass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
            string[] construction_materials = MATERIALS.ALL_METALS;
            EffectorValues decor = BUILDINGS.DECOR.PENALTY.TIER1;
            EffectorValues pollution = NOISE_POLLUTION.NOISY.TIER0;

            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, WIDTH, HEIGHT, anim, hitpoints, construction_time, 
                                                                        construction_mass, construction_materials, 800f, BuildLocationRule.OnFloor, decor, pollution, 0.2f);
            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.Floodable = false;
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.UtilityInputOffset = new CellOffset(1, 2);
            buildingDef.UtilityOutputOffset = new CellOffset(0, 0);

            List<LogicPorts.Port> list = new List<LogicPorts.Port>();
            list.Add(LogicPorts.Port.OutputPort(SmartReservoir.PORT_ID, new CellOffset(0, 0),
                                                SmartReservoir.LOGIC_PORT, SmartReservoir.LOGIC_PORT_ACTIVE,
                                                SmartReservoir.LOGIC_PORT_INACTIVE, false, false));
            buildingDef.LogicOutputPorts = list;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<SmartReservoir>();
            Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
            storage.showDescriptor = true;
            storage.allowItemRemoval = false;
            storage.storageFilters = STORAGEFILTERS.LIQUIDS;
            storage.capacityKg = Loader.Config.liquidStorage;
            storage.SetDefaultStoredItemModifiers(GasReservoirConfig.ReservoirStoredItemModifiers);
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.ignoreMinMassCheck = true;
            conduitConsumer.forceAlwaysSatisfied = true;
            conduitConsumer.alwaysConsume = true;
            conduitConsumer.capacityKG = storage.capacityKg;
            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Liquid;
            conduitDispenser.elementFilter = null;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGetDef<StorageController.Def>();
        }

        private static readonly LogicPorts.Port[] OUTPUT_PORTS = new LogicPorts.Port[]
        {
            LogicPorts.Port.OutputPort(SmartReservoir.PORT_ID, new CellOffset(1, 0), SmartReservoir.LOGIC_PORT,
                                        SmartReservoir.LOGIC_PORT_ACTIVE, SmartReservoir.LOGIC_PORT_INACTIVE, true, false)
        };

    }
}
