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
    internal interface ISVMTransportTypeExtension : ISVMAssetSelectorExtension, ISVMIgnorableDistrictExtensionValue, ISVMColorSelectableExtensionValue { }

    internal abstract class SVMServiceVehicleExtension<SSD, SG> : ExtensionInterfaceDefaultImpl<BuildingConfig, SG>, ISVMTransportTypeExtension where SSD : SVMSysDef<SSD>, new() where SG : SVMServiceVehicleExtension<SSD, SG>
    {
        private const uint DISTRICT_FLAG = 0x100000;
        private const uint BUILDING_FLAG = 0x200000;
        private const uint ID_PART = 0x0FFFFF;
        private const uint TYPE_PART = 0xF00000;

        public override SVMConfigWarehouse.ConfigIndex ConfigIndexKey
        {
            get {
                if (transform.parent == null && SVMController.instance != null)
                {
                    transform.SetParent(SVMController.instance.transform);
                }
                return SVMConfigWarehouse.getConfigAssetsForAI(definition);
            }
        }
        protected override bool AllowGlobal { get { return false; } }

        private List<string> basicAssetsList;

        private ServiceSystemDefinition definition => Singleton<SSD>.instance.GetSSD();

        public void Awake()
        {
            this.transform.SetParent(SVMController.instance.transform);
        }

        public uint GetCodedId(uint id, bool isDistrict)
        {
            return (isDistrict ? DISTRICT_FLAG : BUILDING_FLAG) | (id & ID_PART);
        }

        private void checkId(uint id)
        {
            if ((id & TYPE_PART) == 0)
            {
                throw new Exception("ID ENVIADO PARA EXTENSÃO DEVE SER PREFIXADO");
            }
        }

        #region Asset List
        private List<string> GetAssetList(uint codedId)
        {
            checkId(codedId);
            string value = SafeGet(codedId, BuildingConfig.MODELS);
            if (string.IsNullOrEmpty(value))
            {
                if (basicAssetsList == null) LoadBasicAssets();
                return basicAssetsList;
            }
            else
            {
                return value.Split(ItSepLvl3.ToCharArray()).ToList();
            }
        }
        private Dictionary<string, string> GetSelectedBasicAssets(uint codedId)
        {
            checkId(codedId);
            if (basicAssetsList == null) LoadBasicAssets();
            return GetAssetList(codedId).Where(x => PrefabCollection<VehicleInfo>.FindLoaded(x) != null).ToDictionary(x => x, x => Locale.Get("VEHICLE_TITLE", x));
        }
        private void AddAsset(uint codedId, string assetId)
        {
            checkId(codedId);
            var temp = GetAssetList(codedId);
            if (temp.Contains(assetId)) return;
            temp.Add(assetId);
            SafeSet(codedId, BuildingConfig.MODELS, string.Join(ItSepLvl3, temp.ToArray()));
        }
        private void RemoveAsset(uint codedId, string assetId)
        {
            checkId(codedId);
            var temp = GetAssetList(codedId);
            if (!temp.Contains(assetId)) return;
            temp.RemoveAll(x => x == assetId);
            SafeSet(codedId, BuildingConfig.MODELS, string.Join(ItSepLvl3, temp.ToArray()));
        }
        private void UseDefaultAssets(uint codedId)
        {
            checkId(codedId);
            SafeCleanProperty(codedId, BuildingConfig.MODELS);
        }

        public VehicleInfo GetAModel(uint buildingId)
        {
            uint codedId = buildingId | BUILDING_FLAG;
            if (buildingId > 0)
            {
                if (GetIgnoreDistrict(codedId))
                {
                    return SVMUtils.GetRandomModel(GetAssetList(codedId));
                }
            }
            return SVMUtils.GetRandomModel(GetAssetList(DISTRICT_FLAG | SVMUtils.GetBuildingDistrict(buildingId)));
        }
        public Dictionary<string, string> GetAllBasicAssets()
        {
            if (basicAssetsList == null) LoadBasicAssets();
            return basicAssetsList.ToDictionary(x => x, x => Locale.Get("VEHICLE_TITLE", x));
        }
        private void LoadBasicAssets()
        {
            basicAssetsList = SVMUtils.LoadBasicAssets(definition);
        }

        public Dictionary<string, string> GetSelectedBasicAssetsDistrict(uint district)
        {
            return GetSelectedBasicAssets(district | DISTRICT_FLAG);
        }

        public List<string> GetAssetListDistrict(uint district)
        {
            return GetAssetList(district | DISTRICT_FLAG);
        }

        public void AddAssetDistrict(uint district, string assetId)
        {
            AddAsset(district | DISTRICT_FLAG, assetId);
        }

        public void RemoveAssetDistrict(uint district, string assetId)
        {
            RemoveAsset(district | DISTRICT_FLAG, assetId);
        }

        public void UseDefaultAssetsDistrict(uint district)
        {
            UseDefaultAssets(district | DISTRICT_FLAG);
        }


        public Dictionary<string, string> GetSelectedBasicAssetsBuilding(uint buildingId)
        {
            return GetSelectedBasicAssets(buildingId | BUILDING_FLAG);
        }

        public List<string> GetAssetListBuilding(uint buildingId)
        {
            return GetAssetList(buildingId | BUILDING_FLAG);
        }

        public void AddAssetBuilding(uint buildingId, string assetId)
        {
            AddAsset(buildingId | BUILDING_FLAG, assetId);
        }

        public void RemoveAssetBuilding(uint buildingId, string assetId)
        {
            RemoveAsset(buildingId | BUILDING_FLAG, assetId);
        }

        public void UseDefaultAssetsBuilding(uint buildingId)
        {
            UseDefaultAssets(buildingId | BUILDING_FLAG);
        }

        #endregion

        #region Ignore District
        public bool GetIgnoreDistrict(uint buildingId)
        {
            return Boolean.TryParse(SafeGet(buildingId | BUILDING_FLAG, BuildingConfig.IGNORE_DISTRICT), out bool result) && result;
        }

        public void SetIgnoreDistrict(uint buildingId, bool value)
        {
            SafeSet(buildingId | BUILDING_FLAG, BuildingConfig.IGNORE_DISTRICT, value.ToString());
        }
        #endregion

        #region Color
        private Color32 GetColor(uint codedId)
        {
            if (SVMConfigWarehouse.allowColorChanging(ConfigIndexKey))
            {
                checkId(codedId);
                string value = SafeGet(codedId, BuildingConfig.COLOR);
                if (!string.IsNullOrEmpty(value))
                {
                    var list = value.Split(ItSepLvl3.ToCharArray()).ToList();
                    if (list.Count == 3 && byte.TryParse(list[0], out byte r) && byte.TryParse(list[1], out byte g) && byte.TryParse(list[2], out byte b))
                    {
                        return new Color32(r, g, b, 255);
                    }
                    else
                    {
                        SVMUtils.doLog($"val = {value}; list = {String.Join(",", list.ToArray())} (Size {list.Count})");
                    }
                }
            }
            return Color.clear;
        }

        private void SetColor(uint codedId, Color32 value)
        {
            if (!SVMConfigWarehouse.allowColorChanging(ConfigIndexKey)) return;
            checkId(codedId);
            if (value == Color.clear)
            {
                CleanColor(codedId);
            }
            else
            {
                SafeSet(codedId, BuildingConfig.COLOR, string.Join(ItSepLvl3, new string[] { value.r.ToString(), value.g.ToString(), value.b.ToString() }));
            }
        }
        private void CleanColor(uint codedId)
        {
            if (!SVMConfigWarehouse.allowColorChanging(ConfigIndexKey)) return;
            checkId(codedId);
            SafeCleanProperty(codedId, BuildingConfig.COLOR);
        }

        public Color32 GetEffectiveColorBuilding(uint buildingId)
        {
            if (GetIgnoreDistrict(buildingId))
            {
                return GetColor(buildingId | BUILDING_FLAG);
            }
            else
            {
                return GetColor(SVMUtils.GetBuildingDistrict(buildingId) | DISTRICT_FLAG);
            }
        }

        public Color32 GetColorDistrict(uint id)
        {
            return GetColor(id | DISTRICT_FLAG);
        }

        public void SetColorDistrict(uint id, Color32 value)
        {
            SetColor(id | DISTRICT_FLAG, value);
        }

        public void CleanColorDistrict(uint id)
        {
            CleanColor(id | DISTRICT_FLAG);
        }

        public Color32 GetColorBuilding(uint id)
        {
            return GetColor(id | BUILDING_FLAG);
        }

        public void SetColorBuilding(uint id, Color32 value)
        {
            SetColor(id | BUILDING_FLAG, value);
        }

        public void CleanColorBuilding(uint id)
        {
            CleanColor(id | BUILDING_FLAG);
        }

        #endregion
    }

    internal interface ISVMConfigIndexKeyContainer
    {
        SVMConfigWarehouse.ConfigIndex ConfigIndexKey { get; }
    }

    internal interface ISVMIgnorableDistrictExtensionValue : ISVMConfigIndexKeyContainer
    {
        bool GetIgnoreDistrict(uint buildingId);
        void SetIgnoreDistrict(uint buildingId, bool value);
    }


    internal interface ISVMColorSelectableExtensionValue : ISVMConfigIndexKeyContainer
    {
        Color32 GetColorDistrict(uint id);
        void SetColorDistrict(uint id, Color32 value);
        void CleanColorDistrict(uint id);

        Color32 GetColorBuilding(uint id);
        void SetColorBuilding(uint id, Color32 value);
        void CleanColorBuilding(uint id);


        Color32 GetEffectiveColorBuilding(uint id);
    }

    internal interface ISVMAssetSelectorExtension : ISVMConfigIndexKeyContainer
    {
        Dictionary<string, string> GetAllBasicAssets();
        VehicleInfo GetAModel(uint buildingId);

        Dictionary<string, string> GetSelectedBasicAssetsDistrict(uint rel);
        List<string> GetAssetListDistrict(uint district);
        void AddAssetDistrict(uint district, string assetId);
        void RemoveAssetDistrict(uint district, string assetId);
        void UseDefaultAssetsDistrict(uint district);

        Dictionary<string, string> GetSelectedBasicAssetsBuilding(uint rel);
        List<string> GetAssetListBuilding(uint district);
        void AddAssetBuilding(uint rel, string assetId);
        void RemoveAssetBuilding(uint rel, string assetId);
        void UseDefaultAssetsBuilding(uint rel);
    }


    internal sealed class SVMServiceVehicleExtensionDisCar : SVMServiceVehicleExtension<SVMSysDefDisCar, SVMServiceVehicleExtensionDisCar> { }
    internal sealed class SVMServiceVehicleExtensionDisHel : SVMServiceVehicleExtension<SVMSysDefDisHel, SVMServiceVehicleExtensionDisHel> { }
    internal sealed class SVMServiceVehicleExtensionFirCar : SVMServiceVehicleExtension<SVMSysDefFirCar, SVMServiceVehicleExtensionFirCar> { }
    internal sealed class SVMServiceVehicleExtensionFirHel : SVMServiceVehicleExtension<SVMSysDefFirHel, SVMServiceVehicleExtensionFirHel> { }
    internal sealed class SVMServiceVehicleExtensionGarCar : SVMServiceVehicleExtension<SVMSysDefGarCar, SVMServiceVehicleExtensionGarCar> { }
    internal sealed class SVMServiceVehicleExtensionGbcCar : SVMServiceVehicleExtension<SVMSysDefGbcCar, SVMServiceVehicleExtensionGbcCar> { }
    internal sealed class SVMServiceVehicleExtensionHcrCar : SVMServiceVehicleExtension<SVMSysDefHcrCar, SVMServiceVehicleExtensionHcrCar> { }
    internal sealed class SVMServiceVehicleExtensionHcrHel : SVMServiceVehicleExtension<SVMSysDefHcrHel, SVMServiceVehicleExtensionHcrHel> { }
    internal sealed class SVMServiceVehicleExtensionPolCar : SVMServiceVehicleExtension<SVMSysDefPolCar, SVMServiceVehicleExtensionPolCar> { }
    internal sealed class SVMServiceVehicleExtensionPolHel : SVMServiceVehicleExtension<SVMSysDefPolHel, SVMServiceVehicleExtensionPolHel> { }
    internal sealed class SVMServiceVehicleExtensionRoaCar : SVMServiceVehicleExtension<SVMSysDefRoaCar, SVMServiceVehicleExtensionRoaCar> { }
    internal sealed class SVMServiceVehicleExtensionWatCar : SVMServiceVehicleExtension<SVMSysDefWatCar, SVMServiceVehicleExtensionWatCar> { }
    internal sealed class SVMServiceVehicleExtensionPriCar : SVMServiceVehicleExtension<SVMSysDefPriCar, SVMServiceVehicleExtensionPriCar> { }
    internal sealed class SVMServiceVehicleExtensionDcrCar : SVMServiceVehicleExtension<SVMSysDefDcrCar, SVMServiceVehicleExtensionDcrCar> { }
    internal sealed class SVMServiceVehicleExtensionTaxCar : SVMServiceVehicleExtension<SVMSysDefTaxCar, SVMServiceVehicleExtensionTaxCar> { }
    internal sealed class SVMServiceVehicleExtensionCcrCcr : SVMServiceVehicleExtension<SVMSysDefCcrCcr, SVMServiceVehicleExtensionCcrCcr> { }
    internal sealed class SVMServiceVehicleExtensionSnwCar : SVMServiceVehicleExtension<SVMSysDefSnwCar, SVMServiceVehicleExtensionSnwCar> { }

    public sealed class SVMTransportExtensionUtils
    {

        public static void RemoveAllUnwantedVehicles()
        {
            //for (ushort codedId = 1; codedId < Singleton<BuildingManager>.instance.m_buildings.m_size; codedId++)
            //{
            //    if ((Singleton<BuildingManager>.instance.m_buildings.m_buffer[codedId].Info. & TransportLine.Flags.Created) != TransportLine.Flags.None)
            //    {
            //        uint idx;
            //        IAssetSelectorExtension extension;
            //        if (TLMTransportLineExtension.instance.GetUseCustomConfig(codedId))
            //        {
            //            idx = codedId;
            //            extension = TLMTransportLineExtension.instance;
            //        }
            //        else
            //        {
            //            idx = TLMLineUtils.getPrefix(codedId);
            //            var def = TransportSystemDefinition.from(codedId);
            //            extension = def.GetTransportExtension();
            //        }

            //        TransportLine tl = Singleton<TransportManager>.instance.m_lines.m_buffer[codedId];
            //        var modelList = extension.GetAssetList(idx);
            //        VehicleManager vm = Singleton<VehicleManager>.instance;
            //        VehicleInfo info = vm.m_vehicles.m_buffer[Singleton<TransportManager>.instance.m_lines.m_buffer[codedId].GetVehicle(0)].Info;

            //         SVMUtils.doLog("removeAllUnwantedVehicles: models found: {0}", modelList == null ? "?!?" : modelList.Count.ToString());

            //        if (modelList.Count > 0)
            //        {
            //            Dictionary<ushort, VehicleInfo> vehiclesToRemove = new Dictionary<ushort, VehicleInfo>();
            //            for (int i = 0; i < tl.CountVehicles(codedId); i++)
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

    internal enum BuildingConfig
    {
        MODELS,
        COLOR,
        IGNORE_DISTRICT
    }
}
