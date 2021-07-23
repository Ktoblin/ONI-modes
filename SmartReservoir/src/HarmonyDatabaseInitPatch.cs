using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using STRINGS;
using UnityEngine;

namespace SmartReservoir
{
    [HarmonyPatch(typeof(GeneratedBuildings))]
    [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
    public class ModifiedStorageBuildingsPatch
    {
        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        private static void Prefix()
        {
            CaiLib.Utils.StringUtils.AddBuildingStrings(
                SmartLiquidReservoirConfig.ID,
                "Smart Liquid Reservoir",
                BUILDINGS.PREFABS.LIQUIDRESERVOIR.DESC,//"Reservoirs cannot receive manually delivered resources. Smart reservoir has a variable volume."
                BUILDINGS.PREFABS.LIQUIDRESERVOIR.EFFECT//"Stores any " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " resources piped into it."
            );
            CaiLib.Utils.StringUtils.AddBuildingStrings(
                SmartGasReservoirConfig.ID,
                "Smart Gas Reservoir",
                BUILDINGS.PREFABS.GASRESERVOIR.DESC,//"Reservoirs cannot receive manually delivered resources. Smart reservoir has a variable volume."
                BUILDINGS.PREFABS.GASRESERVOIR.EFFECT//"Stores any " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " resources piped into it."
            );

            ModUtil.AddBuildingToPlanScreen("Base", SmartLiquidReservoirConfig.ID);
            ModUtil.AddBuildingToPlanScreen("Base", SmartGasReservoirConfig.ID);
        }
    }

    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch("Initialize")]
    public class ModifiedStorageDbPatch
    {
        private const string TechID1 = "ImprovedLiquidPiping";
        private const string TechID2 = "HVAC";

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

                list.Add(SmartLiquidReservoirConfig.ID);

                tech = Db.Get().Techs.TryGet(TechID2);
                if (tech == null)
                    return;
                list = (ICollection<string>)tech.GetType().GetField("unlockedItemIDs")?.GetValue(tech);
                if (list == null)
                    return;

                list.Add(SmartGasReservoirConfig.ID);
            }
            else
            {

                System.Reflection.FieldInfo info = typeof(Database.Techs).GetField("TECH_GROUPING");
                Dictionary<string, string[]> dict = (Dictionary<string, string[]>)info.GetValue(null);
                dict[TechID1].Append(SmartLiquidReservoirConfig.ID);
                dict[TechID2].Append(SmartGasReservoirConfig.ID);
                typeof(Database.Techs).GetField("TECH_GROUPING").SetValue(null, dict);
            }
        }
    }
}
