using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using STRINGS;
using UnityEngine;

namespace BleachStoneRefinery
{
    [HarmonyPatch(typeof(GeneratedBuildings))]
    [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
    public class ModifiedStorageBuildingsPatch
    {
        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        private static void Prefix()
        {
            CaiLib.Utils.StringUtils.AddBuildingStrings(
                BleachStoneRefineryConfig.ID,
                "Bleachstone Refinery",
                "Chlorine and sand enters on one side, bleach stone exits on the other.",
                "Transforms " + UI.FormatAsLink("Chlorine", "CHLORINEGAS")
                                                        + " and " + UI.FormatAsLink("Sand", "SAND")
                                                        + " into " + UI.FormatAsLink("Bleach Stone", "BLEACHSTONE") + "."
            );
            CaiLib.Utils.StringUtils.AddBuildingStrings(
                SlimeRefineryConfig.ID,
                "Slime Refinery",
                "Polluted oxygen and dirt enters on one side, slime exits on the other.",
                "Transforms " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN")
                                                        + " and " + UI.FormatAsLink("Dirt", "DIRT")
                                                        + " into " + UI.FormatAsLink("Slime", "SLIMEMOLD") + "."
            );

            ModUtil.AddBuildingToPlanScreen("Refining", BleachStoneRefineryConfig.ID);
            ModUtil.AddBuildingToPlanScreen("Refining", SlimeRefineryConfig.ID);
        }
    }

    [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
    public class ColorPatch
    {
        public static void Postfix(BuildingComplete __instance)
        {
            var kAnimBase = __instance.GetComponent<KAnimControllerBase>();
            if (kAnimBase != null)
            {
                Debug.Log(__instance.name);
                if (__instance.name == "Ktoblin.BleachStoneRefineryComplete")
                {
                    float r = 255 - 60;
                    float g = 255 - 50;
                    float b = 255 - 130;
                    kAnimBase.TintColour = new Color(r / 255f, g / 255f, b / 255f);
                }
                if (__instance.name == "Ktoblin.SlimeRefineryComplete")
                {
                    float r = 255 - 127;
                    float g = 255 - 159;
                    float b = 255 - 159;
                    kAnimBase.TintColour = new Color(r / 255f, g / 255f, b / 255f);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch("Initialize")]
    public class ModifiedStorageDbPatch
    {
        private const string TechID = "Catalytics";

        private static void Postfix()
        {

            if (typeof(Database.Techs).GetField("TECH_GROUPING") == null)
            {
                Tech tech = Db.Get().Techs.TryGet(TechID);
                if (tech == null)
                    return;
                ICollection<string> list = (ICollection<string>)tech.GetType().GetField("unlockedItemIDs")?.GetValue(tech);
                if (list == null)
                    return;

                list.Add(BleachStoneRefineryConfig.ID);
                list.Add(SlimeRefineryConfig.ID);


            }
            else
            {

                System.Reflection.FieldInfo info = typeof(Database.Techs).GetField("TECH_GROUPING");
                Dictionary<string, string[]> dict = (Dictionary<string, string[]>)info.GetValue(null);
                dict[TechID].Append(BleachStoneRefineryConfig.ID);
                dict[TechID].Append(SlimeRefineryConfig.ID);
                typeof(Database.Techs).GetField("TECH_GROUPING").SetValue(null, dict);
            }
        }
    }
}
