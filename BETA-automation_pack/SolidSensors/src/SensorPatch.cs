using Database;
using Harmony;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolidSensors
{
    [HarmonyPatch(typeof(GeneratedBuildings))]
    [HarmonyPatch("LoadGeneratedBuildings")]
    public class SolidConduitElementSensorPatch
    {
        public static LocString NAME = new LocString("Solid Material Element Sensor",
            "STRINGS.BUILDINGS.PREFABS." + SolidConduitElementSensorConfig.ID.ToUpper() + ".NAME");

        public static LocString DESC = new LocString("Element sensors can be used to detect the presence of a specific solid materials on a rails.",
            "STRINGS.BUILDINGS.PREFABS." + SolidConduitElementSensorConfig.ID.ToUpper() + ".DESC");

        public static LocString EFFECT = new LocString("Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) +
                                                        " when the selected " + UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " is detected on a rails.",
            "STRINGS.BUILDINGS.PREFABS." + SolidConduitElementSensorConfig.ID.ToUpper() + ".EFFECT");

        public static LocString LOGIC_PORT = "Internal " + UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID");
        public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if the configured Solid is detected";
        public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);

        static void Prefix()
        {
            Strings.Add(NAME.key.String, NAME.text);
            Strings.Add(DESC.key.String, DESC.text);
            Strings.Add(EFFECT.key.String, EFFECT.text);
            ModUtil.AddBuildingToPlanScreen("Conveyance", SolidConduitElementSensorConfig.ID);
        }

        static void Postfix()
        {
            object obj = Activator.CreateInstance(typeof(SolidConduitElementSensorConfig));
            BuildingConfigManager.Instance.RegisterBuilding(obj as IBuildingConfig);
        }

    }

    [HarmonyPatch(typeof(GeneratedBuildings))]
    [HarmonyPatch("LoadGeneratedBuildings")]
    public class SolidConduitTemperatureSensorPatch
    {
        public static LocString NAME = new LocString("Solid Material Thermo Sensor",
            "STRINGS.BUILDINGS.PREFABS." + SolidConduitTemperatureSensorConfig.ID.ToUpper() + ".NAME");

        public static LocString DESC = new LocString("Thermo sensors disable buildings when their conveoyr contents reach a certain temperature.",
            "STRINGS.BUILDINGS.PREFABS." + SolidConduitTemperatureSensorConfig.ID.ToUpper() + ".DESC");

        public static LocString EFFECT = new LocString("Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) +
                                                        " when ambient " + UI.FormatAsLink("Temperature", "HEAT") + "  enters the chosen range.",
            "STRINGS.BUILDINGS.PREFABS." + SolidConduitTemperatureSensorConfig.ID.ToUpper() + ".EFFECT");

        public static LocString LOGIC_PORT = "Internal " + UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " " + UI.FormatAsLink("Temperature", "HEAT");
        public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + 
                                                    "  if the contained Solid material is within the selected Temperature range";
        public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);

        static void Prefix()
        {
            Debug.Log("Prefix");
            Strings.Add(NAME.key.String, NAME.text);
            Strings.Add(DESC.key.String, DESC.text);
            Strings.Add(EFFECT.key.String, EFFECT.text);
            ModUtil.AddBuildingToPlanScreen("Conveyance", SolidConduitTemperatureSensorConfig.ID);//Shipping
        }

        static void Postfix()
        {
            Debug.Log("Postfix");
            object obj = Activator.CreateInstance(typeof(SolidConduitTemperatureSensorConfig));
            BuildingConfigManager.Instance.RegisterBuilding(obj as IBuildingConfig);
        }

    }

    [HarmonyPatch(typeof(GeneratedBuildings))]
    [HarmonyPatch("LoadGeneratedBuildings")]
    public class SolidConduitDiseaseSensorPatch
    {
        public static LocString NAME = new LocString("Solid Material Germ Sensor",
            "STRINGS.BUILDINGS.PREFABS." + SolidConduitDiseaseSensorConfig.ID.ToUpper() + ".NAME");

        public static LocString DESC = new LocString("Germ sensors can help control automation behavior in the presence of germs.",
            "STRINGS.BUILDINGS.PREFABS." + SolidConduitDiseaseSensorConfig.ID.ToUpper() + ".DESC");

        public static LocString EFFECT = new LocString("Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) +
                                                        " or a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " based on the internal " +
                                                        UI.FormatAsLink("Germ", "DISEASE") + " count in materials on a rails.",
                                                        "STRINGS.BUILDINGS.PREFABS." + SolidConduitDiseaseSensorConfig.ID.ToUpper() + ".EFFECT");

        public static LocString LOGIC_PORT = "Internal " + UI.FormatAsLink("Germ", "DISEASE") + " Count";
        public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) +
                                                    " if the number of Germs on the rails is within the selected range";
        public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);

        static void Prefix()
        {
            Strings.Add(NAME.key.String, NAME.text);
            Strings.Add(DESC.key.String, DESC.text);
            Strings.Add(EFFECT.key.String, EFFECT.text);
            ModUtil.AddBuildingToPlanScreen("Conveyance", SolidConduitDiseaseSensorConfig.ID);
        }

        static void Postfix()
        {
            object obj = Activator.CreateInstance(typeof(SolidConduitDiseaseSensorConfig));
            BuildingConfigManager.Instance.RegisterBuilding(obj as IBuildingConfig);
        }

    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    public class DbPatch
    {
        public static void Prefix()
        {
            List<string> ls = new List<string>(Techs.TECH_GROUPING["SolidTransport"]) { SolidConduitElementSensorConfig.ID, SolidConduitTemperatureSensorConfig.ID };
            Techs.TECH_GROUPING["SolidTransport"] = ls.ToArray();
            List<string> ld = new List<string>(Techs.TECH_GROUPING["MedicineIII"]) { SolidConduitDiseaseSensorConfig.ID };
            Techs.TECH_GROUPING["MedicineIII"] = ld.ToArray();
        }
    }
}
