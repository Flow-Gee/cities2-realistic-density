using Gooee;
using Gooee.Plugins;
using Gooee.Plugins.Attributes;
using RealisticDensity;
using RealisticDensity.UI;

namespace FindStuff.UI
{
    [ControllerTypes( typeof(RealisticDensityController) )]
    public class RealisticDensityPlugin : IGooeePluginWithControllers
    {
        public string Name => Mod.Name;
        public string Version => Mod.Version;
        public string ScriptResource => "RealisticDensity.Resources.ui.js";

        public IController[] Controllers
        {
            get;
            set;
        }
    }
}
