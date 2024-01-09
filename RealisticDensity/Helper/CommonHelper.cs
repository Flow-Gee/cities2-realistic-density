using Game.Prefabs;
using RealisticDensity.Common;
using RealisticDensity.Systems;
using Unity.Mathematics;

namespace RealisticDensity.Helper
{
    internal static class CommonHelper
    {
        public static float CalculateProductionFactor(float factor, float of)
        {
            return of * (factor - 1.0f) / RealisticDensitySystem.kProductionFactor;
        }

        public static WorkplaceData UpdateWorkplaceData(float factor, WorkplaceData workplaceData)
        {
            WorkplaceData updatedWorkplaceData = workplaceData;
            updatedWorkplaceData.m_MaxWorkers = (int)math.round(updatedWorkplaceData.m_MaxWorkers * factor);

            return updatedWorkplaceData;
        }

        public static float MaxWorkersPerCellIncrease(float workforceFactor, float maxWorkersPerCell)
        {
            return Calc.DecimalFloat(CalculateProductionFactor(workforceFactor, maxWorkersPerCell));
        }

        public static int WorkPerUnitIncrease(float workforceFactor, int workPerUnit)
        {
            float balancingFactor = workforceFactor * 0.75f;
            return (int)math.round(CalculateProductionFactor(workforceFactor, workPerUnit) / math.max(1, balancingFactor));
        }
    }
}
