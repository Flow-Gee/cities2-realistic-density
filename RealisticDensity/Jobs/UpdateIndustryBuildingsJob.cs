using Game.Common;
using Game.Companies;
using Game.Prefabs;
using Game.Tools;
using RealisticDensity.Helper;
using RealisticDensity.Prefabs;
using RealisticDensity.Settings;
using RealisticDensity.Systems;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
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
                        ComponentType.ReadOnly<PrefabData>(),
                    ],
                    Any = [
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
            ResourceDataLookup = state.GetComponentLookup<ResourceData>();
        }

        [ReadOnly]
        public EntityTypeHandle EntityTypeHandle;

        public ComponentLookup<WorkplaceData> WorkplaceDataLookup;
        public ComponentLookup<IndustrialProcessData> IndustrialProcessDataLookup;
        public ComponentLookup<ExtractorCompanyData> ExtractorCompanyDataLookup;
        public ComponentLookup<StorageLimitData> StorageLimitDataLookup;
        public ComponentLookup<TransportCompanyData> TransportCompanyDataLookup;
        public ComponentLookup<ResourceData> ResourceDataLookup;
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
        public ComponentLookup<ResourceData> ResourceDataLookup;

        public EconomyParameterData EconomyParameterData;
        public ResourcePrefabs ResourcePrefabs;
        public BuildingData BuildingData;
        public SpawnableBuildingData SpawnableBuildingData;
        public bool OfficesEnabled;
        public float OfficesFactor;
        public float IndustryExtractorFactor;
        public float IndustryProcessingFactor;
        public bool IndustryIncreaseStorageCapacity;
        public bool IndustryIncreaseMaxTransports;

        public void Execute(in ArchetypeChunk chunk,
            int unfilteredChunkIndex,
            bool useEnabledMask,
            in v128 chunkEnabledMask)
        {
            NativeArray<Entity> entities = chunk.GetNativeArray(EntityHandle);
            ChunkEntityEnumerator enumerator = new(useEnabledMask, chunkEnabledMask, chunk.Count);
            PrefabSystem prefabSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<PrefabSystem>();
            while (enumerator.NextEntityIndex(out int i))
            {
                Entity entity = entities[i];
                CompanyPrefab prefab = prefabSystem.GetPrefab<CompanyPrefab>(entity);
                DefaultData realisticDensityData = new();
                WorkplaceData workplaceData = WorkplaceDataLookup[entity];

                IndustrialProcessData industrialProcessData = IndustrialProcessDataLookup[entity];
                bool isOffice = IsOffice(industrialProcessData);
                bool isExtractor = ExtractorCompanyDataLookup.HasComponent(entity);
                if (isOffice && !OfficesEnabled)
                {
                    continue;
                }

                float workforceFactor = isOffice ? OfficesFactor : isExtractor ? IndustryExtractorFactor : IndustryProcessingFactor;

                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(workforceFactor, workplaceData);
                Ecb.SetComponent(i, entity, updatedWorkplaceData);

                IndustrialProcessData updatedIndustrialProcessData = industrialProcessData;
                realisticDensityData.industrialProcessData_MaxWorkersPerCell = industrialProcessData.m_MaxWorkersPerCell;
                updatedIndustrialProcessData.m_MaxWorkersPerCell += DensityHelper.MaxWorkersPerCellIncrease(workforceFactor, industrialProcessData.m_MaxWorkersPerCell);

                realisticDensityData.industrialProcessData_WorkPerUnit = industrialProcessData.m_WorkPerUnit;

                if (isExtractor)
                {
                    updatedIndustrialProcessData.m_WorkPerUnit = DensityHelper.WorkPerUnitForExtractorIndustry(
                        prefab,
                        updatedIndustrialProcessData,
                        updatedWorkplaceData,
                        ResourcePrefabs,
                        ResourceDataLookup,
                        EconomyParameterData,
                        SpawnableBuildingData
                    );
                } else
                {
                    updatedIndustrialProcessData.m_WorkPerUnit = DensityHelper.WorkPerUnitForProcessingIndustry(
                        prefab,
                        updatedIndustrialProcessData,
                        updatedWorkplaceData,
                        ResourcePrefabs,
                        ResourceDataLookup,
                        EconomyParameterData,
                        BuildingData,
                        SpawnableBuildingData
                    );
                }

                Ecb.SetComponent(i, entity, updatedIndustrialProcessData);

                if (IndustryIncreaseStorageCapacity && StorageLimitDataLookup.TryGetComponent(entity, out StorageLimitData storageLimitData))
                {
                    StorageLimitData updatedStorageLimitData = storageLimitData;

                    realisticDensityData.storageLimitData_limit = storageLimitData.m_Limit;
                    float storageLimitFactor = DensityHelper.CalculateProductionFactor(workforceFactor, storageLimitData.m_Limit);
                    updatedStorageLimitData.m_Limit += Mathf.CeilToInt(storageLimitFactor);

                    Ecb.SetComponent(i, entity, updatedStorageLimitData);
                }

                if (IndustryIncreaseMaxTransports && TransportCompanyDataLookup.TryGetComponent(entity, out TransportCompanyData transportCompanyData))
                {
                    TransportCompanyData updatedTransportCompanyData = transportCompanyData;

                    realisticDensityData.transportCompanyData_MaxTransports = transportCompanyData.m_MaxTransports;
                    float maxTransportsFactor = DensityHelper.CalculateProductionFactor(workforceFactor, transportCompanyData.m_MaxTransports);
                    updatedTransportCompanyData.m_MaxTransports += Mathf.CeilToInt(maxTransportsFactor);

                    Ecb.SetComponent(i, entity, updatedTransportCompanyData);
                }

                realisticDensityData.workplaceData_MaxWorkers = workplaceData.m_MaxWorkers;
                Ecb.AddComponent(i, entity, realisticDensityData);
            }

            entities.Dispose();
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