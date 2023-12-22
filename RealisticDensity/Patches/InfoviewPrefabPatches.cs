using Game.Prefabs;
using HarmonyLib;
using RealisticDensity.Systems;

namespace LegacyFlavour.Patches
{
    /// <summary>
    /// Replaces an infoview prefab icon and caches it for updates
    /// </summary>
    [HarmonyPatch(typeof(InfoviewPrefab), "Initialize")]
    class InfoviewPrefab_ConstructorPatch
    {
        static void Prefix(InfoviewPrefab __instance)
        {
            RealisticDensitySystem.EnsureModUIFolder();
        }
    }
}
