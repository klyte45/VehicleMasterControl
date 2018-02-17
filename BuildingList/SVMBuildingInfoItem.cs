
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SVMCW = Klyte.ServiceVehiclesManager.SVMConfigWarehouse;

namespace Klyte.ServiceVehiclesManager.LineList
{
    using ColossalFramework;
    using ColossalFramework.Globalization;
    using ColossalFramework.UI;
    using Extensions;
    using Extensors;
    using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
    using Klyte.ServiceVehiclesManager.Overrides;
    using System;
    using System.Collections;
    using System.Diagnostics;
    using UnityEngine;
    using Utils;
    internal abstract class SVMBuildingInfoItem<T> : ToolsModifierControl where T : SVMSysDef<T>
    {
        private ushort m_buildingID;

        private bool m_secondary;

        private UILabel m_districtName;

        private UILabel m_buildingName;

        private UITextField m_buildingNameField;

        private UILabel m_totalVehicles;

        private UIComponent m_Background;

        private Color32 m_BackgroundColor;

        private bool m_mouseIsOver;

        public ushort buildingId
        {
            get {
                return this.m_buildingID;
            }
            set {
                this.SetBuildingID(value);
            }
        }

        public bool secondary
        {
            get {
                return this.m_secondary;
            }
            set {
                m_secondary = value;
            }
        }
        public string districtName
        {
            get {
                return this.m_districtName.text;
            }
        }

        public string buidingName
        {
            get {
                return this.m_buildingName.text;
            }
        }

        public string prefixesServed
        {
            get {
                return this.m_totalVehicles.text;
            }
        }

        private void SetBuildingID(ushort id)
        {
            this.m_buildingID = id;
        }



        public void RefreshData()
        {
            if (Singleton<BuildingManager>.exists && transform.parent.gameObject.GetComponent<UIComponent>().isVisible)
            {
                GetComponent<UIComponent>().isVisible = true;
                Building b = Singleton<BuildingManager>.instance.m_buildings.m_buffer[this.m_buildingID];
                this.m_buildingName.text = Singleton<BuildingManager>.instance.GetBuildingName(this.m_buildingID, default(InstanceID));
                byte districtID = Singleton<DistrictManager>.instance.GetDistrict(b.m_position);
                string districtName = districtID == 0 ? Locale.Get("SVM_DISTRICT_NONE") : Singleton<DistrictManager>.instance.GetDistrictName(districtID);
                this.m_districtName.text = districtName;

                int count = 0;
                int cargo = 0;
                int capacity = 0;
                int inbound = 0;
                int outbound = 0;
                var ext = SVMBuildingAIOverrideUtils.getBuildingOverrideExtension(b.Info);
                SVMBuildingUtils.CalculateOwnVehicles(buildingId, ref b, ext.GetManagedReasons(b.Info).Keys, ref count, ref cargo, ref capacity, ref inbound, ref outbound);
                m_totalVehicles.prefix = count.ToString();
                m_totalVehicles.suffix = (SVMUtils.GetPrivateField<int>(b.Info.GetAI(), ext.GetVehicleMaxCountField(SVMSysDef<T>.instance.GetSSD().vehicleType)) * SVMBuildingUtils.GetProductionRate(ref b)/100).ToString();
            }
        }

        public void SetBackgroundColor()
        {
            Color32 backgroundColor = this.m_BackgroundColor;
            backgroundColor.a = (byte)((base.component.zOrder % 2 != 0) ? 127 : 255);
            if (this.m_mouseIsOver)
            {
                backgroundColor.r = (byte)Mathf.Min(255, backgroundColor.r * 3 >> 1);
                backgroundColor.g = (byte)Mathf.Min(255, backgroundColor.g * 3 >> 1);
                backgroundColor.b = (byte)Mathf.Min(255, backgroundColor.b * 3 >> 1);
            }
            this.m_Background.color = backgroundColor;
        }

        private void LateUpdate()
        {
            if (base.component.parent.isVisible)
            {
                this.RefreshData();
            }
        }

        private void Awake()
        {
            SVMUtils.clearAllVisibilityEvents(this.GetComponent<UIPanel>());
            base.component.eventZOrderChanged += delegate (UIComponent c, int r)
            {
                this.SetBackgroundColor();
            };
            GameObject.Destroy(base.Find<UICheckBox>("LineVisible").gameObject);
            GameObject.Destroy(base.Find<UIColorField>("LineColor").gameObject);
            GameObject.Destroy(base.Find<UIPanel>("WarningIncomplete"));

            this.m_buildingName = base.Find<UILabel>("LineName");
            this.m_buildingName.area = new Vector4(200,2,198,35);
            this.m_buildingNameField = this.m_buildingName.Find<UITextField>("LineNameField");
            this.m_buildingNameField.maxLength = 256;
            this.m_buildingNameField.eventTextChanged += new PropertyChangedEventHandler<string>(this.OnRename);
            this.m_buildingName.eventMouseEnter += delegate (UIComponent c, UIMouseEventParameter r)
            {
                this.m_buildingName.backgroundSprite = "TextFieldPanelHovered";
            };
            this.m_buildingName.eventMouseLeave += delegate (UIComponent c, UIMouseEventParameter r)
            {
                this.m_buildingName.backgroundSprite = string.Empty;
            };
            this.m_buildingName.eventClick += delegate (UIComponent c, UIMouseEventParameter r)
            {
                this.m_buildingNameField.Show();
                this.m_buildingNameField.text = this.m_buildingName.text;
                this.m_buildingNameField.Focus();
            };
            this.m_buildingNameField.eventLeaveFocus += delegate (UIComponent c, UIFocusEventParameter r)
            {
                this.m_buildingNameField.Hide();
                Singleton<BuildingManager>.instance.StartCoroutine(SVMUtils.setBuildingName(this.m_buildingID, this.m_buildingNameField.text, () =>
                {
                    this.m_buildingName.text = this.m_buildingNameField.text;
                }));
            };

            GameObject.Destroy(base.Find<UICheckBox>("DayLine").gameObject);
            GameObject.Destroy(base.Find<UICheckBox>("NightLine").gameObject);
            GameObject.Destroy(base.Find<UICheckBox>("DayNightLine").gameObject);

            var m_DayLine = base.Find<UICheckBox>("DayLine");

            GameObject.Destroy(base.Find<UICheckBox>("NightLine").gameObject);
            GameObject.Destroy(base.Find<UICheckBox>("DayNightLine").gameObject);
            GameObject.Destroy(m_DayLine.gameObject);

            m_districtName = base.Find<UILabel>("LineStops");
            m_districtName.size = new Vector2(175, 18);
            m_districtName.relativePosition = new Vector3(0, 10);
            m_districtName.pivot = UIPivotPoint.MiddleCenter;
            m_districtName.wordWrap = true;
            m_districtName.autoHeight = true;



            GameObject.Destroy(base.Find<UILabel>("LinePassengers").gameObject);
            this.m_totalVehicles = base.Find<UILabel>("LineVehicles");
            m_totalVehicles.text = "/";

            this.m_Background = base.Find("Background");
            this.m_BackgroundColor = this.m_Background.color;
            this.m_mouseIsOver = false;
            base.component.eventMouseEnter += new MouseEventHandler(this.OnMouseEnter);
            base.component.eventMouseLeave += new MouseEventHandler(this.OnMouseLeave);
            GameObject.Destroy(base.Find<UIButton>("DeleteLine").gameObject);
            base.Find<UIButton>("ViewLine").eventClick += delegate (UIComponent c, UIMouseEventParameter r)
            {
                if (this.m_buildingID != 0)
                {
                    Vector3 position = Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)this.m_buildingID].m_position;
                    InstanceID instanceID = default(InstanceID);
                    instanceID.Building = this.m_buildingID;
                    //SVMController.instance.depotInfoPanel.openDepotInfo(m_buildingID, secondary);
                    ToolsModifierControl.cameraController.SetTarget(instanceID, position, true);
                }
            };
            base.component.eventVisibilityChanged += delegate (UIComponent c, bool v)
            {
                if (v)
                {
                    this.RefreshData();
                }
            };
        }

        private void OnMouseEnter(UIComponent comp, UIMouseEventParameter param)
        {
            if (!this.m_mouseIsOver)
            {
                this.m_mouseIsOver = true;
                this.SetBackgroundColor();
            }
        }

        private void OnMouseLeave(UIComponent comp, UIMouseEventParameter param)
        {
            if (this.m_mouseIsOver)
            {
                this.m_mouseIsOver = false;
                this.SetBackgroundColor();
            }
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        private void OnRename(UIComponent comp, string text)
        {
            Singleton<BuildingManager>.instance.StartCoroutine(SVMUtils.setBuildingName(this.m_buildingID, text, () => { }));
        }

        private void OnLineChanged(ushort id)
        {
            if (id == this.m_buildingID)
            {
                this.RefreshData();
            }
        }

    }

    internal sealed class SVMBuildingInfoItemDisCar : SVMBuildingInfoItem<SVMSysDefDisCar> { }
    internal sealed class SVMBuildingInfoItemDisHel : SVMBuildingInfoItem<SVMSysDefDisHel> { }
    internal sealed class SVMBuildingInfoItemFirCar : SVMBuildingInfoItem<SVMSysDefFirCar> { }
    internal sealed class SVMBuildingInfoItemFirHel : SVMBuildingInfoItem<SVMSysDefFirHel> { }
    internal sealed class SVMBuildingInfoItemGarCar : SVMBuildingInfoItem<SVMSysDefGarCar> { }
    internal sealed class SVMBuildingInfoItemGbcCar : SVMBuildingInfoItem<SVMSysDefGbcCar> { }
    internal sealed class SVMBuildingInfoItemHcrCar : SVMBuildingInfoItem<SVMSysDefHcrCar> { }
    internal sealed class SVMBuildingInfoItemHcrHel : SVMBuildingInfoItem<SVMSysDefHcrHel> { }
    internal sealed class SVMBuildingInfoItemPolCar : SVMBuildingInfoItem<SVMSysDefPolCar> { }
    internal sealed class SVMBuildingInfoItemPolHel : SVMBuildingInfoItem<SVMSysDefPolHel> { }
    internal sealed class SVMBuildingInfoItemRoaCar : SVMBuildingInfoItem<SVMSysDefRoaCar> { }
    internal sealed class SVMBuildingInfoItemWatCar : SVMBuildingInfoItem<SVMSysDefWatCar> { }
    internal sealed class SVMBuildingInfoItemPriCar : SVMBuildingInfoItem<SVMSysDefPriCar> { }
    internal sealed class SVMBuildingInfoItemDcrCar : SVMBuildingInfoItem<SVMSysDefDcrCar> { }
    internal sealed class SVMBuildingInfoItemTaxCar : SVMBuildingInfoItem<SVMSysDefTaxCar> { }
    internal sealed class SVMBuildingInfoItemCcrCcr : SVMBuildingInfoItem<SVMSysDefCcrCcr> { }
    internal sealed class SVMBuildingInfoItemSnwCar : SVMBuildingInfoItem<SVMSysDefSnwCar> { }
}
