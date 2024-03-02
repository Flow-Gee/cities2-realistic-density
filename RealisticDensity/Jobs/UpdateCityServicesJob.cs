
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
using Unity.Mathematics;

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
                        ComponentType.ReadOnly<BuildingData>(),
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
                        ComponentType.ReadOnly<AdminBuildingData>(),
                        ComponentType.ReadOnly<WelfareOfficeData>(),
                        ComponentType.ReadOnly<ResearchFacilityData>(),
                        ComponentType.ReadOnly<TelecomFacilityData>(),
                        ComponentType.ReadOnly<ParkData>(),
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

            BuildingDataLookup = state.GetComponentLookup<BuildingData>();
            WorkplaceDataLookup = state.GetComponentLookup<WorkplaceData>();
            PowerPlantDataLookup = state.GetComponentLookup<PowerPlantData>();
            SchoolDataLookup = state.GetComponentLookup<SchoolData>();
            HospitalDataLookup = state.GetComponentLookup<HospitalData>();
            PoliceStationDataLookup = state.GetComponentLookup<PoliceStationData>();
            PrisonDataLookup = state.GetComponentLookup<PrisonData>();
            FireStationDataLookup = state.GetComponentLookup<FireStationData>();
            CargoTransportStationDataLookup = state.GetComponentLookup<CargoTransportStationData>();
            TransportDepotDataLookup = state.GetComponentLookup<TransportDepotData>();
            GarbageFacilityDataLookup = state.GetComponentLookup<GarbageFacilityData>();
            DeathcareFacilityDataLookup = state.GetComponentLookup<DeathcareFacilityData>();
            PublicTransportStationDataLookup = state.GetComponentLookup<PublicTransportStationData>();
            MaintenanceDepotDataLookup = state.GetComponentLookup<MaintenanceDepotData>();
            PostFacilityDataLookup = state.GetComponentLookup<PostFacilityData>();
            AdminBuildingDataLookup = state.GetComponentLookup<AdminBuildingData>();
            WelfareOfficeDataLookup = state.GetComponentLookup<WelfareOfficeData>();
            ResearchFacilityDataLookup = state.GetComponentLookup<ResearchFacilityData>();
            TelecomFacilityDataLookup = state.GetComponentLookup<TelecomFacilityData>();
            ParkDataLookup = state.GetComponentLookup<ParkData>();
        }

        [ReadOnly]
        public EntityTypeHandle EntityTypeHandle;

        public ComponentLookup<BuildingData> BuildingDataLookup;
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
        public ComponentLookup<AdminBuildingData> AdminBuildingDataLookup;
        public ComponentLookup<WelfareOfficeData> WelfareOfficeDataLookup;
        public ComponentLookup<ResearchFacilityData> ResearchFacilityDataLookup;
        public ComponentLookup<TelecomFacilityData> TelecomFacilityDataLookup;
        public ComponentLookup<ParkData> ParkDataLookup;
    }

    public struct UpdateCityServicesJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public EntityTypeHandle EntityTypeHandle;

        public ComponentLookup<BuildingData> BuildingDataLookup;
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
        public ComponentLookup<AdminBuildingData> AdminBuildingDataLookup;
        public ComponentLookup<WelfareOfficeData> WelfareOfficeDataLookup;
        public ComponentLookup<ResearchFacilityData> ResearchFacilityDataLookup;
        public ComponentLookup<TelecomFacilityData> TelecomFacilityDataLookup;
        public ComponentLookup<ParkData> ParkDataLookup;

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
                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(WorkforceFactors.PowerPlant.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (SchoolDataLookup.HasComponent(entity))
            {
                // TODO: Do change the amount of workplaces in schools based on the current students amount
                // Schools can also be tweaked. A teacher ratio of 1:12 is the average in Germany 2017
                // Based on research from https://www.theglobaleconomy.com/Germany/Student_teacher_ratio_primary_school/#:~:text=For%20that%20indicator%2C%20we%20provide,is%2012.3%20students%20per%20teacher.
                // int teacherAmount = (int)math.round(SchoolDataLookup[entity].m_StudentCapacity / 12.3f);
                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(WorkforceFactors.School.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (HospitalDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(WorkforceFactors.Hospital.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (PoliceStationDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(WorkforceFactors.PoliceStation.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (PrisonDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(WorkforceFactors.Prison.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (FireStationDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(WorkforceFactors.FireStation.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (CargoTransportStationDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(WorkforceFactors.CargoTransportStation.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (TransportDepotDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(WorkforceFactors.TransportDepot.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (GarbageFacilityDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(WorkforceFactors.GarbageFacility.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (DeathcareFacilityDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(WorkforceFactors.DeathcareFacility.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (PublicTransportStationDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(WorkforceFactors.PublicTransportStation.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (MaintenanceDepotDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(WorkforceFactors.MaintenanceDepot.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (PostFacilityDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(WorkforceFactors.PostFacility.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (AdminBuildingDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(WorkforceFactors.AdminBuilding.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (WelfareOfficeDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(WorkforceFactors.WelfareOffice.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (ResearchFacilityDataLookup.HasComponent(entity))
            {
                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(WorkforceFactors.ResearchFacility.Medium, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (TelecomFacilityDataLookup.HasComponent(entity))
            {
                BuildingData buildingData = BuildingDataLookup[entity];
                float lotSizeMultiplier = math.max(1, buildingData.m_LotSize.y * buildingData.m_LotSize.x / 100);
                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(WorkforceFactors.TelecomFacility.Medium * lotSizeMultiplier, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }
            else if (ParkDataLookup.HasComponent(entity))
            {
                BuildingData buildingData = BuildingDataLookup[entity];
                float lotSizeMultiplier = math.max(1, buildingData.m_LotSize.y * buildingData.m_LotSize.x / 100);
                var updatedWorkplaceData = DensityHelper.UpdateWorkplaceData(WorkforceFactors.ParkData.Medium * lotSizeMultiplier, workplaceData);
                Ecb.SetComponent(entityIndex, entity, updatedWorkplaceData);
            }

            realisticDensityData.workplaceData_MaxWorkers = workplaceData.m_MaxWorkers;
            Ecb.AddComponent(entityIndex, entity, realisticDensityData);
        }
    }
}