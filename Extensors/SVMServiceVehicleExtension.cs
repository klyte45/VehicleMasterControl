using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.Threading;
using Klyte.Commons.Interfaces;
using Klyte.ServiceVehiclesManager.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager.Extensors.VehicleExt
{
    internal interface ISVMTransportTypeExtension : ISVMAssetSelectorExtension, ISVMIgnorableDistrictExtensionValue, ISVMColorSelectableExtensionValue, ISVMDistrictServiceRestrictions
    {
    }

    internal abstract class SVMServiceVehicleExtension<SSD, SG> : ExtensionInterfaceDictionaryImpl<SVMConfigWarehouse, SVMConfigWarehouse.ConfigIndex, BuildingConfig, SG>, ISVMTransportTypeExtension where SSD : SVMSysDef<SSD>, new() where SG : SVMServiceVehicleExtension<SSD, SG>
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
                var def = definition;
                return SVMConfigWarehouse.getConfigAssetsForAI(ref def);
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
                return new List<string>();
            }
            else
            {
                return value.Split(ItSepLvl3.ToCharArray()).ToList();
            }
        }
        private void AddAsset(uint codedId, string assetId)
        {
            checkId(codedId);
            var temp = GetAssetList(codedId);
            if (temp.Contains(assetId)) return;
            temp.Add(assetId);
            SafeSet(codedId, BuildingConfig.MODELS, string.Join(ItSepLvl3, temp.Intersect(basicAssetsList).ToArray()));
        }
        private void RemoveAsset(uint codedId, string assetId)
        {
            checkId(codedId);
            var temp = GetAssetList(codedId);
            if (!temp.Contains(assetId)) return;
            temp.RemoveAll(x => x == assetId);
            SafeSet(codedId, BuildingConfig.MODELS, string.Join(ItSepLvl3, temp.Intersect(basicAssetsList).ToArray()));
        }
        private void UseDefaultAssets(uint codedId)
        {
            checkId(codedId);
            SafeCleanProperty(codedId, BuildingConfig.MODELS);
        }

        public VehicleInfo GetAModel(uint buildingId)
        {
            SVMUtils.doLog("[{0}] GetAModel", typeof(SSD).Name);
            List<string> assetList = GetEffectiveAssetList(buildingId, out uint targetCodedId);
            VehicleInfo info = null;
            while (info == null && assetList.Count > 0)
            {
                info = SVMUtils.GetRandomModel(assetList, out string modelName);
                if (info == null)
                {
                    RemoveAsset(targetCodedId, modelName);
                    assetList = GetEffectiveAssetList(buildingId, out targetCodedId);
                }
            }
            return info;
        }

        private List<string> GetEffectiveAssetList(uint buildingId, out uint targetCodedId)
        {
            uint codedId = buildingId | BUILDING_FLAG;
            List<string> assetList;
            if (buildingId > 0 && GetIgnoreDistrict(codedId))
            {
                assetList = GetAssetList(codedId);
                targetCodedId = codedId;
                SVMUtils.doLog("[{0}] GetAModel - assetList (Building) = {1}", typeof(SSD).Name, string.Join(",", assetList.ToArray()));
            }
            else
            {
                targetCodedId = DISTRICT_FLAG | SVMUtils.GetBuildingDistrict(buildingId);
                assetList = GetAssetList(targetCodedId);
                if (assetList == null || assetList.Count == 0)
                {
                    assetList = GetAssetList(DISTRICT_FLAG);
                }
                SVMUtils.doLog("[{0}] GetAModel - assetList (District) = {1}", typeof(SSD).Name, string.Join(",", assetList.ToArray()));
            }
            if ((assetList?.Count ?? 0) == 0)
            {
                if (basicAssetsList == null) LoadBasicAssets();
                assetList = basicAssetsList;
            }
            SVMUtils.doLog("[{0}] GetAModel - assetList (effective) = {1}", typeof(SSD).Name, string.Join(",", assetList.ToArray()));
            return assetList;
        }

        public bool IsModelCompatible(uint buildingId, VehicleInfo vehicleInfo)
        {
            return GetEffectiveAssetList(buildingId, out uint targetCodedId).Contains(vehicleInfo.name);
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


        public List<string> GetAssetListBuilding(uint building)
        {
            return GetAssetList(building | BUILDING_FLAG);
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
                return SVMUtils.DeserializeColor(value, ItSepLvl3);
            }
            return new Color32(0, 0, 0, 1);
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
                SafeSet(codedId, BuildingConfig.COLOR, SVMUtils.SerializeColor(value, ItSepLvl3));
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
                Color32 c = GetColor(buildingId | BUILDING_FLAG);
                if (c.a != 255)
                {
                    c = Color.clear;
                }
                return c;
            }
            else
            {
                Color32 c = GetColor(SVMUtils.GetBuildingDistrict(buildingId) | DISTRICT_FLAG);
                if (c.a != 255)
                {
                    c = GetColor(DISTRICT_FLAG);
                }
                if (c.a != 255)
                {
                    c = Color.clear;
                }
                return c;
            }
        }

        public Color32 GetColorDistrict(uint id)
        {
            var c = GetColor(id | DISTRICT_FLAG);
            if (c.a != 255)
            {
                c = Color.clear;
            }
            return c;
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
            var c = GetColor(id | BUILDING_FLAG);
            if (c.a != 255)
            {
                c = Color.clear;
            }
            return c;
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

        #region District service restrictions
        private bool? m_allowDistrictServiceRestrictions;

        public bool GetAllowDistrictServiceRestrictions()
        {
            if (m_allowDistrictServiceRestrictions == null)
            {
                m_allowDistrictServiceRestrictions = Singleton<SSD>.instance.GetSSD().service != ItemClass.Service.PublicTransport || Singleton<SSD>.instance.GetSSD().subService == ItemClass.SubService.PublicTransportTaxi;
            }
            return m_allowDistrictServiceRestrictions ?? false;
        }

        public bool? GetAllowOutsiders(uint district)
        {
            if (!GetAllowDistrictServiceRestrictions()) throw new Exception("This behaviour not applies to Public Transport (except Taxi)");
            if (!Boolean.TryParse(SafeGet(district | DISTRICT_FLAG, BuildingConfig.DISTRICT_ALLOW_OUTSIDERS), out bool result))
            {
                return null;
            }
            return result;
        }

        public void SetAllowOutsiders(uint district, bool value)
        {
            if (!GetAllowDistrictServiceRestrictions()) throw new Exception("This behaviour not applies to Public Transport (except Taxi)");
            SafeSet(district | DISTRICT_FLAG, BuildingConfig.DISTRICT_ALLOW_OUTSIDERS, value.ToString());
        }

        public bool? GetAllowGoOutside(uint district)
        {
            if (!GetAllowDistrictServiceRestrictions()) throw new Exception("This behaviour not applies to Public Transport (except Taxi)");
            if (!Boolean.TryParse(SafeGet(district | DISTRICT_FLAG, BuildingConfig.DISTRICT_ALLOW_GO_OTHERS), out bool result))
            {
                return null;
            }
            return result;
        }

        public void SetAllowGoOutside(uint district, bool value)
        {
            if (!GetAllowDistrictServiceRestrictions()) throw new Exception("This behaviour not applies to Public Transport (except Taxi)");
            SafeSet(district | DISTRICT_FLAG, BuildingConfig.DISTRICT_ALLOW_GO_OTHERS, value.ToString());
        }

        public bool GetAllowOutsidersEffective(uint district)
        {
            if (district == 0) return true;
            var value = GetAllowOutsiders(district);
            if (value == null)
            {
                return GetAllowOutsiders(0) ?? ServiceVehiclesManagerMod.allowOutsidersAsDefault;
            }
            return value ?? true;
        }

        public bool GetAllowGoOutsideEffective(uint district)
        {
            if (district == 0) return true;
            var value = GetAllowGoOutside(district);
            if (value == null)
            {
                return GetAllowGoOutside(0) ?? ServiceVehiclesManagerMod.allowGoOutsideAsDefault;
            }
            return value ?? true;
        }

        public void CleanAllowOutsiders(uint district)
        {
            SafeCleanProperty(district | DISTRICT_FLAG, BuildingConfig.DISTRICT_ALLOW_OUTSIDERS);
        }

        public void CleanAllowGoOutside(uint district)
        {
            SafeCleanProperty(district | DISTRICT_FLAG, BuildingConfig.DISTRICT_ALLOW_GO_OTHERS);
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

    internal interface ISVMDistrictServiceRestrictions : ISVMConfigIndexKeyContainer
    {
        bool GetAllowDistrictServiceRestrictions();

        bool? GetAllowOutsiders(uint district);
        bool GetAllowOutsidersEffective(uint district);
        void SetAllowOutsiders(uint district, bool value);
        void CleanAllowOutsiders(uint district);

        bool? GetAllowGoOutside(uint district);
        bool GetAllowGoOutsideEffective(uint district);
        void SetAllowGoOutside(uint district, bool value);
        void CleanAllowGoOutside(uint district);
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
        bool IsModelCompatible(uint buildingId, VehicleInfo vehicleInfo);

        List<string> GetAssetListDistrict(uint district);
        void AddAssetDistrict(uint district, string assetId);
        void RemoveAssetDistrict(uint district, string assetId);
        void UseDefaultAssetsDistrict(uint district);

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
    internal sealed class SVMServiceVehicleExtensionRegTra : SVMServiceVehicleExtension<SVMSysDefRegTra, SVMServiceVehicleExtensionRegTra> { }
    internal sealed class SVMServiceVehicleExtensionRegShp : SVMServiceVehicleExtension<SVMSysDefRegShp, SVMServiceVehicleExtensionRegShp> { }
    internal sealed class SVMServiceVehicleExtensionRegPln : SVMServiceVehicleExtension<SVMSysDefRegPln, SVMServiceVehicleExtensionRegPln> { }
    internal sealed class SVMServiceVehicleExtensionCrgTra : SVMServiceVehicleExtension<SVMSysDefCrgTra, SVMServiceVehicleExtensionCrgTra> { }
    internal sealed class SVMServiceVehicleExtensionCrgShp : SVMServiceVehicleExtension<SVMSysDefCrgShp, SVMServiceVehicleExtensionCrgShp> { }
    //internal sealed class SVMServiceVehicleExtensionOutTra : SVMServiceVehicleExtension<SVMSysDefOutTra, SVMServiceVehicleExtensionOutTra> { }
    //internal sealed class SVMServiceVehicleExtensionOutShp : SVMServiceVehicleExtension<SVMSysDefOutShp, SVMServiceVehicleExtensionOutShp> { }
    //internal sealed class SVMServiceVehicleExtensionOutPln : SVMServiceVehicleExtension<SVMSysDefOutPln, SVMServiceVehicleExtensionOutPln> { }
    //internal sealed class SVMServiceVehicleExtensionOutCar : SVMServiceVehicleExtension<SVMSysDefOutPln, SVMServiceVehicleExtensionOutCar> { }
    internal sealed class SVMServiceVehicleExtensionBeaCar : SVMServiceVehicleExtension<SVMSysDefBeaCar, SVMServiceVehicleExtensionBeaCar> { }
    internal sealed class SVMServiceVehicleExtensionPstCar : SVMServiceVehicleExtension<SVMSysDefPstCar, SVMServiceVehicleExtensionPstCar> { }
    internal sealed class SVMServiceVehicleExtensionPstTrk : SVMServiceVehicleExtension<SVMSysDefPstTrk, SVMServiceVehicleExtensionPstTrk> { }

    public sealed class SVMTransportExtensionUtils
    {

        public static void RemoveAllUnwantedVehicles()
        {
            new EnumerableActionThread(new Func<ThreadBase, IEnumerator>(SVMTransportExtensionUtils.RemoveAllUnwantedVehicles));
        }
        public static IEnumerator RemoveAllUnwantedVehicles(ThreadBase t)
        {
            ushort num = 0;
            while ((uint)num < Singleton<VehicleManager>.instance.m_vehicles.m_size)
            {
                var vehicle = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[(int)num];
                var vehicleInfo = vehicle.Info;
                if (vehicleInfo != null && !SVMUtils.IsTrailer(vehicleInfo) && vehicle.m_transportLine == 0 && vehicle.m_sourceBuilding > 0)
                {
                    var buildingInfo = Singleton<BuildingManager>.instance.m_buildings.m_buffer[vehicle.m_sourceBuilding].Info;
                    var buildingSsd = ServiceSystemDefinition.from(buildingInfo, vehicleInfo.m_vehicleType);
                    if (buildingSsd != null)
                    {
                        if (!buildingSsd.GetTransportExtension().IsModelCompatible(vehicle.m_sourceBuilding, vehicleInfo))
                        {
                            Singleton<VehicleManager>.instance.ReleaseVehicle(num);
                        }
                    }

                }
                if (num % 256 == 255)
                {
                    yield return num;
                }
                num++;
            }
            yield break;
        }

    }

    internal enum BuildingConfig
    {
        MODELS,
        COLOR,
        IGNORE_DISTRICT,
        DISTRICT_ALLOW_OUTSIDERS,
        DISTRICT_ALLOW_GO_OTHERS
    }
}
