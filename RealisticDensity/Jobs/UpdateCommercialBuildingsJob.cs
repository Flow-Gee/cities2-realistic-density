using Game.Common;
using Game.Companies;
using Game.Prefabs;
using Game.Tools;
using RealisticDensity.Helper;
using RealisticDensity.Prefabs;
using RealisticDensity.Systems;
using System.Runtime.CompilerServices;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;

namespace RealisticDensity.Jobs
{
    struct UpdateCommercialBuildingsQuery
    {
        public EntityQueryDesc[] Query;

        public UpdateCommercialBuildingsQuery()
        {
            Query = [
                new()
                {
                    All =
                    [
                        ComponentType.ReadOnly<PrefabData>(),
                        ComponentType.ReadOnly<WorkplaceData>(),
                        ComponentType.ReadOnly<ServiceCompanyData>(),
                        ComponentType.ReadOnly<CommercialCompanyData>(),
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

    public struct UpdateCommercialBuildingsTypeHandle
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignHandles(ref SystemState state)
        {
            EntityTypeHandle = state.GetEntityTypeHandle();
            WorkplaceDataLookup = state.GetComponentLookup<WorkplaceData>();
            ServiceCompanyDataLookup = state.GetComponentLookup<ServiceCompanyData>();
            IndustrialProcessDataLookup = state.GetComponentLookup<IndustrialProcessData>();
            ResourceDataLookup = state.GetComponentLookup<ResourceData>();
        }

        [ReadOnly]
        public EntityTypeHandle EntityTypeHandle;

        public ComponentLookup<WorkplaceData> WorkplaceDataLookup;
        public ComponentLookup<ServiceCompanyData> ServiceCompanyDataLookup;
        public ComponentLookup<IndustrialProcessData> IndustrialProcessDataLookup;
        public ComponentLookup<ResourceData> ResourceDataLookup;
    }

    public struct UpdateCommercialBuildingsJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public EntityTypeHandle EntityHandle;

        public ComponentLookup<WorkplaceData> WorkplaceDataLookup;
        public ComponentLookup<ServiceCompanyData> ServiceCompanyDataLookup;
        public ComponentLookup<IndustrialProcessData> IndustrialProcessDataLookup;
        public ComponentLookup<ResourceData> ResourceDataLookup;

        public EconomyParameterData EconomyParameterData;
        public ResourcePrefabs ResourcePrefabs;
        public BuildingData BuildingData;
        public SpawnableBuildingData SpawnableBuildingData;

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
                DefaultData realisticDensityData = new();
                CompanyPrefab prefab = prefabSystem.GetPrefab<CompanyPrefab>(entity);

                WorkplaceData workplaceData = WorkplaceDataLookup[entity];
                ServiceCompanyData serviceCompanyData = ServiceCompanyDataLookup[entity];

                float workforceFactor = WorkforceFactors.Commercial.Medium;
                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(workforceFactor, workplaceData);
                Ecb.SetComponent(i, entity, updatedWorkplaceData);

                ServiceCompanyData updatedServiceCompanyData = serviceCompanyData;

                realisticDensityData.serviceCompanyData_MaxWorkersPerCell = serviceCompanyData.m_MaxWorkersPerCell;
                updatedServiceCompanyData.m_MaxWorkersPerCell += DensityHelper.MaxWorkersPerCellIncrease(workforceFactor, serviceCompanyData.m_MaxWorkersPerCell);

                realisticDensityData.serviceCompanyData_WorkPerUnit = serviceCompanyData.m_WorkPerUnit;
                updatedServiceCompanyData.m_WorkPerUnit = DensityHelper.WorkPerUnitForCommercial(
                    prefab,
                    IndustrialProcessDataLookup[entity],
                    updatedServiceCompanyData,
                    updatedWorkplaceData,
                    ResourcePrefabs,
                    ResourceDataLookup,
                    EconomyParameterData,
                    BuildingData,
                    SpawnableBuildingData
                );

                Ecb.SetComponent(i, entity, updatedServiceCompanyData);

                realisticDensityData.workplaceData_MaxWorkers = workplaceData.m_MaxWorkers;
                Ecb.AddComponent(i, entity, realisticDensityData);
            }
        }
    }
}