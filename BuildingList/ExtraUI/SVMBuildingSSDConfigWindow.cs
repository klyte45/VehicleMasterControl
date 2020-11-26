using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Klyte.Commons.Extensors;
using Klyte.Commons.UI;
using Klyte.Commons.Utils;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager.UI.ExtraUI
{
    internal abstract class SVMBuildingSSDConfigWindow<T> : MonoBehaviour where T : SVMSysDef<T>, new()
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

            ServiceSystemDefinition ssd = SingletonLite<T>.instance.GetSSD();
            bool allowColorChange = ssd.AllowColorChanging();
            if (allowColorChange)
            {
                KlyteMonoUtils.CreateUIElement(out UILabel lbl, m_mainPanel.transform, "DistrictColorLabel", new Vector4(5, m_mainPanel.height - 30, 120, 40));
                KlyteMonoUtils.LimitWidth(lbl, 120);
                lbl.autoSize = true;
                lbl.localeID = "K45_SVM_COLOR_LABEL";

                m_color = KlyteMonoUtils.CreateColorField(m_mainPanel);
                m_color.eventSelectedColorChanged += onChangeColor;

                KlyteMonoUtils.CreateUIElement(out UIButton resetColor, m_mainPanel.transform, "DistrictColorReset", new Vector4(m_mainPanel.width - 110, m_mainPanel.height - 35, 0, 0));
                KlyteMonoUtils.InitButton(resetColor, false, "ButtonMenu");
                KlyteMonoUtils.LimitWidth(resetColor, 100);
                resetColor.textPadding = new RectOffset(5, 5, 5, 2);
                resetColor.autoSize = true;
                resetColor.localeID = "K45_SVM_RESET_COLOR";
                resetColor.eventClick += onResetColor;
            }
            else
            {
                m_mainPanel.height -= 40;
            }
        }

        #region Actions
        private void onResetColor(UIComponent component, UIMouseEventParameter eventParam) => m_color.selectedColor = Color.clear;
        private void onChangeColor(UIComponent component, Color value)
        {
            if (m_isLoading)
            {
                return;
            }

            LogUtils.DoLog("onChangeColor");
            ServiceSystemDefinition ssd = SingletonLite<T>.instance.GetSSD();
            ISVMBuildingExtension ext = ssd.GetBuildingExtension();
            ushort buildingId = m_buildingInfo.buildingIdSel.Building;
            if (ext.GetIgnoreDistrict(buildingId))
            {
                ext.SetColor(buildingId, value);
            }
            else
            {
                ssd.GetDistrictExtension().SetColor(BuildingUtils.GetBuildingDistrict(buildingId), value);
            }
        }
        #endregion

        private void CreateRemoveUndesiredModelsButton()
        {
            KlyteMonoUtils.CreateUIElement<UIButton>(out UIButton removeUndesired, m_mainPanel.transform);
            removeUndesired.relativePosition = new Vector3(m_mainPanel.width - 25f, 10f);
            removeUndesired.textScale = 0.6f;
            removeUndesired.width = 20;
            removeUndesired.height = 20;
            removeUndesired.tooltip = Locale.Get("K45_SVM_REMOVE_UNWANTED_TOOLTIP");
            KlyteMonoUtils.InitButton(removeUndesired, true, "ButtonMenu");
            removeUndesired.name = "DeleteLineButton";
            removeUndesired.isVisible = true;
            removeUndesired.eventClick += (component, eventParam) => SVMTransportExtensionUtils.RemoveAllUnwantedVehicles();

            UISprite icon = removeUndesired.AddUIComponent<UISprite>();
            icon.relativePosition = new Vector3(2, 2);
            icon.width = 18;
            icon.height = 18;
            icon.spriteName = "K45_RemoveUnwantedIcon";
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

            KlyteMonoUtils.CreateUIElement(out m_title, m_mainPanel.transform);
            m_title.textAlignment = UIHorizontalAlignment.Center;
            m_title.autoSize = false;
            m_title.autoHeight = true;
            m_title.width = m_mainPanel.width - 30f;
            m_title.relativePosition = new Vector3(5, 5);
            m_title.textScale = 0.9f;
        }

        private void CreateScrollPanel()
        {
            m_uiHelper = KlyteMonoUtils.CreateScrollPanel(m_mainPanel, out m_scrollablePanel, out m_scrollbar, m_mainPanel.width - 20f, m_mainPanel.height - 90f, new Vector3(5, 45));
            m_scrollablePanel.eventMouseLeave += (x, y) =>
            {
                m_previewPanel.isVisible = false;
            };
        }

        private void BindParentChanges() => m_buildingInfo.EventOnBuildingSelChanged += OnBuildingChange;

        private void OnBuildingChange(ushort buildingId)
        {
            LogUtils.DoLog("EventOnBuildingSelChanged");
            m_isLoading = true;
            IEnumerable<ServiceSystemDefinition> ssds = ServiceSystemDefinition.from(Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingId].Info);
            if (!ssds.Contains(SingletonLite<T>.instance.GetSSD()))
            {
                m_mainPanel.isVisible = false;
                return;
            }
            m_mainPanel.isVisible = true;
            ServiceSystemDefinition ssd = SingletonLite<T>.instance.GetSSD();
            ISVMBuildingExtension ext = ssd.GetBuildingExtension();
            bool isCustomConfig = ext.GetIgnoreDistrict(buildingId);

            LogUtils.DoLog("ssd = {0}", ssd);

            List<string> selectedAssets;
            Color selectedColor;
            if (isCustomConfig)
            {
                selectedAssets = ext.GetAssetList(buildingId);
                selectedColor = ext.GetColor(buildingId);
            }
            else
            {
                ushort districtId = BuildingUtils.GetBuildingDistrict(buildingId);
                ISVMDistrictExtension distExt = ssd.GetDistrictExtension();
                selectedAssets = distExt.GetAssetList(districtId);
                selectedColor = distExt.GetColor(districtId);
            }
            foreach (string i in m_checkboxes.Keys)
            {
                m_checkboxes[i].isChecked = selectedAssets.Contains(i);
            }
            if (m_color != null)
            {
                m_color.selectedColor = selectedColor;
            }

            if (isCustomConfig)
            {
                m_title.text = string.Format(Locale.Get("K45_SVM_ASSET_SELECT_WINDOW_TITLE"), Singleton<BuildingManager>.instance.GetBuildingName(buildingId, default), ssd.NameForServiceSystem);
            }
            else
            {
                ushort districtId = BuildingUtils.GetBuildingDistrict(buildingId);
                if (districtId > 0)
                {
                    m_title.text = string.Format(Locale.Get("K45_SVM_ASSET_SELECT_WINDOW_TITLE_DISTRICT"), Singleton<DistrictManager>.instance.GetDistrictName(districtId), ssd.NameForServiceSystem);
                }
                else
                {
                    m_title.text = string.Format(Locale.Get("K45_SVM_ASSET_SELECT_WINDOW_TITLE_CITY"), ssd.NameForServiceSystem);
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
            foreach (string i in m_checkboxes.Keys)
            {
                UnityEngine.Object.Destroy(m_checkboxes[i].gameObject);
            }
            ServiceSystemDefinition ssd = SingletonLite<T>.instance.GetSSD();
            m_defaultAssets = ssd.GetBuildingExtension().GetAllBasicAssets();
            m_checkboxes = new Dictionary<string, UICheckBox>();

            LogUtils.DoLog("m_defaultAssets Size = {0} ({1})", m_defaultAssets?.Count, string.Join(",", m_defaultAssets.Keys?.ToArray() ?? new string[0]));
            foreach (string i in m_defaultAssets.Keys)
            {
                var checkbox = (UICheckBox) m_uiHelper.AddCheckbox(m_defaultAssets[i], false, (x) =>
                 {
                     ISVMBuildingExtension ext = SingletonLite<T>.instance.GetSSD().GetBuildingExtension();

                     ushort buildingId = m_buildingInfo.buildingIdSel.Building;
                     if (m_isLoading)
                     {
                         return;
                     }

                     if (x)
                     {
                         if (ext.GetIgnoreDistrict(buildingId))
                         {
                             ext.AddAsset(buildingId, i);
                         }
                         else
                         {
                             SingletonLite<T>.instance.GetSSD().GetDistrictExtension().AddAsset(BuildingUtils.GetBuildingDistrict(buildingId), i);
                         }
                     }
                     else
                     {
                         if (ext.GetIgnoreDistrict(buildingId))
                         {
                             ext.RemoveAsset(buildingId, i);
                         }
                         else
                         {
                             SingletonLite<T>.instance.GetSSD().GetDistrictExtension().RemoveAsset(BuildingUtils.GetBuildingDistrict(buildingId), i);
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
            KlyteMonoUtils.CreateUIElement(out m_previewPanel, m_mainPanel.transform);
            m_previewPanel.backgroundSprite = "GenericPanel";
            m_previewPanel.width = m_mainPanel.width + 100f;
            m_previewPanel.height = m_mainPanel.width;
            m_previewPanel.relativePosition = new Vector3(-50f, m_mainPanel.height);
            KlyteMonoUtils.CreateUIElement(out m_preview, m_previewPanel.transform);
            m_preview.size = m_previewPanel.size;
            m_preview.relativePosition = Vector3.zero;
            KlyteMonoUtils.CreateElement(out m_previewRenderer, m_mainPanel.transform);
            m_previewRenderer.Size = m_preview.size * 2f;
            m_preview.texture = m_previewRenderer.Texture;
            m_previewRenderer.Zoom = 3;
            m_previewRenderer.CameraRotation = 40;
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
                m_previewRenderer.CameraRotation -= 2;
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
            m_previewRenderer.RenderVehicle(m_lastInfo, (m_color?.selectedColor ?? Color.clear) == Color.clear ? Color.HSVToRGB(Math.Abs(m_previewRenderer.CameraRotation) / 360f, .5f, .5f) : m_color.selectedColor, true);
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
    //internal sealed class SVMBuildingSSDConfigWindowOutTra : SVMBuildingSSDConfigWindow<SVMSysDefOutTra> { }
    //internal sealed class SVMBuildingSSDConfigWindowOutShp : SVMBuildingSSDConfigWindow<SVMSysDefOutShp> { }
    //internal sealed class SVMBuildingSSDConfigWindowOutPln : SVMBuildingSSDConfigWindow<SVMSysDefOutPln> { }
    //internal sealed class SVMBuildingSSDConfigWindowOutCar : SVMBuildingSSDConfigWindow<SVMSysDefOutCar> { }
    internal sealed class SVMBuildingSSDConfigWindowBeaCar : SVMBuildingSSDConfigWindow<SVMSysDefBeaCar> { }
    internal sealed class SVMBuildingSSDConfigWindowPstCar : SVMBuildingSSDConfigWindow<SVMSysDefPstCar> { }
    internal sealed class SVMBuildingSSDConfigWindowPstTrk : SVMBuildingSSDConfigWindow<SVMSysDefPstTrk> { }
}
