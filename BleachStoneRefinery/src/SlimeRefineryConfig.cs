using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUNING;
using UnityEngine;

namespace BleachStoneRefinery
{
    class SlimeRefineryConfig : IBuildingConfig
    {
        public override BuildingDef CreateBuildingDef()
        {
            string id = "SlimeRefinery";
            int width = 3;
            int height = 4;
            string anim = "oxylite_refinery_kanim";
            int hitpoints = 100;
            float construction_time = 480f;
            string[] construction_materials = new string[]
            {
            "RefinedMetal",
            "Plastic"
            };
            EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, new float[]
            {
            BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0],
            BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0]
            }, construction_materials, 2400f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, tier, 0.2f);
            buildingDef.Overheatable = false;
            buildingDef.RequiresPowerInput = true;
            buildingDef.PowerInputOffset = new CellOffset(0, 0);
            buildingDef.EnergyConsumptionWhenActive = 1200f;
            buildingDef.ExhaustKilowattsWhenActive = 8f;
            buildingDef.SelfHeatKilowattsWhenActive = 4f;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.InputConduitType = ConduitType.Gas;
            buildingDef.UtilityInputOffset = new CellOffset(1, 0);
            buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            Tag tag = SimHashes.ContaminatedOxygen.CreateTag();
            Tag tag2 = ElementLoader.FindElementByHash(SimHashes.Dirt).tag;
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
            OxyliteRefinery oxyliteRefinery = go.AddOrGet<OxyliteRefinery>();
            oxyliteRefinery.emitTag = SimHashes.SlimeMold.CreateTag();
            oxyliteRefinery.emitMass = 10f;
            oxyliteRefinery.dropOffset = new Vector3(0f, 1f);
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Gas;
            conduitConsumer.consumptionRate = 1.2f;
            conduitConsumer.capacityTag = tag;
            conduitConsumer.capacityKG = 6f;
            conduitConsumer.forceAlwaysSatisfied = true;
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = 232f;
            storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
            storage.showInUI = true;
            ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
            manualDeliveryKG.SetStorage(storage);
            manualDeliveryKG.requestedItemTag = tag2;
            manualDeliveryKG.refillMass = 1.8000001f;
            manualDeliveryKG.capacity = 72.00003f;
            manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
            ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
            elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
            {
                new ElementConverter.ConsumedElement(tag, 0.6f),
                new ElementConverter.ConsumedElement(tag2, 0.03f)
            };
            elementConverter.outputElements = new ElementConverter.OutputElement[]
            {
                new ElementConverter.OutputElement(0.6f, SimHashes.SlimeMold, 303.15f, false, true, 0f, 0.5f, 1f, byte.MaxValue, 0)
            };
            Prioritizable.AddRef(go);
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGetDef<PoweredActiveController.Def>();
        }

        public const string ID = "SlimeRefinery";
        public const float EMIT_MASS = 10f;
        public const float INPUT_O2_PER_SECOND = 0.6f;
        public const float OXYLITE_PER_SECOND = 0.6f;
        public const float GOLD_PER_SECOND = 0.03f;
        public const float OUTPUT_TEMP = 303.15f;
        public const float REFILL_RATE = 2400f;
        public const float GOLD_STORAGE_AMOUNT = 72.000003f;
        public const float O2_STORAGE_AMOUNT = 6f;
        public const float STORAGE_CAPACITY = 232f;
    }
}
