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
        }

        [ReadOnly]
        public EntityTypeHandle EntityTypeHandle;

        public ComponentLookup<WorkplaceData> WorkplaceDataLookup;
        public ComponentLookup<ServiceCompanyData> ServiceCompanyDataLookup;
    }

    public struct UpdateCommercialBuildingsJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public EntityTypeHandle EntityHandle;

        public ComponentLookup<WorkplaceData> WorkplaceDataLookup;
        public ComponentLookup<ServiceCompanyData> ServiceCompanyDataLookup;

        public void Execute(in ArchetypeChunk chunk,
            int unfilteredChunkIndex,
            bool useEnabledMask,
            in v128 chunkEnabledMask)
        {
            NativeArray<Entity> entities = chunk.GetNativeArray(EntityHandle);
            ChunkEntityEnumerator enumerator = new(useEnabledMask, chunkEnabledMask, chunk.Count);
            while (enumerator.NextEntityIndex(out int i))
            {
                Entity entity = entities[i];
                DefaultData realisticDensityData = new();

                WorkplaceData workplaceData = WorkplaceDataLookup[entity];
                ServiceCompanyData serviceCompanyData = ServiceCompanyDataLookup[entity];

                float workforceFactor = WorkforceFactors.Commercial.Medium;
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(workforceFactor, workplaceData);
                Ecb.SetComponent(i, entity, updatedWorkplaceData);

                ServiceCompanyData updatedServiceCompanyData = serviceCompanyData;

                realisticDensityData.serviceCompanyData_MaxWorkersPerCell = serviceCompanyData.m_MaxWorkersPerCell;
                updatedServiceCompanyData.m_MaxWorkersPerCell += CommonHelper.MaxWorkersPerCellIncrease(workforceFactor, serviceCompanyData.m_MaxWorkersPerCell);

                realisticDensityData.serviceCompanyData_WorkPerUnit = serviceCompanyData.m_WorkPerUnit;
                updatedServiceCompanyData.m_WorkPerUnit += CommonHelper.WorkPerUnitIncrease(workforceFactor, serviceCompanyData.m_WorkPerUnit);

                Ecb.SetComponent(i, entity, updatedServiceCompanyData);

                realisticDensityData.workplaceData_MaxWorkers = workplaceData.m_MaxWorkers;
                Ecb.AddComponent(i, entity, realisticDensityData);
            }
        }
    }
}