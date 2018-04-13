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
using Klyte.Commons.UI;
using Klyte.Commons;

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
            KlyteModsPanel.instance.AddTab(ModTab.ServiceVehiclesManager, typeof(SVMServiceBuildingDetailPanel), taSVM, "ServiceVehiclesManagerIcon", "Service Vehicles Manager (v" + ServiceVehiclesManagerMod.version + ")");

            var typeTarg = typeof(Redirector<>);
            List<Type> instances = KlyteUtils.GetSubtypesRecursive(typeTarg, typeof(SVMController));

            foreach (Type t in instances)
            {
                gameObject.AddComponent(t);
            }
        }

        public void OpenSVMPanel()
        {
            KlyteModsPanel.instance.OpenAt(ModTab.Addresses);
        }
        public void CloseSVMPanel()
        {
            KCController.instance.CloseKCPanel();
        }


        public void Awake()
        {
            initNearLinesOnWorldInfoPanel();
            ServiceVehiclesManagerMod.instance.showVersionInfoPopup();
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