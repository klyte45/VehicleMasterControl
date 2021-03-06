using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.Math;
using Klyte.Commons.Utils;
using Klyte.VehiclesMasterControl.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klyte.VehiclesMasterControl.Extensors.VehicleExt
{
    public class ServiceSystemDefinition
    {
        public static readonly ServiceSystemDefinition BEAU_CAR = new ServiceSystemDefinition(ItemClass.Service.Beautification, ItemClass.SubService.BeautificationParks, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);

        public static readonly ServiceSystemDefinition DISASTER_CAR = new ServiceSystemDefinition(ItemClass.Service.Disaster, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition DISASTER_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.Disaster, ItemClass.SubService.None, VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level2);

        public static readonly ServiceSystemDefinition FIRE_CAR = new ServiceSystemDefinition(ItemClass.Service.FireDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition FIRE_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.FireDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level1);

        public static readonly ServiceSystemDefinition FISH_TRK = new ServiceSystemDefinition(ItemClass.Service.Fishing, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition FISH_GEN = new ServiceSystemDefinition(ItemClass.Service.Fishing, ItemClass.SubService.None, VehicleInfo.VehicleType.Ship, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition FISH_SLM = new ServiceSystemDefinition(ItemClass.Service.Fishing, ItemClass.SubService.None, VehicleInfo.VehicleType.Ship, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition FISH_SHF = new ServiceSystemDefinition(ItemClass.Service.Fishing, ItemClass.SubService.None, VehicleInfo.VehicleType.Ship, ItemClass.Level.Level3);
        public static readonly ServiceSystemDefinition FISH_TNA = new ServiceSystemDefinition(ItemClass.Service.Fishing, ItemClass.SubService.None, VehicleInfo.VehicleType.Ship, ItemClass.Level.Level4);
        public static readonly ServiceSystemDefinition FISH_ACH = new ServiceSystemDefinition(ItemClass.Service.Fishing, ItemClass.SubService.None, VehicleInfo.VehicleType.Ship, ItemClass.Level.Level5);

        public static readonly ServiceSystemDefinition GARBAGE_CAR = new ServiceSystemDefinition(ItemClass.Service.Garbage, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition GARBBIO_CAR = new ServiceSystemDefinition(ItemClass.Service.Garbage, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition WASTCOL_CAR = new ServiceSystemDefinition(ItemClass.Service.Garbage, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level3);
        public static readonly ServiceSystemDefinition WASTTRN_CAR = new ServiceSystemDefinition(ItemClass.Service.Garbage, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level4);

        public static readonly ServiceSystemDefinition HEALTHCARE_CAR = new ServiceSystemDefinition(ItemClass.Service.HealthCare, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition HEALTHCARE_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.HealthCare, ItemClass.SubService.None, VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level3);
        public static readonly ServiceSystemDefinition DEATHCARE_CAR = new ServiceSystemDefinition(ItemClass.Service.HealthCare, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);

        public static readonly ServiceSystemDefinition INDFARM_TRK = new ServiceSystemDefinition(ItemClass.Service.Industrial, ItemClass.SubService.IndustrialFarming, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition INDFRST_TRK = new ServiceSystemDefinition(ItemClass.Service.Industrial, ItemClass.SubService.IndustrialForestry, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition INDGNRC_TRK = new ServiceSystemDefinition(ItemClass.Service.Industrial, ItemClass.SubService.IndustrialGeneric, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition INDOIL_TRK = new ServiceSystemDefinition(ItemClass.Service.Industrial, ItemClass.SubService.IndustrialOil, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition INDORE_TRK = new ServiceSystemDefinition(ItemClass.Service.Industrial, ItemClass.SubService.IndustrialOre, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);

        public static readonly ServiceSystemDefinition AIR_CLB = new ServiceSystemDefinition(ItemClass.Service.Monument, ItemClass.SubService.None, VehicleInfo.VehicleType.Plane, ItemClass.Level.Level5);

        public static readonly ServiceSystemDefinition INDCMN_VAN = new ServiceSystemDefinition(ItemClass.Service.PlayerIndustry, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition INDCMN_TRK = new ServiceSystemDefinition(ItemClass.Service.PlayerIndustry, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition INDCMN_FRM_TRAILER = new ServiceSystemDefinition(ItemClass.Service.PlayerIndustry, ItemClass.SubService.PlayerIndustryFarming, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);

        public static readonly ServiceSystemDefinition POLICE_CAR = new ServiceSystemDefinition(ItemClass.Service.PoliceDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition POLICE_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.PoliceDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level3);
        public static readonly ServiceSystemDefinition PRISION_CAR = new ServiceSystemDefinition(ItemClass.Service.PoliceDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level4);

        public static readonly ServiceSystemDefinition SNOW_CAR = new ServiceSystemDefinition(ItemClass.Service.Road, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level4);

        public static readonly ServiceSystemDefinition TAXI_CAR = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTaxi, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition CABLECAR_CABLECAR = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportCableCar, VehicleInfo.VehicleType.CableCar, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition REG_TRAIN = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain, VehicleInfo.VehicleType.Train, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition REG_PLANE = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPlane, VehicleInfo.VehicleType.Plane, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition REG_SHIP = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportShip, VehicleInfo.VehicleType.Ship, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition CARG_PLANE = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPlane, VehicleInfo.VehicleType.Plane, ItemClass.Level.Level4);
        public static readonly ServiceSystemDefinition CARG_SHIP = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportShip, VehicleInfo.VehicleType.Ship, ItemClass.Level.Level4);
        public static readonly ServiceSystemDefinition CARG_TRAIN = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain, VehicleInfo.VehicleType.Train, ItemClass.Level.Level4);
        public static readonly ServiceSystemDefinition POST_CAR = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPost, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition POST_TRK = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPost, VehicleInfo.VehicleType.Car, ItemClass.Level.Level5);
        public static readonly ServiceSystemDefinition BALLOON = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTours, VehicleInfo.VehicleType.Balloon, ItemClass.Level.Level4);

        public static readonly ServiceSystemDefinition BICYCLE_CHILD = new ServiceSystemDefinition(ItemClass.Service.Residential, ItemClass.SubService.ResidentialHigh, VehicleInfo.VehicleType.Bicycle, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition BICYCLE_ADULT = new ServiceSystemDefinition(ItemClass.Service.Residential, ItemClass.SubService.ResidentialHigh, VehicleInfo.VehicleType.Bicycle, ItemClass.Level.Level2);

        public static readonly ServiceSystemDefinition WATER_CAR = new ServiceSystemDefinition(ItemClass.Service.Water, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition ROAD_CAR = new ServiceSystemDefinition(ItemClass.Service.Road, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);

        public static readonly ServiceSystemDefinition OUT_TRAIN = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain, VehicleInfo.VehicleType.Train, ItemClass.Level.Level1, true);
        public static readonly ServiceSystemDefinition OUT_PLANE = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPlane, VehicleInfo.VehicleType.Plane, ItemClass.Level.Level1, true);
        public static readonly ServiceSystemDefinition OUT_SHIP = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportShip, VehicleInfo.VehicleType.Ship, ItemClass.Level.Level1, true);
        public static readonly ServiceSystemDefinition OUT_ROAD = new ServiceSystemDefinition(ItemClass.Service.Road, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level5, true);
        public static readonly ServiceSystemDefinition OUT_BUS = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportBus, VehicleInfo.VehicleType.Car, ItemClass.Level.Level3, true);

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
            [OUT_BUS] = SSD.OUT_BUS,

            [BICYCLE_ADULT] = SSD.BICYCLE_ADULT,
            [BICYCLE_CHILD] = SSD.BICYCLE_CHILD,
            [CARG_PLANE] = SSD.CARG_PLANE,
            [FISH_TRK] = SSD.FISH_TRK,
            [FISH_GEN] = SSD.FISH_GEN,
            [FISH_SLM] = SSD.FISH_SLM,
            [FISH_SHF] = SSD.FISH_SHF,
            [FISH_TNA] = SSD.FISH_TNA,
            [FISH_ACH] = SSD.FISH_ACH,
            [INDCMN_FRM_TRAILER] = SSD.INDCMN_FRM_TRAILER,
            [INDCMN_TRK] = SSD.INDCMN_TRK,
            [INDCMN_VAN] = SSD.INDCMN_VAN,
            [INDFARM_TRK] = SSD.INDFARM_TRK,
            [INDFRST_TRK] = SSD.INDFRST_TRK,
            [INDGNRC_TRK] = SSD.INDGNRC_TRK,
            [INDOIL_TRK] = SSD.INDOIL_TRK,
            [INDORE_TRK] = SSD.INDORE_TRK,
            [WASTCOL_CAR] = SSD.WASTCOL_CAR,
            [WASTTRN_CAR] = SSD.WASTTRN_CAR,
            [AIR_CLB] = SSD.AIR_CLB,
            [BALLOON] = SSD.BALLOON
        };

        public static Dictionary<ServiceSystemDefinition, IVMCSysDef> sysDefinitions
        {
            get
            {
                if (VehiclesMasterControlMod.Controller is null)
                {
                    LogUtils.DoErrorLog("ERROR: The mod controller wasn't properly loaded! This may be caused by a mod conflict or a unsupported game loading mode.");
                    return null;
                }
                if (VehiclesMasterControlMod.Controller.m_sysDefinitions.Count == 0)
                {
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[GARBAGE_CAR] = SingletonLite<VMCSysDefGarCar>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[DEATHCARE_CAR] = SingletonLite<VMCSysDefDcrCar>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[REG_PLANE] = SingletonLite<VMCSysDefRegPln>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[REG_TRAIN] = SingletonLite<VMCSysDefRegTra>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[REG_SHIP] = SingletonLite<VMCSysDefRegShp>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[FIRE_CAR] = SingletonLite<VMCSysDefFirCar>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[HEALTHCARE_CAR] = SingletonLite<VMCSysDefHcrCar>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[POLICE_CAR] = SingletonLite<VMCSysDefPolCar>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[CARG_TRAIN] = SingletonLite<VMCSysDefCrgTra>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[CARG_SHIP] = SingletonLite<VMCSysDefCrgShp>.instance;

                    VehiclesMasterControlMod.Controller.m_sysDefinitions[OUT_PLANE] = SingletonLite<VMCSysDefOutPln>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[OUT_TRAIN] = SingletonLite<VMCSysDefOutTra>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[OUT_SHIP] = SingletonLite<VMCSysDefOutShp>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[OUT_ROAD] = SingletonLite<VMCSysDefOutCar>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[OUT_BUS] = SingletonLite<VMCSysDefOutBus>.instance;

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.AfterDark))
                    //{
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[PRISION_CAR] = SingletonLite<VMCSysDefPriCar>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[TAXI_CAR] = SingletonLite<VMCSysDefTaxCar>.instance;
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.Snowfall))
                    //{
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[ROAD_CAR] = SingletonLite<VMCSysDefRoaCar>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[SNOW_CAR] = SingletonLite<VMCSysDefSnwCar>.instance;
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.NaturalDisasters))
                    //{
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[WATER_CAR] = SingletonLite<VMCSysDefWatCar>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[DISASTER_CAR] = SingletonLite<VMCSysDefDisCar>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[DISASTER_HELICOPTER] = SingletonLite<VMCSysDefDisHel>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[FIRE_HELICOPTER] = SingletonLite<VMCSysDefFirHel>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[HEALTHCARE_HELICOPTER] = SingletonLite<VMCSysDefHcrHel>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[POLICE_HELICOPTER] = SingletonLite<VMCSysDefPolHel>.instance;
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.InMotion))
                    //{
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[CABLECAR_CABLECAR] = SingletonLite<VMCSysDefCcrCcr>.instance;
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.GreenCities))
                    //{
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[GARBBIO_CAR] = SingletonLite<VMCSysDefGbcCar>.instance;
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.Parks))
                    //{
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[BEAU_CAR] = SingletonLite<VMCSysDefBeaCar>.instance;
                    //}
                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.Industry))
                    //{
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[POST_CAR] = SingletonLite<VMCSysDefPstCar>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[POST_TRK] = SingletonLite<VMCSysDefPstTrk>.instance;
                    //}
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[BALLOON] = SingletonLite<VMCSysDefTouBal>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[AIR_CLB] = SingletonLite<VMCSysDefClbPln>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[BICYCLE_ADULT] = SingletonLite<VMCSysDefAdtBcc>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[BICYCLE_CHILD] = SingletonLite<VMCSysDefChdBcc>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[CARG_PLANE] = SingletonLite<VMCSysDefCrgPln>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[FISH_TRK] = SingletonLite<VMCSysDefFshTrk>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[FISH_GEN] = SingletonLite<VMCSysDefFshGen>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[FISH_SLM] = SingletonLite<VMCSysDefFshSlm>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[FISH_SHF] = SingletonLite<VMCSysDefFshShf>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[FISH_TNA] = SingletonLite<VMCSysDefFshTna>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[FISH_ACH] = SingletonLite<VMCSysDefFshAch>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[INDCMN_FRM_TRAILER] = SingletonLite<VMCSysDefIfmTrl>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[INDCMN_TRK] = SingletonLite<VMCSysDefIndTrk>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[INDCMN_VAN] = SingletonLite<VMCSysDefIndVan>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[INDFARM_TRK] = SingletonLite<VMCSysDefIfmTrk>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[INDFRST_TRK] = SingletonLite<VMCSysDefIfrTrk>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[INDGNRC_TRK] = SingletonLite<VMCSysDefIgnTrk>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[INDOIL_TRK] = SingletonLite<VMCSysDefIolTrk>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[INDORE_TRK] = SingletonLite<VMCSysDefIorTrk>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[WASTCOL_CAR] = SingletonLite<VMCSysDefWstCol>.instance;
                    VehiclesMasterControlMod.Controller.m_sysDefinitions[WASTTRN_CAR] = SingletonLite<VMCSysDefWstTrn>.instance;
                }
                return VehiclesMasterControlMod.Controller.m_sysDefinitions;
            }
        }


        public bool AllowDistrictServiceRestrictions => AllowRestrictions;
        public CategoryTab Category { get; }
        public string NameForServiceSystem { get; }
        public string FgIconServiceSystem { get; }
        public string IconServiceSystem { get; }



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

        private static string SetFgIcon(ItemClass.Service service, ItemClass.SubService subService, VehicleInfo.VehicleType vehicleType, ItemClass.Level level, bool outsideConnection) => "K45_VMC_" + outsideConnection switch
        {
            true => "OutsideIndicator",
            false => vehicleType switch
            {
                VehicleInfo.VehicleType.Helicopter => "HelicopterIndicator",
                _ => service switch
                {
                    ItemClass.Service.Garbage => level switch
                    {
                        ItemClass.Level.Level3 => "CargoIndicator",
                        ItemClass.Level.Level4 => "OutsideIndicator",
                        _ => ""
                    },
                    ItemClass.Service.PublicTransport => vehicleType switch
                    {
                        VehicleInfo.VehicleType.Balloon => "",
                        _ => level switch
                        {
                            ItemClass.Level.Level4 => "CargoIndicator",
                            ItemClass.Level.Level5 => "CargoIndicator",
                            _ => ""
                        }
                    },
                    ItemClass.Service.Fishing => level switch
                    {
                        ItemClass.Level.Level2 => "AIndicator",
                        ItemClass.Level.Level3 => "BioIndicator",
                        ItemClass.Level.Level4 => "CargoIndicator",
                        ItemClass.Level.Level5 => "DIndicator",
                        _ => ""
                    },
                    ItemClass.Service.Residential => level switch
                    {
                        ItemClass.Level.Level2 => "AIndicator",
                        ItemClass.Level.Level1 => "CargoIndicator",
                        _ => ""
                    },
                    _ => ""
                },
            }
        };

        private static string SetIcon(ItemClass.Service service, ItemClass.SubService subService, VehicleInfo.VehicleType vehicleType, ItemClass.Level level, bool outsideConnection) => service switch
        {
            ItemClass.Service.Beautification => "ToolbarIconBeautification",
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
            ItemClass.Service.Fishing => vehicleType switch
            {
                VehicleInfo.VehicleType.Car => "IconServiceVehicle",
                VehicleInfo.VehicleType.Ship => "IconCargoShip",
                _ => "UNKNOWN FISH",
            },
            ItemClass.Service.Garbage => level switch
            {
                ItemClass.Level.Level1 => "InfoIconGarbage",
                ItemClass.Level.Level2 => "IconPolicyRecycling",
                ItemClass.Level.Level3 => "InfoIconGarbage",
                ItemClass.Level.Level4 => "InfoIconGarbage",
                _ => "UNKNOWN GARBAGE",
            },
            ItemClass.Service.HealthCare => level switch
            {
                ItemClass.Level.Level1 => "ToolbarIconHealthcare",
                ItemClass.Level.Level2 => "ToolbarIconHealthcareHovered",
                ItemClass.Level.Level3 => "ToolbarIconHealthcare",
                _ => "UNKNOWN HEALTHCARE",
            },
            ItemClass.Service.Industrial => subService switch
            {
                ItemClass.SubService.IndustrialFarming => "IconPolicyFarming",
                ItemClass.SubService.IndustrialForestry => "IconPolicyForest",
                ItemClass.SubService.IndustrialGeneric => "ToolbarIconGarbage",
                ItemClass.SubService.IndustrialOil => "IconPolicyOil",
                ItemClass.SubService.IndustrialOre => "IconPolicyOre",

                _ => "UNKNOWN INDUSTRIAL",
            },
            ItemClass.Service.Monument => vehicleType switch
            {
                VehicleInfo.VehicleType.Rocket => "ParkLevelStar",
                VehicleInfo.VehicleType.Plane => "SubBarPublicTransportPlane",
                _ => "UNKNOWN MONUMENT",
            },
            ItemClass.Service.PlayerIndustry => subService switch
            {
                ItemClass.SubService.PlayerIndustryFarming => "SubBarIndustryFarming",
                ItemClass.SubService.None => level switch
                {
                    ItemClass.Level.Level1 => "SubBarDistrictSpecializationIndustrial",
                    ItemClass.Level.Level2 => "resourceIconLuxuryProducts",
                    _ => "UNKNOWN PLAYERINDUSTRY NONE LEVEL",
                },

                _ => "UNKNOWN PLAYERINDUSTRY",
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
            ItemClass.Service.PublicTransport => subService switch
            {
                ItemClass.SubService.PublicTransportTours => vehicleType switch
                {
                    VehicleInfo.VehicleType.Balloon => "IconBalloonTours",
                    _ => "UNKNOWN PUBLICTRANSPORTTOURS"
                },
                ItemClass.SubService.PublicTransportBus => "SubBarPublicTransportBus",
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
            ItemClass.Service.Residential => vehicleType switch
            {
                VehicleInfo.VehicleType.Bicycle => "IconPolicyEncourageBiking",
                _ => "UNKNOWN RESIDENTIAL"
            },
            _ => $"??? {service}",
        };

        private static string SetNameForServiceSystem(
            ItemClass.Service service,
        ItemClass.SubService subService,
        VehicleInfo.VehicleType vehicleType,
        ItemClass.Level level,
        bool outsideConnection) => service switch
        {
            ItemClass.Service.Beautification => Locale.Get("VEHICLE_TITLE", "Park Staff Vehicle 01"),
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
            ItemClass.Service.Fishing => vehicleType switch
            {
                VehicleInfo.VehicleType.Car => Locale.Get("VEHICLE_TITLE", "Fish Truck 01"),
                VehicleInfo.VehicleType.Ship => level switch
                {
                    ItemClass.Level.Level1 => Locale.Get("VEHICLE_TITLE", "Fishing Boat 01"),
                    ItemClass.Level.Level2 => Locale.Get("VEHICLE_TITLE", "Fishing Boat 02"),
                    ItemClass.Level.Level3 => Locale.Get("VEHICLE_TITLE", "Fishing Boat 03"),
                    ItemClass.Level.Level4 => Locale.Get("VEHICLE_TITLE", "Fishing Boat 04"),
                    ItemClass.Level.Level5 => Locale.Get("VEHICLE_TITLE", "Fishing Boat 05"),
                    _ => "UNKNOWN FISH BOAT"
                },
                _ => "UNKNOWN FISH",
            },
            ItemClass.Service.Garbage => level switch
            {
                ItemClass.Level.Level1 => Locale.Get("VEHICLE_TITLE", "Garbage Truck"),
                ItemClass.Level.Level2 => Locale.Get("VEHICLE_TITLE", "Biofuel Garbage Truck 01"),
                ItemClass.Level.Level3 => Locale.Get("VEHICLE_TITLE", "Waste Collection Truck"),
                ItemClass.Level.Level4 => Locale.Get("VEHICLE_TITLE", "Waste Transfer Truck"),
                _ => "UNKNOWN GARBAGE",
            },
            ItemClass.Service.HealthCare => level switch
            {
                ItemClass.Level.Level1 => Locale.Get("VEHICLE_TITLE", "Ambulance"),
                ItemClass.Level.Level2 => Locale.Get("VEHICLE_TITLE", "Hearse"),
                ItemClass.Level.Level3 => Locale.Get("VEHICLE_TITLE", "Medical Helicopter"),
                _ => "UNKNOWN HEALTHCARE",
            },
            ItemClass.Service.Industrial => subService switch
            {
                ItemClass.SubService.IndustrialFarming => Locale.Get("VEHICLE_TITLE", "Farm Truck 01"),
                ItemClass.SubService.IndustrialForestry => Locale.Get("VEHICLE_TITLE", "Forestry Truck"),
                ItemClass.SubService.IndustrialGeneric => Locale.Get("VEHICLE_TITLE", "Lorry"),
                ItemClass.SubService.IndustrialOil => Locale.Get("VEHICLE_TITLE", "Oil Truck"),
                ItemClass.SubService.IndustrialOre => Locale.Get("VEHICLE_TITLE", "Ore Truck"),
                _ => "UNKNOWN INDUSTRIAL",
            },
            ItemClass.Service.Monument => vehicleType switch
            {
                VehicleInfo.VehicleType.Plane => Locale.Get("VEHICLE_TITLE", "Aviation Club Plane 01"),
                _ => "UNKNOWN MONUMENT",
            },
            ItemClass.Service.PlayerIndustry => subService switch
            {
                ItemClass.SubService.PlayerIndustryFarming => Locale.Get("VEHICLE_TITLE", "Truck Animal"),
                ItemClass.SubService.None => level switch
                {
                    ItemClass.Level.Level1 => Locale.Get("VEHICLE_TITLE", "Delivery Van 01"),
                    ItemClass.Level.Level2 => Locale.Get("WAREHOUSEPANEL_RESOURCE", "LuxuryProducts"),
                    _ => "UNKNOWN PLAYERINDUSTRY NONE LEVEL",
                },

                _ => "UNKNOWN PLAYERINDUSTRY",
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
            ItemClass.Service.PublicTransport => subService switch
            {
                ItemClass.SubService.PublicTransportTours => vehicleType switch
                {
                    VehicleInfo.VehicleType.Balloon => Locale.Get("VEHICLE_TITLE", "Hot Air Balloon 01"),
                    _ => "UNKNOWN PUBLICTRANSPORTTOURS"
                },
                ItemClass.SubService.PublicTransportPost => level switch
                {
                    ItemClass.Level.Level2 => Locale.Get("VEHICLE_TITLE", "Post Vehicle 01"),
                    ItemClass.Level.Level5 => Locale.Get("VEHICLE_TITLE", "Post Truck 01"),
                    _ => $"UNKNOWN POST {level}",
                },
                ItemClass.SubService.PublicTransportTaxi => Locale.Get("VEHICLE_TITLE", "Taxi"),
                ItemClass.SubService.PublicTransportCableCar => Locale.Get("VEHICLE_TITLE", "Cable Car"),
                ItemClass.SubService.PublicTransportBus => Locale.Get("VEHICLE_TITLE", "Intercity Bus"),
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
                    ItemClass.Level.Level4 => Locale.Get("VEHICLE_TITLE", "Cargo Airplane 01"),
                    _ => $"UNKNOWN AIRCRAFT {level}",
                },
                _ => $"UNKNOWN PUBLIC TRANSPORT {subService}",
            },
            ItemClass.Service.Residential => vehicleType switch
            {
                VehicleInfo.VehicleType.Bicycle => level switch
                {
                    ItemClass.Level.Level1 => $"{Locale.Get("VEHICLE_TITLE", "Bicycle Child")} ({Locale.Get("ZONEDBUILDING_CHILDREN")})",
                    ItemClass.Level.Level2 => $"{Locale.Get("VEHICLE_TITLE", "Bicycle")} ({Locale.Get("ZONEDBUILDING_ADULTS")})",
                    _ => "UNK BICYCLE LVL RESIDENTIAL"
                },
                _ => "UNKNOWN RESIDENTIAL"
            },
            ItemClass.Service.Water => level switch
            {
                ItemClass.Level.Level1 => Locale.Get("VEHICLE_TITLE", "Water Pumping Truck"),
                _ => "UNKNOWN WATER",
            },
            _ => $"??? {service}",
        };
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
            else if (service == ItemClass.Service.Fishing)
            {
                return CategoryTab.Fish;
            }
            else if (service == ItemClass.Service.Garbage)
            {
                return CategoryTab.Garbage;
            }
            else if (service == ItemClass.Service.Industrial || service == ItemClass.Service.PlayerIndustry)
            {
                return CategoryTab.Industry;
            }
            else if (service == ItemClass.Service.Residential)
            {
                return CategoryTab.Residential;
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


        public VehicleInfo GetAModel(ref Randomizer rand, ushort buildingId)
        {
            VehicleInfo info = null;
            var ssd = this;
            var assetList = ExtensionStaticExtensionMethods.GetEffectiveAssetList(buildingId, ref ssd);
            //LogUtils.DoLog($"assetList.Count = {assetList.Count} | buildingId= {buildingId} | GetBuildingExtension() {GetBuildingExtension()} | GetDistrictExtension() {GetDistrictExtension()} ");
            while (info == null && assetList != null && assetList.Count > 0)
            {
                info = VehicleUtils.GetRandomModel(ref rand, assetList, out string modelName);
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
                var districtId = BuildingUtils.GetBuildingDistrict(buildingId);
                color = GetDistrictExtension().GetColor(districtId);
                if (color == default && districtId != 0)
                {
                    color = GetDistrictExtension().GetColor(0);
                }
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
        CARG_PLANE,
        BEAU_CAR,
        POST_CAR,
        POST_TRK,


        OUT_TRAIN,
        OUT_PLANE,
        OUT_SHIP,
        OUT_ROAD,
        OUT_BUS,

        BICYCLE_ADULT,
        BICYCLE_CHILD,
        FISH_TRK,
        FISH_GEN,
        FISH_SLM,
        FISH_SHF,
        FISH_TNA,
        FISH_ACH,
        INDCMN_FRM_TRAILER,
        INDCMN_TRK,
        INDCMN_VAN,
        INDFARM_TRK,
        INDFRST_TRK,
        INDGNRC_TRK,
        INDOIL_TRK,
        INDORE_TRK,
        WASTCOL_CAR,
        WASTTRN_CAR,
        AIR_CLB,
        BALLOON







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
    public sealed class VMCSysDefOutBus : VMCSysDef<VMCSysDefOutBus> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.OUT_BUS; }
    public sealed class VMCSysDefBeaCar : VMCSysDef<VMCSysDefBeaCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.BEAU_CAR; }
    public sealed class VMCSysDefPstCar : VMCSysDef<VMCSysDefPstCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.POST_CAR; }
    public sealed class VMCSysDefPstTrk : VMCSysDef<VMCSysDefPstTrk> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.POST_TRK; }
    public sealed class VMCSysDefTouBal : VMCSysDef<VMCSysDefTouBal> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.BALLOON; }
    public sealed class VMCSysDefClbPln : VMCSysDef<VMCSysDefClbPln> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.AIR_CLB; }
    public sealed class VMCSysDefAdtBcc : VMCSysDef<VMCSysDefAdtBcc> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.BICYCLE_ADULT; }
    public sealed class VMCSysDefChdBcc : VMCSysDef<VMCSysDefChdBcc> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.BICYCLE_CHILD; }
    public sealed class VMCSysDefCrgPln : VMCSysDef<VMCSysDefCrgPln> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.CARG_PLANE; }
    public sealed class VMCSysDefFshTrk : VMCSysDef<VMCSysDefFshTrk> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.FISH_TRK; }
    public sealed class VMCSysDefFshGen : VMCSysDef<VMCSysDefFshGen> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.FISH_GEN; }
    public sealed class VMCSysDefFshSlm : VMCSysDef<VMCSysDefFshSlm> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.FISH_SLM; }
    public sealed class VMCSysDefFshShf : VMCSysDef<VMCSysDefFshShf> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.FISH_SHF; }
    public sealed class VMCSysDefFshTna : VMCSysDef<VMCSysDefFshTna> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.FISH_TNA; }
    public sealed class VMCSysDefFshAch : VMCSysDef<VMCSysDefFshAch> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.FISH_ACH; }
    public sealed class VMCSysDefIfmTrl : VMCSysDef<VMCSysDefIfmTrl> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.INDCMN_FRM_TRAILER; }
    public sealed class VMCSysDefIndTrk : VMCSysDef<VMCSysDefIndTrk> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.INDCMN_TRK; }
    public sealed class VMCSysDefIndVan : VMCSysDef<VMCSysDefIndVan> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.INDCMN_VAN; }
    public sealed class VMCSysDefIfmTrk : VMCSysDef<VMCSysDefIfmTrk> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.INDFARM_TRK; }
    public sealed class VMCSysDefIfrTrk : VMCSysDef<VMCSysDefIfrTrk> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.INDFRST_TRK; }
    public sealed class VMCSysDefIgnTrk : VMCSysDef<VMCSysDefIgnTrk> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.INDGNRC_TRK; }
    public sealed class VMCSysDefIolTrk : VMCSysDef<VMCSysDefIolTrk> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.INDOIL_TRK; }
    public sealed class VMCSysDefIorTrk : VMCSysDef<VMCSysDefIorTrk> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.INDORE_TRK; }
    public sealed class VMCSysDefWstCol : VMCSysDef<VMCSysDefWstCol> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.WASTCOL_CAR; }
    public sealed class VMCSysDefWstTrn : VMCSysDef<VMCSysDefWstTrn> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.WASTTRN_CAR; }

}
