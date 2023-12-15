using Game;
using Game.Common;
using Game.Companies;
using Game.Prefabs;
using Game.Tools;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
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
                        ComponentType.ReadOnly<StorageLimitData>(),
                        ComponentType.ReadOnly<TransportCompanyData>(),
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
            m_TypeHandle.StorageLimitDataLookup.Update(ref CheckedStateRef);
            m_TypeHandle.TransportCompanyDataLookup.Update(ref CheckedStateRef);

            UpdateWorkProviderJob updateWorkplaceData = new()
            {
                EntityHandle = GetEntityTypeHandle(),

                WorkplaceDataLookup = m_TypeHandle.WorkplaceDataLookup,
                IndustrialProcessDataLookup = m_TypeHandle.IndustrialProcessDataLookup,
                ServiceCompanyDataLookup = m_TypeHandle.ServiceCompanyDataLookup,
                PowerPlantDataLookup = m_TypeHandle.PowerPlantDataLookup,
                SchoolDataLookup = m_TypeHandle.SchoolDataLookup,
                StorageLimitDataLookup = m_TypeHandle.StorageLimitDataLookup,
                TransportCompanyDataLookup = m_TypeHandle.TransportCompanyDataLookup,

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
                StorageLimitDataLookup = state.GetComponentLookup<StorageLimitData>(false);
                TransportCompanyDataLookup = state.GetComponentLookup<TransportCompanyData>(false);
            }

            public ComponentLookup<WorkplaceData> WorkplaceDataLookup;
            public ComponentLookup<IndustrialProcessData> IndustrialProcessDataLookup;
            public ComponentLookup<ServiceCompanyData> ServiceCompanyDataLookup;
            public ComponentLookup<PowerPlantData> PowerPlantDataLookup;
            public ComponentLookup<SchoolData> SchoolDataLookup;
            public ComponentLookup<StorageLimitData> StorageLimitDataLookup;
            public ComponentLookup<TransportCompanyData> TransportCompanyDataLookup;
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
        public ComponentLookup<StorageLimitData> StorageLimitDataLookup;
        public ComponentLookup<TransportCompanyData> TransportCompanyDataLookup;

        public void Execute(in ArchetypeChunk chunk,
            int unfilteredChunkIndex,
            bool useEnabledMask,
            in v128 chunkEnabledMask)
        {
            NativeArray<Entity> entities = chunk.GetNativeArray(EntityHandle);
            ChunkEntityEnumerator enumerator = new(useEnabledMask, chunkEnabledMask, chunk.Count);
            while (enumerator.NextEntityIndex(out int i))
            {
                Modded modded = default;
                Entity entity = entities[i];
                if (WorkplaceDataLookup.TryGetComponent(entity, out WorkplaceData workplaceData))
                {
                    // Power plants need a lot more workers for more realism
                    if (PowerPlantDataLookup.HasComponent(entity))
                    {
                        Ecb.SetComponent(i, entity, UpdateWorkplaceData(WorkforceFactors.PowerPlant.Medium, workplaceData, modded));
                        continue;
                    }

                    if (SchoolDataLookup.HasComponent(entity))
                    {
                        Ecb.SetComponent(i, entity, UpdateWorkplaceData(WorkforceFactors.School.Medium, workplaceData, modded));
                        continue;
                    }

                    // Industrial buildings only
                    if (IndustrialProcessDataLookup.TryGetComponent(entity, out IndustrialProcessData industrialProcessData))
                    {
                        float workforceFactor = WorkforceFactors.IndustryProcessing.High;
                        Ecb.SetComponent(i, entity, UpdateWorkplaceData(workforceFactor, workplaceData, modded));

                        IndustrialProcessData updatedIndustrialProcessData = industrialProcessData;
                        modded.industrialProcessData_MaxWorkersPerCell = industrialProcessData.m_MaxWorkersPerCell;
                        updatedIndustrialProcessData.m_MaxWorkersPerCell *= workforceFactor;
                        modded.industrialProcessData_Output_Amount = industrialProcessData.m_Output.m_Amount;
                        updatedIndustrialProcessData.m_Output.m_Amount *= (int)math.round(workforceFactor);
                        // updatedIndustrialProcessData.m_WorkPerUnit += (int)math.round(updatedIndustrialProcessData.m_WorkPerUnit * workforceFactor);

                        Ecb.SetComponent(i, entity, updatedIndustrialProcessData);

                        if (StorageLimitDataLookup.TryGetComponent(entity, out StorageLimitData storageLimitData))
                        {
                            StorageLimitData updatedStorageLimitData = storageLimitData;
                            modded.storageLimitData_limit = storageLimitData.m_Limit;
                            updatedStorageLimitData.m_Limit *= (int)math.round(workforceFactor);
                            Ecb.SetComponent(i, entity, updatedStorageLimitData);
                        }

                        if (TransportCompanyDataLookup.TryGetComponent(entity, out TransportCompanyData transportCompanyData))
                        {
                            TransportCompanyData updatedTransportCompanyData = transportCompanyData;
                            modded.transportCompanyData_MaxTransports = transportCompanyData.m_MaxTransports;
                            updatedTransportCompanyData.m_MaxTransports = (int)math.round(workforceFactor);
                            Ecb.SetComponent(i, entity, updatedTransportCompanyData);
                        }

                        continue;
                    }

                    // Commercial buildings only
                    if (ServiceCompanyDataLookup.TryGetComponent(entity, out ServiceCompanyData serviceCompanyData))
                    {
                        float workforceFactor = WorkforceFactors.Commercial.High;

                        ServiceCompanyData updatedServiceCompanyData = serviceCompanyData;
                        modded.serviceCompanyData_MaxWorkersPerCell = serviceCompanyData.m_MaxWorkersPerCell;
                        updatedServiceCompanyData.m_MaxWorkersPerCell *= workforceFactor;
                        //updatedServiceCompanyData.m_WorkPerUnit /= adjustmentValue;
                        Ecb.SetComponent(i, entity, updatedServiceCompanyData);
                        continue;
                    }

                    // Add a Modded component to prevent this entity from being updated again
                    Ecb.AddComponent(i, entity, modded);
                }
            }
        }

        private readonly WorkplaceData UpdateWorkplaceData(float factor, WorkplaceData workplaceData, Modded modded)
        {
            WorkplaceData updatedWorkplaceData = workplaceData;
            modded.workplaceData_MaxWorkers = workplaceData.m_MaxWorkers;
            updatedWorkplaceData.m_MaxWorkers = (int)math.round(updatedWorkplaceData.m_MaxWorkers * factor);

            return updatedWorkplaceData;
        }
    }
}
