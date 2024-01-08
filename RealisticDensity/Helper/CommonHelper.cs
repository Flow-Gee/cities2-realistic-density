using Game.Prefabs;
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

        public static WorkplaceData UpdateWorkplaceData(float factor, WorkplaceData workplaceData)
        {
            WorkplaceData updatedWorkplaceData = workplaceData;
            updatedWorkplaceData.m_MaxWorkers = (int)math.round(updatedWorkplaceData.m_MaxWorkers * factor);

            return updatedWorkplaceData;
        }
    }
}
