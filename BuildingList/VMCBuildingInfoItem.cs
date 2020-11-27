namespace Klyte.VehiclesMasterControl.UI
{
    using ColossalFramework;
    using ColossalFramework.Globalization;
    using ColossalFramework.Math;
    using ColossalFramework.UI;
    using Klyte.Commons.Extensors;
    using Klyte.Commons.Utils;
    using Klyte.VehiclesMasterControl.Extensors.VehicleExt;
    using UnityEngine;
    using Utils;
    internal abstract class VMCBuildingInfoItem<T> : ToolsModifierControl where T : VMCSysDef<T>, new()
    {
        private ushort m_buildingID;

        private UILabel m_districtName;

        private UILabel m_directionLabel;

        private UILabel m_buildingName;

        private UITextField m_buildingNameField;

        private UILabel m_totalVehicles;

        private UIPanel m_background;

        private Color32 m_BackgroundColor;

        private bool m_mouseIsOver;

        public ushort buildingId
        {
            get => m_buildingID;
            set => SetBuildingID(value);
        }

        public string districtName => m_districtName.text;

        public string buidingName => m_buildingName.text;

        public string prefixesServed => m_totalVehicles.text;

        private void SetBuildingID(ushort id) => m_buildingID = id;

        private readonly ServiceSystemDefinition sysDef = SingletonLite<T>.instance.GetSSD();
        private UIHelperExtension m_uIHelper;

        public void RefreshData()
        {
            if (Singleton<BuildingManager>.exists && transform.parent.gameObject.GetComponent<UIComponent>().isVisible)
            {
                GetComponent<UIComponent>().isVisible = true;
                ref Building b = ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[m_buildingID];
                m_buildingName.text = Singleton<BuildingManager>.instance.GetBuildingName(m_buildingID, default(InstanceID));
                byte districtID = Singleton<DistrictManager>.instance.GetDistrict(b.m_position);
                string districtName = districtID == 0 ? Locale.Get("K45_VMC_DISTRICT_NONE") : Singleton<DistrictManager>.instance.GetDistrictName(districtID);
                m_districtName.text = districtName;

                int count = 0;
                int cargo = 0;
                int capacity = 0;
                int inbound = 0;
                int outbound = 0;
                VMCBuildingUtils.CalculateOwnVehicles(ref b, ref count, ref cargo, ref capacity, ref inbound, ref outbound);

                int maxCount = Mathf.CeilToInt(VMCBuildingUtils.GetMaxVehiclesBuilding(ref b, sysDef.vehicleType, sysDef.level) * VMCBuildingUtils.GetProductionRate(ref b) / 100f);
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
            Color32 backgroundColor = new Color32(49, 52, 58, 255);
            backgroundColor.a = (byte)((base.component.zOrder % 2 != 0) ? 127 : 255);
            if (m_mouseIsOver)
            {
                backgroundColor.r = (byte)Mathf.Min(255, backgroundColor.r * 3 >> 1);
                backgroundColor.g = (byte)Mathf.Min(255, backgroundColor.g * 3 >> 1);
                backgroundColor.b = (byte)Mathf.Min(255, backgroundColor.b * 3 >> 1);
            }
            m_background.color = backgroundColor;
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
            AwakeBG();

            AwakeLineName();

            AwakeLabels();

            AwakeLineDetail();

            base.component.eventVisibilityChanged += delegate (UIComponent c, bool v)
            {
                if (v)
                {
                    RefreshData();
                }
            };
        }



        private void AwakeLineDetail()
        {
            KlyteMonoUtils.CreateUIElement(out UIButton view, transform, "ViewLine", new Vector4(784, 5, 28, 28));
            KlyteMonoUtils.InitButton(view, true, "LineDetailButton");
            view.eventClick += delegate (UIComponent c, UIMouseEventParameter r)
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
        }

        private void AwakeLabels()
        {
            KlyteMonoUtils.CreateUIElement(out m_districtName, transform);
            m_districtName.name = "districtName";
            m_districtName.autoSize = false;
            m_districtName.minimumSize = new Vector2(145, 18);
            m_districtName.relativePosition = new Vector3(10, 10);
            m_districtName.textAlignment = UIHorizontalAlignment.Center;

            if (sysDef.outsideConnection)
            {
                KlyteMonoUtils.CreateUIElement(out m_directionLabel, transform);
                m_directionLabel.autoSize = false;
                m_directionLabel.size = new Vector2(200, 18);
                m_directionLabel.relativePosition = new Vector3(655, 10);
                m_directionLabel.textAlignment = UIHorizontalAlignment.Center;
                m_directionLabel.wordWrap = true;
                m_directionLabel.autoHeight = true;
            }

            KlyteMonoUtils.CreateUIElement(out m_totalVehicles, transform);
            m_totalVehicles.autoSize = false;
            m_totalVehicles.text = "/";
            m_totalVehicles.width = 180;
            m_totalVehicles.textAlignment = UIHorizontalAlignment.Center;
            m_totalVehicles.relativePosition = new Vector3(500, 12);
        }

        private void AwakeLineName()
        {
            KlyteMonoUtils.CreateUIElement(out m_buildingName, transform, "LineName", new Vector4(200, 2, 198, 35));
            //  m_buildingName.textColor = ForegroundColor;
            m_buildingName.textAlignment = UIHorizontalAlignment.Center;
            m_buildingName.verticalAlignment = UIVerticalAlignment.Middle;
            m_buildingName.wordWrap = true;
            KlyteMonoUtils.CreateUIElement(out m_buildingNameField, transform, "LineNameField", new Vector4(200, 10, 198, 20));
            m_buildingNameField.maxLength = 256;
            m_buildingNameField.isVisible = false;
            m_buildingNameField.verticalAlignment = UIVerticalAlignment.Middle;
            m_buildingNameField.horizontalAlignment = UIHorizontalAlignment.Center;
            m_buildingNameField.selectionSprite = "EmptySprite";
            m_buildingNameField.builtinKeyNavigation = true;
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
                m_buildingName.Hide();
                m_buildingNameField.Show();
                m_buildingNameField.text = m_buildingName.text;
                m_buildingNameField.Focus();
            };
            m_buildingNameField.eventLeaveFocus += delegate (UIComponent c, UIFocusEventParameter r)
            {
                m_buildingNameField.Hide();
                m_buildingName.Show();
                Singleton<BuildingManager>.instance.StartCoroutine(BuildingUtils.SetBuildingName(m_buildingID, m_buildingNameField.text, () =>
                {
                    m_buildingName.text = m_buildingNameField.text;
                }));
            };
        }


        private void AwakeBG()
        {
            m_uIHelper = new UIHelperExtension(GetComponent<UIPanel>());
            KlyteMonoUtils.CreateUIElement<UIPanel>(out m_background, transform, "BG");
            m_mouseIsOver = false;
            component.eventMouseEnter += new MouseEventHandler(OnMouseEnter);
            component.eventMouseLeave += new MouseEventHandler(OnMouseLeave);
            m_background.width = 844;
            m_background.height = 38;
            m_background.relativePosition = default;
            SetBackgroundColor();

            m_uIHelper.Self.width = 844;
            m_uIHelper.Self.height = 38;
            m_background.backgroundSprite = "InfoviewPanel";

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
    internal sealed class VMCBuildingInfoItemOutTra : VMCBuildingInfoItem<VMCSysDefOutTra> { }
    internal sealed class VMCBuildingInfoItemOutShp : VMCBuildingInfoItem<VMCSysDefOutShp> { }
    internal sealed class VMCBuildingInfoItemOutPln : VMCBuildingInfoItem<VMCSysDefOutPln> { }
    internal sealed class VMCBuildingInfoItemOutCar : VMCBuildingInfoItem<VMCSysDefOutCar> { }
    internal sealed class VMCBuildingInfoItemOutBus : VMCBuildingInfoItem<VMCSysDefOutBus> { }
    internal sealed class VMCBuildingInfoItemBeaCar : VMCBuildingInfoItem<VMCSysDefBeaCar> { }
    internal sealed class VMCBuildingInfoItemPstCar : VMCBuildingInfoItem<VMCSysDefPstCar> { }
    internal sealed class VMCBuildingInfoItemPstTrk : VMCBuildingInfoItem<VMCSysDefPstTrk> { }
    internal sealed class VMCBuildingInfoItemWstTrn : VMCBuildingInfoItem<VMCSysDefWstTrn> { }
    internal sealed class VMCBuildingInfoItemIfmTrl : VMCBuildingInfoItem<VMCSysDefIfmTrl> { }
    internal sealed class VMCBuildingInfoItemFshTrk : VMCBuildingInfoItem<VMCSysDefFshTrk> { }
    internal sealed class VMCBuildingInfoItemIndTrk : VMCBuildingInfoItem<VMCSysDefIndTrk> { }
    internal sealed class VMCBuildingInfoItemIfmTrk : VMCBuildingInfoItem<VMCSysDefIfmTrk> { }
    internal sealed class VMCBuildingInfoItemIfrTrk : VMCBuildingInfoItem<VMCSysDefIfrTrk> { }
    internal sealed class VMCBuildingInfoItemIgnTrk : VMCBuildingInfoItem<VMCSysDefIgnTrk> { }
    internal sealed class VMCBuildingInfoItemIolTrk : VMCBuildingInfoItem<VMCSysDefIolTrk> { }
    internal sealed class VMCBuildingInfoItemIorTrk : VMCBuildingInfoItem<VMCSysDefIorTrk> { }
    internal sealed class VMCBuildingInfoItemClbPln : VMCBuildingInfoItem<VMCSysDefClbPln> { }
    internal sealed class VMCBuildingInfoItemCrgPln : VMCBuildingInfoItem<VMCSysDefCrgPln> { }
    internal sealed class VMCBuildingInfoItemWstCol : VMCBuildingInfoItem<VMCSysDefWstCol> { }
    internal sealed class VMCBuildingInfoItemAdtBcc : VMCBuildingInfoItem<VMCSysDefAdtBcc> { }
    internal sealed class VMCBuildingInfoItemChdBcc : VMCBuildingInfoItem<VMCSysDefChdBcc> { }
    internal sealed class VMCBuildingInfoItemTouBal : VMCBuildingInfoItem<VMCSysDefTouBal> { }

}
