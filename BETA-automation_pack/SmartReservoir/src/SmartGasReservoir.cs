using Harmony;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartReservoir
{
    [HarmonyPatch(typeof(GeneratedBuildings))]
    [HarmonyPatch("LoadGeneratedBuildings")]
    class SmartGasReservoirPatch
    {
        public static LocString NAME = new LocString("Smart Gas Reservoir",
            "STRINGS.BUILDINGS.PREFABS." + SmartGasReservoirConfig.ID.ToUpper() + ".NAME");

        public static LocString DESC = new LocString("Smart reservoirs send a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when they require more gas.",
            "STRINGS.BUILDINGS.PREFABS." + SmartGasReservoirConfig.ID.ToUpper() + ".DESC");

        public static LocString EFFECT = new LocString("Stores any " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " resources piped into it.\n\nSends a " +
                                                        UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or " +
                                                        UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) +
                                                        " based on the configuration of the Logic Activation Parameters.",
            "STRINGS.BUILDINGS.PREFABS." + SmartGasReservoirConfig.ID.ToUpper() + ".EFFECT");

        static void Prefix()
        {
            Strings.Add(NAME.key.String, NAME.text);
            Strings.Add(DESC.key.String, DESC.text);
            Strings.Add(EFFECT.key.String, EFFECT.text);
            ModUtil.AddBuildingToPlanScreen("Base", SmartGasReservoirConfig.ID);
        }

        static void Postfix()
        {
            object obj = Activator.CreateInstance(typeof(SmartGasReservoirConfig));
            BuildingConfigManager.Instance.RegisterBuilding(obj as IBuildingConfig);
        }


    }
}
