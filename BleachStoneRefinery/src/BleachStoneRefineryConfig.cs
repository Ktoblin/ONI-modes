using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUNING;
using UnityEngine;

namespace BleachStoneRefinery
{
    class BleachStoneRefineryConfig : IBuildingConfig
    {
        public const string ID = "Ktoblin.BleachStoneRefinery";
        public const float EMIT_MASS = 10f;
        public const float INPUT_O2_PER_SECOND = 0.6f;
        public const float OXYLITE_PER_SECOND = 0.6f;
        public const float GOLD_PER_SECOND = 0.03f;
        public const float OUTPUT_TEMP = 303.15f;
        public const float REFILL_RATE = 2400f;
        public const float GOLD_STORAGE_AMOUNT = 72.000003f;
        public const float O2_STORAGE_AMOUNT = 6f;
        public const float STORAGE_CAPACITY = 232f;

        public override BuildingDef CreateBuildingDef()
        {
            string[] strArray = new string[2]
            {
              "RefinedMetal",
              "Plastic"
            };
            float[] construction_mass = new float[2]
            {
              BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0],
              BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0]
            };
            string[] construction_materials = strArray;
            EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
            EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
            EffectorValues noise = tieR5;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 4, "oxylite_refinery_kanim", 100, 480f, construction_mass, construction_materials, 2400f, BuildLocationRule.OnFloor, tieR1, noise);
            buildingDef.Overheatable = false;
            buildingDef.RequiresPowerInput = true;
            buildingDef.PowerInputOffset = new CellOffset(0, 0);
            buildingDef.EnergyConsumptionWhenActive = 1200f;
            buildingDef.ExhaustKilowattsWhenActive = 8f;
            buildingDef.SelfHeatKilowattsWhenActive = 4f;
            buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.InputConduitType = ConduitType.Gas;
            buildingDef.UtilityInputOffset = new CellOffset(1, 0);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            Tag tag1 = SimHashes.ChlorineGas.CreateTag();
            Tag tag2 = SimHashes.Sand.CreateTag();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            OxyliteRefinery oxyliteRefinery = go.AddOrGet<OxyliteRefinery>();
            oxyliteRefinery.emitTag = SimHashes.BleachStone.CreateTag();
            oxyliteRefinery.emitMass = 10f;
            oxyliteRefinery.dropOffset = new Vector3(0.0f, 1f);
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Gas;
            conduitConsumer.consumptionRate = 1.2f;
            conduitConsumer.capacityTag = tag1;
            conduitConsumer.capacityKG = 6f;
            conduitConsumer.forceAlwaysSatisfied = true;
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = 232f;
            storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
            storage.showInUI = true;
            ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
            manualDeliveryKg.SetStorage(storage);
            manualDeliveryKg.requestedItemTag = tag2;
            manualDeliveryKg.refillMass = 1.8000001f;
            manualDeliveryKg.capacity = 72.00003f;
            manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
            ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
            elementConverter.consumedElements = new ElementConverter.ConsumedElement[2]
            {
              new ElementConverter.ConsumedElement(tag1, 0.6f),
              new ElementConverter.ConsumedElement(tag2, 3f / 1000f)
            };
            elementConverter.outputElements = new ElementConverter.OutputElement[1]
            {
              new ElementConverter.OutputElement(0.6f, SimHashes.BleachStone, 303.15f, storeOutput: true)
            };
            Prioritizable.AddRef(go);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGetDef<PoweredActiveController.Def>();
        }
    }
}
