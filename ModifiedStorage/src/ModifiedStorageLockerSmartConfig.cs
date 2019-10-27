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
        public override BuildingDef CreateBuildingDef()
        {
            string id = "ModifiedStorageLockerSmart";
            int width = 1;
            int height = 2;
            string anim = "modsmartstoragelocker_kanim";
            int hitpoints = 30;
            float construction_time = 60f;
            float[] tier = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
            string[] refined_METALS = MATERIALS.REFINED_METALS;
            float melting_point = 1600f;
            BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
            EffectorValues none = NOISE_POLLUTION.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, refined_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, none, 0.2f);
            buildingDef.Floodable = false;
            buildingDef.AudioCategory = "Metal";
            buildingDef.Overheatable = false;
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 20f;
            buildingDef.ExhaustKilowattsWhenActive = 0.125f;
            return buildingDef;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
            SoundEventVolumeCache.instance.AddVolume("storagelocker_kanim", "StorageLocker_Hit_metallic_low", NOISE_POLLUTION.NOISY.TIER1);
            Prioritizable.AddRef(go);
            Storage storage = go.AddOrGet<Storage>();
            storage.showInUI = true;
            storage.allowItemRemoval = true;
            storage.showDescriptor = true;
            storage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
            storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
            storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
            //CopyBuildingSettings copyBuildingSettings = go.AddOrGet<CopyBuildingSettings>();
            //copyBuildingSettings.copyGroupTag = GameTags.StorageLocker;
            go.AddOrGet<ModifiedStorageLockerSmart>();
            go.AddOrGetDef<StorageController.Def>();
            go.AddOrGet<LogicOperationalController>();
        }

        public const string ID = "ModifiedStorageLockerSmart";

        private static readonly LogicPorts.Port OUTPUT_PORT = LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 1),
                                                                ModifiedStorageLockerSmart.LOGIC_PORT,
                                                                ModifiedStorageLockerSmart.LOGIC_PORT_ACTIVE,
                                                                ModifiedStorageLockerSmart.LOGIC_PORT_INACTIVE, true, false);

    }

}
