using ColossalFramework;
using ColossalFramework.UI;
using Klyte.Commons;
using Klyte.Commons.Extensors;
using Klyte.Commons.UI;
using Klyte.Commons.Utils;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.TextureAtlas;
using Klyte.ServiceVehiclesManager.UI;
using Klyte.ServiceVehiclesManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager
{
    internal class SVMController : Singleton<SVMController>
    {
        public void Start()
        {
            KlyteModsPanel.instance.AddTab(ModTab.ServiceVehiclesManager, typeof(SVMServiceBuildingDetailPanel), SVMCommonTextureAtlas.instance.atlas, "ServiceVehiclesManagerIcon", "Service Vehicles Manager (v" + ServiceVehiclesManagerMod.version + ")");

            SVMUtils.createUIElement(out UIPanel buildingInfoParent, FindObjectOfType<UIView>().transform, "SVMBuildingInfoPanel", new Vector4(0, 0, 0, 1));

            buildingInfoParent.gameObject.AddComponent<SVMBuildingInfoPanel>();

            var typeTarg = typeof(Redirector<>);
            List<Type> instances = KlyteUtils.GetSubtypesRecursive(typeTarg, typeof(SVMController));

            foreach (Type t in instances)
            {
                SVMUtils.doLog($"ADD REDIR: [{t.Name}]");
                gameObject.AddComponent(t);
            }
        }

        public void OpenSVMPanel()
        {
            KlyteModsPanel.instance.OpenAt(ModTab.ServiceVehiclesManager);
        }
        public void CloseSVMPanel()
        {
            KCController.instance.CloseKCPanel();
        }


        public void Awake()
        {
            initNearLinesOnWorldInfoPanel();
        }

        private void initNearLinesOnWorldInfoPanel()
        {


            BuildingWorldInfoPanel[] panelList = GameObject.Find("UIView").GetComponentsInChildren<BuildingWorldInfoPanel>();
            SVMUtils.doLog("WIP LIST: [{0}]", string.Join(", ", panelList.Select(x => x.name).ToArray()));

            foreach (BuildingWorldInfoPanel wip in panelList)
            {
                SVMUtils.doLog("LOADING WIP HOOK FOR: {0}", wip.name);

                UIPanel parent = wip.gameObject.GetComponent<UIPanel>();

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
                    var maxCount = SVMBuildingUtils.GetMaxVehiclesBuilding(buildingId, ssd.vehicleType, ssd.level);
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
            saida.atlas = SVMCommonTextureAtlas.instance.atlas;
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