using ColossalFramework;
using Klyte.ServiceVehiclesManager.Utils;
using Klyte.TransportLinesManager.Extensors.TransportTypeExt;
using Klyte.TransportLinesManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Klyte.ServiceVehiclesManager.Extensors.VehicleExt
{
    internal class ServiceSystemDefinition
    {
        public static readonly ServiceSystemDefinition DISASTER_CAR = new ServiceSystemDefinition(ItemClass.Service.Disaster, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition DISASTER_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.Disaster, ItemClass.SubService.None, VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition FIRE_CAR = new ServiceSystemDefinition(ItemClass.Service.FireDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition FIRE_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.FireDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition GARBAGE_CAR = new ServiceSystemDefinition(ItemClass.Service.Garbage, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition HEALTHCARE_CAR = new ServiceSystemDefinition(ItemClass.Service.HealthCare, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition HEALTHCARE_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.HealthCare, ItemClass.SubService.None, VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level3);
        public static readonly ServiceSystemDefinition DEATHCARE_CAR = new ServiceSystemDefinition(ItemClass.Service.HealthCare, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition POLICE_CAR = new ServiceSystemDefinition(ItemClass.Service.PoliceDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition POLICE_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.PoliceDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level3);
        public static readonly ServiceSystemDefinition ROAD_CAR = new ServiceSystemDefinition(ItemClass.Service.Road, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition WATER_CAR = new ServiceSystemDefinition(ItemClass.Service.Water, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.None);
        public static readonly ServiceSystemDefinition PRISION_CAR = new ServiceSystemDefinition(ItemClass.Service.PoliceDepartment, ItemClass.SubService.None, VehicleInfo.VehicleType.Car, ItemClass.Level.Level4);
        public static readonly ServiceSystemDefinition TAXI_CAR = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTaxi, VehicleInfo.VehicleType.Car, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition CABLECAR_CABLECAR = new ServiceSystemDefinition(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportCableCar, VehicleInfo.VehicleType.CableCar, ItemClass.Level.Level1);
        public static readonly Dictionary<ServiceSystemDefinition, ISVMTransportTypeExtension> availableDefinitions = new Dictionary<ServiceSystemDefinition, ISVMTransportTypeExtension>()
        {
            [DISASTER_CAR] = SVMServiceVehicleExtensionDisCar.instance,
            [DISASTER_HELICOPTER] = SVMServiceVehicleExtensionDisHel.instance,
            [FIRE_CAR] = SVMServiceVehicleExtensionFirCar.instance,
            [FIRE_HELICOPTER] = SVMServiceVehicleExtensionFirHel.instance,
            [GARBAGE_CAR] = SVMServiceVehicleExtensionGarCar.instance,
            [HEALTHCARE_CAR] = SVMServiceVehicleExtensionHcrCar.instance,
            [HEALTHCARE_HELICOPTER] = SVMServiceVehicleExtensionHcrHel.instance,
            [DEATHCARE_CAR] = SVMServiceVehicleExtensionDcrCar.instance,
            [POLICE_CAR] = SVMServiceVehicleExtensionPolCar.instance,
            [POLICE_HELICOPTER] = SVMServiceVehicleExtensionPolHel.instance,
            [ROAD_CAR] = SVMServiceVehicleExtensionRoaCar.instance,
            [WATER_CAR] = SVMServiceVehicleExtensionWatCar.instance,
            [PRISION_CAR] = SVMServiceVehicleExtensionPriCar.instance,
            [TAXI_CAR] = SVMServiceVehicleExtensionTaxCar.instance,
            [CABLECAR_CABLECAR] = SVMServiceVehicleExtensionCcrCcr.instance,
        };
        public static readonly Dictionary<ServiceSystemDefinition, Type> sysDefinitions = new Dictionary<ServiceSystemDefinition, Type>()
        {
            [DISASTER_CAR] = typeof(SVMSysDefDisCar),
            [DISASTER_HELICOPTER] = typeof(SVMSysDefDisHel),
            [FIRE_CAR] = typeof(SVMSysDefFirCar),
            [FIRE_HELICOPTER] = typeof(SVMSysDefFirHel),
            [GARBAGE_CAR] = typeof(SVMSysDefGarCar),
            [HEALTHCARE_CAR] = typeof(SVMSysDefHcrCar),
            [HEALTHCARE_HELICOPTER] = typeof(SVMSysDefHcrHel),
            [DEATHCARE_CAR] = typeof(SVMSysDefDcrCar),
            [POLICE_CAR] = typeof(SVMSysDefPolCar),
            [POLICE_HELICOPTER] = typeof(SVMSysDefPolHel),
            [ROAD_CAR] = typeof(SVMSysDefRoaCar),
            [WATER_CAR] = typeof(SVMSysDefWatCar),
            [PRISION_CAR] = typeof(SVMSysDefPriCar),
            [TAXI_CAR] = typeof(SVMSysDefTaxCar),
            [CABLECAR_CABLECAR] = typeof(SVMSysDefCcrCcr),
        };

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


        private ServiceSystemDefinition(
        ItemClass.Service service,
        ItemClass.SubService subService,
        VehicleInfo.VehicleType vehicleType,
        ItemClass.Level level)
        {
            this.vehicleType = vehicleType;
            this.service = service;
            this.level = level;
            this.subService = subService;
        }

        internal ISVMTransportTypeExtension GetTransportExtension()
        {
            return availableDefinitions[this];
        }

        public bool isFromSystem(VehicleInfo info)
        {
            return info.m_class.m_service == service && subService == info.m_class.m_subService && info.m_vehicleType == vehicleType && info.m_class.m_level == level;
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

            return level == other.level && service == other.service && subService == other.subService && vehicleType == other.vehicleType;
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
            return availableDefinitions.Keys.FirstOrDefault(x => x.service == info.m_class.m_service && x.subService == info.m_class.m_subService && x.vehicleType == info.m_vehicleType && x.level == info.m_class.m_level);
        }

        public static ServiceSystemDefinition from(BuildingInfo info)
        {
            if (info == null)
            {
                return default(ServiceSystemDefinition);
            }
            return availableDefinitions.Keys.FirstOrDefault(x => x.service == info.m_class.m_service && x.subService == info.m_class.m_subService && x.level == info.m_class.m_level);
        }

        public SVMConfigWarehouse.ConfigIndex toConfigIndex()
        {
            return SVMConfigWarehouse.getConfigServiceSystemForDefinition(this);
        }

        public override string ToString()
        {
            return service.ToString() + "|" + subService.ToString() + "|" + level.ToString() + "|" + vehicleType.ToString();
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
    internal sealed class SVMSysDefFirCar : SVMSysDef<SVMSysDefFirCar> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.FIRE_HELICOPTER; } }
    internal sealed class SVMSysDefFirHel : SVMSysDef<SVMSysDefFirHel> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.FIRE_CAR; } }
    internal sealed class SVMSysDefGarCar : SVMSysDef<SVMSysDefGarCar> { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.GARBAGE_CAR; } }
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
}
