using Colossal.Serialization.Entities;
using RealisticDensity.Systems;
using Unity.Entities;

namespace RealisticDensity.Prefabs
{
    public struct RealisticDensityData : IComponentData, IQueryTypeParameter, ISerializable
    {
        public RealisticDensityData()
        {

        }

        public int version = RealisticDensitySystem.kProductionFactor;

        // WorkplaceData
        public int workplaceData_MaxWorkers = default;

        // IndustrialProcessData
        public float industrialProcessData_MaxWorkersPerCell = default;
        public int industrialProcessData_WorkPerUnit = default;
        public int industrialProcessData_Output_Amount = default;

        // StorageLimitData
        public int storageLimitData_limit = default;

        // TransportCompanyData
        public int transportCompanyData_MaxTransports = default;

        // ServiceCompanyData
        public float serviceCompanyData_MaxWorkersPerCell = default;
        public int serviceCompanyData_WorkPerUnit = default;

        public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
        {
            writer.Write(version);
            writer.Write(workplaceData_MaxWorkers);
            writer.Write(industrialProcessData_MaxWorkersPerCell);
            writer.Write(industrialProcessData_WorkPerUnit);
            writer.Write(industrialProcessData_Output_Amount);
            writer.Write(storageLimitData_limit);
            writer.Write(transportCompanyData_MaxTransports);
            writer.Write(serviceCompanyData_MaxWorkersPerCell);
            writer.Write(serviceCompanyData_WorkPerUnit);
        }

        public void Deserialize<TReader>(TReader reader) where TReader : IReader
        {
            reader.Read(out version);
            reader.Read(out workplaceData_MaxWorkers);
            reader.Read(out industrialProcessData_MaxWorkersPerCell);
            reader.Read(out industrialProcessData_WorkPerUnit);
            reader.Read(out industrialProcessData_Output_Amount);
            reader.Read(out storageLimitData_limit);
            reader.Read(out transportCompanyData_MaxTransports);
            reader.Read(out serviceCompanyData_MaxWorkersPerCell);
            reader.Read(out serviceCompanyData_WorkPerUnit);
        }
    }
}
