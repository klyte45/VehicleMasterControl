using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.TransportLinesManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager.Utils
{
    class SVMUtils : KlyteUtils
    {
        #region Logging
        public static void doLog(string format, params object[] args)
        {
            if (ServiceVehiclesManagerMod.instance != null)
            {
                if (ServiceVehiclesManagerMod.debugMode)
                {
                    Debug.LogWarningFormat("SVMv" + ServiceVehiclesManagerMod.version + " " + format, args);
                }
            }
            else
            {
                Console.WriteLine("SVMv" + ServiceVehiclesManagerMod.version + " " + format, args);
            }
        }
        public static void doErrorLog(string format, params object[] args)
        {
            if (ServiceVehiclesManagerMod.instance != null)
            {
                Debug.LogErrorFormat("SVMv" + ServiceVehiclesManagerMod.version + " " + format, args);
            }
            else
            {
                Console.WriteLine("SVMv" + ServiceVehiclesManagerMod.version + " " + format, args);
            }

        }

        internal static List<string> LoadBasicAssets(ServiceSystemDefinition definition)
        {
            List<string> basicAssetsList = new List<string>();            
            for (uint num = 0u; (ulong)num < (ulong)((long)PrefabCollection<VehicleInfo>.PrefabCount()); num += 1u)
            {
                VehicleInfo prefab = PrefabCollection<VehicleInfo>.GetPrefab(num);
                if (!(prefab == null) && definition.isFromSystem(prefab) && !IsTrailer(prefab))
                {
                    basicAssetsList.Add(prefab.name);
                }
            }
            return basicAssetsList;
        }
        #endregion
    }
}
