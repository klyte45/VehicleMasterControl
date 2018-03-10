using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Extensions;
using Klyte.Harmony;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.Utils;
using Klyte.Commons.Extensors;
using Klyte.Commons.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using ColossalFramework.Math;

namespace Klyte.ServiceVehiclesManager.Overrides
{
    internal sealed class SVMBuildingAIOverrideUtils
    {
        private static Dictionary<Type, Type> subtypes = null;

        public static IBasicBuildingAIOverrides getBuildingOverrideExtension(BuildingInfo info)
        {
            PrefabAI targetAi = info.GetAI();
            Type targetTypeAi = targetAi.GetType();
            if (subtypes == null)
            {
                subtypes = new Dictionary<Type, Type>();
                foreach (Type t in SVMUtils.GetSubtypesRecursive(typeof(BasicBuildingAIOverrides<,>), typeof(SVMBuildingAIOverrideUtils)))
                {
                    try
                    {
                        subtypes[t.BaseType.GetGenericArguments()[1]] = t;
                    }
                    catch { }
                }
                SVMUtils.doLog("GetOverride - Classes:\r\n\t{0}", string.Join("\r\n\t", subtypes?.Select(x => x.Key.ToString() + "=>" + x.Value.ToString())?.ToArray() ?? new string[0]));
            }
            Type targetClass = null;
            IBasicBuildingAIOverrides value = null;
            if (!subtypes.ContainsKey(targetTypeAi))
            {
                foreach (var clazz in subtypes.Keys)
                {
                    if (clazz.IsAssignableFrom(targetTypeAi))
                    {
                        value = (IBasicBuildingAIOverrides)SVMUtils.GetPrivateStaticField("instance", subtypes[clazz]);
                        //SVMUtils.doLog("GetOverride - clazz = {0}; value = {1}", clazz, value);
                        if (value.AcceptsAI(targetAi))
                        {
                            targetClass = subtypes[clazz];
                            break;
                        }
                    }
                }
                SVMUtils.doLog("GetOverride - targetClass = {0} ({1})", targetClass, targetTypeAi);
                if (targetClass == null) return null;
            }
            else
            {
                targetClass = subtypes[targetTypeAi];
                value = (IBasicBuildingAIOverrides)SVMUtils.GetPrivateStaticField("instance", targetClass);
            }
            //SVMUtils.doLog("GetOverride - value = {0}", value);
            return (IBasicBuildingAIOverrides)value;
        }
    }

    internal sealed class DisasterResponseBuildingAIOverrides : BasicBuildingAIOverrides<DisasterResponseBuildingAIOverrides, DisasterResponseBuildingAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.Collapsed] = Tuple.New(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.Collapsed2] = Tuple.New(VehicleInfo.VehicleType.Helicopter, true, false),
        };
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(DisasterResponseBuildingAI ai, TransferManager.TransferOffer offer) => reasons;

        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh)
        {
            if (veh == VehicleInfo.VehicleType.Car)
            {
                return "m_vehicleCount";
            }
            else if (veh == VehicleInfo.VehicleType.Helicopter)
            {
                return "m_helicopterCount";
            }
            return null;
        }
        public override bool AllowVehicleType(VehicleInfo.VehicleType type, DisasterResponseBuildingAI ai) => type == VehicleInfo.VehicleType.Car || type == VehicleInfo.VehicleType.Helicopter;
    }

    internal sealed class CemeteryAIOverrides : BasicBuildingAIOverrides<CemeteryAIOverrides, CemeteryAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.Dead] = Tuple.New(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.DeadMove] = Tuple.New(VehicleInfo.VehicleType.Car, false, true),
        };
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(CemeteryAI ai, TransferManager.TransferOffer offer) => reasons;
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => "m_hearseCount";
        public override bool AllowVehicleType(VehicleInfo.VehicleType type, CemeteryAI ai) => type == VehicleInfo.VehicleType.Car;
    }

    internal sealed class FireStationAIOverrides : BasicBuildingAIOverrides<FireStationAIOverrides, FireStationAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.Fire] = Tuple.New(VehicleInfo.VehicleType.Car, true, false)
        };
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(FireStationAI ai, TransferManager.TransferOffer offer) => reasons;
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => "m_fireTruckCount";
        public override bool AllowVehicleType(VehicleInfo.VehicleType type, FireStationAI ai) => type == VehicleInfo.VehicleType.Car;
    }

    internal sealed class HelicopterDepotAIOverrides : BasicBuildingAIOverrides<HelicopterDepotAIOverrides, HelicopterDepotAI>
    {
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(HelicopterDepotAI ai, TransferManager.TransferOffer offer)
        {
            var r1 = (TransferManager.TransferReason)SVMUtils.ExecuteReflectionMethod(ai, "GetTransferReason1");
            var r2 = (TransferManager.TransferReason)SVMUtils.ExecuteReflectionMethod(ai, "GetTransferReason2");

            var result = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>();

            if (r1 != TransferManager.TransferReason.None)
            {
                result[r1] = Tuple.New(VehicleInfo.VehicleType.Helicopter, true, false);
            }
            if (r2 != TransferManager.TransferReason.None)
            {
                result[r2] = Tuple.New(VehicleInfo.VehicleType.Helicopter, true, false);
            }

            return result;
        }

        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => "m_helicopterCount";
        public override bool AllowVehicleType(VehicleInfo.VehicleType type, HelicopterDepotAI ai) => type == VehicleInfo.VehicleType.Helicopter;
    }

    internal sealed class HospitalAIOverrides : BasicBuildingAIOverrides<HospitalAIOverrides, HospitalAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.Sick] = Tuple.New(VehicleInfo.VehicleType.Car, true, false)
        };
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(HospitalAI ai, TransferManager.TransferOffer offer) => reasons;

        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => "m_ambulanceCount";
        public override bool AllowVehicleType(VehicleInfo.VehicleType type, HospitalAI ai) => type == VehicleInfo.VehicleType.Car;
    }

    internal sealed class MaintenanceDepotAIOverrides : BasicBuildingAIOverrides<MaintenanceDepotAIOverrides, MaintenanceDepotAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.RoadMaintenance] = Tuple.New(VehicleInfo.VehicleType.Car, true, false)
        };
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(MaintenanceDepotAI ai, TransferManager.TransferOffer offer) => reasons;
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => "m_maintenanceTruckCount";
        public override bool AllowVehicleType(VehicleInfo.VehicleType type, MaintenanceDepotAI ai) => type == VehicleInfo.VehicleType.Car;
    }

    internal sealed class PoliceStationAIOverrides : BasicBuildingAIOverrides<PoliceStationAIOverrides, PoliceStationAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.Crime] = Tuple.New(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.CriminalMove] = Tuple.New(VehicleInfo.VehicleType.Car, true, false),
        };
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(PoliceStationAI ai, TransferManager.TransferOffer offer) => reasons;
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => "m_policeCarCount";
        public override bool AllowVehicleType(VehicleInfo.VehicleType type, PoliceStationAI ai) => type == VehicleInfo.VehicleType.Car;
    }

    internal sealed class SnowDumpAIOverrides : BasicBuildingAIOverrides<SnowDumpAIOverrides, SnowDumpAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.Snow] = Tuple.New(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.SnowMove] = Tuple.New(VehicleInfo.VehicleType.Car, false, true),
        };
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(SnowDumpAI ai, TransferManager.TransferOffer offer) => reasons;
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => "m_snowTruckCount";
        public override bool AllowVehicleType(VehicleInfo.VehicleType type, SnowDumpAI ai) => type == VehicleInfo.VehicleType.Car;
    }

    internal sealed class WaterFacilityAIOverrides : BasicBuildingAIOverrides<WaterFacilityAIOverrides, WaterFacilityAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.FloodWater] = Tuple.New(VehicleInfo.VehicleType.Car, true, false),
        };
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasonsNull = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>();
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(WaterFacilityAI ai, TransferManager.TransferOffer offer)
        {
            if (ai.m_sewageOutlet != 0 && ai.m_sewageStorage != 0 && ai.m_pumpingVehicles != 0)
            {
                return reasons;
            }
            else
            {
                return reasonsNull;
            }
        }

        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => "m_pumpingVehicles";
        public override bool AllowVehicleType(VehicleInfo.VehicleType type, WaterFacilityAI ai) => type == VehicleInfo.VehicleType.Car;
    }

    internal sealed class LandfillSiteAIOverrides : BasicBuildingAIOverrides<LandfillSiteAIOverrides, LandfillSiteAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.Garbage] = Tuple.New(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.GarbageMove] = Tuple.New(VehicleInfo.VehicleType.Car, false, true),
        };

        public override bool AllowVehicleType(VehicleInfo.VehicleType type, LandfillSiteAI ai) => type == VehicleInfo.VehicleType.Car;
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(LandfillSiteAI ai, TransferManager.TransferOffer offer) => reasons;
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => "m_garbageTruckCount";
    }

    internal sealed class DepotAIOverrides : BasicBuildingAIOverrides<DepotAIOverrides, DepotAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasonsTaxi = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.Taxi] = Tuple.New(VehicleInfo.VehicleType.Car, false, true),
        };
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasonsTrain = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.PassengerTrain] = Tuple.New(VehicleInfo.VehicleType.Train, false, true),
            [TransferManager.TransferReason.DummyTrain] = Tuple.New(VehicleInfo.VehicleType.Train, false, true),
        };
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasonsShip = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.PassengerShip] = Tuple.New(VehicleInfo.VehicleType.Ship, false, true),
            [TransferManager.TransferReason.DummyShip] = Tuple.New(VehicleInfo.VehicleType.Ship, false, true),
        };
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasonsPlane = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.PassengerPlane] = Tuple.New(VehicleInfo.VehicleType.Plane, false, true),
            [TransferManager.TransferReason.DummyPlane] = Tuple.New(VehicleInfo.VehicleType.Plane, false, true),
        };
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasonsCableCar = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.CableCar] = Tuple.New(VehicleInfo.VehicleType.CableCar, false, true),
        };
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasonsNone = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
        };

        public override bool AllowVehicleType(VehicleInfo.VehicleType type, DepotAI ai)
        {
            if (ai is TransportStationAI) return false;
            List<VehicleInfo.VehicleType> allowedTypes = new List<VehicleInfo.VehicleType>();
            AddAllowedType(ai?.m_transportInfo?.m_transportType, allowedTypes);
            AddAllowedType(ai?.m_secondaryTransportInfo?.m_transportType, allowedTypes);
            return allowedTypes.Contains(type);
        }

        public static void AddAllowedType(TransportInfo.TransportType? type, List<VehicleInfo.VehicleType> allowedTypes)
        {
            switch (type)
            {
                //case TransportInfo.TransportType.Bus:
                case TransportInfo.TransportType.Taxi:
                    //case TransportInfo.TransportType.EvacuationBus:
                    allowedTypes.Add(VehicleInfo.VehicleType.Car);
                    break;
                //case TransportInfo.TransportType.Metro:
                case TransportInfo.TransportType.Train:
                    //case TransportInfo.TransportType.Monorail:
                    allowedTypes.Add(VehicleInfo.VehicleType.Train);
                    break;
                case TransportInfo.TransportType.Ship:
                    allowedTypes.Add(VehicleInfo.VehicleType.Ship);
                    break;
                case TransportInfo.TransportType.Airplane:
                    allowedTypes.Add(VehicleInfo.VehicleType.Plane);
                    break;
                //case TransportInfo.TransportType.Tram:
                //allowedTypes.Add(VehicleInfo.VehicleType.Tram);
                //break;
                case TransportInfo.TransportType.CableCar:
                    allowedTypes.Add(VehicleInfo.VehicleType.CableCar);
                    break;
            }
        }

        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(DepotAI ai, TransferManager.TransferOffer offer)
        {
            if (offer.TransportLine != 0)
            {
                return reasonsNone;
            }
            List<VehicleInfo.VehicleType> allowedTypes = new List<VehicleInfo.VehicleType>();
            AddAllowedType(ai?.m_transportInfo?.m_transportType, allowedTypes);
            AddAllowedType(ai?.m_secondaryTransportInfo?.m_transportType, allowedTypes);

            IEnumerable<KeyValuePair<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>> result = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>();
            foreach (var type in allowedTypes)
            {
                switch (type)
                {
                    case VehicleInfo.VehicleType.Car:
                        result = result.Union(reasonsTaxi);
                        break;
                    case VehicleInfo.VehicleType.Train:
                        result = result.Union(reasonsTrain);
                        break;
                    case VehicleInfo.VehicleType.Ship:
                        result = result.Union(reasonsShip);
                        break;
                    case VehicleInfo.VehicleType.Plane:
                        result = result.Union(reasonsPlane);
                        break;
                    case VehicleInfo.VehicleType.CableCar:
                        result = result.Union(reasonsCableCar);
                        break;

                }
            }
            return result.ToDictionary(x => x.Key, x => x.Value);
        }
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => "m_maxVehicleCount";
        public override bool AcceptsAI(PrefabAI ai) => ai.GetType() == typeof(DepotAI);
    }

    internal sealed class TransportStationAIOverrides : BasicBuildingAIOverrides<TransportStationAIOverrides, TransportStationAI>
    {

        public override bool AllowVehicleType(VehicleInfo.VehicleType type, TransportStationAI ai)
        {
            List<VehicleInfo.VehicleType> allowedTypes = new List<VehicleInfo.VehicleType>();
            DepotAIOverrides.AddAllowedType(ai?.m_transportInfo?.m_transportType, allowedTypes);
            DepotAIOverrides.AddAllowedType(ai?.m_secondaryTransportInfo?.m_transportType, allowedTypes);
            return allowedTypes.Contains(type);
        }
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(TransportStationAI ai, TransferManager.TransferOffer offer)
        {
            return DepotAIOverrides.instance.GetManagedReasons(ai, offer);
        }
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => DepotAIOverrides.instance.GetVehicleMaxCountField(veh);


        private static MethodInfo FindConnectionVehicle = typeof(TransportStationAI).GetMethod("FindConnectionVehicle", allFlags);
        private static MethodInfo FindConnectionBuilding = typeof(TransportStationAI).GetMethod("FindConnectionBuilding", allFlags);

        private static bool CreateOutgoingVehicle(bool __result, TransportStationAI __instance, ushort buildingID, ref Building buildingData, ushort startStop, int gateIndex)
        {
            if (__instance.m_transportLineInfo != null && (ushort)FindConnectionVehicle.Invoke(__instance, new object[] { buildingID, buildingData, startStop, 3000f }) == 0)
            {
                SVMUtils.doLog("START CreateOutgoingVehicle: {0} , {1}", typeof(TransportStationAI), __instance.name);
                ServiceSystemDefinition def = ServiceSystemDefinition.from(buildingData.Info).FirstOrDefault();
                if (def == null)
                {
                    SVMUtils.doLog("SSD Não definido para: {0} {1} {2}", buildingData.Info.m_class.m_service, buildingData.Info.m_class.m_subService, buildingData.Info.m_class.m_level);
                    return false;
                }
                SVMUtils.doLog("[{1}] SSD = {0}", def, "CreateIncomingVehicle");
                VehicleInfo randomVehicleInfo = ServiceSystemDefinition.availableDefinitions[def].GetAModel(buildingID);
                SVMUtils.doLog("[{1}] Veh = {0}", randomVehicleInfo?.ToString() ?? "<NULL>", "CreateIncomingVehicle");
                if (randomVehicleInfo != null)
                {
                    Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                    Randomizer randomizer = default(Randomizer);
                    randomizer.seed = (ulong)((long)gateIndex);
                    Vector3 vector;
                    Vector3 vector2;
                    __instance.CalculateSpawnPosition(buildingID, ref buildingData, ref randomizer, randomVehicleInfo, out vector, out vector2);
                    TransportInfo transportInfo = __instance.m_transportInfo;
                    if (__instance.m_secondaryTransportInfo != null && __instance.m_secondaryTransportInfo.m_class.m_subService == __instance.m_transportLineInfo.m_class.m_subService)
                    {
                        transportInfo = __instance.m_secondaryTransportInfo;
                    }
                    ushort num;
                    if (randomVehicleInfo.m_vehicleAI.CanSpawnAt(vector) && Singleton<VehicleManager>.instance.CreateVehicle(out num, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, vector, transportInfo.m_vehicleReason, false, true))
                    {
                        vehicles.m_buffer[(int)num].m_gateIndex = (byte)gateIndex;
                        Vehicle[] expr_12E_cp_0 = vehicles.m_buffer;
                        ushort expr_12E_cp_1 = num;
                        expr_12E_cp_0[(int)expr_12E_cp_1].m_flags = (expr_12E_cp_0[(int)expr_12E_cp_1].m_flags | (Vehicle.Flags.Importing | Vehicle.Flags.Exporting));
                        randomVehicleInfo.m_vehicleAI.SetSource(num, ref vehicles.m_buffer[(int)num], buildingID);
                        randomVehicleInfo.m_vehicleAI.SetTarget(num, ref vehicles.m_buffer[(int)num], startStop);
                        SVMUtils.doLog("END CreateOutgoingVehicle: {0} , {1}", typeof(TransportStationAI), __instance.name);
                        __result = true;
                        return false;
                    }
                }
            }
            SVMUtils.doLog("END2 CreateOutgoingVehicle: {0} , {1}", typeof(TransportStationAI), __instance.name);
            __result = false;
            return false;
        }

        private static bool CreateIncomingVehicle(bool __result, TransportStationAI __instance, ushort buildingID, ref Building buildingData, ushort startStop, int gateIndex)
        {
            if (__instance.m_transportLineInfo != null && (ushort)FindConnectionVehicle.Invoke(__instance, new object[] { buildingID, buildingData, startStop, 3000f }) == 0)
            {
                SVMUtils.doLog("START CreateIncomingVehicle: {0} , {1}", typeof(TransportStationAI), __instance.name);
                ServiceSystemDefinition def = ServiceSystemDefinition.from(buildingData.Info).FirstOrDefault();
                if (def == null)
                {
                    SVMUtils.doLog("SSD Não definido para: {0} {1} {2}", buildingData.Info.m_class.m_service, buildingData.Info.m_class.m_subService, buildingData.Info.m_class.m_level);
                    return false;
                }
                SVMUtils.doLog("[{1}] SSD = {0}", def, "CreateIncomingVehicle");
                VehicleInfo randomVehicleInfo = ServiceSystemDefinition.availableDefinitions[def].GetAModel(buildingID);
                SVMUtils.doLog("[{1}] Veh = {0}", randomVehicleInfo?.ToString() ?? "<NULL>", "CreateIncomingVehicle");

                if (randomVehicleInfo != null)
                {
                    ushort num = (ushort)FindConnectionBuilding.Invoke(__instance, new object[] { startStop });
                    if (num != 0)
                    {
                        Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                        BuildingInfo info = Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)num].Info;
                        Randomizer randomizer = default(Randomizer);
                        randomizer.seed = (ulong)((long)gateIndex);
                        Vector3 vector;
                        Vector3 vector2;
                        info.m_buildingAI.CalculateSpawnPosition(num, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)num], ref randomizer, randomVehicleInfo, out vector, out vector2);
                        TransportInfo transportInfo = __instance.m_transportInfo;
                        if (__instance.m_secondaryTransportInfo != null && __instance.m_secondaryTransportInfo.m_class.m_subService == __instance.m_transportLineInfo.m_class.m_subService)
                        {
                            transportInfo = __instance.m_secondaryTransportInfo;
                        }
                        if (randomVehicleInfo.m_vehicleAI.CanSpawnAt(vector) && Singleton<VehicleManager>.instance.CreateVehicle(out ushort num2, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, vector, transportInfo.m_vehicleReason, true, false))
                        {
                            vehicles.m_buffer[(int)num2].m_gateIndex = (byte)gateIndex;
                            Vehicle[] expr_172_cp_0 = vehicles.m_buffer;
                            ushort expr_172_cp_1 = num2;
                            expr_172_cp_0[(int)expr_172_cp_1].m_flags = (expr_172_cp_0[(int)expr_172_cp_1].m_flags | (Vehicle.Flags.Importing | Vehicle.Flags.Exporting));
                            randomVehicleInfo.m_vehicleAI.SetSource(num2, ref vehicles.m_buffer[(int)num2], num);
                            randomVehicleInfo.m_vehicleAI.SetSource(num2, ref vehicles.m_buffer[(int)num2], buildingID);
                            randomVehicleInfo.m_vehicleAI.SetTarget(num2, ref vehicles.m_buffer[(int)num2], startStop);
                            SVMUtils.doLog("END CreateIncomingVehicle: {0} , {1}", typeof(TransportStationAI), __instance.name);
                            __result = true;
                            return false;
                        }
                    }
                }
            }
            SVMUtils.doLog("END2 CreateIncomingVehicle: {0} , {1}", typeof(TransportStationAI), __instance.name);
            __result = false;
            return false;
        }

        public override void Awake()
        {
            instance = this;
            var from = typeof(TransportStationAI).GetMethod("CreateIncomingVehicle", allFlags);
            var to = typeof(TransportStationAIOverrides).GetMethod("CreateIncomingVehicle", allFlags);
            var from2 = typeof(TransportStationAI).GetMethod("CreateOutgoingVehicle", allFlags);
            var to2 = typeof(TransportStationAIOverrides).GetMethod("CreateOutgoingVehicle", allFlags);
            SVMUtils.doLog("Loading Hooks: {0} ({1}=>{2})", typeof(TransportStationAI), from, to);
            SVMUtils.doLog("Loading Hooks: {0} ({1}=>{2})", typeof(TransportStationAI), from2, to2);
            AddRedirect(from, to);
            AddRedirect(from2, to2);
        }
    }

    internal sealed class CargoStationAIOverrides : BasicBuildingAIOverrides<CargoStationAIOverrides, CargoStationAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasonsTrain = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.DummyTrain] = Tuple.New(VehicleInfo.VehicleType.Train, false, true),
        };
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasonsShip = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.DummyShip] = Tuple.New(VehicleInfo.VehicleType.Ship, false, true),
        };
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasonsPlane = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.DummyPlane] = Tuple.New(VehicleInfo.VehicleType.Plane, false, true),
        };
        public override bool AllowVehicleType(VehicleInfo.VehicleType type, CargoStationAI ai)
        {
            List<VehicleInfo.VehicleType> allowedTypes = new List<VehicleInfo.VehicleType>();
            if (ai?.m_transportInfo?.m_transportType == TransportInfo.TransportType.Airplane || ai?.m_transportInfo2?.m_transportType == TransportInfo.TransportType.Airplane)
            {
                allowedTypes.Add(VehicleInfo.VehicleType.Plane);
            }
            if (ai?.m_transportInfo?.m_transportType == TransportInfo.TransportType.Ship || ai?.m_transportInfo2?.m_transportType == TransportInfo.TransportType.Ship)
            {
                allowedTypes.Add(VehicleInfo.VehicleType.Ship);
            }
            if (ai?.m_transportInfo?.m_transportType == TransportInfo.TransportType.Train || ai?.m_transportInfo2?.m_transportType == TransportInfo.TransportType.Train)
            {
                allowedTypes.Add(VehicleInfo.VehicleType.Train);
            }
            return allowedTypes.Contains(type);
        }

        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(CargoStationAI ai, TransferManager.TransferOffer offer)
        {
            List<VehicleInfo.VehicleType> allowedTypes = new List<VehicleInfo.VehicleType>();
            DepotAIOverrides.AddAllowedType(ai?.m_transportInfo?.m_transportType, allowedTypes);
            DepotAIOverrides.AddAllowedType(ai?.m_transportInfo2?.m_transportType, allowedTypes);
            IEnumerable<KeyValuePair<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>> result = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>();
            foreach (var type in allowedTypes)
            {
                switch (type)
                {
                    case VehicleInfo.VehicleType.Train:
                        result = result.Union(reasonsTrain);
                        break;
                    case VehicleInfo.VehicleType.Ship:
                        result = result.Union(reasonsShip);
                        break;
                    case VehicleInfo.VehicleType.Plane:
                        result = result.Union(reasonsPlane);
                        break;
                }
            }

            return result.ToDictionary(x => x.Key, x => x.Value);
        }

        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => null;

        public override void Awake()
        {
            instance = this;
        }
    }

    internal sealed class OutsideConnectionAIOverrides : BasicBuildingAIOverrides<OutsideConnectionAIOverrides, OutsideConnectionAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.DummyTrain] = Tuple.New(VehicleInfo.VehicleType.Train, false, true),
            [TransferManager.TransferReason.DummyShip] = Tuple.New(VehicleInfo.VehicleType.Ship, false, true),
            [TransferManager.TransferReason.DummyPlane] = Tuple.New(VehicleInfo.VehicleType.Plane, false, true),
            [TransferManager.TransferReason.DummyCar] = Tuple.New(VehicleInfo.VehicleType.Car, false, true),
        };

        public override bool AllowVehicleType(VehicleInfo.VehicleType type, OutsideConnectionAI ai) => type == VehicleInfo.VehicleType.Train || type == VehicleInfo.VehicleType.Car || type == VehicleInfo.VehicleType.Ship || type == VehicleInfo.VehicleType.Plane;
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(OutsideConnectionAI ai, TransferManager.TransferOffer offer)
        {
            if (offer.TransportLine != 0)
            {
                return new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>();
            }
            return reasons;
        }
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => null;

        public override void Awake()
        {
            instance = this;
            var from = typeof(OutsideConnectionAI).GetMethod("StartConnectionTransferImpl", allFlags);
            var to = typeof(OutsideConnectionAIOverrides).GetMethod("StartTransferOverride", allFlags);
            SVMUtils.doLog("Loading Hooks: {0} ({1}=>{2})", typeof(OutsideConnectionAI), from, to);
            AddRedirect(from, to);
        }
        private static bool StartTransferOverride(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {

            SVMUtils.doLog("START TRANSFER: {0} , {1}", typeof(OutsideConnectionAI), material);
            ServiceSystemDefinition def = null;
            switch (material)
            {
                case TransferManager.TransferReason.DummyTrain:
                    if (offer.Building != buildingID)
                    {
                        if (Singleton<SimulationManager>.instance.m_randomizer.Int32(2u) == 0)
                        {
                            def = ServiceSystemDefinition.CARG_TRAIN;
                        }
                        else
                        {
                            def = ServiceSystemDefinition.REG_TRAIN;
                        }
                        goto OfferProcessing;
                    }
                    break;
                case TransferManager.TransferReason.DummyShip:
                    if (offer.Building != buildingID)
                    {
                        if (Singleton<SimulationManager>.instance.m_randomizer.Int32(2u) == 0)
                        {
                            def = ServiceSystemDefinition.CARG_SHIP;
                        }
                        else
                        {
                            def = ServiceSystemDefinition.REG_SHIP;
                        }
                        goto OfferProcessing;
                    }
                    break;
                case TransferManager.TransferReason.DummyPlane:
                    if (offer.Building != buildingID)
                    {
                        def = ServiceSystemDefinition.REG_PLANE;
                        goto OfferProcessing;
                    }
                    break;
            }
            SVMUtils.doLog("END TRANSFER: {0} , {1} (not set)", typeof(OutsideConnectionAI), material);
            return true;

            OfferProcessing:
            SVMUtils.doLog("[{1}] SSD = {0}", def, material);
            VehicleInfo randomVehicleInfo = ServiceSystemDefinition.availableDefinitions[def].GetAModel(offer.Building);
            SVMUtils.doLog("[{1}] Veh = {0}", randomVehicleInfo?.ToString() ?? "<NULL>", material);

            Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
            if (Singleton<VehicleManager>.instance.CreateVehicle(out ushort vehId, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, data.m_position, material, false, true))
            {
                Vehicle[] expr_BB7_cp_0 = vehicles.m_buffer;
                ushort expr_BB7_cp_1 = vehId;
                expr_BB7_cp_0[(int)expr_BB7_cp_1].m_flags = (expr_BB7_cp_0[(int)expr_BB7_cp_1].m_flags | Vehicle.Flags.DummyTraffic);
                Vehicle[] expr_BD6_cp_0 = vehicles.m_buffer;
                ushort expr_BD6_cp_1 = vehId;
                expr_BD6_cp_0[(int)expr_BD6_cp_1].m_flags = (expr_BD6_cp_0[(int)expr_BD6_cp_1].m_flags & ~Vehicle.Flags.WaitingCargo);

                randomVehicleInfo.m_vehicleAI.SetSource(vehId, ref vehicles.m_buffer[(int)vehId], buildingID);
                randomVehicleInfo.m_vehicleAI.StartTransfer(vehId, ref vehicles.m_buffer[(int)vehId], material, offer);
                SVMUtils.doLog("END TRANSFER: {0} , {1} (found)", typeof(OutsideConnectionAI), material);
                return false;
            }
            SVMUtils.doLog("END TRANSFER: {0} , {1} (not found)", typeof(OutsideConnectionAI), material);
            return true;
        }

    }
}
