using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using STRINGS;

namespace ModifiedStorage
{
    [HarmonyPatch(typeof(GeneratedBuildings))]
    [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
    public class ModifiedStorageBuildingsPatch
    {
        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        private static void Prefix()
        {
            CaiLib.Utils.StringUtils.AddBuildingStrings(
                ModifiedRefrigeratorConfig.ID,
                "Modified Refrigerator",
                "Food spoilage can be slowed by ambient conditions as well as by refrigerators.\nSend a " +
                                                    UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when mass of food reached High Threshold." +
                                                    "And send a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when mass of food below Low Threshold.",
                "Stores " + UI.FormatAsLink("Food", "FOOD") + ".\n\nSends a " +
                                                        UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or " +
                                                        UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) +
                                                        " based on the configuration of the Logic Activation Parameters."
            );
            CaiLib.Utils.StringUtils.AddBuildingStrings(
                ModifiedStorageLockerSmartConfig.ID,
                "Modified Smart Storage Bin",
                "Smart storage bins allow for the automation of resource organization based on type and mass.\nSend a " +
                                                    UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when mass of solid materials reached High Threshold." +
                                                    "And send a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when mass of solid materials below Low Threshold.",
                "Stores the Solid resources of your choosing.\n\nSends a " +
                                                        UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or " +
                                                        UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) +
                                                        " based on the configuration of the Logic Activation Parameters."
            );

            ModUtil.AddBuildingToPlanScreen("Food", ModifiedRefrigeratorConfig.ID);
            ModUtil.AddBuildingToPlanScreen("Base", ModifiedStorageLockerSmartConfig.ID);
        }
    }

    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch("Initialize")]
    public class ModifiedStorageDbPatch
    {
        private const string TechID1 = "Agriculture";
        private const string TechID2 = "SmartStorage";

        private static void Postfix()
        {

            if (typeof(Database.Techs).GetField("TECH_GROUPING") == null)
            {
                Tech tech = Db.Get().Techs.TryGet(TechID1);
                if (tech == null)
                    return;
                ICollection<string> list = (ICollection<string>)tech.GetType().GetField("unlockedItemIDs")?.GetValue(tech);
                if (list == null)
                    return;

                list.Add(ModifiedRefrigeratorConfig.ID);

                tech = Db.Get().Techs.TryGet(TechID2);
                if (tech == null)
                    return;
                list = (ICollection<string>)tech.GetType().GetField("unlockedItemIDs")?.GetValue(tech);
                if (list == null)
                    return;

                list.Add(ModifiedStorageLockerSmartConfig.ID);
            }
            else
            {

                System.Reflection.FieldInfo info = typeof(Database.Techs).GetField("TECH_GROUPING");
                Dictionary<string, string[]> dict = (Dictionary<string, string[]>)info.GetValue(null);
                dict[TechID1].Append(ModifiedRefrigeratorConfig.ID);
                dict[TechID2].Append(ModifiedStorageLockerSmartConfig.ID);
                typeof(Database.Techs).GetField("TECH_GROUPING").SetValue(null, dict);
            }
        }
    }
}
