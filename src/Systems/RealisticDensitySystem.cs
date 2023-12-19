using Game;
using Unity.Entities;
using UnityEngine.Scripting;
using Game.SceneFlow;
using RealisticDensity.Settings;
using RealisticDensity.Jobs;
using Unity.Jobs;

namespace RealisticDensity.Systems
{

    public partial class RealisticDensitySystem : GameSystemBase
    {
        readonly public static int kComponentVersion = 1;

        // Workforce is a third of the production outcome calculation for spawnable entities like commercial, industrial and offices
        readonly public static int kProductionFactor = 3;

        private EndFrameBarrier Barrier;

        public LocalSettings m_LocalSettings;
        public static LocalSettingsItem Settings;
        public bool m_LocalSettingsLoaded = false;

        private UpdateSpawnablesTypeHandle m_UpdateSpawnablesJobTypeHandle;
        private EntityQuery m_UpdateSpawnablesJobQuery;
        private UpdateCityServicesTypeHandle m_UpdateCityServicesJobTypeHandle;
        private EntityQuery m_UpdateCityServicesJobQuery;

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

            RequireAnyForUpdate(m_UpdateCityServicesJobQuery, m_UpdateSpawnablesJobQuery);
            Mod.Instance.Log.Info("System created.");
        }

        [Preserve]
        protected override void OnUpdate()
        {
            if (Settings.DisableMod == true || GameManager.instance == null || !GameManager.instance.gameMode.IsGameOrEditor())
            {
                return;
            }

            if (!m_UpdateCityServicesJobQuery.IsEmptyIgnoreFilter)
            {
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

            if (!m_UpdateSpawnablesJobQuery.IsEmptyIgnoreFilter)
            {
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
    }
}
