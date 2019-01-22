using ColossalFramework;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.Overrides;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ItemClass;

namespace Klyte.ServiceVehiclesManager.Utils
{
    class SVMBuildingUtils
    {
        public static void CalculateOwnVehicles(ushort buildingID, ref Building data, IEnumerable<TransferManager.TransferReason> materials, ref int count, ref int cargo, ref int capacity, ref int inbound, ref int outbound)
        {
            VehicleManager instance = Singleton<VehicleManager>.instance;
            ushort num = data.m_ownVehicles;
            int num2 = 0;
            while (num != 0)
            {
                if (materials.Contains((TransferManager.TransferReason)instance.m_vehicles.m_buffer[num].m_transferType))
                {
                    VehicleInfo info = instance.m_vehicles.m_buffer[(int)num].Info;
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
                }
                num = instance.m_vehicles.m_buffer[num].m_nextOwnVehicle;
                if (++num2 > 16384)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }
        }

        public static int GetMaxVehiclesBuilding(ushort buildingID, VehicleInfo.VehicleType type, Level level)
        {
            Building b = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID];
            var ext = SVMBuildingAIOverrideUtils.getBuildingOverrideExtensionStrict(b.Info);
            var maxField = ext.GetVehicleMaxCountField(type, level);
            if (maxField == null)
            {
                return 0xFFFFFF;
            }
            return (SVMUtils.GetPrivateField<int>(b.Info.GetAI(), maxField) * SVMBuildingUtils.GetProductionRate(ref b) / 100);
        }

        public static int GetProductionRate(ref Building b)
        {
            int budget = Singleton<EconomyManager>.instance.GetBudget(b.Info.m_class);
            return PlayerBuildingAI.GetProductionRate(100, budget);
        }

        public static List<ushort> getAllBuildingsFromCity(ServiceSystemDefinition ssd, int? districtId = null, bool strict = false, bool mustAllowSpawn = false)
        {
            List<ushort> saida = new List<ushort>();
            var bm = Singleton<BuildingManager>.instance;
            FastList<ushort> buildings;
            var ext = ssd.GetTransportExtension();
            if (ssd.outsideConnection)
            {
                buildings = bm.GetOutsideConnections();
            }
            else
            {
                buildings = bm.GetServiceBuildings(ssd.service);
            }

            SVMUtils.doLog("getAllBuildingsFromCity ({0}) buildings = {1} (s={2})", ssd, buildings.ToArray(), buildings.m_size);

            foreach (ushort i in buildings)
            {
                if (ssd.isFromSystem(bm.m_buildings.m_buffer[i].Info))
                {
                    if (districtId != null && ext.GetAllowDistrictServiceRestrictions())
                    {
                        var buildingDistrict = DistrictManager.instance.GetDistrict(bm.m_buildings.m_buffer[i].m_position);
                        if (districtId != buildingDistrict && (strict || !ext.GetAllowGoOutsideEffective(buildingDistrict)))
                        {
                            continue;
                        }
                    }
                    if (mustAllowSpawn)
                    {
                        int max = GetMaxVehiclesBuilding(i, ssd.vehicleType, ssd.level);
                        int count = 0;
                        int cargo = 0;
                        int capacity = 0;
                        int inbound = 0;
                        int outbound = 0;
                        SVMBuildingUtils.CalculateOwnVehicles(i, ref bm.m_buildings.m_buffer[i], SVMBuildingAIOverrideUtils.getBuildingOverrideExtensionStrict(bm.m_buildings.m_buffer[i].Info).GetManagedReasons(bm.m_buildings.m_buffer[i].Info).Where(x => x.Value.vehicleLevel == null).Select(x => x.Key), ref count, ref cargo, ref capacity, ref inbound, ref outbound);
                        if (count >= max) continue;
                    }
                    saida.Add(i);
                }
            }
            SVMUtils.doLog("getAllBuildingsFromCity ({0}) buildings = {1} (s={2}); saida.sz = {3}", ssd, buildings.ToArray(), buildings.m_size, saida.Count);
            return saida;
        }
    }
}
