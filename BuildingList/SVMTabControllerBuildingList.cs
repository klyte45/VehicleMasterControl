using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Extensions;
using Klyte.Harmony;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.UI;
using Klyte.ServiceVehiclesManager.Overrides;
using Klyte.ServiceVehiclesManager.Utils;
using Klyte.TransportLinesManager.Extensors;
using Klyte.TransportLinesManager.Extensors.BuildingAIExt;
using Klyte.TransportLinesManager.Extensors.TransportTypeExt;
using Klyte.TransportLinesManager.LineList.ExtraUI;
using Klyte.TransportLinesManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager.UI
{
    internal abstract class SVMTabControllerBuildingHooks<T, V> : Redirector<T> where T : SVMTabControllerBuildingHooks<T, V> where V : SVMSysDef<V>
    {
        private static SVMTabControllerBuildingHooks<T, V> instance;

        public static void AfterCreateBuilding(bool __result, BuildingInfo info)
        {
            if (__result && Singleton<V>.instance.GetSSD().isFromSystem(info))
            {
                SVMTabControllerBuildingList<V>.instance.m_LinesUpdated = false;
            }
        }
        public static void AfterRemoveBuilding(ushort building)
        {
            if (Singleton<V>.instance.GetSSD().isFromSystem(Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].Info))
            {
                SVMTabControllerBuildingList<V>.instance.m_LinesUpdated = false;
            }
        }

        public override void Awake()
        {
            instance = this;
            ServiceSystemDefinition def = Singleton<V>.instance.GetSSD();

            var from = typeof(BuildingManager).GetMethod("CreateBuilding", allFlags);
            var to = typeof(SVMTabControllerBuildingHooks<T, V>).GetMethod("AfterCreateBuilding", allFlags);
            var from2 = typeof(BuildingManager).GetMethod("ReleaseBuilding", allFlags);
            var to2 = typeof(SVMTabControllerBuildingHooks<T, V>).GetMethod("AfterRemoveBuilding", allFlags);
            SVMUtils.doLog("Loading After Hooks: {0} ({1}=>{2})", typeof(BuildingManager), from, to);
            SVMUtils.doLog("Loading After Hooks: {0} ({1}=>{2})", typeof(BuildingManager), from2, to2);
            AddRedirect(from, null, to);
            AddRedirect(from2, null, to2);
        }
    }
    internal sealed class SVMTabControllerBuildingHooksDisCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksDisCar, SVMSysDefDisCar> { }
    internal sealed class SVMTabControllerBuildingHooksDisHel : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksDisHel, SVMSysDefDisHel> { }
    internal sealed class SVMTabControllerBuildingHooksFirCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksFirCar, SVMSysDefFirCar> { }
    internal sealed class SVMTabControllerBuildingHooksFirHel : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksFirHel, SVMSysDefFirHel> { }
    internal sealed class SVMTabControllerBuildingHooksGarCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksGarCar, SVMSysDefGarCar> { }
    internal sealed class SVMTabControllerBuildingHooksGbcCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksGbcCar, SVMSysDefGbcCar> { }
    internal sealed class SVMTabControllerBuildingHooksHcrCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksHcrCar, SVMSysDefHcrCar> { }
    internal sealed class SVMTabControllerBuildingHooksHcrHel : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksHcrHel, SVMSysDefHcrHel> { }
    internal sealed class SVMTabControllerBuildingHooksPolCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksPolCar, SVMSysDefPolCar> { }
    internal sealed class SVMTabControllerBuildingHooksPolHel : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksPolHel, SVMSysDefPolHel> { }
    internal sealed class SVMTabControllerBuildingHooksRoaCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksRoaCar, SVMSysDefRoaCar> { }
    internal sealed class SVMTabControllerBuildingHooksWatCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksWatCar, SVMSysDefWatCar> { }
    internal sealed class SVMTabControllerBuildingHooksPriCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksPriCar, SVMSysDefPriCar> { }
    internal sealed class SVMTabControllerBuildingHooksDcrCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksDcrCar, SVMSysDefDcrCar> { }
    internal sealed class SVMTabControllerBuildingHooksTaxCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksTaxCar, SVMSysDefTaxCar> { }
    internal sealed class SVMTabControllerBuildingHooksCcrCcr : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksCcrCcr, SVMSysDefCcrCcr> { }
    internal sealed class SVMTabControllerBuildingHooksSnwCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksSnwCar, SVMSysDefSnwCar> { }
    internal sealed class SVMTabControllerBuildingHooksRegTra : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksRegTra, SVMSysDefRegTra> { }
    internal sealed class SVMTabControllerBuildingHooksRegShp : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksRegShp, SVMSysDefRegShp> { }
    internal sealed class SVMTabControllerBuildingHooksRegPln : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksRegPln, SVMSysDefRegPln> { }



    internal abstract class SVMTabControllerBuildingList<T> : UICustomControl where T : SVMSysDef<T>
    {
        public static SVMTabControllerBuildingList<T> instance { get; private set; }

        private UIScrollablePanel mainPanel;
        private static readonly string kLineTemplate = "LineTemplate";
        public bool m_LinesUpdated = false;

        #region Awake
        private void Awake()
        {
            instance = this;
            mainPanel = GetComponentInChildren<UIScrollablePanel>();
            mainPanel.autoLayout = true;
            mainPanel.autoLayoutDirection = LayoutDirection.Vertical;
        }
        #endregion

        private void Update()
        {
            if (!mainPanel.isVisible) return;
            if (!this.m_LinesUpdated)
            {
                this.RefreshLines();
            }
        }

        private void AddToList(ushort buildingID, ref int count)
        {
            SVMBuildingInfoItem<T> buildingInfoItem;
            Type implClassBuildingLine = SVMUtils.GetImplementationForGenericType(typeof(SVMBuildingInfoItem<>), typeof(T));
            if (count >= mainPanel.components.Count)
            {
                var temp = UITemplateManager.Get<PublicTransportLineInfo>(kLineTemplate).gameObject;
                GameObject.Destroy(temp.GetComponent<PublicTransportLineInfo>());
                buildingInfoItem = (SVMBuildingInfoItem<T>)temp.AddComponent(implClassBuildingLine);
                mainPanel.AttachUIComponent(buildingInfoItem.gameObject);
            }
            else
            {
                buildingInfoItem = (SVMBuildingInfoItem<T>)mainPanel.components[count].GetComponent(implClassBuildingLine);
            }
            buildingInfoItem.buildingId = buildingID;
            buildingInfoItem.RefreshData();
            count++;
        }

        public void RefreshLines()
        {
            if (Singleton<BuildingManager>.exists)
            {
                int count = 0;
                var buildingList = SVMBuildingUtils.getAllBuildingsFromCity(Singleton<T>.instance.GetSSD());

                SVMUtils.doLog("{0} buildingList = {1} (s={2})", GetType(), buildingList.ToArray(), buildingList.Count);
                foreach (ushort buildingID in buildingList)
                {
                    Building b = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID];
                    var ext = SVMBuildingAIOverrideUtils.getBuildingOverrideExtension(b.Info);
                    var maxVehicle = SVMUtils.GetPrivateField<int>(b.Info.GetAI(), ext.GetVehicleMaxCountField(SVMSysDef<T>.instance.GetSSD().vehicleType));
                    if (maxVehicle > 0)
                    {
                        AddToList(buildingID, ref count);
                    }
                }
                RemoveExtraLines(count);

                m_LinesUpdated = true;
            }
        }

        private void RemoveExtraLines(int linesCount)
        {
            while (mainPanel.components.Count > linesCount)
            {
                UIComponent uIComponent = mainPanel.components[linesCount];
                mainPanel.RemoveUIComponent(uIComponent);
                Destroy(uIComponent.gameObject);
            }
        }
    }
    internal sealed class SVMTabControllerBuildingListDisCar : SVMTabControllerBuildingList<SVMSysDefDisCar> { }
    internal sealed class SVMTabControllerBuildingListDisHel : SVMTabControllerBuildingList<SVMSysDefDisHel> { }
    internal sealed class SVMTabControllerBuildingListFirCar : SVMTabControllerBuildingList<SVMSysDefFirCar> { }
    internal sealed class SVMTabControllerBuildingListFirHel : SVMTabControllerBuildingList<SVMSysDefFirHel> { }
    internal sealed class SVMTabControllerBuildingListGarCar : SVMTabControllerBuildingList<SVMSysDefGarCar> { }
    internal sealed class SVMTabControllerBuildingListGbcCar : SVMTabControllerBuildingList<SVMSysDefGbcCar> { }
    internal sealed class SVMTabControllerBuildingListHcrCar : SVMTabControllerBuildingList<SVMSysDefHcrCar> { }
    internal sealed class SVMTabControllerBuildingListHcrHel : SVMTabControllerBuildingList<SVMSysDefHcrHel> { }
    internal sealed class SVMTabControllerBuildingListPolCar : SVMTabControllerBuildingList<SVMSysDefPolCar> { }
    internal sealed class SVMTabControllerBuildingListPolHel : SVMTabControllerBuildingList<SVMSysDefPolHel> { }
    internal sealed class SVMTabControllerBuildingListRoaCar : SVMTabControllerBuildingList<SVMSysDefRoaCar> { }
    internal sealed class SVMTabControllerBuildingListWatCar : SVMTabControllerBuildingList<SVMSysDefWatCar> { }
    internal sealed class SVMTabControllerBuildingListPriCar : SVMTabControllerBuildingList<SVMSysDefPriCar> { }
    internal sealed class SVMTabControllerBuildingListDcrCar : SVMTabControllerBuildingList<SVMSysDefDcrCar> { }
    internal sealed class SVMTabControllerBuildingListTaxCar : SVMTabControllerBuildingList<SVMSysDefTaxCar> { }
    internal sealed class SVMTabControllerBuildingListCcrCcr : SVMTabControllerBuildingList<SVMSysDefCcrCcr> { }
    internal sealed class SVMTabControllerBuildingListSnwCar : SVMTabControllerBuildingList<SVMSysDefSnwCar> { }
    internal sealed class SVMTabControllerBuildingListRegTra : SVMTabControllerBuildingList<SVMSysDefRegTra> { }
    internal sealed class SVMTabControllerBuildingListRegShp : SVMTabControllerBuildingList<SVMSysDefRegShp> { }
    internal sealed class SVMTabControllerBuildingListRegPln : SVMTabControllerBuildingList<SVMSysDefRegPln> { }

}
