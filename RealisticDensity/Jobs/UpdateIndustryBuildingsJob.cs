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
using UnityEngine;

namespace RealisticDensity.Jobs
{
    struct UpdateIndustryBuildingsQuery
    {
        public EntityQueryDesc[] Query;

        public UpdateIndustryBuildingsQuery()
        {
            Query = [
                new()
                {
                    All =
                    [
                        ComponentType.ReadOnly<WorkplaceData>(),
                        ComponentType.ReadOnly<IndustrialProcessData>(),
                        ComponentType.ReadOnly<IndustrialCompanyData>(),
                    ],
                    None =
                    [
                        ComponentType.Exclude<DefaultData>(),
                        ComponentType.Exclude<Deleted>(),
                        ComponentType.Exclude<Temp>(),
                    ],
                }
            ];
        }
    }

    public struct UpdateIndustryBuildingsTypeHandle
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignHandles(ref SystemState state)
        {
            EntityTypeHandle = state.GetEntityTypeHandle();
            WorkplaceDataLookup = state.GetComponentLookup<WorkplaceData>();
            IndustrialProcessDataLookup = state.GetComponentLookup<IndustrialProcessData>();
            ExtractorCompanyDataLookup = state.GetComponentLookup<ExtractorCompanyData>();
            StorageLimitDataLookup = state.GetComponentLookup<StorageLimitData>();
            TransportCompanyDataLookup = state.GetComponentLookup<TransportCompanyData>();
        }

        [ReadOnly]
        public EntityTypeHandle EntityTypeHandle;

        public ComponentLookup<WorkplaceData> WorkplaceDataLookup;
        public ComponentLookup<IndustrialProcessData> IndustrialProcessDataLookup;
        public ComponentLookup<ExtractorCompanyData> ExtractorCompanyDataLookup;
        public ComponentLookup<StorageLimitData> StorageLimitDataLookup;
        public ComponentLookup<TransportCompanyData> TransportCompanyDataLookup;
    }

    public struct UpdateIndustryBuildingsJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public EntityTypeHandle EntityHandle;

        public ComponentLookup<WorkplaceData> WorkplaceDataLookup;
        public ComponentLookup<IndustrialProcessData> IndustrialProcessDataLookup;
        public ComponentLookup<ExtractorCompanyData> ExtractorCompanyDataLookup;
        public ComponentLookup<StorageLimitData> StorageLimitDataLookup;
        public ComponentLookup<TransportCompanyData> TransportCompanyDataLookup;

        public void Execute(in ArchetypeChunk chunk,
            int unfilteredChunkIndex,
            bool useEnabledMask,
            in v128 chunkEnabledMask)
        {
            RealisticDensitySettings settings = RealisticDensitySystem.Settings;
            NativeArray<Entity> entities = chunk.GetNativeArray(EntityHandle);
            ChunkEntityEnumerator enumerator = new(useEnabledMask, chunkEnabledMask, chunk.Count);
            while (enumerator.NextEntityIndex(out int i))
            {
                Entity entity = entities[i];
                DefaultData realisticDensityData = new();
                WorkplaceData workplaceData = WorkplaceDataLookup[entity];

                IndustrialProcessData industrialProcessData = IndustrialProcessDataLookup[entity];
                bool isOffice = IsOffice(industrialProcessData);
                if (isOffice && !settings.OfficesEnabled)
                {
                    continue;
                }

                float workforceFactor = isOffice ? settings.OfficesFactor : ExtractorCompanyDataLookup.HasComponent(entity) ? settings.IndustryExtractorFactor : settings.IndustryProcessingFactor;

                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(workforceFactor, workplaceData);
                Ecb.SetComponent(i, entity, updatedWorkplaceData);

                IndustrialProcessData updatedIndustrialProcessData = industrialProcessData;
                realisticDensityData.industrialProcessData_MaxWorkersPerCell = industrialProcessData.m_MaxWorkersPerCell;
                updatedIndustrialProcessData.m_MaxWorkersPerCell += CommonHelper.MaxWorkersPerCellIncrease(workforceFactor, industrialProcessData.m_MaxWorkersPerCell);

                realisticDensityData.industrialProcessData_WorkPerUnit = industrialProcessData.m_WorkPerUnit;
                updatedIndustrialProcessData.m_WorkPerUnit += CommonHelper.WorkPerUnitIncrease(workforceFactor, industrialProcessData.m_WorkPerUnit);

                Ecb.SetComponent(i, entity, updatedIndustrialProcessData);

                if (settings.IndustryIncreaseStorageCapacity && StorageLimitDataLookup.TryGetComponent(entity, out StorageLimitData storageLimitData))
                {
                    StorageLimitData updatedStorageLimitData = storageLimitData;

                    realisticDensityData.storageLimitData_limit = storageLimitData.m_Limit;
                    float storageLimitFactor = CommonHelper.CalculateProductionFactor(workforceFactor, storageLimitData.m_Limit);
                    updatedStorageLimitData.m_Limit += Mathf.CeilToInt(storageLimitFactor);

                    Ecb.SetComponent(i, entity, updatedStorageLimitData);
                }

                if (settings.IndustryIncreaseMaxTransports && TransportCompanyDataLookup.TryGetComponent(entity, out TransportCompanyData transportCompanyData))
                {
                    TransportCompanyData updatedTransportCompanyData = transportCompanyData;

                    realisticDensityData.transportCompanyData_MaxTransports = transportCompanyData.m_MaxTransports;
                    float maxTransportsFactor = CommonHelper.CalculateProductionFactor(workforceFactor, transportCompanyData.m_MaxTransports);
                    updatedTransportCompanyData.m_MaxTransports += Mathf.CeilToInt(maxTransportsFactor);

                    Ecb.SetComponent(i, entity, updatedTransportCompanyData);
                }

                realisticDensityData.workplaceData_MaxWorkers = workplaceData.m_MaxWorkers;
                Ecb.AddComponent(i, entity, realisticDensityData);
            }
        }

        private readonly bool IsOffice(IndustrialProcessData industrialProcessData)
        {
            return industrialProcessData.m_Output.m_Resource switch
            {
                Game.Economy.Resource.Software or Game.Economy.Resource.Financial or Game.Economy.Resource.Media => true,
                _ => false,
            };
        }
    }
}