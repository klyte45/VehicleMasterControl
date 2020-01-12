using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.Threading;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using Klyte.ServiceVehiclesManager.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager.Extensors.VehicleExt
{
    public interface ISVMBuildingExtension : IAssetSelectorExtension, ISVMIgnorableDistrictExtensionValue, IColorSelectableExtension
    {
    }
    public interface ISVMDistrictExtension : IColorSelectableExtension, IAssetSelectorExtension, ISVMDistrictServiceRestrictionsExtension
    {
    }
    public interface ISVMBuildingStorage : IAssetSelectorStorage, ISVMIgnorableDistrictStorage, IColorSelectableStorage
    {
    }
    public interface ISVMDistrictStorage : IAssetSelectorStorage, IColorSelectableStorage, ISVMDistrictServiceRestrictionsStorage
    {
    }

    public abstract class SVMServiceVehicleExtension<SSD, STR, SG> : DataExtensorBase<SG>, IAssetSelectorExtension, IColorSelectableExtension where SSD : SVMSysDef<SSD>, new() where STR : IAssetSelectorStorage, IColorSelectableStorage, new() where SG : SVMServiceVehicleExtension<SSD, STR, SG>, new()
    {
        private Dictionary<string, string> m_basicAssetsList;

        private ServiceSystemDefinition definition => SingletonLite<SSD>.instance.GetSSD();

        [XmlElement("Configurations")]
        public SimpleNonSequentialList<STR> Configurations { get; set; } = new SimpleNonSequentialList<STR>();


        public STR SafeGet(uint prefix)
        {
            if (!Configurations.ContainsKey(prefix))
            {
                Configurations[prefix] = new STR();
            }
            return Configurations[prefix];
        }
        IAssetSelectorStorage ISafeGettable<IAssetSelectorStorage>.SafeGet(uint index) => SafeGet(index);
        IColorSelectableStorage ISafeGettable<IColorSelectableStorage>.SafeGet(uint index) => SafeGet(index);


        #region Asset List
        public Dictionary<string, string> GetSelectedAssets(ushort idx)
        {
            if (m_basicAssetsList == null)
            {
                LoadBasicAssets();
            }
            List<string> assetList = ExtensionStaticExtensionMethods.GetAssetList(this, idx);
            return m_basicAssetsList.Where(x => assetList.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
        }
        public Dictionary<string, string> GetAllBasicAssets()
        {
            if (m_basicAssetsList == null)
            {
                LoadBasicAssets();
            }
            return m_basicAssetsList;

        }
        private void LoadBasicAssets()
        {
            ServiceSystemDefinition ssd = definition;
            m_basicAssetsList = SVMUtils.LoadBasicAssets(ref ssd).ToDictionary(x => x, x => Locale.Get("VEHICLE_TITLE", x));
        }

        #endregion

    }

    public class SVMBuildingInstanceExtensor<SSD> : SVMServiceVehicleExtension<SSD, SVMBuildingInstanceConfigStorage, SVMBuildingInstanceExtensor<SSD>>, ISVMBuildingExtension where SSD : SVMSysDef<SSD>, new()
    {
        public override string SaveId => $"K45_SVM_BuildingInstanceConfig_{typeof(SSD).Name}";
        ISVMIgnorableDistrictStorage ISafeGettable<ISVMIgnorableDistrictStorage>.SafeGet(uint index) => SafeGet(index);
    }
    public class SVMDistrictExtensor<SSD> : SVMServiceVehicleExtension<SSD, SVMDistrictConfigStorage, SVMDistrictExtensor<SSD>>, ISVMDistrictExtension where SSD : SVMSysDef<SSD>, new()
    {
        public override string SaveId => $"K45_SVM_DistricteConfig_{typeof(SSD).Name}";

        ISVMDistrictServiceRestrictionsStorage ISafeGettable<ISVMDistrictServiceRestrictionsStorage>.SafeGet(uint index) => SafeGet(index);
    }


    [XmlRoot("DistrictConfig")]
    public class SVMDistrictConfigStorage : ISVMDistrictStorage
    {
        [XmlAttribute("allowOutsiders")]
        public bool? AllowOutsiders { get; set; }
        [XmlAttribute("serveOtherDistricts")]
        public bool? ServeOtherDistricts { get; set; }
        [XmlAttribute("ignoreDistrict")]
        public bool IgnoreDistrict { get; set; }
        [XmlArray("Assets")]
        [XmlArrayItem("Asset")]
        public SimpleXmlList<string> AssetList { get; set; }
        [XmlIgnore]
        public Color Color { get => m_cachedColor; set => m_cachedColor = value; }
        [XmlIgnore]
        private Color m_cachedColor;
        [XmlAttribute("color")]
        public string PropColorStr { get => m_cachedColor == default ? null : ColorExtensions.ToRGB(Color); set => m_cachedColor = value.IsNullOrWhiteSpace() ? default : (Color) ColorExtensions.FromRGB(value); }
    }

    [XmlRoot("BuildingInstanceConfig")]
    public class SVMBuildingInstanceConfigStorage : ISVMBuildingStorage
    {
        [XmlAttribute("ignoreDistrict")]
        public bool IgnoreDistrict { get; set; }
        [XmlArray("Assets")]
        [XmlArrayItem("Asset")]
        public SimpleXmlList<string> AssetList { get; set; }
        [XmlIgnore]
        public Color Color { get => m_cachedColor; set => m_cachedColor = value; }
        [XmlIgnore]
        private Color m_cachedColor;
        [XmlAttribute("color")]
        public string PropColorStr { get => m_cachedColor == default ? null : ColorExtensions.ToRGB(Color); set => m_cachedColor = value.IsNullOrWhiteSpace() ? default : (Color) ColorExtensions.FromRGB(value); }
    }

    public interface ISVMIgnorableDistrictExtensionValue : ISafeGettable<ISVMIgnorableDistrictStorage>
    {
    }
    public interface ISVMIgnorableDistrictStorage
    {
        bool IgnoreDistrict { get; set; }
    }

    public interface ISVMDistrictServiceRestrictionsExtension : ISafeGettable<ISVMDistrictServiceRestrictionsStorage>
    {
    }

    public interface ISVMDistrictServiceRestrictionsStorage
    {
        bool? AllowOutsiders { get; set; }
        bool? ServeOtherDistricts { get; set; }
    }



    //public sealed class SVMServiceVehicleExtensionDisCar : SVMServiceVehicleExtension<SVMSysDefDisCar, SVMServiceVehicleExtensionDisCar> { }
    //public sealed class SVMServiceVehicleExtensionDisHel : SVMServiceVehicleExtension<SVMSysDefDisHel, SVMServiceVehicleExtensionDisHel> { }
    //public sealed class SVMServiceVehicleExtensionFirCar : SVMServiceVehicleExtension<SVMSysDefFirCar, SVMServiceVehicleExtensionFirCar> { }
    //public sealed class SVMServiceVehicleExtensionFirHel : SVMServiceVehicleExtension<SVMSysDefFirHel, SVMServiceVehicleExtensionFirHel> { }
    //public sealed class SVMServiceVehicleExtensionGarCar : SVMServiceVehicleExtension<SVMSysDefGarCar, SVMServiceVehicleExtensionGarCar> { }
    //public sealed class SVMServiceVehicleExtensionGbcCar : SVMServiceVehicleExtension<SVMSysDefGbcCar, SVMServiceVehicleExtensionGbcCar> { }
    //public sealed class SVMServiceVehicleExtensionHcrCar : SVMServiceVehicleExtension<SVMSysDefHcrCar, SVMServiceVehicleExtensionHcrCar> { }
    //public sealed class SVMServiceVehicleExtensionHcrHel : SVMServiceVehicleExtension<SVMSysDefHcrHel, SVMServiceVehicleExtensionHcrHel> { }
    //public sealed class SVMServiceVehicleExtensionPolCar : SVMServiceVehicleExtension<SVMSysDefPolCar, SVMServiceVehicleExtensionPolCar> { }
    //public sealed class SVMServiceVehicleExtensionPolHel : SVMServiceVehicleExtension<SVMSysDefPolHel, SVMServiceVehicleExtensionPolHel> { }
    //public sealed class SVMServiceVehicleExtensionRoaCar : SVMServiceVehicleExtension<SVMSysDefRoaCar, SVMServiceVehicleExtensionRoaCar> { }
    //public sealed class SVMServiceVehicleExtensionWatCar : SVMServiceVehicleExtension<SVMSysDefWatCar, SVMServiceVehicleExtensionWatCar> { }
    //public sealed class SVMServiceVehicleExtensionPriCar : SVMServiceVehicleExtension<SVMSysDefPriCar, SVMServiceVehicleExtensionPriCar> { }
    //public sealed class SVMServiceVehicleExtensionDcrCar : SVMServiceVehicleExtension<SVMSysDefDcrCar, SVMServiceVehicleExtensionDcrCar> { }
    //public sealed class SVMServiceVehicleExtensionTaxCar : SVMServiceVehicleExtension<SVMSysDefTaxCar, SVMServiceVehicleExtensionTaxCar> { }
    //public sealed class SVMServiceVehicleExtensionCcrCcr : SVMServiceVehicleExtension<SVMSysDefCcrCcr, SVMServiceVehicleExtensionCcrCcr> { }
    //public sealed class SVMServiceVehicleExtensionSnwCar : SVMServiceVehicleExtension<SVMSysDefSnwCar, SVMServiceVehicleExtensionSnwCar> { }
    //public sealed class SVMServiceVehicleExtensionRegTra : SVMServiceVehicleExtension<SVMSysDefRegTra, SVMServiceVehicleExtensionRegTra> { }
    //public sealed class SVMServiceVehicleExtensionRegShp : SVMServiceVehicleExtension<SVMSysDefRegShp, SVMServiceVehicleExtensionRegShp> { }
    //public sealed class SVMServiceVehicleExtensionRegPln : SVMServiceVehicleExtension<SVMSysDefRegPln, SVMServiceVehicleExtensionRegPln> { }
    //public sealed class SVMServiceVehicleExtensionCrgTra : SVMServiceVehicleExtension<SVMSysDefCrgTra, SVMServiceVehicleExtensionCrgTra> { }
    //public sealed class SVMServiceVehicleExtensionCrgShp : SVMServiceVehicleExtension<SVMSysDefCrgShp, SVMServiceVehicleExtensionCrgShp> { }
    ////public sealed class SVMServiceVehicleExtensionOutTra : SVMServiceVehicleExtension<SVMSysDefOutTra, SVMServiceVehicleExtensionOutTra> { }
    ////public sealed class SVMServiceVehicleExtensionOutShp : SVMServiceVehicleExtension<SVMSysDefOutShp, SVMServiceVehicleExtensionOutShp> { }
    ////public sealed class SVMServiceVehicleExtensionOutPln : SVMServiceVehicleExtension<SVMSysDefOutPln, SVMServiceVehicleExtensionOutPln> { }
    ////public sealed class SVMServiceVehicleExtensionOutCar : SVMServiceVehicleExtension<SVMSysDefOutPln, SVMServiceVehicleExtensionOutCar> { }
    //public sealed class SVMServiceVehicleExtensionBeaCar : SVMServiceVehicleExtension<SVMSysDefBeaCar, SVMServiceVehicleExtensionBeaCar> { }
    //public sealed class SVMServiceVehicleExtensionPstCar : SVMServiceVehicleExtension<SVMSysDefPstCar, SVMServiceVehicleExtensionPstCar> { }
    //public sealed class SVMServiceVehicleExtensionPstTrk : SVMServiceVehicleExtension<SVMSysDefPstTrk, SVMServiceVehicleExtensionPstTrk> { }

    public static class ExtensionStaticExtensionMethods
    {
        #region Assets List
        public static List<string> GetAssetList<T>(this T it, uint idx) where T : IAssetSelectorExtension => it.SafeGet(idx).AssetList;
        public static List<string> GetSelectedBasicAssets<T>(this T it, uint idx) where T : IAssetSelectorExtension => it.GetAssetList(idx).Intersect(it.GetAllBasicAssets().Keys).ToList();
        public static void AddAsset<T>(this T it, uint idx, string assetId) where T : IAssetSelectorExtension
        {
            List<string> list = it.GetAssetList(idx);
            if (list.Contains(assetId))
            {
                return;
            }
            list.Add(assetId);
        }
        public static void RemoveAsset<T>(this T it, uint idx, string assetId) where T : IAssetSelectorExtension
        {
            List<string> list = it.GetAssetList(idx);
            if (!list.Contains(assetId))
            {
                return;
            }
            list.RemoveAll(x => x == assetId);
        }
        public static void UseDefaultAssets<T>(this T it, uint idx) where T : IAssetSelectorExtension => it.GetAssetList(idx).Clear();

        private static List<string> GetEffectiveAssetList(uint buildingId, ref ServiceSystemDefinition ssd)
        {
            ISVMBuildingExtension buildingExtension = ssd.GetBuildingExtension();

            List<string> assetList = buildingExtension.GetSelectedBasicAssets(buildingId);
            if (assetList.Count == 0)
            {
                ISVMDistrictExtension districtExtension = ssd.GetDistrictExtension();
                assetList = districtExtension.GetSelectedBasicAssets(buildingId);
            }
            return assetList;
        }

        public static bool IsModelCompatible(uint buildingId, VehicleInfo vehicleInfo, ref ServiceSystemDefinition ssd) => GetEffectiveAssetList(buildingId, ref ssd).Contains(vehicleInfo.name);
        #endregion

        #region Color
        public static Color GetColor<T>(this T it, uint prefix) where T : IColorSelectableExtension => it.SafeGet(prefix).Color;

        public static void SetColor<T>(this T it, uint prefix, Color value) where T : IColorSelectableExtension
        {
            if (value.a < 1)
            {
                it.CleanColor(prefix);
            }
            else
            {
                it.SafeGet(prefix).Color = value;
            }
        }

        public static void CleanColor<T>(this T it, uint prefix) where T : IColorSelectableExtension => it.SafeGet(prefix).Color = default;
        #endregion
        #region District ServiceRestrictions
        public static bool? GetAllowOutsiders<T>(this T it, uint prefix) where T : ISVMDistrictServiceRestrictionsExtension => it.SafeGet(prefix).AllowOutsiders;
        public static void SetAllowOutsiders<T>(this T it, uint prefix, bool value) where T : ISVMDistrictServiceRestrictionsExtension => it.SafeGet(prefix).AllowOutsiders = value;
        public static void ClearAllowOutsiders<T>(this T it, uint prefix) where T : ISVMDistrictServiceRestrictionsExtension => it.SafeGet(prefix).AllowOutsiders = null;
        public static bool? GetAllowServeOtherDistricts<T>(this T it, uint prefix) where T : ISVMDistrictServiceRestrictionsExtension => it.SafeGet(prefix).ServeOtherDistricts;
        public static void SetAllowServeOtherDistricts<T>(this T it, uint prefix, bool value) where T : ISVMDistrictServiceRestrictionsExtension => it.SafeGet(prefix).ServeOtherDistricts = value;
        public static void ClearServeOtherDistricts<T>(this T it, uint prefix) where T : ISVMDistrictServiceRestrictionsExtension => it.SafeGet(prefix).ServeOtherDistricts = null;
        #endregion
        #region Ignore District
        public static bool GetIgnoreDistrict<T>(this T it, uint prefix) where T : ISVMIgnorableDistrictExtensionValue => it.SafeGet(prefix).IgnoreDistrict;
        public static void SetIgnoreDistrict<T>(this T it, uint prefix, bool value) where T : ISVMIgnorableDistrictExtensionValue => it.SafeGet(prefix).IgnoreDistrict = value;
        #endregion
        //#region District service restrictions
        //private bool? m_allowDistrictServiceRestrictions;

        //public bool GetAllowDistrictServiceRestrictions()
        //{
        //    if (m_allowDistrictServiceRestrictions == null)
        //    {
        //        m_allowDistrictServiceRestrictions = Singleton<SSD>.instance.GetSSD().service != ItemClass.Service.PublicTransport || Singleton<SSD>.instance.GetSSD().subService == ItemClass.SubService.PublicTransportTaxi;
        //    }
        //    return m_allowDistrictServiceRestrictions ?? false;
        //}

        //public bool? GetAllowOutsiders(uint district)
        //{
        //    if (!GetAllowDistrictServiceRestrictions())
        //    {
        //        throw new Exception("This behaviour not applies to Public Transport (except Taxi)");
        //    }

        //    if (!bool.TryParse(, out bool result))
        //    {
        //        return null;
        //    }
        //    return SafeGet(district);
        //}

        //public void SetAllowOutsiders(uint district, bool value)
        //{
        //    if (!GetAllowDistrictServiceRestrictions())
        //    {
        //        throw new Exception("This behaviour not applies to Public Transport (except Taxi)");
        //    }

        //    SafeSet(district | DISTRICT_FLAG, BuildingConfig.DISTRICT_ALLOW_OUTSIDERS, value.ToString());
        //}

        //public bool? GetAllowGoOutside(uint district)
        //{
        //    if (!GetAllowDistrictServiceRestrictions())
        //    {
        //        throw new Exception("This behaviour not applies to Public Transport (except Taxi)");
        //    }

        //    if (!bool.TryParse(SafeGet(district | DISTRICT_FLAG, BuildingConfig.DISTRICT_ALLOW_GO_OTHERS), out bool result))
        //    {
        //        return null;
        //    }
        //    return result;
        //}

        //public void SetAllowGoOutside(uint district, bool value)
        //{
        //    if (!GetAllowDistrictServiceRestrictions())
        //    {
        //        throw new Exception("This behaviour not applies to Public Transport (except Taxi)");
        //    }

        //    SafeSet(district | DISTRICT_FLAG, BuildingConfig.DISTRICT_ALLOW_GO_OTHERS, value.ToString());
        //}

        //public bool GetAllowOutsidersEffective(uint district)
        //{
        //    if (district == 0)
        //    {
        //        return true;
        //    }

        //    bool? value = GetAllowOutsiders(district);
        //    if (value == null)
        //    {
        //        return GetAllowOutsiders(0) ?? ServiceVehiclesManagerMod.allowOutsidersAsDefault;
        //    }
        //    return value ?? true;
        //}

        //public bool GetAllowGoOutsideEffective(uint district)
        //{
        //    if (district == 0)
        //    {
        //        return true;
        //    }

        //    bool? value = GetAllowGoOutside(district);
        //    if (value == null)
        //    {
        //        return GetAllowGoOutside(0) ?? ServiceVehiclesManagerMod.allowGoOutsideAsDefault;
        //    }
        //    return value ?? true;
        //}

        //public void CleanAllowOutsiders(uint district) => SafeCleanProperty(district | DISTRICT_FLAG, BuildingConfig.DISTRICT_ALLOW_OUTSIDERS);

        //public void CleanAllowGoOutside(uint district) => SafeCleanProperty(district | DISTRICT_FLAG, BuildingConfig.DISTRICT_ALLOW_GO_OTHERS);
        //#endregion
    }


    public sealed class SVMTransportExtensionUtils
    {

        public static void RemoveAllUnwantedVehicles() => new EnumerableActionThread(new Func<ThreadBase, IEnumerator>(SVMTransportExtensionUtils.RemoveAllUnwantedVehicles));
        public static IEnumerator RemoveAllUnwantedVehicles(ThreadBase t)
        {
            ushort num = 0;
            while (num < Singleton<VehicleManager>.instance.m_vehicles.m_size)
            {
                Vehicle vehicle = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[num];
                VehicleInfo vehicleInfo = vehicle.Info;
                if (vehicleInfo != null && !VehicleUtils.IsTrailer(vehicleInfo) && vehicle.m_transportLine == 0 && vehicle.m_sourceBuilding > 0)
                {
                    BuildingInfo buildingInfo = Singleton<BuildingManager>.instance.m_buildings.m_buffer[vehicle.m_sourceBuilding].Info;
                    var buildingSsd = ServiceSystemDefinition.from(buildingInfo, vehicleInfo.m_vehicleType);
                    if (buildingSsd != null)
                    {
                        if (!ExtensionStaticExtensionMethods.IsModelCompatible(vehicle.m_sourceBuilding, vehicleInfo, ref buildingSsd))
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

    public interface IAssetSelectorExtension : ISafeGettable<IAssetSelectorStorage>
    {
        Dictionary<string, string> GetAllBasicAssets();
    }

    public interface IAssetSelectorStorage
    {
        SimpleXmlList<string> AssetList { get; }
    }

    public interface IColorSelectableExtension : ISafeGettable<IColorSelectableStorage>
    {
    }
    public interface IColorSelectableStorage
    {
        Color Color { get; set; }
    }
    public interface ISafeGettable<T>
    {
        T SafeGet(uint index);
    }


}
