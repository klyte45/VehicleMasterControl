using Klyte.Commons.Utils;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using System.Collections.Generic;
using System.Linq;

namespace Klyte.ServiceVehiclesManager.Utils
{
    internal static class SVMUtils
    {
        internal static List<string> LoadBasicAssets(ref ServiceSystemDefinition definition)
        {
            var basicAssetsList = new List<string>();
            for (uint num = 0u; num < (ulong) PrefabCollection<VehicleInfo>.PrefabCount(); num += 1u)
            {
                VehicleInfo prefab = PrefabCollection<VehicleInfo>.GetPrefab(num);
                if (!(prefab == null) && definition.isFromSystem(prefab) && !VehicleUtils.IsTrailer(prefab))
                {
                    basicAssetsList.Add(prefab.name);
                }
            }
            return basicAssetsList;
        }
        internal static T logAndReturn<T>(T itemToLog, string comment = "")
        {
            if (ServiceVehiclesManagerMod.DebugMode)
            {
                LogUtils.DoLog($"LOG OBJ: {itemToLog} ({comment})");
            }

            return itemToLog;
        }

        internal static T logAndReturn<T, X>(T itemToLog, string comment = "") where T : IEnumerable<X>
        {
            if (ServiceVehiclesManagerMod.DebugMode)
            {
                LogUtils.DoLog($"LOG OBJ: [{string.Join(",", itemToLog?.Select(x => x.ToString())?.ToArray())}] ({comment})");
            }

            return itemToLog;
        }
    }
}
