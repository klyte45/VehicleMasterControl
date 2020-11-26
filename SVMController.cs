using ColossalFramework;
using ColossalFramework.UI;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.UI;
using Klyte.ServiceVehiclesManager.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager
{
    public class SVMController : BaseController<ServiceVehiclesManagerMod, SVMController>
    {

        public static readonly string FOLDER_NAME = "ServiceVehicleManager";
        public static readonly string FOLDER_PATH = FileUtils.BASE_FOLDER_PATH + FOLDER_NAME;

        public void Start()
        {
            KlyteMonoUtils.CreateUIElement(out UIPanel buildingInfoParent, FindObjectOfType<UIView>().transform, "SVMBuildingInfoPanel", new Vector4(0, 0, 0, 1));

            buildingInfoParent.gameObject.AddComponent<SVMBuildingInfoPanel>();
        }

        public void OpenSVMPanel() => ServiceVehiclesManagerMod.Instance.OpenPanelAtModTab();


        public void Awake() => initNearLinesOnWorldInfoPanel();

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
                UIButton buildingEditShortcut = parent.Find<UIButton>("SVMBuildingShortcut");
                if (!buildingEditShortcut)
                {
                    buildingEditShortcut = initBuildingEditOnWorldInfoPanel(parent);
                }
                FieldInfo prop = typeof(WorldInfoPanel).GetField("m_InstanceID", System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Instance);
                ushort buildingId = ((InstanceID) (prop.GetValue(parent.gameObject.GetComponent<WorldInfoPanel>()))).Building;
                IEnumerable<ServiceSystemDefinition> ssds = ServiceSystemDefinition.from(Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingId].Info);

                byte count = 0;
                foreach (ServiceSystemDefinition ssd in ssds)
                {
                    int maxCount = SVMBuildingUtils.GetMaxVehiclesBuilding(buildingId, ssd.vehicleType, ssd.level);
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
            saida.name = "SVMBuildingShortcut";
            saida.color = new Color32(170, 170, 170, 255);
            saida.hoveredColor = Color.white;
            saida.tooltipLocaleID = "K45_SVM_GOTO_BUILDING_CONFIG_EDIT";
            KlyteMonoUtils.InitButtonSameSprite(saida, "ServiceVehiclesManagerIcon");
            saida.eventClick += (x, y) =>
            {
                FieldInfo prop = typeof(WorldInfoPanel).GetField("m_InstanceID", BindingFlags.NonPublic | BindingFlags.Instance);
                ushort buildingId = ((InstanceID) (prop.GetValue(parent.gameObject.GetComponent<WorldInfoPanel>()))).Building;
                SVMBuildingInfoPanel.instance.openInfo(buildingId);
            };
            return saida;
        }
    }
}