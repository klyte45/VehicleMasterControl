using ColossalFramework.Math;
using ColossalFramework.Plugins;
using Harmony;
using Klyte.Commons.Extensors;
using Klyte.Commons.Utils;
using Klyte.VehiclesMasterControl.Extensors.VehicleExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Klyte.VehiclesMasterControl.Overrides
{
    internal class BuildingAIOverrides : Redirector, IRedirectable
    {
        public static VehicleInfo GetRandomVehicleInfoNoVehicle(VehicleManager vm, ref Randomizer randomizer, ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, ushort buildingId, ref Building building)
        {
            // A
            var ssds = ServiceSystemDefinition.from(service, subService, level, building.Info.m_buildingAI is OutsideConnectionAI).FirstOrDefault();
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

        public static VehicleInfo GetRandomVehicleInfoWithVehicle(VehicleManager vm, ref Randomizer randomizer, ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, VehicleInfo.VehicleType vehicleType, ushort buildingId, ref Building building)
        {
            // A
            var ssds = ServiceSystemDefinition.from(service, subService, level, vehicleType, building.Info.m_buildingAI is OutsideConnectionAI);
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

        private static OpCode[] stlocCodes = new OpCode[]
        {
            OpCodes.Stloc,
            OpCodes.Stloc_0,
            OpCodes.Stloc_1,
            OpCodes.Stloc_2,
            OpCodes.Stloc_3,
            OpCodes.Stloc_S,
        };

        private static OpCode[] ldlocCodes = new OpCode[]
        {
            OpCodes.Ldloc,
            OpCodes.Ldloc_0,
            OpCodes.Ldloc_1,
            OpCodes.Ldloc_2,
            OpCodes.Ldloc_3,
            OpCodes.Ldloc_S,
        };

        public static IEnumerable<CodeInstruction> TranspileGetVehicleStatic(IEnumerable<CodeInstruction> instr, ILGenerator il) => TranspileGetVehicleImpl(instr, il, OpCodes.Ldarg_0, OpCodes.Ldarg_1);
        public static IEnumerable<CodeInstruction> TranspileGetVehicle(IEnumerable<CodeInstruction> instr, ILGenerator il) => TranspileGetVehicleImpl(instr, il, OpCodes.Ldarg_1, OpCodes.Ldarg_2);
        public static IEnumerable<CodeInstruction> TranspileGetVehicleImpl(IEnumerable<CodeInstruction> instr, ILGenerator il, OpCode argOpcode, OpCode argOpcodeBuilding)
        {
            var instrList = new List<CodeInstruction>(instr);

            for (int i = 1; i < instrList.Count; i++)
            {
                if (instrList[i].opcode == OpCodes.Callvirt && instrList[i].operand is MethodInfo mi && mi.DeclaringType == typeof(VehicleManager) && mi.Name == "GetRandomVehicleInfo")
                {
                    instrList[i].opcode = argOpcode;
                    instrList[i].operand = null;
                    instrList.Insert(i + 1, new CodeInstruction(argOpcodeBuilding));
                    if (mi.GetParameters().Where(x => x.ParameterType == typeof(VehicleInfo.VehicleType)).Count() > 0)
                    {
                        instrList.Insert(i + 2, new CodeInstruction(OpCodes.Call, typeof(BuildingAIOverrides).GetMethod("GetRandomVehicleInfoWithVehicle")));
                    }
                    else
                    {
                        instrList.Insert(i + 2, new CodeInstruction(OpCodes.Call, typeof(BuildingAIOverrides).GetMethod("GetRandomVehicleInfoNoVehicle")));
                    }
                    i += 4;
                }
               ;
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

        private static Tuple<Type, string>[] staticMethodsToTranspile = new Tuple<Type, string>[]
        {
            Tuple.New(typeof(OutsideConnectionAI),"StartConnectionTransferImpl" ),
        };


        public void Awake()
        {
            var transpileMethod = GetType().GetMethod("TranspileGetVehicle", RedirectorUtils.allFlags);
            var transpileMethodStatic = GetType().GetMethod("TranspileGetVehicleStatic", RedirectorUtils.allFlags);

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
            foreach (var item in staticMethodsToTranspile)
            {
                foreach (MethodInfo from in item.First.GetMethods(RedirectorUtils.allFlags).Where(x => x.Name == item.Second))
                {
                    try
                    {
                        AddRedirect(from, null, null, transpileMethodStatic);
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
