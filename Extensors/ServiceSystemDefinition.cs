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
        public static readonly ServiceSystemDefinition DISASTER_CAR = new ServiceSystemDefinition(ItemClass.Service.Disaster, VehicleInfo.VehicleType.Car);
        public static readonly ServiceSystemDefinition DISASTER_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.Disaster, VehicleInfo.VehicleType.Helicopter);
        public static readonly ServiceSystemDefinition FIRE_CAR = new ServiceSystemDefinition(ItemClass.Service.FireDepartment, VehicleInfo.VehicleType.Car);
        public static readonly ServiceSystemDefinition FIRE_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.FireDepartment, VehicleInfo.VehicleType.Helicopter);
        public static readonly ServiceSystemDefinition GARBAGE = new ServiceSystemDefinition(ItemClass.Service.Garbage, VehicleInfo.VehicleType.Car);
        public static readonly ServiceSystemDefinition HEALTHCARE_CAR = new ServiceSystemDefinition(ItemClass.Service.HealthCare, VehicleInfo.VehicleType.Car);
        public static readonly ServiceSystemDefinition HEALTHCARE_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.HealthCare, VehicleInfo.VehicleType.Helicopter);
        public static readonly ServiceSystemDefinition POLICE_CAR = new ServiceSystemDefinition(ItemClass.Service.PoliceDepartment, VehicleInfo.VehicleType.Car);
        public static readonly ServiceSystemDefinition POLICE_HELICOPTER = new ServiceSystemDefinition(ItemClass.Service.PoliceDepartment, VehicleInfo.VehicleType.Helicopter);
        public static readonly ServiceSystemDefinition ROAD_CAR = new ServiceSystemDefinition(ItemClass.Service.Road, VehicleInfo.VehicleType.Car);
        public static readonly List<ServiceSystemDefinition> availableDefinitions = new List<ServiceSystemDefinition>(new ServiceSystemDefinition[] { DISASTER_CAR, DISASTER_HELICOPTER, FIRE_CAR, FIRE_HELICOPTER, GARBAGE, HEALTHCARE_CAR, HEALTHCARE_HELICOPTER, POLICE_CAR, POLICE_HELICOPTER, ROAD_CAR });

        public ItemClass.Service service
        {
            get;
        }
        public VehicleInfo.VehicleType vehicleType
        {
            get;
        }

        private ServiceSystemDefinition(
        ItemClass.Service service,
        VehicleInfo.VehicleType vehicleType)
        {
            this.vehicleType = vehicleType;
            this.service = service;
        }

        internal ISVMTransportTypeExtension GetTransportExtension()
        {
            if (this == DISASTER_CAR) { return SVMServiceVehicleExtensionDisCar.instance; }
            if (this == DISASTER_HELICOPTER) { return SVMServiceVehicleExtensionDisHel.instance; }
            if (this == GARBAGE) { return SVMServiceVehicleExtensionGarCar.instance; }
            if (this == FIRE_CAR) { return SVMServiceVehicleExtensionFirCar.instance; }
            if (this == FIRE_HELICOPTER) { return SVMServiceVehicleExtensionFirHel.instance; }
            if (this == HEALTHCARE_CAR) { return SVMServiceVehicleExtensionHcrCar.instance; }
            if (this == HEALTHCARE_HELICOPTER) { return SVMServiceVehicleExtensionHcrHel.instance; }
            if (this == POLICE_CAR) { return SVMServiceVehicleExtensionPolCar.instance; }
            if (this == POLICE_HELICOPTER) { return SVMServiceVehicleExtensionPolHel.instance; }
            if (this == ROAD_CAR) { return SVMServiceVehicleExtensionRoaCar.instance; }
            return null;

        }

        public bool isFromSystem(VehicleInfo info)
        {
            return info.m_class.m_service == service && info.m_vehicleType == vehicleType && SVMUtils.HasField(info.GetAI(), "m_passengerCapacity");
        }

        public bool isFromSystem(DepotAI p)
        {
            return (p.m_info.m_class.m_service == service && p.m_transportInfo.m_vehicleType == vehicleType && p.m_maxVehicleCount > 0)
                || (p.m_secondaryTransportInfo != null && p.m_secondaryTransportInfo.m_vehicleType == vehicleType && p.m_maxVehicleCount2 > 0);
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

            return this.service == other.service && this.vehicleType == other.vehicleType;
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

        public static ServiceSystemDefinition from(PrefabAI buildingAI)
        {
            DepotAI depotAI = buildingAI as DepotAI;
            if (depotAI == null)
            {
                return null;
            }
            return from(depotAI.m_transportInfo);
        }

        public static ServiceSystemDefinition from(TransportInfo info)
        {
            if (info == null)
            {
                return default(ServiceSystemDefinition);
            }
            return availableDefinitions.FirstOrDefault(x => x.service == info.m_class.m_service && x.vehicleType == info.m_vehicleType);
        }
        public static ServiceSystemDefinition from(VehicleInfo info)
        {
            if (info == null)
            {
                return default(ServiceSystemDefinition);
            }
            return availableDefinitions.FirstOrDefault(x => x.service == info.m_class.m_service && x.vehicleType == info.m_vehicleType);
        }
        public static ServiceSystemDefinition from(uint lineId)
        {
            TransportLine t = Singleton<TransportManager>.instance.m_lines.m_buffer[lineId];
            return from(t.Info);
        }

        public SVMConfigWarehouse.ConfigIndex toConfigIndex()
        {
            return SVMConfigWarehouse.ConfigIndex.NIL;// SVMConfigWarehouse.getConfigTransportSystemForDefinition(this);
        }

        public override string ToString()
        {
            return service.ToString() + "|" + vehicleType.ToString();
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
    public sealed class SVMSysDefGarCar : SVMSysDef { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.GARBAGE; } }
    public sealed class SVMSysDefHcrCar : SVMSysDef { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.HEALTHCARE_CAR; } }
    public sealed class SVMSysDefHcrHel : SVMSysDef { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.HEALTHCARE_HELICOPTER; } }
    public sealed class SVMSysDefPolCar : SVMSysDef { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.POLICE_CAR; } }
    public sealed class SVMSysDefPolHel : SVMSysDef { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.POLICE_HELICOPTER; } }
    public sealed class SVMSysDefRoaCar : SVMSysDef { internal override ServiceSystemDefinition GetSSD() { return ServiceSystemDefinition.ROAD_CAR; } }
}
