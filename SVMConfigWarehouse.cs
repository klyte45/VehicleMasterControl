using ColossalFramework;
using ColossalFramework.Globalization;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.Utils;
using Klyte.TransportLinesManager.Extensors;
using Klyte.TransportLinesManager.Extensors.TransportTypeExt;
using Klyte.TransportLinesManager.Interfaces;
using Klyte.TransportLinesManager.Utils;
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
        public const string TRUE_VALUE          = "1";
        public const string FALSE_VALUE         = "0";

        public SVMConfigWarehouse() { }

        public override bool getDefaultBoolValueForProperty(ConfigIndex i)
        {
            return defaultTrueBoolProperties.Contains(i);
        }
        public override int getDefaultIntValueForProperty(ConfigIndex i)
        {
            switch (i)
            {
                default:
                    return 0;
            }
        }

        public static Color32 getColorForTransportType(ConfigIndex i)
        {
            switch (i & ConfigIndex.SYSTEM_PART)
            {
                case ConfigIndex.DISASTER_CAR: return new Color32(250, 104, 0, 255);
                case ConfigIndex.DISASTER_HELICOPTER: return new Color32(73, 27, 137, 255);
                case ConfigIndex.FIRE_CAR: return new Color32(58, 224, 50, 255);
                case ConfigIndex.FIRE_HELICOPTER: return new Color32(53, 121, 188, 255);
                case ConfigIndex.GARBAGE_CAR: return new Color32(0xa8, 0x01, 0x7a, 255);
                case ConfigIndex.HEALTHCARE_CAR: return new Color32(0xa3, 0xb0, 0, 255);
                case ConfigIndex.HEALTHCARE_HELICOPTER: return new Color32(0xd8, 0x01, 0xaa, 255);
                case ConfigIndex.DEATHCARE_CAR: return new Color32(0xe3, 0xf0, 0, 255);
                case ConfigIndex.POLICE_CAR: return new Color32(217, 51, 89, 255);
                case ConfigIndex.POLICE_HELICOPTER: return new Color32(31, 96, 225, 255);
                case ConfigIndex.ROAD_CAR: return new Color32(60, 184, 120, 255);
                case ConfigIndex.WATER_CAR: return new Color32(202, 162, 31, 255);
                case ConfigIndex.PRISION_CAR: return new Color32(202, 162, 31, 255);
                default: return new Color();

            }
        }

        public static string getNameForServiceSystem(ConfigIndex i)
        {
            switch (i & ConfigIndex.SYSTEM_PART)
            {
                case ConfigIndex.DISASTER_CAR: return Locale.Get("VEHICLE_TITLE", "Train Engine");
                case ConfigIndex.DISASTER_HELICOPTER: return Locale.Get("VEHICLE_TITLE", "Tram");
                case ConfigIndex.FIRE_CAR: return Locale.Get("VEHICLE_TITLE", "Metro");
                case ConfigIndex.FIRE_HELICOPTER: return Locale.Get("VEHICLE_TITLE", "Bus");
                case ConfigIndex.GARBAGE_CAR: return Locale.Get("VEHICLE_TITLE", "Aircraft Passenger");
                case ConfigIndex.HEALTHCARE_CAR: return Locale.Get("VEHICLE_TITLE", "Ship Passenger");
                case ConfigIndex.HEALTHCARE_HELICOPTER: return Locale.Get("VEHICLE_TITLE", "Blimp");
                case ConfigIndex.DEATHCARE_CAR: return Locale.Get("VEHICLE_TITLE", "Ferry");
                case ConfigIndex.POLICE_CAR: return Locale.Get("VEHICLE_TITLE", "Monorail Front");
                case ConfigIndex.POLICE_HELICOPTER: return Locale.Get("VEHICLE_TITLE", "Cable Car");
                case ConfigIndex.ROAD_CAR: return Locale.Get("VEHICLE_TITLE", "Taxi");
                case ConfigIndex.WATER_CAR: return Locale.Get("VEHICLE_TITLE", "Evacuation Bus");
                case ConfigIndex.PRISION_CAR: return Locale.Get("VEHICLE_TITLE", "Evacuation Bus");
                default: return "???";

            }
        }


        public enum ConfigIndex
        {
            NIL = -1,
            VEHICLE_PART = 0xF00000,
            SYSTEM_PART = 0x0F8000,
            LEVEL_PART = 0x007000,
            TYPE_PART = 0x000F00,
            DESC_DATA = 0xFF,

            GLOBAL_CONFIG = 0x1000000,

            TYPE_STRING = 0x100,
            TYPE_INT = 0x200,
            TYPE_BOOL = 0x300,
            TYPE_LIST = 0x400,
            TYPE_DICTIONARY = 0x500,

            //AUTO_COLOR_ENABLED                          = GLOBAL_CONFIG | 0x2 | TYPE_BOOL,

            DISASTER_CAR = (1 << 24)          | (ItemClass.Service.Disaster         << 15) | ((ItemClass.Level.Level2+1)<<12),
            DISASTER_HELICOPTER = (7 << 24)   | (ItemClass.Service.Disaster         << 15) | ((ItemClass.Level.Level2+1)<<12),
            FIRE_CAR = (1 << 24)              | (ItemClass.Service.FireDepartment   << 15) | ((ItemClass.Level.Level1+1)<<12),
            FIRE_HELICOPTER = (7 << 24)       | (ItemClass.Service.FireDepartment   << 15) | ((ItemClass.Level.Level1+1)<<12),
            GARBAGE_CAR = (1 << 24)           | (ItemClass.Service.Garbage          << 15) | ((ItemClass.Level.Level2+1)<<12),
            HEALTHCARE_CAR = (1 << 24)        | (ItemClass.Service.HealthCare       << 15) | ((ItemClass.Level.Level1+1)<<12),
            HEALTHCARE_HELICOPTER = (7 << 24) | (ItemClass.Service.HealthCare       << 15) | ((ItemClass.Level.Level3+1)<<12),
            DEATHCARE_CAR = (1 << 24)         | (ItemClass.Service.HealthCare       << 15) | ((ItemClass.Level.Level2+1)<<12),
            POLICE_CAR = (1 << 24)            | (ItemClass.Service.PoliceDepartment << 15) | ((ItemClass.Level.Level1+1)<<12),
            POLICE_HELICOPTER = (7 << 24)     | (ItemClass.Service.PoliceDepartment << 15) | ((ItemClass.Level.Level3+1)<<12),
            ROAD_CAR = (1 << 24)              | (ItemClass.Service.Road             << 15) | ((ItemClass.Level.Level2+1)<<12),
            WATER_CAR = (1 << 24)             | (ItemClass.Service.Water            << 15) | ((ItemClass.Level.None  +1)<<12),
            PRISION_CAR = (1 << 24)           | (ItemClass.Service.PoliceDepartment << 15) | ((ItemClass.Level.Level4+1)<<12),


            //PREFIX = 0x1 | TYPE_INT,      


        }

        public static ConfigIndex[] defaultTrueBoolProperties => new ConfigIndex[] {

        };


    }
}
