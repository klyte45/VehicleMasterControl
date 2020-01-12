using ColossalFramework;
using Klyte.Commons.Utils;
using Klyte.ServiceVehiclesManager.Overrides;
using Klyte.ServiceVehiclesManager.UI;
using Klyte.ServiceVehiclesManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager.Extensors.VehicleExt
{
    public struct ServiceSystemDefinition
    {
        public static readonly ServiceSystemDefinition DISASTER_CAR = new ServiceSystemDefinition(ItemClass.Service.Disaster, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition DISASTER_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.Disaster, ItemClass.SubService.None, VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition FIRE_CAR = new ServiceSystemDefinition(ItemClass.Service.FireDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition FIRE_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.FireDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition GARBAGE_CAR = new ServiceSystemDefinition(ItemClass.Service.Garbage, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition GARBBIO_CAR = new ServiceSystemDefinition(ItemClass.Service.Garbage, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);


        public static readonly ServiceSystemDefinition HEALTHCARE_CAR = new ServiceSystemDefinition(ItemClass.Service.HealthCare, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition HEALTHCARE_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.HealthCare, ItemClass.SubService.None, VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level3);
        public static readonly ServiceSystemDefinition DEATHCARE_CAR = new ServiceSystemDefinition(ItemClass.Service.HealthCare, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition POLICE_CAR = new ServiceSystemDefinition(ItemClass.Service.PoliceDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition POLICE_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.PoliceDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level3);

        internal bool GetAllowDistrictServiceRestrictions() => throw new NotImplementedException();

        public static readonly ServiceSystemDefinition ROAD_CAR = new ServiceSystemDefinition(ItemClass.Service.Road, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition WATER_CAR = new ServiceSystemDefinition(ItemClass.Service.Water, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition PRISION_CAR = new ServiceSystemDefinition(ItemClass.Service.PoliceDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level4);
        public static readonly ServiceSystemDefinition TAXI_CAR = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTaxi, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition CABLECAR_CABLECAR = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportCableCar, VehicleInfo.VehicleType.CableCar, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition SNOW_CAR = new ServiceSystemDefinition(ItemClass.Service.Road, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level4);
        public static readonly ServiceSystemDefinition REG_TRAIN = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain, VehicleInfo.VehicleType.Train, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition REG_PLANE = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPlane, VehicleInfo.VehicleType.Plane, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition REG_SHIP = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportShip, VehicleInfo.VehicleType.Ship, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition CARG_SHIP = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportShip, VehicleInfo.VehicleType.Ship, ItemClass.Level.Level4);
        public static readonly ServiceSystemDefinition CARG_TRAIN = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain, VehicleInfo.VehicleType.Train, ItemClass.Level.Level4);
        public static readonly ServiceSystemDefinition BEAU_CAR = new ServiceSystemDefinition(ItemClass.Service.Beautification, ItemClass.SubService.BeautificationParks, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition POST_CAR = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPost, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition POST_TRK = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPost, VehicleInfo.VehicleType.Car, ItemClass.Level.Level5);

        //public static readonly ServiceSystemDefinition OUT_TRAIN = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain, VehicleInfo.VehicleType.Train, ItemClass.Level.Level1, true);
        //public static readonly ServiceSystemDefinition OUT_PLANE = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPlane, VehicleInfo.VehicleType.Plane, ItemClass.Level.Level1, true);
        //public static readonly ServiceSystemDefinition OUT_SHIP = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportShip, VehicleInfo.VehicleType.Ship, ItemClass.Level.Level1, true);
        //public static readonly ServiceSystemDefinition OUT_ROAD = new ServiceSystemDefinition(ItemClass.Service.Road, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level5, true);   

        public bool AllowRestrictions { get; private set; }

        public long Idx
        {
            get; private set;
        }

        internal ISVMDistrictExtension GetDistrictExtension() => sysDefinitions[this].GetExtensionDistrict();
        internal ISVMBuildingExtension GetBuildingExtension() => sysDefinitions[this].GetExtensionBuilding();

        public static readonly Dictionary<ServiceSystemDefinition, ISVMBuildingExtension> m_availableDefinitions = new Dictionary<ServiceSystemDefinition, ISVMBuildingExtension>();
        public static Dictionary<ServiceSystemDefinition, ISVMSysDef> sysDefinitions
        {
            get {
                if (m_sysDefinitions.Count == 0)
                {
                    m_sysDefinitions[GARBAGE_CAR] = SingletonLite<SVMSysDefGarCar>.instance;
                    m_sysDefinitions[DEATHCARE_CAR] = SingletonLite<SVMSysDefDcrCar>.instance;
                    m_sysDefinitions[REG_PLANE] = SingletonLite<SVMSysDefRegPln>.instance;
                    m_sysDefinitions[REG_TRAIN] = SingletonLite<SVMSysDefRegTra>.instance;
                    m_sysDefinitions[REG_SHIP] = SingletonLite<SVMSysDefRegShp>.instance;
                    m_sysDefinitions[FIRE_CAR] = SingletonLite<SVMSysDefFirCar>.instance;
                    m_sysDefinitions[HEALTHCARE_CAR] = SingletonLite<SVMSysDefHcrCar>.instance;
                    m_sysDefinitions[POLICE_CAR] = SingletonLite<SVMSysDefPolCar>.instance;
                    m_sysDefinitions[CARG_TRAIN] = SingletonLite<SVMSysDefCrgTra>.instance;
                    m_sysDefinitions[CARG_SHIP] = SingletonLite<SVMSysDefCrgShp>.instance;

                    //m_sysDefinitions[OUT_PLANE] = SingletonLite<SVMSysDefOutPln>.instance;
                    //m_sysDefinitions[OUT_TRAIN] = SingletonLite<SVMSysDefOutTra>.instance;
                    //m_sysDefinitions[OUT_SHIP] = SingletonLite<SVMSysDefOutShp>.instance;
                    //m_sysDefinitions[OUT_ROAD] = SingletonLite<SVMSysDefOutCar>.instance;

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.AfterDark))
                    //{
                    m_sysDefinitions[PRISION_CAR] = SingletonLite<SVMSysDefPriCar>.instance;
                    m_sysDefinitions[TAXI_CAR] = SingletonLite<SVMSysDefTaxCar>.instance;
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.Snowfall))
                    //{
                    m_sysDefinitions[ROAD_CAR] = SingletonLite<SVMSysDefRoaCar>.instance;
                    m_sysDefinitions[SNOW_CAR] = SingletonLite<SVMSysDefSnwCar>.instance;
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.NaturalDisasters))
                    //{
                    m_sysDefinitions[WATER_CAR] = SingletonLite<SVMSysDefWatCar>.instance;
                    m_sysDefinitions[DISASTER_CAR] = SingletonLite<SVMSysDefDisCar>.instance;
                    m_sysDefinitions[DISASTER_HELICOPTER] = SingletonLite<SVMSysDefDisHel>.instance;
                    m_sysDefinitions[FIRE_HELICOPTER] = SingletonLite<SVMSysDefFirHel>.instance;
                    m_sysDefinitions[HEALTHCARE_HELICOPTER] = SingletonLite<SVMSysDefHcrHel>.instance;
                    m_sysDefinitions[POLICE_HELICOPTER] = SingletonLite<SVMSysDefPolHel>.instance;
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.InMotion))
                    //{
                    m_sysDefinitions[CABLECAR_CABLECAR] = SingletonLite<SVMSysDefCcrCcr>.instance;
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.GreenCities))
                    //{
                    m_sysDefinitions[GARBBIO_CAR] = SingletonLite<SVMSysDefGbcCar>.instance;
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.Parks))
                    //{
                    m_sysDefinitions[BEAU_CAR] = SingletonLite<SVMSysDefBeaCar>.instance;
                    //}
                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.Industry))
                    //{
                    m_sysDefinitions[POST_CAR] = SingletonLite<SVMSysDefPstCar>.instance;
                    m_sysDefinitions[POST_TRK] = SingletonLite<SVMSysDefPstTrk>.instance;
                    //}
                }
                return m_sysDefinitions;
            }
        }

        internal CategoryTab getCategory() => throw new NotImplementedException();
        internal string getNameForServiceSystem() => throw new NotImplementedException();
        internal string getFgIconServiceSystem() => throw new NotImplementedException();
        internal string getIconServiceSystem() => throw new NotImplementedException();

        private static readonly Dictionary<ServiceSystemDefinition, ISVMSysDef> m_sysDefinitions = new Dictionary<ServiceSystemDefinition, ISVMSysDef>();

        public ItemClass.Service service
        {
            get;
        }

        public ItemClass.SubService subService
        {
            get;
        }
        public VehicleInfo.VehicleType vehicleType
        {
            get;
        }
        public ItemClass.Level level
        {
            get;
        }
        public bool outsideConnection
        {
            get;
        }


        private ServiceSystemDefinition(
        ItemClass.Service service,
        ItemClass.SubService subService,
        VehicleInfo.VehicleType vehicleType,
        ItemClass.Level level,
        bool outsideConnection = false)
        {
            this.vehicleType = vehicleType;
            this.service = service;
            this.level = level;
            this.subService = subService;
            this.outsideConnection = outsideConnection;
            Idx = ((int) service << 48) | ((int) subService << 40) | ((int) level << 32) | (Mathf.RoundToInt(Mathf.Log((int) vehicleType) / Mathf.Log(2)) << 8) | (outsideConnection ? 1 : 0);
            AllowRestrictions = service != ItemClass.Service.PublicTransport || subService == ItemClass.SubService.PublicTransportTaxi;
        }

        public Type GetDefType() => sysDefinitions[this].GetType();

        public bool isFromSystem(VehicleInfo info) => info.m_class.m_service == service && subService == info.m_class.m_subService && info.m_vehicleType == vehicleType && info.m_class.m_level == level;

        public bool isFromSystem(BuildingInfo info)
        {
            if (ServiceVehiclesManagerMod.DebugMode)
            {
                //LogUtils.DoLog($"[{info?.GetAI()?.GetType()}->{this}]" +
                //    $" info.m_class.m_service == service = { info?.m_class?.m_service == service};" +
                //    $" subService == info.m_class.m_subService = { subService == info?.m_class?.m_subService };" +
                //    $" info?.GetAI() is OutsideConnectionAI == outsideConnection = {info?.GetAI() is OutsideConnectionAI == outsideConnection };" +
                //    $" info.m_class.m_level == level = {info?.m_class?.m_level} == {level} = {info?.m_class?.m_level == level};" +
                //    $" SVMBuildingAIOverrideUtils.getBuildingOverrideExtension(info).Count = {SVMBuildingAIOverrideUtils.getBuildingOverrideExtension(info)?.Count};" +
                //    $" ExtraAllowedLevels = [{string.Join(",", SVMBuildingAIOverrideUtils.getBuildingOverrideExtension(info).SelectMany(x => x?.ExtraAllowedLevels() ?? new List<ItemClass.Level>()).Select(x => x.ToString() ?? "<NULL>")?.ToArray())}];" +
                //    $" instance?.vehicleType ({vehicleType}) ;" +
                //    $" aiOverride?.AllowVehicleType(vehicleType) = {vehicleType}) ;" +
                //    $" SVMBuildingAIOverrideUtils.getBuildingOverrideExtension(info).Where = {ListAiOverrides(info).Count()} ");
            }
            return ListAiOverrides(info).Count() > 0;
        }

        private static IEnumerable<IBasicBuildingAIOverrides> ListAiOverrides(BuildingInfo info, ref ServiceSystemDefinition ssd)
        {
            if (SVMBuildingAIOverrideUtils.getBuildingOverrideExtension(info).Count == 0)
            {
                return new List<IBasicBuildingAIOverrides>();
            }
            ServiceSystemDefinition instance = ssd;
            return SVMBuildingAIOverrideUtils.getBuildingOverrideExtension(info).Where(aiOverride =>
               (info?.m_class?.m_service == instance.service)
            && instance.subService == info?.m_class?.m_subService
            && ((instance.outsideConnection) || info?.m_class?.m_level == instance.level || (aiOverride?.ExtraAllowedLevels()?.Contains(instance.level) ?? false))
            && (info?.GetAI() is OutsideConnectionAI) == instance.outsideConnection
            && SVMUtils.logAndReturn(aiOverride?.AllowVehicleType(SVMUtils.logAndReturn(instance.vehicleType, "EFF VEHICLE TYPE TESTED"), info?.GetAI()) ?? SVMUtils.logAndReturn(false, "AI OVERRIDE NULL!!!!!"), "AllowVehicleType")
            );
        }
        private IEnumerable<IBasicBuildingAIOverrides> ListAiOverrides(BuildingInfo info) => ListAiOverrides(info, ref this);

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() != typeof(ServiceSystemDefinition))
            {
                return false;
            }
            var other = (ServiceSystemDefinition) obj;

            return level == other.level && service == other.service && subService == other.subService && vehicleType == other.vehicleType && outsideConnection == other.outsideConnection;
        }

        public static bool operator ==(ServiceSystemDefinition a, ServiceSystemDefinition b)
        {
            if (Equals(a, null) || Equals(b, null))
            {
                return Equals(a, null) == Equals(b, null);
            }
            return a.Equals(b);
        }
        public static bool operator !=(ServiceSystemDefinition a, ServiceSystemDefinition b) => !(a == b);

        public static ServiceSystemDefinition from(VehicleInfo info)
        {
            if (info == null)
            {
                return default;
            }
            return sysDefinitions.Keys.FirstOrDefault(x => x.service == info.m_class.m_service && x.subService == info.m_class.m_subService && x.vehicleType == info.m_vehicleType && x.level == info.m_class.m_level && !x.outsideConnection);
        }

        public static ServiceSystemDefinition from(BuildingInfo info, VehicleInfo.VehicleType type)
        {
            if (info == null)
            {
                return default;
            }
            return sysDefinitions.Keys.FirstOrDefault(x => x.service == info.m_class.m_service && x.subService == info.m_class.m_subService && (x.level == info.m_class.m_level || x.outsideConnection) && x.vehicleType == type && x.outsideConnection == info.GetAI() is OutsideConnectionAI);
        }

        public static IEnumerable<ServiceSystemDefinition> from(BuildingInfo info)
        {
            if (info == null)
            {
                return new List<ServiceSystemDefinition>();
            }
            return sysDefinitions.Keys.Where(x => x.isFromSystem(info));
        }

        //public SVMConfigWarehouse.ConfigIndex toConfigIndex()
        //{
        //    ServiceSystemDefinition th = this;
        //    return SVMConfigWarehouse.getConfigServiceSystemForDefinition(ref th);
        //}

        public override string ToString() => service.ToString() + "|" + subService.ToString() + "|" + level.ToString() + "|" + vehicleType.ToString() + "|" + outsideConnection;

        public override int GetHashCode()
        {
            int hashCode = 286451371;
            hashCode = hashCode * -1521134295 + service.GetHashCode();
            hashCode = hashCode * -1521134295 + vehicleType.GetHashCode();
            hashCode = hashCode * -1521134295 + subService.GetHashCode();
            hashCode = hashCode * -1521134295 + vehicleType.GetHashCode();
            return hashCode;
        }


        public VehicleInfo GetAModel(ushort buildingId)
        {
            VehicleInfo info = null;
            List<string> assetList = GetBuildingExtension().GetSelectedBasicAssets(buildingId);
            if (assetList.Count == 0)
            {
                assetList = GetDistrictExtension().GetSelectedBasicAssets(BuildingUtils.GetBuildingDistrict(buildingId));
            }
            while (info == null && assetList.Count > 0)
            {
                info = VehicleUtils.GetRandomModel(assetList, out string modelName);
                if (info == null)
                {
                    GetBuildingExtension().RemoveAsset(buildingId, modelName);
                    GetDistrictExtension().RemoveAsset(BuildingUtils.GetBuildingDistrict(buildingId), modelName);
                    assetList.Remove(modelName);
                }
            }
            return info;
        }
        public Color GetModelColor(uint buildingId)
        {
            Color color = GetBuildingExtension().GetColor(buildingId);
            if (color == default)
            {
                color = GetDistrictExtension().GetColor(BuildingUtils.GetBuildingDistrict(buildingId));
            }
            return color;
        }
        internal bool AllowColorChanging() => !notAllowedColorChange.Contains(this);


        private static readonly ServiceSystemDefinition[] notAllowedColorChange = new ServiceSystemDefinition[]
        {
            FIRE_CAR                ,
            HEALTHCARE_CAR          ,
            POLICE_CAR              ,
            ROAD_CAR                ,
            SNOW_CAR                ,
            PRISION_CAR             ,
            TAXI_CAR                ,
        };
    }

    public interface ISVMSysDef
    {
        public abstract ServiceSystemDefinition GetSSD();
        public ISVMBuildingExtension GetExtensionBuilding();
        public ISVMDistrictExtension GetExtensionDistrict();
    }
    public abstract class SVMSysDef<T> : SingletonLite<T>, ISVMSysDef where T : SVMSysDef<T>, ISVMSysDef, new()
    {
        public abstract ServiceSystemDefinition GetSSD();
        public ISVMBuildingExtension GetExtensionBuilding() => SVMBuildingInstanceExtensor<T>.Instance;
        public ISVMDistrictExtension GetExtensionDistrict() => SVMDistrictExtensor<T>.Instance;
    }
    public sealed class SVMSysDefDisCar : SVMSysDef<SVMSysDefDisCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.DISASTER_CAR; }
    public sealed class SVMSysDefDisHel : SVMSysDef<SVMSysDefDisHel> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.DISASTER_HELICOPTER; }
    public sealed class SVMSysDefFirCar : SVMSysDef<SVMSysDefFirCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.FIRE_CAR; }
    public sealed class SVMSysDefFirHel : SVMSysDef<SVMSysDefFirHel> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.FIRE_HELICOPTER; }
    public sealed class SVMSysDefGarCar : SVMSysDef<SVMSysDefGarCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.GARBAGE_CAR; }
    public sealed class SVMSysDefGbcCar : SVMSysDef<SVMSysDefGbcCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.GARBBIO_CAR; }
    public sealed class SVMSysDefHcrCar : SVMSysDef<SVMSysDefHcrCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.HEALTHCARE_CAR; }
    public sealed class SVMSysDefHcrHel : SVMSysDef<SVMSysDefHcrHel> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.HEALTHCARE_HELICOPTER; }
    public sealed class SVMSysDefPolCar : SVMSysDef<SVMSysDefPolCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.POLICE_CAR; }
    public sealed class SVMSysDefPolHel : SVMSysDef<SVMSysDefPolHel> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.POLICE_HELICOPTER; }
    public sealed class SVMSysDefRoaCar : SVMSysDef<SVMSysDefRoaCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.ROAD_CAR; }
    public sealed class SVMSysDefDcrCar : SVMSysDef<SVMSysDefDcrCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.DEATHCARE_CAR; }
    public sealed class SVMSysDefWatCar : SVMSysDef<SVMSysDefWatCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.WATER_CAR; }
    public sealed class SVMSysDefPriCar : SVMSysDef<SVMSysDefPriCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.PRISION_CAR; }
    public sealed class SVMSysDefTaxCar : SVMSysDef<SVMSysDefTaxCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.TAXI_CAR; }
    public sealed class SVMSysDefCcrCcr : SVMSysDef<SVMSysDefCcrCcr> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.CABLECAR_CABLECAR; }
    public sealed class SVMSysDefSnwCar : SVMSysDef<SVMSysDefSnwCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.SNOW_CAR; }
    public sealed class SVMSysDefRegShp : SVMSysDef<SVMSysDefRegShp> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.REG_SHIP; }
    public sealed class SVMSysDefRegTra : SVMSysDef<SVMSysDefRegTra> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.REG_TRAIN; }
    public sealed class SVMSysDefRegPln : SVMSysDef<SVMSysDefRegPln> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.REG_PLANE; }
    public sealed class SVMSysDefCrgTra : SVMSysDef<SVMSysDefCrgTra> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.CARG_TRAIN; }
    public sealed class SVMSysDefCrgShp : SVMSysDef<SVMSysDefCrgShp> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.CARG_SHIP; }
    //public sealed class SVMSysDefOutShp : SVMSysDef<SVMSysDefOutShp> { public override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.OUT_SHIP; } }
    //public sealed class SVMSysDefOutTra : SVMSysDef<SVMSysDefOutTra> { public override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.OUT_TRAIN; } }
    //public sealed class SVMSysDefOutPln : SVMSysDef<SVMSysDefOutPln> { public override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.OUT_PLANE; } }
    //public sealed class SVMSysDefOutCar : SVMSysDef<SVMSysDefOutCar> { public override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.OUT_ROAD; } }
    public sealed class SVMSysDefBeaCar : SVMSysDef<SVMSysDefBeaCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.BEAU_CAR; }
    public sealed class SVMSysDefPstCar : SVMSysDef<SVMSysDefPstCar> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.POST_CAR; }
    public sealed class SVMSysDefPstTrk : SVMSysDef<SVMSysDefPstTrk> { public override ServiceSystemDefinition GetSSD() => ServiceSystemDefinition.POST_TRK; }
}
