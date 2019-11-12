using Database;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BleachStoneRefinery
{
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
                if (__instance.name == "SlimeRefineryComplete")
                {
                    float r = 255 - 127;
                    float g = 255 - 159;
                    float b = 255 - 159;
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
            List<string> ls = new List<string>(Techs.TECH_GROUPING["Catalytics"]) { BleachStoneRefineryConfig.ID, SlimeRefineryConfig.ID };
            Techs.TECH_GROUPING["Catalytics"] = ls.ToArray();
        }
    }
}
