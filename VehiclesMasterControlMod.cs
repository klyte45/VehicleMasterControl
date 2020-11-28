using ColossalFramework;
using ColossalFramework.Globalization;
using Klyte.Commons.Extensors;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using Klyte.VehiclesMasterControl.UI;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyVersion("1.0.0.1")]

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

        }

    }
}
