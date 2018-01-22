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
        public static readonly ServiceSystemDefinition DISASTER_CAR          = new ServiceSystemDefinition(ItemClass.Service.Disaster        , VehicleInfo.VehicleType.Car,        ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition DISASTER_HELICOPTER   = new ServiceSystemDefinition(ItemClass.Service.Disaster        , VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition FIRE_CAR              = new ServiceSystemDefinition(ItemClass.Service.FireDepartment  , VehicleInfo.VehicleType.Car,        ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition FIRE_HELICOPTER       = new ServiceSystemDefinition(ItemClass.Service.FireDepartment  , VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition GARBAGE_CAR               = new ServiceSystemDefinition(ItemClass.Service.Garbage         , VehicleInfo.VehicleType.Car,        ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition HEALTHCARE_CAR        = new ServiceSystemDefinition(ItemClass.Service.HealthCare      , VehicleInfo.VehicleType.Car,        ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition HEALTHCARE_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.HealthCare      , VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level3);
        public static readonly ServiceSystemDefinition DEATHCARE_CAR         = new ServiceSystemDefinition(ItemClass.Service.HealthCare      , VehicleInfo.VehicleType.Car,        ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition POLICE_CAR            = new ServiceSystemDefinition(ItemClass.Service.PoliceDepartment, VehicleInfo.VehicleType.Car,        ItemClass.Level.Level1);
        public static readonly ServiceSystemDefinition POLICE_HELICOPTER     = new ServiceSystemDefinition(ItemClass.Service.PoliceDepartment, VehicleInfo.VehicleType.Helicopter, ItemClass.Level.Level3);
        public static readonly ServiceSystemDefinition ROAD_CAR              = new ServiceSystemDefinition(ItemClass.Service.Road            , VehicleInfo.VehicleType.Car,        ItemClass.Level.Level2);
        public static readonly ServiceSystemDefinition WATER_CAR             = new ServiceSystemDefinition(ItemClass.Service.Water           , VehicleInfo.VehicleType.Car,        ItemClass.Level.None  );
        public static readonly ServiceSystemDefinition PRISION_CAR           = new ServiceSystemDefinition(ItemClass.Service.PoliceDepartment, VehicleInfo.VehicleType.Car,        ItemClass.Level.Level4);
        public static readonly Dictionary<ServiceSystemDefinition, ISVMTransportTypeExtension> availableDefinitions = new Dictionary<ServiceSystemDefinition, ISVMTransportTypeExtension>()
        {
            [DISASTER_CAR]          = SVMServiceVehicleExtensionDisCar.instance,
            [DISASTER_HELICOPTER]   = SVMServiceVehicleExtensionDisHel.instance,
            [FIRE_CAR]              = SVMServiceVehicleExtensionFirCar.instance,
            [FIRE_HELICOPTER]       = SVMServiceVehicleExtensionFirHel.instance,
            [GARBAGE_CAR]               = SVMServiceVehicleExtensionGarCar.instance,
            [HEALTHCARE_CAR]        = SVMServiceVehicleExtensionHcrCar.instance,
            [HEALTHCARE_HELICOPTER] = SVMServiceVehicleExtensionHcrHel.instance,
            [DEATHCARE_CAR]         = SVMServiceVehicleExtensionDcrCar.instance,
            [POLICE_CAR]            = SVMServiceVehicleExtensionPolCar.instance,
            [POLICE_HELICOPTER]     = SVMServiceVehicleExtensionPolHel.instance,
            [ROAD_CAR]              = SVMServiceVehicleExtensionRoaCar.instance,
            [WATER_CAR]             = SVMServiceVehicleExtensionWatCar.instance,
            [PRISION_CAR]           = SVMServiceVehicleExtensionPriCar.instance,
        };

        public ItemClass.Service service
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
        VehicleInfo.VehicleType vehicleType,
        ItemClass.Level level)
        {
            this.vehicleType = vehicleType;
            this.service = service;
            this.level = level;
        }

        internal ISVMTransportTypeExtension GetTransportExtension()
        {
            return availableDefinitions[this];
        }

        public bool isFromSystem(VehicleInfo info)
        {
            return info.m_class.m_service == service && info.m_vehicleType == vehicleType && info.m_class.m_level == level;
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

            return level ==other.level && service==other.service && vehicleType==other.vehicleType;
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
            return availableDefinitions.Keys.FirstOrDefault(x => x.service == info.m_class.m_service && x.vehicleType == info.m_vehicleType && x.level == info.m_class.m_level);
        }

        public static ServiceSystemDefinition from(BuildingInfo info)
        {
            if (info == null)
            {
                return default(ServiceSystemDefinition);
            }
            return availableDefinitions.Keys.FirstOrDefault(x => x.service == info.m_class.m_service && x.level == info.m_class.m_level);
        }

        public SVMConfigWarehouse.ConfigIndex toConfigIndex()
        {
            return SVMConfigWarehouse.getConfigServiceSystemForDefinition(this);
        }

        public override string ToString()
        {
            return service.ToString() + "|" + level.ToString() + "|" + vehicleType.ToString();
        }

        public override int GetHashCode()
        {
            var hashCode = 286451371;
            hashCode = hashCode * -1521134295 + service.GetHashCode();
            hashCode = hashCode * -1521134295 + vehicleType.GetHashCode();
            return hashCode;
        }
    }

    public abstract class SVMSysDef : Singleton<SVMSysDef> { internal abstract ServiceSystemDefinition GetSSD(); }
    public sealed class SVMSysDefDisCar : SVMSysDef { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.DISASTER_CAR; } }
    public sealed class SVMSysDefDisHel : SVMSysDef { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.DISASTER_HELICOPTER; } }
    public sealed class SVMSysDefFirCar : SVMSysDef { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.FIRE_HELICOPTER; } }
    public sealed class SVMSysDefFirHel : SVMSysDef { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.FIRE_CAR; } }
    public sealed class SVMSysDefGarCar : SVMSysDef { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.GARBAGE_CAR; } }
    public sealed class SVMSysDefHcrCar : SVMSysDef { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.HEALTHCARE_CAR; } }
    public sealed class SVMSysDefHcrHel : SVMSysDef { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.HEALTHCARE_HELICOPTER; } }
    public sealed class SVMSysDefPolCar : SVMSysDef { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.POLICE_CAR; } }
    public sealed class SVMSysDefPolHel : SVMSysDef { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.POLICE_HELICOPTER; } }
    public sealed class SVMSysDefRoaCar : SVMSysDef { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.ROAD_CAR; } }
}
