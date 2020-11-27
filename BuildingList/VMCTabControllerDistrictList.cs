using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Klyte.Commons.Extensors;
using Klyte.Commons.Utils;
using Klyte.VehiclesMasterControl.Extensors.VehicleExt;
using Klyte.VehiclesMasterControl.UI.ExtraUI;
using UnityEngine;

namespace Klyte.VehiclesMasterControl.UI
{

    internal abstract class VMCTabControllerDistrictList<T> : UICustomControl where T : VMCSysDef<T>, new()
    {
        public UIScrollablePanel mainPanel { get; private set; }
        public OnButtonSelect<int> eventOnDistrictSelectionChanged;
        public OnButtonSelect<Color32> eventOnColorDistrictChanged;
        public UIHelperExtension m_uiHelper;

        private UIColorField m_districtColor;
        private VMCAssetSelectorWindowDistrictTab m_assetSelectorWindow;

        private bool allowColorChange;
        private bool isLoading = true;

        #region Awake
        private void Awake()
        {
            VMCTabPanel.eventOnDistrictSelectionChanged += onDistrictChanged;


            mainPanel = GetComponentInChildren<UIScrollablePanel>();
            mainPanel.autoLayout = false;
            m_uiHelper = new UIHelperExtension(mainPanel);

            KlyteMonoUtils.CreateUIElement(out UILabel lbl, mainPanel.transform, "DistrictColorLabel", new Vector4(5, 5, 250, 40));

            allowColorChange = SingletonLite<T>.instance.GetSSD().AllowColorChanging();
            if (allowColorChange)
            {
                KlyteMonoUtils.LimitWidth(lbl, 250);
                lbl.autoSize = true;
                lbl.localeID = "K45_VMC_DISTRICT_COLOR_LABEL";

                m_districtColor = KlyteMonoUtils.CreateColorField(mainPanel);
                m_districtColor.eventSelectedColorChanged += onChangeDistrictColor;

                KlyteMonoUtils.CreateUIElement(out UIButton resetColor, mainPanel.transform, "DistrictColorReset", new Vector4(290, 0, 0, 0));
                KlyteMonoUtils.InitButton(resetColor, false, "ButtonMenu");
                KlyteMonoUtils.LimitWidth(resetColor, 200);
                resetColor.textPadding = new RectOffset(5, 5, 5, 2);
                resetColor.autoSize = true;
                resetColor.localeID = "K45_VMC_RESET_COLOR";
                resetColor.eventClick += onResetColor;
            }
            ServiceSystemDefinition ssd = SingletonLite<T>.instance.GetSSD();
            IVMCDistrictExtension extension = SingletonLite<T>.instance.GetExtensionDistrict();

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

            SingletonLite<T>.instance.GetExtensionDistrict().SetColor((uint)currentDistrict, value);
            eventOnColorDistrictChanged?.Invoke(value);
        }
        #endregion

        private static bool getCurrentSelectedId(out int currentDistrict)
        {
            currentDistrict = VMCTabPanel.Instance.getCurrentSelectedDistrictId();
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
                    m_districtColor.selectedColor = SingletonLite<T>.instance.GetSSD().GetDistrictExtension().GetColor((uint)currentDistrict);
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
    internal sealed class VMCTabControllerDistrictListDisCar : VMCTabControllerDistrictList<VMCSysDefDisCar> { }
    internal sealed class VMCTabControllerDistrictListDisHel : VMCTabControllerDistrictList<VMCSysDefDisHel> { }
    internal sealed class VMCTabControllerDistrictListFirCar : VMCTabControllerDistrictList<VMCSysDefFirCar> { }
    internal sealed class VMCTabControllerDistrictListFirHel : VMCTabControllerDistrictList<VMCSysDefFirHel> { }
    internal sealed class VMCTabControllerDistrictListGarCar : VMCTabControllerDistrictList<VMCSysDefGarCar> { }
    internal sealed class VMCTabControllerDistrictListGbcCar : VMCTabControllerDistrictList<VMCSysDefGbcCar> { }
    internal sealed class VMCTabControllerDistrictListHcrCar : VMCTabControllerDistrictList<VMCSysDefHcrCar> { }
    internal sealed class VMCTabControllerDistrictListHcrHel : VMCTabControllerDistrictList<VMCSysDefHcrHel> { }
    internal sealed class VMCTabControllerDistrictListPolCar : VMCTabControllerDistrictList<VMCSysDefPolCar> { }
    internal sealed class VMCTabControllerDistrictListPolHel : VMCTabControllerDistrictList<VMCSysDefPolHel> { }
    internal sealed class VMCTabControllerDistrictListRoaCar : VMCTabControllerDistrictList<VMCSysDefRoaCar> { }
    internal sealed class VMCTabControllerDistrictListWatCar : VMCTabControllerDistrictList<VMCSysDefWatCar> { }
    internal sealed class VMCTabControllerDistrictListPriCar : VMCTabControllerDistrictList<VMCSysDefPriCar> { }
    internal sealed class VMCTabControllerDistrictListDcrCar : VMCTabControllerDistrictList<VMCSysDefDcrCar> { }
    internal sealed class VMCTabControllerDistrictListTaxCar : VMCTabControllerDistrictList<VMCSysDefTaxCar> { }
    internal sealed class VMCTabControllerDistrictListCcrCcr : VMCTabControllerDistrictList<VMCSysDefCcrCcr> { }
    internal sealed class VMCTabControllerDistrictListSnwCar : VMCTabControllerDistrictList<VMCSysDefSnwCar> { }
    internal sealed class VMCTabControllerDistrictListRegTra : VMCTabControllerDistrictList<VMCSysDefRegTra> { }
    internal sealed class VMCTabControllerDistrictListRegShp : VMCTabControllerDistrictList<VMCSysDefRegShp> { }
    internal sealed class VMCTabControllerDistrictListRegPln : VMCTabControllerDistrictList<VMCSysDefRegPln> { }
    internal sealed class VMCTabControllerDistrictListCrgTra : VMCTabControllerDistrictList<VMCSysDefCrgTra> { }
    internal sealed class VMCTabControllerDistrictListCrgShp : VMCTabControllerDistrictList<VMCSysDefCrgShp> { }
    internal sealed class VMCTabControllerDistrictListBeaCar : VMCTabControllerDistrictList<VMCSysDefBeaCar> { }
    internal sealed class VMCTabControllerDistrictListPstCar : VMCTabControllerDistrictList<VMCSysDefPstCar> { }
    internal sealed class VMCTabControllerDistrictListPstTrk : VMCTabControllerDistrictList<VMCSysDefPstTrk> { }
    internal sealed class VMCTabControllerDistrictListOutBus : VMCTabControllerDistrictList<VMCSysDefOutBus> { }
    internal sealed class VMCTabControllerDistrictListTouBal : VMCTabControllerDistrictList<VMCSysDefTouBal> { }
    internal sealed class VMCTabControllerDistrictListClbPln : VMCTabControllerDistrictList<VMCSysDefClbPln> { }
    internal sealed class VMCTabControllerDistrictListAdtBcc : VMCTabControllerDistrictList<VMCSysDefAdtBcc> { }
    internal sealed class VMCTabControllerDistrictListChdBcc : VMCTabControllerDistrictList<VMCSysDefChdBcc> { }
    internal sealed class VMCTabControllerDistrictListCrgPln : VMCTabControllerDistrictList<VMCSysDefCrgPln> { }
    internal sealed class VMCTabControllerDistrictListFshTrk : VMCTabControllerDistrictList<VMCSysDefFshTrk> { }
    internal sealed class VMCTabControllerDistrictListFshGen : VMCTabControllerDistrictList<VMCSysDefFshGen> { }
    internal sealed class VMCTabControllerDistrictListFshSlm : VMCTabControllerDistrictList<VMCSysDefFshSlm> { }
    internal sealed class VMCTabControllerDistrictListFshShf : VMCTabControllerDistrictList<VMCSysDefFshShf> { }
    internal sealed class VMCTabControllerDistrictListFshTna : VMCTabControllerDistrictList<VMCSysDefFshTna> { }
    internal sealed class VMCTabControllerDistrictListFshAch : VMCTabControllerDistrictList<VMCSysDefFshAch> { }
    internal sealed class VMCTabControllerDistrictListIfmTrl : VMCTabControllerDistrictList<VMCSysDefIfmTrl> { }
    internal sealed class VMCTabControllerDistrictListIndTrk : VMCTabControllerDistrictList<VMCSysDefIndTrk> { }
    internal sealed class VMCTabControllerDistrictListIndVan : VMCTabControllerDistrictList<VMCSysDefIndVan> { }
    internal sealed class VMCTabControllerDistrictListIfmTrk : VMCTabControllerDistrictList<VMCSysDefIfmTrk> { }
    internal sealed class VMCTabControllerDistrictListIfrTrk : VMCTabControllerDistrictList<VMCSysDefIfrTrk> { }
    internal sealed class VMCTabControllerDistrictListIgnTrk : VMCTabControllerDistrictList<VMCSysDefIgnTrk> { }
    internal sealed class VMCTabControllerDistrictListIolTrk : VMCTabControllerDistrictList<VMCSysDefIolTrk> { }
    internal sealed class VMCTabControllerDistrictListIorTrk : VMCTabControllerDistrictList<VMCSysDefIorTrk> { }
    internal sealed class VMCTabControllerDistrictListWstCol : VMCTabControllerDistrictList<VMCSysDefWstCol> { }
    internal sealed class VMCTabControllerDistrictListWstTrn : VMCTabControllerDistrictList<VMCSysDefWstTrn> { }

}
