using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Extensions;
using Klyte.Harmony;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.Utils;
using Klyte.TransportLinesManager.Extensors;
using Klyte.TransportLinesManager.Extensors.BuildingAIExt;
using Klyte.TransportLinesManager.Extensors.TransportTypeExt;
using Klyte.TransportLinesManager.LineList.ExtraUI;
using Klyte.TransportLinesManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager.UI
{

    internal abstract class SVMTabControllerDistrictList<T> : UICustomControl where T : SVMSysDef<T>
    {
        private UIScrollablePanel mainPanel;

        private UIColorField m_districtColor;

        #region Awake
        private void Awake()
        {
            mainPanel = GetComponentInChildren<UIScrollablePanel>();
            SVMUtils.createUIElement(out UILabel lbl, mainPanel.transform, "Label Test", new Vector4(5, 5, 250, 40));
            lbl.text = "District > " + Singleton<T>.instance.GetSSD().ToString();
        }
        #endregion

        private void Update()
        {

        }
    }
    internal sealed class SVMTabControllerDistrictListDisCar : SVMTabControllerDistrictList<SVMSysDefDisCar> { }
    internal sealed class SVMTabControllerDistrictListDisHel : SVMTabControllerDistrictList<SVMSysDefDisHel> { }
    internal sealed class SVMTabControllerDistrictListFirCar : SVMTabControllerDistrictList<SVMSysDefFirCar> { }
    internal sealed class SVMTabControllerDistrictListFirHel : SVMTabControllerDistrictList<SVMSysDefFirHel> { }
    internal sealed class SVMTabControllerDistrictListGarCar : SVMTabControllerDistrictList<SVMSysDefGarCar> { }
    internal sealed class SVMTabControllerDistrictListHcrCar : SVMTabControllerDistrictList<SVMSysDefHcrCar> { }
    internal sealed class SVMTabControllerDistrictListHcrHel : SVMTabControllerDistrictList<SVMSysDefHcrHel> { }
    internal sealed class SVMTabControllerDistrictListPolCar : SVMTabControllerDistrictList<SVMSysDefPolCar> { }
    internal sealed class SVMTabControllerDistrictListPolHel : SVMTabControllerDistrictList<SVMSysDefPolHel> { }
    internal sealed class SVMTabControllerDistrictListRoaCar : SVMTabControllerDistrictList<SVMSysDefRoaCar> { }
    internal sealed class SVMTabControllerDistrictListWatCar : SVMTabControllerDistrictList<SVMSysDefWatCar> { }
    internal sealed class SVMTabControllerDistrictListPriCar : SVMTabControllerDistrictList<SVMSysDefPriCar> { }
    internal sealed class SVMTabControllerDistrictListDcrCar : SVMTabControllerDistrictList<SVMSysDefDcrCar> { }
    internal sealed class SVMTabControllerDistrictListTaxCar : SVMTabControllerDistrictList<SVMSysDefTaxCar> { }
    internal sealed class SVMTabControllerDistrictListCcrCcr : SVMTabControllerDistrictList<SVMSysDefCcrCcr> { }

}
