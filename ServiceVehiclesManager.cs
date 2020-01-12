using ColossalFramework;
using ColossalFramework.Globalization;
using Klyte.Commons.Extensors;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using Klyte.ServiceVehiclesManager.UI;
using Klyte.ServiceVehiclesManager.Utils;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyVersion("2.99.99.99")]

namespace Klyte.ServiceVehiclesManager
{
    public class ServiceVehiclesManagerMod : BasicIUserMod<ServiceVehiclesManagerMod, SVMController, SVMTabPanel>
    {

        public ServiceVehiclesManagerMod() => Construct();


        private SavedBool m_allowOutsidersAsDefault = new SavedBool("SVMAllowGoOutsideAsDefault", Settings.gameSettingsFile, true, true);
        private SavedBool m_allowGoOutsideAsDefault = new SavedBool("SVMAllowOutsidersAsDefault", Settings.gameSettingsFile, true, true);

        public static bool allowOutsidersAsDefault => Instance.m_allowOutsidersAsDefault.value;
        public static bool allowServeOtherDistrictsAsDefault => Instance.m_allowGoOutsideAsDefault.value;

        public override string SimpleName => "Service Vehicles Manager";
        public override string Description => "Extension for managing the service vehicles. Requires Klyte Commons.";

        public override string IconName => "K45_SVMIcon";

        public override void DoErrorLog(string fmt, params object[] args) => LogUtils.DoErrorLog(fmt, args);

        public override void DoLog(string fmt, params object[] args) => LogUtils.DoLog(fmt, args);

        public override void LoadSettings()
        {
        }
        public override void TopSettingsUI(UIHelperExtension helper)
        {
            UIHelperExtension group8 = helper.AddGroupExtended(Locale.Get("K45_SVM_DISTRICT_SERVICE_RESTRICTIONS"));
            group8.AddCheckboxLocale("K45_SVM_DEFAULT_ALLOW_OUTSIDERS", m_allowOutsidersAsDefault.value, (x) => { m_allowOutsidersAsDefault.value = x; });
            group8.AddCheckboxLocale("K45_SVM_DEFAULT_ALLOW_GO_OUTSIDE", m_allowGoOutsideAsDefault.value, (x) => { m_allowGoOutsideAsDefault.value = x; });
            group8.AddLabel(Locale.Get("K45_SVM_DEFAULT_RESTRICTIONS_NOTE2")).textColor = Color.white;
            group8.AddLabel(Locale.Get("K45_SVM_DEFAULT_RESTRICTIONS_NOTE")).textColor = Color.yellow;
            group8.AddLabel(Locale.Get("K45_SVM_DEFAULT_RESTRICTIONS_NOTE3")).textColor = Color.red;

        }

    }
}
