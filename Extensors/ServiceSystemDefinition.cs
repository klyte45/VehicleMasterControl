using ColossalFramework;
using ColossalFramework.Globalization;
using Klyte.Commons.Utils;
using Klyte.VehiclesMasterControl.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klyte.VehiclesMasterControl.Extensors.VehicleExt
{
    public struct ServiceSystemDefinition
    {
        public static readonly ServiceSystemDefinition DISASTER_CAR = new ServiceSystemDefinition(ItemClass.Service.Disaster, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition DISASTER_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.Disaster, ItemClass.SubService.None, VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level2);

        public static readonly ServiceSystemDefinition FIRE_CAR = new ServiceSystemDefinition(ItemClass.Service.FireDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition FIRE_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.FireDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level1);

        public static readonly ServiceSystemDefinition GARBAGE_CAR = new ServiceSystemDefinition(ItemClass.Service.Garbage, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition GARBBIO_CAR = new ServiceSystemDefinition(ItemClass.Service.Garbage, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);


        public static readonly ServiceSystemDefinition HEALTHCARE_CAR = new ServiceSystemDefinition(ItemClass.Service.HealthCare, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition HEALTHCARE_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.HealthCare, ItemClass.SubService.None, VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level3);
        public static readonly ServiceSystemDefinition DEATHCARE_CAR = new ServiceSystemDefinition(ItemClass.Service.HealthCare, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);

        public static readonly ServiceSystemDefinition POLICE_CAR = new ServiceSystemDefinition(ItemClass.Service.PoliceDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition POLICE_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.PoliceDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level3);
        public static readonly ServiceSystemDefinition PRISION_CAR = new ServiceSystemDefinition(ItemClass.Service.PoliceDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level4);

        public static readonly ServiceSystemDefinition SNOW_CAR = new ServiceSystemDefinition(ItemClass.Service.Road, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level4);
        public static readonly ServiceSystemDefinition ROAD_CAR = new ServiceSystemDefinition(ItemClass.Service.Road, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);

        public static readonly ServiceSystemDefinition WATER_CAR = new ServiceSystemDefinition(ItemClass.Service.Water, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition TAXI_CAR = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTaxi, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition CABLECAR_CABLECAR = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportCableCar, VehicleInfo.VehicleType.CableCar, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition REG_TRAIN = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain, VehicleInfo.VehicleType.Train, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition REG_PLANE = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPlane, VehicleInfo.VehicleType.Plane, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition REG_SHIP = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportShip, VehicleInfo.VehicleType.Ship, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition CARG_SHIP = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportShip, VehicleInfo.VehicleType.Ship, ItemClass.Level.Level4);
        public static readonly ServiceSystemDefinition CARG_TRAIN = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain, VehicleInfo.VehicleType.Train, ItemClass.Level.Level4);
        public static readonly ServiceSystemDefinition BEAU_CAR = new ServiceSystemDefinition(ItemClass.Service.Beautification, ItemClass.SubService.BeautificationParks, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition POST_CAR = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPost, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition POST_TRK = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPost, VehicleInfo.VehicleType.Car, ItemClass.Level.Level5);

        public static readonly ServiceSystemDefinition OUT_TRAIN = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain, VehicleInfo.VehicleType.Train, ItemClass.Level.Level1, true);
        public static readonly ServiceSystemDefinition OUT_PLANE = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPlane, VehicleInfo.VehicleType.Plane, ItemClass.Level.Level1, true);
        public static readonly ServiceSystemDefinition OUT_SHIP = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportShip, VehicleInfo.VehicleType.Ship, ItemClass.Level.Level1, true);
        public static readonly ServiceSystemDefinition OUT_ROAD = new ServiceSystemDefinition(ItemClass.Service.Road, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level5, true);

        public bool AllowRestrictions { get; private set; }

        public long Idx
        {
            get; private set;
        }

        internal IVMCDistrictExtension GetDistrictExtension() => sysDefinitions[this].GetExtensionDistrict();
        internal IVMCBuildingExtension GetBuildingExtension() => sysDefinitions[this].GetExtensionBuilding();

        internal SSD ToIndex => m_indexMap[this];

        public static readonly Dictionary<ServiceSystemDefinition, SSD> m_indexMap = new Dictionary<ServiceSystemDefinition, SSD>()
        {
            [DISASTER_CAR] = SSD.DISASTER_CAR,
            [DISASTER_HELICOPTER] = SSD.DISASTER_HELICOPTER,
            [FIRE_CAR] = SSD.FIRE_CAR,
            [FIRE_HELICOPTER] = SSD.FIRE_HELICOPTER,
            [GARBAGE_CAR] = SSD.GARBAGE_CAR,
            [GARBBIO_CAR] = SSD.GARBBIO_CAR,
            [HEALTHCARE_CAR] = SSD.HEALTHCARE_CAR,
            [HEALTHCARE_HELICOPTER] = SSD.HEALTHCARE_HELICOPTER,
            [DEATHCARE_CAR] = SSD.DEATHCARE_CAR,
            [POLICE_CAR] = SSD.POLICE_CAR,
            [POLICE_HELICOPTER] = SSD.POLICE_HELICOPTER,
            [PRISION_CAR] = SSD.PRISION_CAR,
            [SNOW_CAR] = SSD.SNOW_CAR,
            [ROAD_CAR] = SSD.ROAD_CAR,
            [WATER_CAR] = SSD.WATER_CAR,
            [TAXI_CAR] = SSD.TAXI_CAR,
            [CABLECAR_CABLECAR] = SSD.CABLECAR_CABLECAR,
            [REG_TRAIN] = SSD.REG_TRAIN,
            [REG_PLANE] = SSD.REG_PLANE,
            [REG_SHIP] = SSD.REG_SHIP,
            [CARG_SHIP] = SSD.CARG_SHIP,
            [CARG_TRAIN] = SSD.CARG_TRAIN,
            [BEAU_CAR] = SSD.BEAU_CAR,
            [POST_CAR] = SSD.POST_CAR,
            [POST_TRK] = SSD.POST_TRK,
            [OUT_TRAIN] = SSD.OUT_TRAIN,
            [OUT_PLANE] = SSD.OUT_PLANE,
            [OUT_SHIP] = SSD.OUT_SHIP,
            [OUT_ROAD] = SSD.OUT_ROAD,
        };

        public static Dictionary<ServiceSystemDefinition, IVMCSysDef> sysDefinitions
        {
            get {
                if (m_sysDefinitions.Count == 0)
                {
                    m_sysDefinitions[GARBAGE_CAR] = SingletonLite<VMCSysDefGarCar>.instance;
                    m_sysDefinitions[DEATHCARE_CAR] = SingletonLite<VMCSysDefDcrCar>.instance;
                    m_sysDefinitions[REG_PLANE] = SingletonLite<VMCSysDefRegPln>.instance;
                    m_sysDefinitions[REG_TRAIN] = SingletonLite<VMCSysDefRegTra>.instance;
                    m_sysDefinitions[REG_SHIP] = SingletonLite<VMCSysDefRegShp>.instance;
                    m_sysDefinitions[FIRE_CAR] = SingletonLite<VMCSysDefFirCar>.instance;
                    m_sysDefinitions[HEALTHCARE_CAR] = SingletonLite<VMCSysDefHcrCar>.instance;
                    m_sysDefinitions[POLICE_CAR] = SingletonLite<VMCSysDefPolCar>.instance;
                    m_sysDefinitions[CARG_TRAIN] = SingletonLite<VMCSysDefCrgTra>.instance;
                    m_sysDefinitions[CARG_SHIP] = SingletonLite<VMCSysDefCrgShp>.instance;

                    m_sysDefinitions[OUT_PLANE] = SingletonLite<VMCSysDefOutPln>.instance;
                    m_sysDefinitions[OUT_TRAIN] = SingletonLite<VMCSysDefOutTra>.instance;
                    m_sysDefinitions[OUT_SHIP] = SingletonLite<VMCSysDefOutShp>.instance;
                    m_sysDefinitions[OUT_ROAD] = SingletonLite<VMCSysDefOutCar>.instance;

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.AfterDark))
                    //{
                    m_sysDefinitions[PRISION_CAR] = SingletonLite<VMCSysDefPriCar>.instance;
                    m_sysDefinitions[TAXI_CAR] = SingletonLite<VMCSysDefTaxCar>.instance;
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.Snowfall))
                    //{
                    m_sysDefinitions[ROAD_CAR] = SingletonLite<VMCSysDefRoaCar>.instance;
                    m_sysDefinitions[SNOW_CAR] = SingletonLite<VMCSysDefSnwCar>.instance;
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.NaturalDisasters))
                    //{
                    m_sysDefinitions[WATER_CAR] = SingletonLite<VMCSysDefWatCar>.instance;
                    m_sysDefinitions[DISASTER_CAR] = SingletonLite<VMCSysDefDisCar>.instance;
                    m_sysDefinitions[DISASTER_HELICOPTER] = SingletonLite<VMCSysDefDisHel>.instance;
                    m_sysDefinitions[FIRE_HELICOPTER] = SingletonLite<VMCSysDefFirHel>.instance;
                    m_sysDefinitions[HEALTHCARE_HELICOPTER] = SingletonLite<VMCSysDefHcrHel>.instance;
                    m_sysDefinitions[POLICE_HELICOPTER] = SingletonLite<VMCSysDefPolHel>.instance;
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.InMotion))
                    //{
                    m_sysDefinitions[CABLECAR_CABLECAR] = SingletonLite<VMCSysDefCcrCcr>.instance;
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.GreenCities))
                    //{
                    m_sysDefinitions[GARBBIO_CAR] = SingletonLite<VMCSysDefGbcCar>.instance;
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.Parks))
                    //{
                    m_sysDefinitions[BEAU_CAR] = SingletonLite<VMCSysDefBeaCar>.instance;
                    //}
                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.Industry))
                    //{
                    m_sysDefinitions[POST_CAR] = SingletonLite<VMCSysDefPstCar>.instance;
                    m_sysDefinitions[POST_TRK] = SingletonLite<VMCSysDefPstTrk>.instance;
                    //}
                }
                return m_sysDefinitions;
            }
        }


        public bool AllowDistrictServiceRestrictions => AllowRestrictions;
        public CategoryTab Category { get; }
        public string NameForServiceSystem { get; }
        public string FgIconServiceSystem { get; }
        public string IconServiceSystem { get; }

        private static readonly Dictionary<ServiceSystemDefinition, IVMCSysDef> m_sysDefinitions = new Dictionary<ServiceSystemDefinition, IVMCSysDef>();

        public ItemClass.Service service
        {
            get;
        }

        public ItemClass.SubService subService
        {
            get;
        }
        public VehicleInfo.VehicleType vehicleType
        {
            get;
        }
        public ItemClass.Level level
        {
            get;
        }
        public bool outsideConnection
        {
            get;
        }


        private ServiceSystemDefinition(
        ItemClass.Service service,
        ItemClass.SubService subService,
        VehicleInfo.VehicleType vehicleType,
        ItemClass.Level level,
        bool outsideConnection = false)
        {
            this.vehicleType = vehicleType;
            this.service = service;
            this.level = level;
            this.subService = subService;
            this.outsideConnection = outsideConnection;
            Idx = ((int)service << 48) | ((int)subService << 40) | ((int)level << 32) | (Mathf.RoundToInt(Mathf.Log((int)vehicleType) / Mathf.Log(2)) << 8) | (outsideConnection ? 1 : 0);
            AllowRestrictions = service != ItemClass.Service.PublicTransport || subService == ItemClass.SubService.PublicTransportTaxi;
            Category = SetCategory(service, outsideConnection);
            NameForServiceSystem = SetNameForServiceSystem(service, subService, vehicleType, level, outsideConnection);
            IconServiceSystem = SetIcon(service, subService, vehicleType, level, outsideConnection);
            FgIconServiceSystem = SetFgIcon(service, subService, vehicleType, level, outsideConnection);
        }

        private static string SetFgIcon(ItemClass.Service service, ItemClass.SubService subService, VehicleInfo.VehicleType vehicleType, ItemClass.Level level, bool outsideConnection)
        {
            return "K45_VMC_" + outsideConnection switch
            {
                true => "OutsideIndicator",
                false => vehicleType switch
                {
                    VehicleInfo.VehicleType.Helicopter => "HelicopterIndicator",
                    _ => service switch
                    {
                        ItemClass.Service.Garbage => level switch
                        {
                            ItemClass.Level.Level2 => "BioIndicator",
                            _ => ""
                        },
                        ItemClass.Service.PublicTransport => level switch
                        {
                            ItemClass.Level.Level4 => "CargoIndicator",
                            ItemClass.Level.Level5 => "CargoIndicator",
                            _ => ""
                        },
                        _ => ""
                    },
                }
            };
        }

        private static string SetIcon(ItemClass.Service service, ItemClass.SubService subService, VehicleInfo.VehicleType vehicleType, ItemClass.Level level, bool outsideConnection)
        {
            return service switch
            {
                ItemClass.Service.Disaster => vehicleType switch
                {
                    VehicleInfo.VehicleType.Car => "SubBarFireDepartmentDisaster",
                    VehicleInfo.VehicleType.Helicopter => "SubBarFireDepartmentDisaster",
                    _ => "UNKNOWN DISASTER",
                },
                ItemClass.Service.FireDepartment => vehicleType switch
                {
                    VehicleInfo.VehicleType.Car => "InfoIconFireSafety",
                    VehicleInfo.VehicleType.Helicopter => "InfoIconFireSafety",
                    _ => "UNKNOWN FIRE",
                },
                ItemClass.Service.Garbage => level switch
                {
                    ItemClass.Level.Level1 => "InfoIconGarbage",
                    ItemClass.Level.Level2 => "InfoIconGarbage",
                    _ => "UNKNOWN GARBAGE",
                },
                ItemClass.Service.HealthCare => level switch
                {
                    ItemClass.Level.Level1 => "ToolbarIconHealthcare",
                    ItemClass.Level.Level2 => "ToolbarIconHealthcareHovered",
                    ItemClass.Level.Level3 => "ToolbarIconHealthcare",
                    _ => "UNKNOWN HEALTHCARE",
                },
                ItemClass.Service.PoliceDepartment => level switch
                {
                    ItemClass.Level.Level1 => "ToolbarIconPolice",
                    ItemClass.Level.Level3 => "ToolbarIconPolice",
                    ItemClass.Level.Level4 => "IconPolicyDoubleSentences",
                    _ => "UNKNOWN POLICE",
                },
                ItemClass.Service.Road => level switch
                {
                    ItemClass.Level.Level2 => "ToolbarIconRoads",
                    ItemClass.Level.Level4 => "InfoIconSnow",
                    ItemClass.Level.Level5 => outsideConnection switch
                    {
                        true => "ToolbarIconRoads",
                        _ => "UNKNOWN ROAD 5"
                    },
                    _ => "UNKNOWN ROAD",
                },
                ItemClass.Service.Water => level switch
                {
                    ItemClass.Level.Level1 => "ToolbarIconWaterAndSewage",
                    _ => "UNKNOWN WATER",
                },
                ItemClass.Service.Beautification => "ToolbarIconBeautification",
                ItemClass.Service.PublicTransport => subService switch
                {
                    ItemClass.SubService.PublicTransportPost => level switch
                    {
                        ItemClass.Level.Level2 => "InfoIconPost",
                        ItemClass.Level.Level5 => "InfoIconPost",
                        _ => $"UNKNOWN POST {level}",
                    },
                    ItemClass.SubService.PublicTransportTaxi => "SubBarPublicTransportTaxi",
                    ItemClass.SubService.PublicTransportCableCar => "SubBarPublicTransportCableCar",
                    ItemClass.SubService.PublicTransportTrain => level switch
                    {
                        ItemClass.Level.Level1 => outsideConnection switch
                        {
                            true => "SubBarPublicTransportTrain",
                            false => "SubBarPublicTransportTrain"
                        },
                        ItemClass.Level.Level4 => "SubBarPublicTransportTrain",
                        _ => $"UNKNOWN TRAIN {level}",
                    },

                    ItemClass.SubService.PublicTransportShip => level switch
                    {
                        ItemClass.Level.Level1 => outsideConnection switch
                        {
                            true => "SubBarPublicTransportShip",
                            false => "SubBarPublicTransportShip",
                        },
                        ItemClass.Level.Level4 => "SubBarPublicTransportShip",
                        _ => $"UNKNOWN SHIP {level}",
                    },
                    ItemClass.SubService.PublicTransportPlane => level switch
                    {
                        ItemClass.Level.Level1 => outsideConnection switch
                        {
                            true => "SubBarPublicTransportPlane",
                            false => "SubBarPublicTransportPlane",
                        },
                        ItemClass.Level.Level4 => "SubBarPublicTransportPlane",
                        _ => $"UNKNOWN AIRCRAFT {level}",
                    },
                    _ => $"UNKNOWN PUBLIC TRANSPORT {subService}",
                },
                _ => $"??? {service}",
            };

        }

        private static string SetNameForServiceSystem(
            ItemClass.Service service,
        ItemClass.SubService subService,
        VehicleInfo.VehicleType vehicleType,
        ItemClass.Level level,
        bool outsideConnection)
        {
            return service switch
            {
                ItemClass.Service.Disaster => vehicleType switch
                {
                    VehicleInfo.VehicleType.Car => Locale.Get("VEHICLE_TITLE", "Disaster Response Vehicle"),
                    VehicleInfo.VehicleType.Helicopter => Locale.Get("VEHICLE_TITLE", "Disaster Response Helicopter"),
                    _ => "UNKNOWN DISASTER",
                },
                ItemClass.Service.FireDepartment => vehicleType switch
                {
                    VehicleInfo.VehicleType.Car => Locale.Get("VEHICLE_TITLE", "Fire Truck"),
                    VehicleInfo.VehicleType.Helicopter => Locale.Get("VEHICLE_TITLE", "Fire Helicopter"),
                    _ => "UNKNOWN FIRE",
                },
                ItemClass.Service.Garbage => level switch
                {
                    ItemClass.Level.Level1 => Locale.Get("VEHICLE_TITLE", "Garbage Truck"),
                    ItemClass.Level.Level2 => Locale.Get("VEHICLE_TITLE", "Biofuel Garbage Truck 01"),
                    _ => "UNKNOWN GARBAGE",
                },
                ItemClass.Service.HealthCare => level switch
                {
                    ItemClass.Level.Level1 => Locale.Get("VEHICLE_TITLE", "Ambulance"),
                    ItemClass.Level.Level2 => Locale.Get("VEHICLE_TITLE", "Hearse"),
                    ItemClass.Level.Level3 => Locale.Get("VEHICLE_TITLE", "Medical Helicopter"),
                    _ => "UNKNOWN HEALTHCARE",
                },
                ItemClass.Service.PoliceDepartment => level switch
                {
                    ItemClass.Level.Level1 => Locale.Get("VEHICLE_TITLE", "Police Car"),
                    ItemClass.Level.Level3 => Locale.Get("VEHICLE_TITLE", "Police Helicopter"),
                    ItemClass.Level.Level4 => Locale.Get("VEHICLE_TITLE", "PoliceVan"),
                    _ => "UNKNOWN POLICE",
                },
                ItemClass.Service.Road => level switch
                {
                    ItemClass.Level.Level2 => Locale.Get("VEHICLE_TITLE", "Engineering_Truck"),
                    ItemClass.Level.Level4 => Locale.Get("VEHICLE_TITLE", "Snowplow"),
                    ItemClass.Level.Level5 => outsideConnection switch
                    {
                        true => Locale.Get("AREA_YES_HIGHWAYCONNECTION"),
                        _ => "UNKNOWN ROAD 5"
                    },
                    _ => "UNKNOWN ROAD",
                },
                ItemClass.Service.Water => level switch
                {
                    ItemClass.Level.Level1 => Locale.Get("VEHICLE_TITLE", "Water Pumping Truck"),
                    _ => "UNKNOWN WATER",
                },
                ItemClass.Service.Beautification => Locale.Get("VEHICLE_TITLE", "Park Staff Vehicle 01"),
                ItemClass.Service.PublicTransport => subService switch
                {
                    ItemClass.SubService.PublicTransportPost => level switch
                    {
                        ItemClass.Level.Level2 => Locale.Get("VEHICLE_TITLE", "Post Vehicle 01"),
                        ItemClass.Level.Level5 => Locale.Get("VEHICLE_TITLE", "Post Truck 01"),
                        _ => $"UNKNOWN POST {level}",
                    },
                    ItemClass.SubService.PublicTransportTaxi => Locale.Get("VEHICLE_TITLE", "Taxi"),
                    ItemClass.SubService.PublicTransportCableCar => Locale.Get("VEHICLE_TITLE", "Cable Car"),
                    ItemClass.SubService.PublicTransportTrain => level switch
                    {
                        ItemClass.Level.Level1 => outsideConnection switch
                        {
                            true => Locale.Get("AREA_YES_TRAINCONNECTION"),
                            false => Locale.Get("VEHICLE_TITLE", "Train Engine")
                        },
                        ItemClass.Level.Level4 => Locale.Get("VEHICLE_TITLE", "Train Cargo Engine"),
                        _ => $"UNKNOWN TRAIN {level}",
                    },

                    ItemClass.SubService.PublicTransportShip => level switch
                    {
                        ItemClass.Level.Level1 => outsideConnection switch
                        {
                            true => Locale.Get("AREA_YES_SHIPCONNECTION"),
                            false => Locale.Get("VEHICLE_TITLE", "Ship Passenger"),
                        },
                        ItemClass.Level.Level4 => Locale.Get("VEHICLE_TITLE", "Ship Cargo"),
                        _ => $"UNKNOWN SHIP {level}",
                    },
                    ItemClass.SubService.PublicTransportPlane => level switch
                    {
                        ItemClass.Level.Level1 => outsideConnection switch
                        {
                            true => Locale.Get("AREA_YES_SHIPCONNECTION"),
                            false => Locale.Get("VEHICLE_TITLE", "Aircraft Passenger"),
                        },
                        ItemClass.Level.Level4 => Locale.Get("VEHICLE_TITLE", "Aircraft Cargo"),
                        _ => $"UNKNOWN AIRCRAFT {level}",
                    },
                    _ => $"UNKNOWN PUBLIC TRANSPORT {subService}",
                },
                _ => $"??? {service}",
            };
        }

        private static CategoryTab SetCategory(ItemClass.Service service, bool outsideConnection)
        {
            if (outsideConnection)
            {
                return CategoryTab.OutsideConnection;
            }
            else if (service == ItemClass.Service.Disaster || service == ItemClass.Service.FireDepartment)
            {
                return CategoryTab.EmergencyVehicles;
            }
            else if (service == ItemClass.Service.HealthCare)
            {
                return CategoryTab.HealthcareVehicles;
            }
            else if (service == ItemClass.Service.PoliceDepartment)
            {
                return CategoryTab.SecurityVehicles;
            }
            else if (service == ItemClass.Service.PublicTransport)
            {
                return CategoryTab.PublicTransport;
            }
            else
            {
                return CategoryTab.OtherServices;
            }
        }

        public Type GetDefType() => sysDefinitions[this].GetType();

        public bool isFromSystem(VehicleInfo info) => info != null && info.m_class.m_service == service && subService == info.m_class.m_subService && info.m_vehicleType == vehicleType && info.m_class.m_level == level;
        public bool isFromSystem(BuildingInfo info) => info != null && info.m_class.m_service == service && subService == info.m_class.m_subService && info.m_class.m_level == level;

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() != typeof(ServiceSystemDefinition))
            {
                return false;
            }
            var other = (ServiceSystemDefinition)obj;

            return level == other.level && service == other.service && subService == other.subService && vehicleType == other.vehicleType && outsideConnection == other.outsideConnection;
        }

        public static bool operator ==(ServiceSystemDefinition a, ServiceSystemDefinition b)
        {
            if (Equals(a, null) || Equals(b, null))
            {
                return Equals(a, null) == Equals(b, null);
            }
            return a.Equals(b);
        }
        public static bool operator !=(ServiceSystemDefinition a, ServiceSystemDefinition b) => !(a == b);

        public static ServiceSystemDefinition from(VehicleInfo info)
        {
            if (info == null)
            {
                return default;
            }
            return sysDefinitions.Keys.FirstOrDefault(x => x.service == info.m_class.m_service && x.subService == info.m_class.m_subService && x.vehicleType == info.m_vehicleType && x.level == info.m_class.m_level && !x.outsideConnection);
        }

        public static ServiceSystemDefinition from(BuildingInfo info, VehicleInfo.VehicleType type)
        {
            if (info == null)
            {
                return default;
            }
            return from(info.GetService(), info.GetSubService(), info.GetClassLevel(), type, info.m_buildingAI is OutsideConnectionAI);
        }

        public static IEnumerable<ServiceSystemDefinition> from(BuildingInfo info)
        {
            if (info == null)
            {
                return new List<ServiceSystemDefinition>();
            }
            return from(info.GetService(), info.GetSubService(), info.GetClassLevel(), info.m_buildingAI is OutsideConnectionAI);
        }

        public static IEnumerable<ServiceSystemDefinition> from(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, bool isOutsideConnection) => sysDefinitions.Keys.Where(x => x.service == service && x.subService == subService && x.level == level && x.outsideConnection == isOutsideConnection);
        public static ServiceSystemDefinition from(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, VehicleInfo.VehicleType type, bool isOutsideConnection) => sysDefinitions.Keys.Where(x => x.service == service && x.subService == subService && x.level == level && x.vehicleType == type && x.outsideConnection == isOutsideConnection).FirstOrDefault();

        public static ServiceSystemDefinition from(SSD index) => m_indexMap.Where(x => x.Value == index).FirstOrDefault().Key;


        public override string ToString() => service.ToString() + "|" + subService.ToString() + "|" + level.ToString() + "|" + vehicleType.ToString() + "|" + outsideConnection;

        public override int GetHashCode()
        {
            int hashCode = 286451371;
            hashCode = hashCode * -1521134295 + service.GetHashCode();
            hashCode = hashCode * -1521134295 + vehicleType.GetHashCode();
            hashCode = hashCode * -1521134295 + subService.GetHashCode();
            hashCode = hashCode * -1521134295 + vehicleType.GetHashCode();
            return hashCode;
        }


        public VehicleInfo GetAModel(ushort buildingId)
        {
            VehicleInfo info = null;
            List<string> assetList = GetBuildingExtension().GetSelectedBasicAssets(buildingId);
            if (assetList == null || assetList.Count == 0)
            {
                assetList = GetDistrictExtension().GetSelectedBasicAssets(BuildingUtils.GetBuildingDistrict(buildingId));
            }
            while (info == null && assetList != null && assetList.Count > 0)
            {
                info = VehicleUtils.GetRandomModel(assetList, out string modelName);
                if (info == null)
                {
                    GetBuildingExtension().RemoveAsset(buildingId, modelName);
                    GetDistrictExtension().RemoveAsset(BuildingUtils.GetBuildingDistrict(buildingId), modelName);
                    assetList.Remove(modelName);
                }
            }
            return info;
        }
        public Color GetModelColor(uint buildingId)
        {
            Color color = GetBuildingExtension().GetColor(buildingId);
            if (color == default)
            {
                color = GetDistrictExtension().GetColor(BuildingUtils.GetBuildingDistrict(buildingId));
            }
            return color;
        }
        internal bool AllowColorChanging() => !notAllowedColorChange.Contains(this);


        private static readonly ServiceSystemDefinition[] notAllowedColorChange = new ServiceSystemDefinition[]
        {
            FIRE_CAR                ,
            HEALTHCARE_CAR          ,
            POLICE_CAR              ,
            ROAD_CAR                ,
            SNOW_CAR                ,
            PRISION_CAR             ,
            TAXI_CAR                ,
        };
    }

    public enum SSD
    {
        DISASTER_CAR,
        DISASTER_HELICOPTER,


        FIRE_CAR,
        FIRE_HELICOPTER,


        GARBAGE_CAR,
        GARBBIO_CAR,



        HEALTHCARE_CAR,
        HEALTHCARE_HELICOPTER,
        DEATHCARE_CAR,


        POLICE_CAR,
        POLICE_HELICOPTER,
        PRISION_CAR,


        SNOW_CAR,
        ROAD_CAR,


        WATER_CAR,
        TAXI_CAR,
        CABLECAR_CABLECAR,
        REG_TRAIN,
        REG_PLANE,
        REG_SHIP,
        CARG_SHIP,
        CARG_TRAIN,
        BEAU_CAR,
        POST_CAR,
        POST_TRK,


        OUT_TRAIN,
        OUT_PLANE,
        OUT_SHIP,
        OUT_ROAD,
    }
    public interface IVMCSysDef
    {
        public abstract ServiceSystemDefinition GetSSD();
        public IVMCBuildingExtension GetExtensionBuilding();
        public IVMCDistrictExtension GetExtensionDistrict();
    }
    public abstract class VMCSysDef<SSD> : SingletonLite<SSD>, IVMCSysDef where SSD : VMCSysDef<SSD>, IVMCSysDef, new()
    {
        public abstract ServiceSystemDefinition GetSSD();
        public IVMCBuildingExtension GetExtensionBuilding() => VMCBuildingInstanceExtensor<SSD>.Instance;
        public IVMCDistrictExtension GetExtensionDistrict() => VMCDistrictExtensor<SSD>.Instance;
    }
    public sealed class VMCSysDefDisCar : VMCSysDef<VMCSysDefDisCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.DISASTER_CAR; }
    public sealed class VMCSysDefDisHel : VMCSysDef<VMCSysDefDisHel> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.DISASTER_HELICOPTER; }
    public sealed class VMCSysDefFirCar : VMCSysDef<VMCSysDefFirCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.FIRE_CAR; }
    public sealed class VMCSysDefFirHel : VMCSysDef<VMCSysDefFirHel> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.FIRE_HELICOPTER; }
    public sealed class VMCSysDefGarCar : VMCSysDef<VMCSysDefGarCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.GARBAGE_CAR; }
    public sealed class VMCSysDefGbcCar : VMCSysDef<VMCSysDefGbcCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.GARBBIO_CAR; }
    public sealed class VMCSysDefHcrCar : VMCSysDef<VMCSysDefHcrCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.HEALTHCARE_CAR; }
    public sealed class VMCSysDefHcrHel : VMCSysDef<VMCSysDefHcrHel> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.HEALTHCARE_HELICOPTER; }
    public sealed class VMCSysDefPolCar : VMCSysDef<VMCSysDefPolCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.POLICE_CAR; }
    public sealed class VMCSysDefPolHel : VMCSysDef<VMCSysDefPolHel> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.POLICE_HELICOPTER; }
    public sealed class VMCSysDefRoaCar : VMCSysDef<VMCSysDefRoaCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.ROAD_CAR; }
    public sealed class VMCSysDefDcrCar : VMCSysDef<VMCSysDefDcrCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.DEATHCARE_CAR; }
    public sealed class VMCSysDefWatCar : VMCSysDef<VMCSysDefWatCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.WATER_CAR; }
    public sealed class VMCSysDefPriCar : VMCSysDef<VMCSysDefPriCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.PRISION_CAR; }
    public sealed class VMCSysDefTaxCar : VMCSysDef<VMCSysDefTaxCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.TAXI_CAR; }
    public sealed class VMCSysDefCcrCcr : VMCSysDef<VMCSysDefCcrCcr> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.CABLECAR_CABLECAR; }
    public sealed class VMCSysDefSnwCar : VMCSysDef<VMCSysDefSnwCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.SNOW_CAR; }
    public sealed class VMCSysDefRegShp : VMCSysDef<VMCSysDefRegShp> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.REG_SHIP; }
    public sealed class VMCSysDefRegTra : VMCSysDef<VMCSysDefRegTra> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.REG_TRAIN; }
    public sealed class VMCSysDefRegPln : VMCSysDef<VMCSysDefRegPln> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.REG_PLANE; }
    public sealed class VMCSysDefCrgTra : VMCSysDef<VMCSysDefCrgTra> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.CARG_TRAIN; }
    public sealed class VMCSysDefCrgShp : VMCSysDef<VMCSysDefCrgShp> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.CARG_SHIP; }
    public sealed class VMCSysDefOutShp : VMCSysDef<VMCSysDefOutShp> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.OUT_SHIP; }
    public sealed class VMCSysDefOutTra : VMCSysDef<VMCSysDefOutTra> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.OUT_TRAIN; }
    public sealed class VMCSysDefOutPln : VMCSysDef<VMCSysDefOutPln> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.OUT_PLANE; }
    public sealed class VMCSysDefOutCar : VMCSysDef<VMCSysDefOutCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.OUT_ROAD; }
    public sealed class VMCSysDefBeaCar : VMCSysDef<VMCSysDefBeaCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.BEAU_CAR; }
    public sealed class VMCSysDefPstCar : VMCSysDef<VMCSysDefPstCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.POST_CAR; }
    public sealed class VMCSysDefPstTrk : VMCSysDef<VMCSysDefPstTrk> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.POST_TRK; }
}
