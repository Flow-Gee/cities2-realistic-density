using Game.Common;
using Game.Prefabs;
using Game.Tools;
using RealisticDensity.Prefabs;
using RealisticDensity.Settings;
using RealisticDensity.Systems;
using System.Runtime.CompilerServices;
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
                        ComponentType.ReadOnly<BuildingPropertyData>(),
                        ComponentType.ReadOnly<OfficeBuilding>(),
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

    public struct UpdateHighOfficesTypeHandle
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignHandles(ref SystemState state)
        {
            EntityTypeHandle = state.GetEntityTypeHandle();
            BuildingPropertyDataLookup = state.GetComponentLookup<BuildingPropertyData>(false);
        }

        [ReadOnly]
        public EntityTypeHandle EntityTypeHandle;
        public ComponentLookup<BuildingPropertyData> BuildingPropertyDataLookup;
    }

    public struct UpdateHighOfficesJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public EntityTypeHandle EntityHandle;

        public ComponentLookup<BuildingPropertyData> BuildingPropertyDataLookup;

        public void Execute(in ArchetypeChunk chunk,
            int unfilteredChunkIndex,
            bool useEnabledMask,
            in v128 chunkEnabledMask)
        {
            RealisticDensitySettings settings = RealisticDensitySystem.Settings;
            NativeArray<Entity> entities = chunk.GetNativeArray(EntityHandle);
            ChunkEntityEnumerator enumerator = new(useEnabledMask, chunkEnabledMask, chunk.Count);
            while (enumerator.NextEntityIndex(out int i))
            {
                Entity entity = entities[i];
                RealisticDensityData realisticDensityData = default;
                if (BuildingPropertyDataLookup.TryGetComponent(entity, out BuildingPropertyData buildingPropertyData))
                {
                    float workforceFactor = settings.OfficesFactor;

                    BuildingPropertyData updatedBuildingPropertyData = buildingPropertyData;
                    realisticDensityData.buildingPropertyData_SpaceMultiplier = buildingPropertyData.m_SpaceMultiplier;
                    updatedBuildingPropertyData.m_SpaceMultiplier *= workforceFactor;

                    Ecb.SetComponent(i, entity, updatedBuildingPropertyData);
                    Ecb.AddComponent(i, entity, realisticDensityData);
                }
            }
        }
    }
}