using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.UI;
using Klyte.ServiceVehiclesManager.Utils;
using Klyte.Commons.Extensors;
using Klyte.Commons.Utils;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager
{
    internal class SVMController : Singleton<SVMController>
    {
        internal static UITextureAtlas taSVM;
        private UIButton openSVMPanelButton;
        private SVMServiceBuildingDetailPanel m_listPanel;
        private UIPanel buildingInfoParent;

        public void destroy()
        {
            Destroy(openSVMPanelButton);
        }

        public void Start()
        {
            UITabstrip toolStrip = ToolsModifierControl.mainToolbar.GetComponentInChildren<UITabstrip>();
            openSVMPanelButton = toolStrip.AddTab();
            this.openSVMPanelButton.size = new Vector2(49f, 49f);
            this.openSVMPanelButton.name = "ServiceVehiclesManagerButton";
            this.openSVMPanelButton.tooltip = "Service Vehicles Manager (v" + ServiceVehiclesManagerMod.version + ")";
            this.openSVMPanelButton.relativePosition = new Vector3(0f, 5f);
            toolStrip.AddTab("ServiceVehiclesManagerButton", this.openSVMPanelButton.gameObject, null, null);
            openSVMPanelButton.atlas = taSVM;
            openSVMPanelButton.normalBgSprite = "ServiceVehiclesManagerIconSmall";
            openSVMPanelButton.focusedFgSprite = "ToolbarIconGroup6Focused";
            openSVMPanelButton.hoveredFgSprite = "ToolbarIconGroup6Hovered";
            this.openSVMPanelButton.eventButtonStateChanged += delegate (UIComponent c, UIButton.ButtonState s)
            {
                if (s == UIButton.ButtonState.Focused)
                {
                    internal_OpenSVMPanel();
                }
                else
                {
                    internal_CloseSVMPanel();
                }
            };
            m_listPanel = SVMServiceBuildingDetailPanel.Get();

            SVMUtils.createUIElement(out buildingInfoParent, FindObjectOfType<UIView>().transform, "SVMBuildingInfoPanel", new Vector4(0, 0, 0, 1));

            buildingInfoParent.gameObject.AddComponent<SVMBuildingInfoPanel>();
            var typeTarg = typeof(Redirector<>);
            List<Type> instances = GetSubtypesRecursive(typeTarg);

            foreach (Type t in instances)
            {
                gameObject.AddComponent(t);
            }
        }

        private static List<Type> GetSubtypesRecursive(Type typeTarg)
        {
            var classes = from t in Assembly.GetAssembly(typeof(SVMController)).GetTypes()
                          let y = t.BaseType
                          where t.IsClass && y != null && y.IsGenericType == typeTarg.IsGenericType && (y.GetGenericTypeDefinition() == typeTarg || y.BaseType == typeTarg)
                          select t;
            List<Type> result = new List<Type>();
            foreach (Type t in classes)
            {
                if (t.IsAbstract)
                {
                    result.AddRange(GetSubtypesRecursive(t));
                }
                else
                {
                    result.Add(t);
                }
            }
            return result;
        }

        public void Awake()
        {
            initNearLinesOnWorldInfoPanel();
            ServiceVehiclesManagerMod.instance.showVersionInfoPopup();
        }

        private void ToggleSVMPanel()
        {
            openSVMPanelButton.SimulateClick();
        }
        public void OpenSVMPanel()
        {
            if (!m_listPanel.GetComponent<UIPanel>().isVisible)
            {
                openSVMPanelButton.SimulateClick();
            }
        }
        public void CloseSVMPanel()
        {
            if (m_listPanel.GetComponent<UIPanel>().isVisible)
            {
                openSVMPanelButton.SimulateClick();
            }
        }

        private void internal_CloseSVMPanel()
        {
            m_listPanel.GetComponent<UIPanel>().isVisible = false;
            openSVMPanelButton.Unfocus();
            openSVMPanelButton.state = UIButton.ButtonState.Normal;
        }

        private void internal_OpenSVMPanel()
        {
            m_listPanel.GetComponent<UIPanel>().isVisible = true;
            SVMBuildingInfoPanel.instance.Hide();
        }

        private void initNearLinesOnWorldInfoPanel()
        {

            UIPanel parent = GameObject.Find("UIView").transform.GetComponentInChildren<CityServiceWorldInfoPanel>().gameObject.GetComponent<UIPanel>();

            if (parent == null)
                return;
            parent.eventVisibilityChanged += (component, value) =>
            {
                updateBuildingEditShortcutButton(parent);
            };
            parent.eventPositionChanged += (component, value) =>
            {
                updateBuildingEditShortcutButton(parent);
            };
        }

        private void updateBuildingEditShortcutButton(UIPanel parent)
        {
            if (parent != null)
            {
                UIButton buildingEditShortcut = parent.Find<UIButton>("SVMBuildingShortcut");
                if (!buildingEditShortcut)
                {
                    buildingEditShortcut = initBuildingEditOnWorldInfoPanel(parent);
                }
                var prop = typeof(WorldInfoPanel).GetField("m_InstanceID", System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Instance);
                ushort buildingId = ((InstanceID)(prop.GetValue(parent.gameObject.GetComponent<WorldInfoPanel>()))).Building;
                var ssds = ServiceSystemDefinition.from(Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingId].Info);

                byte count = 0;
                foreach (var ssd in ssds)
                {
                    var maxCount = SVMBuildingUtils.GetMaxVehiclesBuilding(buildingId, ssd.vehicleType);
                    if (maxCount > 0)
                    {
                        count++;
                        break;
                    }
                }
                buildingEditShortcut.isVisible = count > 0;
            }
        }
        private UIButton initBuildingEditOnWorldInfoPanel(UIPanel parent)
        {
            UIButton saida = parent.AddUIComponent<UIButton>();
            saida.relativePosition = new Vector3(-40, parent.height - 50);
            saida.atlas = taSVM;
            saida.width = 30;
            saida.height = 30;
            saida.name = "SVMBuildingShortcut";
            saida.color = new Color32(170, 170, 170, 255);
            saida.hoveredColor = Color.white;
            saida.tooltipLocaleID = "SVM_GOTO_BUILDING_CONFIG_EDIT";
            SVMUtils.initButtonSameSprite(saida, "ServiceVehiclesManagerIcon");
            saida.eventClick += (x, y) =>
            {
                var prop = typeof(WorldInfoPanel).GetField("m_InstanceID", BindingFlags.NonPublic | BindingFlags.Instance);
                ushort buildingId = ((InstanceID)(prop.GetValue(parent.gameObject.GetComponent<WorldInfoPanel>()))).Building;
                SVMBuildingInfoPanel.instance.openInfo(buildingId);
            };
            return saida;
        }
    }
}