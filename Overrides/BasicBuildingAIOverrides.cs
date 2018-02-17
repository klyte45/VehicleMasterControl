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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager.Overrides
{
    internal abstract class BasicBuildingAIOverrides<T, U> : Redirector<T> where T : BasicBuildingAIOverrides<T, U>, new() where U : BuildingAI
    {
        #region Overrides
        protected static BasicBuildingAIOverrides<T, U> instance;

        protected abstract Dictionary<TransferManager.TransferReason, Tuple<VehicleInfo.VehicleType, bool, bool>> GetManagedReasons(U ai);

        public static bool StartTransfer(U __instance, ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            var managedReasons = instance?.GetManagedReasons(__instance);
            if (!managedReasons?.Keys.Contains(material) ?? true)
            {
                return true;
            }

            SVMUtils.doLog("START TRANSFER: {0}", typeof(U));
            foreach (var tr in managedReasons)
            {
                if (!ProcessOffer(buildingID, data, material, offer, tr.Key, tr.Value, __instance))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool ProcessOffer(ushort buildingID, Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer, TransferManager.TransferReason trTarget, Tuple<VehicleInfo.VehicleType, bool, bool> tup, U instance)
        {
            if (material == trTarget)
            {
                ServiceSystemDefinition def = ServiceSystemDefinition.from(instance.m_info, tup.First);
                if (def == null)
                {
                    SVMUtils.doLog("SSD Não definido para: {0} {1} {2} {3}", instance.m_info.m_class.m_service, instance.m_info.m_class.m_subService, instance.m_info.m_class.m_level, tup.First);
                    return true;
                }
                VehicleInfo randomVehicleInfo = ServiceSystemDefinition.availableDefinitions[def].GetAModel(buildingID);
                if (randomVehicleInfo != null)
                {
                    Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                    if (Singleton<VehicleManager>.instance.CreateVehicle(out ushort num, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, data.m_position, material, tup.Second, tup.Third))
                    {
                        randomVehicleInfo.m_vehicleAI.SetSource(num, ref vehicles.m_buffer[(int)num], buildingID);
                        randomVehicleInfo.m_vehicleAI.StartTransfer(num, ref vehicles.m_buffer[(int)num], material, offer);
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region Hooking

        public override void Awake()
        {
            instance = this;
            var from = typeof(U).GetMethod("StartTransfer", allFlags);
            var to = typeof(BasicBuildingAIOverrides<T, U>).GetMethod("StartTransfer", allFlags);
            SVMUtils.doLog("Loading Hooks: {0} ({1}=>{2})", typeof(U), from, to);
            AddRedirect(from, to);
        }

        #endregion
    }
    

    public class Tuple<T1, T2, T3>
    {
        public T1 First { get; private set; }
        public T2 Second { get; private set; }
        public T3 Third { get; private set; }
        internal Tuple(T1 first, T2 second, T3 third)
        {
            First = first;
            Second = second;
            Third = third;
        }
    }

    public static class Tuple
    {
        public static Tuple<T1, T2, T3> New<T1, T2, T3>(T1 first, T2 second, T3 third)
        {
            var tuple = new Tuple<T1, T2, T3>(first, second, third);
            return tuple;
        }
    }
}
