using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Extensions;
using Klyte.Harmony;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.Overrides;
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

    class SVMServiceBuildingDetailPanel : UICustomControl
    {
        private const int NUM_SERVICES = 0;
        private static SVMServiceBuildingDetailPanel instance;

        private UIPanel controlContainer;
        private UIPanel mainPanel;
        private UIPanel m_titleLineBuildings;

        private UITabstrip m_StripMain;
        private UITabstrip m_StripDistricts;
        private UITabstrip m_StripBuilings;

        private UIDropDown m_selectDistrict;
        private Dictionary<string, int> m_cachedDistricts;
        private string m_lastSelectedItem;

        public static OnButtonClicked eventOnDistrictSelectionChanged;

        public static SVMServiceBuildingDetailPanel Get()
        {
            if (instance)
            {
                return instance;
            }
            UIView view = FindObjectOfType<UIView>();
            SVMUtils.createUIElement(out UIPanel panelObj, view.transform);

            return instance = panelObj.gameObject.AddComponent<SVMServiceBuildingDetailPanel>();
        }

        #region Awake
        private void Awake()
        {
            controlContainer = GetComponent<UIPanel>();
            controlContainer.area = new Vector4(0, 0, 0, 0);
            controlContainer.isVisible = false;
            controlContainer.name = "SVMPanel";

            SVMUtils.createUIElement(out mainPanel, controlContainer.transform, "SVMListPanel", new Vector4(395, 58, 875, 510));
            mainPanel.backgroundSprite = "MenuPanel2";

            CreateTitleBar();


            SVMUtils.createUIElement(out m_StripMain, mainPanel.transform, "SVMTabstrip", new Vector4(5, 40, mainPanel.width - 10, 40));

            SVMUtils.createUIElement(out UITabContainer tabContainer, mainPanel.transform, "SVMTabContainer", new Vector4(0, 80, mainPanel.width, mainPanel.height - 80));
            m_StripMain.tabPages = tabContainer;

            UIButton tabPerBuilding = CreateTabTemplate();
            tabPerBuilding.normalFgSprite = "ToolbarIconMonuments";
            tabPerBuilding.tooltip = Locale.Get("SVM_CONFIG_PER_BUILDING_TAB");

            SVMUtils.createUIElement(out UIPanel contentContainerPerBuilding, null);
            contentContainerPerBuilding.name = "Container";
            contentContainerPerBuilding.area = new Vector4(0, 0, mainPanel.width, mainPanel.height - 80);

            m_StripMain.AddTab("SVMPerBuilding", tabPerBuilding.gameObject, contentContainerPerBuilding.gameObject);
            CreateTitleRowBuilding(ref m_titleLineBuildings, contentContainerPerBuilding);
            CreateSsdTabstrip(ref m_StripBuilings, m_titleLineBuildings, contentContainerPerBuilding, true);

            UIButton tabPerDistrict = CreateTabTemplate();
            tabPerDistrict.normalFgSprite = "ToolbarIconDistrict";
            tabPerDistrict.tooltip = Locale.Get("SVM_CONFIG_PER_DISTRICT_TAB");

            SVMUtils.createUIElement(out UIPanel contentContainerPerDistrict, mainPanel.transform);
            contentContainerPerDistrict.name = "Container2";
            contentContainerPerDistrict.area = new Vector4(0, 0, mainPanel.width, mainPanel.height - 80);

            m_StripMain.AddTab("SVMPerDistrict", tabPerDistrict.gameObject, contentContainerPerDistrict.gameObject);
            CreateSsdTabstrip(ref m_StripDistricts, null, contentContainerPerDistrict);

            m_cachedDistricts = SVMUtils.getValidDistricts();

            m_selectDistrict = UIHelperExtension.CloneBasicDropDownLocalized("SVM_DISTRICT_TITLE", m_cachedDistricts.Keys.OrderBy(x => x).ToArray(), OnDistrictSelect, 0, contentContainerPerDistrict);
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

            DistrictManagerOverrides.eventOnDistrictRenamed += reloadDistricts;

            m_StripMain.selectedIndex = -1;
            m_StripBuilings.selectedIndex = -1;
            m_StripDistricts.selectedIndex = -1;
        }

        private void OnDistrictSelect(int x)
        {
            String oldSel = m_lastSelectedItem;
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
            m_cachedDistricts = SVMUtils.getValidDistricts();
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

        private static void CreateSsdTabstrip(ref UITabstrip strip, UIPanel titleLine, UIComponent parent, bool buildings = false)
        {
            SVMUtils.createUIElement(out strip, parent.transform, "SVMTabstrip", new Vector4(5, 0, parent.width - 10, 40));

            var effectiveOffsetY = strip.height + (titleLine?.height ?? 0);

            SVMUtils.createUIElement(out UITabContainer tabContainer, parent.transform, "SVMTabContainer", new Vector4(5, effectiveOffsetY + 5, parent.width - 10, parent.height - effectiveOffsetY - 10));
            strip.tabPages = tabContainer;

            UIButton tabTemplate = CreateTabTemplate();

            UIComponent scrollTemplate = CreateContentTemplate(parent.width - 10, parent.height - effectiveOffsetY - 10);

            foreach (var kv in ServiceSystemDefinition.sysDefinitions)
            {
                GameObject tab = Instantiate(tabTemplate.gameObject);
                GameObject body = Instantiate(scrollTemplate.gameObject);
                var configIdx = kv.Key.toConfigIndex();
                String name = SVMConfigWarehouse.getNameForServiceSystem(configIdx);
                String bgIcon = SVMConfigWarehouse.getIconServiceSystem(configIdx);
                String fgIcon = SVMConfigWarehouse.getFgIconServiceSystem(configIdx);
                UIButton tabButton = tab.GetComponent<UIButton>();
                tabButton.tooltip = name;
                tabButton.normalFgSprite = bgIcon;
                if (!string.IsNullOrEmpty(fgIcon))
                {
                    SVMUtils.createUIElement(out UISprite sprite, tabButton.transform, "OverSprite", new Vector4(0, 0, 40, 40));
                    sprite.spriteName = fgIcon;
                    sprite.atlas = SVMController.taSVM;
                }
                Type[] components;
                if (buildings)
                {
                    Type targetType = KlyteUtils.GetImplementationForGenericType(typeof(SVMTabControllerBuildingList<>), kv.Value);
                    components = new Type[] { targetType };
                }
                else
                {
                    Type targetType = KlyteUtils.GetImplementationForGenericType(typeof(SVMTabControllerDistrictList<>), kv.Value);
                    components = new Type[] { targetType };
                }
                strip.AddTab(name, tab, body, components);
            }
        }



        private static UIButton CreateTabTemplate()
        {
            SVMUtils.createUIElement(out UIButton tabTemplate, null, "SVMTabTemplate");
            SVMUtils.initButton(tabTemplate, false, "GenericTab");
            tabTemplate.autoSize = false;
            tabTemplate.width = 40;
            tabTemplate.height = 40;
            tabTemplate.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
            return tabTemplate;
        }

        private static void CreateTitleRowBuilding(ref UIPanel titleLine, UIComponent parent)
        {
            SVMUtils.createUIElement(out titleLine, parent.transform, "SVMtitleline", new Vector4(5, 40, parent.width - 10, 40));

            SVMUtils.createUIElement(out UILabel districtNameLabel, titleLine.transform, "District");
            districtNameLabel.autoSize = false;
            districtNameLabel.area = new Vector4(0, 10, 175, 18);
            districtNameLabel.textAlignment = UIHorizontalAlignment.Center;
            districtNameLabel.text = Locale.Get("TUTORIAL_ADVISER_TITLE", "District");

            SVMUtils.createUIElement(out UILabel buildingNameLabel, titleLine.transform, "District");
            buildingNameLabel.autoSize = false;
            buildingNameLabel.area = new Vector4(200, 10, 198, 18);
            buildingNameLabel.textAlignment = UIHorizontalAlignment.Center;
            buildingNameLabel.text = Locale.Get("SVM_BUILDING_NAME_LABEL");

            SVMUtils.createUIElement(out UILabel vehicleCapacityLabel, titleLine.transform, "District");
            vehicleCapacityLabel.autoSize = false;
            vehicleCapacityLabel.area = new Vector4(400, 10, 200, 18);
            vehicleCapacityLabel.textAlignment = UIHorizontalAlignment.Center;
            vehicleCapacityLabel.text = Locale.Get("SVM_VEHICLE_CAPACITY_LABEL");
        }

        private void CreateTitleBar()
        {
            SVMUtils.createUIElement(out UILabel titlebar, mainPanel.transform, "SVMListPanel", new Vector4(75, 10, mainPanel.width - 150, 20));
            titlebar.autoSize = false;
            titlebar.text = "Service Vehicles Manager v" + ServiceVehiclesManagerMod.version;
            titlebar.textAlignment = UIHorizontalAlignment.Center;
            SVMUtils.createDragHandle(titlebar, mainPanel);

            SVMUtils.createUIElement(out UIButton closeButton, mainPanel.transform, "CloseButton", new Vector4(mainPanel.width - 37, 5, 32, 32));
            SVMUtils.initButton(closeButton, false, "buttonclose", true);
            closeButton.hoveredBgSprite = "buttonclosehover";
            closeButton.eventClick += (x, y) =>
            {
                SVMController.instance.CloseSVMPanel();
            };

            SVMUtils.createUIElement(out UISprite logo, mainPanel.transform, "SVMLogo", new Vector4(22, 5f, 32, 32));
            logo.atlas = SVMController.taSVM;
            logo.spriteName = "ServiceVehiclesManagerIcon";
            SVMUtils.createDragHandle(logo, mainPanel);
        }

        private static UIComponent CreateContentTemplate(float width, float height)
        {
            SVMUtils.createUIElement(out UIPanel contentContainer, null);
            contentContainer.name = "Container";
            contentContainer.area = new Vector4(0, 0, width, height);
            SVMUtils.createUIElement(out UIScrollablePanel scrollPanel, contentContainer.transform, "ScrollPanel");
            scrollPanel.width = contentContainer.width - 20f;
            scrollPanel.height = contentContainer.height;
            scrollPanel.autoLayoutDirection = LayoutDirection.Vertical;
            scrollPanel.autoLayoutStart = LayoutStart.TopLeft;
            scrollPanel.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
            scrollPanel.autoLayout = true;
            scrollPanel.clipChildren = true;
            scrollPanel.relativePosition = new Vector3(5, 0);

            SVMUtils.createUIElement(out UIPanel trackballPanel, contentContainer.transform, "Trackball");
            trackballPanel.width = 10f;
            trackballPanel.height = scrollPanel.height;
            trackballPanel.autoLayoutDirection = LayoutDirection.Horizontal;
            trackballPanel.autoLayoutStart = LayoutStart.TopLeft;
            trackballPanel.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
            trackballPanel.autoLayout = true;
            trackballPanel.relativePosition = new Vector3(contentContainer.width - 15, 0);

            SVMUtils.createUIElement(out UIScrollbar scrollBar, trackballPanel.transform, "Scrollbar");
            scrollBar.width = 10f;
            scrollBar.height = scrollBar.parent.height;
            scrollBar.orientation = UIOrientation.Vertical;
            scrollBar.pivot = UIPivotPoint.BottomLeft;
            scrollBar.AlignTo(trackballPanel, UIAlignAnchor.TopRight);
            scrollBar.minValue = 0f;
            scrollBar.value = 0f;
            scrollBar.incrementAmount = 25f;

            SVMUtils.createUIElement(out UISlicedSprite scrollBg, scrollBar.transform, "ScrollbarBg");
            scrollBg.relativePosition = Vector2.zero;
            scrollBg.autoSize = true;
            scrollBg.size = scrollBg.parent.size;
            scrollBg.fillDirection = UIFillDirection.Vertical;
            scrollBg.spriteName = "ScrollbarTrack";
            scrollBar.trackObject = scrollBg;

            SVMUtils.createUIElement(out UISlicedSprite scrollFg, scrollBg.transform, "ScrollbarFg");
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


        public void SetActiveTab(int idx)
        {
            this.m_StripMain.selectedIndex = idx;
        }

        private void Update()
        {
        }
    }


}
