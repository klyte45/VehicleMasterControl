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
    internal interface ISVMTransportTypeExtension : IAssetSelectorExtension, IBudgetableExtension, ISVMIgnorableDistrictExtensionValue, ISVMColorSelectableExtensionValue { }

    internal abstract class SVMServiceVehicleExtension<SSD, SG> : ExtensionInterfaceDefaultImpl<BuildingConfig, SG>, ISVMTransportTypeExtension where SSD : SVMSysDef<SSD>, new() where SG : SVMServiceVehicleExtension<SSD, SG>
    {
        public const uint DISTRICT_FLAG = 0x100000;
        public const uint BUILDING_FLAG = 0x200000;
        public const uint ID_PART = 0x0FFFFF;
        public const uint TYPE_PART = 0xF00000;

        protected override SVMConfigWarehouse.ConfigIndex ConfigIndexKey
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

        #region Budget Multiplier
        public uint[] GetBudgetsMultiplier(uint codedId)
        {
            checkId(codedId);

            string value = SafeGet(codedId, BuildingConfig.BUDGET_MULTIPLIER);
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
        public uint GetBudgetMultiplierForHour(uint codedId, int hour)
        {
            checkId(codedId);
            uint[] savedMultipliers = GetBudgetsMultiplier(codedId);
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
        public void SetBudgetMultiplier(uint codedId, uint[] multipliers)
        {
            checkId(codedId);
            SafeSet(codedId, BuildingConfig.BUDGET_MULTIPLIER, string.Join(ItSepLvl3, multipliers.Select(x => x.ToString()).ToArray()));
        }
        #endregion


        #region Asset List
        public List<string> GetAssetList(uint codedId)
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
        public Dictionary<string, string> GetSelectedBasicAssets(uint codedId)
        {
            checkId(codedId);
            if (basicAssetsList == null) LoadBasicAssets();
            return GetAssetList(codedId).Where(x => PrefabCollection<VehicleInfo>.FindLoaded(x) != null).ToDictionary(x => x, x => string.Format("[Cap={0}] {1}", SVMUtils.getCapacity(PrefabCollection<VehicleInfo>.FindLoaded(x)), Locale.Get("VEHICLE_TITLE", x)));
        }
        public Dictionary<string, string> GetAllBasicAssets(uint nil = 0)
        {
            if (basicAssetsList == null) LoadBasicAssets();
            return basicAssetsList.ToDictionary(x => x, x => string.Format("[Cap={0}] {1}", SVMUtils.getCapacity(PrefabCollection<VehicleInfo>.FindLoaded(x)), Locale.Get("VEHICLE_TITLE", x)));
        }
        public void AddAsset(uint codedId, string assetId)
        {
            checkId(codedId);
            var temp = GetAssetList(codedId);
            if (temp.Contains(assetId)) return;
            temp.Add(assetId);
            SafeSet(codedId, BuildingConfig.MODELS, string.Join(ItSepLvl3, temp.ToArray()));
        }
        public void RemoveAsset(uint codedId, string assetId)
        {
            checkId(codedId);
            var temp = GetAssetList(codedId);
            if (!temp.Contains(assetId)) return;
            temp.RemoveAll(x => x == assetId);
            SafeSet(codedId, BuildingConfig.MODELS, string.Join(ItSepLvl3, temp.ToArray()));
        }
        public void UseDefaultAssets(uint codedId)
        {
            checkId(codedId);
            SafeCleanProperty(codedId, BuildingConfig.MODELS);
        }
        public VehicleInfo GetAModel(ushort codedId)
        {
            checkId(codedId);
            if ((codedId & BUILDING_FLAG) > 0)
            {
                if (GetIgnoreDistrict(codedId))
                {
                    return SVMUtils.GetRandomModel(GetAssetList(codedId));
                }
            }
            return SVMUtils.GetRandomModel(GetAssetList(DISTRICT_FLAG | SVMUtils.GetBuildingDistrict(codedId & ID_PART)));
        }
        public void LoadBasicAssets()
        {
            basicAssetsList = SVMUtils.LoadBasicAssets(definition);
        }
        #endregion

        #region Ignore District
        public bool GetIgnoreDistrict(uint codedId)
        {
            checkId(codedId);
            return Boolean.TryParse(SafeGet(codedId, BuildingConfig.IGNORE_DISTRICT), out bool result) && result;
        }

        public void SetIgnoreDistrict(uint codedId, bool value)
        {
            checkId(codedId);
            SafeSet(codedId, BuildingConfig.IGNORE_DISTRICT, value.ToString());
        }
        #endregion
        #region Color
        public Color32 GetColor(uint codedId)
        {
            checkId(codedId);
            string value = SafeGet(codedId, BuildingConfig.MODELS);
            if (!string.IsNullOrEmpty(value))
            {
                var list = value.Split(ItSepLvl3.ToCharArray()).ToList();
                if (list.Count == 3 && byte.TryParse(list[0], out byte r) && byte.TryParse(list[1], out byte g) && byte.TryParse(list[2], out byte b))
                {
                    return new Color32(r, g, b, 255);
                }
            }
            return Color.clear;
        }

        public void SetColor(uint codedId, Color32 value)
        {
            checkId(codedId);
            SafeSet(codedId, BuildingConfig.COLOR, string.Join(ItSepLvl3, new string[] { value.r.ToString(), value.g.ToString(), value.b.ToString() }));
        }
        public void CleanColor(uint codedId)
        {
            checkId(codedId);
            SafeCleanProperty(codedId, BuildingConfig.COLOR);
        }
        #endregion
    }

    internal interface ISVMIgnorableDistrictExtensionValue
    {
        bool GetIgnoreDistrict(uint codedId);
        void SetIgnoreDistrict(uint codedId, bool value);
    }


    internal interface ISVMColorSelectableExtensionValue
    {
        Color32 GetColor(uint codedId);
        void SetColor(uint codedId, Color32 value);
        void CleanColor(uint codedId);
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
    internal sealed class SVMServiceVehicleExtensionWatCar : SVMServiceVehicleExtension<SVMSysDefWatCar, SVMServiceVehicleExtensionWatCar> { }
    internal sealed class SVMServiceVehicleExtensionPriCar : SVMServiceVehicleExtension<SVMSysDefPriCar, SVMServiceVehicleExtensionPriCar> { }
    internal sealed class SVMServiceVehicleExtensionDcrCar : SVMServiceVehicleExtension<SVMSysDefDcrCar, SVMServiceVehicleExtensionDcrCar> { }
    internal sealed class SVMServiceVehicleExtensionTaxCar : SVMServiceVehicleExtension<SVMSysDefTaxCar, SVMServiceVehicleExtensionTaxCar> { }
    internal sealed class SVMServiceVehicleExtensionCcrCcr : SVMServiceVehicleExtension<SVMSysDefCcrCcr, SVMServiceVehicleExtensionCcrCcr> { }

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
        BUDGET_MULTIPLIER,
        COLOR,
        IGNORE_DISTRICT
    }
}
