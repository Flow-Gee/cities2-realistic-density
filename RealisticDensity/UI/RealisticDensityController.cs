using FindStuff.Configuration;
using Gooee.Plugins;
using Gooee.Plugins.Attributes;
using RealisticDensity.Configuration;
using System.Diagnostics;
using System;

namespace RealisticDensity.UI
{
    public class RealisticDensityController : Controller<RealisticDensityViewModel>
    {
        public readonly static RealisticDensityConfig _config = ConfigBase.Load<RealisticDensityConfig>();

        public override RealisticDensityViewModel Configure()
        {
            return new RealisticDensityViewModel
            {
                IsVisible = false,
                IsEnabled = false,
            };
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnModelUpdated()
        {

        }

        [OnTrigger]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called by gooee.")]
        private void OnToggleVisible()
        {
            Model.IsVisible = !Model.IsVisible;
            TriggerUpdate();
        }

        /// <summary>
        /// Open a URL in the web browser
        /// </summary>
        /// <param name="url"></param>
        /// Copyright by optimus-code
        [OnTrigger]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called by gooee.")]
        private void OpenURL(string url)
        {
            if (string.IsNullOrEmpty(url))
                return;

            try
            {
                // Launch the URL in the default browser
                Process.Start(url);
            }
            catch (Exception ex)
            {
                // Handle exceptions, if any
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}
