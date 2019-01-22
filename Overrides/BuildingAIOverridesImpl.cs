using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.UI;
using Klyte.Commons.Utils;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static ItemClass;

namespace Klyte.ServiceVehiclesManager.Overrides
{
    internal sealed class SVMBuildingAIOverrideUtils
    {
        private static ILookup<Type, Type> subtypes = null;

        public static List<IBasicBuildingAIOverrides> getBuildingOverrideExtension(BuildingInfo info)
        {
            PrefabAI targetAi = info.GetAI();
            Type targetTypeAi = targetAi.GetType();
            if (subtypes == null)
            {
                var subclasses = SVMUtils.GetSubtypesRecursive(typeof(BasicBuildingAIOverrides<,>), typeof(SVMBuildingAIOverrideUtils));
                SVMUtils.doLog("GetOverride pré - subclasses:\r\n\t{0}", string.Join("\r\n\t", subclasses?.Select(x => x.ToString())?.ToArray() ?? new string[0]));
                subtypes = subclasses.ToLookup(x =>
                {
                    try
                    {
                        return x.BaseType.GetGenericArguments()[1];
                    }
                    catch
                    {
                        SVMUtils.doErrorLog("ERROR ADDING SUBTYPE {0}!\r\n{1}", x, subclasses);
                        return null;
                    }
                }, x => x);

                SVMUtils.doLog("GetOverride - Classes:\r\n\t{0}", string.Join("\r\n\t", subtypes?.Select(x => x.Key.ToString() + "=>" + x.ToList().ToString())?.ToArray() ?? new string[0]));
            }
            List<Type> targetClasses = new List<Type>();
            List<IBasicBuildingAIOverrides> value = null;
            if (!subtypes.Contains(targetTypeAi))
            {
                foreach (var clazz in subtypes.Select(x => x.Key))
                {
                    if (clazz.IsAssignableFrom(targetTypeAi))
                    {
                        foreach (var selClazz in subtypes[clazz])
                        {
                            var ai = (IBasicBuildingAIOverrides)SVMUtils.GetPrivateStaticField("instance", selClazz);
                            //SVMUtils.doLog("GetOverride - clazz = {0}; value = {1}", clazz, value);
                            if (ai.AcceptsAI(targetAi))
                            {
                                targetClasses.Add(selClazz);
                            }
                        }
                    }
                }
                SVMUtils.doLog("GetOverride - targetClasses = {0} ({1})", targetClasses, targetTypeAi);
            }
            else
            {
                targetClasses = subtypes[targetTypeAi].ToList();
            }
            value = targetClasses.Select(targetClass => (IBasicBuildingAIOverrides)SVMUtils.GetPrivateStaticField("instance", targetClass)).ToList();
            //SVMUtils.doLog("GetOverride - value = {0}", value);
            return value;
        }

        public static IBasicBuildingAIOverrides getBuildingOverrideExtensionStrict(BuildingInfo info)
        {
            return getBuildingOverrideExtension(info).FirstOrDefault(x => !x.ExtraAllowedLevels().Contains(info.m_class.m_level));
        }
    }


    internal sealed class DisasterResponseBuildingAIOverrides : BasicBuildingAIOverrides<DisasterResponseBuildingAIOverrides, DisasterResponseBuildingAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasons = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.Collapsed] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.Collapsed2] = new StartTransferCallStructure(VehicleInfo.VehicleType.Helicopter, true, false),
        };
        public override Dictionary<TransferManager.TransferReason, StartTransferCallStructure> GetManagedReasons(DisasterResponseBuildingAI ai, TransferManager.TransferOffer offer) => reasons;

        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh, Level level)
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
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasons = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.Dead] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.DeadMove] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, false, true),
        };
        public override Dictionary<TransferManager.TransferReason, StartTransferCallStructure> GetManagedReasons(CemeteryAI ai, TransferManager.TransferOffer offer) => reasons;
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh, Level level) => "m_hearseCount";
        public override bool AllowVehicleType(VehicleInfo.VehicleType type, CemeteryAI ai) => type == VehicleInfo.VehicleType.Car;
    }

    internal sealed class FireStationAIOverrides : BasicBuildingAIOverrides<FireStationAIOverrides, FireStationAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasons = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.Fire] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false)
        };
        public override Dictionary<TransferManager.TransferReason, StartTransferCallStructure> GetManagedReasons(FireStationAI ai, TransferManager.TransferOffer offer) => reasons;
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh, Level level) => "m_fireTruckCount";
        public override bool AllowVehicleType(VehicleInfo.VehicleType type, FireStationAI ai) => type == VehicleInfo.VehicleType.Car;
    }

    internal sealed class HelicopterDepotAIOverrides : BasicBuildingAIOverrides<HelicopterDepotAIOverrides, HelicopterDepotAI>
    {
        public override Dictionary<TransferManager.TransferReason, StartTransferCallStructure> GetManagedReasons(HelicopterDepotAI ai, TransferManager.TransferOffer offer)
        {
            var r1 = (TransferManager.TransferReason)SVMUtils.ExecuteReflectionMethod(ai, "GetTransferReason1");
            var r2 = (TransferManager.TransferReason)SVMUtils.ExecuteReflectionMethod(ai, "GetTransferReason2");

            var result = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>();

            if (r1 != TransferManager.TransferReason.None)
            {
                result[r1] = new StartTransferCallStructure(VehicleInfo.VehicleType.Helicopter, true, false);
            }
            if (r2 != TransferManager.TransferReason.None)
            {
                result[r2] = new StartTransferCallStructure(VehicleInfo.VehicleType.Helicopter, true, false);
            }

            return result;
        }

        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh, Level level) => "m_helicopterCount";
        public override bool AllowVehicleType(VehicleInfo.VehicleType type, HelicopterDepotAI ai) => type == VehicleInfo.VehicleType.Helicopter;
    }

    internal sealed class HospitalAIOverrides : BasicBuildingAIOverrides<HospitalAIOverrides, HospitalAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasons = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.Sick] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false)
        };
        public override Dictionary<TransferManager.TransferReason, StartTransferCallStructure> GetManagedReasons(HospitalAI ai, TransferManager.TransferOffer offer) => reasons;

        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh, Level level) => "m_ambulanceCount";
        public override bool AllowVehicleType(VehicleInfo.VehicleType type, HospitalAI ai) => type == VehicleInfo.VehicleType.Car;
    }

    internal sealed class MaintenanceDepotAIOverrides : BasicBuildingAIOverrides<MaintenanceDepotAIOverrides, MaintenanceDepotAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasons = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.RoadMaintenance] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.ParkMaintenance] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false)
        };
        public override Dictionary<TransferManager.TransferReason, StartTransferCallStructure> GetManagedReasons(MaintenanceDepotAI ai, TransferManager.TransferOffer offer) => reasons;
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh, Level level) => "m_maintenanceTruckCount";
        public override bool AllowVehicleType(VehicleInfo.VehicleType type, MaintenanceDepotAI ai) => type == VehicleInfo.VehicleType.Car;
    }

    internal sealed class PoliceStationAIOverrides : BasicBuildingAIOverrides<PoliceStationAIOverrides, PoliceStationAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasons = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.Crime] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.CriminalMove] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false),
        };
        public override Dictionary<TransferManager.TransferReason, StartTransferCallStructure> GetManagedReasons(PoliceStationAI ai, TransferManager.TransferOffer offer) => reasons;
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh, Level level) => "m_policeCarCount";
        public override bool AllowVehicleType(VehicleInfo.VehicleType type, PoliceStationAI ai) => type == VehicleInfo.VehicleType.Car;
    }

    internal sealed class SnowDumpAIOverrides : BasicBuildingAIOverrides<SnowDumpAIOverrides, SnowDumpAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasons = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.Snow] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.SnowMove] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, false, true),
        };
        public override Dictionary<TransferManager.TransferReason, StartTransferCallStructure> GetManagedReasons(SnowDumpAI ai, TransferManager.TransferOffer offer) => reasons;
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh, Level level) => "m_snowTruckCount";
        public override bool AllowVehicleType(VehicleInfo.VehicleType type, SnowDumpAI ai) => type == VehicleInfo.VehicleType.Car;
    }

    internal sealed class WaterFacilityAIOverrides : BasicBuildingAIOverrides<WaterFacilityAIOverrides, WaterFacilityAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasons = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.FloodWater] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false),
        };
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasonsNull = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>();
        public override Dictionary<TransferManager.TransferReason, StartTransferCallStructure> GetManagedReasons(WaterFacilityAI ai, TransferManager.TransferOffer offer)
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

        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh, Level level) => "m_pumpingVehicles";
        public override bool AllowVehicleType(VehicleInfo.VehicleType type, WaterFacilityAI ai) => type == VehicleInfo.VehicleType.Car;
    }

    internal sealed class LandfillSiteAIOverrides : BasicBuildingAIOverrides<LandfillSiteAIOverrides, LandfillSiteAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasons = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.Garbage] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.GarbageMove] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, false, true),
        };

        public override bool AllowVehicleType(VehicleInfo.VehicleType type, LandfillSiteAI ai) => type == VehicleInfo.VehicleType.Car;
        public override Dictionary<TransferManager.TransferReason, StartTransferCallStructure> GetManagedReasons(LandfillSiteAI ai, TransferManager.TransferOffer offer) => reasons;
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh, Level level) => "m_garbageTruckCount";
    }


    internal sealed class PostOfficeAIOverrides : BasicBuildingAIOverrides<PostOfficeAIOverrides, PostOfficeAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasons = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.UnsortedMail] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, false, true, Level.Level5),
            [TransferManager.TransferReason.IncomingMail] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false, Level.Level5),
            [TransferManager.TransferReason.OutgoingMail] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false, Level.Level5),
            [TransferManager.TransferReason.SortedMail] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false, Level.Level5),
            [TransferManager.TransferReason.Mail] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false)
        };

        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasonsInv = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.UnsortedMail] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false, Level.Level5),
            [TransferManager.TransferReason.IncomingMail] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, false, true, Level.Level5),
            [TransferManager.TransferReason.OutgoingMail] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, false, true, Level.Level5),
            [TransferManager.TransferReason.SortedMail] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, false, true, Level.Level5),
            [TransferManager.TransferReason.Mail] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false)
        };

        public override bool AllowVehicleType(VehicleInfo.VehicleType type, PostOfficeAI ai) => type == VehicleInfo.VehicleType.Car;
        public override Dictionary<TransferManager.TransferReason, StartTransferCallStructure> GetManagedReasons(PostOfficeAI ai, TransferManager.TransferOffer offer) => ai.m_sortingRate == 0 ? reasonsInv : reasons;
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh, Level level) => level == Level.Level5 ? "m_postTruckCount" : "m_postVanCount";
        public override List<Level> ExtraAllowedLevels() => new Level[] { Level.Level5 }.ToList();

        protected override bool ProcessOffer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer, TransferManager.TransferReason trTarget, StartTransferCallStructure tup, PostOfficeAI instance)
        {
            if (material == TransferManager.TransferReason.Mail)
            {
                //return base.ProcessOffer(buildingID, ref data, material, offer, trTarget, tup, instance);
                if (material == trTarget)
                {
                    SVMUtils.doLog("VAN POSTOFFICE!");
                    ServiceSystemDefinition def = ServiceSystemDefinition.from(instance.m_info, tup.vehicleType);
                    if (def == null)
                    {
                        SVMUtils.doLog("SSD Não definido para: {0} {1} {2} {3}", instance.m_info.m_class.m_service, instance.m_info.m_class.m_subService, tup.vehicleLevel ?? instance.m_info.m_class.m_level, tup.vehicleType);
                        return false;
                    }

                    SVMUtils.doLog("[{1}] SSD = {0}", def, material);
                    VehicleInfo randomVehicleInfo = ServiceSystemDefinition.availableDefinitions[def].GetAModel(buildingID);
                    SVMUtils.doLog("[{1}] Veh = {0}", randomVehicleInfo?.ToString() ?? "<NULL>", material);
                    if (randomVehicleInfo != null)
                    {
                        Vehicle vehicle = default(Vehicle);
                        randomVehicleInfo.m_vehicleAI.GetSize(0, ref vehicle, out int num, out int num2);
                        int num3 = (data.m_customBuffer2 * 1000);
                        if (num3 >= num2)
                        {
                            Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                            instance.CalculateSpawnPosition(buildingID, ref data, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, out Vector3 position, out Vector3 vector2);
                            if (Singleton<VehicleManager>.instance.CreateVehicle(out ushort vehId, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, position, material, true, false))
                            {
                                randomVehicleInfo.m_vehicleAI.SetSource(vehId, ref vehicles.m_buffer[(int)vehId], buildingID);
                                randomVehicleInfo.m_vehicleAI.StartTransfer(vehId, ref vehicles.m_buffer[(int)vehId], material, offer);
                                return true;
                            }
                        }

                    }
                }
                return false;
            }
            else return PostOfficeAITruckOverrides.ProcessOfferTruck(buildingID, ref data, material, offer, trTarget, tup, instance);

        }
    }

    internal sealed class PostOfficeAITruckOverrides : BasicBuildingAIOverrides<PostOfficeAITruckOverrides, PostOfficeAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasons = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.UnsortedMail] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, false, true),
            [TransferManager.TransferReason.IncomingMail] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.OutgoingMail] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.SortedMail] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false),
        };

        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasonsInv = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.UnsortedMail] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, true, false),
            [TransferManager.TransferReason.IncomingMail] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, false, true),
            [TransferManager.TransferReason.OutgoingMail] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, false, true),
            [TransferManager.TransferReason.SortedMail] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, false, true),
        };

        public override bool AllowVehicleType(VehicleInfo.VehicleType type, PostOfficeAI ai) => type == VehicleInfo.VehicleType.Car;
        public override Dictionary<TransferManager.TransferReason, StartTransferCallStructure> GetManagedReasons(PostOfficeAI ai, TransferManager.TransferOffer offer) => ai.m_sortingRate == 0 ? reasonsInv : reasons;

        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh, Level level) => "m_postTruckCount";

        protected override bool ProcessOffer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer, TransferManager.TransferReason trTarget, StartTransferCallStructure tup, PostOfficeAI instance)
        {
            return ProcessOfferTruck(buildingID, ref data, material, offer, trTarget, tup, instance);
        }
        public static bool ProcessOfferTruck(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer, TransferManager.TransferReason trTarget, StartTransferCallStructure tup, PostOfficeAI instance)
        {
            if (material == trTarget)
            {
                SVMUtils.doLog("TRUCK POSTOFFICE!");
                ServiceSystemDefinition def = ServiceSystemDefinition.from(instance.m_info, tup.vehicleType);
                if (def == null)
                {
                    SVMUtils.doLog("SSD Não definido para: {0} {1} {2} {3}", instance.m_info.m_class.m_service, instance.m_info.m_class.m_subService, tup.vehicleLevel ?? instance.m_info.m_class.m_level, tup.vehicleType);
                    return false;
                }
                object[] args = new object[] { buildingID, data, 0, 0, 0, 0, 0, 0, 0, 0 };
                SVMUtils.RunPrivateMethod<object>(instance, "CalculateVehicles", args);
                if (ServiceVehiclesManagerMod.debugMode) SVMUtils.doLog($"[{material}]RESULT: {string.Join(";", args.Select(x => x?.ToString()).ToArray())}");
                bool flag = (instance.m_sortingRate == 0) ? (material != TransferManager.TransferReason.UnsortedMail) : (material == TransferManager.TransferReason.UnsortedMail);
                if ((int)args[7] < instance.m_postTruckCount)
                {
                    SVMUtils.doLog("[{1}] SSD = {0}", def, material);
                    VehicleInfo randomVehicleInfo = ServiceSystemDefinition.availableDefinitions[def].GetAModel(buildingID);
                    SVMUtils.doLog("[{1}] Veh = {0}", randomVehicleInfo?.ToString() ?? "<NULL>", material);
                    if (randomVehicleInfo != null)
                    {
                        Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                        instance.CalculateSpawnPosition(buildingID, ref data, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, out Vector3 position, out Vector3 vector2);
                        if (Singleton<VehicleManager>.instance.CreateVehicle(out ushort vehId, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, position, material, flag, !flag))
                        {
                            randomVehicleInfo.m_vehicleAI.SetSource(vehId, ref vehicles.m_buffer[(int)vehId], buildingID);
                            randomVehicleInfo.m_vehicleAI.StartTransfer(vehId, ref vehicles.m_buffer[(int)vehId], material, offer);
                            ushort building = offer.Building;
                            if (building != 0 && (Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)building].m_flags & Building.Flags.IncomingOutgoing) != Building.Flags.None)
                            {
                                randomVehicleInfo.m_vehicleAI.GetSize(vehId, ref vehicles.m_buffer[(int)vehId], out int amount, out int num14);
                                CommonBuildingAI.ExportResource(buildingID, ref data, material, amount);
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }

    internal sealed class DepotAIOverrides : BasicBuildingAIOverrides<DepotAIOverrides, DepotAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasonsTaxi = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.Taxi] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, false, true),
        };
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasonsTrain = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.PassengerTrain] = new StartTransferCallStructure(VehicleInfo.VehicleType.Train, false, true),
            [TransferManager.TransferReason.DummyTrain] = new StartTransferCallStructure(VehicleInfo.VehicleType.Train, false, true),
        };
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasonsShip = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.PassengerShip] = new StartTransferCallStructure(VehicleInfo.VehicleType.Ship, false, true),
            [TransferManager.TransferReason.DummyShip] = new StartTransferCallStructure(VehicleInfo.VehicleType.Ship, false, true),
        };
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasonsPlane = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.PassengerPlane] = new StartTransferCallStructure(VehicleInfo.VehicleType.Plane, false, true),
            [TransferManager.TransferReason.DummyPlane] = new StartTransferCallStructure(VehicleInfo.VehicleType.Plane, false, true),
        };
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasonsCableCar = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.CableCar] = new StartTransferCallStructure(VehicleInfo.VehicleType.CableCar, false, true),
        };
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasonsNone = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
        };

        public override bool AllowVehicleType(VehicleInfo.VehicleType type, DepotAI ai)
        {
            if (ai is TransportStationAI) return false;
            List<VehicleInfo.VehicleType> allowedTypes = new List<VehicleInfo.VehicleType>();
            AddAllowedType(ai?.m_transportInfo?.m_transportType, ref allowedTypes);
            AddAllowedType(ai?.m_secondaryTransportInfo?.m_transportType, ref allowedTypes);
            return allowedTypes.Contains(type);
        }

        public static void AddAllowedType(TransportInfo.TransportType? type, ref List<VehicleInfo.VehicleType> allowedTypes)
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

        public override Dictionary<TransferManager.TransferReason, StartTransferCallStructure> GetManagedReasons(DepotAI ai, TransferManager.TransferOffer offer)
        {
            if (offer.TransportLine != 0)
            {
                return reasonsNone;
            }
            List<VehicleInfo.VehicleType> allowedTypes = new List<VehicleInfo.VehicleType>();
            AddAllowedType(ai?.m_transportInfo?.m_transportType, ref allowedTypes);
            AddAllowedType(ai?.m_secondaryTransportInfo?.m_transportType, ref allowedTypes);

            IEnumerable<KeyValuePair<TransferManager.TransferReason, StartTransferCallStructure>> result = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>();
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
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh, Level level) => "m_maxVehicleCount";
        public override bool AcceptsAI(PrefabAI ai) => typeof(DepotAI).IsAssignableFrom(ai.GetType());
    }

    internal sealed class TransportStationAIOverrides : BasicBuildingAIOverrides<TransportStationAIOverrides, TransportStationAI>
    {

        public override bool AllowVehicleType(VehicleInfo.VehicleType type, TransportStationAI ai)
        {
            List<VehicleInfo.VehicleType> allowedTypes = new List<VehicleInfo.VehicleType>();
            DepotAIOverrides.AddAllowedType(ai?.m_transportInfo?.m_transportType, ref allowedTypes);
            DepotAIOverrides.AddAllowedType(ai?.m_secondaryTransportInfo?.m_transportType, ref allowedTypes);
            return allowedTypes.Contains(type);
        }
        public override Dictionary<TransferManager.TransferReason, StartTransferCallStructure> GetManagedReasons(TransportStationAI ai, TransferManager.TransferOffer offer)
        {
            return DepotAIOverrides.instance.GetManagedReasons(ai, offer);
        }
        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh, Level level) => DepotAIOverrides.instance.GetVehicleMaxCountField(veh, level);


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
                    __instance.CalculateSpawnPosition(buildingID, ref buildingData, ref randomizer, randomVehicleInfo, out Vector3 vector, out Vector3 vector2);
                    TransportInfo transportInfo = __instance.m_transportInfo;
                    if (__instance.m_secondaryTransportInfo != null && __instance.m_secondaryTransportInfo.m_class.m_subService == __instance.m_transportLineInfo.m_class.m_subService)
                    {
                        transportInfo = __instance.m_secondaryTransportInfo;
                    }
                    if (randomVehicleInfo.m_vehicleAI.CanSpawnAt(vector) && Singleton<VehicleManager>.instance.CreateVehicle(out ushort num, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, vector, transportInfo.m_vehicleReason, false, true))
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
                        info.m_buildingAI.CalculateSpawnPosition(num, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)num], ref randomizer, randomVehicleInfo, out Vector3 vector, out Vector3 vector2);
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

        public override void AwakeBody()
        {
            instance = this;
            var from = typeof(TransportStationAI).GetMethod("CreateIncomingVehicle", allFlags);
            var to = typeof(TransportStationAIOverrides).GetMethod("CreateIncomingVehicle", allFlags);
            var from2 = typeof(TransportStationAI).GetMethod("CreateOutgoingVehicle", allFlags);
            var to2 = typeof(TransportStationAIOverrides).GetMethod("CreateOutgoingVehicle", allFlags);
            SVMUtils.doLog("Loading Hooks: {0} ({1}=>{2})", typeof(TransportStationAI), from, to);
            AddRedirect(from, to);
            SVMUtils.doLog("Loading Hooks: {0} ({1}=>{2})", typeof(TransportStationAI), from2, to2);
            AddRedirect(from2, to2);
        }
    }

    internal sealed class CargoStationAIOverrides : BasicBuildingAIOverrides<CargoStationAIOverrides, CargoStationAI>
    {
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasonsTrain = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.DummyTrain] = new StartTransferCallStructure(VehicleInfo.VehicleType.Train, false, true),
        };
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasonsShip = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.DummyShip] = new StartTransferCallStructure(VehicleInfo.VehicleType.Ship, false, true),
        };
        private readonly Dictionary<TransferManager.TransferReason, StartTransferCallStructure> reasonsPlane = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>
        {
            [TransferManager.TransferReason.DummyPlane] = new StartTransferCallStructure(VehicleInfo.VehicleType.Plane, false, true),
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

        public override Dictionary<TransferManager.TransferReason, StartTransferCallStructure> GetManagedReasons(CargoStationAI ai, TransferManager.TransferOffer offer)
        {
            List<VehicleInfo.VehicleType> allowedTypes = new List<VehicleInfo.VehicleType>();
            DepotAIOverrides.AddAllowedType(ai?.m_transportInfo?.m_transportType, ref allowedTypes);
            DepotAIOverrides.AddAllowedType(ai?.m_transportInfo2?.m_transportType, ref allowedTypes);
            IEnumerable<KeyValuePair<TransferManager.TransferReason, StartTransferCallStructure>> result = new Dictionary<TransferManager.TransferReason, StartTransferCallStructure>();
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

        public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh, Level level) => null;

        public override void AwakeBody()
        {
            instance = this;
        }
    }

    /* internal sealed class OutsideConnectionAIOverrides : BasicBuildingAIOverrides<OutsideConnectionAIOverrides, OutsideConnectionAI>
     {
         private readonly Dictionary<TransferManager.TransferReason,StartTransferCallStructure> reasons = new Dictionary<TransferManager.TransferReason,StartTransferCallStructure>
         {
             [TransferManager.TransferReason.DummyTrain] = new StartTransferCallStructure(VehicleInfo.VehicleType.Train, false, true),
             [TransferManager.TransferReason.DummyShip] = new StartTransferCallStructure(VehicleInfo.VehicleType.Ship, false, true),
             [TransferManager.TransferReason.DummyPlane] = new StartTransferCallStructure(VehicleInfo.VehicleType.Plane, false, true),
             [TransferManager.TransferReason.DummyCar] = new StartTransferCallStructure(VehicleInfo.VehicleType.Car, false, true),
         };

         public override bool AllowVehicleType(VehicleInfo.VehicleType type, OutsideConnectionAI ai) => type == VehicleInfo.VehicleType.Train || type == VehicleInfo.VehicleType.Car || type == VehicleInfo.VehicleType.Ship || type == VehicleInfo.VehicleType.Plane;
         public override Dictionary<TransferManager.TransferReason,StartTransferCallStructure> GetManagedReasons(OutsideConnectionAI ai, TransferManager.TransferOffer offer)
         {
             if (offer.TransportLine != 0)
             {
                 return new Dictionary<TransferManager.TransferReason,StartTransferCallStructure>();
             }
             return reasons;
         }
         public override string GetVehicleMaxCountField(VehicleInfo.VehicleType veh, Level level) => null;

         public override void AwakeBody()
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
             if (randomVehicleInfo != null)
             {
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
             }
             SVMUtils.doLog("END TRANSFER: {0} , {1} (not found)", typeof(OutsideConnectionAI), material);
             return true;
         }

     }
     */
}
