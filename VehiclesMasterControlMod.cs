using ColossalFramework;
using ColossalFramework.Globalization;
using Klyte.Commons.Extensors;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using Klyte.VehiclesMasterControl.UI;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyVersion("0.0.0.*")]

namespace Klyte.VehiclesMasterControl
{
    public class VehiclesMasterControlMod : BasicIUserMod<VehiclesMasterControlMod, VMCController, VMCTabPanel>
    {
        private SavedBool m_allowOutsidersAsDefault = new SavedBool("VMCAllowGoOutsideAsDefault", Settings.gameSettingsFile, true, true);
        private SavedBool m_allowGoOutsideAsDefault = new SavedBool("VMCAllowOutsidersAsDefault", Settings.gameSettingsFile, true, true);

        public static bool allowOutsidersAsDefault => Instance.m_allowOutsidersAsDefault.value;
        public static bool allowServeOtherDistrictsAsDefault => Instance.m_allowGoOutsideAsDefault.value;

        public override string SimpleName => "Vehicle Master Control";
        public override string Description => "Extension for managing service & industries vehicles.";

        public override string IconName => "K45_VMCIcon";

        public override void DoErrorLog(string fmt, params object[] args) => LogUtils.DoErrorLog(fmt, args);

        public override void DoLog(string fmt, params object[] args) => LogUtils.DoLog(fmt, args);

        public override void TopSettingsUI(UIHelperExtension helper)
        {
            var classes = new Dictionary<VehicleInfo.VehicleType, Dictionary<ItemClass, VehicleInfo>>();
            for (uint num = 0u; num < (ulong)PrefabCollection<VehicleInfo>.PrefabCount(); num += 1u)
            {
                VehicleInfo prefab = PrefabCollection<VehicleInfo>.GetPrefab(num);
                if (!classes.ContainsKey(prefab.m_vehicleType))
                {
                    classes[prefab.m_vehicleType] = new Dictionary<ItemClass, VehicleInfo>();
                }
                if (!classes[prefab.m_vehicleType].ContainsKey(prefab.m_class))
                {
                    classes[prefab.m_vehicleType][prefab.m_class] = prefab;
                }
            }
            foreach (var vehicleLst in classes)
            {
                foreach (var clazz in vehicleLst.Value)
                {
                    CODebugBase<LogChannel>.Warn(LogChannel.Core, string.Format("{0}\t\t\t\t\t\t\t\t\t\t\t\t\t{1},{2},{3},{4}\t\t\t\t\t\t\t\t\t\t{5}", clazz.Key, clazz.Key.m_service, clazz.Key.m_subService, clazz.Key.m_level, vehicleLst.Key, clazz.Value));
                }
            }

            UIHelperExtension group8 = helper.AddGroupExtended(Locale.Get("K45_VMC_DISTRICT_SERVICE_RESTRICTIONS"));
            group8.AddCheckboxLocale("K45_VMC_DEFAULT_ALLOW_OUTSIDERS", m_allowOutsidersAsDefault.value, (x) => { m_allowOutsidersAsDefault.value = x; });
            group8.AddCheckboxLocale("K45_VMC_DEFAULT_ALLOW_GO_OUTSIDE", m_allowGoOutsideAsDefault.value, (x) => { m_allowGoOutsideAsDefault.value = x; });
            group8.AddLabel(Locale.Get("K45_VMC_DEFAULT_RESTRICTIONS_NOTE2")).textColor = Color.white;
            group8.AddLabel(Locale.Get("K45_VMC_DEFAULT_RESTRICTIONS_NOTE")).textColor = Color.yellow;
            group8.AddLabel(Locale.Get("K45_VMC_DEFAULT_RESTRICTIONS_NOTE3")).textColor = Color.red;

        }

    }
}
