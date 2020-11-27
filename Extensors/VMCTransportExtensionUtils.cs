using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.Threading;
using Klyte.Commons.Utils;
using System.Collections;

namespace Klyte.VehiclesMasterControl.Extensors.VehicleExt
{
    public sealed class VMCTransportExtensionUtils
    {

        //    public static void RemoveAllUnwantedVehicles() { using var x = new EnumerableActionThread(new Func<ThreadBase, IEnumerator>(RemoveAllUnwantedVehicles)); }
        public static IEnumerator RemoveAllUnwantedVehicles(ThreadBase t)
        {
            ushort num = 0;
            while (num < Singleton<VehicleManager>.instance.m_vehicles.m_size)
            {
                VehicleInfo vehicleInfo = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[num].Info;
                if (vehicleInfo != null && !VehicleUtils.IsTrailer(vehicleInfo) && Singleton<VehicleManager>.instance.m_vehicles.m_buffer[num].m_transportLine == 0 && Singleton<VehicleManager>.instance.m_vehicles.m_buffer[num].m_sourceBuilding > 0)
                {
                    BuildingInfo buildingInfo = Singleton<BuildingManager>.instance.m_buildings.m_buffer[Singleton<VehicleManager>.instance.m_vehicles.m_buffer[num].m_sourceBuilding].Info;
                    var buildingSsd = ServiceSystemDefinition.from(buildingInfo, vehicleInfo.m_vehicleType);
                    if (buildingSsd != null)
                    {
                        if (!ExtensionStaticExtensionMethods.IsModelCompatible(Singleton<VehicleManager>.instance.m_vehicles.m_buffer[num].m_sourceBuilding, vehicleInfo, ref buildingSsd))
                        {
                            Singleton<VehicleManager>.instance.ReleaseVehicle(num);
                        }
                    }

                }
                if (num % 256 == 255)
                {
                    yield return num;
                }
                num++;
            }
            yield break;
        }

        public static void RemoveAllUnwantedVehicles()
        {
            var r = new Randomizer();
            for (ushort vehicleId = 1; vehicleId < Singleton<VehicleManager>.instance.m_vehicles.m_size; vehicleId++)
            {
                if ((Singleton<VehicleManager>.instance.m_vehicles.m_buffer[vehicleId].m_flags & Vehicle.Flags.Created) != 0)
                {
                    ref Vehicle vehicle = ref Singleton<VehicleManager>.instance.m_vehicles.m_buffer[vehicleId];
                    VehicleInfo vehicleInfo = vehicle.Info;
                    if (vehicleInfo != null && !VehicleUtils.IsTrailer(vehicleInfo) && vehicle.m_transportLine == 0 && vehicle.m_sourceBuilding > 0)
                    {
                        var buildingSsd = ServiceSystemDefinition.from(vehicleInfo);
                        if (buildingSsd != null)
                        {
                            if (!ExtensionStaticExtensionMethods.IsModelCompatible(vehicle.m_sourceBuilding, vehicleInfo, ref buildingSsd))
                            {
                                if (vehicleInfo.m_vehicleAI is CarAI)
                                {
                                    var model = buildingSsd.GetAModel(ref r, vehicle.m_sourceBuilding);
                                    VehicleUtils.ReplaceVehicleModel(vehicleId, model);
                                }
                                else
                                {
                                    Singleton<VehicleManager>.instance.ReleaseVehicle(vehicleId);
                                }
                            }
                        }
                    }

                }
            }
        }

    }


}
