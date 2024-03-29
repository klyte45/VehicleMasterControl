﻿using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Commons.Extensors;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using Klyte.VehiclesMasterControl.Extensors.VehicleExt;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klyte.VehiclesMasterControl.UI
{

    public class VMCTabPanel : BasicKPanel<VehiclesMasterControlMod, VMCController, VMCTabPanel>
    {
        public UIPanel controlContainer { get; private set; }

        public override float PanelWidth => 875;

        public override float PanelHeight => 550;

        private UIPanel mainPanel;
        private UIPanel m_titleLineBuildings;

        private UILabel m_directionLabel;

        //   private UITabstrip m_StripMain;
        private UITabstrip m_StripDistricts;
        //    private UITabstrip m_StripBuilings;

        private Dictionary<CategoryTab, UITabstrip> m_StripDistrictsStrips = new Dictionary<CategoryTab, UITabstrip>();
        //  private Dictionary<CategoryTab, UITabstrip> m_StripBuilingsStrips = new Dictionary<CategoryTab, UITabstrip>();

        private UIDropDown m_selectDistrict;
        private Dictionary<string, int> m_cachedDistricts;
        private string m_lastSelectedItem;

        public static OnButtonClicked eventOnDistrictSelectionChanged;

        #region Awake
        protected override void AwakeActions()
        {
            controlContainer = GetComponent<UIPanel>();
            controlContainer.area = new Vector4(0, 0, 0, 0);
            controlContainer.isVisible = false;
            controlContainer.name = "VMCPanel";

            KlyteMonoUtils.CreateUIElement(out mainPanel, controlContainer.transform, "VMCListPanel", new Vector4(0, 0, 875, 550));
            mainPanel.backgroundSprite = "MenuPanel2";

            CreateTitleBar();


            //KlyteMonoUtils.CreateUIElement(out m_StripMain, mainPanel.transform, "VMCTabstrip", new Vector4(5, 40, mainPanel.width - 10, 40));

            //KlyteMonoUtils.CreateUIElement(out UITabContainer tabContainer, mainPanel.transform, "VMCTabContainer", new Vector4(0, 80, mainPanel.width, mainPanel.height - 80));
            //m_StripMain.tabPages = tabContainer;

            //UIButton tabPerBuilding = CreateTabTemplate();
            //tabPerBuilding.normalFgSprite = "ToolbarIconMonuments";
            //tabPerBuilding.tooltip = Locale.Get("K45_VMC_CONFIG_PER_BUILDING_TAB");

            //KlyteMonoUtils.CreateUIElement(out UIPanel contentContainerPerBuilding, null);
            //contentContainerPerBuilding.name = "Container";
            //contentContainerPerBuilding.area = new Vector4(0, 40, mainPanel.width, mainPanel.height - 80);

            //m_StripMain.AddTab("VMCPerBuilding", tabPerBuilding.gameObject, contentContainerPerBuilding.gameObject);
            //CreateTitleRowBuilding(ref m_titleLineBuildings, contentContainerPerBuilding);
            //CreateSsdTabstrip(ref m_StripBuilings, ref m_StripBuilingsStrips, m_titleLineBuildings, contentContainerPerBuilding, true);

            //UIButton tabPerDistrict = CreateTabTemplate();
            //tabPerDistrict.normalFgSprite = "ToolbarIconDistrict";
            //tabPerDistrict.tooltip = Locale.Get("K45_VMC_CONFIG_PER_DISTRICT_TAB");

            KlyteMonoUtils.CreateUIElement(out UIPanel contentContainerPerDistrict, mainPanel.transform);
            contentContainerPerDistrict.name = "Container2";
            contentContainerPerDistrict.area = new Vector4(0, 50, mainPanel.width, mainPanel.height - 50);

            //m_StripMain.AddTab("VMCPerDistrict", tabPerDistrict.gameObject, contentContainerPerDistrict.gameObject);
            CreateSsdTabstrip(ref m_StripDistricts, ref m_StripDistrictsStrips, null, contentContainerPerDistrict);

            m_cachedDistricts = DistrictUtils.GetValidDistricts();

            m_selectDistrict = UIHelperExtension.CloneBasicDropDownLocalized("K45_VMC_DISTRICT_TITLE", m_cachedDistricts.Keys.OrderBy(x => x).ToArray(), OnDistrictSelect, 0, contentContainerPerDistrict);
            UIPanel container = m_selectDistrict.GetComponentInParent<UIPanel>();
            container.autoLayoutDirection = LayoutDirection.Horizontal;
            container.autoFitChildrenHorizontally = true;
            container.autoFitChildrenVertically = true;
            container.pivot = UIPivotPoint.TopRight;
            container.anchor = UIAnchorStyle.Top | UIAnchorStyle.Right;
            container.relativePosition = new Vector3(contentContainerPerDistrict.width - container.width - 10, 0);
            UILabel label = container.GetComponentInChildren<UILabel>();
            label.padding.top = 10;
            label.padding.right = 10;

            VehiclesMasterControlMod.Controller.eventOnDistrictChanged += ReloadDistricts;

            //   m_StripMain.selectedIndex = -1;
            //   m_StripBuilings.selectedIndex = -1;
            m_StripDistricts.selectedIndex = -1;

            foreach (UITabstrip strip in m_StripDistrictsStrips.Values)
            {
                strip.selectedIndex = -1;
            }
            //foreach (UITabstrip strip in m_StripBuilingsStrips.Values)
            //{
            //    strip.selectedIndex = -1;
            //}

            mainPanel.eventVisibilityChanged += (x, y) =>
            {
                if (y)
                {
                    VehiclesMasterControlMod.Instance.ShowVersionInfoPopup();
                }
            };
        }

        internal void OpenAt(ref ServiceSystemDefinition ssd)
        {
            //   m_StripMain.selectedIndex = 0;
            if (ssd != null)
            {
                var catIdx = ssd.Category;
                m_StripDistricts.selectedIndex = (int)catIdx;
                m_StripDistrictsStrips[catIdx].selectedIndex = m_StripDistrictsStrips[catIdx].Find<UIComponent>(ssd.GetDefType().Name)?.zOrder ?? 0;
            }
            VehiclesMasterControlMod.Controller.OpenVMCPanel();
        }

        private void OnDistrictSelect(int x)
        {
            string oldSel = m_lastSelectedItem;
            try
            {
                m_lastSelectedItem = m_selectDistrict.items[x];
            }
            catch
            {
                if (m_selectDistrict.items.Length > 0)
                {
                    m_lastSelectedItem = m_selectDistrict.items[0];
                }
                else
                {
                    m_lastSelectedItem = null;
                    m_selectDistrict.selectedIndex = -1;
                }
            }
            if (oldSel != m_lastSelectedItem)
            {
                eventOnDistrictSelectionChanged?.Invoke();
            }
        }


        private void ReloadDistricts()
        {
            m_cachedDistricts = DistrictUtils.GetValidDistricts();
            m_selectDistrict.items = m_cachedDistricts.Keys.OrderBy(x => x).ToArray();
            m_selectDistrict.selectedValue = m_lastSelectedItem;
            if (m_selectDistrict.selectedIndex < 0)
            {
                m_selectDistrict.selectedIndex = 0;
            }

            eventOnDistrictSelectionChanged?.Invoke();
        }

        public int getCurrentSelectedDistrictId()
        {
            if (m_lastSelectedItem == null || !m_cachedDistricts.ContainsKey(m_lastSelectedItem))
            {
                return -1;
            }
            return m_cachedDistricts[m_lastSelectedItem];
        }

        private void CreateSsdTabstrip(ref UITabstrip strip, ref Dictionary<CategoryTab, UITabstrip> substrips, UIPanel titleLine, UIComponent parent, bool buildings = false)
        {
            KlyteMonoUtils.CreateUIElement(out strip, parent.transform, "VMCTabstrip", new Vector4(5, 0, parent.width - 10, 40));

            float effectiveOffsetY = strip.height + (titleLine?.height ?? 0);

            KlyteMonoUtils.CreateUIElement(out UITabContainer tabContainer, parent.transform, "VMCTabContainer", new Vector4(0, 40, parent.width, parent.height - 40));
            strip.tabPages = tabContainer;

            UIButton tabTemplate = CreateTabTemplate();

            UIComponent bodyContent = CreateContentTemplate(parent.width - 10, parent.height - effectiveOffsetY - 50);
            KlyteMonoUtils.CreateUIElement(out UIPanel bodySuper, null);
            bodySuper.name = "Container";
            bodySuper.area = new Vector4(0, 40, parent.width, parent.height - 50);

            var tabsCategories = new Dictionary<CategoryTab, UIComponent>();

            foreach (CategoryTab catTab in Enum.GetValues(typeof(CategoryTab)).Cast<CategoryTab>())
            {
                GameObject tabCategory = Instantiate(tabTemplate.gameObject);
                GameObject contentCategory = Instantiate(bodySuper.gameObject);
                UIButton tabButtonSuper = tabCategory.GetComponent<UIButton>();
                tabButtonSuper.tooltip = catTab.getCategoryName();
                tabButtonSuper.normalFgSprite = catTab.getCategoryIcon();
                tabsCategories[catTab] = strip.AddTab(catTab.ToString(), tabCategory, contentCategory);
                tabsCategories[catTab].isVisible = false;
                KlyteMonoUtils.CreateUIElement(out UITabstrip subStrip, contentCategory.transform, "VMCTabstripCat" + catTab, new Vector4(5, 0, bodySuper.width - 10, 40));
                KlyteMonoUtils.CreateUIElement(out UITabContainer tabSubContainer, contentCategory.transform, "VMCTabContainer" + catTab, new Vector4(5, effectiveOffsetY, bodySuper.width - 10, bodySuper.height - effectiveOffsetY));
                subStrip.tabPages = tabSubContainer;
                substrips[catTab] = subStrip;
            }
            foreach (KeyValuePair<ServiceSystemDefinition, IVMCSysDef> kv in ServiceSystemDefinition.sysDefinitions)
            {
                if (kv.Value.GetSSD().GetDistrictExtension().GetAllBasicAssets().Count == 0)
                {
                    continue;
                }

                GameObject tab = Instantiate(tabTemplate.gameObject);
                GameObject body = Instantiate(bodyContent.gameObject);
                string name = kv.Value.GetType().Name;
                LogUtils.DoLog($"kv.Key = {kv.Key}; kv.Value= {kv.Value} ");
                string bgIcon = kv.Key.IconServiceSystem;
                string fgIcon = kv.Key.FgIconServiceSystem;
                UIButton tabButton = tab.GetComponent<UIButton>();
                tabButton.tooltip = kv.Key.NameForServiceSystem;
                tabButton.normalFgSprite = bgIcon;
                if (!string.IsNullOrEmpty(fgIcon))
                {
                    KlyteMonoUtils.CreateUIElement(out UISprite sprite, tabButton.transform, "OverSprite", new Vector4(0, 0, 40, 40));
                    sprite.spriteName = fgIcon;
                }
                Type[] components;
                Type targetType;
                if (buildings)
                {
                    try
                    {
                        targetType = ReflectionUtils.GetImplementationForGenericType(typeof(VMCTabControllerBuildingList<>), kv.Value.GetType());
                        components = new Type[] { targetType };
                    }
                    catch
                    {
                        continue;
                    }
                }
                else
                {
                    try
                    {
                        targetType = ReflectionUtils.GetImplementationForGenericType(typeof(VMCTabControllerDistrictList<>), kv.Value.GetType());
                        components = new Type[] { targetType };
                    }
                    catch
                    {
                        continue;
                    }

                }
                CategoryTab catTab = kv.Key.Category;
                substrips[catTab].AddTab(name, tab, body, components);

                body.GetComponent<UIComponent>().eventVisibilityChanged += (x, y) =>
                {
                    if (y)
                    {
                        //   m_directionLabel.isVisible = kv.Key.outsideConnection;
                    }
                };
                tabsCategories[catTab].isVisible = true;
            }
        }



        private static UIButton CreateTabTemplate()
        {
            KlyteMonoUtils.CreateUIElement(out UIButton tabTemplate, null, "VMCTabTemplate");
            KlyteMonoUtils.InitButton(tabTemplate, false, "GenericTab");
            tabTemplate.autoSize = false;
            tabTemplate.width = 40;
            tabTemplate.height = 40;
            tabTemplate.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
            return tabTemplate;
        }

        private void CreateTitleRowBuilding(ref UIPanel titleLine, UIComponent parent)
        {
            KlyteMonoUtils.CreateUIElement(out titleLine, parent.transform, "VMCtitleline", new Vector4(5, 80, parent.width - 10, 40));

            KlyteMonoUtils.CreateUIElement(out UILabel districtNameLabel, titleLine.transform, "districtNameLabel");
            districtNameLabel.autoSize = false;
            districtNameLabel.area = new Vector4(0, 10, 175, 18);
            districtNameLabel.textAlignment = UIHorizontalAlignment.Center;
            districtNameLabel.text = Locale.Get("TUTORIAL_ADVISER_TITLE", "District");

            KlyteMonoUtils.CreateUIElement(out UILabel buildingNameLabel, titleLine.transform, "buildingNameLabel");
            buildingNameLabel.autoSize = false;
            buildingNameLabel.area = new Vector4(200, 10, 198, 18);
            buildingNameLabel.textAlignment = UIHorizontalAlignment.Center;
            buildingNameLabel.text = Locale.Get("K45_VMC_BUILDING_NAME_LABEL");

            KlyteMonoUtils.CreateUIElement(out UILabel vehicleCapacityLabel, titleLine.transform, "vehicleCapacityLabel");
            vehicleCapacityLabel.autoSize = false;
            vehicleCapacityLabel.area = new Vector4(475, 10, 200, 18);
            vehicleCapacityLabel.textAlignment = UIHorizontalAlignment.Center;
            vehicleCapacityLabel.text = Locale.Get("K45_VMC_VEHICLE_CAPACITY_LABEL");

            KlyteMonoUtils.CreateUIElement(out m_directionLabel, titleLine.transform, "directionLabel");
            m_directionLabel.autoSize = false;
            m_directionLabel.area = new Vector4(600, 10, 200, 18);
            m_directionLabel.textAlignment = UIHorizontalAlignment.Center;
            m_directionLabel.text = Locale.Get("K45_VMC_DIRECTION_LABEL");

        }

        private void CreateTitleBar()
        {
            KlyteMonoUtils.CreateUIElement(out UILabel titlebar, mainPanel.transform, "VMCListPanel", new Vector4(75, 10, mainPanel.width - 150, 20));
            titlebar.autoSize = false;
            titlebar.text = VehiclesMasterControlMod.Instance.Name;
            titlebar.textAlignment = UIHorizontalAlignment.Center;

            KlyteMonoUtils.CreateUIElement(out UIButton closeButton, mainPanel.transform, "CloseButton", new Vector4(mainPanel.width - 37, 5, 32, 32));
            KlyteMonoUtils.InitButton(closeButton, false, "buttonclose", true);
            closeButton.hoveredBgSprite = "buttonclosehover";
            closeButton.eventClick += (x, y) =>
            {
                VehiclesMasterControlMod.Instance.ClosePanel();
            };

            KlyteMonoUtils.CreateUIElement(out UISprite logo, mainPanel.transform, "VMCLogo", new Vector4(22, 5f, 32, 32));
            logo.spriteName = VehiclesMasterControlMod.Instance.IconName;
        }

        private static UIComponent CreateContentTemplate(float width, float height)
        {

            KlyteMonoUtils.CreateUIElement(out UIPanel contentContainer, null);
            contentContainer.name = "Container";
            contentContainer.area = new Vector4(0, 0, width, height);
            KlyteMonoUtils.CreateScrollPanel(contentContainer, out UIScrollablePanel mcrollablePanel, out UIScrollbar sc, width - 20f, height);
            return contentContainer;
        }
        #endregion


        //     public void SetActiveTab(int idx) => m_StripMain.selectedIndex = idx;


    }

    public enum CategoryTab
    {
        OutsideConnection,
        PublicTransport,
        EmergencyVehicles,
        SecurityVehicles,
        HealthcareVehicles,
        Fish,
        Garbage,
        Industry,
        Residential,
        OtherServices,
    }

    public static class CategoryTabExtension
    {
        public static string getCategoryName(this CategoryTab tab)
        {
            switch (tab)
            {
                case CategoryTab.EmergencyVehicles:
                    return Locale.Get("MAIN_TOOL_ND", "FireDepartment");
                case CategoryTab.OutsideConnection:
                    return Locale.Get("AREA_CONNECTIONS");
                case CategoryTab.PublicTransport:
                    return Locale.Get("ASSETIMPORTER_CATEGORY", "PublicTransport");
                case CategoryTab.SecurityVehicles:
                    return Locale.Get("ASSETIMPORTER_CATEGORY", "Police");
                case CategoryTab.HealthcareVehicles:
                    return Locale.Get("ASSETIMPORTER_CATEGORY", "Healthcare");
                case CategoryTab.OtherServices:
                    return Locale.Get("ROUTECHECKBOX6");
                case CategoryTab.Fish:
                    return Locale.Get("MAIN_CATEGORY", "IndustryFishing");
                case CategoryTab.Garbage:
                    return Locale.Get("MAIN_TOOL", "Garbage");
                case CategoryTab.Industry:
                    return Locale.Get("TUTORIAL_ADVISER_TITLE", "InfoViewIndustry");
                case CategoryTab.Residential:
                    return Locale.Get("STATS_16");
                default:
                    throw new Exception($"Not supported: {tab}");
            }

        }
        public static string getCategoryIcon(this CategoryTab tab)
        {
            switch (tab)
            {
                case CategoryTab.EmergencyVehicles:
                    return "SubBarFireDepartmentDisaster";
                case CategoryTab.OutsideConnection:
                    return "IconRightArrow";
                case CategoryTab.PublicTransport:
                    return "ToolbarIconPublicTransport";
                case CategoryTab.SecurityVehicles:
                    return "ToolbarIconPolice";
                case CategoryTab.HealthcareVehicles:
                    return "ToolbarIconHealthcare";
                case CategoryTab.OtherServices:
                    return "ToolbarIconHelp";
                case CategoryTab.Fish:
                    return "InfoIconFishing";
                case CategoryTab.Garbage:
                    return "InfoIconGarbage";
                case CategoryTab.Industry:
                    return "ToolbarIconGarbage";
                case CategoryTab.Residential:
                    return "SubBarDistrictSpecializationResidential";
                default:
                    throw new Exception($"Not supported: {tab}");
            }

        }
    }

}
