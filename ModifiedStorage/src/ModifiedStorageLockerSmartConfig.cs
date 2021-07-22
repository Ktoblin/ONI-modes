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
    class ModifiedStorageLockerSmartConfig : IBuildingConfig
    {
        public const string ID = "Ktoblin.ModifiedStorageLockerSmart";

        public override BuildingDef CreateBuildingDef()
        {
            float[] tieR3 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
            string[] refinedMetals = MATERIALS.REFINED_METALS;
            EffectorValues none = TUNING.NOISE_POLLUTION.NONE;
            EffectorValues tieR1 = TUNING.BUILDINGS.DECOR.PENALTY.TIER1;
            EffectorValues noise = none;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "modsmartstoragelocker_kanim", 30, 60f, tieR3, refinedMetals, 1600f, BuildLocationRule.OnFloor, tieR1, noise);
            buildingDef.Floodable = false;
            buildingDef.AudioCategory = "Metal";
            buildingDef.Overheatable = false;
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 20f;
            buildingDef.ExhaustKilowattsWhenActive = 0.125f;
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
                {
                  LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 1), (string) ModifiedStorageLockerSmart.LOGIC_PORT, (string) ModifiedStorageLockerSmart.LOGIC_PORT_ACTIVE, (string) ModifiedStorageLockerSmart.LOGIC_PORT_INACTIVE, true)
                };
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            SoundEventVolumeCache.instance.AddVolume("storagelocker_kanim", "StorageLocker_Hit_metallic_low", TUNING.NOISE_POLLUTION.NOISY.TIER1);
            Prioritizable.AddRef(go);
            Storage storage = go.AddOrGet<Storage>();
            storage.showInUI = true;
            storage.allowItemRemoval = true;
            storage.showDescriptor = true;
            storage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
            storage.storageFullMargin = TUNING.STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
            storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
            storage.showCapacityStatusItem = true;
            storage.showCapacityAsMainStatus = true;
            go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.StorageLocker;
            go.AddOrGet<ModifiedStorageLockerSmart>();
            go.AddOrGet<UserNameable>();
            go.AddOrGetDef<StorageController.Def>();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            BuildingTemplates.DoPostConfigure(go);
        }

    }

}
