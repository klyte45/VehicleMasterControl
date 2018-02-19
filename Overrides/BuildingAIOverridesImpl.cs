using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Extensions;
using Klyte.Harmony;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.Utils;
using Klyte.TransportLinesManager.Extensors;
using Klyte.TransportLinesManager.Extensors.TransportTypeExt;
using Klyte.TransportLinesManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager.Overrides
{
    internal sealed class SVMBuildingAIOverrideUtils
    {
        private static Dictionary<Type, Type> subtypes = null;

        public static IBasicBuildingAIOverrides getBuildingOverrideExtension(BuildingInfo info)
        {
            Type targetAi = info.GetAI().GetType();
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
            if (!subtypes.ContainsKey(targetAi)) return null;
            Type targetClass = subtypes[targetAi];
            var value = SVMUtils.GetPrivateStaticField("instance", targetClass);
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
        public override bool AllowVehicleType(VehicleInfo.VehicleType type) => type == VehicleInfo.VehicleType.Car || type == VehicleInfo.VehicleType.Helicopter;
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
        public override bool AllowVehicleType(VehicleInfo.VehicleType type) => type == VehicleInfo.VehicleType.Car;
    }

    internal sealed class FireStationAIOverrides : BasicBuildingAIOverrides<FireStationAIOverrides, FireStationAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.Fire] = Tuple.New(VehicleInfo.VehicleType.Car, true, false)
        };
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(FireStationAI ai, TransferManager.TransferOffer offer) => reasons;
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => "m_fireTruckCount";
        public override bool AllowVehicleType(VehicleInfo.VehicleType type) => type == VehicleInfo.VehicleType.Car;
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
        public override bool AllowVehicleType(VehicleInfo.VehicleType type) => type == VehicleInfo.VehicleType.Helicopter;
    }

    internal sealed class HospitalAIOverrides : BasicBuildingAIOverrides<HospitalAIOverrides, HospitalAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.Sick] = Tuple.New(VehicleInfo.VehicleType.Car, true, false)
        };
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(HospitalAI ai, TransferManager.TransferOffer offer) => reasons;

        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => "m_ambulanceCount";
        public override bool AllowVehicleType(VehicleInfo.VehicleType type) => type == VehicleInfo.VehicleType.Car;
    }

    internal sealed class MaintenanceDepotAIOverrides : BasicBuildingAIOverrides<MaintenanceDepotAIOverrides, MaintenanceDepotAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.RoadMaintenance] = Tuple.New(VehicleInfo.VehicleType.Car, true, false)
        };
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(MaintenanceDepotAI ai, TransferManager.TransferOffer offer) => reasons;
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => "m_maintenanceTruckCount";
        public override bool AllowVehicleType(VehicleInfo.VehicleType type) => type == VehicleInfo.VehicleType.Car;
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
        public override bool AllowVehicleType(VehicleInfo.VehicleType type) => type == VehicleInfo.VehicleType.Car;
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
        public override bool AllowVehicleType(VehicleInfo.VehicleType type) => type == VehicleInfo.VehicleType.Car;
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
        public override bool AllowVehicleType(VehicleInfo.VehicleType type) => type == VehicleInfo.VehicleType.Car;
    }

    internal sealed class LandfillSiteAIOverrides : BasicBuildingAIOverrides<LandfillSiteAIOverrides, LandfillSiteAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.Garbage] = Tuple.New(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.GarbageMove] = Tuple.New(VehicleInfo.VehicleType.Car, false, true),
        };

        public override bool AllowVehicleType(VehicleInfo.VehicleType type) => type == VehicleInfo.VehicleType.Car;
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(LandfillSiteAI ai, TransferManager.TransferOffer offer) => reasons;
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => "m_garbageTruckCount";
    }

    internal sealed class DepotAIOverrides : BasicBuildingAIOverrides<DepotAIOverrides, DepotAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.Taxi] = Tuple.New(VehicleInfo.VehicleType.Car, false, true),
        };

        public override bool AllowVehicleType(VehicleInfo.VehicleType type) => type == VehicleInfo.VehicleType.Car;
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(DepotAI ai, TransferManager.TransferOffer offer) => reasons;
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => "m_maxVehicleCount";

    }

    internal sealed class CableCarStationAIOverrides : BasicBuildingAIOverrides<CableCarStationAIOverrides, CableCarStationAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.CableCar] = Tuple.New(VehicleInfo.VehicleType.CableCar, false, true),
        };

        public override bool AllowVehicleType(VehicleInfo.VehicleType type) => type == VehicleInfo.VehicleType.CableCar;
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(CableCarStationAI ai, TransferManager.TransferOffer offer) => reasons;
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => "m_maxVehicleCount";

    }
    internal sealed class TransportStationAIOverrides : BasicBuildingAIOverrides<TransportStationAIOverrides, TransportStationAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.PassengerTrain] = Tuple.New(VehicleInfo.VehicleType.Train, false, true),
            [TransferManager.TransferReason.PassengerPlane] = Tuple.New(VehicleInfo.VehicleType.Plane, false, true),
        };

        public override bool AllowVehicleType(VehicleInfo.VehicleType type) => type == VehicleInfo.VehicleType.Train || type == VehicleInfo.VehicleType.Plane;
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(TransportStationAI ai, TransferManager.TransferOffer offer)
        {
            if (offer.TransportLine != 0)
            {
                return new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>();
            }
            return reasons;
        }
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => "m_maxVehicleCount";

    }
    internal sealed class HarborAIOverrides : BasicBuildingAIOverrides<HarborAIOverrides, HarborAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.PassengerShip] = Tuple.New(VehicleInfo.VehicleType.Ship, false, true),
        };

        public override bool AllowVehicleType(VehicleInfo.VehicleType type) => type == VehicleInfo.VehicleType.Ship;
        public override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(HarborAI ai, TransferManager.TransferOffer offer)
        {
            if (offer.TransportLine != 0)
            {
                return new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>();
            }
            return reasons;
        }
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh) => "m_maxVehicleCount";

    }
}
