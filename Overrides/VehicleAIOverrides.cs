using ColossalFramework;
using Klyte.ServiceVehiclesManager.Utils;
using Klyte.TransportLinesManager.Extensors;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.UI.ExtraUI;
using Klyte.TransportLinesManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager.Overrides
{
    class VehicleAIOverrides : Redirector<VehicleAIOverrides>
    {
        public static bool PreGetColor(ref Color __result, ushort vehicleID, ref Vehicle data, InfoManager.InfoMode infoMode)
        {
            if (infoMode != InfoManager.InfoMode.None && infoMode == InfoManager.InfoMode.TrafficRoutes && Singleton<InfoManager>.instance.CurrentSubMode == InfoManager.SubInfoMode.Default)
            {
                return true;
            }

            ServiceSystemDefinition def = ServiceSystemDefinition.from(data.Info);
            if (def == default(ServiceSystemDefinition))
            {
                return true;
            }
            ushort buildingId = data.m_sourceBuilding;
            if (buildingId == 0)
            {
                return true;
            }

            Color c = def.GetTransportExtension().GetEffectiveColorBuilding(buildingId);
            if (c == Color.clear)
            {
                return true;
            }
            __result = c;
            return false;
        }

        public override void Awake()
        {
            #region Release Line Hooks
            MethodInfo preGetColor = typeof(VehicleAIOverrides).GetMethod("PreGetColor", allFlags);
            MethodInfo origMethod = typeof(VehicleAI).GetMethod("GetColor", new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(InfoManager.InfoMode) });

            SVMUtils.doLog("Loading VehicleAIOverrides ({0}=>{1})", origMethod, preGetColor);
            AddRedirect(origMethod, preGetColor);
            #endregion
        }
    }
}
