using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Klyte.Commons.Extensors;
using Klyte.Commons.Utils;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.UI.ExtraUI;
using Klyte.ServiceVehiclesManager.Utils;
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
        private UICheckBox m_districtAllowOutsiders;
        private UICheckBox m_districtAllowGoOutside;
        private UIButton m_resetValues;
        private SVMAssetSelectorWindowDistrictTab m_assetSelectorWindow;

        private bool allowColorChange;
        private bool isLoading = true;

        private static ISVMTransportTypeExtension extension => Singleton<T>.instance.GetSSD().GetTransportExtension();

        #region Awake
        private void Awake()
        {
            SVMTabPanel.eventOnDistrictSelectionChanged += onDistrictChanged;


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
            if (extension.GetAllowDistrictServiceRestrictions())
            {
                m_districtAllowOutsiders = m_uiHelper.AddCheckboxLocale("SVM_ALLOW_OUTSIDERS", true, (x) =>
                {
                    if (!getCurrentSelectedId(out int currentDistrict) || isLoading) return;
                    extension.SetAllowOutsiders((uint)currentDistrict, x);
                    m_districtAllowOutsiders.GetComponentInChildren<UILabel>().textColor = Color.white;
                });
                m_districtAllowGoOutside = m_uiHelper.AddCheckboxLocale("SVM_ALLOW_GO_OUTSIDE", true, (x) =>
                {
                    if (!getCurrentSelectedId(out int currentDistrict) || isLoading) return;
                    extension.SetAllowGoOutside((uint)currentDistrict, x);
                    m_districtAllowGoOutside.GetComponentInChildren<UILabel>().textColor = Color.white;
                });

                m_resetValues = (UIButton)m_uiHelper.AddButton(Locale.Get("SVM_RESET_VALUE_CITY_DEFAULT"), () =>
                {
                    if (!getCurrentSelectedId(out int currentDistrict) || isLoading) return;
                    extension.CleanAllowGoOutside((uint)currentDistrict);
                    extension.CleanAllowOutsiders((uint)currentDistrict);
                    onDistrictChanged();
                });

                m_districtAllowOutsiders.relativePosition = new Vector2(0, 30);
                m_districtAllowGoOutside.relativePosition = new Vector2(0, 60);
                m_resetValues.relativePosition = new Vector2(0, 90);
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
            currentDistrict = SVMTabPanel.instance.getCurrentSelectedDistrictId();
            return currentDistrict >= 0;
        }

        #region On Selection Changed
        private void onDistrictChanged()
        {
            if (getCurrentSelectedId(out int currentDistrict))
            {
                isLoading = true;
                mainPanel.isVisible = true;
                if (m_districtColor != null)
                {
                    m_districtColor.selectedColor = Singleton<T>.instance.GetSSD().GetTransportExtension().GetColorDistrict((uint)currentDistrict);
                }
                if (m_districtAllowOutsiders != null && m_districtAllowGoOutside != null)
                {
                    m_districtAllowOutsiders.isChecked = extension.GetAllowOutsiders((uint)currentDistrict) ?? extension.GetAllowOutsidersEffective((uint)currentDistrict);
                    m_districtAllowOutsiders.GetComponentInChildren<UILabel>().textColor = extension.GetAllowOutsiders((uint)currentDistrict) == null ? Color.yellow : Color.white;
                    m_districtAllowGoOutside.isChecked = extension.GetAllowGoOutside((uint)currentDistrict) ?? extension.GetAllowGoOutsideEffective((uint)currentDistrict);
                    m_districtAllowGoOutside.GetComponentInChildren<UILabel>().textColor = extension.GetAllowGoOutside((uint)currentDistrict) == null ? Color.yellow : Color.white;
                }
                eventOnDistrictSelectionChanged?.Invoke(currentDistrict);
                isLoading = false;
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
    internal sealed class SVMTabControllerDistrictListBeaCar : SVMTabControllerDistrictList<SVMSysDefBeaCar> { }
    internal sealed class SVMTabControllerDistrictListPstCar : SVMTabControllerDistrictList<SVMSysDefPstCar> { }
    internal sealed class SVMTabControllerDistrictListPstTrk : SVMTabControllerDistrictList<SVMSysDefPstTrk> { }

}
