using BepInEx;
using HarmonyLib;
using System.Reflection;
using System.Linq;

#if BEPINEX_V6
    using BepInEx.Unity.Mono;
#endif

namespace RealisticDensity
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private Mod Mod;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called by BepInEx.")]
        private void Awake()
        {
            Mod = new Mod();
            Mod.OnLoad();

            Mod.Log.Info("Loading Harmony patches.");

            var harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID + "_Cities2Harmony");
            var patchedMethods = harmony.GetPatchedMethods().ToArray();

            Mod.Log.Info($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded! Patched methods " + patchedMethods.Length);
            foreach (var patchedMethod in patchedMethods)
            {
                Mod.Log.Info($"Patched method: {patchedMethod.Module.Name}:{patchedMethod.Name}");
            }
        }
    }
}