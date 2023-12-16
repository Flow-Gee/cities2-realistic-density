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
using Game.SceneFlow;

namespace RealisticDensity.Systems
{

    public partial class RealisticDensitySystem : GameSystemBase
    {
        readonly public static int kComponentVersion = 1;

        // Workforce is a third of the production outcome calculation for spawnable entities like commercial, industrial and offices
        readonly public static int kProductionFactor = 3;

        public EntityQuery m_WorkplaceDataGroup;
        private TypeHandle m_TypeHandle;
        private EndFrameBarrier m_EndFrameBarrier;

        [Preserve]
        protected override void OnCreate()
        {
            base.OnCreate();
            m_EndFrameBarrier = World.GetOrCreateSystemManaged<EndFrameBarrier>();
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
                        ComponentType.ReadOnly<HospitalData>(),
                        ComponentType.ReadOnly<PoliceStationData>(),
                        ComponentType.ReadOnly<FireStationData>(),
                        ComponentType.ReadOnly<CargoTransportStationData>(),
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
            });

            RequireForUpdate(m_WorkplaceDataGroup);
            UnityEngine.Debug.Log("[RealisticDensity]: System created...");
        }

        [Preserve]
        protected override void OnUpdate()
        {
            if (GameManager.instance == null || !GameManager.instance.gameMode.IsGameOrEditor())
            {
                UnityEngine.Debug.Log("Nope!");
                return;
            }

            UnityEngine.Debug.Log("[RealisticDensity]: Do some magic...");

            m_TypeHandle.AssignHandles(ref CheckedStateRef);
            UpdateWorkProviderJob updateWorkplaceData = new()
            {
                EntityHandle = GetEntityTypeHandle(),

                WorkplaceDataLookup = m_TypeHandle.WorkplaceDataLookup,
                IndustrialProcessDataLookup = m_TypeHandle.IndustrialProcessDataLookup,
                ServiceCompanyDataLookup = m_TypeHandle.ServiceCompanyDataLookup,
                PowerPlantDataLookup = m_TypeHandle.PowerPlantDataLookup,
                SchoolDataLookup = m_TypeHandle.SchoolDataLookup,
                HospitalDataLookup = m_TypeHandle.HospitalDataLookup,
                PoliceStationDataLookup = m_TypeHandle.PoliceStationDataLookup,
                FireStationDataLookup = m_TypeHandle.FireStationDataLookup,
                CargoTransportStationDataLookup = m_TypeHandle.CargoTransportStationDataLookup,
                StorageLimitDataLookup = m_TypeHandle.StorageLimitDataLookup,
                TransportCompanyDataLookup = m_TypeHandle.TransportCompanyDataLookup,

                Ecb = m_EndFrameBarrier.CreateCommandBuffer().AsParallelWriter(),
            };
            Dependency = updateWorkplaceData.Schedule(m_WorkplaceDataGroup, Dependency);
            m_EndFrameBarrier.AddJobHandleForProducer(Dependency);
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
                HospitalDataLookup = state.GetComponentLookup<HospitalData>(false);
                PoliceStationDataLookup = state.GetComponentLookup<PoliceStationData>(false);
                FireStationDataLookup = state.GetComponentLookup<FireStationData>(false);
                CargoTransportStationDataLookup = state.GetComponentLookup<CargoTransportStationData>(false);
                StorageLimitDataLookup = state.GetComponentLookup<StorageLimitData>(false);
                TransportCompanyDataLookup = state.GetComponentLookup<TransportCompanyData>(false);
            }

            public ComponentLookup<WorkplaceData> WorkplaceDataLookup;
            public ComponentLookup<IndustrialProcessData> IndustrialProcessDataLookup;
            public ComponentLookup<ServiceCompanyData> ServiceCompanyDataLookup;
            public ComponentLookup<PowerPlantData> PowerPlantDataLookup;
            public ComponentLookup<SchoolData> SchoolDataLookup;
            public ComponentLookup<HospitalData> HospitalDataLookup;
            public ComponentLookup<PoliceStationData> PoliceStationDataLookup;
            public ComponentLookup<FireStationData> FireStationDataLookup;
            public ComponentLookup<CargoTransportStationData> CargoTransportStationDataLookup;
            public ComponentLookup<StorageLimitData> StorageLimitDataLookup;
            public ComponentLookup<TransportCompanyData> TransportCompanyDataLookup;
        }
    }

    public struct UpdateWorkProviderJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public EntityTypeHandle EntityHandle;

        public ComponentLookup<RealisticDensityData> RealisticDensityDataLookup;
        public ComponentLookup<WorkplaceData> WorkplaceDataLookup;
        public ComponentLookup<IndustrialProcessData> IndustrialProcessDataLookup;
        public ComponentLookup<ServiceCompanyData> ServiceCompanyDataLookup;
        public ComponentLookup<PowerPlantData> PowerPlantDataLookup;
        public ComponentLookup<SchoolData> SchoolDataLookup;
        public ComponentLookup<HospitalData> HospitalDataLookup;
        public ComponentLookup<PoliceStationData> PoliceStationDataLookup;
        public ComponentLookup<FireStationData> FireStationDataLookup;
        public ComponentLookup<CargoTransportStationData> CargoTransportStationDataLookup;
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
                Entity entity = entities[i];
                RealisticDensityData realisticDensityData = default;
                if (WorkplaceDataLookup.TryGetComponent(entity, out WorkplaceData workplaceData))
                {
                    // Power plants need a lot more workers for more realism
                    if (PowerPlantDataLookup.HasComponent(entity))
                    {
                        Ecb.SetComponent(i, entity, UpdateWorkplaceData(WorkforceFactors.PowerPlant.Medium, workplaceData, ref realisticDensityData));
                    }

                    // Todo: Do change the amount of workplaces in schools based on the current students amount
                    // Schools can also be tweaked. A teacher ratio of 1:12 is the average in Germany 2017
                    // Based on research from https://www.theglobaleconomy.com/Germany/Student_teacher_ratio_primary_school/#:~:text=For%20that%20indicator%2C%20we%20provide,is%2012.3%20students%20per%20teacher.
                    if (SchoolDataLookup.HasComponent(entity))
                    {
                        // int teacherAmount = (int)math.round(SchoolDataLookup[entity].m_StudentCapacity / 12.3f);
                        Ecb.SetComponent(i, entity, UpdateWorkplaceData(WorkforceFactors.School.Medium, workplaceData, ref realisticDensityData));
                    }

                    if (HospitalDataLookup.HasComponent(entity))
                    {
                        Ecb.SetComponent(i, entity, UpdateWorkplaceData(WorkforceFactors.Hospital.Medium, workplaceData, ref realisticDensityData));
                    }

                    if (PoliceStationDataLookup.HasComponent(entity))
                    {
                        Ecb.SetComponent(i, entity, UpdateWorkplaceData(WorkforceFactors.PoliceStation.Medium, workplaceData, ref realisticDensityData));
                    }

                    if (FireStationDataLookup.HasComponent(entity))
                    {
                        Ecb.SetComponent(i, entity, UpdateWorkplaceData(WorkforceFactors.FireStation.Medium, workplaceData, ref realisticDensityData));
                    }

                    if (CargoTransportStationDataLookup.HasComponent(entity))
                    {
                        Ecb.SetComponent(i, entity, UpdateWorkplaceData(WorkforceFactors.CargoTransportStation.Medium, workplaceData, ref realisticDensityData));
                    }

                    // Industrial buildings only
                    if (IndustrialProcessDataLookup.TryGetComponent(entity, out IndustrialProcessData industrialProcessData))
                    {
                        float workforceFactor = WorkforceFactors.IndustryProcessing.High;
                        Ecb.SetComponent(i, entity, UpdateWorkplaceData(workforceFactor, workplaceData, ref realisticDensityData));

                        IndustrialProcessData updatedIndustrialProcessData = industrialProcessData;
                        realisticDensityData.industrialProcessData_MaxWorkersPerCell = industrialProcessData.m_MaxWorkersPerCell;
                        updatedIndustrialProcessData.m_MaxWorkersPerCell *= workforceFactor;

                        realisticDensityData.industrialProcessData_Output_Amount = industrialProcessData.m_Output.m_Amount;
                        updatedIndustrialProcessData.m_Output.m_Amount += (int)math.round(CalculateProductionFactor(workforceFactor, industrialProcessData.m_Output.m_Amount));

                        realisticDensityData.industrialProcessData_WorkPerUnit = industrialProcessData.m_WorkPerUnit;
                        updatedIndustrialProcessData.m_WorkPerUnit -= (int) math.round(CalculateProductionFactor(workforceFactor, updatedIndustrialProcessData.m_WorkPerUnit));
                        Ecb.SetComponent(i, entity, updatedIndustrialProcessData);

                        if (StorageLimitDataLookup.TryGetComponent(entity, out StorageLimitData storageLimitData))
                        {
                            StorageLimitData updatedStorageLimitData = storageLimitData;
                            realisticDensityData.storageLimitData_limit = storageLimitData.m_Limit;
                            updatedStorageLimitData.m_Limit += (int)math.round(CalculateProductionFactor(workforceFactor, storageLimitData.m_Limit));
                            Ecb.SetComponent(i, entity, updatedStorageLimitData);
                        }

                        if (TransportCompanyDataLookup.TryGetComponent(entity, out TransportCompanyData transportCompanyData))
                        {
                            TransportCompanyData updatedTransportCompanyData = transportCompanyData;
                            realisticDensityData.transportCompanyData_MaxTransports = transportCompanyData.m_MaxTransports;
                            updatedTransportCompanyData.m_MaxTransports += (int)math.round(CalculateProductionFactor(workforceFactor, transportCompanyData.m_MaxTransports));
                            Ecb.SetComponent(i, entity, updatedTransportCompanyData);
                        }
                    }

                    // Commercial buildings only
                    if (ServiceCompanyDataLookup.TryGetComponent(entity, out ServiceCompanyData serviceCompanyData))
                    {
                        float workforceFactor = WorkforceFactors.Commercial.High;

                        ServiceCompanyData updatedServiceCompanyData = serviceCompanyData;

                        realisticDensityData.serviceCompanyData_MaxWorkersPerCell = serviceCompanyData.m_MaxWorkersPerCell;
                        updatedServiceCompanyData.m_MaxWorkersPerCell += CalculateProductionFactor(workforceFactor, serviceCompanyData.m_MaxWorkersPerCell);

                        realisticDensityData.serviceCompanyData_MaxWorkersPerCell = serviceCompanyData.m_WorkPerUnit;
                        updatedServiceCompanyData.m_WorkPerUnit -= (int)math.round(CalculateProductionFactor(workforceFactor, serviceCompanyData.m_WorkPerUnit));
                        Ecb.SetComponent(i, entity, updatedServiceCompanyData);
                    }

                    // Add a Modded component to prevent this entity from being updated again
                    Ecb.AddComponent(i, entity, realisticDensityData);
                }
            }
        }

        private readonly float CalculateProductionFactor(float factor, float of)
        {
            return (int)(math.round(of * factor) - of) / RealisticDensitySystem.kProductionFactor;
        }

        private readonly WorkplaceData UpdateWorkplaceData(float factor, WorkplaceData workplaceData, ref RealisticDensityData realisticDensityData)
        {
            WorkplaceData updatedWorkplaceData = workplaceData;
            realisticDensityData.workplaceData_MaxWorkers = workplaceData.m_MaxWorkers;
            updatedWorkplaceData.m_MaxWorkers = (int)math.round(updatedWorkplaceData.m_MaxWorkers * factor);

            return updatedWorkplaceData;
        }
    }
}
