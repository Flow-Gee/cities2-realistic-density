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
using Unity.Mathematics;
using UnityEngine.Scripting;
using RealisticDensity.Prefabs;

namespace RealisticDensity.Systems
{

    public partial class RealisticDensitySystem : GameSystemBase
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
                        ComponentType.Exclude<RealisticDensityData>(),
                        ComponentType.Exclude<Deleted>(),
                        ComponentType.Exclude<Temp>(),
                    ],
                }
            });

            RequireForUpdate(m_WorkplaceDataGroup);
            UnityEngine.Debug.Log("[RealisticDensity]: System created...");
        }

        [Preserve]
        protected override void OnUpdate()
        {
            UnityEngine.Debug.Log("[RealisticDensity]: Updating prefabs...");

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
        readonly static int ProductionFactor = 3;
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
                RealisticDensityData realisticDensityData = default;
                Entity entity = entities[i];
                if (WorkplaceDataLookup.TryGetComponent(entity, out WorkplaceData workplaceData))
                {
                    // Power plants need a lot more workers for more realism
                    if (PowerPlantDataLookup.HasComponent(entity))
                    {
                        Ecb.SetComponent(i, entity, UpdateWorkplaceData(WorkforceFactors.PowerPlant.Medium, workplaceData, realisticDensityData));
                    }

                    if (SchoolDataLookup.HasComponent(entity))
                    {
                        Ecb.SetComponent(i, entity, UpdateWorkplaceData(WorkforceFactors.School.Medium, workplaceData, realisticDensityData));
                    }

                    // Industrial buildings only
                    if (IndustrialProcessDataLookup.TryGetComponent(entity, out IndustrialProcessData industrialProcessData))
                    {
                        float workforceFactor = WorkforceFactors.IndustryProcessing.High;
                        Ecb.SetComponent(i, entity, UpdateWorkplaceData(workforceFactor, workplaceData, realisticDensityData));

                        IndustrialProcessData updatedIndustrialProcessData = industrialProcessData;
                        realisticDensityData.industrialProcessData_MaxWorkersPerCell = industrialProcessData.m_MaxWorkersPerCell;
                        updatedIndustrialProcessData.m_MaxWorkersPerCell *= workforceFactor;

                        realisticDensityData.industrialProcessData_Output_Amount = industrialProcessData.m_Output.m_Amount;
                        updatedIndustrialProcessData.m_Output.m_Amount += CalculateProductionFactor(industrialProcessData.m_Output.m_Amount, workforceFactor);

                        realisticDensityData.industrialProcessData_WorkPerUnit = industrialProcessData.m_WorkPerUnit;
                        updatedIndustrialProcessData.m_WorkPerUnit -= CalculateProductionFactor(updatedIndustrialProcessData.m_WorkPerUnit, workforceFactor);
                        Ecb.SetComponent(i, entity, updatedIndustrialProcessData);

                        if (StorageLimitDataLookup.TryGetComponent(entity, out StorageLimitData storageLimitData))
                        {
                            StorageLimitData updatedStorageLimitData = storageLimitData;
                            realisticDensityData.storageLimitData_limit = storageLimitData.m_Limit;
                            updatedStorageLimitData.m_Limit += CalculateProductionFactor(storageLimitData.m_Limit, workforceFactor);
                            Ecb.SetComponent(i, entity, updatedStorageLimitData);
                        }

                        if (TransportCompanyDataLookup.TryGetComponent(entity, out TransportCompanyData transportCompanyData))
                        {
                            TransportCompanyData updatedTransportCompanyData = transportCompanyData;
                            realisticDensityData.transportCompanyData_MaxTransports = transportCompanyData.m_MaxTransports;
                            updatedTransportCompanyData.m_MaxTransports += CalculateProductionFactor(transportCompanyData.m_MaxTransports, workforceFactor);
                            Ecb.SetComponent(i, entity, updatedTransportCompanyData);
                        }
                    }

                    // Commercial buildings only
                    if (ServiceCompanyDataLookup.TryGetComponent(entity, out ServiceCompanyData serviceCompanyData))
                    {
                        float workforceFactor = WorkforceFactors.Commercial.High;

                        ServiceCompanyData updatedServiceCompanyData = serviceCompanyData;

                        realisticDensityData.serviceCompanyData_MaxWorkersPerCell = serviceCompanyData.m_MaxWorkersPerCell;
                        updatedServiceCompanyData.m_MaxWorkersPerCell += CalculateProductionFactor(serviceCompanyData.m_MaxWorkersPerCell, workforceFactor);

                        realisticDensityData.serviceCompanyData_MaxWorkersPerCell = serviceCompanyData.m_WorkPerUnit;
                        updatedServiceCompanyData.m_WorkPerUnit -= CalculateProductionFactor(serviceCompanyData.m_WorkPerUnit, workforceFactor);
                        Ecb.SetComponent(i, entity, updatedServiceCompanyData);
                    }

                    // Add a Modded component to prevent this entity from being updated again
                    Ecb.AddComponent(i, entity, realisticDensityData);
                }
            }
        }

        private readonly int CalculateProductionFactor(float of, float factor)
        {
            return (int)(of - ((int)math.round(of * factor))) / ProductionFactor;
        }

        private readonly WorkplaceData UpdateWorkplaceData(float factor, WorkplaceData workplaceData, RealisticDensityData realisticDensityData)
        {
            WorkplaceData updatedWorkplaceData = workplaceData;
            realisticDensityData.workplaceData_MaxWorkers = workplaceData.m_MaxWorkers;
            updatedWorkplaceData.m_MaxWorkers = (int)math.round(updatedWorkplaceData.m_MaxWorkers * factor);

            return updatedWorkplaceData;
        }
    }
}
