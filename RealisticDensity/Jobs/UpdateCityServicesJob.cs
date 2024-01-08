
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
                        ComponentType.ReadOnly<ServiceObjectData>(),
                    ],
                    Any =
                    [
                        ComponentType.ReadOnly<PowerPlantData>(),
                        ComponentType.ReadOnly<SchoolData>(),
                        ComponentType.ReadOnly<HospitalData>(),
                        ComponentType.ReadOnly<PoliceStationData>(),
                        ComponentType.ReadOnly<PrisonData>(),
                        ComponentType.ReadOnly<FireStationData>(),
                        ComponentType.ReadOnly<CargoTransportStationData>(),
                        ComponentType.ReadOnly<TransportDepotData>(),
                        ComponentType.ReadOnly<GarbageFacilityData>(),
                        ComponentType.ReadOnly<DeathcareFacilityData>(),
                        ComponentType.ReadOnly<PublicTransportStationData>(),
                        ComponentType.ReadOnly<MaintenanceDepotData>(),
                        ComponentType.ReadOnly<PostFacilityData>(),
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
            PrisonDataLookup = state.GetComponentLookup<PrisonData>(false);
            FireStationDataLookup = state.GetComponentLookup<FireStationData>(false);
            CargoTransportStationDataLookup = state.GetComponentLookup<CargoTransportStationData>(false);
            TransportDepotDataLookup = state.GetComponentLookup<TransportDepotData>(false);
            GarbageFacilityDataLookup = state.GetComponentLookup<GarbageFacilityData>(false);
            DeathcareFacilityDataLookup = state.GetComponentLookup<DeathcareFacilityData>(false);
            PublicTransportStationDataLookup = state.GetComponentLookup<PublicTransportStationData>(false);
            MaintenanceDepotDataLookup = state.GetComponentLookup<MaintenanceDepotData>(false);
            PostFacilityDataLookup = state.GetComponentLookup<PostFacilityData>(false);
        }

        [ReadOnly]
        public EntityTypeHandle EntityTypeHandle;

        public ComponentLookup<WorkplaceData> WorkplaceDataLookup;
        public ComponentLookup<PowerPlantData> PowerPlantDataLookup;
        public ComponentLookup<SchoolData> SchoolDataLookup;
        public ComponentLookup<HospitalData> HospitalDataLookup;
        public ComponentLookup<PoliceStationData> PoliceStationDataLookup;
        public ComponentLookup<PrisonData> PrisonDataLookup;
        public ComponentLookup<FireStationData> FireStationDataLookup;
        public ComponentLookup<CargoTransportStationData> CargoTransportStationDataLookup;
        public ComponentLookup<TransportDepotData> TransportDepotDataLookup;
        public ComponentLookup<GarbageFacilityData> GarbageFacilityDataLookup;
        public ComponentLookup<DeathcareFacilityData> DeathcareFacilityDataLookup;
        public ComponentLookup<PublicTransportStationData> PublicTransportStationDataLookup;
        public ComponentLookup<MaintenanceDepotData> MaintenanceDepotDataLookup;
        public ComponentLookup<PostFacilityData> PostFacilityDataLookup;
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
        public ComponentLookup<PrisonData> PrisonDataLookup;
        public ComponentLookup<FireStationData> FireStationDataLookup;
        public ComponentLookup<CargoTransportStationData> CargoTransportStationDataLookup;
        public ComponentLookup<TransportDepotData> TransportDepotDataLookup;
        public ComponentLookup<GarbageFacilityData> GarbageFacilityDataLookup;
        public ComponentLookup<DeathcareFacilityData> DeathcareFacilityDataLookup;
        public ComponentLookup<PublicTransportStationData> PublicTransportStationDataLookup;
        public ComponentLookup<MaintenanceDepotData> MaintenanceDepotDataLookup;
        public ComponentLookup<PostFacilityData> PostFacilityDataLookup;

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
                if (WorkplaceDataLookup.TryGetComponent(entity, out WorkplaceData workplaceData))
                {
                    UpdateWorkplaceData(entity, workplaceData, entityIndex);
                }
            }
        }

        private void UpdateWorkplaceData(Entity entity, WorkplaceData workplaceData, int entityIndex)
        {
            DefaultData realisticDensityData = new();

            // Power plants need a lot more workers for more realism
            if (PowerPlantDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(WorkforceFactors.PowerPlant.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (SchoolDataLookup.HasComponent(entity))
            {
                // TODO: Do change the amount of workplaces in schools based on the current students amount
                // Schools can also be tweaked. A teacher ratio of 1:12 is the average in Germany 2017
                // Based on research from https://www.theglobaleconomy.com/Germany/Student_teacher_ratio_primary_school/#:~:text=For%20that%20indicator%2C%20we%20provide,is%2012.3%20students%20per%20teacher.
                // int teacherAmount = (int)math.round(SchoolDataLookup[entity].m_StudentCapacity / 12.3f);
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(WorkforceFactors.School.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (HospitalDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(WorkforceFactors.Hospital.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (PoliceStationDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(WorkforceFactors.PoliceStation.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (PrisonDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(WorkforceFactors.Prison.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (FireStationDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(WorkforceFactors.FireStation.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (CargoTransportStationDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(WorkforceFactors.CargoTransportStation.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (TransportDepotDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(WorkforceFactors.TransportDepot.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (GarbageFacilityDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(WorkforceFactors.GarbageFacility.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (DeathcareFacilityDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(WorkforceFactors.DeathcareFacility.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (PublicTransportStationDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(WorkforceFactors.PublicTransportStation.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (MaintenanceDepotDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(WorkforceFactors.MaintenanceDepot.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (PostFacilityDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = CommonHelper.UpdateWorkplaceData(WorkforceFactors.PostFacility.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }

            realisticDensityData.workplaceData_MaxWorkers = workplaceData.m_MaxWorkers;
            Ecb.AddComponent(entityIndex, entity, realisticDensityData);
        }
    }
}