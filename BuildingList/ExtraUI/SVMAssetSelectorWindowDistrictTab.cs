using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Klyte.Extensions;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.UI;
using Klyte.ServiceVehiclesManager.Utils;
using Klyte.Commons.UI;
using Klyte.Commons.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager.UI.ExtraUI
{
    internal class SVMAssetSelectorWindowDistrictTab : MonoBehaviour
    {
        private UIScrollablePanel m_parent;
        private UIPanel m_mainPanel;
        private UIHelperExtension m_uiHelper;
        private UILabel m_title;
        private ServiceSystemDefinition m_system;
        private Color m_lastColor = Color.clear;
        public void setTabContent<T>(SVMTabControllerDistrictList<T> tabContent) where T : SVMSysDef<T>
        {
            if (m_system == null)
            {
                m_system = Singleton<T>.instance.GetSSD();
                m_parent = tabContent.mainPanel;
                CreateWindow();
                tabContent.eventOnDistrictSelectionChanged += onDistrictChanged;
                tabContent.eventOnColorDistrictChanged += (color) =>
                {
                    SVMUtils.doLog("eventOnColorDistrictChanged");
                    m_lastColor = color;
                };
            }
        }

        private void onDistrictChanged(int districtId)
        {
            if (districtId < 0) return;
            SVMUtils.doLog("eventOnDistrictSelectionChanged");
            m_isLoading = true;
            List<string> selectedAssets;
            selectedAssets = extension.GetAssetListDistrict((uint)districtId);

            SVMUtils.doLog("selectedAssets Size = {0} ({1})", selectedAssets?.Count, string.Join(",", selectedAssets?.ToArray() ?? new string[0]));
            foreach (var i in m_checkboxes.Keys)
            {
                m_checkboxes[i].isChecked = selectedAssets.Contains(i);
            }
            m_title.text = Locale.Get("SVM_DISTRICT_ASSET_SELECT_WINDOW_TITLE_PREFIX");

            m_isLoading = false;
        }

        private UIScrollablePanel m_scrollablePanel;
        private UIScrollbar m_scrollbar;
        private AVOPreviewRenderer m_previewRenderer;
        private UITextureSprite m_preview;
        private UIPanel m_previewPanel;
        private VehicleInfo m_lastInfo;
        private Dictionary<string, string> m_defaultAssets = new Dictionary<string, string>();
        private Dictionary<string, UICheckBox> m_checkboxes = new Dictionary<string, UICheckBox>();
        private bool m_isLoading;
        private ISVMTransportTypeExtension extension => m_system.GetTransportExtension();

        private void CreateWindow()
        {
            CreateMainPanel();

            CreateScrollPanel();

            SetPreviewWindow();

            CreateRemoveUndesiredModelsButton();

            CreateCheckboxes();
        }

        private void CreateCheckboxes()
        {
            foreach (var i in m_checkboxes?.Keys)
            {
                UnityEngine.Object.Destroy(m_checkboxes[i].gameObject);
            }
            m_defaultAssets = extension.GetAllBasicAssets();
            m_checkboxes = new Dictionary<string, UICheckBox>();

            SVMUtils.doLog("m_defaultAssets Size = {0} ({1})", m_defaultAssets?.Count, string.Join(",", m_defaultAssets.Keys?.ToArray() ?? new string[0]));
            foreach (var i in m_defaultAssets.Keys)
            {
                var checkbox = (UICheckBox)m_uiHelper.AddCheckbox(m_defaultAssets[i], false, (x) =>
                {
                    int districtIdx = SVMServiceBuildingDetailPanel.instance.getCurrentSelectedDistrictId();
                    if (m_isLoading || districtIdx < 0) return;
                    if (x)
                    {
                        extension.AddAssetDistrict((uint)districtIdx, i);
                    }
                    else
                    {
                        extension.RemoveAssetDistrict((uint)districtIdx, i);
                    }
                });
                CreateModelCheckBox(i, checkbox);
                checkbox.label.tooltip = checkbox.label.text;
                checkbox.label.textScale = 0.9f;
                checkbox.label.transform.localScale = new Vector3(Math.Min((m_mainPanel.width - 70) / checkbox.label.width, 1), 1);
                m_checkboxes[i] = checkbox;
            }
        }

        private void CreateRemoveUndesiredModelsButton()
        {
            SVMUtils.createUIElement<UIButton>(out UIButton removeUndesired, m_mainPanel.transform);
            removeUndesired.relativePosition = new Vector3(m_mainPanel.width - 25f, 10f);
            removeUndesired.textScale = 0.6f;
            removeUndesired.width = 20;
            removeUndesired.height = 20;
            removeUndesired.tooltip = Locale.Get("SVM_REMOVE_UNWANTED_TOOLTIP");
            SVMUtils.initButton(removeUndesired, true, "ButtonMenu");
            removeUndesired.name = "DeleteLineButton";
            removeUndesired.isVisible = true;
            removeUndesired.eventClick += (component, eventParam) =>
            {
                SVMTransportExtensionUtils.RemoveAllUnwantedVehicles();
            };

            var icon = removeUndesired.AddUIComponent<UISprite>();
            icon.relativePosition = new Vector3(2, 2);
            icon.atlas = SVMController.taSVM;
            icon.width = 18;
            icon.height = 18;
            icon.spriteName = "RemoveUnwantedIcon";
        }

        private void CreateMainPanel()
        {
            SVMUtils.createUIElement(out m_mainPanel, m_parent.transform);
            m_mainPanel.Hide();
            m_mainPanel.relativePosition = new Vector3(m_parent.width - 375f, 0.0f);
            m_mainPanel.width = 350;
            m_mainPanel.height = m_parent.height;
            m_mainPanel.zOrder = 50;
            m_mainPanel.color = new Color32(255, 255, 255, 255);
            m_mainPanel.name = "AssetSelectorWindow";
            m_mainPanel.autoLayoutPadding = new RectOffset(5, 5, 10, 10);
            m_mainPanel.autoLayout = false;
            m_mainPanel.useCenter = true;
            m_mainPanel.wrapLayout = false;
            m_mainPanel.canFocus = true;
            m_parent.eventVisibilityChanged += (component, value) =>
            {
                m_mainPanel.isVisible = value;
            };

            SVMUtils.createUIElement(out m_title, m_mainPanel.transform);
            m_title.textAlignment = UIHorizontalAlignment.Center;
            m_title.autoSize = false;
            m_title.autoHeight = true;
            m_title.width = m_mainPanel.width - 30f;
            m_title.relativePosition = new Vector3(5, 5);
            m_title.textScale = 0.9f;
            m_title.localeID = "SVM_DISTRICT_ASSET_SELECT_WINDOW_TITLE_PREFIX";
        }

        private void CreateScrollPanel()
        {
            SVMUtils.createUIElement(out m_scrollablePanel, m_mainPanel.transform);
            m_scrollablePanel.width = m_mainPanel.width - 20f;
            m_scrollablePanel.height = m_mainPanel.height - 50f;
            m_scrollablePanel.autoLayoutDirection = LayoutDirection.Vertical;
            m_scrollablePanel.autoLayoutStart = LayoutStart.TopLeft;
            m_scrollablePanel.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
            m_scrollablePanel.scrollPadding = new RectOffset(10, 10, 10, 10);
            m_scrollablePanel.autoLayout = true;
            m_scrollablePanel.clipChildren = true;
            m_scrollablePanel.relativePosition = new Vector3(5, 45);
            m_scrollablePanel.backgroundSprite = "ScrollbarTrack";

            SVMUtils.createUIElement(out UIPanel trackballPanel, m_mainPanel.transform);
            trackballPanel.width = 10f;
            trackballPanel.height = m_scrollablePanel.height;
            trackballPanel.autoLayoutDirection = LayoutDirection.Horizontal;
            trackballPanel.autoLayoutStart = LayoutStart.TopLeft;
            trackballPanel.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
            trackballPanel.autoLayout = true;
            trackballPanel.relativePosition = new Vector3(m_mainPanel.width - 15, 45);


            SVMUtils.createUIElement(out m_scrollbar, trackballPanel.transform);
            m_scrollbar.width = 10f;
            m_scrollbar.height = m_scrollbar.parent.height;
            m_scrollbar.orientation = UIOrientation.Vertical;
            m_scrollbar.pivot = UIPivotPoint.BottomLeft;
            m_scrollbar.AlignTo(trackballPanel, UIAlignAnchor.TopRight);
            m_scrollbar.minValue = 0f;
            m_scrollbar.value = 0f;
            m_scrollbar.incrementAmount = 25f;

            SVMUtils.createUIElement(out UISlicedSprite scrollBg, m_scrollbar.transform);
            scrollBg.relativePosition = Vector2.zero;
            scrollBg.autoSize = true;
            scrollBg.size = scrollBg.parent.size;
            scrollBg.fillDirection = UIFillDirection.Vertical;
            scrollBg.spriteName = "ScrollbarTrack";
            m_scrollbar.trackObject = scrollBg;

            SVMUtils.createUIElement(out UISlicedSprite scrollFg, scrollBg.transform);
            scrollFg.relativePosition = Vector2.zero;
            scrollFg.fillDirection = UIFillDirection.Vertical;
            scrollFg.autoSize = true;
            scrollFg.width = scrollFg.parent.width - 4f;
            scrollFg.spriteName = "ScrollbarThumb";
            m_scrollbar.thumbObject = scrollFg;
            m_scrollablePanel.verticalScrollbar = m_scrollbar;
            m_scrollablePanel.eventMouseWheel += delegate (UIComponent component, UIMouseEventParameter param)
            {
                m_scrollablePanel.scrollPosition += new Vector2(0f, Mathf.Sign(param.wheelDelta) * -1f * m_scrollbar.incrementAmount);
            };

            m_uiHelper = new UIHelperExtension(m_scrollablePanel);
        }

        private void CreateModelCheckBox(string i, UICheckBox checkbox)
        {
            checkbox.eventMouseEnter += (x, y) =>
            {
                m_lastInfo = PrefabCollection<VehicleInfo>.FindLoaded(i);
                redrawModel();
            };

        }

        private void SetPreviewWindow()
        {
            SVMUtils.createUIElement(out m_previewPanel, m_mainPanel.transform);
            m_previewPanel.backgroundSprite = "GenericPanel";
            m_previewPanel.width = m_parent.width - 400f;
            m_previewPanel.height = 170;
            m_previewPanel.relativePosition = new Vector3(-m_previewPanel.width, m_mainPanel.height - 175);
            SVMUtils.createUIElement(out m_preview, m_previewPanel.transform);
            this.m_preview.size = m_previewPanel.size;
            this.m_preview.relativePosition = Vector3.zero;
            SVMUtils.createElement(out m_previewRenderer, m_mainPanel.transform);
            this.m_previewRenderer.size = this.m_preview.size * 2f;
            this.m_preview.texture = this.m_previewRenderer.texture;
            m_previewRenderer.zoom = 3;
            m_previewRenderer.cameraRotation = 40;
        }

        private void Update()
        {
            if (m_lastInfo != default(VehicleInfo) && m_parent.isVisible)
            {
                this.m_previewRenderer.cameraRotation -= 2;
                redrawModel();
            }
        }

        private void redrawModel()
        {
            if (m_lastInfo == default(VehicleInfo))
            {
                m_previewPanel.isVisible = false;
                return;
            }
            m_previewPanel.isVisible = true;
            m_previewRenderer.RenderVehicle(m_lastInfo, m_lastColor == Color.clear ? Color.HSVToRGB(Math.Abs(m_previewRenderer.cameraRotation) / 360f, .5f, .5f) : m_lastColor, true);
        }
    }
}
