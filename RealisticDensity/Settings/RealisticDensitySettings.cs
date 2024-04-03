using RealisticDensity.Systems;

namespace RealisticDensity.Settings
{
    public class RealisticDensitySettings
    {
        public string Version { get; } = Mod.Version;

        public bool DisableMod { get; set; } = false;
        public bool SpawnablesEnabled { get; set; } = true;
        public bool CommercialsEnabled { get; set; } = true;
        public float CommercialsFactor { get; set; } = WorkforceFactors.Commercial.Medium;
        public bool OfficesEnabled { get; set; } = true;
        public float OfficesFactor { get; set; } = WorkforceFactors.Office.Medium;
        public bool IndustriesEnabled { get; set; } = true;
        public float IndustryExtractorFactor { get; set; } = WorkforceFactors.IndustryExtractor.Medium;
        public float IndustryProcessingFactor { get; set; } = WorkforceFactors.IndustryProcessing.Medium;
        public float IndustrySellingFactor { get; set; } = WorkforceFactors.IndustrySelling.Medium;
        public bool IndustryIncreaseStorageCapacity { get; set; } = false;
        public bool IndustryIncreaseMaxTransports { get; set; } = true;
        public bool CityServicesEnabled { get; set; } = true;
    }
}
