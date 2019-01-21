using ColossalFramework;
using Klyte.ServiceVehiclesManager.Overrides;
using Klyte.ServiceVehiclesManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Klyte.ServiceVehiclesManager.Extensors.VehicleExt
{
    internal class ServiceSystemDefinition
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

        //public static readonly ServiceSystemDefinition OUT_TRAIN = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain, VehicleInfo.VehicleType.Train, ItemClass.Level.Level1, true);
        //public static readonly ServiceSystemDefinition OUT_PLANE = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPlane, VehicleInfo.VehicleType.Plane, ItemClass.Level.Level1, true);
        //public static readonly ServiceSystemDefinition OUT_SHIP = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportShip, VehicleInfo.VehicleType.Ship, ItemClass.Level.Level1, true);
        //public static readonly ServiceSystemDefinition OUT_ROAD = new ServiceSystemDefinition(ItemClass.Service.Road, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level5, true);


        public static Dictionary<ServiceSystemDefinition, ISVMTransportTypeExtension> availableDefinitions
        {
            get {
                if (m_availableDefinitions.Count == 0)
                {
                    m_availableDefinitions[GARBAGE_CAR] = SVMServiceVehicleExtensionGarCar.instance;
                    m_availableDefinitions[DEATHCARE_CAR] = SVMServiceVehicleExtensionDcrCar.instance;
                    m_availableDefinitions[REG_TRAIN] = SVMServiceVehicleExtensionRegTra.instance;
                    m_availableDefinitions[REG_SHIP] = SVMServiceVehicleExtensionRegShp.instance;
                    m_availableDefinitions[REG_PLANE] = SVMServiceVehicleExtensionRegPln.instance;
                    m_availableDefinitions[FIRE_CAR] = SVMServiceVehicleExtensionFirCar.instance;
                    m_availableDefinitions[HEALTHCARE_CAR] = SVMServiceVehicleExtensionHcrCar.instance;
                    m_availableDefinitions[POLICE_CAR] = SVMServiceVehicleExtensionPolCar.instance;


                    m_availableDefinitions[CARG_TRAIN] = SVMServiceVehicleExtensionCrgTra.instance;
                    m_availableDefinitions[CARG_SHIP] = SVMServiceVehicleExtensionCrgShp.instance;

                    //m_availableDefinitions[OUT_TRAIN] = SVMServiceVehicleExtensionOutTra.instance;
                    //m_availableDefinitions[OUT_SHIP] = SVMServiceVehicleExtensionOutShp.instance;
                    //m_availableDefinitions[OUT_PLANE] = SVMServiceVehicleExtensionOutPln.instance;
                    //m_availableDefinitions[OUT_ROAD] = SVMServiceVehicleExtensionOutCar.instance;

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.AfterDark))
                    //{
                        m_availableDefinitions[PRISION_CAR] = SVMServiceVehicleExtensionPriCar.instance;
                        m_availableDefinitions[TAXI_CAR] = SVMServiceVehicleExtensionTaxCar.instance;
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.Snowfall))
                    //{
                        m_availableDefinitions[ROAD_CAR] = SVMServiceVehicleExtensionRoaCar.instance;
                        m_availableDefinitions[SNOW_CAR] = SVMServiceVehicleExtensionSnwCar.instance;
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.NaturalDisasters))
                    //{
                        m_availableDefinitions[DISASTER_CAR] = SVMServiceVehicleExtensionDisCar.instance;
                        m_availableDefinitions[DISASTER_HELICOPTER] = SVMServiceVehicleExtensionDisHel.instance;
                        m_availableDefinitions[FIRE_HELICOPTER] = SVMServiceVehicleExtensionFirHel.instance;
                        m_availableDefinitions[HEALTHCARE_HELICOPTER] = SVMServiceVehicleExtensionHcrHel.instance;
                        m_availableDefinitions[POLICE_HELICOPTER] = SVMServiceVehicleExtensionPolHel.instance;
                        m_availableDefinitions[WATER_CAR] = SVMServiceVehicleExtensionWatCar.instance;
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.InMotion))
                    //{
                        m_availableDefinitions[CABLECAR_CABLECAR] = SVMServiceVehicleExtensionCcrCcr.instance;
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.GreenCities))
                    //{
                        m_availableDefinitions[GARBBIO_CAR] = SVMServiceVehicleExtensionGbcCar.instance;
                    //}
                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.Parks))
                    //{
                        m_availableDefinitions[BEAU_CAR] = SVMServiceVehicleExtensionBeaCar.instance;
                    //}
                }
                return m_availableDefinitions;
            }
        }
        public static readonly Dictionary<ServiceSystemDefinition, ISVMTransportTypeExtension> m_availableDefinitions = new Dictionary<ServiceSystemDefinition, ISVMTransportTypeExtension>();
        public static Dictionary<ServiceSystemDefinition, Type> sysDefinitions
        {
            get {
                if (m_sysDefinitions.Count == 0)
                {
                    m_sysDefinitions[GARBAGE_CAR] = typeof(SVMSysDefGarCar);
                    m_sysDefinitions[DEATHCARE_CAR] = typeof(SVMSysDefDcrCar);
                    m_sysDefinitions[REG_PLANE] = typeof(SVMSysDefRegPln);
                    m_sysDefinitions[REG_TRAIN] = typeof(SVMSysDefRegTra);
                    m_sysDefinitions[REG_SHIP] = typeof(SVMSysDefRegShp);
                    m_sysDefinitions[FIRE_CAR] = typeof(SVMSysDefFirCar);
                    m_sysDefinitions[HEALTHCARE_CAR] = typeof(SVMSysDefHcrCar);
                    m_sysDefinitions[POLICE_CAR] = typeof(SVMSysDefPolCar);
                    m_sysDefinitions[CARG_TRAIN] = typeof(SVMSysDefCrgTra);
                    m_sysDefinitions[CARG_SHIP] = typeof(SVMSysDefCrgShp);

                    //m_sysDefinitions[OUT_PLANE] = typeof(SVMSysDefOutPln);
                    //m_sysDefinitions[OUT_TRAIN] = typeof(SVMSysDefOutTra);
                    //m_sysDefinitions[OUT_SHIP] = typeof(SVMSysDefOutShp);
                    //m_sysDefinitions[OUT_ROAD] = typeof(SVMSysDefOutCar);

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.AfterDark))
                    //{
                        m_sysDefinitions[PRISION_CAR] = typeof(SVMSysDefPriCar);
                        m_sysDefinitions[TAXI_CAR] = typeof(SVMSysDefTaxCar);
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.Snowfall))
                    //{
                        m_sysDefinitions[ROAD_CAR] = typeof(SVMSysDefRoaCar);
                        m_sysDefinitions[SNOW_CAR] = typeof(SVMSysDefSnwCar);
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.NaturalDisasters))
                    //{
                        m_sysDefinitions[WATER_CAR] = typeof(SVMSysDefWatCar);
                        m_sysDefinitions[DISASTER_CAR] = typeof(SVMSysDefDisCar);
                        m_sysDefinitions[DISASTER_HELICOPTER] = typeof(SVMSysDefDisHel);
                        m_sysDefinitions[FIRE_HELICOPTER] = typeof(SVMSysDefFirHel);
                        m_sysDefinitions[HEALTHCARE_HELICOPTER] = typeof(SVMSysDefHcrHel);
                        m_sysDefinitions[POLICE_HELICOPTER] = typeof(SVMSysDefPolHel);
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.InMotion))
                    //{
                        m_sysDefinitions[CABLECAR_CABLECAR] = typeof(SVMSysDefCcrCcr);
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.GreenCities))
                    //{
                        m_sysDefinitions[GARBBIO_CAR] = typeof(SVMSysDefGbcCar);
                    //}

                    //if (Singleton<LoadingManager>.instance.SupportsExpansion(ICities.Expansion.Parks))
                    //{
                        m_sysDefinitions[BEAU_CAR] = typeof(SVMSysDefBeaCar);
                    //}
                }
                return m_sysDefinitions;
            }
        }
        private static readonly Dictionary<ServiceSystemDefinition, Type> m_sysDefinitions = new Dictionary<ServiceSystemDefinition, Type>();

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
        }

        internal ISVMTransportTypeExtension GetTransportExtension()
        {
            return availableDefinitions[this];
        }

        internal Type GetDefType()
        {
            return sysDefinitions[this];
        }

        public bool isFromSystem(VehicleInfo info)
        {
            return info.m_class.m_service == service && subService == info.m_class.m_subService && info.m_vehicleType == vehicleType && info.m_class.m_level == level;
        }

        public bool isFromSystem(BuildingInfo info)
        {
            if (ServiceVehiclesManagerMod.debugMode)
            {
                SVMUtils.doLog("[{4}->{5}] info.m_class.m_service == service = {0}; subService == info.m_class.m_subService = {1}; info.m_class.m_level == level = {2}; aiOverride?.AllowVehicleType(vehicleType) = {3} ", info?.m_class?.m_service == service, subService == info?.m_class?.m_subService, info?.m_class?.m_level == level, SVMBuildingAIOverrideUtils.getBuildingOverrideExtension(info)?.AllowVehicleType(vehicleType, info?.GetAI()), info?.GetAI()?.GetType(), this);
            }
            return info?.m_class?.m_service == service && subService == info?.m_class?.m_subService && (info?.m_class?.m_level == level || outsideConnection) && info?.GetAI() is OutsideConnectionAI == outsideConnection && (SVMBuildingAIOverrideUtils.getBuildingOverrideExtension(info)?.AllowVehicleType(vehicleType, info?.GetAI()) ?? false);
        }

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
            ServiceSystemDefinition other = (ServiceSystemDefinition)obj;

            return level == other.level && service == other.service && subService == other.subService && vehicleType == other.vehicleType && outsideConnection == other.outsideConnection;
        }

        public static bool operator ==(ServiceSystemDefinition a, ServiceSystemDefinition b)
        {
            if (Object.Equals(a, null) || Object.Equals(b, null))
            {
                return Object.Equals(a, null) == Object.Equals(b, null);
            }
            return a.Equals(b);
        }
        public static bool operator !=(ServiceSystemDefinition a, ServiceSystemDefinition b)
        {
            return !(a == b);
        }

        public static ServiceSystemDefinition from(VehicleInfo info)
        {
            if (info == null)
            {
                return default(ServiceSystemDefinition);
            }
            return availableDefinitions.Keys.FirstOrDefault(x => x.service == info.m_class.m_service && x.subService == info.m_class.m_subService && x.vehicleType == info.m_vehicleType && x.level == info.m_class.m_level && !x.outsideConnection);
        }

        public static ServiceSystemDefinition from(BuildingInfo info, VehicleInfo.VehicleType type)
        {
            if (info == null)
            {
                return default(ServiceSystemDefinition);
            }
            return availableDefinitions.Keys.FirstOrDefault(x => x.service == info.m_class.m_service && x.subService == info.m_class.m_subService && (x.level == info.m_class.m_level || x.outsideConnection) && x.vehicleType == type && x.outsideConnection == info.GetAI() is OutsideConnectionAI);
        }

        public static IEnumerable<ServiceSystemDefinition> from(BuildingInfo info)
        {
            if (info == null)
            {
                return new List<ServiceSystemDefinition>();
            }
            return availableDefinitions.Keys.Where(x => x.service == info.m_class.m_service && x.subService == info.m_class.m_subService && (x.level == info.m_class.m_level || x.outsideConnection) && x.outsideConnection == info.GetAI() is OutsideConnectionAI && (SVMBuildingAIOverrideUtils.getBuildingOverrideExtension(info)?.AllowVehicleType(x.vehicleType, info.GetAI()) ?? false));
        }

        public SVMConfigWarehouse.ConfigIndex toConfigIndex()
        {
            var th = this;
            return SVMConfigWarehouse.getConfigServiceSystemForDefinition(ref th);
        }

        public override string ToString()
        {
            return service.ToString() + "|" + subService.ToString() + "|" + level.ToString() + "|" + vehicleType.ToString() + "|" + outsideConnection;
        }

        public override int GetHashCode()
        {
            var hashCode = 286451371;
            hashCode = hashCode * -1521134295 + service.GetHashCode();
            hashCode = hashCode * -1521134295 + vehicleType.GetHashCode();
            hashCode = hashCode * -1521134295 + subService.GetHashCode();
            hashCode = hashCode * -1521134295 + vehicleType.GetHashCode();
            return hashCode;
        }
    }

    internal abstract class SVMSysDef<T> : Singleton<T> where T : SVMSysDef<T>
    {
        internal abstract ServiceSystemDefinition GetSSD();
        public void Awake()
        {
            this.transform.SetParent(SVMController.instance.transform);
        }
    }
    internal sealed class SVMSysDefDisCar : SVMSysDef<SVMSysDefDisCar> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.DISASTER_CAR; } }
    internal sealed class SVMSysDefDisHel : SVMSysDef<SVMSysDefDisHel> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.DISASTER_HELICOPTER; } }
    internal sealed class SVMSysDefFirCar : SVMSysDef<SVMSysDefFirCar> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.FIRE_CAR; } }
    internal sealed class SVMSysDefFirHel : SVMSysDef<SVMSysDefFirHel> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.FIRE_HELICOPTER; } }
    internal sealed class SVMSysDefGarCar : SVMSysDef<SVMSysDefGarCar> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.GARBAGE_CAR; } }
    internal sealed class SVMSysDefGbcCar : SVMSysDef<SVMSysDefGbcCar> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.GARBBIO_CAR; } }
    internal sealed class SVMSysDefHcrCar : SVMSysDef<SVMSysDefHcrCar> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.HEALTHCARE_CAR; } }
    internal sealed class SVMSysDefHcrHel : SVMSysDef<SVMSysDefHcrHel> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.HEALTHCARE_HELICOPTER; } }
    internal sealed class SVMSysDefPolCar : SVMSysDef<SVMSysDefPolCar> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.POLICE_CAR; } }
    internal sealed class SVMSysDefPolHel : SVMSysDef<SVMSysDefPolHel> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.POLICE_HELICOPTER; } }
    internal sealed class SVMSysDefRoaCar : SVMSysDef<SVMSysDefRoaCar> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.ROAD_CAR; } }
    internal sealed class SVMSysDefDcrCar : SVMSysDef<SVMSysDefDcrCar> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.DEATHCARE_CAR; } }
    internal sealed class SVMSysDefWatCar : SVMSysDef<SVMSysDefWatCar> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.WATER_CAR; } }
    internal sealed class SVMSysDefPriCar : SVMSysDef<SVMSysDefPriCar> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.PRISION_CAR; } }
    internal sealed class SVMSysDefTaxCar : SVMSysDef<SVMSysDefTaxCar> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.TAXI_CAR; } }
    internal sealed class SVMSysDefCcrCcr : SVMSysDef<SVMSysDefCcrCcr> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.CABLECAR_CABLECAR; } }
    internal sealed class SVMSysDefSnwCar : SVMSysDef<SVMSysDefSnwCar> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.SNOW_CAR; } }
    internal sealed class SVMSysDefRegShp : SVMSysDef<SVMSysDefRegShp> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.REG_SHIP; } }
    internal sealed class SVMSysDefRegTra : SVMSysDef<SVMSysDefRegTra> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.REG_TRAIN; } }
    internal sealed class SVMSysDefRegPln : SVMSysDef<SVMSysDefRegPln> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.REG_PLANE; } }
    internal sealed class SVMSysDefCrgTra : SVMSysDef<SVMSysDefCrgTra> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.CARG_TRAIN; } }
    internal sealed class SVMSysDefCrgShp : SVMSysDef<SVMSysDefCrgShp> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.CARG_SHIP; } }
    //internal sealed class SVMSysDefOutShp : SVMSysDef<SVMSysDefOutShp> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.OUT_SHIP; } }
    //internal sealed class SVMSysDefOutTra : SVMSysDef<SVMSysDefOutTra> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.OUT_TRAIN; } }
    //internal sealed class SVMSysDefOutPln : SVMSysDef<SVMSysDefOutPln> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.OUT_PLANE; } }
    //internal sealed class SVMSysDefOutCar : SVMSysDef<SVMSysDefOutCar> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.OUT_ROAD; } }
    internal sealed class SVMSysDefBeaCar : SVMSysDef<SVMSysDefBeaCar> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.BEAU_CAR; } }
}
