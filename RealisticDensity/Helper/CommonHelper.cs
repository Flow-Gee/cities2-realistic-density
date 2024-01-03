using Game.Prefabs;
using RealisticDensity.Prefabs;
using RealisticDensity.Systems;
using Unity.Mathematics;

namespace RealisticDensity.Helper
{
    internal static class CommonHelper
    {
        public static float CalculateProductionFactor(float factor, float of)
        {
            return (of * factor - of) / RealisticDensitySystem.kProductionFactor;
        }

        public static WorkplaceData UpdateWorkplaceData(float factor, WorkplaceData workplaceData, ref DefaultData realisticDensityData)
        {
            WorkplaceData updatedWorkplaceData = workplaceData;
            realisticDensityData.workplaceData_MaxWorkers = workplaceData.m_MaxWorkers;
            updatedWorkplaceData.m_MaxWorkers = (int)math.round(updatedWorkplaceData.m_MaxWorkers * factor);

            return updatedWorkplaceData;
        }
    }
}
