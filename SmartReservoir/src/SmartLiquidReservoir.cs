using Database;
using Harmony;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SmartReservoir
{
    [HarmonyPatch(typeof(GeneratedBuildings))]
    [HarmonyPatch("LoadGeneratedBuildings")]
    public class SmartLiquidReservoirPatch
    {
        public static LocString NAME = new LocString("Smart Liquid Reservoir",
            "STRINGS.BUILDINGS.PREFABS." + SmartLiquidReservoirConfig.ID.ToUpper() + ".NAME");

        public static LocString DESC = new LocString("Smart reservoirs send a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when they require more liquid.",
            "STRINGS.BUILDINGS.PREFABS." + SmartLiquidReservoirConfig.ID.ToUpper() + ".DESC");

        public static LocString EFFECT = new LocString("Stores any " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " resources piped into it.\n\nSends a " +
                                                        UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or " +
                                                        UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) +
                                                        " based on the configuration of the Logic Activation Parameters.",
            "STRINGS.BUILDINGS.PREFABS." + SmartLiquidReservoirConfig.ID.ToUpper() + ".EFFECT");

        static void Prefix()
        {
            Strings.Add(NAME.key.String, NAME.text);
            Strings.Add(DESC.key.String, DESC.text);
            Strings.Add(EFFECT.key.String, EFFECT.text);
            ModUtil.AddBuildingToPlanScreen("Base", SmartLiquidReservoirConfig.ID);
        }

        static void Postfix()
        {
            object obj = Activator.CreateInstance(typeof(SmartLiquidReservoirConfig));
            BuildingConfigManager.Instance.RegisterBuilding(obj as IBuildingConfig);
        }


    }

}


