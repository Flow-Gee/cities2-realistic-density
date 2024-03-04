using RealisticDensity.Configuration;
using System.Collections.Generic;

namespace FindStuff.Configuration
{
    public class RealisticDensityConfig : ConfigBase
    {
        protected override string ConfigFileName => "config.json";

        public bool OrderByAscending
        {
            get;
            set;
        } = true;

        public bool EnableShortcut
        {
            get;
            set;
        } = false;

        public bool ExpertMode
        {
            get;
            set;
        } = false;

        public Dictionary<string, ushort> RecentSearches
        {
            get;
            set;
        } = [];
    }
}
