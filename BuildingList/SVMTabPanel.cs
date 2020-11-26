using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Commons.Extensors;
using Klyte.Commons.Utils;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager.UI
{

    public class SVMTabPanel : UICustomControl
    {
        private const int NUM_SERVICES = 0;
        public static SVMTabPanel instance { get; private set; }

        public UIPanel controlContainer { get; private set; }
        private UIPanel mainPanel;
        private UIPanel m_titleLineBuildings;

        private UILabel m_directionLabel;

        private UITabstrip m_StripMain;
        private UITabstrip m_StripDistricts;
        private UITabstrip m_StripBuilings;

        private Dictionary<CategoryTab, UITabstrip> m_StripDistrictsStrips = new Dictionary<CategoryTab, UITabstrip>();
        private Dictionary<CategoryTab, UITabstrip> m_StripBuilingsStrips = new Dictionary<CategoryTab, UITabstrip>();

        private UIDropDown m_selectDistrict;
        private Dictionary<string, int> m_cachedDistricts;
        private string m_lastSelectedItem;

        public static OnButtonClicked eventOnDistrictSelectionChanged;

        #region Awake
        private void Awake()
        {
            instance = this;

            controlContainer = GetComponent<UIPanel>();
            controlContainer.area = new Vector4(0, 0, 0, 0);
            controlContainer.isVisible = false;
            controlContainer.name = "SVMPanel";

            KlyteMonoUtils.CreateUIElement(out mainPanel, controlContainer.transform, "SVMListPanel", new Vector4(0, 0, 875, 550));
            mainPanel.backgroundSprite = "MenuPanel2";

            CreateTitleBar();


            KlyteMonoUtils.CreateUIElement(out m_StripMain, mainPanel.transform, "SVMTabstrip", new Vector4(5, 40, mainPanel.width - 10, 40));

            KlyteMonoUtils.CreateUIElement(out UITabContainer tabContainer, mainPanel.transform, "SVMTabContainer", new Vector4(0, 80, mainPanel.width, mainPanel.height - 80));
            m_StripMain.tabPages = tabContainer;

            UIButton tabPerBuilding = CreateTabTemplate();
            tabPerBuilding.normalFgSprite = "ToolbarIconMonuments";
            tabPerBuilding.tooltip = Locale.Get("K45_SVM_CONFIG_PER_BUILDING_TAB");

            KlyteMonoUtils.CreateUIElement(out UIPanel contentContainerPerBuilding, null);
            contentContainerPerBuilding.name = "Container";
            contentContainerPerBuilding.area = new Vector4(0, 40, mainPanel.width, mainPanel.height - 80);

            m_StripMain.AddTab("SVMPerBuilding", tabPerBuilding.gameObject, contentContainerPerBuilding.gameObject);
            CreateTitleRowBuilding(ref m_titleLineBuildings, contentContainerPerBuilding);
            CreateSsdTabstrip(ref m_StripBuilings, ref m_StripBuilingsStrips, m_titleLineBuildings, contentContainerPerBuilding, true);

            UIButton tabPerDistrict = CreateTabTemplate();
            tabPerDistrict.normalFgSprite = "ToolbarIconDistrict";
            tabPerDistrict.tooltip = Locale.Get("K45_SVM_CONFIG_PER_DISTRICT_TAB");

            KlyteMonoUtils.CreateUIElement(out UIPanel contentContainerPerDistrict, mainPanel.transform);
            contentContainerPerDistrict.name = "Container2";
            contentContainerPerDistrict.area = new Vector4(0, 40, mainPanel.width, mainPanel.height - 80);

            m_StripMain.AddTab("SVMPerDistrict", tabPerDistrict.gameObject, contentContainerPerDistrict.gameObject);
            CreateSsdTabstrip(ref m_StripDistricts, ref m_StripDistrictsStrips, null, contentContainerPerDistrict);

            m_cachedDistricts = DistrictUtils.GetValidDistricts();

            m_selectDistrict = UIHelperExtension.CloneBasicDropDownLocalized("K45_SVM_DISTRICT_TITLE", m_cachedDistricts.Keys.OrderBy(x => x).ToArray(), OnDistrictSelect, 0, contentContainerPerDistrict);
            UIPanel container = m_selectDistrict.GetComponentInParent<UIPanel>();
            container.autoLayoutDirection = LayoutDirection.Horizontal;
            container.autoFitChildrenHorizontally = true;
            container.autoFitChildrenVertically = true;
            container.pivot = UIPivotPoint.TopRight;
            container.anchor = UIAnchorStyle.Top | UIAnchorStyle.Right;
            container.relativePosition = new Vector3(contentContainerPerDistrict.width - container.width - 10, -40);
            UILabel label = container.GetComponentInChildren<UILabel>();
            label.padding.top = 10;
            label.padding.right = 10;

            //DistrictManagerOverrides.eventOnDistrictChanged += reloadDistricts;

            m_StripMain.selectedIndex = -1;
            m_StripBuilings.selectedIndex = -1;
            m_StripDistricts.selectedIndex = -1;

            foreach (UITabstrip strip in m_StripDistrictsStrips.Values)
            {
                strip.selectedIndex = -1;
            }
            foreach (UITabstrip strip in m_StripBuilingsStrips.Values)
            {
                strip.selectedIndex = -1;
            }

            mainPanel.eventVisibilityChanged += (x, y) =>
            {
                if (y)
                {
                    ServiceVehiclesManagerMod.Instance.ShowVersionInfoPopup();
                }
            };
        }

        internal void OpenAt(ref ServiceSystemDefinition ssd)
        {
            m_StripMain.selectedIndex = 0;
            if (ssd != null)
            {
                var catIdx = ssd.Category;
                m_StripBuilings.selectedIndex = (int)catIdx;
                m_StripBuilingsStrips[catIdx].selectedIndex = m_StripBuilingsStrips[catIdx].Find<UIComponent>(ssd.GetDefType().Name)?.zOrder ?? 0;
            }
            ServiceVehiclesManagerMod.Instance.Controller.OpenSVMPanel();
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


        private void reloadDistricts()
        {
            m_cachedDistricts = DistrictUtils.GetValidDistricts();
            m_selectDistrict.items = m_cachedDistricts.Keys.OrderBy(x => x).ToArray();
            m_selectDistrict.selectedValue = m_lastSelectedItem;
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
            KlyteMonoUtils.CreateUIElement(out strip, parent.transform, "SVMTabstrip", new Vector4(5, 0, parent.width - 10, 40));

            float effectiveOffsetY = strip.height + (titleLine?.height ?? 0);

            KlyteMonoUtils.CreateUIElement(out UITabContainer tabContainer, parent.transform, "SVMTabContainer", new Vector4(0, 40, parent.width, parent.height - 40));
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
                KlyteMonoUtils.CreateUIElement(out UITabstrip subStrip, contentCategory.transform, "SVMTabstripCat" + catTab, new Vector4(5, 0, bodySuper.width - 10, 40));
                KlyteMonoUtils.CreateUIElement(out UITabContainer tabSubContainer, contentCategory.transform, "SVMTabContainer" + catTab, new Vector4(5, effectiveOffsetY, bodySuper.width - 10, bodySuper.height - effectiveOffsetY));
                subStrip.tabPages = tabSubContainer;
                substrips[catTab] = subStrip;
            }
            foreach (KeyValuePair<ServiceSystemDefinition, ISVMSysDef> kv in ServiceSystemDefinition.sysDefinitions)
            {
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
                    targetType = ReflectionUtils.GetImplementationForGenericType(typeof(SVMTabControllerBuildingList<>), kv.Value.GetType());
                    components = new Type[] { targetType };
                }
                else
                {
                    try
                    {
                        targetType = ReflectionUtils.GetImplementationForGenericType(typeof(SVMTabControllerDistrictList<>), kv.Value.GetType());
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
                        m_directionLabel.isVisible = kv.Key.outsideConnection;
                    }
                };
                tabsCategories[catTab].isVisible = true;
            }
        }



        private static UIButton CreateTabTemplate()
        {
            KlyteMonoUtils.CreateUIElement(out UIButton tabTemplate, null, "SVMTabTemplate");
            KlyteMonoUtils.InitButton(tabTemplate, false, "GenericTab");
            tabTemplate.autoSize = false;
            tabTemplate.width = 40;
            tabTemplate.height = 40;
            tabTemplate.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
            return tabTemplate;
        }

        private void CreateTitleRowBuilding(ref UIPanel titleLine, UIComponent parent)
        {
            KlyteMonoUtils.CreateUIElement(out titleLine, parent.transform, "SVMtitleline", new Vector4(5, 80, parent.width - 10, 40));

            KlyteMonoUtils.CreateUIElement(out UILabel districtNameLabel, titleLine.transform, "districtNameLabel");
            districtNameLabel.autoSize = false;
            districtNameLabel.area = new Vector4(0, 10, 175, 18);
            districtNameLabel.textAlignment = UIHorizontalAlignment.Center;
            districtNameLabel.text = Locale.Get("TUTORIAL_ADVISER_TITLE", "District");

            KlyteMonoUtils.CreateUIElement(out UILabel buildingNameLabel, titleLine.transform, "buildingNameLabel");
            buildingNameLabel.autoSize = false;
            buildingNameLabel.area = new Vector4(200, 10, 198, 18);
            buildingNameLabel.textAlignment = UIHorizontalAlignment.Center;
            buildingNameLabel.text = Locale.Get("K45_SVM_BUILDING_NAME_LABEL");

            KlyteMonoUtils.CreateUIElement(out UILabel vehicleCapacityLabel, titleLine.transform, "vehicleCapacityLabel");
            vehicleCapacityLabel.autoSize = false;
            vehicleCapacityLabel.area = new Vector4(475, 10, 200, 18);
            vehicleCapacityLabel.textAlignment = UIHorizontalAlignment.Center;
            vehicleCapacityLabel.text = Locale.Get("K45_SVM_VEHICLE_CAPACITY_LABEL");

            KlyteMonoUtils.CreateUIElement(out m_directionLabel, titleLine.transform, "directionLabel");
            m_directionLabel.autoSize = false;
            m_directionLabel.area = new Vector4(600, 10, 200, 18);
            m_directionLabel.textAlignment = UIHorizontalAlignment.Center;
            m_directionLabel.text = Locale.Get("K45_SVM_DIRECTION_LABEL");

        }

        private void CreateTitleBar()
        {
            KlyteMonoUtils.CreateUIElement(out UILabel titlebar, mainPanel.transform, "SVMListPanel", new Vector4(75, 10, mainPanel.width - 150, 20));
            titlebar.autoSize = false;
            titlebar.text = "Service Vehicles Manager v" + ServiceVehiclesManagerMod.Version;
            titlebar.textAlignment = UIHorizontalAlignment.Center;

            KlyteMonoUtils.CreateUIElement(out UIButton closeButton, mainPanel.transform, "CloseButton", new Vector4(mainPanel.width - 37, 5, 32, 32));
            KlyteMonoUtils.InitButton(closeButton, false, "buttonclose", true);
            closeButton.hoveredBgSprite = "buttonclosehover";
            closeButton.eventClick += (x, y) =>
            {
                ServiceVehiclesManagerMod.Instance.ClosePanel();
            };

            KlyteMonoUtils.CreateUIElement(out UISprite logo, mainPanel.transform, "SVMLogo", new Vector4(22, 5f, 32, 32));
            logo.spriteName = ServiceVehiclesManagerMod.Instance.IconName;
        }

        private static UIComponent CreateContentTemplate(float width, float height)
        {
            KlyteMonoUtils.CreateUIElement(out UIPanel contentContainer, null);
            contentContainer.name = "Container";
            contentContainer.area = new Vector4(0, 0, width, height);
            KlyteMonoUtils.CreateUIElement(out UIScrollablePanel scrollPanel, contentContainer.transform, "ScrollPanel");
            scrollPanel.width = contentContainer.width - 20f;
            scrollPanel.height = contentContainer.height;
            scrollPanel.autoLayoutDirection = LayoutDirection.Vertical;
            scrollPanel.autoLayoutStart = LayoutStart.TopLeft;
            scrollPanel.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
            scrollPanel.autoLayout = true;
            scrollPanel.clipChildren = true;
            scrollPanel.relativePosition = new Vector3(5, 0);

            KlyteMonoUtils.CreateUIElement(out UIPanel trackballPanel, contentContainer.transform, "Trackball");
            trackballPanel.width = 10f;
            trackballPanel.height = scrollPanel.height;
            trackballPanel.autoLayoutDirection = LayoutDirection.Horizontal;
            trackballPanel.autoLayoutStart = LayoutStart.TopLeft;
            trackballPanel.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
            trackballPanel.autoLayout = true;
            trackballPanel.relativePosition = new Vector3(contentContainer.width - 15, 0);

            KlyteMonoUtils.CreateUIElement(out UIScrollbar scrollBar, trackballPanel.transform, "Scrollbar");
            scrollBar.width = 10f;
            scrollBar.height = scrollBar.parent.height;
            scrollBar.orientation = UIOrientation.Vertical;
            scrollBar.pivot = UIPivotPoint.BottomLeft;
            scrollBar.AlignTo(trackballPanel, UIAlignAnchor.TopRight);
            scrollBar.minValue = 0f;
            scrollBar.value = 0f;
            scrollBar.incrementAmount = 25f;

            KlyteMonoUtils.CreateUIElement(out UISlicedSprite scrollBg, scrollBar.transform, "ScrollbarBg");
            scrollBg.relativePosition = Vector2.zero;
            scrollBg.autoSize = true;
            scrollBg.size = scrollBg.parent.size;
            scrollBg.fillDirection = UIFillDirection.Vertical;
            scrollBg.spriteName = "ScrollbarTrack";
            scrollBar.trackObject = scrollBg;

            KlyteMonoUtils.CreateUIElement(out UISlicedSprite scrollFg, scrollBg.transform, "ScrollbarFg");
            scrollFg.relativePosition = Vector2.zero;
            scrollFg.fillDirection = UIFillDirection.Vertical;
            scrollFg.autoSize = true;
            scrollFg.width = scrollFg.parent.width - 4f;
            scrollFg.spriteName = "ScrollbarThumb";
            scrollBar.thumbObject = scrollFg;
            scrollPanel.verticalScrollbar = scrollBar;
            scrollPanel.eventMouseWheel += delegate (UIComponent component, UIMouseEventParameter param)
            {
                scrollPanel.scrollPosition += new Vector2(0f, Mathf.Sign(param.wheelDelta) * -1f * scrollBar.incrementAmount);
                param.Use();
            };
            return contentContainer;
        }
        #endregion


        public void SetActiveTab(int idx) => m_StripMain.selectedIndex = idx;

        private void Update()
        {
        }
    }

    public enum CategoryTab
    {
        OutsideConnection,
        PublicTransport,
        EmergencyVehicles,
        SecurityVehicles,
        HealthcareVehicles,
        OtherServices
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
                default:
                    throw new Exception($"Not supported: {tab}");
            }

        }
    }

}
