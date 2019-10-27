using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

using UnityEngine;
using Database;
using TUNING;

namespace BleachStoneRefinery
{
    [HarmonyPatch(typeof(GeneratedBuildings))]
    [HarmonyPatch("LoadGeneratedBuildings")]
    public class BleachStoneRefineryPatch
    {
        public static LocString NAME = new LocString("Bleachstone Refinery",
            "STRINGS.BUILDINGS.PREFABS." + BleachStoneRefineryConfig.ID.ToUpper() + ".NAME");

        public static LocString DESC = new LocString("Chlorine and sand enters on one side, bleach stone exits on the other.",
            "STRINGS.BUILDINGS.PREFABS." + BleachStoneRefineryConfig.ID.ToUpper() + ".DESC");

        public static LocString EFFECT = new LocString("Transforms " + STRINGS.UI.FormatAsLink("Chlorine", "CHLORINEGAS") 
                                                        + " and " + STRINGS.UI.FormatAsLink("Sand", "SAND") 
                                                        + " into " + STRINGS.UI.FormatAsLink("Bleach Stone", "BLEACHSTONE") + ".",
            "STRINGS.BUILDINGS.PREFABS." + BleachStoneRefineryConfig.ID.ToUpper() + ".EFFECT");

        static void Prefix()
        {
            Strings.Add(NAME.key.String, NAME.text);
            Strings.Add(DESC.key.String, DESC.text);
            Strings.Add(EFFECT.key.String, EFFECT.text);
            ModUtil.AddBuildingToPlanScreen("Refining", BleachStoneRefineryConfig.ID);
        }

        static void Postfix()
        {
            object obj = Activator.CreateInstance(typeof(BleachStoneRefineryConfig));
            BuildingConfigManager.Instance.RegisterBuilding(obj as IBuildingConfig);
        }

    }

    //ColorPatch
    [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
    public class ColorPatch
    {
        public static void Postfix(BuildingComplete __instance)
        {
            var kAnimBase = __instance.GetComponent<KAnimControllerBase>();
            if (kAnimBase != null)
            {
                Debug.Log(__instance.name);
                if (__instance.name == "BleachStoneRefineryComplete")
                {
                    float r = 255 - 60;
                    float g = 255 - 50;
                    float b = 255 - 130;
                    kAnimBase.TintColour = new Color(r / 255f, g / 255f, b / 255f);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    public class DbPatch
    {
        public static void Prefix()
        {
            List<string> ls = new List<string>(Techs.TECH_GROUPING["Catalytics"]) { BleachStoneRefineryConfig.ID };
            Techs.TECH_GROUPING["Catalytics"] = ls.ToArray();
        }
    }
}


