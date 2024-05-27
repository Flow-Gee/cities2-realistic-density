using System.Collections.Generic;
using Colossal;
using Game.Modding;
using RealisticDensity.Systems;

namespace RealisticDensity.Settings
{
    public class Setting : ModSetting
    {
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

        public Setting(IMod mod) : base(mod)
        {
        }

        public override void SetDefaults()
        {

        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Realistic Density" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DisableMod)), "Disable Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DisableMod)), "Completely disable the functionality of this mod" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpawnablesEnabled)), "Enable Spawnables" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpawnablesEnabled)), "Enable or disable mod for spawnables" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CommercialsEnabled)), "Enable Commercials" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CommercialsEnabled)), "Enable or disable mod for commercials" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CommercialsFactor)), "Commercials Factor" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CommercialsFactor)), "Set the commercials mod for factor" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OfficesEnabled)), "Enable Offices" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OfficesEnabled)), "Enable or disable mod for offices" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OfficesFactor)), "Offices Factor" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OfficesFactor)), "Set the offices factor" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.IndustriesEnabled)), "Enable Industries" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.IndustriesEnabled)), "Enable or disable industries" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.IndustryExtractorFactor)), "Industry Extractor Factor" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.IndustryExtractorFactor)), "Set the industry extractor factor" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.IndustryProcessingFactor)), "Industry Processing Factor" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.IndustryProcessingFactor)), "Set the industry processing factor" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.IndustrySellingFactor)), "Industry Selling Factor" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.IndustrySellingFactor)), "Set the industry selling factor" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.IndustryIncreaseStorageCapacity)), "Increase Industry Storage Capacity" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.IndustryIncreaseStorageCapacity)), "Enable or disable increase in industry storage capacity" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.IndustryIncreaseMaxTransports)), "Increase Industry Max Transports" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.IndustryIncreaseMaxTransports)), "Enable or disable mod for increase in industry max transports" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CityServicesEnabled)), "Enable City Services" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CityServicesEnabled)), "Enable or disable mod for city services" },
            };
        }

        public void Unload()
        {
        }
    }
}
