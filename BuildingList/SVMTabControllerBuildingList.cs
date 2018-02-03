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

    internal abstract class SVMTabControllerBuildingList<T> : UICustomControl where T : SVMSysDef<T>
    {
        private UIScrollablePanel mainPanel;

        #region Awake
        private void Awake()
        {
            mainPanel = GetComponentInChildren<UIScrollablePanel>();
            SVMUtils.createUIElement(out UILabel lbl, mainPanel.transform, "Label Test", new Vector4(5, 5, 250, 40));
            lbl.text = Singleton<T>.instance.GetSSD().ToString();
        }
        #endregion

        private void Update()
        {

        }
    }
    internal sealed class SVMTabControllerBuildingListDisCar : SVMTabControllerBuildingList<SVMSysDefDisCar> { }
    internal sealed class SVMTabControllerBuildingListDisHel : SVMTabControllerBuildingList<SVMSysDefDisHel> { }
    internal sealed class SVMTabControllerBuildingListFirCar : SVMTabControllerBuildingList<SVMSysDefFirCar> { }
    internal sealed class SVMTabControllerBuildingListFirHel : SVMTabControllerBuildingList<SVMSysDefFirHel> { }
    internal sealed class SVMTabControllerBuildingListGarCar : SVMTabControllerBuildingList<SVMSysDefGarCar> { }
    internal sealed class SVMTabControllerBuildingListHcrCar : SVMTabControllerBuildingList<SVMSysDefHcrCar> { }
    internal sealed class SVMTabControllerBuildingListHcrHel : SVMTabControllerBuildingList<SVMSysDefHcrHel> { }
    internal sealed class SVMTabControllerBuildingListPolCar : SVMTabControllerBuildingList<SVMSysDefPolCar> { }
    internal sealed class SVMTabControllerBuildingListPolHel : SVMTabControllerBuildingList<SVMSysDefPolHel> { }
    internal sealed class SVMTabControllerBuildingListRoaCar : SVMTabControllerBuildingList<SVMSysDefRoaCar> { }
    internal sealed class SVMTabControllerBuildingListWatCar : SVMTabControllerBuildingList<SVMSysDefWatCar> { }
    internal sealed class SVMTabControllerBuildingListPriCar : SVMTabControllerBuildingList<SVMSysDefPriCar> { }
    internal sealed class SVMTabControllerBuildingListDcrCar : SVMTabControllerBuildingList<SVMSysDefDcrCar> { }
    internal sealed class SVMTabControllerBuildingListTaxCar : SVMTabControllerBuildingList<SVMSysDefTaxCar> { }
    internal sealed class SVMTabControllerBuildingListCcrCcr : SVMTabControllerBuildingList<SVMSysDefCcrCcr> { }
    internal sealed class SVMTabControllerBuildingListSnwCar : SVMTabControllerBuildingList<SVMSysDefSnwCar> { }

}
