using ColossalFramework;
using ColossalFramework.DataBinding;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Commons.Extensors;
using Klyte.Commons.Interfaces;
using Klyte.ServiceVehiclesManager.i18n;
using Klyte.ServiceVehiclesManager.Utils;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyVersion("2.0.0.9999")]

namespace Klyte.ServiceVehiclesManager
{
    public class ServiceVehiclesManagerMod : BasicIUserMod<ServiceVehiclesManagerMod, SVMLocaleUtils, SVMResourceLoader>
    {

        public ServiceVehiclesManagerMod()
        {
            Construct();
        }

        private SavedBool m_allowOutsidersAsDefault = new SavedBool("SVMAllowGoOutsideAsDefault", Settings.gameSettingsFile, true, true);
        private SavedBool m_allowGoOutsideAsDefault = new SavedBool("SVMAllowOutsidersAsDefault", Settings.gameSettingsFile, true, true);
        public static bool allowOutsidersAsDefault => instance.m_allowOutsidersAsDefault.value;
        public static bool allowGoOutsideAsDefault => instance.m_allowGoOutsideAsDefault.value;

        public override string SimpleName => "Service Vehicles Manager";
        public override string Description => "Extension for managing the service vehicles. Requires Klyte Commons.";

        public override void doErrorLog(string fmt, params object[] args)
        {
            SVMUtils.doErrorLog(fmt, args);
        }

        public override void doLog(string fmt, params object[] args)
        {
            SVMUtils.doLog(fmt, args);
        }

        public override void LoadSettings()
        {
        }

        public override void OnCreated(ILoading loading)
        {
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            SVMUtils.doLog("LEVEL LOAD");
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame && mode != LoadMode.NewGameFromScenario)
            {
                SVMUtils.doLog("NOT GAME ({0})", mode);
                return;
            }

            SVMController.Ensure();
        }

        public override void OnLevelUnloading()
        {
        }

        public override void OnReleased()
        {
        }

        public override void TopSettingsUI(UIHelperExtension helper)
        {
            UIHelperExtension group8 = helper.AddGroupExtended(Locale.Get("SVM_DISTRICT_SERVICE_RESTRICTIONS"));
            group8.AddCheckboxLocale("SVM_DEFAULT_ALLOW_OUTSIDERS", m_allowOutsidersAsDefault.value, (x) => { m_allowOutsidersAsDefault.value = x; });
            group8.AddCheckboxLocale("SVM_DEFAULT_ALLOW_GO_OUTSIDE", m_allowGoOutsideAsDefault.value, (x) => { m_allowGoOutsideAsDefault.value = x; });
            group8.AddLabel(Locale.Get("SVM_DEFAULT_RESTRICTIONS_NOTE2")).textColor = Color.white;
            group8.AddLabel(Locale.Get("SVM_DEFAULT_RESTRICTIONS_NOTE")).textColor = Color.yellow;
            group8.AddLabel(Locale.Get("SVM_DEFAULT_RESTRICTIONS_NOTE3")).textColor = Color.red;

        }
    }
}
