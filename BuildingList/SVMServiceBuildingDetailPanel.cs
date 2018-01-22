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

    class SVMServiceBuildingDetailPanel : UICustomControl
    {
        private enum LineSortCriterion
        {
            DEFAULT,
            NAME,
            STOP,
            VEHICLE,
            PASSENGER,
            LINE_NUMBER
        }

        private enum DepotSortCriterion
        {
            DEFAULT,
            NAME,
            DISTRICT
        }

        private const int NUM_SERVICES = 0;
        private static SVMServiceBuildingDetailPanel instance;

        private UIPanel controlContainer;
        private UIPanel mainPanel;

        private int m_LastLineCount;

        private bool m_Ready;


        private bool m_LinesUpdated;

        private bool[] m_ToggleAllState;

        private LineSortCriterion m_LastSortCriterionLines;

        private UITabstrip m_StripMain;


        private UITabstrip m_StripDistricts;
        private UITabstrip m_StripBuilings;
        private UIPanel m_titleLineBuildings;

        private bool m_isDepotView = true;

        private bool m_showDayNightLines = true;
        private bool m_showDayLines = true;
        private bool m_showNightLines = true;
        private bool m_showDisabledLines = true;

        private int m_busCount = 0;
        private int m_tramCount = 0;
        private int m_metroCount = 0;
        private int m_trainCount = 0;
        private int m_blimpCount = 0;
        private int m_ferryCount = 0;
        private int m_monorailCount = 0;
        private int m_evacCount = 0;

        //TLM
        private int m_shipCount = 0;
        private int m_planeCount = 0;


        private bool m_isChangingTab;

        private UILabel m_LineCount;


        //stríp buttons
        private UIButton bus_strip;
        private UIButton tram_strip;
        private UIButton metro_strip;
        private UIButton train_strip;
        private UIButton ferry_strip;
        private UIButton blimp_strip;
        private UIButton monorail_strip;
        private UIButton ship_strip;
        private UIButton plane_strip;
        private UIButton evac_strip;

        private UIButton planeDepot_strip;
        private UIButton blimpDepot_strip;
        private UIButton shipDepot_strip;
        private UIButton ferryDepot_strip;
        private UIButton trainDepot_strip;
        private UIButton monorailDepot_strip;
        private UIButton metroDepot_strip;
        private UIButton tramDepot_strip;
        private UIButton busDepot_strip;
        private UIButton evacDepot_strip;


        public static SVMServiceBuildingDetailPanel Create()
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
            CreateSsdTabstrip(ref m_StripBuilings, m_titleLineBuildings, contentContainerPerBuilding);

            UIButton tabPerDistrict = CreateTabTemplate();
            tabPerDistrict.normalFgSprite = "ToolbarIconDistrict";
            tabPerDistrict.tooltip = Locale.Get("SVM_CONFIG_PER_BUILDING_TAB");

            SVMUtils.createUIElement(out UIPanel contentContainerPerDistrict, mainPanel.transform);
            contentContainerPerDistrict.name = "Container2";
            contentContainerPerDistrict.area = new Vector4(0, 0, mainPanel.width, mainPanel.height - 80);

            m_StripMain.AddTab("SVMPerDistrict", tabPerDistrict.gameObject, contentContainerPerDistrict.gameObject);
            CreateSsdTabstrip(ref m_StripDistricts, null, contentContainerPerDistrict, 0);

            UIButton tabGeneralDistrictInfo = CreateTabTemplate();
            tabGeneralDistrictInfo.normalFgSprite = "ToolbarIconDistrict";
            tabGeneralDistrictInfo.tooltip = Locale.Get("SVM_GENERAL_DISTRICT_TAB");
            var districtSpecialTab = CreateContentTemplate(contentContainerPerDistrict.width - 10, contentContainerPerDistrict.height);
            m_StripDistricts.AddTab(name, tabGeneralDistrictInfo.gameObject, districtSpecialTab.gameObject).zOrder = 0;

            m_StripMain.selectedIndex = -1;
            m_StripBuilings.selectedIndex = -1;
            m_StripDistricts.selectedIndex = -1;

        }

        private static void CreateSsdTabstrip(ref UITabstrip strip, UIPanel titleLine, UIComponent parent, float offsetY = 0)
        {
            SVMUtils.createUIElement(out strip, parent.transform, "SVMTabstrip", new Vector4(5, offsetY, parent.width - 10, 40));

            var effectiveOffsetY = offsetY + strip.height + (titleLine?.height ?? 0);

            SVMUtils.createUIElement(out UITabContainer tabContainer, parent.transform, "SVMTabContainer", new Vector4(5, effectiveOffsetY + 5, parent.width - 10, parent.height - effectiveOffsetY - 10));
            strip.tabPages = tabContainer;

            UIButton tabTemplate = CreateTabTemplate();

            UIComponent scrollTemplate = CreateContentTemplate(parent.width - 10, parent.height - effectiveOffsetY - 10);

            foreach (var kv in ServiceSystemDefinition.availableDefinitions)
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
                strip.AddTab(name, tab, body);
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
            buildingNameLabel.area = new Vector4(175, 10, 200, 18);
            buildingNameLabel.textAlignment = UIHorizontalAlignment.Center;
            buildingNameLabel.text = Locale.Get("SVM_BUILDING_NAME_LABEL");

            SVMUtils.createUIElement(out UILabel vehicleCapacityLabel, titleLine.transform, "District");
            vehicleCapacityLabel.autoSize = false;
            vehicleCapacityLabel.area = new Vector4(375, 10, 200, 18);
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

        #region Sorting

        //private static int CompareDepotNames(UIComponent left, UIComponent right)
        //{
        //    TLMPublicTransportDepotInfo component = left.GetComponent<TLMPublicTransportDepotInfo>();
        //    TLMPublicTransportDepotInfo component2 = right.GetComponent<TLMPublicTransportDepotInfo>();
        //    return string.Compare(component.buidingName, component2.buidingName, StringComparison.InvariantCulture); //NaturalCompare(component.lineName, component2.lineName);
        //}

        //private static int CompareDepotDistricts(UIComponent left, UIComponent right)
        //{
        //    TLMPublicTransportDepotInfo component = left.GetComponent<TLMPublicTransportDepotInfo>();
        //    TLMPublicTransportDepotInfo component2 = right.GetComponent<TLMPublicTransportDepotInfo>();
        //    return string.Compare(component.districtName, component2.districtName, StringComparison.InvariantCulture); //NaturalCompare(component.lineName, component2.lineName);
        //}

        //private static int CompareNames(UIComponent left, UIComponent right)
        //{
        //    TLMPublicTransportLineInfoItem component = left.GetComponent<TLMPublicTransportLineInfoItem>();
        //    TLMPublicTransportLineInfoItem component2 = right.GetComponent<TLMPublicTransportLineInfoItem>();
        //    return string.Compare(component.lineName, component2.lineName, StringComparison.InvariantCulture); //NaturalCompare(component.lineName, component2.lineName);
        //}

        //private static int CompareLineNumbers(UIComponent left, UIComponent right)
        //{
        //    if (left == null || right == null)
        //        return 0;
        //    TLMPublicTransportLineInfoItem component = left.GetComponent<TLMPublicTransportLineInfoItem>();
        //    TLMPublicTransportLineInfoItem component2 = right.GetComponent<TLMPublicTransportLineInfoItem>();
        //    if (component == null || component2 == null)
        //        return 0;
        //    return component.lineNumber.CompareTo(component2.lineNumber);
        //}

        //private static int CompareStops(UIComponent left, UIComponent right)
        //{
        //    TLMPublicTransportLineInfoItem component = left.GetComponent<TLMPublicTransportLineInfoItem>();
        //    TLMPublicTransportLineInfoItem component2 = right.GetComponent<TLMPublicTransportLineInfoItem>();
        //    return NaturalCompare(component2.stopCounts, component.stopCounts);
        //}

        //private static int CompareVehicles(UIComponent left, UIComponent right)
        //{
        //    TLMPublicTransportLineInfoItem component = left.GetComponent<TLMPublicTransportLineInfoItem>();
        //    TLMPublicTransportLineInfoItem component2 = right.GetComponent<TLMPublicTransportLineInfoItem>();
        //    return NaturalCompare(component2.vehicleCounts, component.vehicleCounts);
        //}

        //private static int ComparePassengers(UIComponent left, UIComponent right)
        //{
        //    TLMPublicTransportLineInfoItem component = left.GetComponent<TLMPublicTransportLineInfoItem>();
        //    TLMPublicTransportLineInfoItem component2 = right.GetComponent<TLMPublicTransportLineInfoItem>();
        //    return component2.passengerCountsInt.CompareTo(component.passengerCountsInt);
        //}
        //private static int NaturalCompare(string left, string right)
        //{
        //    return (int)typeof(PublicTransportDetailPanel).GetMethod("NaturalCompare", Redirector<TLMDepotAI>.allFlags).Invoke(null, new object[] { left, right });
        //}
        //private void OnNameSort()
        //{
        //    UIComponent uIComponent = this.m_Strip.tabContainer.components[this.m_Strip.selectedIndex].Find("Container");
        //    if (uIComponent.components.Count == 0)
        //        return;
        //    Quicksort(uIComponent.components, new Comparison<UIComponent>(CompareNames));
        //    this.m_LastSortCriterionLines = LineSortCriterion.NAME;
        //    uIComponent.Invalidate();
        //}

        //private void OnDepotNameSort()
        //{
        //    if (!m_isDepotView || m_isPrefixEditor)
        //        return;
        //    UIComponent uIComponent = this.m_Strip.tabContainer.components[this.m_Strip.selectedIndex].Find("Container");
        //    if (uIComponent.components.Count == 0)
        //        return;
        //    Quicksort(uIComponent.components, new Comparison<UIComponent>(CompareDepotNames));
        //    // m_LastSortCriterionDepot = DepotSortCriterion.NAME;
        //    uIComponent.Invalidate();
        //}

        //private void OnDepotDistrictSort()
        //{
        //    if (!m_isDepotView || m_isPrefixEditor)
        //        return;
        //    UIComponent uIComponent = this.m_Strip.tabContainer.components[this.m_Strip.selectedIndex].Find("Container");
        //    if (uIComponent.components.Count == 0)
        //        return;
        //    Quicksort(uIComponent.components, new Comparison<UIComponent>(CompareDepotDistricts));
        //    //m_LastSortCriterionDepot = DepotSortCriterion.DISTRICT;
        //    uIComponent.Invalidate();
        //}

        //private void OnStopSort()
        //{
        //    if (!m_isLineView)
        //        return;
        //    UIComponent uIComponent = this.m_Strip.tabContainer.components[this.m_Strip.selectedIndex].Find("Container");
        //    if (uIComponent.components.Count == 0)
        //        return;
        //    Quicksort(uIComponent.components, new Comparison<UIComponent>(CompareStops));
        //    this.m_LastSortCriterionLines = LineSortCriterion.STOP;
        //    uIComponent.Invalidate();
        //}

        //private void OnVehicleSort()
        //{
        //    if (!m_isLineView)
        //        return;
        //    UIComponent uIComponent = this.m_Strip.tabContainer.components[this.m_Strip.selectedIndex].Find("Container");
        //    if (uIComponent.components.Count == 0)
        //        return;
        //    Quicksort(uIComponent.components, new Comparison<UIComponent>(CompareVehicles));
        //    this.m_LastSortCriterionLines = LineSortCriterion.VEHICLE;
        //    uIComponent.Invalidate();
        //}

        //private void OnPassengerSort()
        //{
        //    if (!m_isLineView)
        //        return;
        //    UIComponent uIComponent = this.m_Strip.tabContainer.components[this.m_Strip.selectedIndex].Find("Container");
        //    if (uIComponent.components.Count == 0)
        //        return;
        //    Quicksort(uIComponent.components, new Comparison<UIComponent>(ComparePassengers));
        //    this.m_LastSortCriterionLines = LineSortCriterion.PASSENGER;
        //    uIComponent.Invalidate();
        //}

        //private void OnLineNumberSort()
        //{
        //    if (!m_isLineView)
        //        return;
        //    UIComponent uIComponent = this.m_Strip.tabContainer.components[this.m_Strip.selectedIndex].Find("Container");
        //    if (uIComponent.components.Count == 0)
        //        return;
        //    Quicksort(uIComponent.components, new Comparison<UIComponent>(CompareLineNumbers));
        //    this.m_LastSortCriterionLines = LineSortCriterion.LINE_NUMBER;
        //    uIComponent.Invalidate();
        //}

        public static void Quicksort(IList<UIComponent> elements, Comparison<UIComponent> comp)
        {
            Quicksort(elements, 0, elements.Count - 1, comp);
        }

        public static void Quicksort(IList<UIComponent> elements, int left, int right, Comparison<UIComponent> comp)
        {
            int i = left;
            int num = right;
            UIComponent y = elements[(left + right) / 2];
            while (i <= num)
            {
                while (comp(elements[i], y) < 0)
                {
                    i++;
                }
                while (comp(elements[num], y) > 0)
                {
                    num--;
                }
                if (i <= num)
                {
                    UIComponent value = elements[i];
                    elements[i] = elements[num];
                    elements[i].forceZOrder = i;
                    elements[num] = value;
                    elements[num].forceZOrder = num;
                    i++;
                    num--;
                }
            }
            if (left < num)
            {
                Quicksort(elements, left, num, comp);
            }
            if (i < right)
            {
                Quicksort(elements, i, right, comp);
            }
        }
        #endregion

        public void SetActiveTab(int idx)
        {
            this.m_StripMain.selectedIndex = idx;
        }

        public void RefreshLines()
        {

        }

        private static void RemoveExtraLines(int linesCount, ref UIComponent component)
        {
            while (component.components.Count > linesCount)
            {
                UIComponent uIComponent = component.components[linesCount];
                component.RemoveUIComponent(uIComponent);
                UnityEngine.Object.Destroy(uIComponent.gameObject);
            }
        }



        private void OnTabChanged(UIComponent c, int idx)
        {
        }

        private void CheckChangedFunction(UIComponent c, bool r)
        {
            if (!m_isChangingTab)
            {
                this.OnChangeVisibleAll(r);
            }
        }

        private void OnChangeVisibleAll(bool visible)
        {
            if (this.m_StripMain.selectedIndex < 0 && visible)
            {
                this.m_StripMain.selectedIndex = 0;
            }
            else if (this.m_StripMain.selectedIndex > -1 && this.m_StripMain.selectedIndex < this.m_StripMain.tabContainer.components.Count)
            {
                this.m_ToggleAllState[this.m_StripMain.selectedIndex] = visible;
                UIComponent uIComponent = this.m_StripMain.tabContainer.components[this.m_StripMain.selectedIndex].Find("Container");
                if (uIComponent != null)
                {
                    for (int i = 0; i < uIComponent.components.Count; i++)
                    {
                        UIComponent uIComponent2 = uIComponent.components[i];
                        if (uIComponent2 != null)
                        {
                            UICheckBox uICheckBox = uIComponent2.Find<UICheckBox>("LineVisible");
                            if (uICheckBox)
                            {
                                uICheckBox.isChecked = visible;
                            }
                        }
                    }
                }
                this.RefreshLines();
            }
            else if (visible)
            {
                this.RefreshLines();
            }
        }

        private void Update()
        {
            if (false && Singleton<TransportManager>.exists && this.m_Ready && this.m_LastLineCount != Singleton<TransportManager>.instance.m_lineCount)
            {
                this.RefreshLines();
            }
            if (this.m_LinesUpdated)
            {
                this.m_LinesUpdated = false;
                //if (this.m_LastSortCriterionLines != LineSortCriterion.DEFAULT)
                //{
                //    if (this.m_LastSortCriterionLines == LineSortCriterion.NAME)
                //    {
                //        this.OnNameSort();
                //    }
                //    else if (this.m_LastSortCriterionLines == LineSortCriterion.PASSENGER)
                //    {
                //        this.OnPassengerSort();
                //    }
                //    else if (this.m_LastSortCriterionLines == LineSortCriterion.STOP)
                //    {
                //        this.OnStopSort();
                //    }
                //    else if (this.m_LastSortCriterionLines == LineSortCriterion.VEHICLE)
                //    {
                //        this.OnVehicleSort();
                //    }
                //    else if (this.m_LastSortCriterionLines == LineSortCriterion.LINE_NUMBER)
                //    {
                //        this.OnLineNumberSort();
                //    }
                //}
                //else
                //{
                //    this.OnLineNumberSort();
                //}
            }
        }










    }


}
