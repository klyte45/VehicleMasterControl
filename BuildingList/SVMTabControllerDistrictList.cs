using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Klyte.Commons.Extensors;
using Klyte.Commons.Utils;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.UI.ExtraUI;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager.UI
{

    internal abstract class SVMTabControllerDistrictList<T> : UICustomControl where T : SVMSysDef<T>, new()
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

        #region Awake
        private void Awake()
        {
            SVMTabPanel.eventOnDistrictSelectionChanged += onDistrictChanged;


            mainPanel = GetComponentInChildren<UIScrollablePanel>();
            mainPanel.autoLayout = false;
            m_uiHelper = new UIHelperExtension(mainPanel);

            KlyteMonoUtils.CreateUIElement(out UILabel lbl, mainPanel.transform, "DistrictColorLabel", new Vector4(5, 5, 250, 40));

            allowColorChange = SingletonLite<T>.instance.GetSSD().AllowColorChanging();
            if (allowColorChange)
            {
                KlyteMonoUtils.LimitWidth(lbl, 250);
                lbl.autoSize = true;
                lbl.localeID = "K45_SVM_DISTRICT_COLOR_LABEL";

                m_districtColor = KlyteMonoUtils.CreateColorField(mainPanel);
                m_districtColor.eventSelectedColorChanged += onChangeDistrictColor;

                KlyteMonoUtils.CreateUIElement(out UIButton resetColor, mainPanel.transform, "DistrictColorReset", new Vector4(290, 0, 0, 0));
                KlyteMonoUtils.InitButton(resetColor, false, "ButtonMenu");
                KlyteMonoUtils.LimitWidth(resetColor, 200);
                resetColor.textPadding = new RectOffset(5, 5, 5, 2);
                resetColor.autoSize = true;
                resetColor.localeID = "K45_SVM_RESET_COLOR";
                resetColor.eventClick += onResetColor;
            }
            ServiceSystemDefinition ssd = SingletonLite<T>.instance.GetSSD();
            ISVMDistrictExtension extension = SingletonLite<T>.instance.GetExtensionDistrict();
            if (ssd.AllowDistrictServiceRestrictions)
            {
                m_districtAllowOutsiders = m_uiHelper.AddCheckboxLocale("K45_SVM_ALLOW_OUTSIDERS", true, (x) =>
                {
                    if (!getCurrentSelectedId(out int currentDistrict) || isLoading)
                    {
                        return;
                    }

                    extension.SetAllowOutsiders((uint) currentDistrict, x);
                    m_districtAllowOutsiders.GetComponentInChildren<UILabel>().textColor = Color.white;
                });
                m_districtAllowGoOutside = m_uiHelper.AddCheckboxLocale("K45_SVM_ALLOW_GO_OUTSIDE", true, (x) =>
                {
                    if (!getCurrentSelectedId(out int currentDistrict) || isLoading)
                    {
                        return;
                    }

                    extension.SetAllowServeOtherDistricts((uint) currentDistrict, x);
                    m_districtAllowGoOutside.GetComponentInChildren<UILabel>().textColor = Color.white;
                });

                m_resetValues = (UIButton) m_uiHelper.AddButton(Locale.Get("K45_SVM_RESET_VALUE_CITY_DEFAULT"), () =>
                 {
                     if (!getCurrentSelectedId(out int currentDistrict) || isLoading)
                     {
                         return;
                     }

                     extension.ClearServeOtherDistricts((uint) currentDistrict);
                     extension.ClearAllowOutsiders((uint) currentDistrict);
                     onDistrictChanged();
                 });

                m_districtAllowOutsiders.relativePosition = new Vector2(0, 30);
                m_districtAllowGoOutside.relativePosition = new Vector2(0, 60);
                m_resetValues.relativePosition = new Vector2(0, 90);
            }
            KlyteMonoUtils.CreateElement(out m_assetSelectorWindow, mainPanel.transform);
            m_assetSelectorWindow.setTabContent(this);

        }


        #endregion
        #region Actions
        private void onResetColor(UIComponent component, UIMouseEventParameter eventParam) => m_districtColor.selectedColor = Color.clear;
        private void onChangeDistrictColor(UIComponent component, Color value)
        {
            LogUtils.DoLog("onChangeDistrictColor");
            if (!getCurrentSelectedId(out int currentDistrict))
            {
                return;
            }

            SingletonLite<T>.instance.GetExtensionDistrict().SetColor((uint) currentDistrict, value);
            eventOnColorDistrictChanged?.Invoke(value);
        }
        #endregion

        private static bool getCurrentSelectedId(out int currentDistrict)
        {
            currentDistrict = SVMTabPanel.Instance.getCurrentSelectedDistrictId();
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
                    m_districtColor.selectedColor = SingletonLite<T>.instance.GetSSD().GetDistrictExtension().GetColor((uint) currentDistrict);
                }
                if (m_districtAllowOutsiders != null && m_districtAllowGoOutside != null)
                {
                    m_districtAllowOutsiders.isChecked = SingletonLite<T>.instance.GetExtensionDistrict().GetAllowOutsiders((uint) currentDistrict) ?? ServiceVehiclesManagerMod.allowOutsidersAsDefault;
                    m_districtAllowOutsiders.GetComponentInChildren<UILabel>().textColor = SingletonLite<T>.instance.GetExtensionDistrict().GetAllowOutsiders((uint) currentDistrict) == null ? Color.yellow : Color.white;
                    m_districtAllowGoOutside.isChecked = SingletonLite<T>.instance.GetExtensionDistrict().GetAllowServeOtherDistricts((uint) currentDistrict) ?? ServiceVehiclesManagerMod.allowServeOtherDistrictsAsDefault;
                    m_districtAllowGoOutside.GetComponentInChildren<UILabel>().textColor = SingletonLite<T>.instance.GetExtensionDistrict().GetAllowServeOtherDistricts((uint) currentDistrict) == null ? Color.yellow : Color.white;
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
