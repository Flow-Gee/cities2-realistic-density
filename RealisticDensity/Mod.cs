using Colossal.Logging;
using Game;
using Game.Modding;
using RealisticDensity.Systems;
using System;
using Colossal.IO.AssetDatabase;
using Game.SceneFlow;
using RealisticDensity.Settings;
using Unity.Entities;

namespace RealisticDensity
{
    public class Mod : IMod
    {
        public static Mod Instance { get; set; }
        private static readonly ILog _log = LogManager.GetLogger(nameof(RealisticDensity)).SetShowsErrorsInUI(false);
        private World _world;
        public static Setting Setting;

        public void OnLoad(UpdateSystem updateSystem)
        {
            Setting = new Setting(this);
            Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(Setting));
            AssetDatabase.global.LoadSettings(nameof(RealisticDensity), Setting, new Setting(this));

            updateSystem.UpdateAt<RealisticDensitySystem>(SystemUpdatePhase.ModificationEnd);
            _world = updateSystem.World;
        }

        private void SafelyRemove<T>()
            where T : GameSystemBase
        {
            var system = _world.GetExistingSystemManaged<T>();

            if (system != null)
                _world?.DestroySystemManaged(system);
        }

        public void OnDispose()
        {
            SafelyRemove<RealisticDensitySystem>();
        }

        public static void DebugLog(string message)
        {
            _log.Info(message);
        }
    }
}
