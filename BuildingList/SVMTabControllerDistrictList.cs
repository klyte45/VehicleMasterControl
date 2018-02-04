using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Extensions;
using Klyte.Harmony;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.UI.ExtraUI;
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
        public UIScrollablePanel mainPanel { get; private set; }
        public OnButtonSelect<int> eventOnDistrictSelectionChanged;
        public OnButtonSelect<Color32> eventOnColorDistrictChanged;
        public UIHelperExtension m_uiHelper;

        private UIColorField m_districtColor;
        private SVMAssetSelectorWindowDistrictTab m_assetSelectorWindow;

        private UISlider[] m_budgetSliders = new UISlider[8];
        private UIButton m_enableBudgetPerHour;
        private UIButton m_disableBudgetPerHour;
        private UILabel m_lineBudgetSlidersTitle;

        private bool allowColorChange;

        private static ISVMTransportTypeExtension extension => Singleton<T>.instance.GetSSD().GetTransportExtension();

        #region Awake
        private void Awake()
        {
            SVMServiceBuildingDetailPanel.eventOnDistrictSelectionChanged += onDistrictChanged;

            allowColorChange = SVMConfigWarehouse.allowColorChanging(extension.ConfigIndexKey);

            mainPanel = GetComponentInChildren<UIScrollablePanel>();
            mainPanel.autoLayout = false;
            m_uiHelper = new UIHelperExtension(mainPanel);

            SVMUtils.createUIElement(out UILabel lbl, mainPanel.transform, "DistrictColorLabel", new Vector4(5, 5, 250, 40));
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

            CreateBudgetSliders();
            CreateBudgetToggleButtons();

            SVMUtils.createElement(out m_assetSelectorWindow, mainPanel.transform);
            m_assetSelectorWindow.setTabContent(this);

        }

        private void CreateBudgetSliders()
        {
            SVMUtils.createUIElement(out m_lineBudgetSlidersTitle, mainPanel.transform);
            m_lineBudgetSlidersTitle.autoSize = false;
            m_lineBudgetSlidersTitle.relativePosition = new Vector3(15f, 30f);
            m_lineBudgetSlidersTitle.width = 400f;
            m_lineBudgetSlidersTitle.height = 36f;
            m_lineBudgetSlidersTitle.textScale = 0.9f;
            m_lineBudgetSlidersTitle.textAlignment = UIHorizontalAlignment.Center;
            m_lineBudgetSlidersTitle.name = "LineBudgetSlidersTitle";
            m_lineBudgetSlidersTitle.font = UIHelperExtension.defaultFontCheckbox;
            m_lineBudgetSlidersTitle.wordWrap = true;
            m_lineBudgetSlidersTitle.localeID = "SVM_BUDGET_SLIDERS_DISTRICT_TITLE";

            for (int i = 0; i < m_budgetSliders.Length; i++)
            {
                m_budgetSliders[i] = GenerateVerticalBudgetMultiplierField(m_uiHelper, i);
            }
        }
        private void CreateBudgetToggleButtons()
        {
            SVMUtils.createUIElement(out m_enableBudgetPerHour, mainPanel.transform);
            m_enableBudgetPerHour.relativePosition = new Vector3(400, 70);
            m_enableBudgetPerHour.textScale = 0.6f;
            m_enableBudgetPerHour.width = 40;
            m_enableBudgetPerHour.height = 40;
            m_enableBudgetPerHour.tooltip = Locale.Get("SVM_USE_PER_PERIOD_BUDGET");
            SVMUtils.initButton(m_enableBudgetPerHour, true, "ButtonMenu");
            m_enableBudgetPerHour.name = "EnableBudgetPerHour";
            m_enableBudgetPerHour.isVisible = true;
            m_enableBudgetPerHour.eventClick += (component, eventParam) =>
            {
                if (!getCurrentSelectedId(out int currentDistrict)) return;
                uint[] multipliers = extension.GetBudgetsMultiplierDistrict((uint)currentDistrict);
                uint[] newSaveData = new uint[8];
                for (int i = 0; i < 8; i++)
                {
                    newSaveData[i] = multipliers[0];
                }
                extension.SetBudgetMultiplierDistrict((uint)currentDistrict, newSaveData);

                updateSliders();
            };

            UISprite icon = m_enableBudgetPerHour.AddUIComponent<UISprite>();
            icon.relativePosition = new Vector3(2, 2);
            icon.atlas = SVMController.taSVM;
            icon.width = 36;
            icon.height = 36;
            icon.spriteName = "PerHourIcon";


            SVMUtils.createUIElement(out m_disableBudgetPerHour, mainPanel.transform);
            m_disableBudgetPerHour.relativePosition = new Vector3(400, 70);
            m_disableBudgetPerHour.textScale = 0.6f;
            m_disableBudgetPerHour.width = 40;
            m_disableBudgetPerHour.height = 40;
            m_disableBudgetPerHour.tooltip = Locale.Get("SVM_USE_SINGLE_BUDGET");
            SVMUtils.initButton(m_disableBudgetPerHour, true, "ButtonMenu");
            m_disableBudgetPerHour.name = "DisableBudgetPerHour";
            m_disableBudgetPerHour.isVisible = true;
            m_disableBudgetPerHour.eventClick += (component, eventParam) =>
            {
                if (!getCurrentSelectedId(out int currentDistrict)) return;
                uint[] multipliers = extension.GetBudgetsMultiplierDistrict((uint)currentDistrict);
                uint[] newSaveData = new uint[] { multipliers[0] };
                extension.SetBudgetMultiplierDistrict((uint)currentDistrict, newSaveData);

                updateSliders();
            };

            icon = m_disableBudgetPerHour.AddUIComponent<UISprite>();
            icon.relativePosition = new Vector3(2, 2);
            icon.atlas = SVMController.taSVM;
            icon.width = 36;
            icon.height = 36;
            icon.spriteName = "24hLineIcon";
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
            currentDistrict = SVMServiceBuildingDetailPanel.Get().getCurrentSelectedDistrictId();
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
                updateSliders();
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
            updateSliders();
        }

        private void Update()
        {
            if (m_districtColor != null && mainPanel.isVisible)
            {
                m_districtColor.area = new Vector4(260, 5, 20, 20);
            }
        }

        #region Budget
        private void setBudgetHour(float x, int selectedHourIndex)
        {
            if (!getCurrentSelectedId(out int currentDistrict)) return;
            ushort val = (ushort)(x * 100 + 0.5f);
            uint[] saveData;
            saveData = extension.GetBudgetsMultiplierDistrict((uint)currentDistrict);

            if (selectedHourIndex >= saveData.Length || saveData[selectedHourIndex] == val)
            {
                return;
            }

            saveData[selectedHourIndex] = val;
            extension.SetBudgetMultiplierDistrict((uint)currentDistrict, saveData);
        }

        private UISlider GenerateVerticalBudgetMultiplierField(UIHelperExtension uiHelper, int idx)
        {
            UISlider bugdetSlider = (UISlider)uiHelper.AddSlider(Locale.Get("SVM_BUDGET_MULTIPLIER_LABEL"), 0f, 7.5f, 0.05f, -1,
                (x) =>
                {

                });
            UILabel budgetSliderLabel = bugdetSlider.transform.parent.GetComponentInChildren<UILabel>();
            UIPanel budgetSliderPanel = bugdetSlider.GetComponentInParent<UIPanel>();

            budgetSliderPanel.relativePosition = new Vector2(45 * idx + 15, 50);
            budgetSliderPanel.width = 40;
            budgetSliderPanel.height = 160;
            bugdetSlider.zOrder = 0;
            budgetSliderPanel.autoLayout = true;

            bugdetSlider.size = new Vector2(40, 100);
            bugdetSlider.scrollWheelAmount = 0;
            bugdetSlider.orientation = UIOrientation.Vertical;
            bugdetSlider.clipChildren = true;
            bugdetSlider.thumbOffset = new Vector2(0, -100);
            bugdetSlider.color = Color.black;

            bugdetSlider.thumbObject.width = 40;
            bugdetSlider.thumbObject.height = 200;
            ((UISprite)bugdetSlider.thumbObject).spriteName = "ScrollbarThumb";
            ((UISprite)bugdetSlider.thumbObject).color = new Color32(1, 140, 46, 255);

            budgetSliderLabel.textScale = 0.5f;
            budgetSliderLabel.autoSize = false;
            budgetSliderLabel.wordWrap = true;
            budgetSliderLabel.pivot = UIPivotPoint.TopCenter;
            budgetSliderLabel.textAlignment = UIHorizontalAlignment.Center;
            budgetSliderLabel.text = string.Format(" x{0:0.00}", 0);
            budgetSliderLabel.prefix = Locale.Get("SVM_BUDGET_MULTIPLIER_PERIOD_LABEL", idx);
            budgetSliderLabel.width = 40;
            budgetSliderLabel.font = UIHelperExtension.defaultFontCheckbox;

            var idx_loc = idx;
            bugdetSlider.eventValueChanged += delegate (UIComponent c, float val)
            {
                budgetSliderLabel.text = string.Format(" x{0:0.00}", val);
                setBudgetHour(val, idx_loc);
            };

            return bugdetSlider;
        }

        private void updateSliders()
        {

            if (!getCurrentSelectedId(out int currentDistrict)) return;

            uint[] multipliers = extension.GetBudgetsMultiplierDistrict((uint)currentDistrict);

            m_disableBudgetPerHour.isVisible = multipliers.Length == 8;
            m_enableBudgetPerHour.isVisible = multipliers.Length == 1;

            for (int i = 0; i < m_budgetSliders.Length; i++)
            {
                UILabel budgetSliderLabel = m_budgetSliders[i].transform.parent.GetComponentInChildren<UILabel>();
                if (i == 0)
                {
                    if (multipliers.Length == 1)
                    {
                        budgetSliderLabel.prefix = Locale.Get("SVM_BUDGET_MULTIPLIER_PERIOD_LABEL_ALL");
                    }
                    else
                    {
                        budgetSliderLabel.prefix = Locale.Get("SVM_BUDGET_MULTIPLIER_PERIOD_LABEL", 0);
                    }
                }
                else
                {
                    m_budgetSliders[i].isEnabled = multipliers.Length == 8;
                    m_budgetSliders[i].parent.isVisible = multipliers.Length == 8;
                }

                if (i < multipliers.Length)
                {
                    m_budgetSliders[i].value = multipliers[i] / 100f;
                }
            }

        }
        #endregion
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

}
