using Colossal.Logging;
using Game;
using Game.Modding;
using RealisticDensity.Systems;

namespace RealisticDensity
{
    public sealed class Mod : IMod
    {
        public const string Name = MyPluginInfo.PLUGIN_NAME;
        public const string Version = MyPluginInfo.PLUGIN_VERSION;
        public static Mod Instance { get; set; }
        internal ILog Log { get; private set; }
        public void OnLoad()
        {
            Instance = this;
            Log = LogManager.GetLogger(Name);
#if VERBOSE
            Log.effectivenessLevel = Level.Verbose;
#elif DEBUG
            Log.effectivenessLevel = Level.Debug;
#endif

            Log.Info("Loading.");
        }

        public void OnCreateWorld(UpdateSystem updateSystem)
        {
            Log.Info("Add system to world.");
            UnityEngine.Debug.Log("Add system to world.");
            updateSystem.UpdateAt<RealisticDensitySystem>(SystemUpdatePhase.GameSimulation);
        }

        public void OnDispose()
        {
            Log.Info("Mod disposed.");
            Instance = null;
        }

        public static void DebugLog(string message)
        {
            UnityEngine.Debug.Log($"[{Mod.Name} | v{Mod.Version}] {message}");
        }
    }
}
