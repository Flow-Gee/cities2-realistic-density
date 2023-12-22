using Colossal.UI.Binding;
using Game.Audio;
using Game.Prefabs;
using Game.UI;
using System;
using System.Diagnostics;
using Unity.Entities;

namespace RealisticDensity.Systems
{
    public class RealisticDensityUISystem : UISystemBase
    {
        private readonly string kGroup = "89pleasure_realisticdensity";
        static readonly string[] TRIGGER_UPDATE_PROPERTIES = new[]
        {
            "UseStickyWhiteness",
            "WhitenessToggle",
            "TimeOfDay",
            "Weather",
            "FreezeVisualTime"
        };

        private RealisticDensitySystem m_RealisticDensitySystem;
        private EntityQuery _soundQuery;

        /// <summary>
        /// Create our bindings
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            m_RealisticDensitySystem = World.GetOrCreateSystemManaged<RealisticDensitySystem>();
            _soundQuery = GetEntityQuery(ComponentType.ReadOnly<ToolUXSoundSettingsData>());


            AddBinding(new TriggerBinding(kGroup, "triggerSound", TriggerUISound));
            AddBinding(new TriggerBinding<string>(kGroup, "launchUrl", OpenURL));
        }

        /// <summary>
        /// Open a URL in the web browser
        /// </summary>
        /// <param name="url"></param>
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

        /// <summary>
        /// Trigger a UI sound
        /// </summary>
        private void TriggerUISound()
        {
            AudioManager.instance.PlayUISoundIfNotPlaying(_soundQuery.GetSingleton<ToolUXSoundSettingsData>().m_SnapSound);
        }
    }
}