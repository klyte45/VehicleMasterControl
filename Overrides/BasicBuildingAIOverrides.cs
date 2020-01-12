using ColossalFramework;
using ColossalFramework.UI;
using Klyte.Commons.Extensors;
using Klyte.Commons.Utils;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ItemClass;
using static VehicleInfo;

namespace Klyte.ServiceVehiclesManager.Overrides
{
    internal interface IBasicBuildingAIOverrides
    {
        string GetVehicleMaxCountField(VehicleInfo.VehicleType veh, Level level);
        Dictionary<TransferManager.TransferReason, StartTransferCallStructure> GetManagedReasons(BuildingInfo info);
        bool AllowVehicleType(VehicleInfo.VehicleType type, PrefabAI ai);
        bool AcceptsAI(PrefabAI ai);
        List<Level> ExtraAllowedLevels();
    }
    internal interface IBasicBuildingAIOverrides<U> : IBasicBuildingAIOverrides where U : PrefabAI
    {
        bool AllowVehicleType(VehicleInfo.VehicleType veh, U ai);
        Dictionary<TransferManager.TransferReason, StartTransferCallStructure> GetManagedReasons(U ai, TransferManager.TransferOffer offer);
    }

    internal struct StartTransferCallStructure
    {
        public VehicleType vehicleType;
        public bool tramsferToSource;
        public bool transferToTarget;
        public Level? vehicleLevel;

        public StartTransferCallStructure(VehicleType vehicleType, bool tramsferToSource, bool transferToTarget, Level? vehicleLevel = null)
        {
            this.vehicleType = vehicleType;
            this.tramsferToSource = tramsferToSource;
            this.transferToTarget = transferToTarget;
            this.vehicleLevel = vehicleLevel;
        }
    }

    internal abstract class BasicBuildingAIOverrides<T, U> : Redirector, IRedirectable, IBasicBuildingAIOverrides<U> where T : BasicBuildingAIOverrides<T, U>, new() where U : BuildingAI
    {
        #region Overrides
        protected static List<Level> emptyLevelList = new List<Level>();

        public static BasicBuildingAIOverrides<T, U> instance;
        public Redirector RedirectorInstance => this;

        public Dictionary<TransferManager.TransferReason, StartTransferCallStructure> GetManagedReasons(BuildingInfo info) => GetManagedReasons((U) info.GetAI(), default(TransferManager.TransferOffer));
        public abstract Dictionary<TransferManager.TransferReason, StartTransferCallStructure> GetManagedReasons(U ai, TransferManager.TransferOffer offer);
        public abstract bool AllowVehicleType(VehicleInfo.VehicleType type, U ai);
        public abstract string GetVehicleMaxCountField(VehicleInfo.VehicleType veh, Level level);
        public bool AllowVehicleType(VehicleInfo.VehicleType veh, PrefabAI ai) => AllowVehicleType(veh, (U) ai);
        public virtual bool AcceptsAI(PrefabAI ai) => true;

        public virtual List<Level> ExtraAllowedLevels() => emptyLevelList;

        public static bool StartTransferDepot(U __instance, ushort buildingID, ref Building data, TransferManager.TransferReason reason, TransferManager.TransferOffer offer) => StartTransfer(__instance, buildingID, ref data, reason, offer);

        public static bool StartTransfer(U __instance, ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            var managedReasons = instance.GetManagedReasons(__instance, offer);
            if (!managedReasons?.Keys.Contains(material) ?? true)
            {
                return true;
            }

            LogUtils.DoLog("START TRANSFER: {0} , {1}", typeof(U), material);
            foreach (var tr in managedReasons)
            {
                if (instance.ProcessOffer(buildingID, ref data, material, offer, tr.Key, tr.Value, __instance))
                {
                    return false;
                }
            }
            LogUtils.DoLog("END TRANSFER: {0} , {1}", typeof(U), material);
            return true;
        }

        protected virtual bool ProcessOffer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer, TransferManager.TransferReason trTarget, StartTransferCallStructure tup, U instance)
        {
            return true;
            //if (material == trTarget)
            //{
            //    var def = ServiceSystemDefinition.from(instance.m_info, tup.vehicleType);
            //    if (def == null)
            //    {
            //        LogUtils.DoLog("SSD Não definido para: {0} {1} {2} {3}", instance.m_info.m_class.m_service, instance.m_info.m_class.m_subService, tup.vehicleLevel ?? instance.m_info.m_class.m_level, tup.vehicleType);
            //        return false;
            //    }

            //    #region District Check
            //    var ext = def.GetTransportExtension();

            //    if (ext.GetAllowDistrictServiceRestrictions())
            //    {
            //        byte buildingDistrict = DistrictManager.instance.GetDistrict(data.m_position);
            //        byte offerDistrict = DistrictManager.instance.GetDistrict(offer.Position);
            //        if (buildingDistrict != offerDistrict)
            //        {
            //            if (ServiceVehiclesManagerMod.debugMode)
            //            {
            //                LogUtils.DoLog($"Building serving different district: building {data.Info} ({buildingID} - districtId {buildingDistrict} {DistrictManager.instance.GetDistrictName(buildingDistrict)}) => offerDistrict: {offerDistrict} {DistrictManager.instance.GetDistrictName(offerDistrict)}");
            //            }

            //            if (!ext.GetAllowOutsidersEffective(offerDistrict))
            //            {
            //                List<ushort> districtBuildings = SVMBuildingUtils.getAllBuildingsFromCity(def, offerDistrict, true, true);

            //                if (ServiceVehiclesManagerMod.debugMode)
            //                {
            //                    LogUtils.DoLog($"Offer doesn't allow outsiders. Available buildings found: {districtBuildings}");
            //                }

            //                if (districtBuildings.Count > 0)
            //                {
            //                    ushort newBuildingId = districtBuildings[Singleton<SimulationManager>.instance.m_randomizer.Int32((uint) districtBuildings.Count)];
            //                    return ProcessOffer(newBuildingId, ref BuildingManager.instance.m_buildings.m_buffer[newBuildingId], material, offer, trTarget, tup, instance);
            //                }
            //                if (ServiceVehiclesManagerMod.debugMode)
            //                {
            //                    LogUtils.DoLog($"Ignoring because no buildings can handle it");
            //                }
            //            }
            //            if (!ext.GetAllowGoOutsideEffective(buildingDistrict))
            //            {
            //                List<ushort> districtBuildings = SVMBuildingUtils.getAllBuildingsFromCity(def, offerDistrict, false, true);
            //                if (ServiceVehiclesManagerMod.debugMode)
            //                {
            //                    LogUtils.DoLog($"Building district doesn't allow go out. Available buildings found: {districtBuildings}");
            //                }

            //                if (districtBuildings.Count > 0)
            //                {
            //                    ushort newBuildingId = districtBuildings[Singleton<SimulationManager>.instance.m_randomizer.Int32((uint) districtBuildings.Count)];
            //                    return ProcessOffer(newBuildingId, ref BuildingManager.instance.m_buildings.m_buffer[newBuildingId], material, offer, trTarget, tup, instance);
            //                }
            //                if (ServiceVehiclesManagerMod.debugMode)
            //                {
            //                    LogUtils.DoLog($"Ignoring because no buildings can handle it (2)");
            //                }
            //            }
            //        }
            //    }
            //    #endregion

            //    LogUtils.DoLog("[{1}] SSD = {0}", def, material);
            //    VehicleInfo randomVehicleInfo = ServiceSystemDefinition.sysDefinitions[def].GetAModel(buildingID);
            //    LogUtils.DoLog("[{1}] Veh = {0}", randomVehicleInfo?.ToString() ?? "<NULL>", material);
            //    if (randomVehicleInfo != null)
            //    {
            //        Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
            //        instance.CalculateSpawnPosition(buildingID, ref data, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, out Vector3 position, out Vector3 vector2);
            //        if (Singleton<VehicleManager>.instance.CreateVehicle(out ushort num, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, position, material, tup.tramsferToSource, tup.transferToTarget))
            //        {
            //            randomVehicleInfo.m_vehicleAI.SetSource(num, ref vehicles.m_buffer[num], buildingID);
            //            randomVehicleInfo.m_vehicleAI.StartTransfer(num, ref vehicles.m_buffer[num], material, offer);
            //            return true;
            //        }
            //    }
            //}
            //return false;
        }
        #endregion

        #region Hooking

        public void Awake()
        {
            instance = this;
            System.Reflection.MethodInfo from = typeof(U).GetMethod("StartTransfer", RedirectorUtils.allFlags);
            if (from == null)
            {
                if (typeof(DepotAI).IsAssignableFrom(typeof(U)))
                {
                    from = typeof(DepotAI).GetMethod("StartTransfer", RedirectorUtils.allFlags);
                }
            }
            System.Reflection.MethodInfo to = typeof(BasicBuildingAIOverrides<T, U>).GetMethod(typeof(DepotAI).IsAssignableFrom(typeof(U)) ? "StartTransferDepot" : "StartTransfer", RedirectorUtils.allFlags);
            LogUtils.DoLog("Loading Hooks: {0} ({1}=>{2})", typeof(U), from, to);
            if (from == null)
            {
                LogUtils.DoErrorLog($"ORIGINAL METHOD NOT FOUND FOR {typeof(U)}! Skipping");
            }
            else
            {
                AddRedirect(from, to);
            }
        }

        #endregion

    }
}
