using ColossalFramework;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.TransportLinesManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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

        public static int GetProductionRate(ref Building b)
        {
            int budget = Singleton<EconomyManager>.instance.GetBudget(b.Info.m_class);
            return PlayerBuildingAI.GetProductionRate(100, budget);
        }

        public static List<ushort> getAllBuildingsFromCity(ServiceSystemDefinition ssd)
        {
            List<ushort> saida = new List<ushort>();
            var bm = Singleton<BuildingManager>.instance;
            var buildings = bm.GetServiceBuildings(ssd.service);

            SVMUtils.doLog("getAllBuildingsFromCity ({0}) buildings = {1} (s={2})", ssd, buildings.ToArray(), buildings.m_size); 

            foreach (ushort i in buildings)
            {
                if (ssd.isFromSystem(bm.m_buildings.m_buffer[i].Info))
                {
                    saida.Add(i);
                }
            }
            SVMUtils.doLog("getAllBuildingsFromCity ({0}) buildings = {1} (s={2}); saida.sz = {3}", ssd, buildings.ToArray(), buildings.m_size, saida.Count);
            return saida;
        }
    }
}
