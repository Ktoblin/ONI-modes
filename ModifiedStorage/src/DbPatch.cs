using Database;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModifiedStorage
{
    [HarmonyPatch(typeof(Db), "Initialize")]
    public class DbPatch
    {
        public static void Prefix()
        {
            List<string> la = new List<string>(Techs.TECH_GROUPING["Agriculture"]) { ModifiedRefrigeratorConfig.ID };
            Techs.TECH_GROUPING["Agriculture"] = la.ToArray();
            List<string> ls = new List<string>(Techs.TECH_GROUPING["SmartStorage"]) { ModifiedStorageLockerSmartConfig.ID };
            Techs.TECH_GROUPING["SmartStorage"] = ls.ToArray();
        }
    }
}
