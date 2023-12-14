using Game;
using Game.Common;
using Game.Companies;
using Game.Prefabs;
using Game.Tools;
using System.Runtime.CompilerServices;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.Scripting;
using WorkforceRealismEnhancement.Prefabs;

namespace WorkforceRealismEnhancement.Systems
{

    public partial class WorkforceRealistmEnhancementSystem : GameSystemBase
    {
        public EntityQuery m_WorkplaceDataGroup;
        private TypeHandle m_TypeHandle;
        private ModificationEndBarrier m_ModificationEndBarrier;

        [Preserve]
        protected override void OnCreate()
        {
            base.OnCreate();
            m_ModificationEndBarrier = World.GetOrCreateSystemManaged<ModificationEndBarrier>();
            m_WorkplaceDataGroup = GetEntityQuery(new EntityQueryDesc[]
            {
                new() {
                    All =
                    [
                        ComponentType.ReadOnly<WorkplaceData>(),
                    ],
                    Any =
                    [
                        ComponentType.ReadOnly<IndustrialProcessData>(),
                        ComponentType.ReadOnly<ServiceCompanyData>(),
                        ComponentType.ReadOnly<PowerPlantData>(),
                        ComponentType.ReadOnly<SchoolData>(),
                    ],
                    None =
                    [
                        ComponentType.Exclude<Modded>(),
                        ComponentType.Exclude<Deleted>(),
                        ComponentType.Exclude<Temp>(),
                    ],
                }
            });

            RequireForUpdate(m_WorkplaceDataGroup);
            UnityEngine.Debug.Log("WorkplaceBalancerSystem created");
        }

        [Preserve]
        protected override void OnUpdate()
        {
            UnityEngine.Debug.Log("[WREM]: Updating prefabs...");

            m_TypeHandle.WorkplaceDataLookup.Update(ref CheckedStateRef);
            m_TypeHandle.IndustrialProcessDataLookup.Update(ref CheckedStateRef);
            m_TypeHandle.ServiceCompanyDataLookup.Update(ref CheckedStateRef);
            m_TypeHandle.PowerPlantDataLookup.Update(ref CheckedStateRef);
            m_TypeHandle.SchoolDataLookup.Update(ref CheckedStateRef);

            UpdateWorkProviderJob updateWorkplaceData = new()
            {
                EntityHandle = GetEntityTypeHandle(),

                WorkplaceDataLookup = m_TypeHandle.WorkplaceDataLookup,
                IndustrialProcessDataLookup = m_TypeHandle.IndustrialProcessDataLookup,
                ServiceCompanyDataLookup = m_TypeHandle.ServiceCompanyDataLookup,
                PowerPlantDataLookup = m_TypeHandle.PowerPlantDataLookup,
                SchoolDataLookup = m_TypeHandle.SchoolDataLookup,

                Ecb = m_ModificationEndBarrier.CreateCommandBuffer().AsParallelWriter(),
            };
            Dependency = updateWorkplaceData.Schedule(m_WorkplaceDataGroup, Dependency);
            m_ModificationEndBarrier.AddJobHandleForProducer(Dependency);
        }

        protected override void OnCreateForCompiler()
        {
            base.OnCreateForCompiler();
            m_TypeHandle.AssignHandles(ref CheckedStateRef);
        }

        private struct TypeHandle
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void AssignHandles(ref SystemState state)
            {
                WorkplaceDataLookup = state.GetComponentLookup<WorkplaceData>(false);
                IndustrialProcessDataLookup = state.GetComponentLookup<IndustrialProcessData>(false);
                ServiceCompanyDataLookup = state.GetComponentLookup<ServiceCompanyData>(false);
                PowerPlantDataLookup = state.GetComponentLookup<PowerPlantData>(false);
                SchoolDataLookup = state.GetComponentLookup<SchoolData>(false);
            }

            public ComponentLookup<WorkplaceData> WorkplaceDataLookup;
            public ComponentLookup<IndustrialProcessData> IndustrialProcessDataLookup;
            public ComponentLookup<ServiceCompanyData> ServiceCompanyDataLookup;
            public ComponentLookup<PowerPlantData> PowerPlantDataLookup;
            public ComponentLookup<SchoolData> SchoolDataLookup;
        }
    }


    public struct UpdateWorkProviderJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public EntityTypeHandle EntityHandle;

        public ComponentLookup<WorkplaceData> WorkplaceDataLookup;
        public ComponentLookup<IndustrialProcessData> IndustrialProcessDataLookup;
        public ComponentLookup<ServiceCompanyData> ServiceCompanyDataLookup;
        public ComponentLookup<PowerPlantData> PowerPlantDataLookup;
        public ComponentLookup<SchoolData> SchoolDataLookup;

        // The multiplier to change the overall workforce
        int workforceAdjustment;

        public void Execute(in ArchetypeChunk chunk,
            int unfilteredChunkIndex,
            bool useEnabledMask,
            in v128 chunkEnabledMask)
        {
            workforceAdjustment = 2;
            NativeArray<Entity> entities = chunk.GetNativeArray(EntityHandle);
            ChunkEntityEnumerator enumerator = new(useEnabledMask, chunkEnabledMask, chunk.Count);
            while (enumerator.NextEntityIndex(out int i))
            {
                Entity entity = entities[i];
                if (WorkplaceDataLookup.TryGetComponent(entity, out WorkplaceData workplaceData))
                {
                    // Power plants need a lot more workers for more realism
                    if (PowerPlantDataLookup.HasComponent(entity))
                    {
                        workforceAdjustment = 8;
                    }

                    if (SchoolDataLookup.HasComponent(entity))
                    {
                        workforceAdjustment = 3;
                    }

                    WorkplaceData updatedWorkplaceData = workplaceData;
                    updatedWorkplaceData.m_MaxWorkers *= workforceAdjustment;
                    Ecb.SetComponent(i, entity, updatedWorkplaceData);

                    if (IndustrialProcessDataLookup.TryGetComponent(entity, out IndustrialProcessData industrialProcessData))
                    {
                        IndustrialProcessData updatedIndustrialProcessData = industrialProcessData;
                        updatedIndustrialProcessData.m_MaxWorkersPerCell *= workforceAdjustment;
                        //updatedIndustrialProcessData.m_WorkPerUnit /= adjustmentValue;
                        Ecb.SetComponent(i, entity, updatedIndustrialProcessData);
                    }

                    if (ServiceCompanyDataLookup.TryGetComponent(entity, out ServiceCompanyData serviceCompanyData))
                    {
                        ServiceCompanyData updatedServiceCompanyData = serviceCompanyData;
                        updatedServiceCompanyData.m_MaxWorkersPerCell *= workforceAdjustment;
                        //updatedServiceCompanyData.m_WorkPerUnit /= adjustmentValue;
                        Ecb.SetComponent(i, entity, updatedServiceCompanyData);
                    }

                    Ecb.AddComponent(i, entity, new Modded());
                }
            }
        }
    }
}
