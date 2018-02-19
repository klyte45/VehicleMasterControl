using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.UI;
using Klyte.Extensions;
using System;
using System.Linq;
using UnityEngine;
using SVMCW = Klyte.ServiceVehiclesManager.SVMConfigWarehouse;
using Klyte.TransportLinesManager.Extensors;
using System.Collections.Generic;
using ColossalFramework.Globalization;
using Klyte.TransportLinesManager.LineList;
using Klyte.TransportLinesManager.Extensors.BuildingAIExt;
using Klyte.TransportLinesManager.Utils;
using Klyte.TransportLinesManager.UI;
using Klyte.TransportLinesManager.Extensors.TransportTypeExt;
using Klyte.ServiceVehiclesManager.UI;
using Klyte.ServiceVehiclesManager.Utils;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.Overrides;
using Klyte.ServiceVehiclesManager.UI.ExtraUI;

namespace Klyte.ServiceVehiclesManager.UI
{
    internal class SVMBuildingInfoPanel : MonoBehaviour
    {
        public static SVMBuildingInfoPanel instance;
        //line info	
        public UIPanel buildingInfoPanel => m_buildingInfoPanel;
        private UIPanel m_buildingInfoPanel;
        private InstanceID m_buildingIdSelecionado;
        private CameraController m_CameraController;
        private string lastDepotName;
        private UILabel vehiclesInUseLabel;
        private UIButton buildingTypeIcon;
        private UISprite buildingTypeIconFg;
        private UILabel upkeepCost;
        private UITextField buildingNameField;
        private TLMWorkerChartPanel workerChart;
        private UICheckBox m_ignoreDistrict;
        private UIHelperExtension m_uiHelper;
        public event OnButtonSelect<ushort> EventOnBuildingSelChanged;

        public Transform panelTransform
        {
            get {
                return m_buildingInfoPanel.transform;
            }
        }

        public GameObject panelGameObject
        {
            get {
                try
                {
                    return m_buildingInfoPanel.gameObject;
                }
                catch
                {
                    return null;
                }
            }
        }

        public bool isVisible => m_buildingInfoPanel.isVisible;


        public InstanceID buildingIdSel => m_buildingIdSelecionado;

        public CameraController cameraController => m_CameraController;

        public void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
            GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
            if (gameObject != null)
            {
                m_CameraController = gameObject.GetComponent<CameraController>();
            }
            createInfoView();
            CreateIgnoreDistrictOption();

            SVMUtils.createUIElement(out UIPanel confContainer, panelTransform, "SubConfigContainer", new Vector4(m_buildingInfoPanel.width, 0, 0, m_buildingInfoPanel.height));
            confContainer.autoLayout = true;
            confContainer.autoLayoutDirection = LayoutDirection.Horizontal;
            confContainer.wrapLayout = false;
            confContainer.height = m_buildingInfoPanel.height;
            confContainer.clipChildren = false;

            foreach (var kv in ServiceSystemDefinition.sysDefinitions)
            {
                Type targetType = KlyteUtils.GetImplementationForGenericType(typeof(SVMBuildingSSDConfigWindow<>), kv.Value);
                SVMUtils.createElement(targetType, confContainer.transform);
            }

        }


        private void CreateIgnoreDistrictOption()
        {
            m_ignoreDistrict = m_uiHelper.AddCheckboxLocale("SVM_IGNORE_DISTRICT_CONFIG", false);
            m_ignoreDistrict.relativePosition = new Vector3(5f, 150f);
            m_ignoreDistrict.eventCheckChanged += delegate (UIComponent comp, bool value)
            {
                if (Singleton<BuildingManager>.exists && m_buildingIdSelecionado.Building != 0)
                {
                    var ssds = ServiceSystemDefinition.from(Singleton<BuildingManager>.instance.m_buildings.m_buffer[m_buildingIdSelecionado.Building].Info);
                    foreach (var ssd in ssds)
                    {
                        var ext = ssd.GetTransportExtension();
                        ext.SetIgnoreDistrict(m_buildingIdSelecionado.Building, value);
                    }
                    EventOnBuildingSelChanged?.Invoke(m_buildingIdSelecionado.Building);
                }
            };
            m_ignoreDistrict.label.textScale = 0.9f;
        }


        private void Show() => m_buildingInfoPanel.Show();

        public void Hide()
        {
            m_buildingInfoPanel.Hide();
            SVMServiceBuildingDetailPanel.Get().OpenAt(ServiceSystemDefinition.from(Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingIdSel.Building].Info).First());
        }




        //ACOES
        private void saveBuildingName(UITextField u)
        {
            string value = u.text;

            Singleton<BuildingManager>.instance.StartCoroutine(SVMUtils.setBuildingName(m_buildingIdSelecionado.Building, value, () =>
            {
                buildingNameField.text = Singleton<BuildingManager>.instance.GetBuildingName(m_buildingIdSelecionado.Building, default(InstanceID));
                EventOnBuildingSelChanged?.Invoke(m_buildingIdSelecionado.Building);
            }));
        }

        private void createInfoView()
        {
            //line info painel

            SVMUtils.createUIElement(out m_buildingInfoPanel, gameObject.transform);
            m_buildingInfoPanel.Hide();
            m_buildingInfoPanel.relativePosition = new Vector3(394.0f, 70.0f);
            m_buildingInfoPanel.width = 650;
            m_buildingInfoPanel.height = 290;
            m_buildingInfoPanel.zOrder = 50;
            m_buildingInfoPanel.color = new Color32(255, 255, 255, 255);
            m_buildingInfoPanel.backgroundSprite = "MenuPanel2";
            m_buildingInfoPanel.name = "BuildingInfoPanel";
            m_buildingInfoPanel.autoLayoutPadding = new RectOffset(5, 5, 10, 10);
            m_buildingInfoPanel.autoLayout = false;
            m_buildingInfoPanel.useCenter = true;
            m_buildingInfoPanel.wrapLayout = false;
            m_buildingInfoPanel.canFocus = true;
            SVMUtils.createDragHandle(m_buildingInfoPanel, m_buildingInfoPanel, 35f);



            SVMUtils.createUIElement(out buildingTypeIcon, m_buildingInfoPanel.transform);
            buildingTypeIcon.autoSize = false;
            buildingTypeIcon.relativePosition = new Vector3(10f, 7f);
            buildingTypeIcon.width = 30;
            buildingTypeIcon.height = 30;
            buildingTypeIcon.name = "BuildingTypeIcon";
            buildingTypeIcon.clipChildren = true;
            buildingTypeIcon.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
            SVMUtils.createDragHandle(buildingTypeIcon, m_buildingInfoPanel);

            SVMUtils.createUIElement(out buildingTypeIconFg, buildingTypeIcon.transform);
            buildingTypeIconFg.autoSize = false;
            buildingTypeIconFg.relativePosition = new Vector3(0, 0);
            buildingTypeIconFg.width = 30;
            buildingTypeIconFg.height = 30;
            buildingTypeIconFg.name = "BuildingTypeIconFg";
            buildingTypeIconFg.clipChildren = true;
            buildingTypeIconFg.atlas = SVMController.taSVM;
            SVMUtils.createDragHandle(buildingTypeIconFg, m_buildingInfoPanel);

            SVMUtils.createUIElement(out buildingNameField, m_buildingInfoPanel.transform);
            buildingNameField.autoSize = false;
            buildingNameField.relativePosition = new Vector3(160f, 10f);
            buildingNameField.horizontalAlignment = UIHorizontalAlignment.Center;
            buildingNameField.text = "NOME";
            buildingNameField.width = 450;
            buildingNameField.height = 25;
            buildingNameField.name = "BuildingNameLabel";
            buildingNameField.maxLength = 256;
            buildingNameField.textScale = 1.5f;
            SVMUtils.uiTextFieldDefaults(buildingNameField);
            buildingNameField.eventGotFocus += (component, eventParam) =>
            {
                lastDepotName = buildingNameField.text;
            };
            buildingNameField.eventTextSubmitted += (component, eventParam) =>
            {
                if (lastDepotName != buildingNameField.text)
                {
                    saveBuildingName(buildingNameField);
                }
            };

            SVMUtils.createUIElement(out vehiclesInUseLabel, m_buildingInfoPanel.transform);
            vehiclesInUseLabel.autoSize = false;
            vehiclesInUseLabel.relativePosition = new Vector3(10f, 60f);
            vehiclesInUseLabel.textAlignment = UIHorizontalAlignment.Left;
            vehiclesInUseLabel.text = "";
            vehiclesInUseLabel.width = 550;
            vehiclesInUseLabel.height = 25;
            vehiclesInUseLabel.name = "VehiclesInUseLabel";
            vehiclesInUseLabel.textScale = 0.8f;
            vehiclesInUseLabel.prefix = Locale.Get("SVM_VEHICLE_CAPACITY_LABEL") + ": ";

            SVMUtils.createUIElement(out upkeepCost, m_buildingInfoPanel.transform);
            upkeepCost.autoSize = false;
            upkeepCost.relativePosition = new Vector3(10f, 75);
            upkeepCost.textAlignment = UIHorizontalAlignment.Left;
            upkeepCost.width = 250;
            upkeepCost.height = 25;
            upkeepCost.name = "UpkeepLabel";
            upkeepCost.textScale = 0.8f;

            SVMUtils.createUIElement(out UIButton voltarButton2, m_buildingInfoPanel.transform);
            voltarButton2.relativePosition = new Vector3(m_buildingInfoPanel.width - 33f, 5f);
            voltarButton2.width = 28;
            voltarButton2.height = 28;
            SVMUtils.initButton(voltarButton2, true, "DeleteLineButton");
            voltarButton2.name = "LineInfoCloseButton";
            voltarButton2.eventClick += closeBuildingInfo;

            workerChart = new TLMWorkerChartPanel(this.panelTransform, new Vector3(400f, 90f));
            m_uiHelper = new UIHelperExtension(m_buildingInfoPanel);
        }

        public void Update()
        {
            if (!this.m_buildingInfoPanel.isVisible) return;
            Building b = Singleton<BuildingManager>.instance.m_buildings.m_buffer[m_buildingIdSelecionado.Building];

            if (!(b.Info.GetAI() is BuildingAI basicAI))
            {
                closeBuildingInfo(null, null);
                return;
            }

            var ssds = ServiceSystemDefinition.from(b.Info);
            List<string> textVehicles = new List<string>();
            foreach (var ssd in ssds)
            {
                int count = 0;
                int cargo = 0;
                int capacity = 0;
                int inbound = 0;
                int outbound = 0;
                var ext = SVMBuildingAIOverrideUtils.getBuildingOverrideExtension(b.Info);
                SVMBuildingUtils.CalculateOwnVehicles(m_buildingIdSelecionado.Building, ref b, ext.GetManagedReasons(b.Info).Keys, ref count, ref cargo, ref capacity, ref inbound, ref outbound);
                int maxVehicles = (SVMUtils.GetPrivateField<int>(b.Info.GetAI(), ext.GetVehicleMaxCountField(ssd.vehicleType)) * SVMBuildingUtils.GetProductionRate(ref b) / 100);
                string maxVehiclesStr = maxVehicles > 0x3FFF ? "∞" : maxVehicles.ToString();
                textVehicles.Add($"{count}/{maxVehiclesStr} ({Locale.Get("SVM_VEHICLE_TYPE", ssd.vehicleType.ToString())})");
            }
            vehiclesInUseLabel.text = string.Join(" | ", textVehicles.ToArray());
            upkeepCost.text = LocaleFormatter.FormatUpkeep(basicAI.GetResourceRate(m_buildingIdSelecionado.Building, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)m_buildingIdSelecionado.Building], EconomyManager.Resource.Maintenance), false);

            uint num = Singleton<BuildingManager>.instance.m_buildings.m_buffer[m_buildingIdSelecionado.Building].m_citizenUnits;
            int num2 = 0;
            int num3 = 0;
            int unskill = 0;
            int oneSchool = 0;
            int twoSchool = 0;
            int threeSchool = 0;

            CitizenManager instance = Singleton<CitizenManager>.instance;
            while (num != 0u)
            {
                uint nextUnit = instance.m_units.m_buffer[(int)((UIntPtr)num)].m_nextUnit;
                if ((ushort)(instance.m_units.m_buffer[(int)((UIntPtr)num)].m_flags & CitizenUnit.Flags.Work) != 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        uint citizen = instance.m_units.m_buffer[(int)((UIntPtr)num)].GetCitizen(i);
                        if (citizen != 0u && !instance.m_citizens.m_buffer[(int)((UIntPtr)citizen)].Dead && (instance.m_citizens.m_buffer[(int)((UIntPtr)citizen)].m_flags & Citizen.Flags.MovingIn) == Citizen.Flags.None)
                        {
                            num3++;
                            switch (instance.m_citizens.m_buffer[(int)((UIntPtr)citizen)].EducationLevel)
                            {
                                case Citizen.Education.Uneducated:
                                    unskill++;
                                    break;
                                case Citizen.Education.OneSchool:
                                    oneSchool++;
                                    break;
                                case Citizen.Education.TwoSchools:
                                    twoSchool++;
                                    break;
                                case Citizen.Education.ThreeSchools:
                                    threeSchool++;
                                    break;
                            }
                        }
                    }
                }
                num = nextUnit;
                if (++num2 > 524288)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }

            workerChart.SetValues(new int[] { unskill, oneSchool, twoSchool, threeSchool }, new int[] { SVMUtils.GetPrivateField<int>(basicAI, "m_workPlaceCount0"), SVMUtils.GetPrivateField<int>(basicAI, "m_workPlaceCount1"), SVMUtils.GetPrivateField<int>(basicAI, "m_workPlaceCount2"), SVMUtils.GetPrivateField<int>(basicAI, "m_workPlaceCount3") });
        }



        public void closeBuildingInfo(UIComponent component, UIMouseEventParameter eventParam) => Hide();

        public void openInfo(ushort buildingID)
        {
            WorldInfoPanel.HideAllWorldInfoPanels();

            var ssds = ServiceSystemDefinition.from(Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info);
            var ext = ssds.First().GetTransportExtension();

            m_buildingIdSelecionado = default(InstanceID);
            m_ignoreDistrict.isChecked = ext.GetIgnoreDistrict(buildingID);
            m_buildingIdSelecionado.Building = buildingID;
            buildingNameField.text = Singleton<BuildingManager>.instance.GetBuildingName(buildingID, default(InstanceID));


            var configIdx = ssds.First().toConfigIndex();


            String bgIcon = SVMConfigWarehouse.getIconServiceSystem(configIdx);
            String fgIcon = SVMConfigWarehouse.getFgIconServiceSystem(configIdx);

            buildingTypeIcon.normalFgSprite = bgIcon;
            buildingTypeIconFg.spriteName = fgIcon;

            SVMController.instance.CloseSVMPanel();
            Show();
            EventOnBuildingSelChanged?.Invoke(buildingID);
        }
    }
}

