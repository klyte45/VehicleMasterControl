using Klyte.Commons.Utils;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager.Utils
{
    class SVMUtils : KlyteUtils
    {
        #region Logging
        public static void doLog(string format, params object[] args)
        {
            try
            {
                if (ServiceVehiclesManagerMod.debugMode)
                {
                    Console.WriteLine("SVMv" + ServiceVehiclesManagerMod.version + " " + format, args);
                }
            }
            catch
            {
                Debug.LogErrorFormat("SVMv" + ServiceVehiclesManagerMod.version + " Erro ao fazer log: {0} (args = {1})", format, args == null ? "[]" : string.Join(",", args.Select(x => x != null ? x.ToString() : "--NULL--").ToArray()));
            }
        }
        public static void doErrorLog(string format, params object[] args)
        {
            try
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
            catch
            {
                Debug.LogErrorFormat("SVMv" + ServiceVehiclesManagerMod.version + " Erro ao logar ERRO!!!: {0} (args = [{1}])", format, args == null ? "" : string.Join(",", args.Select(x => x != null ? x.ToString() : "--NULL--").ToArray()));
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
