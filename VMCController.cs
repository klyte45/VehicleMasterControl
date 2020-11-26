using ColossalFramework;
using ColossalFramework.UI;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using Klyte.VehiclesMasterControl.Extensors.VehicleExt;
using Klyte.VehiclesMasterControl.UI;
using Klyte.VehiclesMasterControl.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Klyte.VehiclesMasterControl
{
    public class VMCController : BaseController<VehiclesMasterControlMod, VMCController>
    {

        public static readonly string FOLDER_NAME = "VehiclesMasterControl";
        public static readonly string FOLDER_PATH = FileUtils.BASE_FOLDER_PATH + FOLDER_NAME;

        protected override void StartActions()
        {
            KlyteMonoUtils.CreateUIElement(out UIPanel buildingInfoParent, FindObjectOfType<UIView>().transform, "VMCBuildingInfoPanel", new Vector4(0, 0, 0, 1));

            buildingInfoParent.gameObject.AddComponent<VMCBuildingInfoPanel>();
            initNearLinesOnWorldInfoPanel();
            m_districtCooldown = 10;
        }

        public void OpenVMCPanel() => VehiclesMasterControlMod.Instance.OpenPanelAtModTab();

        public event Action eventOnDistrictChanged;

        private static int m_districtCooldown;
        public static void OnDistrictChanged() => m_districtCooldown = 10;

        public void Update()
        {
            if (m_districtCooldown > 0)
            {
                m_districtCooldown--;
                if (m_districtCooldown == 0)
                {
                    eventOnDistrictChanged?.Invoke();
                }
            }
        }

        private void initNearLinesOnWorldInfoPanel()
        {


            BuildingWorldInfoPanel[] panelList = GameObject.Find("UIView").GetComponentsInChildren<BuildingWorldInfoPanel>();
            LogUtils.DoLog("WIP LIST: [{0}]", string.Join(", ", panelList.Select(x => x.name).ToArray()));

            foreach (BuildingWorldInfoPanel wip in panelList)
            {
                LogUtils.DoLog("LOADING WIP HOOK FOR: {0}", wip.name);

                UIPanel parent = wip.gameObject.GetComponent<UIPanel>();

                if (parent == null)
                {
                    return;
                }

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
                UIButton buildingEditShortcut = parent.Find<UIButton>("VMCBuildingShortcut");
                if (!buildingEditShortcut)
                {
                    buildingEditShortcut = initBuildingEditOnWorldInfoPanel(parent);
                }
                FieldInfo prop = typeof(WorldInfoPanel).GetField("m_InstanceID", System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Instance);
                ushort buildingId = ((InstanceID)(prop.GetValue(parent.gameObject.GetComponent<WorldInfoPanel>()))).Building;
                IEnumerable<ServiceSystemDefinition> ssds = ServiceSystemDefinition.from(Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingId].Info);

                byte count = 0;
                foreach (ServiceSystemDefinition ssd in ssds)
                {
                    int maxCount = VMCBuildingUtils.GetMaxVehiclesBuilding(ref BuildingManager.instance.m_buildings.m_buffer[buildingId], ssd.vehicleType, ssd.level);
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
            saida.width = 30;
            saida.height = 30;
            saida.name = "VMCBuildingShortcut";
            saida.color = new Color32(170, 170, 170, 255);
            saida.hoveredColor = Color.white;
            saida.tooltipLocaleID = "K45_VMC_GOTO_BUILDING_CONFIG_EDIT";
            KlyteMonoUtils.InitButtonSameSprite(saida, "VehiclesMasterControlIcon");
            saida.eventClick += (x, y) =>
            {
                FieldInfo prop = typeof(WorldInfoPanel).GetField("m_InstanceID", BindingFlags.NonPublic | BindingFlags.Instance);
                ushort buildingId = ((InstanceID)(prop.GetValue(parent.gameObject.GetComponent<WorldInfoPanel>()))).Building;
                VMCBuildingInfoPanel.instance.openInfo(buildingId);
            };
            return saida;
        }
    }
}