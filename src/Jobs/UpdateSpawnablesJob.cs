using Game.Common;
using Game.Companies;
using Game.Prefabs;
using Game.Tools;
using RealisticDensity.Common;
using RealisticDensity.Helper;
using RealisticDensity.Prefabs;
using RealisticDensity.Settings;
using RealisticDensity.Systems;
using System.Runtime.CompilerServices;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace RealisticDensity.Jobs
{
    struct UpdateSpawnablesQuery
    {
        public EntityQueryDesc[] Query;

        public UpdateSpawnablesQuery()
        {
            Query = [
                new()
                {
                    All =
                    [
                        ComponentType.ReadOnly<WorkplaceData>(),
                    ],
                    Any =
                    [
                        ComponentType.ReadOnly<IndustrialProcessData>(),
                        ComponentType.ReadOnly<IndustrialCompanyData>(),
                        ComponentType.ReadOnly<ServiceCompanyData>(),
                        ComponentType.ReadOnly<StorageLimitData>(),
                        ComponentType.ReadOnly<TransportCompanyData>(),
                    ],
                    None =
                    [
                        ComponentType.ReadOnly<RealisticDensityData>(),
                        ComponentType.Exclude<Deleted>(),
                        ComponentType.Exclude<Temp>(),
                    ],
                }
            ];
        }
    }

    public struct UpdateSpawnablesTypeHandle
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignHandles(ref SystemState state)
        {
            EntityTypeHandle = state.GetEntityTypeHandle();
            WorkplaceDataLookup = state.GetComponentLookup<WorkplaceData>(false);
            IndustrialProcessDataLookup = state.GetComponentLookup<IndustrialProcessData>(false);
            IndustrialCompanyDataLookup = state.GetComponentLookup<IndustrialCompanyData>(false);
            ExtractorCompanyDataLookup = state.GetComponentLookup<ExtractorCompanyData>(false);
            CommercialCompanyDataLookup = state.GetComponentLookup<CommercialCompanyData>(false);
            ServiceCompanyDataLookup = state.GetComponentLookup<ServiceCompanyData>(false);
            StorageLimitDataLookup = state.GetComponentLookup<StorageLimitData>(false);
            TransportCompanyDataLookup = state.GetComponentLookup<TransportCompanyData>(false);
        }

        [ReadOnly]
        public EntityTypeHandle EntityTypeHandle;

        public ComponentLookup<WorkplaceData> WorkplaceDataLookup;
        public ComponentLookup<IndustrialProcessData> IndustrialProcessDataLookup;
        public ComponentLookup<IndustrialCompanyData> IndustrialCompanyDataLookup;
        public ComponentLookup<ExtractorCompanyData> ExtractorCompanyDataLookup;
        public ComponentLookup<CommercialCompanyData> CommercialCompanyDataLookup;
        public ComponentLookup<ServiceCompanyData> ServiceCompanyDataLookup;
        public ComponentLookup<StorageLimitData> StorageLimitDataLookup;
        public ComponentLookup<TransportCompanyData> TransportCompanyDataLookup;
    }

    public struct UpdateSpawnablesJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public EntityTypeHandle EntityHandle;

        public ComponentLookup<WorkplaceData> WorkplaceDataLookup;
        public ComponentLookup<IndustrialProcessData> IndustrialProcessDataLookup;
        public ComponentLookup<IndustrialCompanyData> IndustrialCompanyDataLookup;
        public ComponentLookup<ExtractorCompanyData> ExtractorCompanyDataLookup;
        public ComponentLookup<CommercialCompanyData> CommercialCompanyDataLookup;
        public ComponentLookup<ServiceCompanyData> ServiceCompanyDataLookup;
        public ComponentLookup<StorageLimitData> StorageLimitDataLookup;
        public ComponentLookup<TransportCompanyData> TransportCompanyDataLookup;

        public void Execute(in ArchetypeChunk chunk,
            int unfilteredChunkIndex,
            bool useEnabledMask,
            in v128 chunkEnabledMask)
        {
            LocalSettingsItem settings = RealisticDensitySystem.Settings;
            NativeArray<Entity> entities = chunk.GetNativeArray(EntityHandle);
            ChunkEntityEnumerator enumerator = new(useEnabledMask, chunkEnabledMask, chunk.Count);
            while (enumerator.NextEntityIndex(out int i))
            {
                Entity entity = entities[i];
                RealisticDensityData realisticDensityData = default;
                if (WorkplaceDataLookup.TryGetComponent(entity, out WorkplaceData workplaceData))
                {
                    
                    // Industrial buildings only
                    if (settings.IndustriesEnabled && IsIndustrialCompany(entity, out IndustrialProcessData industrialProcessData))
                    {
                        float workforceFactor = ExtractorCompanyDataLookup.HasComponent(entity) ? settings.IndustryExtractorFactor : settings.IndustryProcessingFactor;

                        var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(workforceFactor, workplaceData, ref realisticDensityData);
                        Ecb.SetComponent(i, entity, updatedWorkplaceData);

                        IndustrialProcessData updatedIndustrialProcessData = industrialProcessData;

                        realisticDensityData.industrialProcessData_MaxWorkersPerCell = industrialProcessData.m_MaxWorkersPerCell;
                        float maxWorkersPerCellfactor = CommonHelper.CalculateProductionFactor(workforceFactor, industrialProcessData.m_MaxWorkersPerCell);
                        updatedIndustrialProcessData.m_MaxWorkersPerCell += Calc.DecimalFloat(maxWorkersPerCellfactor);

                        realisticDensityData.industrialProcessData_WorkPerUnit = industrialProcessData.m_WorkPerUnit;
                        float workPerUnitFactor = CommonHelper.CalculateProductionFactor(workforceFactor, industrialProcessData.m_WorkPerUnit);
                        updatedIndustrialProcessData.m_WorkPerUnit += (int)math.round(workPerUnitFactor);

                        Ecb.SetComponent(i, entity, updatedIndustrialProcessData);

                        if (settings.IndustryIncreaseStorageCapacity && StorageLimitDataLookup.TryGetComponent(entity, out StorageLimitData storageLimitData))
                        {
                            StorageLimitData updatedStorageLimitData = storageLimitData;

                            realisticDensityData.storageLimitData_limit = storageLimitData.m_Limit;
                            float storageLimitFactor = CommonHelper.CalculateProductionFactor(workforceFactor, storageLimitData.m_Limit);
                            updatedStorageLimitData.m_Limit += (int)math.round(storageLimitFactor);

                            Ecb.SetComponent(i, entity, updatedStorageLimitData);
                        }

                        if (TransportCompanyDataLookup.TryGetComponent(entity, out TransportCompanyData transportCompanyData))
                        {
                            TransportCompanyData updatedTransportCompanyData = transportCompanyData;

                            realisticDensityData.transportCompanyData_MaxTransports = transportCompanyData.m_MaxTransports;
                            float maxTransportsFactor = CommonHelper.CalculateProductionFactor(workforceFactor, transportCompanyData.m_MaxTransports);
                            updatedTransportCompanyData.m_MaxTransports += (int)math.round(maxTransportsFactor);

                            Ecb.SetComponent(i, entity, updatedTransportCompanyData);
                        }

                        Ecb.AddComponent(i, entity, realisticDensityData);
                    }

                    // Commercial buildings only
                    if (settings.CommercialsEnabled && IsCommercialCompany(entity, out ServiceCompanyData serviceCompanyData))
                    {
                        float workforceFactor = WorkforceFactors.Commercial.High;
                        var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(workforceFactor, workplaceData, ref realisticDensityData);
                        Ecb.SetComponent(i, entity, updatedWorkplaceData);

                        ServiceCompanyData updatedServiceCompanyData = serviceCompanyData;

                        realisticDensityData.serviceCompanyData_MaxWorkersPerCell = serviceCompanyData.m_MaxWorkersPerCell;
                        float maxWorkersPerCellFactor = CommonHelper.CalculateProductionFactor(workforceFactor, serviceCompanyData.m_MaxWorkersPerCell);
                        updatedServiceCompanyData.m_MaxWorkersPerCell += Calc.DecimalFloat(maxWorkersPerCellFactor);

                        realisticDensityData.serviceCompanyData_WorkPerUnit = serviceCompanyData.m_WorkPerUnit;
                        float workPerUnitFactor = CommonHelper.CalculateProductionFactor(workforceFactor, serviceCompanyData.m_WorkPerUnit);
                        updatedServiceCompanyData.m_WorkPerUnit += (int)math.round(workPerUnitFactor);

                        Ecb.SetComponent(i, entity, updatedServiceCompanyData);
                        Ecb.AddComponent(i, entity, realisticDensityData);
                    }
                }
            }
        }

        private bool IsIndustrialCompany(Entity entity, out IndustrialProcessData industrialProcessData)
        {
            if (IndustrialCompanyDataLookup.HasComponent(entity) && IndustrialProcessDataLookup.TryGetComponent(entity, out industrialProcessData))
            {
                return true;
            }

            industrialProcessData = default;
            return false;
        }

        private bool IsCommercialCompany(Entity entity, out ServiceCompanyData serviceCompanyData)
        {
            if (CommercialCompanyDataLookup.HasComponent(entity) && ServiceCompanyDataLookup.TryGetComponent(entity, out serviceCompanyData))
            {
                return true;
            }

            serviceCompanyData = default;
            return false;
        }
    }
}