using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUNING;
using UnityEngine;

namespace ModifiedStorage
{
    [SerializationConfig(MemberSerialization.OptIn)]
    class ModifiedRefrigeratorConfig : IBuildingConfig
    {
        public const string ID = "Ktoblin.ModifiedRefrigerator";
        private const int ENERGY_SAVER_POWER = 20;

        public override BuildingDef CreateBuildingDef()
        {
            float[] tieR4 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
            string[] rawMinerals = MATERIALS.RAW_MINERALS;
            EffectorValues tieR0 = TUNING.NOISE_POLLUTION.NOISY.TIER0;
            EffectorValues tieR1 = TUNING.BUILDINGS.DECOR.BONUS.TIER1;
            EffectorValues noise = tieR0;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "modfridge_kanim", 30, 10f, tieR4, rawMinerals, 800f, BuildLocationRule.OnFloor, tieR1, noise);
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 60f;
            buildingDef.ExhaustKilowattsWhenActive = 0.5f;
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
                {
                  LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 1), 
                                            (string) ModifiedRefrigerator.LOGIC_PORT,  
                                            (string) ModifiedRefrigerator.LOGIC_PORT_ACTIVE, 
                                            (string) ModifiedRefrigerator.LOGIC_PORT_INACTIVE)
                };
            buildingDef.Floodable = false;
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "Metal";
            SoundEventVolumeCache.instance.AddVolume("fridge_kanim", "Refrigerator_open", TUNING.NOISE_POLLUTION.NOISY.TIER1);
            SoundEventVolumeCache.instance.AddVolume("fridge_kanim", "Refrigerator_close", TUNING.NOISE_POLLUTION.NOISY.TIER1);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.showInUI = true;
            storage.showDescriptor = true;
            storage.storageFilters = STORAGEFILTERS.FOOD;
            storage.allowItemRemoval = true;
            storage.capacityKg = 100f;
            storage.storageFullMargin = TUNING.STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
            storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
            storage.showCapacityStatusItem = true;
            Prioritizable.AddRef(go);
            go.AddOrGet<TreeFilterable>();
            go.AddOrGet<ModifiedRefrigerator>();
            go.AddOrGetDef<RefrigeratorController.Def>().powerSaverEnergyUsage = 20f;
            go.AddOrGet<UserNameable>();
            go.AddOrGet<DropAllWorkable>();
            go.AddOrGetDef<StorageController.Def>();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            BuildingTemplates.DoPostConfigure(go);
        }
    }

}
