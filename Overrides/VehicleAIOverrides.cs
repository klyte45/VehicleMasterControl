using Klyte.Commons.Extensors;
using Klyte.Commons.Utils;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using System;
using System.Reflection;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager.Overrides
{
    internal class VehicleAIOverrides : Redirector, IRedirectable
    {
        public Redirector RedirectorInstance => this;

        public static bool PreGetColor(ref Color __result, ushort vehicleID, ref Vehicle data, InfoManager.InfoMode infoMode)
        {
            if (data.m_transportLine == 0)
            {
                if (infoMode != InfoManager.InfoMode.None)
                {
                    return true;
                }

                var def = ServiceSystemDefinition.from(data.Info);
                if (def == default)
                {
                    return true;
                }
                ushort buildingId = data.m_sourceBuilding;
                if (buildingId == 0)
                {
                    return true;
                }

                Color c = def.GetModelColor(buildingId);
                if (c == Color.clear)
                {
                    return true;
                }
                __result = c;
                return false;
            }
            return true;
        }

        public void Awake()
        {
            #region Release Line Hooks
            MethodInfo preGetColor = typeof(VehicleAIOverrides).GetMethod("PreGetColor", RedirectorUtils.allFlags);
            MethodInfo origMethod = typeof(VehicleAI).GetMethod("GetColor", new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(InfoManager.InfoMode) });

            MethodInfo origMethodTrain = typeof(PassengerTrainAI).GetMethod("GetColor", new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(InfoManager.InfoMode) });
            MethodInfo origMethodPlane = typeof(PassengerPlaneAI).GetMethod("GetColor", new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(InfoManager.InfoMode) });
            MethodInfo origMethodShip = typeof(PassengerShipAI).GetMethod("GetColor", new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(InfoManager.InfoMode) });

            LogUtils.DoLog("Loading VehicleAIOverrides ({0}=>{1})", origMethod, preGetColor);
            AddRedirect(origMethod, preGetColor);
            AddRedirect(origMethodTrain, preGetColor);
            AddRedirect(origMethodPlane, preGetColor);
            AddRedirect(origMethodShip, preGetColor);
            #endregion
        }
    }
}
