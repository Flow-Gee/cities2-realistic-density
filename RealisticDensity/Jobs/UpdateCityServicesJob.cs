
using Game.Common;
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
    public struct UpdateCityServicesQuery
    {
        public EntityQueryDesc[] Query;

        public UpdateCityServicesQuery()
        {
            Query =
            [
                new()
                {
                    All =
                    [
                        ComponentType.ReadOnly<WorkplaceData>(),
                    ],
                    Any =
                    [
                        ComponentType.ReadOnly<PowerPlantData>(),
                        ComponentType.ReadOnly<SchoolData>(),
                        ComponentType.ReadOnly<HospitalData>(),
                        ComponentType.ReadOnly<PoliceStationData>(),
                        ComponentType.ReadOnly<FireStationData>(),
                        ComponentType.ReadOnly<CargoTransportStationData>(),
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

    public struct UpdateCityServicesTypeHandle
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignHandles(ref SystemState state)
        {
            EntityTypeHandle = state.GetEntityTypeHandle();
            WorkplaceDataLookup = state.GetComponentLookup<WorkplaceData>(false);
            PowerPlantDataLookup = state.GetComponentLookup<PowerPlantData>(false);
            SchoolDataLookup = state.GetComponentLookup<SchoolData>(false);
            HospitalDataLookup = state.GetComponentLookup<HospitalData>(false);
            PoliceStationDataLookup = state.GetComponentLookup<PoliceStationData>(false);
            FireStationDataLookup = state.GetComponentLookup<FireStationData>(false);
            CargoTransportStationDataLookup = state.GetComponentLookup<CargoTransportStationData>(false);
        }

        [ReadOnly]
        public EntityTypeHandle EntityTypeHandle;

        public ComponentLookup<WorkplaceData> WorkplaceDataLookup;
        public ComponentLookup<PowerPlantData> PowerPlantDataLookup;
        public ComponentLookup<SchoolData> SchoolDataLookup;
        public ComponentLookup<HospitalData> HospitalDataLookup;
        public ComponentLookup<PoliceStationData> PoliceStationDataLookup;
        public ComponentLookup<FireStationData> FireStationDataLookup;
        public ComponentLookup<CargoTransportStationData> CargoTransportStationDataLookup;
    }

    public struct UpdateCityServicesJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public EntityTypeHandle EntityTypeHandle;

        public ComponentLookup<WorkplaceData> WorkplaceDataLookup;
        public ComponentLookup<PowerPlantData> PowerPlantDataLookup;
        public ComponentLookup<SchoolData> SchoolDataLookup;
        public ComponentLookup<HospitalData> HospitalDataLookup;
        public ComponentLookup<PoliceStationData> PoliceStationDataLookup;
        public ComponentLookup<FireStationData> FireStationDataLookup;
        public ComponentLookup<CargoTransportStationData> CargoTransportStationDataLookup;

        public void Execute(in ArchetypeChunk chunk,
            int unfilteredChunkIndex,
            bool useEnabledMask,
            in v128 chunkEnabledMask)
        {
            NativeArray<Entity> entities = chunk.GetNativeArray(EntityTypeHandle);
            ChunkEntityEnumerator enumerator = new(useEnabledMask, chunkEnabledMask, chunk.Count);
            while (enumerator.NextEntityIndex(out int entityIndex))
            {
                Entity entity = entities[entityIndex];
                RealisticDensityData realisticDensityData = default;
                if (WorkplaceDataLookup.TryGetComponent(entity, out WorkplaceData workplaceData))
                {
                    UpdateWorkplaceData(entity, workplaceData, ref realisticDensityData, entityIndex);
                }
            }
        }

        private void UpdateWorkplaceData(Entity entity, WorkplaceData workplaceData, ref RealisticDensityData realisticDensityData, int entityIndex)
        {
            // Power plants need a lot more workers for more realism
            if (PowerPlantDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(WorkforceFactors.PowerPlant.Medium, workplaceData, ref realisticDensityData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (SchoolDataLookup.HasComponent(entity))
            {
                // TODO: Do change the amount of workplaces in schools based on the current students amount
                // Schools can also be tweaked. A teacher ratio of 1:12 is the average in Germany 2017
                // Based on research from https://www.theglobaleconomy.com/Germany/Student_teacher_ratio_primary_school/#:~:text=For%20that%20indicator%2C%20we%20provide,is%2012.3%20students%20per%20teacher.
                // int teacherAmount = (int)math.round(SchoolDataLookup[entity].m_StudentCapacity / 12.3f);
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(WorkforceFactors.School.Medium, workplaceData, ref realisticDensityData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }

            else if (HospitalDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(WorkforceFactors.Hospital.Medium, workplaceData, ref realisticDensityData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }

            else if (PoliceStationDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(WorkforceFactors.PoliceStation.Medium, workplaceData, ref realisticDensityData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }

            else if (FireStationDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(WorkforceFactors.FireStation.Medium, workplaceData, ref realisticDensityData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }

            else if (CargoTransportStationDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(WorkforceFactors.CargoTransportStation.Medium, workplaceData, ref realisticDensityData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }

            Ecb.AddComponent(entityIndex, entity, realisticDensityData);
        }
    }
}