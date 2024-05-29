using Game.Common;
using Game.Prefabs;
using Game.Tools;
using RealisticDensity.Prefabs;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;

namespace RealisticDensity.Jobs
{
    struct UpdateHighOfficesQuery
    {
        public EntityQueryDesc[] Query;

        public UpdateHighOfficesQuery()
        {
            Query = [
                new()
                {
                    All =
                    [
                        ComponentType.ReadOnly<SpawnableBuildingData>(),
                        ComponentType.ReadOnly<BuildingData>(),
                        ComponentType.ReadOnly<PrefabData>(),
                        ComponentType.ReadOnly<BuildingPropertyData>(),
                        ComponentType.ReadOnly<OfficeBuilding>(),
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

    public struct UpdateHighOfficesTypeHandle
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignHandles(ref SystemState state)
        {
            EntityTypeHandle = state.GetEntityTypeHandle();
            BuildingPropertyDataLookup = state.GetComponentLookup<BuildingPropertyData>();
        }

        [ReadOnly]
        public EntityTypeHandle EntityTypeHandle;
        public ComponentLookup<BuildingPropertyData> BuildingPropertyDataLookup;
    }

    [BurstCompile]
    public struct UpdateHighOfficesJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public EntityTypeHandle EntityHandle;
        public ComponentLookup<BuildingPropertyData> BuildingPropertyDataLookup;
        public float OfficesFactor;

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
                BuildingPropertyData buildingPropertyData = BuildingPropertyDataLookup[entity];

                BuildingPropertyData updatedBuildingPropertyData = buildingPropertyData;
                realisticDensityData.buildingPropertyData_SpaceMultiplier = buildingPropertyData.m_SpaceMultiplier;
                updatedBuildingPropertyData.m_SpaceMultiplier *= OfficesFactor;

                Ecb.SetComponent(i, entity, updatedBuildingPropertyData);
                Ecb.AddComponent(i, entity, realisticDensityData);
            }
        }
    }
}