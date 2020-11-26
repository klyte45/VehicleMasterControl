using ColossalFramework;
using Klyte.Commons.Utils;
using Klyte.VehiclesMasterControl.Extensors.VehicleExt;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klyte.VehiclesMasterControl.Utils
{
    internal class VMCBuildingUtils
    {
        public static void CalculateOwnVehicles(ref Building data, ref int count, ref int cargo, ref int capacity, ref int inbound, ref int outbound)
        {
            VehicleManager instance = Singleton<VehicleManager>.instance;
            ushort num = data.m_ownVehicles;
            int num2 = 0;
            while (num != 0)
            {
                VehicleInfo info = instance.m_vehicles.m_buffer[num].Info;
                info.m_vehicleAI.GetSize(num, ref instance.m_vehicles.m_buffer[num], out int a, out int num3);
                cargo += Mathf.Min(a, num3);
                capacity += num3;
                count++;
                if ((instance.m_vehicles.m_buffer[num].m_flags & (Vehicle.Flags.Importing)) != 0)
                {
                    inbound++;
                }
                if ((instance.m_vehicles.m_buffer[num].m_flags & (Vehicle.Flags.Exporting)) != 0)
                {
                    outbound++;
                }

                num = instance.m_vehicles.m_buffer[num].m_nextOwnVehicle;
                if (++num2 > 16384)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }
        }

        public static int GetMaxVehiclesBuilding(ref Building b, VehicleInfo.VehicleType vehicleType, ItemClass.Level level)
        {
            switch (b.Info.m_buildingAI)
            {
                case var buildingAi when buildingAi is CableCarStationAI target:
                    return target.m_maxVehicleCount;
                case var buildingAi when buildingAi is CargoHarborAI target:
                    return 999999;
                case var buildingAi when buildingAi is CargoStationAI target:
                    return 999999;
                case var buildingAi when buildingAi is CemeteryAI target:
                    return target.m_hearseCount;
                case var buildingAi when buildingAi is CommercialBuildingAI target:
                    return 999999;
                case var buildingAi when buildingAi is DepotAI target:
                    return target.m_maxVehicleCount;
                case var buildingAi when buildingAi is DisasterResponseBuildingAI target:
                    return vehicleType == VehicleInfo.VehicleType.Car ? target.m_vehicleCount : target.m_helicopterCount;
                case var buildingAi when buildingAi is FireStationAI target:
                    return vehicleType == VehicleInfo.VehicleType.Car ? target.m_fireTruckCount : 0;
                case var buildingAi when buildingAi is FishFarmAI target:
                    return target.m_outputVehicleCount;
                case var buildingAi when buildingAi is FishingHarborAI target:
                    return vehicleType == VehicleInfo.VehicleType.Ship ? target.m_boatCount : target.m_outputVehicleCount;
                case var buildingAi when buildingAi is HarborAI target:
                    return target.m_maxVehicleCount;
                case var buildingAi when buildingAi is HelicopterDepotAI target:
                    return target.m_helicopterCount;
                case var buildingAi when buildingAi is HospitalAI target:
                    return vehicleType == VehicleInfo.VehicleType.Car ? target.AmbulanceCount : 0;
                case var buildingAi when buildingAi is IndustrialBuildingAI target:
                    return 9999999;
                case var buildingAi when buildingAi is IndustrialExtractorAI target:
                    return 999999;
                case var buildingAi when buildingAi is IndustryBuildingAI target:
                    return 99999;
                case var buildingAi when buildingAi is LandfillSiteAI target:
                    return target.m_garbageTruckCount;
                case var buildingAi when buildingAi is LivestockExtractorAI target:
                    return 9999999;
                case var buildingAi when buildingAi is MaintenanceDepotAI target:
                    return target.m_maintenanceTruckCount;
                case var buildingAi when buildingAi is MedicalCenterAI target:
                    return target.AmbulanceCount;
                case var buildingAi when buildingAi is OfficeBuildingAI target:
                    return 9999999;
                case var buildingAi when buildingAi is OutsideConnectionAI target:
                    return 999999;
                case var buildingAi when buildingAi is PoliceStationAI target:
                    return vehicleType == VehicleInfo.VehicleType.Car ? target.m_policeCarCount : 0;
                case var buildingAi when buildingAi is PostOfficeAI target:
                    return level == ServiceSystemDefinition.POST_CAR.level ? target.m_postVanCount : target.m_postTruckCount;
                case var buildingAi when buildingAi is PrivateAirportAI target:
                    return target.m_vehicleCount;
                case var buildingAi when buildingAi is ShelterAI target:
                    return target.m_evacuationBusCount;
                case var buildingAi when buildingAi is SnowDumpAI target:
                    return target.m_snowTruckCount;
                case var buildingAi when buildingAi is TaxiStandAI target:
                    return target.m_maxVehicleCount;
                case var buildingAi when buildingAi is TourBuildingAI target:
                    return target.m_vehicleCount;
                case var buildingAi when buildingAi is TransportStationAI target:
                    return target.m_maxVehicleCount;
                case var buildingAi when buildingAi is UltimateRecyclingPlantAI target:
                    return target.m_garbageTruckCount;
                case var buildingAi when buildingAi is UniqueFactoryAI target:
                    return target.m_outputVehicleCount;
                case var buildingAi when buildingAi is WarehouseAI target:
                    return target.m_truckCount;
            }
            LogUtils.DoLog($"NOT FOUND COUNT FOR: {b.Info.m_buildingAI.GetType()}!");
            return 0;
        }


        public static int GetProductionRate(ref Building b)
        {
            int budget = Singleton<EconomyManager>.instance.GetBudget(b.Info.m_class);
            return PlayerBuildingAI.GetProductionRate(100, budget);
        }

        public static List<ushort> getAllBuildingsFromCity(ref ServiceSystemDefinition ssd, int? districtId = null, bool strict = false, bool mustAllowSpawn = false)
        {
            var saida = new List<ushort>();
            BuildingManager bm = Singleton<BuildingManager>.instance;
            FastList<ushort> buildings;
            IVMCDistrictExtension ext = ssd.GetDistrictExtension();
            if (ssd.outsideConnection)
            {
                buildings = bm.GetOutsideConnections();
            }
            else
            {
                buildings = bm.GetServiceBuildings(ssd.service);
            }

            LogUtils.DoLog("getAllBuildingsFromCity ({0}) buildings = {1} (s={2})", ssd, buildings.ToArray(), buildings.m_size);

            foreach (ushort i in buildings)
            {
                ref Building building = ref bm.m_buildings.m_buffer[i];
                if (ssd.isFromSystem(building.Info))
                {
                    if (districtId != null && ssd.AllowRestrictions)
                    {
                        byte buildingDistrict = DistrictManager.instance.GetDistrict(bm.m_buildings.m_buffer[i].m_position);
                        if (districtId != buildingDistrict && (strict || (!ext.GetAllowServeOtherDistricts(buildingDistrict) ?? VehiclesMasterControlMod.allowServeOtherDistrictsAsDefault)))
                        {
                            continue;
                        }
                    }
                    if (mustAllowSpawn)
                    {
                        int max = GetMaxVehiclesBuilding(ref building, ssd.vehicleType, ssd.level);
                        int count = 0;
                        int cargo = 0;
                        int capacity = 0;
                        int inbound = 0;
                        int outbound = 0;
                        VMCBuildingUtils.CalculateOwnVehicles(ref bm.m_buildings.m_buffer[i], ref count, ref cargo, ref capacity, ref inbound, ref outbound);
                        if (count >= max)
                        {
                            continue;
                        }
                    }
                    saida.Add(i);
                }
            }
            LogUtils.DoLog("getAllBuildingsFromCity ({0}) buildings = {1} (s={2}); saida.sz = {3}", ssd, buildings.ToArray(), buildings.m_size, saida.Count);
            return saida;
        }
    }
}
