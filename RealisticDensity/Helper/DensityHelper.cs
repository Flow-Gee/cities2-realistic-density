using Game.Companies;
using Game.Economy;
using Game.Prefabs;
using Game.Simulation;
using RealisticDensity.Common;
using RealisticDensity.Systems;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RealisticDensity.Helper
{
    internal static class DensityHelper
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

        public static int WorkPerUnitForCommercial(CompanyPrefab prefab, IndustrialProcessData industrialProcessData, ServiceCompanyData serviceCompanyData, WorkplaceData workplaceData, ResourcePrefabs resourcePrefabs, ComponentLookup<ResourceData> resourceDatas, EconomyParameterData economyParameterData, BuildingData buildingData, SpawnableBuildingData spawnableBuildingData)
        {
            ServiceAvailable serviceAvailable = new()
            {
                m_MeanPriority = 0.5f,
                m_ServiceAvailable = serviceCompanyData.m_MaxService / 2
            };
            float3 tradeCosts = EconomyUtils.BuildPseudoTradeCost(5000f, industrialProcessData, resourceDatas, resourcePrefabs);
            int workers = Mathf.RoundToInt(serviceCompanyData.m_MaxWorkersPerCell * 1000f);
            int num2 = 1;
            int num3 = 65536;
            serviceCompanyData.m_WorkPerUnit = 512;
            do
            {
                int estimatedProfit = ServiceCompanySystem.EstimateDailyProfit(workers, serviceAvailable, serviceCompanyData, buildingData, industrialProcessData, ref economyParameterData, workplaceData, spawnableBuildingData, resourcePrefabs, resourceDatas, tradeCosts);
                if (0.001f * estimatedProfit > prefab.profitability)
                {
                    num2 = serviceCompanyData.m_WorkPerUnit;
                }
                else
                {
                    num3 = serviceCompanyData.m_WorkPerUnit;
                }
                serviceCompanyData.m_WorkPerUnit = (num2 + num3) / 2;
            }
            while (num2 < num3 - 1);
            if (serviceCompanyData.m_WorkPerUnit == 0)
            {
                Debug.Log(string.Format("Warning: calculated work per unit for service company prefab {0} is zero", prefab.name));
            }

            return serviceCompanyData.m_WorkPerUnit;
        }

        public static int WorkPerUnitForExtractorIndustry(CompanyPrefab prefab, IndustrialProcessData industrialProcessData, WorkplaceData workplaceData, ResourcePrefabs resourcePrefabs, ComponentLookup<ResourceData> resourceDatas, EconomyParameterData economyParameterData, SpawnableBuildingData spawnableBuildingData)
        {
            int fittingWorkers = ExtractorAISystem.GetFittingWorkers(156.25f, 1f, industrialProcessData);
            int num4 = 1;
            int num5 = 65536;
            industrialProcessData.m_WorkPerUnit = 512;
            do
            {
                int estimatedProduction = ExtractorCompanySystem.EstimateDailyProduction(1f, fittingWorkers, 1, workplaceData, industrialProcessData, ref economyParameterData);
                int estimatedProfit = ExtractorCompanySystem.EstimateDailyProfit(estimatedProduction, fittingWorkers, industrialProcessData, ref economyParameterData, workplaceData, spawnableBuildingData, resourcePrefabs, resourceDatas);
                if (estimatedProfit * 64f / 10000f > prefab.profitability)
                {
                    num4 = industrialProcessData.m_WorkPerUnit;
                }
                else
                {
                    num5 = industrialProcessData.m_WorkPerUnit;
                }
                industrialProcessData.m_WorkPerUnit = (num4 + num5) / 2;
            }
            while (num4 < num5 - 1);
            industrialProcessData.m_WorkPerUnit = num4;
            if (industrialProcessData.m_WorkPerUnit == 0)
            {
                Debug.LogError(string.Format("calculated work per unit for extractor company prefab {0} is zero", prefab.name));
                industrialProcessData.m_WorkPerUnit = 1;
            }

            return industrialProcessData.m_WorkPerUnit;
        }

        public static int WorkPerUnitForProcessingIndustry(CompanyPrefab prefab, IndustrialProcessData industrialProcessData, WorkplaceData workplaceData, ResourcePrefabs resourcePrefabs, ComponentLookup<ResourceData> resourceDatas, EconomyParameterData economyParameterData, BuildingData buildingData, SpawnableBuildingData spawnableBuildingData)
        {
            float3 float2 = EconomyUtils.BuildPseudoTradeCost(5000f, industrialProcessData, resourceDatas, resourcePrefabs);
            int num6 = Mathf.RoundToInt(industrialProcessData.m_MaxWorkersPerCell * 1000f);
            int num7 = 1;
            int num8 = 65536;
            industrialProcessData.m_WorkPerUnit = 512;
            do
            {
                int estimatedProfit = ProcessingCompanySystem.EstimateDailyProfit(num6, industrialProcessData, buildingData, ref economyParameterData, float2, workplaceData, spawnableBuildingData, resourcePrefabs, resourceDatas);
                if (economyParameterData.m_IndustrialProfitFactor * estimatedProfit > prefab.profitability)
                {
                    num7 = industrialProcessData.m_WorkPerUnit;
                }
                else
                {
                    num8 = industrialProcessData.m_WorkPerUnit;
                }
                industrialProcessData.m_WorkPerUnit = (num7 + num8) / 2;
            }
            while (num7 < num8 - 1);
            if (industrialProcessData.m_WorkPerUnit == 0)
            {
                Debug.Log(string.Format("Warning: calculated work per unit for industry company prefab {0} is zero", prefab.name));
            }
            
            return industrialProcessData.m_WorkPerUnit;
        }
    }
}
