using Gooee.Plugins;
using RealisticDensity.Systems;

namespace RealisticDensity.UI
{
    public class RealisticDensityViewModel : Model
    {
        public bool IsVisible
        {
            get;
            set;
        }

        public bool IsEnabled
        {
            get;
            set;
        } = false;

        public float CommercialFactor
        {
            get;
            set;
        } = WorkforceFactors.Commercial.Medium;
    }
}
