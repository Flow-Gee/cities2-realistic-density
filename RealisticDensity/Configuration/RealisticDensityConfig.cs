using RealisticDensity.Configuration;

namespace FindStuff.Configuration
{
    public class RealisticDensityConfig : ConfigBase
    {
        protected override string ConfigFileName => "config.json";

        public bool IsEnabled
        {
            get;
            set;
        } = false;
    }
}
