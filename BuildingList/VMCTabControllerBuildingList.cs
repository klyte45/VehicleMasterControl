using ColossalFramework;
using ColossalFramework.UI;
using Klyte.Commons.Utils;
using Klyte.VehiclesMasterControl.Extensors.VehicleExt;
using Klyte.VehiclesMasterControl.Overrides;
using Klyte.VehiclesMasterControl.Utils;
using System;
using System.Linq;
using UnityEngine;

namespace Klyte.VehiclesMasterControl.UI
{

    internal abstract class VMCTabControllerBuildingList<T> : UICustomControl where T : VMCSysDef<T>, new()
    {
        public static VMCTabControllerBuildingList<T> instance { get; private set; }
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
            VMCBuildingInfoItem<T> buildingInfoItem;
            Type implClassBuildingLine = ReflectionUtils.GetImplementationForGenericType(typeof(VMCBuildingInfoItem<>), typeof(T));
            if (count >= mainPanel.components.Count)
            {
                GameObject temp = UITemplateManager.Get<PublicTransportLineInfo>(kLineTemplate).gameObject;
                GameObject.Destroy(temp.GetComponent<PublicTransportLineInfo>());
                buildingInfoItem = (VMCBuildingInfoItem<T>) temp.AddComponent(implClassBuildingLine);
                mainPanel.AttachUIComponent(buildingInfoItem.gameObject);
            }
            else
            {
                buildingInfoItem = (VMCBuildingInfoItem<T>) mainPanel.components[count].GetComponent(implClassBuildingLine);
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
                System.Collections.Generic.List<ushort> buildingList = VMCBuildingUtils.getAllBuildingsFromCity(ref ssd);

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
    internal sealed class VMCTabControllerBuildingListDisCar : VMCTabControllerBuildingList<VMCSysDefDisCar> { }
    internal sealed class VMCTabControllerBuildingListDisHel : VMCTabControllerBuildingList<VMCSysDefDisHel> { }
    internal sealed class VMCTabControllerBuildingListFirCar : VMCTabControllerBuildingList<VMCSysDefFirCar> { }
    internal sealed class VMCTabControllerBuildingListFirHel : VMCTabControllerBuildingList<VMCSysDefFirHel> { }
    internal sealed class VMCTabControllerBuildingListGarCar : VMCTabControllerBuildingList<VMCSysDefGarCar> { }
    internal sealed class VMCTabControllerBuildingListGbcCar : VMCTabControllerBuildingList<VMCSysDefGbcCar> { }
    internal sealed class VMCTabControllerBuildingListHcrCar : VMCTabControllerBuildingList<VMCSysDefHcrCar> { }
    internal sealed class VMCTabControllerBuildingListHcrHel : VMCTabControllerBuildingList<VMCSysDefHcrHel> { }
    internal sealed class VMCTabControllerBuildingListPolCar : VMCTabControllerBuildingList<VMCSysDefPolCar> { }
    internal sealed class VMCTabControllerBuildingListPolHel : VMCTabControllerBuildingList<VMCSysDefPolHel> { }
    internal sealed class VMCTabControllerBuildingListRoaCar : VMCTabControllerBuildingList<VMCSysDefRoaCar> { }
    internal sealed class VMCTabControllerBuildingListWatCar : VMCTabControllerBuildingList<VMCSysDefWatCar> { }
    internal sealed class VMCTabControllerBuildingListPriCar : VMCTabControllerBuildingList<VMCSysDefPriCar> { }
    internal sealed class VMCTabControllerBuildingListDcrCar : VMCTabControllerBuildingList<VMCSysDefDcrCar> { }
    internal sealed class VMCTabControllerBuildingListTaxCar : VMCTabControllerBuildingList<VMCSysDefTaxCar> { }
    internal sealed class VMCTabControllerBuildingListCcrCcr : VMCTabControllerBuildingList<VMCSysDefCcrCcr> { }
    internal sealed class VMCTabControllerBuildingListSnwCar : VMCTabControllerBuildingList<VMCSysDefSnwCar> { }
    internal sealed class VMCTabControllerBuildingListRegTra : VMCTabControllerBuildingList<VMCSysDefRegTra> { }
    internal sealed class VMCTabControllerBuildingListRegShp : VMCTabControllerBuildingList<VMCSysDefRegShp> { }
    internal sealed class VMCTabControllerBuildingListRegPln : VMCTabControllerBuildingList<VMCSysDefRegPln> { }
    internal sealed class VMCTabControllerBuildingListCrgTra : VMCTabControllerBuildingList<VMCSysDefCrgTra> { }
    internal sealed class VMCTabControllerBuildingListCrgShp : VMCTabControllerBuildingList<VMCSysDefCrgShp> { }
    //internal sealed class VMCTabControllerBuildingListOutTra : VMCTabControllerBuildingList<VMCSysDefOutTra> { }
    //internal sealed class VMCTabControllerBuildingListOutShp : VMCTabControllerBuildingList<VMCSysDefOutShp> { }
    //internal sealed class VMCTabControllerBuildingListOutPln : VMCTabControllerBuildingList<VMCSysDefOutPln> { }
    //internal sealed class VMCTabControllerBuildingListOutCar : VMCTabControllerBuildingList<VMCSysDefOutCar> { }
    internal sealed class VMCTabControllerBuildingListBeaCar : VMCTabControllerBuildingList<VMCSysDefBeaCar> { }
    internal sealed class VMCTabControllerBuildingListPstCar : VMCTabControllerBuildingList<VMCSysDefPstCar> { }
    internal sealed class VMCTabControllerBuildingListPstTrk : VMCTabControllerBuildingList<VMCSysDefPstTrk> { }

}
