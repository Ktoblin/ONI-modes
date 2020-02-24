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
    public class SlimeRefineryPatch
    {
        public static LocString NAME = new LocString("Slime Refinery",
            "STRINGS.BUILDINGS.PREFABS." + SlimeRefineryConfig.ID.ToUpper() + ".NAME");

        public static LocString DESC = new LocString("Polluted oxygen and dirt enters on one side, slime exits on the other.",
            "STRINGS.BUILDINGS.PREFABS." + SlimeRefineryConfig.ID.ToUpper() + ".DESC");

        public static LocString EFFECT = new LocString("Transforms " + STRINGS.UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") 
                                                        + " and " + STRINGS.UI.FormatAsLink("Dirt", "DIRT") 
                                                        + " into " + STRINGS.UI.FormatAsLink("Slime", "SLIMEMOLD") + ".",
            "STRINGS.BUILDINGS.PREFABS." + SlimeRefineryConfig.ID.ToUpper() + ".EFFECT");

        static void Prefix()
        {
            Strings.Add(NAME.key.String, NAME.text);
            Strings.Add(DESC.key.String, DESC.text);
            Strings.Add(EFFECT.key.String, EFFECT.text);
            ModUtil.AddBuildingToPlanScreen("Refining", SlimeRefineryConfig.ID);
        }

        static void Postfix()
        {
            object obj = Activator.CreateInstance(typeof(SlimeRefineryConfig));
            BuildingConfigManager.Instance.RegisterBuilding(obj as IBuildingConfig);
        }

    }
}


