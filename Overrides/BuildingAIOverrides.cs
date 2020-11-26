using ColossalFramework.Math;
using Harmony;
using Klyte.Commons.Extensors;
using Klyte.Commons.Utils;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Klyte.ServiceVehiclesManager.Overrides
{
    internal class BuildingAIOverrides : Redirector, IRedirectable
    {
        public static VehicleInfo GetRandomVehicleInfoNoVehicle(ref Randomizer randomizer, ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, ushort buildingId)
        {
            // A
            var ssds = ServiceSystemDefinition.from(service, subService, level).FirstOrDefault();
            if (ssds != default)
            {
                var vehicleInfo = ssds.GetAModel(buildingId);
                if (vehicleInfo != default)
                {
                    return vehicleInfo;
                }
            }
            return VehicleManager.instance.GetRandomVehicleInfo(ref randomizer, service, subService, level);
        }

        public static VehicleInfo GetRandomVehicleInfoWithVehicle(ref Randomizer randomizer, ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, VehicleInfo.VehicleType vehicleType, ushort buildingId)
        {
            // A
            var ssds = ServiceSystemDefinition.from(service, subService, level, vehicleType);
            if (ssds != default)
            {
                var vehicleInfo = ssds.GetAModel(buildingId);
                if (vehicleInfo != default)
                {
                    return vehicleInfo;
                }
            }
            return VehicleManager.instance.GetRandomVehicleInfo(ref randomizer, service, subService, level, vehicleType);
        }

        public static IEnumerable<CodeInstruction> TranspileGetVehicle(IEnumerable<CodeInstruction> instr, ILGenerator il)
        {
            var instrList = new List<CodeInstruction>(instr);

            for (int i = 1; i < instrList.Count; i++)
            {
                if (instrList[i].opcode == OpCodes.Callvirt && instrList[i].operand is MethodInfo mi && mi.DeclaringType == typeof(VehicleManager) && mi.Name == "GetRandomVehicleInfo")
                {
                    instrList[i] = new CodeInstruction(OpCodes.Ldarg_1);
                    if (mi.GetParameters().Where(x => x.ParameterType == typeof(VehicleInfo.VehicleType)).Count() > 0)
                    {
                        instrList.Insert(i + 1, new CodeInstruction(OpCodes.Call, typeof(BuildingAIOverrides).GetMethod("GetRandomVehicleInfoWithVehicle")));
                    }
                    else
                    {
                        instrList.Insert(i + 1, new CodeInstruction(OpCodes.Call, typeof(BuildingAIOverrides).GetMethod("GetRandomVehicleInfoNoVehicle")));
                    }
                    i += 3;
                }
            }

            LogUtils.PrintMethodIL(instrList);
            return instrList;
        }

        #region Hooking

        private static Tuple<Type, string>[] methodsToTranspile = new Tuple<Type, string>[]
        {

            Tuple.New(typeof(DisasterResponseBuildingAI),"StartTransfer"        ),
            Tuple.New(typeof(FireStationAI),"StartTransfer"                     ),
            Tuple.New(typeof(FishFarmAI),"StartTransfer"                        ),
            Tuple.New(typeof(FishingHarborAI),"StartTransfer"                   ),
            Tuple.New(typeof(FishingHarborAI),"TrySpawnBoat"                    ),
            Tuple.New(typeof(HelicopterDepotAI),"StartTransfer"                 ),
            Tuple.New(typeof(CableCarStationAI),"CreateVehicle"                 ),
            Tuple.New(typeof(CemeteryAI),"StartTransfer"                        ),
            Tuple.New(typeof(DepotAI),"StartTransfer"                           ),
            Tuple.New(typeof(HospitalAI),"StartTransfer"                        ),
            Tuple.New(typeof(IndustrialBuildingAI),"StartTransfer"              ),
            Tuple.New(typeof(IndustrialExtractorAI),"StartTransfer"             ),
            Tuple.New(typeof(LandfillSiteAI),"StartTransfer"                    ),
            Tuple.New(typeof(MaintenanceDepotAI),"StartTransfer"                ),
            Tuple.New(typeof(OutsideConnectionAI),"StartConnectionTransferImpl" ),
            Tuple.New(typeof(PoliceStationAI),"StartTransfer"                   ),
            Tuple.New(typeof(PostOfficeAI),"StartTransfer"                      ),
            Tuple.New(typeof(PrivateAirportAI),"CheckVehicles"                  ),
            Tuple.New(typeof(ShelterAI),"EndRelocating"                         ),
            Tuple.New(typeof(ShelterAI),"HandleEvacuationVehicles"              ),
            Tuple.New(typeof(SnowDumpAI),"StartTransfer"                        ),
            Tuple.New(typeof(TaxiStandAI),"StartTransfer"                       ),
            Tuple.New(typeof(TourBuildingAI),"CheckVehicles"                    ),
            Tuple.New(typeof(TransportStationAI),"CreateIncomingVehicle"        ),
            Tuple.New(typeof(TransportStationAI),"CreateOutgoingVehicle"        ),
            Tuple.New(typeof(WaterFacilityAI),"StartTransfer"                   ),
        };


        public void Awake()
        {
            var transpileMethod = GetType().GetMethod("TranspileGetVehicle", RedirectorUtils.allFlags);

            foreach (var item in methodsToTranspile)
            {
                foreach (MethodInfo from in item.First.GetMethods(RedirectorUtils.allFlags).Where(x => x.Name == item.Second))
                {
                    try
                    {
                        AddRedirect(from, null, null, transpileMethod);
                    }
                    catch (Exception e)
                    {
                        LogUtils.DoErrorLog($"Failed transpiling: {item.First}.{item.Second} as {from}\n{e}\n--------");
                    }
                }
            }
        }

        #endregion

    }
}
