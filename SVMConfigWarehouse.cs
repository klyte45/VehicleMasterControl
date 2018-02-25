using ColossalFramework;
using ColossalFramework.Globalization;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.Utils;
using Klyte.Commons.Extensors;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager
{
    internal class SVMConfigWarehouse : ConfigWarehouseBase<SVMConfigWarehouse.ConfigIndex, SVMConfigWarehouse>
    {
        public const string CONFIG_FILENAME = "ServiceVehiclesManager";

        public override string ConfigFilename => CONFIG_FILENAME;
        public const string TRUE_VALUE = "1";
        public const string FALSE_VALUE = "0";

        public SVMConfigWarehouse() { }

        public override bool getDefaultBoolValueForProperty(ConfigIndex i)
        {
            return defaultTrueBoolProperties.Contains(i);
        }

        internal static ConfigIndex getConfigAssetsForAI(ServiceSystemDefinition definition)
        {
            return getConfigServiceSystemForDefinition(definition) | ConfigIndex.EXTENSION_CONFIG_DISTRICT;
        }

        public override int getDefaultIntValueForProperty(ConfigIndex i)
        {
            switch (i)
            {
                default:
                    return 0;
            }
        }

        public static string getNameForServiceSystem(ConfigIndex i)
        {
            switch (i & ConfigIndex.SSD_PART)
            {
                case ConfigIndex.DISASTER_CAR: return Locale.Get("VEHICLE_TITLE", "Disaster Response Vehicle");
                case ConfigIndex.DISASTER_HELICOPTER: return Locale.Get("VEHICLE_TITLE", "Disaster Response Helicopter");
                case ConfigIndex.FIRE_CAR: return Locale.Get("VEHICLE_TITLE", "Fire Truck");
                case ConfigIndex.FIRE_HELICOPTER: return Locale.Get("VEHICLE_TITLE", "Fire Helicopter");
                case ConfigIndex.GARBAGE_CAR: return Locale.Get("VEHICLE_TITLE", "Garbage Truck");
                case ConfigIndex.GARBBIO_CAR: return Locale.Get("VEHICLE_TITLE", "Biofuel Garbage Truck 01");
                case ConfigIndex.HEALTHCARE_CAR: return Locale.Get("VEHICLE_TITLE", "Ambulance");
                case ConfigIndex.HEALTHCARE_HELICOPTER: return Locale.Get("VEHICLE_TITLE", "Medical Helicopter");
                case ConfigIndex.DEATHCARE_CAR: return Locale.Get("VEHICLE_TITLE", "Hearse");
                case ConfigIndex.POLICE_CAR: return Locale.Get("VEHICLE_TITLE", "Police Car");
                case ConfigIndex.POLICE_HELICOPTER: return Locale.Get("VEHICLE_TITLE", "Police Helicopter");
                case ConfigIndex.ROAD_CAR: return Locale.Get("VEHICLE_TITLE", "Engineering_Truck");
                case ConfigIndex.WATER_CAR: return Locale.Get("VEHICLE_TITLE", "Water Pumping Truck");
                case ConfigIndex.PRISION_CAR: return Locale.Get("VEHICLE_TITLE", "PoliceVan");
                case ConfigIndex.TAXI_CAR: return Locale.Get("VEHICLE_TITLE", "Taxi");
                case ConfigIndex.CABLECAR_CABLECAR: return Locale.Get("VEHICLE_TITLE", "Cable Car");
                case ConfigIndex.SNOW_CAR: return Locale.Get("VEHICLE_TITLE", "Snowplow");
                case ConfigIndex.REG_TRAIN: return Locale.Get("VEHICLE_TITLE", "Train Engine");
                case ConfigIndex.REG_SHIP: return Locale.Get("VEHICLE_TITLE", "Ship Passenger");
                case ConfigIndex.REG_PLANE: return Locale.Get("VEHICLE_TITLE", "Aircraft Passenger");
                default: return "???" + (i & ConfigIndex.SSD_PART).ToString("X");

            }
        }
        public static string getIconServiceSystem(ConfigIndex i)
        {
            switch (i & ConfigIndex.SSD_PART)
            {
                case ConfigIndex.DISASTER_CAR: return "SubBarFireDepartmentDisaster";
                case ConfigIndex.DISASTER_HELICOPTER: return "SubBarFireDepartmentDisaster";
                case ConfigIndex.FIRE_CAR: return "InfoIconFireSafety";
                case ConfigIndex.FIRE_HELICOPTER: return "InfoIconFireSafety";
                case ConfigIndex.GARBAGE_CAR: return "InfoIconGarbage";
                case ConfigIndex.GARBBIO_CAR: return "InfoIconGarbage";
                case ConfigIndex.HEALTHCARE_CAR: return "ToolbarIconHealthcare";
                case ConfigIndex.HEALTHCARE_HELICOPTER: return "ToolbarIconHealthcare";
                case ConfigIndex.DEATHCARE_CAR: return "ToolbarIconHealthcareHovered";
                case ConfigIndex.POLICE_CAR: return "ToolbarIconPolice";
                case ConfigIndex.POLICE_HELICOPTER: return "ToolbarIconPolice";
                case ConfigIndex.ROAD_CAR: return "ToolbarIconRoads";
                case ConfigIndex.WATER_CAR: return "ToolbarIconWaterAndSewage";
                case ConfigIndex.PRISION_CAR: return "IconPolicyDoubleSentences";
                case ConfigIndex.TAXI_CAR: return "SubBarPublicTransportTaxi";
                case ConfigIndex.CABLECAR_CABLECAR: return "SubBarPublicTransportCableCar";
                case ConfigIndex.SNOW_CAR: return "InfoIconSnow";
                case ConfigIndex.REG_TRAIN: return "SubBarPublicTransportTrain";
                case ConfigIndex.REG_SHIP: return "SubBarPublicTransportShip";
                case ConfigIndex.REG_PLANE: return "SubBarPublicTransportPlane";
                default: return "???" + (i & ConfigIndex.SSD_PART).ToString("X");
            }
        }
        public static bool allowColorChanging(ConfigIndex i)
        {
            switch (i & ConfigIndex.SSD_PART)
            {
                case ConfigIndex.FIRE_CAR:
                case ConfigIndex.HEALTHCARE_CAR:
                case ConfigIndex.POLICE_CAR:
                case ConfigIndex.ROAD_CAR:
                case ConfigIndex.SNOW_CAR:
                case ConfigIndex.PRISION_CAR:
                case ConfigIndex.TAXI_CAR:
                    return false;
                case ConfigIndex.DISASTER_CAR:
                case ConfigIndex.DISASTER_HELICOPTER:
                case ConfigIndex.FIRE_HELICOPTER:
                case ConfigIndex.GARBAGE_CAR:
                case ConfigIndex.GARBBIO_CAR:
                case ConfigIndex.HEALTHCARE_HELICOPTER:
                case ConfigIndex.DEATHCARE_CAR:
                case ConfigIndex.POLICE_HELICOPTER:
                case ConfigIndex.WATER_CAR:
                case ConfigIndex.CABLECAR_CABLECAR:
                case ConfigIndex.REG_TRAIN:
                case ConfigIndex.REG_SHIP:
                case ConfigIndex.REG_PLANE:
                default:
                    return true;
            }
        }

        public static string getFgIconServiceSystem(ConfigIndex i)
        {
            switch (i & ConfigIndex.SSD_PART)
            {
                case ConfigIndex.DISASTER_CAR:
                case ConfigIndex.FIRE_CAR:
                case ConfigIndex.GARBAGE_CAR:
                case ConfigIndex.GARBBIO_CAR:
                case ConfigIndex.HEALTHCARE_CAR:
                case ConfigIndex.DEATHCARE_CAR:
                case ConfigIndex.POLICE_CAR:
                case ConfigIndex.ROAD_CAR:
                case ConfigIndex.WATER_CAR:
                case ConfigIndex.PRISION_CAR:
                case ConfigIndex.CABLECAR_CABLECAR:
                case ConfigIndex.TAXI_CAR:
                case ConfigIndex.SNOW_CAR:
                case ConfigIndex.REG_TRAIN:
                case ConfigIndex.REG_SHIP:
                case ConfigIndex.REG_PLANE:
                    return "";
                case ConfigIndex.FIRE_HELICOPTER:
                case ConfigIndex.HEALTHCARE_HELICOPTER:
                case ConfigIndex.POLICE_HELICOPTER:
                case ConfigIndex.DISASTER_HELICOPTER:
                    return "HelicopterIndicator";
                default: return "???" + (i & ConfigIndex.SSD_PART).ToString("X");
            }
        }

        public enum ConfigIndex : uint
        {
            NIL = 0,
            VEHICLE_PART = 0x3C000000,
            SUBSYS_PART = 0x03F00000,
            SYSTEM_PART = 0x000F8000,
            LEVEL_PART = 0x00007000,
            TYPE_PART = 0x00000F00,
            DESC_DATA = 0xFF,
            CONFIG_GROUP = 0xC0000000,
            SSD_PART = VEHICLE_PART | SUBSYS_PART | SYSTEM_PART | LEVEL_PART,

            GLOBAL_CONFIG = 0x40000000,
            SYSTEM_CONFIG = 0x80000000,

            TYPE_STRING = 0x100,
            TYPE_INT = 0x200,
            TYPE_BOOL = 0x300,
            TYPE_LIST = 0x400,
            TYPE_DICTIONARY = 0x500,

            //AUTO_COLOR_ENABLED                          = GLOBAL_CONFIG | 0x2 | TYPE_BOOL,

            DISASTER_CAR = (1 << 26) | (ItemClass.SubService.None << 20) | (ItemClass.Service.Disaster << 15) | ((ItemClass.Level.Level2 + 1) << 12),
            DISASTER_HELICOPTER = (7 << 26) | (ItemClass.SubService.None << 20) | (ItemClass.Service.Disaster << 15) | ((ItemClass.Level.Level2 + 1) << 12),
            FIRE_CAR = (1 << 26) | (ItemClass.SubService.None << 20) | (ItemClass.Service.FireDepartment << 15) | ((ItemClass.Level.Level1 + 1) << 12),
            FIRE_HELICOPTER = (7 << 26) | (ItemClass.SubService.None << 20) | (ItemClass.Service.FireDepartment << 15) | ((ItemClass.Level.Level1 + 1) << 12),
            GARBAGE_CAR = (1 << 26) | (ItemClass.SubService.None << 20) | (ItemClass.Service.Garbage << 15) | ((ItemClass.Level.Level1 + 1) << 12),
            GARBBIO_CAR = (1 << 26) | (ItemClass.SubService.None << 20) | (ItemClass.Service.Garbage << 15) | ((ItemClass.Level.Level2 + 1) << 12),
            HEALTHCARE_CAR = (1 << 26) | (ItemClass.SubService.None << 20) | (ItemClass.Service.HealthCare << 15) | ((ItemClass.Level.Level1 + 1) << 12),
            HEALTHCARE_HELICOPTER = (7 << 26) | (ItemClass.SubService.None << 20) | (ItemClass.Service.HealthCare << 15) | ((ItemClass.Level.Level3 + 1) << 12),
            DEATHCARE_CAR = (1 << 26) | (ItemClass.SubService.None << 20) | (ItemClass.Service.HealthCare << 15) | ((ItemClass.Level.Level2 + 1) << 12),
            POLICE_CAR = (1 << 26) | (ItemClass.SubService.None << 20) | (ItemClass.Service.PoliceDepartment << 15) | ((ItemClass.Level.Level1 + 1) << 12),
            POLICE_HELICOPTER = (7 << 26) | (ItemClass.SubService.None << 20) | (ItemClass.Service.PoliceDepartment << 15) | ((ItemClass.Level.Level3 + 1) << 12),
            ROAD_CAR = (1 << 26) | (ItemClass.SubService.None << 20) | (ItemClass.Service.Road << 15) | ((ItemClass.Level.Level2 + 1) << 12),
            WATER_CAR = (1 << 26) | (ItemClass.SubService.None << 20) | (ItemClass.Service.Water << 15) | ((ItemClass.Level.Level1 + 1) << 12),
            PRISION_CAR = (1 << 26) | (ItemClass.SubService.None << 20) | (ItemClass.Service.PoliceDepartment << 15) | ((ItemClass.Level.Level4 + 1) << 12),
            TAXI_CAR = (1 << 26) | (ItemClass.SubService.PublicTransportTaxi << 20) | (ItemClass.Service.PublicTransport << 15) | ((ItemClass.Level.Level1 + 1) << 12),
            CABLECAR_CABLECAR = (13 << 26) | (ItemClass.SubService.PublicTransportCableCar << 20) | (ItemClass.Service.PublicTransport << 15) | ((ItemClass.Level.Level1 + 1) << 12),
            SNOW_CAR = (1 << 26) | (ItemClass.SubService.None << 20) | (ItemClass.Service.Road << 15) | ((ItemClass.Level.Level4 + 1) << 12),
            REG_TRAIN = (3 << 26) | (ItemClass.SubService.PublicTransportTrain << 20) | (ItemClass.Service.PublicTransport << 15) | ((ItemClass.Level.Level1 + 1) << 12),
            REG_SHIP = (4 << 26) | (ItemClass.SubService.PublicTransportShip << 20) | (ItemClass.Service.PublicTransport << 15) | ((ItemClass.Level.Level1 + 1) << 12),
            REG_PLANE = (5 << 26) | (ItemClass.SubService.PublicTransportPlane << 20) | (ItemClass.Service.PublicTransport << 15) | ((ItemClass.Level.Level1 + 1) << 12),

            EXTENSION_CONFIG_DISTRICT = 0x1 | TYPE_STRING | SYSTEM_CONFIG,
            BASIC_CONFIG_DISTRICT = 0x2 | TYPE_STRING | GLOBAL_CONFIG,

            DISASTER_CAR_CONFIG = DISASTER_CAR | EXTENSION_CONFIG_DISTRICT,
            DISASTER_HELICOPTER_CONFIG = DISASTER_HELICOPTER | EXTENSION_CONFIG_DISTRICT,
            FIRE_CAR_CONFIG = FIRE_CAR | EXTENSION_CONFIG_DISTRICT,
            FIRE_HELICOPTER_CONFIG = FIRE_HELICOPTER | EXTENSION_CONFIG_DISTRICT,
            GARBAGE_CAR_CONFIG = GARBAGE_CAR | EXTENSION_CONFIG_DISTRICT,
            GARBBIO_CAR_CONFIG = GARBBIO_CAR | EXTENSION_CONFIG_DISTRICT,
            HEALTHCARE_CAR_CONFIG = HEALTHCARE_CAR | EXTENSION_CONFIG_DISTRICT,
            HEALTHCARE_HELICOPTER_CONFIG = HEALTHCARE_HELICOPTER | EXTENSION_CONFIG_DISTRICT,
            DEATHCARE_CAR_CONFIG = DEATHCARE_CAR | EXTENSION_CONFIG_DISTRICT,
            POLICE_CAR_CONFIG = POLICE_CAR | EXTENSION_CONFIG_DISTRICT,
            POLICE_HELICOPTER_CONFIG = POLICE_HELICOPTER | EXTENSION_CONFIG_DISTRICT,
            ROAD_CAR_CONFIG = ROAD_CAR | EXTENSION_CONFIG_DISTRICT,
            WATER_CAR_CONFIG = WATER_CAR | EXTENSION_CONFIG_DISTRICT,
            PRISION_CAR_CONFIG = PRISION_CAR | EXTENSION_CONFIG_DISTRICT,
            TAXI_CAR_CONFIG = TAXI_CAR | EXTENSION_CONFIG_DISTRICT,
            CABLECAR_CABLECAR_CONFIG = CABLECAR_CABLECAR | EXTENSION_CONFIG_DISTRICT,
            REGTRAIN_TRAIN_CONFIG = REG_TRAIN | EXTENSION_CONFIG_DISTRICT,
        }

        public static ConfigIndex[] defaultTrueBoolProperties => new ConfigIndex[] {

        };

        internal static ConfigIndex getConfigServiceSystemForDefinition(ServiceSystemDefinition serviceSystemDefinition)
        {
            if (serviceSystemDefinition == ServiceSystemDefinition.DISASTER_CAR) return ConfigIndex.DISASTER_CAR;
            if (serviceSystemDefinition == ServiceSystemDefinition.DISASTER_HELICOPTER) return ConfigIndex.DISASTER_HELICOPTER;
            if (serviceSystemDefinition == ServiceSystemDefinition.FIRE_CAR) return ConfigIndex.FIRE_CAR;
            if (serviceSystemDefinition == ServiceSystemDefinition.FIRE_HELICOPTER) return ConfigIndex.FIRE_HELICOPTER;
            if (serviceSystemDefinition == ServiceSystemDefinition.GARBAGE_CAR) return ConfigIndex.GARBAGE_CAR;
            if (serviceSystemDefinition == ServiceSystemDefinition.GARBBIO_CAR) return ConfigIndex.GARBBIO_CAR;
            if (serviceSystemDefinition == ServiceSystemDefinition.HEALTHCARE_CAR) return ConfigIndex.HEALTHCARE_CAR;
            if (serviceSystemDefinition == ServiceSystemDefinition.HEALTHCARE_HELICOPTER) return ConfigIndex.HEALTHCARE_HELICOPTER;
            if (serviceSystemDefinition == ServiceSystemDefinition.DEATHCARE_CAR) return ConfigIndex.DEATHCARE_CAR;
            if (serviceSystemDefinition == ServiceSystemDefinition.POLICE_CAR) return ConfigIndex.POLICE_CAR;
            if (serviceSystemDefinition == ServiceSystemDefinition.POLICE_HELICOPTER) return ConfigIndex.POLICE_HELICOPTER;
            if (serviceSystemDefinition == ServiceSystemDefinition.ROAD_CAR) return ConfigIndex.ROAD_CAR;
            if (serviceSystemDefinition == ServiceSystemDefinition.WATER_CAR) return ConfigIndex.WATER_CAR;
            if (serviceSystemDefinition == ServiceSystemDefinition.PRISION_CAR) return ConfigIndex.PRISION_CAR;
            if (serviceSystemDefinition == ServiceSystemDefinition.TAXI_CAR) return ConfigIndex.TAXI_CAR;
            if (serviceSystemDefinition == ServiceSystemDefinition.CABLECAR_CABLECAR) return ConfigIndex.CABLECAR_CABLECAR;
            if (serviceSystemDefinition == ServiceSystemDefinition.SNOW_CAR) return ConfigIndex.SNOW_CAR;
            if (serviceSystemDefinition == ServiceSystemDefinition.REG_TRAIN) return ConfigIndex.REG_TRAIN;
            if (serviceSystemDefinition == ServiceSystemDefinition.REG_PLANE) return ConfigIndex.REG_PLANE;
            if (serviceSystemDefinition == ServiceSystemDefinition.REG_SHIP) return ConfigIndex.REG_SHIP;
            return ConfigIndex.NIL;
        }
    }
}

