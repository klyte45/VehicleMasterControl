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
    internal abstract class SVMBuildingSSDConfigWindow<T> : MonoBehaviour where T : SVMSysDef<T>
    {
        private UIPanel m_mainPanel;
        private UIHelperExtension m_uiHelper;
        private UILabel m_title;
        private SVMBuildingInfoPanel m_buildingInfo => SVMBuildingInfoPanel.instance;
        private UIColorField m_color;

        private UIScrollablePanel m_scrollablePanel;
        private UIScrollbar m_scrollbar;
        private AVOPreviewRenderer m_previewRenderer;
        private UITextureSprite m_preview;
        private UIPanel m_previewPanel;
        private VehicleInfo m_lastInfo;
        private Dictionary<string, string> m_defaultAssets = new Dictionary<string, string>();
        private Dictionary<string, UICheckBox> m_checkboxes = new Dictionary<string, UICheckBox>();
        private bool m_isLoading;

        private void Awake()
        {
            CreateMainPanel();

            CreateScrollPanel();

            SetPreviewWindow();

            BindParentChanges();

            CreateRemoveUndesiredModelsButton();

            PopulateCheckboxes();

            var ssd = Singleton<T>.instance.GetSSD();
            var extension = ssd.GetTransportExtension();
            bool allowColorChange = SVMConfigWarehouse.allowColorChanging(extension.ConfigIndexKey);
            if (allowColorChange)
            {
                SVMUtils.createUIElement(out UILabel lbl, m_mainPanel.transform, "DistrictColorLabel", new Vector4(5, m_mainPanel.height - 30, 120, 40));
                SVMUtils.LimitWidth(lbl, 120);
                lbl.autoSize = true;
                lbl.localeID = "SVM_COLOR_LABEL";

                m_color = KlyteUtils.CreateColorField(m_mainPanel);
                m_color.eventSelectedColorChanged += onChangeColor;

                SVMUtils.createUIElement(out UIButton resetColor, m_mainPanel.transform, "DistrictColorReset", new Vector4(m_mainPanel.width - 110, m_mainPanel.height - 35, 0, 0));
                SVMUtils.initButton(resetColor, false, "ButtonMenu");
                SVMUtils.LimitWidth(resetColor, 100);
                resetColor.textPadding = new RectOffset(5, 5, 5, 2);
                resetColor.autoSize = true;
                resetColor.localeID = "SVM_RESET_COLOR";
                resetColor.eventClick += onResetColor;
            }
            else
            {
                m_mainPanel.height -= 40;
            }
        }

        #region Actions
        private void onResetColor(UIComponent component, UIMouseEventParameter eventParam)
        {
            m_color.selectedColor = Color.clear;
        }
        private void onChangeColor(UIComponent component, Color value)
        {
            if (m_isLoading) return;
            SVMUtils.doLog("onChangeColor");
            var ssd = Singleton<T>.instance.GetSSD();
            var ext = ssd.GetTransportExtension();
            ushort buildingId = m_buildingInfo.buildingIdSel.Building;
            if (ext.GetIgnoreDistrict(buildingId))
            {
                ext.SetColorBuilding(buildingId, value);
            }
            else
            {
                ext.SetColorDistrict(SVMUtils.GetBuildingDistrict(buildingId), value);
            }
        }
        #endregion

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
            m_mainPanel = gameObject.AddComponent<UIPanel>();
            m_mainPanel.Hide();
            m_mainPanel.width = 250;
            m_mainPanel.height = 350;
            m_mainPanel.zOrder = 50;
            m_mainPanel.color = new Color32(255, 255, 255, 255);
            m_mainPanel.backgroundSprite = "MenuPanel2";
            m_mainPanel.name = "AssetSelectorWindow_" + typeof(T).Name;
            m_mainPanel.autoLayoutPadding = new RectOffset(5, 5, 10, 10);
            m_mainPanel.autoLayout = false;
            m_mainPanel.useCenter = true;
            m_mainPanel.wrapLayout = false;
            m_mainPanel.canFocus = true;
            GetComponentInParent<UIComponent>().eventVisibilityChanged += (component, value) =>
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
        }

        private void CreateScrollPanel()
        {
            m_uiHelper = SVMUtils.CreateScrollPanel(m_mainPanel, out m_scrollablePanel, out m_scrollbar, m_mainPanel.width - 20f, m_mainPanel.height - 90f, new Vector3(5, 45));
            m_scrollablePanel.eventMouseLeave += (x, y) =>
            {
                m_previewPanel.isVisible = false;
            };
        }

        private void BindParentChanges()
        {
            m_buildingInfo.EventOnBuildingSelChanged += OnBuildingChange;
        }

        private void OnBuildingChange(ushort buildingId)
        {
            SVMUtils.doLog("EventOnBuildingSelChanged");
            m_isLoading = true;
            IEnumerable<ServiceSystemDefinition> ssds = ServiceSystemDefinition.from(Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingId].Info);
            if (!ssds.Contains(Singleton<T>.instance.GetSSD()))
            {
                m_mainPanel.isVisible = false;
                return;
            }
            m_mainPanel.isVisible = true;
            var ssd = Singleton<T>.instance.GetSSD();
            var ext = ssd.GetTransportExtension();
            bool isCustomConfig = ext.GetIgnoreDistrict(buildingId);

            SVMUtils.doLog("ssd = {0}", ssd);

            List<string> selectedAssets;
            Color selectedColor;
            if (isCustomConfig)
            {
                selectedAssets = ext.GetAssetListBuilding(buildingId);
                selectedColor = ext.GetColorBuilding(buildingId);
            }
            else
            {
                var districtId = SVMUtils.GetBuildingDistrict(buildingId);
                selectedAssets = ext.GetAssetListDistrict(districtId);
                selectedColor = ext.GetColorDistrict(districtId);
            }
            foreach (var i in m_checkboxes.Keys)
            {
                m_checkboxes[i].isChecked = selectedAssets.Contains(i);
            }
            if (m_color != null)
            {
                m_color.selectedColor = selectedColor;
            }

            if (isCustomConfig)
            {
                m_title.text = string.Format(Locale.Get("SVM_ASSET_SELECT_WINDOW_TITLE"), Singleton<BuildingManager>.instance.GetBuildingName(buildingId, default(InstanceID)), SVMConfigWarehouse.getNameForServiceSystem(ssd.toConfigIndex()));
            }
            else
            {
                var districtId = SVMUtils.GetBuildingDistrict(buildingId);
                if (districtId > 0)
                {
                    m_title.text = string.Format(Locale.Get("SVM_ASSET_SELECT_WINDOW_TITLE_DISTRICT"), Singleton<DistrictManager>.instance.GetDistrictName(SVMUtils.GetBuildingDistrict(buildingId)), SVMConfigWarehouse.getNameForServiceSystem(ssd.toConfigIndex()));
                }
                else
                {
                    m_title.text = string.Format(Locale.Get("SVM_ASSET_SELECT_WINDOW_TITLE_CITY"), SVMConfigWarehouse.getNameForServiceSystem(ssd.toConfigIndex()));
                }
            }

            m_isLoading = false;
            m_previewPanel.isVisible = false;
        }


        private void CreateModelCheckBox(string i, UICheckBox checkbox)
        {
            checkbox.eventMouseEnter += (x, y) =>
            {
                m_lastInfo = PrefabCollection<VehicleInfo>.FindLoaded(i);
                redrawModel();
            };
        }

        private void PopulateCheckboxes()
        {
            foreach (var i in m_checkboxes.Keys)
            {
                UnityEngine.Object.Destroy(m_checkboxes[i].gameObject);
            }
            var ssd = Singleton<T>.instance.GetSSD();
            m_defaultAssets = ssd.GetTransportExtension().GetAllBasicAssets();
            m_checkboxes = new Dictionary<string, UICheckBox>();

            SVMUtils.doLog("m_defaultAssets Size = {0} ({1})", m_defaultAssets?.Count, string.Join(",", m_defaultAssets.Keys?.ToArray() ?? new string[0]));
            foreach (var i in m_defaultAssets.Keys)
            {
                var checkbox = (UICheckBox)m_uiHelper.AddCheckbox(m_defaultAssets[i], false, (x) =>
                {
                    var ext = Singleton<T>.instance.GetSSD().GetTransportExtension();

                    ushort buildingId = m_buildingInfo.buildingIdSel.Building;
                    if (m_isLoading) return;
                    if (x)
                    {
                        if (ext.GetIgnoreDistrict(buildingId))
                        {
                            ext.AddAssetBuilding(buildingId, i);
                        }
                        else
                        {
                            ext.AddAssetDistrict(SVMUtils.GetBuildingDistrict(buildingId), i);
                        }
                    }
                    else
                    {
                        if (ext.GetIgnoreDistrict(buildingId))
                        {
                            ext.RemoveAssetBuilding(buildingId, i);
                        }
                        else
                        {
                            ext.RemoveAssetDistrict(SVMUtils.GetBuildingDistrict(buildingId), i);
                        }
                    }
                });
                CreateModelCheckBox(i, checkbox);
                checkbox.label.tooltip = checkbox.label.text;
                checkbox.label.textScale = 0.9f;
                checkbox.label.transform.localScale = new Vector3(Math.Min((m_mainPanel.width - 50) / checkbox.label.width, 1), 1);
                m_checkboxes[i] = checkbox;
            }
        }

        private void SetPreviewWindow()
        {
            SVMUtils.createUIElement(out m_previewPanel, m_mainPanel.transform);
            m_previewPanel.backgroundSprite = "GenericPanel";
            m_previewPanel.width = m_mainPanel.width + 100f;
            m_previewPanel.height = m_mainPanel.width;
            m_previewPanel.relativePosition = new Vector3(-50f, m_mainPanel.height);
            SVMUtils.createUIElement(out m_preview, m_previewPanel.transform);
            this.m_preview.size = m_previewPanel.size;
            this.m_preview.relativePosition = Vector3.zero;
            SVMUtils.createElement(out m_previewRenderer, m_mainPanel.transform);
            this.m_previewRenderer.size = this.m_preview.size * 2f;
            this.m_preview.texture = this.m_previewRenderer.texture;
            m_previewRenderer.zoom = 3;
            m_previewRenderer.cameraRotation = 40;
            m_previewPanel.isVisible = false;
        }

        public void Update()
        {
            if (m_color != null)
            {
                m_color.area = new Vector4(80, 319, 20, 20);
            }
            if (m_lastInfo != default(VehicleInfo) && m_previewPanel.isVisible)
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
            m_previewRenderer.RenderVehicle(m_lastInfo, (m_color?.selectedColor ?? Color.clear) == Color.clear ? Color.HSVToRGB(Math.Abs(m_previewRenderer.cameraRotation) / 360f, .5f, .5f) : m_color.selectedColor, true);
        }
    }
    internal sealed class SVMBuildingSSDConfigWindowDisCar : SVMBuildingSSDConfigWindow<SVMSysDefDisCar> { }
    internal sealed class SVMBuildingSSDConfigWindowDisHel : SVMBuildingSSDConfigWindow<SVMSysDefDisHel> { }
    internal sealed class SVMBuildingSSDConfigWindowFirCar : SVMBuildingSSDConfigWindow<SVMSysDefFirCar> { }
    internal sealed class SVMBuildingSSDConfigWindowFirHel : SVMBuildingSSDConfigWindow<SVMSysDefFirHel> { }
    internal sealed class SVMBuildingSSDConfigWindowGarCar : SVMBuildingSSDConfigWindow<SVMSysDefGarCar> { }
    internal sealed class SVMBuildingSSDConfigWindowGbcCar : SVMBuildingSSDConfigWindow<SVMSysDefGbcCar> { }
    internal sealed class SVMBuildingSSDConfigWindowHcrCar : SVMBuildingSSDConfigWindow<SVMSysDefHcrCar> { }
    internal sealed class SVMBuildingSSDConfigWindowHcrHel : SVMBuildingSSDConfigWindow<SVMSysDefHcrHel> { }
    internal sealed class SVMBuildingSSDConfigWindowPolCar : SVMBuildingSSDConfigWindow<SVMSysDefPolCar> { }
    internal sealed class SVMBuildingSSDConfigWindowPolHel : SVMBuildingSSDConfigWindow<SVMSysDefPolHel> { }
    internal sealed class SVMBuildingSSDConfigWindowRoaCar : SVMBuildingSSDConfigWindow<SVMSysDefRoaCar> { }
    internal sealed class SVMBuildingSSDConfigWindowWatCar : SVMBuildingSSDConfigWindow<SVMSysDefWatCar> { }
    internal sealed class SVMBuildingSSDConfigWindowPriCar : SVMBuildingSSDConfigWindow<SVMSysDefPriCar> { }
    internal sealed class SVMBuildingSSDConfigWindowDcrCar : SVMBuildingSSDConfigWindow<SVMSysDefDcrCar> { }
    internal sealed class SVMBuildingSSDConfigWindowTaxCar : SVMBuildingSSDConfigWindow<SVMSysDefTaxCar> { }
    internal sealed class SVMBuildingSSDConfigWindowCcrCcr : SVMBuildingSSDConfigWindow<SVMSysDefCcrCcr> { }
    internal sealed class SVMBuildingSSDConfigWindowSnwCar : SVMBuildingSSDConfigWindow<SVMSysDefSnwCar> { }
    internal sealed class SVMBuildingSSDConfigWindowRegTra : SVMBuildingSSDConfigWindow<SVMSysDefRegTra> { }
    internal sealed class SVMBuildingSSDConfigWindowRegShp : SVMBuildingSSDConfigWindow<SVMSysDefRegShp> { }
    internal sealed class SVMBuildingSSDConfigWindowRegPln : SVMBuildingSSDConfigWindow<SVMSysDefRegPln> { }
    internal sealed class SVMBuildingSSDConfigWindowCrgTra : SVMBuildingSSDConfigWindow<SVMSysDefCrgTra> { }
    internal sealed class SVMBuildingSSDConfigWindowCrgShp : SVMBuildingSSDConfigWindow<SVMSysDefCrgShp> { }
    internal sealed class SVMBuildingSSDConfigWindowOutTra : SVMBuildingSSDConfigWindow<SVMSysDefOutTra> { }
    internal sealed class SVMBuildingSSDConfigWindowOutShp : SVMBuildingSSDConfigWindow<SVMSysDefOutShp> { }
    internal sealed class SVMBuildingSSDConfigWindowOutPln : SVMBuildingSSDConfigWindow<SVMSysDefOutPln> { }
    internal sealed class SVMBuildingSSDConfigWindowOutCar : SVMBuildingSSDConfigWindow<SVMSysDefOutCar> { }
}
