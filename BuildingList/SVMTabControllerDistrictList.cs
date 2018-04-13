using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Extensions;
using Klyte.Harmony;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.UI.ExtraUI;
using Klyte.ServiceVehiclesManager.Utils;
using Klyte.Commons.Utils;
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
        public UIScrollablePanel mainPanel { get; private set; }
        public OnButtonSelect<int> eventOnDistrictSelectionChanged;
        public OnButtonSelect<Color32> eventOnColorDistrictChanged;
        public UIHelperExtension m_uiHelper;

        private UIColorField m_districtColor;
        private SVMAssetSelectorWindowDistrictTab m_assetSelectorWindow;
        
        private bool allowColorChange;

        private static ISVMTransportTypeExtension extension => Singleton<T>.instance.GetSSD().GetTransportExtension();

        #region Awake
        private void Awake()
        {
            SVMServiceBuildingDetailPanel.eventOnDistrictSelectionChanged += onDistrictChanged;


            mainPanel = GetComponentInChildren<UIScrollablePanel>();
            mainPanel.autoLayout = false;
            m_uiHelper = new UIHelperExtension(mainPanel);

            SVMUtils.createUIElement(out UILabel lbl, mainPanel.transform, "DistrictColorLabel", new Vector4(5, 5, 250, 40));
           
            allowColorChange = SVMConfigWarehouse.allowColorChanging(extension.ConfigIndexKey);
            if (allowColorChange)
            {
                SVMUtils.LimitWidth(lbl, 250);
                lbl.autoSize = true;
                lbl.localeID = "SVM_DISTRICT_COLOR_LABEL";

                m_districtColor = KlyteUtils.CreateColorField(mainPanel);
                m_districtColor.eventSelectedColorChanged += onChangeDistrictColor;

                SVMUtils.createUIElement(out UIButton resetColor, mainPanel.transform, "DistrictColorReset", new Vector4(290, 0, 0, 0));
                SVMUtils.initButton(resetColor, false, "ButtonMenu");
                SVMUtils.LimitWidth(resetColor, 200);
                resetColor.textPadding = new RectOffset(5, 5, 5, 2);
                resetColor.autoSize = true;
                resetColor.localeID = "SVM_RESET_COLOR";
                resetColor.eventClick += onResetColor;
            }


            SVMUtils.createElement(out m_assetSelectorWindow, mainPanel.transform);
            m_assetSelectorWindow.setTabContent(this);

        }


        #endregion
        #region Actions
        private void onResetColor(UIComponent component, UIMouseEventParameter eventParam)
        {
            m_districtColor.selectedColor = Color.clear;
        }
        private void onChangeDistrictColor(UIComponent component, Color value)
        {
            SVMUtils.doLog("onChangeDistrictColor");
            if (!getCurrentSelectedId(out int currentDistrict)) return;
            extension.SetColorDistrict((uint)currentDistrict, value);
            eventOnColorDistrictChanged?.Invoke(value);
        }
        #endregion

        private static bool getCurrentSelectedId(out int currentDistrict)
        {
            currentDistrict = SVMServiceBuildingDetailPanel.instance.getCurrentSelectedDistrictId();
            return currentDistrict >= 0;
        }

        #region On Selection Changed
        private void onDistrictChanged()
        {
            if (getCurrentSelectedId(out int currentDistrict))
            {
                mainPanel.isVisible = true;
                if (m_districtColor != null)
                {
                    m_districtColor.selectedColor = Singleton<T>.instance.GetSSD().GetTransportExtension().GetColorDistrict((uint)currentDistrict);
                }
                eventOnDistrictSelectionChanged?.Invoke(currentDistrict);
            }
            else
            {
                mainPanel.isVisible = false;
            }
        }
        #endregion

        private void Start()
        {
        }

        private void Update()
        {
            if (m_districtColor != null && mainPanel.isVisible)
            {
                m_districtColor.area = new Vector4(260, 5, 20, 20);
            }
        }

    }
    internal sealed class SVMTabControllerDistrictListDisCar : SVMTabControllerDistrictList<SVMSysDefDisCar> { }
    internal sealed class SVMTabControllerDistrictListDisHel : SVMTabControllerDistrictList<SVMSysDefDisHel> { }
    internal sealed class SVMTabControllerDistrictListFirCar : SVMTabControllerDistrictList<SVMSysDefFirCar> { }
    internal sealed class SVMTabControllerDistrictListFirHel : SVMTabControllerDistrictList<SVMSysDefFirHel> { }
    internal sealed class SVMTabControllerDistrictListGarCar : SVMTabControllerDistrictList<SVMSysDefGarCar> { }
    internal sealed class SVMTabControllerDistrictListGbcCar : SVMTabControllerDistrictList<SVMSysDefGbcCar> { }
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
    internal sealed class SVMTabControllerDistrictListSnwCar : SVMTabControllerDistrictList<SVMSysDefSnwCar> { }
    internal sealed class SVMTabControllerDistrictListRegTra : SVMTabControllerDistrictList<SVMSysDefRegTra> { }
    internal sealed class SVMTabControllerDistrictListRegShp : SVMTabControllerDistrictList<SVMSysDefRegShp> { }
    internal sealed class SVMTabControllerDistrictListRegPln : SVMTabControllerDistrictList<SVMSysDefRegPln> { }
    internal sealed class SVMTabControllerDistrictListCrgTra : SVMTabControllerDistrictList<SVMSysDefCrgTra> { }
    internal sealed class SVMTabControllerDistrictListCrgShp : SVMTabControllerDistrictList<SVMSysDefCrgShp> { }

}
