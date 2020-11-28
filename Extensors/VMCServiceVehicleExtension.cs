using ColossalFramework;
using ColossalFramework.Globalization;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using Klyte.VehiclesMasterControl.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

namespace Klyte.VehiclesMasterControl.Extensors.VehicleExt
{
    public interface IVMCBuildingExtension : IAssetSelectorExtension, IVMCIgnorableDistrictExtensionValue, IColorSelectableExtension
    {
    }
    public interface IVMCDistrictExtension : IColorSelectableExtension, IAssetSelectorExtension, IVMCDistrictServiceRestrictionsExtension
    {
    }
    public interface IVMCBuildingStorage : IAssetSelectorStorage, IVMCIgnorableDistrictStorage, IColorSelectableStorage
    {
    }
    public interface IVMCDistrictStorage : IAssetSelectorStorage, IColorSelectableStorage, IVMCDistrictServiceRestrictionsStorage
    {
    }

    public abstract class VMCServiceVehicleExtension<SSD, STR, SG> : DataExtensorBase<SG>, IAssetSelectorExtension, IColorSelectableExtension where SSD : VMCSysDef<SSD>, new() where STR : IAssetSelectorStorage, IColorSelectableStorage, new() where SG : VMCServiceVehicleExtension<SSD, STR, SG>, new()
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
            m_basicAssetsList = VMCUtils.LoadBasicAssets(ref ssd).ToDictionary(x => x, x => Locale.Get("VEHICLE_TITLE", x));
        }

        #endregion

    }

    public class VMCBuildingInstanceExtensor<SSD> : VMCServiceVehicleExtension<SSD, VMCBuildingInstanceConfigStorage, VMCBuildingInstanceExtensor<SSD>>, IVMCBuildingExtension where SSD : VMCSysDef<SSD>, new()
    {
        public override string SaveId => $"K45_VMC_BuildingInstanceConfig_{typeof(SSD).Name}";
        IVMCIgnorableDistrictStorage ISafeGettable<IVMCIgnorableDistrictStorage>.SafeGet(uint index) => SafeGet(index);
    }
    public class VMCDistrictExtensor<SSD> : VMCServiceVehicleExtension<SSD, VMCDistrictConfigStorage, VMCDistrictExtensor<SSD>>, IVMCDistrictExtension where SSD : VMCSysDef<SSD>, new()
    {
        public override string SaveId => $"K45_VMC_DistricteConfig_{typeof(SSD).Name}";

        IVMCDistrictServiceRestrictionsStorage ISafeGettable<IVMCDistrictServiceRestrictionsStorage>.SafeGet(uint index) => SafeGet(index);
    }


    [XmlRoot("DistrictConfig")]
    public class VMCDistrictConfigStorage : IVMCDistrictStorage
    {
        [XmlAttribute("allowOutsiders")]
        public bool? AllowOutsiders { get; set; }
        [XmlAttribute("serveOtherDistricts")]
        public bool? ServeOtherDistricts { get; set; }
        [XmlAttribute("ignoreDistrict")]
        public bool IgnoreDistrict { get; set; }
        [XmlElement("Assets")]
        public SimpleXmlList<string> AssetList { get; set; } = new SimpleXmlList<string>();
        [XmlIgnore]
        public Color Color { get => m_cachedColor; set => m_cachedColor = value; }
        [XmlIgnore]
        private Color m_cachedColor;
        [XmlAttribute("color")]
        public string PropColorStr { get => m_cachedColor == default ? null : ColorExtensions.ToRGB(Color); set => m_cachedColor = value.IsNullOrWhiteSpace() ? default : (Color)ColorExtensions.FromRGB(value); }
    }

    [XmlRoot("BuildingInstanceConfig")]
    public class VMCBuildingInstanceConfigStorage : IVMCBuildingStorage
    {
        [XmlAttribute("ignoreDistrict")]
        public bool IgnoreDistrict { get; set; }

        [XmlElement("Assets")]
        public SimpleXmlList<string> AssetList { get; set; } = new SimpleXmlList<string>();
        [XmlIgnore]
        public Color Color { get => m_cachedColor; set => m_cachedColor = value; }
        [XmlIgnore]
        private Color m_cachedColor;
        [XmlAttribute("color")]
        public string PropColorStr { get => m_cachedColor == default ? null : ColorExtensions.ToRGB(Color); set => m_cachedColor = value.IsNullOrWhiteSpace() ? default : (Color)ColorExtensions.FromRGB(value); }
    }

    public interface IVMCIgnorableDistrictExtensionValue : ISafeGettable<IVMCIgnorableDistrictStorage>
    {
    }
    public interface IVMCIgnorableDistrictStorage
    {
        bool IgnoreDistrict { get; set; }
    }

    public interface IVMCDistrictServiceRestrictionsExtension : ISafeGettable<IVMCDistrictServiceRestrictionsStorage>
    {
    }

    public interface IVMCDistrictServiceRestrictionsStorage
    {
        bool? AllowOutsiders { get; set; }
        bool? ServeOtherDistricts { get; set; }
    }



    //public sealed class VMCServiceVehicleExtensionDisCar : VMCServiceVehicleExtension<VMCSysDefDisCar, VMCServiceVehicleExtensionDisCar> { }
    //public sealed class VMCServiceVehicleExtensionDisHel : VMCServiceVehicleExtension<VMCSysDefDisHel, VMCServiceVehicleExtensionDisHel> { }
    //public sealed class VMCServiceVehicleExtensionFirCar : VMCServiceVehicleExtension<VMCSysDefFirCar, VMCServiceVehicleExtensionFirCar> { }
    //public sealed class VMCServiceVehicleExtensionFirHel : VMCServiceVehicleExtension<VMCSysDefFirHel, VMCServiceVehicleExtensionFirHel> { }
    //public sealed class VMCServiceVehicleExtensionGarCar : VMCServiceVehicleExtension<VMCSysDefGarCar, VMCServiceVehicleExtensionGarCar> { }
    //public sealed class VMCServiceVehicleExtensionGbcCar : VMCServiceVehicleExtension<VMCSysDefGbcCar, VMCServiceVehicleExtensionGbcCar> { }
    //public sealed class VMCServiceVehicleExtensionHcrCar : VMCServiceVehicleExtension<VMCSysDefHcrCar, VMCServiceVehicleExtensionHcrCar> { }
    //public sealed class VMCServiceVehicleExtensionHcrHel : VMCServiceVehicleExtension<VMCSysDefHcrHel, VMCServiceVehicleExtensionHcrHel> { }
    //public sealed class VMCServiceVehicleExtensionPolCar : VMCServiceVehicleExtension<VMCSysDefPolCar, VMCServiceVehicleExtensionPolCar> { }
    //public sealed class VMCServiceVehicleExtensionPolHel : VMCServiceVehicleExtension<VMCSysDefPolHel, VMCServiceVehicleExtensionPolHel> { }
    //public sealed class VMCServiceVehicleExtensionRoaCar : VMCServiceVehicleExtension<VMCSysDefRoaCar, VMCServiceVehicleExtensionRoaCar> { }
    //public sealed class VMCServiceVehicleExtensionWatCar : VMCServiceVehicleExtension<VMCSysDefWatCar, VMCServiceVehicleExtensionWatCar> { }
    //public sealed class VMCServiceVehicleExtensionPriCar : VMCServiceVehicleExtension<VMCSysDefPriCar, VMCServiceVehicleExtensionPriCar> { }
    //public sealed class VMCServiceVehicleExtensionDcrCar : VMCServiceVehicleExtension<VMCSysDefDcrCar, VMCServiceVehicleExtensionDcrCar> { }
    //public sealed class VMCServiceVehicleExtensionTaxCar : VMCServiceVehicleExtension<VMCSysDefTaxCar, VMCServiceVehicleExtensionTaxCar> { }
    //public sealed class VMCServiceVehicleExtensionCcrCcr : VMCServiceVehicleExtension<VMCSysDefCcrCcr, VMCServiceVehicleExtensionCcrCcr> { }
    //public sealed class VMCServiceVehicleExtensionSnwCar : VMCServiceVehicleExtension<VMCSysDefSnwCar, VMCServiceVehicleExtensionSnwCar> { }
    //public sealed class VMCServiceVehicleExtensionRegTra : VMCServiceVehicleExtension<VMCSysDefRegTra, VMCServiceVehicleExtensionRegTra> { }
    //public sealed class VMCServiceVehicleExtensionRegShp : VMCServiceVehicleExtension<VMCSysDefRegShp, VMCServiceVehicleExtensionRegShp> { }
    //public sealed class VMCServiceVehicleExtensionRegPln : VMCServiceVehicleExtension<VMCSysDefRegPln, VMCServiceVehicleExtensionRegPln> { }
    //public sealed class VMCServiceVehicleExtensionCrgTra : VMCServiceVehicleExtension<VMCSysDefCrgTra, VMCServiceVehicleExtensionCrgTra> { }
    //public sealed class VMCServiceVehicleExtensionCrgShp : VMCServiceVehicleExtension<VMCSysDefCrgShp, VMCServiceVehicleExtensionCrgShp> { }
    ////public sealed class VMCServiceVehicleExtensionOutTra : VMCServiceVehicleExtension<VMCSysDefOutTra, VMCServiceVehicleExtensionOutTra> { }
    ////public sealed class VMCServiceVehicleExtensionOutShp : VMCServiceVehicleExtension<VMCSysDefOutShp, VMCServiceVehicleExtensionOutShp> { }
    ////public sealed class VMCServiceVehicleExtensionOutPln : VMCServiceVehicleExtension<VMCSysDefOutPln, VMCServiceVehicleExtensionOutPln> { }
    ////public sealed class VMCServiceVehicleExtensionOutCar : VMCServiceVehicleExtension<VMCSysDefOutPln, VMCServiceVehicleExtensionOutCar> { }
    //public sealed class VMCServiceVehicleExtensionBeaCar : VMCServiceVehicleExtension<VMCSysDefBeaCar, VMCServiceVehicleExtensionBeaCar> { }
    //public sealed class VMCServiceVehicleExtensionPstCar : VMCServiceVehicleExtension<VMCSysDefPstCar, VMCServiceVehicleExtensionPstCar> { }
    //public sealed class VMCServiceVehicleExtensionPstTrk : VMCServiceVehicleExtension<VMCSysDefPstTrk, VMCServiceVehicleExtensionPstTrk> { }

    public static class ExtensionStaticExtensionMethods
    {
        #region Assets List
        public static List<string> GetAssetList<T>(this T it, uint idx) where T : IAssetSelectorExtension => it.SafeGet(idx).AssetList;
        public static List<string> GetSelectedBasicAssets<T>(this T it, uint idx) where T : IAssetSelectorExtension => it.GetAssetList(idx)?.Intersect(it.GetAllBasicAssets().Keys).ToList();
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

        internal static List<string> GetEffectiveAssetList(uint buildingId, ref ServiceSystemDefinition ssd)
        {
            IVMCBuildingExtension buildingExtension = ssd.GetBuildingExtension();
            List<string> assetList = ssd.GetBuildingExtension().GetSelectedBasicAssets(buildingId);
            if (assetList == null || assetList.Count == 0)
            {
                var district = BuildingUtils.GetBuildingDistrict(buildingId);
                assetList = ssd.GetDistrictExtension().GetSelectedBasicAssets(BuildingUtils.GetBuildingDistrict(buildingId));
                if (district != 0 && (assetList == null || assetList.Count == 0))
                {
                    assetList = ssd.GetDistrictExtension().GetSelectedBasicAssets(0);
                }
            }
            return assetList;
        }

        public static bool IsModelCompatible(uint buildingId, VehicleInfo vehicleInfo, ref ServiceSystemDefinition ssd)
        {
            var effList = GetEffectiveAssetList(buildingId, ref ssd);
            return !ssd.GetBuildingExtension().GetAllBasicAssets().ContainsKey(vehicleInfo.name) || effList.Count == 0 || effList.Contains(vehicleInfo.name);
        }
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
        public static bool? GetAllowOutsiders<T>(this T it, uint prefix) where T : IVMCDistrictServiceRestrictionsExtension => it.SafeGet(prefix).AllowOutsiders;
        public static void SetAllowOutsiders<T>(this T it, uint prefix, bool value) where T : IVMCDistrictServiceRestrictionsExtension => it.SafeGet(prefix).AllowOutsiders = value;
        public static void ClearAllowOutsiders<T>(this T it, uint prefix) where T : IVMCDistrictServiceRestrictionsExtension => it.SafeGet(prefix).AllowOutsiders = null;
        public static bool? GetAllowServeOtherDistricts<T>(this T it, uint prefix) where T : IVMCDistrictServiceRestrictionsExtension => it.SafeGet(prefix).ServeOtherDistricts;
        public static void SetAllowServeOtherDistricts<T>(this T it, uint prefix, bool value) where T : IVMCDistrictServiceRestrictionsExtension => it.SafeGet(prefix).ServeOtherDistricts = value;
        public static void ClearServeOtherDistricts<T>(this T it, uint prefix) where T : IVMCDistrictServiceRestrictionsExtension => it.SafeGet(prefix).ServeOtherDistricts = null;
        #endregion
        #region Ignore District
        public static bool GetIgnoreDistrict<T>(this T it, uint prefix) where T : IVMCIgnorableDistrictExtensionValue => it.SafeGet(prefix).IgnoreDistrict;
        public static void SetIgnoreDistrict<T>(this T it, uint prefix, bool value) where T : IVMCIgnorableDistrictExtensionValue => it.SafeGet(prefix).IgnoreDistrict = value;
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
        //        return GetAllowOutsiders(0) ?? VehiclesMasterControlMod.allowOutsidersAsDefault;
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
        //        return GetAllowGoOutside(0) ?? VehiclesMasterControlMod.allowGoOutsideAsDefault;
        //    }
        //    return value ?? true;
        //}

        //public void CleanAllowOutsiders(uint district) => SafeCleanProperty(district | DISTRICT_FLAG, BuildingConfig.DISTRICT_ALLOW_OUTSIDERS);

        //public void CleanAllowGoOutside(uint district) => SafeCleanProperty(district | DISTRICT_FLAG, BuildingConfig.DISTRICT_ALLOW_GO_OTHERS);
        //#endregion
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
