namespace Klyte.VehiclesMasterControl.UI
{
    using ColossalFramework;
    using ColossalFramework.Globalization;
    using ColossalFramework.Math;
    using ColossalFramework.UI;
    using Klyte.Commons.Utils;
    using Klyte.VehiclesMasterControl.Extensors.VehicleExt;
    using Klyte.VehiclesMasterControl.Overrides;
    using System.Linq;
    using UnityEngine;
    using Utils;
    internal abstract class VMCBuildingInfoItem<T> : ToolsModifierControl where T : VMCSysDef<T>, new()
    {
        private ushort m_buildingID;

        private bool m_secondary;

        private UILabel m_districtName;

        private UILabel m_directionLabel;

        private UILabel m_buildingName;

        private UITextField m_buildingNameField;

        private UILabel m_totalVehicles;

        private UIComponent m_Background;

        private Color32 m_BackgroundColor;

        private bool m_mouseIsOver;

        public ushort buildingId
        {
            get => m_buildingID;
            set => SetBuildingID(value);
        }

        public bool secondary
        {
            get => m_secondary;
            set => m_secondary = value;
        }
        public string districtName => m_districtName.text;

        public string buidingName => m_buildingName.text;

        public string prefixesServed => m_totalVehicles.text;

        private void SetBuildingID(ushort id) => m_buildingID = id;

        private ServiceSystemDefinition sysDef = SingletonLite<T>.instance.GetSSD();



        public void RefreshData()
        {
            if (Singleton<BuildingManager>.exists && transform.parent.gameObject.GetComponent<UIComponent>().isVisible)
            {
                GetComponent<UIComponent>().isVisible = true;
                Building b = Singleton<BuildingManager>.instance.m_buildings.m_buffer[m_buildingID];
                m_buildingName.text = Singleton<BuildingManager>.instance.GetBuildingName(m_buildingID, default(InstanceID));
                byte districtID = Singleton<DistrictManager>.instance.GetDistrict(b.m_position);
                string districtName = districtID == 0 ? Locale.Get("K45_VMC_DISTRICT_NONE") : Singleton<DistrictManager>.instance.GetDistrictName(districtID);
                m_districtName.text = districtName;

                int count = 0;
                int cargo = 0;
                int capacity = 0;
                int inbound = 0;
                int outbound = 0;
                ItemClass.Level defLevel = b.Info.m_class.m_level;
                VMCBuildingUtils.CalculateOwnVehicles(buildingId, ref b, ref count, ref cargo, ref capacity, ref inbound, ref outbound);

                int maxCount = VMCBuildingUtils.GetMaxVehiclesBuilding(buildingId, sysDef.vehicleType, sysDef.level);
                m_totalVehicles.prefix = count.ToString();
                m_totalVehicles.suffix = maxCount > 0x3FFF ? "∞" : maxCount.ToString();
                if (sysDef.outsideConnection)
                {
                    float angle = Vector2.zero.GetAngleToPoint(VectorUtils.XZ(b.m_position));
                    m_directionLabel.prefix = $"{angle:n1}°";
                    m_directionLabel.text = " - ";
                    m_directionLabel.suffix = CardinalPoint.GetCardinalPoint(angle).ToString();
                }
            }
        }

        public void SetBackgroundColor()
        {
            Color32 backgroundColor = m_BackgroundColor;
            backgroundColor.a = (byte) ((base.component.zOrder % 2 != 0) ? 127 : 255);
            if (m_mouseIsOver)
            {
                backgroundColor.r = (byte) Mathf.Min(255, backgroundColor.r * 3 >> 1);
                backgroundColor.g = (byte) Mathf.Min(255, backgroundColor.g * 3 >> 1);
                backgroundColor.b = (byte) Mathf.Min(255, backgroundColor.b * 3 >> 1);
            }
            m_Background.color = backgroundColor;
        }

        private void LateUpdate()
        {
            if (base.component.parent.isVisible)
            {
                RefreshData();
            }
        }

        private void Awake()
        {
            KlyteMonoUtils.ClearAllVisibilityEvents(GetComponent<UIPanel>());
            base.component.eventZOrderChanged += delegate (UIComponent c, int r)
            {
                SetBackgroundColor();
            };
            GameObject.Destroy(base.Find<UICheckBox>("LineVisible").gameObject);
            GameObject.Destroy(base.Find<UIColorField>("LineColor").gameObject);
            GameObject.Destroy(base.Find<UIPanel>("WarningIncomplete"));
            GameObject.Destroy(base.Find<UIPanel>("LineModelSelectorContainer"));

            m_buildingName = base.Find<UILabel>("LineName");
            m_buildingName.area = new Vector4(200, 2, 198, 35);
            m_buildingNameField = m_buildingName.Find<UITextField>("LineNameField");
            m_buildingNameField.maxLength = 256;
            m_buildingNameField.eventTextChanged += new PropertyChangedEventHandler<string>(OnRename);
            m_buildingName.eventMouseEnter += delegate (UIComponent c, UIMouseEventParameter r)
            {
                m_buildingName.backgroundSprite = "TextFieldPanelHovered";
            };
            m_buildingName.eventMouseLeave += delegate (UIComponent c, UIMouseEventParameter r)
            {
                m_buildingName.backgroundSprite = string.Empty;
            };
            m_buildingName.eventClick += delegate (UIComponent c, UIMouseEventParameter r)
            {
                m_buildingNameField.Show();
                m_buildingNameField.text = m_buildingName.text;
                m_buildingNameField.Focus();
            };
            m_buildingNameField.eventLeaveFocus += delegate (UIComponent c, UIFocusEventParameter r)
            {
                m_buildingNameField.Hide();
                Singleton<BuildingManager>.instance.StartCoroutine(BuildingUtils.SetBuildingName(m_buildingID, m_buildingNameField.text, () =>
                {
                    m_buildingName.text = m_buildingNameField.text;
                }));
            };

            GameObject.Destroy(base.Find<UICheckBox>("DayLine").gameObject);
            GameObject.Destroy(base.Find<UICheckBox>("NightLine").gameObject);
            GameObject.Destroy(base.Find<UICheckBox>("DayNightLine").gameObject);

            UICheckBox m_DayLine = base.Find<UICheckBox>("DayLine");

            GameObject.Destroy(base.Find<UICheckBox>("NightLine").gameObject);
            GameObject.Destroy(base.Find<UICheckBox>("DayNightLine").gameObject);
            GameObject.Destroy(m_DayLine.gameObject);

            m_districtName = base.Find<UILabel>("LineStops");
            m_districtName.size = new Vector2(175, 18);
            m_districtName.relativePosition = new Vector3(0, 10);
            m_districtName.pivot = UIPivotPoint.MiddleCenter;
            m_districtName.wordWrap = true;
            m_districtName.autoHeight = true;


            m_directionLabel = base.Find<UILabel>("LinePassengers");
            if (sysDef.outsideConnection)
            {
                m_directionLabel.size = new Vector2(200, 18);
                m_directionLabel.relativePosition = new Vector3(600, 10);
                m_directionLabel.wordWrap = true;
                m_directionLabel.autoHeight = true;
            }
            else
            {
                GameObject.Destroy(m_directionLabel.gameObject);
            }



            m_totalVehicles = base.Find<UILabel>("LineVehicles");
            m_totalVehicles.text = "/";

            m_Background = base.Find("Background");
            m_BackgroundColor = m_Background.color;
            m_mouseIsOver = false;
            base.component.eventMouseEnter += new MouseEventHandler(OnMouseEnter);
            base.component.eventMouseLeave += new MouseEventHandler(OnMouseLeave);
            GameObject.Destroy(base.Find<UIButton>("DeleteLine").gameObject);
            base.Find<UIButton>("ViewLine").eventClick += delegate (UIComponent c, UIMouseEventParameter r)
            {
                if (m_buildingID != 0)
                {
                    Vector3 position = Singleton<BuildingManager>.instance.m_buildings.m_buffer[m_buildingID].m_position;
                    var instanceID = default(InstanceID);
                    instanceID.Building = m_buildingID;
                    VMCBuildingInfoPanel.instance.openInfo(m_buildingID);
                    ToolsModifierControl.cameraController.m_unlimitedCamera = true;
                    ToolsModifierControl.cameraController.SetTarget(instanceID, position, true);
                }
            };
            base.component.eventVisibilityChanged += delegate (UIComponent c, bool v)
            {
                if (v)
                {
                    RefreshData();
                }
            };
        }

        private void OnMouseEnter(UIComponent comp, UIMouseEventParameter param)
        {
            if (!m_mouseIsOver)
            {
                m_mouseIsOver = true;
                SetBackgroundColor();
            }
        }

        private void OnMouseLeave(UIComponent comp, UIMouseEventParameter param)
        {
            if (m_mouseIsOver)
            {
                m_mouseIsOver = false;
                SetBackgroundColor();
            }
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        private void OnRename(UIComponent comp, string text) => Singleton<BuildingManager>.instance.StartCoroutine(BuildingUtils.SetBuildingName(m_buildingID, text, () => { }));

        private void OnLineChanged(ushort id)
        {
            if (id == m_buildingID)
            {
                RefreshData();
            }
        }

    }

    internal sealed class VMCBuildingInfoItemDisCar : VMCBuildingInfoItem<VMCSysDefDisCar> { }
    internal sealed class VMCBuildingInfoItemDisHel : VMCBuildingInfoItem<VMCSysDefDisHel> { }
    internal sealed class VMCBuildingInfoItemFirCar : VMCBuildingInfoItem<VMCSysDefFirCar> { }
    internal sealed class VMCBuildingInfoItemFirHel : VMCBuildingInfoItem<VMCSysDefFirHel> { }
    internal sealed class VMCBuildingInfoItemGarCar : VMCBuildingInfoItem<VMCSysDefGarCar> { }
    internal sealed class VMCBuildingInfoItemGbcCar : VMCBuildingInfoItem<VMCSysDefGbcCar> { }
    internal sealed class VMCBuildingInfoItemHcrCar : VMCBuildingInfoItem<VMCSysDefHcrCar> { }
    internal sealed class VMCBuildingInfoItemHcrHel : VMCBuildingInfoItem<VMCSysDefHcrHel> { }
    internal sealed class VMCBuildingInfoItemPolCar : VMCBuildingInfoItem<VMCSysDefPolCar> { }
    internal sealed class VMCBuildingInfoItemPolHel : VMCBuildingInfoItem<VMCSysDefPolHel> { }
    internal sealed class VMCBuildingInfoItemRoaCar : VMCBuildingInfoItem<VMCSysDefRoaCar> { }
    internal sealed class VMCBuildingInfoItemWatCar : VMCBuildingInfoItem<VMCSysDefWatCar> { }
    internal sealed class VMCBuildingInfoItemPriCar : VMCBuildingInfoItem<VMCSysDefPriCar> { }
    internal sealed class VMCBuildingInfoItemDcrCar : VMCBuildingInfoItem<VMCSysDefDcrCar> { }
    internal sealed class VMCBuildingInfoItemTaxCar : VMCBuildingInfoItem<VMCSysDefTaxCar> { }
    internal sealed class VMCBuildingInfoItemCcrCcr : VMCBuildingInfoItem<VMCSysDefCcrCcr> { }
    internal sealed class VMCBuildingInfoItemSnwCar : VMCBuildingInfoItem<VMCSysDefSnwCar> { }
    internal sealed class VMCBuildingInfoItemRegTra : VMCBuildingInfoItem<VMCSysDefRegTra> { }
    internal sealed class VMCBuildingInfoItemRegShp : VMCBuildingInfoItem<VMCSysDefRegShp> { }
    internal sealed class VMCBuildingInfoItemRegPln : VMCBuildingInfoItem<VMCSysDefRegPln> { }
    internal sealed class VMCBuildingInfoItemCrgTra : VMCBuildingInfoItem<VMCSysDefCrgTra> { }
    internal sealed class VMCBuildingInfoItemCrgShp : VMCBuildingInfoItem<VMCSysDefCrgShp> { }
    //internal sealed class VMCBuildingInfoItemOutTra : VMCBuildingInfoItem<VMCSysDefOutTra> { }
    //internal sealed class VMCBuildingInfoItemOutShp : VMCBuildingInfoItem<VMCSysDefOutShp> { }
    //internal sealed class VMCBuildingInfoItemOutPln : VMCBuildingInfoItem<VMCSysDefOutPln> { }
    //internal sealed class VMCBuildingInfoItemOutCar : VMCBuildingInfoItem<VMCSysDefOutCar> { }
    internal sealed class VMCBuildingInfoItemBeaCar : VMCBuildingInfoItem<VMCSysDefBeaCar> { }
    internal sealed class VMCBuildingInfoItemPstCar : VMCBuildingInfoItem<VMCSysDefPstCar> { }
    internal sealed class VMCBuildingInfoItemPstTrk : VMCBuildingInfoItem<VMCSysDefPstTrk> { }
}
