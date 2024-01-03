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

        public LocalSettings m_LocalSettings;
        public static RealisticDensitySettings Settings;
        public bool m_LocalSettingsLoaded = false;

        private UpdateSpawnablesTypeHandle m_UpdateSpawnablesJobTypeHandle;
        private EntityQuery m_UpdateSpawnablesJobQuery;
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
            UpdateSpawnablesQuery updateSpawnablesQuery = new();
            m_UpdateSpawnablesJobQuery = GetEntityQuery(updateSpawnablesQuery.Query);
            UpdateHighOfficesQuery updateHighOfficesQuery = new();
            m_UpdateHighOfficesJobQuery = GetEntityQuery(updateHighOfficesQuery.Query);

            RequireAnyForUpdate(m_UpdateCityServicesJobQuery, m_UpdateSpawnablesJobQuery);
            Mod.Instance.Log.DebugFormat("System created.");
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
                Mod.Instance.Log.Info("Run UpdateCityServicesJob");
                m_UpdateCityServicesJobTypeHandle.AssignHandles(ref CheckedStateRef);
                UpdateCityServicesJob updateCityServicesJob = new()
                {
                    Ecb = Barrier.CreateCommandBuffer().AsParallelWriter(),
                    EntityTypeHandle = m_UpdateCityServicesJobTypeHandle.EntityTypeHandle,
                    WorkplaceDataLookup = m_UpdateCityServicesJobTypeHandle.WorkplaceDataLookup,
                    PowerPlantDataLookup = m_UpdateCityServicesJobTypeHandle.PowerPlantDataLookup,
                    SchoolDataLookup = m_UpdateCityServicesJobTypeHandle.SchoolDataLookup,
                    HospitalDataLookup = m_UpdateCityServicesJobTypeHandle.HospitalDataLookup,
                    PoliceStationDataLookup = m_UpdateCityServicesJobTypeHandle.PoliceStationDataLookup,
                    FireStationDataLookup = m_UpdateCityServicesJobTypeHandle.FireStationDataLookup,
                    CargoTransportStationDataLookup = m_UpdateCityServicesJobTypeHandle.CargoTransportStationDataLookup
                };
                Dependency = updateCityServicesJob.Schedule(m_UpdateCityServicesJobQuery, Dependency);
                Barrier.AddJobHandleForProducer(Dependency);
            }

            if (m_LocalSettings.Settings.SpawnablesEnabled && !m_UpdateSpawnablesJobQuery.IsEmptyIgnoreFilter)
            {
                Mod.Instance.Log.Info("Run UpdateSpawnablesJob");
                m_UpdateSpawnablesJobTypeHandle.AssignHandles(ref CheckedStateRef);
                UpdateSpawnablesJob updateSpawnablesJob = new()
                {
                    Ecb = Barrier.CreateCommandBuffer().AsParallelWriter(),
                    EntityHandle = m_UpdateSpawnablesJobTypeHandle.EntityTypeHandle,
                    WorkplaceDataLookup = m_UpdateSpawnablesJobTypeHandle.WorkplaceDataLookup,
                    IndustrialProcessDataLookup = m_UpdateSpawnablesJobTypeHandle.IndustrialProcessDataLookup,
                    IndustrialCompanyDataLookup = m_UpdateSpawnablesJobTypeHandle.IndustrialCompanyDataLookup,
                    ExtractorCompanyDataLookup = m_UpdateSpawnablesJobTypeHandle.ExtractorCompanyDataLookup,
                    CommercialCompanyDataLookup = m_UpdateSpawnablesJobTypeHandle.CommercialCompanyDataLookup,
                    ServiceCompanyDataLookup = m_UpdateSpawnablesJobTypeHandle.ServiceCompanyDataLookup,
                    StorageLimitDataLookup = m_UpdateSpawnablesJobTypeHandle.StorageLimitDataLookup,
                    TransportCompanyDataLookup = m_UpdateSpawnablesJobTypeHandle.TransportCompanyDataLookup,
                };
                Dependency = updateSpawnablesJob.Schedule(m_UpdateSpawnablesJobQuery, Dependency);
                Barrier.AddJobHandleForProducer(Dependency);
            }

            if (m_LocalSettings.Settings.OfficesEnabled && !m_UpdateHighOfficesJobQuery.IsEmptyIgnoreFilter)
            {
                Mod.Instance.Log.Info("Run UpdateHighOfficesJob");
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
            Mod.Instance.Log.Info("System destroyed.");

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
                Mod.Instance.Log.Error($"Error loading settings: {e.Message}");
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
