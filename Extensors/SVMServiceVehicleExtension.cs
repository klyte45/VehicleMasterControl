using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.Math;
using Klyte.TransportLinesManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Klyte.TransportLinesManager.Interfaces;
using Klyte.TransportLinesManager.Extensors.TransportLineExt;
using Klyte.ServiceVehiclesManager.Interfaces;
using Klyte.ServiceVehiclesManager.Utils;

namespace Klyte.ServiceVehiclesManager.Extensors.VehicleExt
{
    public interface ISVMTransportTypeExtension : IAssetSelectorExtension, IBudgetableExtension { }

    internal abstract class SVMServiceVehicleExtension<SSD, SG> : ExtensionInterfaceDefaultImpl<DistrictConfigIndex, SG>, ISVMTransportTypeExtension where SSD : SVMSysDef, new() where SG : SVMServiceVehicleExtension<SSD, SG>
    {

        protected override SVMConfigWarehouse.ConfigIndex ConfigIndexKey
        {
            get {
                return SVMConfigWarehouse.ConfigIndex.NIL;// SVMConfigWarehouse.getConfigAssetsForAI(definition);
            }
        }
        protected override bool AllowGlobal { get { return false; } }

        private List<string> basicAssetsList;

        private ServiceSystemDefinition definition => Singleton<SSD>.instance.GetSSD();

        #region Budget Multiplier
        public uint[] GetBudgetsMultiplier(uint prefix)
        {
            string value = SafeGet(prefix, DistrictConfigIndex.BUDGET_MULTIPLIER);
            if (value == null) return new uint[] { 100 };
            string[] savedMultipliers = value.Split(ItSepLvl3.ToCharArray());

            uint[] result = new uint[savedMultipliers.Length];
            for (int i = 0; i < result.Length; i++)
            {
                if (uint.TryParse(savedMultipliers[i], out uint parsed))
                {
                    result[i] = parsed;
                }
                else
                {
                    return new uint[] { 100 };
                }
            }
            return result;
        }
        public uint GetBudgetMultiplierForHour(uint prefix, int hour)
        {
            uint[] savedMultipliers = GetBudgetsMultiplier(prefix);
            if (savedMultipliers.Length == 1)
            {
                return savedMultipliers[0];
            }
            else if (savedMultipliers.Length == 8)
            {
                return savedMultipliers[((hour + 23) / 3) % 8];
            }
            return 100;
        }
        public void SetBudgetMultiplier(uint prefix, uint[] multipliers)
        {
            SafeSet(prefix, DistrictConfigIndex.BUDGET_MULTIPLIER, string.Join(ItSepLvl3, multipliers.Select(x => x.ToString()).ToArray()));
        }
        #endregion

        
        #region Asset List
        public List<string> GetAssetList(uint prefix)
        {
            string value = SafeGet(prefix, DistrictConfigIndex.MODELS);
            if (string.IsNullOrEmpty(value))
            {
                return new List<string>();
            }
            else
            {
                return value.Split(ItSepLvl3.ToCharArray()).ToList();
            }
        }
        public Dictionary<string, string> GetSelectedBasicAssets(uint prefix)
        {
            if (basicAssetsList == null) LoadBasicAssets();
            return GetAssetList(prefix).Where(x => PrefabCollection<VehicleInfo>.FindLoaded(x) != null).ToDictionary(x => x, x => string.Format("[Cap={0}] {1}", SVMUtils.getCapacity(PrefabCollection<VehicleInfo>.FindLoaded(x)), Locale.Get("VEHICLE_TITLE", x)));
        }
        public Dictionary<string, string> GetAllBasicAssets(uint nil = 0)
        {
            if (basicAssetsList == null) LoadBasicAssets();
            return basicAssetsList.ToDictionary(x => x, x => string.Format("[Cap={0}] {1}", SVMUtils.getCapacity(PrefabCollection<VehicleInfo>.FindLoaded(x)), Locale.Get("VEHICLE_TITLE", x)));
        }
        public void AddAsset(uint prefix, string assetId)
        {
            var temp = GetAssetList(prefix);
            if (temp.Contains(assetId)) return;
            temp.Add(assetId);
            SafeSet(prefix, DistrictConfigIndex.MODELS, string.Join(ItSepLvl3, temp.ToArray()));
        }
        public void RemoveAsset(uint prefix, string assetId)
        {
            var temp = GetAssetList(prefix);
            if (!temp.Contains(assetId)) return;
            temp.RemoveAll(x => x == assetId);
            SafeSet(prefix, DistrictConfigIndex.MODELS, string.Join(ItSepLvl3, temp.ToArray()));
        }
        public void UseDefaultAssets(uint prefix)
        {
            SafeCleanProperty(prefix, DistrictConfigIndex.MODELS);
        }
        public VehicleInfo GetAModel(ushort buildingId)
        {
            return SVMUtils.GetRandomModel(GetAssetList(SVMUtils.GetBuildingDistrict(buildingId)));
        }
        public void LoadBasicAssets()
        {
            //basicAssetsList = SVMUtils.LoadBasicAssets(definition);
        }
        #endregion

    }

    internal sealed class SVMServiceVehicleExtensionDisCar : SVMServiceVehicleExtension<SVMSysDefDisCar, SVMServiceVehicleExtensionDisCar> { }
    internal sealed class SVMServiceVehicleExtensionDisHel : SVMServiceVehicleExtension<SVMSysDefDisHel, SVMServiceVehicleExtensionDisHel> { }
    internal sealed class SVMServiceVehicleExtensionFirCar : SVMServiceVehicleExtension<SVMSysDefFirCar, SVMServiceVehicleExtensionFirCar> { }
    internal sealed class SVMServiceVehicleExtensionFirHel : SVMServiceVehicleExtension<SVMSysDefFirHel, SVMServiceVehicleExtensionFirHel> { }
    internal sealed class SVMServiceVehicleExtensionGarCar : SVMServiceVehicleExtension<SVMSysDefGarCar, SVMServiceVehicleExtensionGarCar> { }
    internal sealed class SVMServiceVehicleExtensionHcrCar : SVMServiceVehicleExtension<SVMSysDefHcrCar, SVMServiceVehicleExtensionHcrCar> { }
    internal sealed class SVMServiceVehicleExtensionHcrHel : SVMServiceVehicleExtension<SVMSysDefHcrHel, SVMServiceVehicleExtensionHcrHel> { }
    internal sealed class SVMServiceVehicleExtensionPolCar : SVMServiceVehicleExtension<SVMSysDefPolCar, SVMServiceVehicleExtensionPolCar> { }
    internal sealed class SVMServiceVehicleExtensionPolHel : SVMServiceVehicleExtension<SVMSysDefPolHel, SVMServiceVehicleExtensionPolHel> { }
    internal sealed class SVMServiceVehicleExtensionRoaCar : SVMServiceVehicleExtension<SVMSysDefRoaCar, SVMServiceVehicleExtensionRoaCar> { }
    internal sealed class SVMServiceVehicleExtensionWatCar : SVMServiceVehicleExtension<SVMSysDefRoaCar, SVMServiceVehicleExtensionWatCar> { }
    internal sealed class SVMServiceVehicleExtensionPriCar : SVMServiceVehicleExtension<SVMSysDefRoaCar, SVMServiceVehicleExtensionPriCar> { }
    internal sealed class SVMServiceVehicleExtensionDcrCar : SVMServiceVehicleExtension<SVMSysDefRoaCar, SVMServiceVehicleExtensionDcrCar> { }

    public sealed class SVMTransportExtensionUtils
    {

        public static void RemoveAllUnwantedVehicles()
        {
            //for (ushort buildingId = 1; buildingId < Singleton<BuildingManager>.instance.m_buildings.m_size; buildingId++)
            //{
            //    if ((Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingId].Info. & TransportLine.Flags.Created) != TransportLine.Flags.None)
            //    {
            //        uint idx;
            //        IAssetSelectorExtension extension;
            //        if (TLMTransportLineExtension.instance.GetUseCustomConfig(buildingId))
            //        {
            //            idx = buildingId;
            //            extension = TLMTransportLineExtension.instance;
            //        }
            //        else
            //        {
            //            idx = TLMLineUtils.getPrefix(buildingId);
            //            var def = TransportSystemDefinition.from(buildingId);
            //            extension = def.GetTransportExtension();
            //        }

            //        TransportLine tl = Singleton<TransportManager>.instance.m_lines.m_buffer[buildingId];
            //        var modelList = extension.GetAssetList(idx);
            //        VehicleManager vm = Singleton<VehicleManager>.instance;
            //        VehicleInfo info = vm.m_vehicles.m_buffer[Singleton<TransportManager>.instance.m_lines.m_buffer[buildingId].GetVehicle(0)].Info;

            //         SVMUtils.doLog("removeAllUnwantedVehicles: models found: {0}", modelList == null ? "?!?" : modelList.Count.ToString());

            //        if (modelList.Count > 0)
            //        {
            //            Dictionary<ushort, VehicleInfo> vehiclesToRemove = new Dictionary<ushort, VehicleInfo>();
            //            for (int i = 0; i < tl.CountVehicles(buildingId); i++)
            //            {
            //                var vehicle = tl.GetVehicle(i);
            //                if (vehicle != 0)
            //                {
            //                    VehicleInfo info2 = vm.m_vehicles.m_buffer[(int)vehicle].Info;
            //                    if (!modelList.Contains(info2.name))
            //                    {
            //                        vehiclesToRemove[vehicle] = info2;
            //                    }
            //                }
            //            }
            //            foreach (var item in vehiclesToRemove)
            //            {
            //                item.Value.m_vehicleAI.SetTransportLine(item.Key, ref vm.m_vehicles.m_buffer[item.Key], 0);
            //            }
            //        }
            //    }
            //}
        }
    }




    internal enum DistrictConfigIndex
    {
        MODELS,
        PREFIX_NAME,
        BUDGET_MULTIPLIER,
        TICKET_PRICE
    }
}
