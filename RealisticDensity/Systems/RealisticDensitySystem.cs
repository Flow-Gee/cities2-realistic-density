using Game;
using Unity.Entities;
using UnityEngine.Scripting;
using Game.SceneFlow;
using RealisticDensity.Settings;
using RealisticDensity.Jobs;
using Unity.Jobs;
using Game.UI;
using System.IO;
using Colossal.Serialization.Entities;
using ExtendedTooltip.Settings;
using Game.Prefabs;

namespace RealisticDensity.Systems
{
    public partial class RealisticDensitySystem : GameSystemBase
    {
        readonly public static int kComponentVersion = 1;

        // Workforce is a third of the production outcome calculation for spawnable entities like commercial, industrial and offices
        readonly public static int kProductionFactor = 3;

        private static readonly string AssemblyPath = Path.GetDirectoryName(typeof(RealisticDensitySystem).Assembly.Location);
        public static string UIPath = AssemblyPath + "\\UI\\";

        private EndFrameBarrier Barrier;
        private PrefabSystem m_PrefabSystem;

        public LocalSettings m_LocalSettings;
        public static RealisticDensitySettings Settings;
        public bool m_LocalSettingsLoaded = false;

        private UpdateCommercialBuildingsTypeHandle m_UpdateCommercialBuildingsTypeHandle;
        private EntityQuery m_UpdateCommercialBuildingsQuery;
        private UpdateIndustryBuildingsTypeHandle m_UpdateIndustryBuildingsTypeHandle;
        private EntityQuery m_UpdateIndustryBuildingsQuery;
        private UpdateCityServicesTypeHandle m_UpdateCityServicesJobTypeHandle;
        private EntityQuery m_UpdateCityServicesJobQuery;
        private UpdateHighOfficesTypeHandle m_UpdateHighOfficesJobTypeHandle;
        private EntityQuery m_UpdateHighOfficesJobQuery;

        [Preserve]
        protected override void OnCreate()
        {
            base.OnCreate();

            // Load settings
            LoadSettings();
            Settings = m_LocalSettings.Settings;

            // Create a barrier system using the default world
            Barrier = World.GetOrCreateSystemManaged<EndFrameBarrier>();

            // Job Queries
            UpdateCityServicesQuery updateCityServicesQuery = new();
            m_UpdateCityServicesJobQuery = GetEntityQuery(updateCityServicesQuery.Query);
            UpdateCommercialBuildingsQuery updateCommercialBuildingsQuery = new();
            m_UpdateCommercialBuildingsQuery = GetEntityQuery(updateCommercialBuildingsQuery.Query);
            UpdateIndustryBuildingsQuery updateIndustryBuildingsQuery = new();
            m_UpdateIndustryBuildingsQuery = GetEntityQuery(updateIndustryBuildingsQuery.Query);
            UpdateHighOfficesQuery updateHighOfficesQuery = new();
            m_UpdateHighOfficesJobQuery = GetEntityQuery(updateHighOfficesQuery.Query);

            RequireAnyForUpdate(m_UpdateCityServicesJobQuery, m_UpdateCommercialBuildingsQuery, m_UpdateIndustryBuildingsQuery, m_UpdateHighOfficesJobQuery);
            Mod.DebugLog("System created.");
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
        }

        [Preserve]
        protected override void OnUpdate()
        {
            if (Settings.DisableMod == true || GameManager.instance == null || !GameManager.instance.gameMode.IsGameOrEditor())
            {
                return;
            }

            if (m_LocalSettings.Settings.CityServicesEnabled && !m_UpdateCityServicesJobQuery.IsEmptyIgnoreFilter)
            {
                Mod.DebugLog("Run UpdateCityServicesJob");
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

            if (m_LocalSettings.Settings.SpawnablesEnabled && m_LocalSettings.Settings.CommercialsEnabled && !m_UpdateCommercialBuildingsQuery.IsEmptyIgnoreFilter)
            {
                Mod.DebugLog("Run UpdateCommercialBuildingsJob");
                m_UpdateCommercialBuildingsTypeHandle.AssignHandles(ref CheckedStateRef);
                UpdateCommercialBuildingsJob updateCommercialBuildingsJob = new()
                {
                    Ecb = Barrier.CreateCommandBuffer().AsParallelWriter(),
                    EntityHandle = m_UpdateCommercialBuildingsTypeHandle.EntityTypeHandle,
                    WorkplaceDataLookup = m_UpdateCommercialBuildingsTypeHandle.WorkplaceDataLookup,
                    ServiceCompanyDataLookup = m_UpdateCommercialBuildingsTypeHandle.ServiceCompanyDataLookup,
                };
                Dependency = updateCommercialBuildingsJob.Schedule(m_UpdateCommercialBuildingsQuery, Dependency);
                Barrier.AddJobHandleForProducer(Dependency);
            }

            if (m_LocalSettings.Settings.SpawnablesEnabled && m_LocalSettings.Settings.IndustriesEnabled && !m_UpdateIndustryBuildingsQuery.IsEmptyIgnoreFilter)
            {
                Mod.DebugLog("Run UpdateIndustryBuildingsJob");
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
                };
                Dependency = updateIndustryBuildingsJob.Schedule(m_UpdateIndustryBuildingsQuery, Dependency);
                Barrier.AddJobHandleForProducer(Dependency);
            }

            if (m_LocalSettings.Settings.OfficesEnabled && !m_UpdateHighOfficesJobQuery.IsEmptyIgnoreFilter)
            {
                Mod.DebugLog("Run UpdateHighOfficesJob");
                m_UpdateHighOfficesJobTypeHandle.AssignHandles(ref CheckedStateRef);
                UpdateHighOfficesJob updateHighOfficesJob = new()
                {
                    Ecb = Barrier.CreateCommandBuffer().AsParallelWriter(),
                    EntityHandle = m_UpdateHighOfficesJobTypeHandle.EntityTypeHandle,
                    BuildingPropertyDataLookup = m_UpdateHighOfficesJobTypeHandle.BuildingPropertyDataLookup,
                };
                Dependency = updateHighOfficesJob.Schedule(m_UpdateHighOfficesJobQuery, Dependency);
                Barrier.AddJobHandleForProducer(Dependency);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnityEngine.Debug.Log("[RealisticDensity] System destroyed.");

        }

        private void LoadSettings()
        {
            try
            {
                m_LocalSettings = new();
                m_LocalSettings.Init();
                m_LocalSettingsLoaded = true;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log($"[RealisticDensity] Error loading settings: {e.Message}");
            }
        }

        public static void EnsureModUIFolder()
        {
            var resourceHandler = (GameUIResourceHandler)GameManager.instance.userInterface.view.uiSystem.resourceHandler;

            if (resourceHandler == null || resourceHandler.HostLocationsMap.ContainsKey("realisticdensityui"))
                return;

            resourceHandler.HostLocationsMap.Add("realisticdensityui", [UIPath]);
        }
    }
}
