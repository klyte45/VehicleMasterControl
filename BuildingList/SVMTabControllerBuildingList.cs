using ColossalFramework;
using ColossalFramework.UI;
using Klyte.Commons.Utils;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.Overrides;
using Klyte.ServiceVehiclesManager.Utils;
using System;
using System.Linq;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager.UI
{

    internal abstract class SVMTabControllerBuildingList<T> : UICustomControl where T : SVMSysDef<T>, new()
    {
        public static SVMTabControllerBuildingList<T> instance { get; private set; }
        public static bool exists => instance != null;

        public bool m_LinesUpdated = false;
        private UIScrollablePanel mainPanel;
        private static readonly string kLineTemplate = "LineTemplate";


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
            if (!mainPanel.isVisible)
            {
                m_LinesUpdated = false;
                return;
            }
            if (!m_LinesUpdated)
            {
                RefreshLines();
            }
        }

        private void AddToList(ushort buildingID, ref int count)
        {
            SVMBuildingInfoItem<T> buildingInfoItem;
            Type implClassBuildingLine = ReflectionUtils.GetImplementationForGenericType(typeof(SVMBuildingInfoItem<>), typeof(T));
            if (count >= mainPanel.components.Count)
            {
                GameObject temp = UITemplateManager.Get<PublicTransportLineInfo>(kLineTemplate).gameObject;
                GameObject.Destroy(temp.GetComponent<PublicTransportLineInfo>());
                buildingInfoItem = (SVMBuildingInfoItem<T>) temp.AddComponent(implClassBuildingLine);
                mainPanel.AttachUIComponent(buildingInfoItem.gameObject);
            }
            else
            {
                buildingInfoItem = (SVMBuildingInfoItem<T>) mainPanel.components[count].GetComponent(implClassBuildingLine);
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
                ServiceSystemDefinition ssd = SingletonLite<T>.instance.GetSSD();
                System.Collections.Generic.List<ushort> buildingList = SVMBuildingUtils.getAllBuildingsFromCity(ref ssd);

                LogUtils.DoLog("{0} buildingList = [{1}] (s={2})", GetType(), string.Join(",", buildingList.Select(x => x.ToString()).ToArray()), buildingList.Count);
                foreach (ushort buildingID in buildingList)
                {
                    Building b = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID];
                    int maxVehicle =99;
                    if ( maxVehicle > 0)
                    {
                        AddToList(buildingID, ref count);
                    }

                }
                RemoveExtraLines(count);
                LogUtils.DoLog("{0} final count = {1}", GetType(), count);

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
    internal sealed class SVMTabControllerBuildingListCrgTra : SVMTabControllerBuildingList<SVMSysDefCrgTra> { }
    internal sealed class SVMTabControllerBuildingListCrgShp : SVMTabControllerBuildingList<SVMSysDefCrgShp> { }
    //internal sealed class SVMTabControllerBuildingListOutTra : SVMTabControllerBuildingList<SVMSysDefOutTra> { }
    //internal sealed class SVMTabControllerBuildingListOutShp : SVMTabControllerBuildingList<SVMSysDefOutShp> { }
    //internal sealed class SVMTabControllerBuildingListOutPln : SVMTabControllerBuildingList<SVMSysDefOutPln> { }
    //internal sealed class SVMTabControllerBuildingListOutCar : SVMTabControllerBuildingList<SVMSysDefOutCar> { }
    internal sealed class SVMTabControllerBuildingListBeaCar : SVMTabControllerBuildingList<SVMSysDefBeaCar> { }
    internal sealed class SVMTabControllerBuildingListPstCar : SVMTabControllerBuildingList<SVMSysDefPstCar> { }
    internal sealed class SVMTabControllerBuildingListPstTrk : SVMTabControllerBuildingList<SVMSysDefPstTrk> { }

}
