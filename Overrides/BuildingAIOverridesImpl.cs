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
    internal sealed class DisasterResponseBuildingAIOverrides : BasicBuildingAIOverrides<DisasterResponseBuildingAIOverrides, DisasterResponseBuildingAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.Collapsed] = Tuple.New(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.Collapsed2] = Tuple.New(VehicleInfo.VehicleType.Helicopter, true, false),
        };
        protected override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(DisasterResponseBuildingAI ai)
        {
            return reasons;
        }
    }
    internal sealed class CemeteryAIOverrides : BasicBuildingAIOverrides<CemeteryAIOverrides, CemeteryAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.Dead] = Tuple.New(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.DeadMove] = Tuple.New(VehicleInfo.VehicleType.Car, false, true),
        };
        protected override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(CemeteryAI ai)
        {
            return reasons;
        }
    }
    internal sealed class FireStationAIOverrides : BasicBuildingAIOverrides<FireStationAIOverrides, FireStationAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.Fire] = Tuple.New(VehicleInfo.VehicleType.Car, true, false)
        };
        protected override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(FireStationAI ai)
        {
            return reasons;
        }
    }
    internal sealed class HelicopterDepotAIOverrides : BasicBuildingAIOverrides<HelicopterDepotAIOverrides, HelicopterDepotAI>
    {
        protected override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(HelicopterDepotAI ai)
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
    }
    internal sealed class HospitalAIOverrides : BasicBuildingAIOverrides<HospitalAIOverrides, HospitalAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.Sick] = Tuple.New(VehicleInfo.VehicleType.Car, true, false)
        };
        protected override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(HospitalAI ai)
        {
            return reasons;
        }
    }
    internal sealed class MaintenanceDepotAIOverrides : BasicBuildingAIOverrides<MaintenanceDepotAIOverrides, MaintenanceDepotAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.RoadMaintenance] = Tuple.New(VehicleInfo.VehicleType.Car, true, false)
        };
        protected override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(MaintenanceDepotAI ai)
        {
            return reasons;
        }
    }
    internal sealed class PoliceStationAIOverrides : BasicBuildingAIOverrides<PoliceStationAIOverrides, PoliceStationAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.Crime] = Tuple.New(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.CriminalMove] = Tuple.New(VehicleInfo.VehicleType.Car, true, false)
        };
        protected override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(PoliceStationAI ai)
        {
            return reasons;
        }
    }
    internal sealed class SnowDumpAIOverrides : BasicBuildingAIOverrides<SnowDumpAIOverrides, SnowDumpAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.Snow] = Tuple.New(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.SnowMove] = Tuple.New(VehicleInfo.VehicleType.Car, false, true),
        };
        protected override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(SnowDumpAI ai)
        {
            return reasons;
        }
    }
    internal sealed class WaterFacilityAIOverrides : BasicBuildingAIOverrides<WaterFacilityAIOverrides, WaterFacilityAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.FloodWater] = Tuple.New(VehicleInfo.VehicleType.Car, true, false),
        };
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasonsNull = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>();
        protected override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(WaterFacilityAI ai)
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
    }
    internal sealed class LandfillSiteAIOverrides : BasicBuildingAIOverrides<LandfillSiteAIOverrides, LandfillSiteAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> reasons = new Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>>
        {
            [TransferManager.TransferReason.Garbage] = Tuple.New(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.GarbageMove] = Tuple.New(VehicleInfo.VehicleType.Car, false, true),
        };
        protected override Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(LandfillSiteAI ai)
        {
            return reasons;
        }
    }
}
