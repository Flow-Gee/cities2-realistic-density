﻿using Game;
using Unity.Entities;
using UnityEngine.Scripting;
using Game.SceneFlow;
using RealisticDensity.Settings;
using RealisticDensity.Jobs;
using Unity.Jobs;
using Game.Common;
using Colossal.Serialization.Entities;
using Game.Prefabs;
using Unity.Mathematics;
using Unity.Burst;

namespace RealisticDensity.Systems
{
    [BurstCompile]
    public partial class RealisticDensitySystem : GameSystemBase
    {
        readonly public static int kComponentVersion = 1;

        // Workforce is a third of the production outcome calculation for spawnable entities like commercial, industrial and offices
        readonly public static int kProductionFactor = 3;

        private ModificationEndBarrier Barrier;
        public static Setting Settings;

        private UpdateCommercialBuildingsTypeHandle m_UpdateCommercialBuildingsTypeHandle;
        private EntityQuery m_UpdateCommercialBuildingsQuery;
        private UpdateIndustryBuildingsTypeHandle m_UpdateIndustryBuildingsTypeHandle;
        private EntityQuery m_UpdateIndustryBuildingsQuery;
        private UpdateCityServicesTypeHandle m_UpdateCityServicesJobTypeHandle;
        private EntityQuery m_UpdateCityServicesJobQuery;
        private UpdateHighOfficesTypeHandle m_UpdateHighOfficesJobTypeHandle;
        private EntityQuery m_UpdateHighOfficesJobQuery;

        private EntityQuery m_EconomyParameterQuery;
        private ResourcePrefabs m_ResourcePrefabs;

        [Preserve]
        protected override void OnCreate()
        {
            base.OnCreate();

            // Load settings
            Settings = Mod.Setting;

            // Create a barrier system using the default world
            Barrier = World.GetOrCreateSystemManaged<ModificationEndBarrier>();

            m_ResourcePrefabs = World.GetOrCreateSystemManaged<ResourceSystem>().GetPrefabs();

            // Job Queries
            UpdateCityServicesQuery updateCityServicesQuery = new();
            m_UpdateCityServicesJobQuery = GetEntityQuery(updateCityServicesQuery.Query);
            UpdateCommercialBuildingsQuery updateCommercialBuildingsQuery = new();
            m_UpdateCommercialBuildingsQuery = GetEntityQuery(updateCommercialBuildingsQuery.Query);
            UpdateIndustryBuildingsQuery updateIndustryBuildingsQuery = new();
            m_UpdateIndustryBuildingsQuery = GetEntityQuery(updateIndustryBuildingsQuery.Query);
            UpdateHighOfficesQuery updateHighOfficesQuery = new();
            m_UpdateHighOfficesJobQuery = GetEntityQuery(updateHighOfficesQuery.Query);

            m_EconomyParameterQuery = GetEntityQuery(new ComponentType[] { ComponentType.ReadOnly<EconomyParameterData>() });

            RequireAnyForUpdate(
                m_UpdateCommercialBuildingsQuery,
                m_UpdateIndustryBuildingsQuery,
                m_UpdateCityServicesJobQuery,
                m_UpdateHighOfficesJobQuery,
                m_EconomyParameterQuery
            );

            Mod.DebugLog("System created.");
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            Enabled = false;

            if (mode == GameMode.Game)
                Enabled = true;
        }

        [Preserve]
        protected override void OnUpdate()
        {
            if (Settings.DisableMod == true || GameManager.instance == null || !GameManager.instance.gameMode.IsGameOrEditor())
            {
                return;
            }

            if (Settings.CityServicesEnabled && !m_UpdateCityServicesJobQuery.IsEmptyIgnoreFilter)
            {
                UpdateCityBuildings();
            }

            if (Settings.SpawnablesEnabled && Settings.CommercialsEnabled && !m_UpdateCommercialBuildingsQuery.IsEmptyIgnoreFilter)
            {
                UpdateCommercialBuildings();
            }

            if (Settings.SpawnablesEnabled && Settings.IndustriesEnabled && !m_UpdateIndustryBuildingsQuery.IsEmptyIgnoreFilter)
            {
                UpdateIndustryBuildings();
            }

            if (Settings.OfficesEnabled && !m_UpdateHighOfficesJobQuery.IsEmptyIgnoreFilter)
            {
                UpdateHighOffices();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Mod.DebugLog("System destroyed.");

        }

        private void UpdateCityBuildings()
        {
            Mod.DebugLog("Adjust city buildings.");
            m_UpdateCityServicesJobTypeHandle.AssignHandles(ref CheckedStateRef);
            UpdateCityServicesJob updateCityServicesJob = new()
            {
                Ecb = Barrier.CreateCommandBuffer().AsParallelWriter(),
                EntityTypeHandle = m_UpdateCityServicesJobTypeHandle.EntityTypeHandle,
                BuildingDataLookup = m_UpdateCityServicesJobTypeHandle.BuildingDataLookup,
                WorkplaceDataLookup = m_UpdateCityServicesJobTypeHandle.WorkplaceDataLookup,
                PowerPlantDataLookup = m_UpdateCityServicesJobTypeHandle.PowerPlantDataLookup,
                SchoolDataLookup = m_UpdateCityServicesJobTypeHandle.SchoolDataLookup,
                HospitalDataLookup = m_UpdateCityServicesJobTypeHandle.HospitalDataLookup,
                PoliceStationDataLookup = m_UpdateCityServicesJobTypeHandle.PoliceStationDataLookup,
                PrisonDataLookup = m_UpdateCityServicesJobTypeHandle.PrisonDataLookup,
                FireStationDataLookup = m_UpdateCityServicesJobTypeHandle.FireStationDataLookup,
                CargoTransportStationDataLookup = m_UpdateCityServicesJobTypeHandle.CargoTransportStationDataLookup,
                TransportDepotDataLookup = m_UpdateCityServicesJobTypeHandle.TransportDepotDataLookup,
                GarbageFacilityDataLookup = m_UpdateCityServicesJobTypeHandle.GarbageFacilityDataLookup,
                DeathcareFacilityDataLookup = m_UpdateCityServicesJobTypeHandle.DeathcareFacilityDataLookup,
                PublicTransportStationDataLookup = m_UpdateCityServicesJobTypeHandle.PublicTransportStationDataLookup,
                MaintenanceDepotDataLookup = m_UpdateCityServicesJobTypeHandle.MaintenanceDepotDataLookup,
                PostFacilityDataLookup = m_UpdateCityServicesJobTypeHandle.PostFacilityDataLookup,
                AdminBuildingDataLookup = m_UpdateCityServicesJobTypeHandle.AdminBuildingDataLookup,
                WelfareOfficeDataLookup = m_UpdateCityServicesJobTypeHandle.WelfareOfficeDataLookup,
                ResearchFacilityDataLookup = m_UpdateCityServicesJobTypeHandle.ResearchFacilityDataLookup,
                TelecomFacilityDataLookup = m_UpdateCityServicesJobTypeHandle.TelecomFacilityDataLookup,
                ParkDataLookup = m_UpdateCityServicesJobTypeHandle.ParkDataLookup,
            };
            Dependency = updateCityServicesJob.Schedule(m_UpdateCityServicesJobQuery, Dependency);
            Barrier.AddJobHandleForProducer(Dependency);
        }

        private void UpdateCommercialBuildings()
        {
            Mod.DebugLog("Adjust commercial buildings.");
            
            BuildingData buildingData = new()
            {
                m_LotSize = new int2(100, 10)
            };
            SpawnableBuildingData spawnableBuildingData = new()
            {
                m_Level = 1
            };

            m_UpdateCommercialBuildingsTypeHandle.AssignHandles(ref CheckedStateRef);
            UpdateCommercialBuildingsJob updateCommercialBuildingsJob = new()
            {
                Ecb = Barrier.CreateCommandBuffer().AsParallelWriter(),
                EntityHandle = m_UpdateCommercialBuildingsTypeHandle.EntityTypeHandle,
                WorkplaceDataLookup = m_UpdateCommercialBuildingsTypeHandle.WorkplaceDataLookup,
                ServiceCompanyDataLookup = m_UpdateCommercialBuildingsTypeHandle.ServiceCompanyDataLookup,
                IndustrialProcessDataLookup = m_UpdateCommercialBuildingsTypeHandle.IndustrialProcessDataLookup,
                ResourceDataLookup = m_UpdateCommercialBuildingsTypeHandle.ResourceDataLookup,
                EconomyParameterData = m_EconomyParameterQuery.GetSingleton<EconomyParameterData>(),
                ResourcePrefabs = m_ResourcePrefabs,
                BuildingData = buildingData,
                SpawnableBuildingData = spawnableBuildingData,
            };
            Dependency = updateCommercialBuildingsJob.Schedule(m_UpdateCommercialBuildingsQuery, Dependency);
            Barrier.AddJobHandleForProducer(Dependency);
        }

        private void UpdateIndustryBuildings()
        {
            Mod.DebugLog("Adjust industrial buildings.");

            BuildingData buildingData = new()
            {
                m_LotSize = new int2(100, 10)
            };
            SpawnableBuildingData spawnableBuildingData = new()
            {
                m_Level = 1
            };

            m_UpdateIndustryBuildingsTypeHandle.AssignHandles(ref CheckedStateRef);
            UpdateIndustryBuildingsJob updateIndustryBuildingsJob = new()
            {
                Ecb = Barrier.CreateCommandBuffer().AsParallelWriter(),
                EntityHandle = m_UpdateIndustryBuildingsTypeHandle.EntityTypeHandle,
                WorkplaceDataLookup = m_UpdateIndustryBuildingsTypeHandle.WorkplaceDataLookup,
                IndustrialProcessDataLookup = m_UpdateIndustryBuildingsTypeHandle.IndustrialProcessDataLookup,
                ExtractorCompanyDataLookup = m_UpdateIndustryBuildingsTypeHandle.ExtractorCompanyDataLookup,
                StorageLimitDataLookup = m_UpdateIndustryBuildingsTypeHandle.StorageLimitDataLookup,
                TransportCompanyDataLookup = m_UpdateIndustryBuildingsTypeHandle.TransportCompanyDataLookup,
                ResourceDataLookup = m_UpdateIndustryBuildingsTypeHandle.ResourceDataLookup,
                EconomyParameterData = m_EconomyParameterQuery.GetSingleton<EconomyParameterData>(),
                ResourcePrefabs = m_ResourcePrefabs,
                BuildingData = buildingData,
                SpawnableBuildingData = spawnableBuildingData,
                OfficesEnabled = Settings.OfficesEnabled,
                OfficesFactor = Settings.OfficesFactor,
                IndustryExtractorFactor = Settings.IndustryExtractorFactor,
                IndustryProcessingFactor = Settings.IndustryProcessingFactor,
                IndustryIncreaseMaxTransports = Settings.IndustryIncreaseMaxTransports,
                IndustryIncreaseStorageCapacity = Settings.IndustryIncreaseStorageCapacity,
            };
            Dependency = updateIndustryBuildingsJob.Schedule(m_UpdateIndustryBuildingsQuery, Dependency);
            Barrier.AddJobHandleForProducer(Dependency);
        }

        private void UpdateHighOffices()
        {
            Mod.DebugLog("Adjust high office buildings.");
            m_UpdateHighOfficesJobTypeHandle.AssignHandles(ref CheckedStateRef);
            UpdateHighOfficesJob updateHighOfficesJob = new()
            {
                Ecb = Barrier.CreateCommandBuffer().AsParallelWriter(),
                EntityHandle = m_UpdateHighOfficesJobTypeHandle.EntityTypeHandle,
                BuildingPropertyDataLookup = m_UpdateHighOfficesJobTypeHandle.BuildingPropertyDataLookup,
                OfficesFactor = Settings.OfficesFactor,
            };
            Dependency = updateHighOfficesJob.Schedule(m_UpdateHighOfficesJobQuery, Dependency);
            Barrier.AddJobHandleForProducer(Dependency);
        }
    }
}
