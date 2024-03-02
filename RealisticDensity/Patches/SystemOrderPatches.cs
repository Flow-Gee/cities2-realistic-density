using Game;
using Game.Common;
using Game.Prefabs;
using HarmonyLib;
using RealisticDensity.Systems;

namespace RealisticDensity.Patches
{
    [HarmonyPatch(typeof(SystemOrder))]
    internal class SystemOrderPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SystemOrder), nameof(SystemOrder.Initialize))]
        public static void GetSystemOrder(UpdateSystem updateSystem)
        {
            updateSystem?.UpdateAt<RealisticDensitySystem>(SystemUpdatePhase.ModificationEnd);
        }
    }
}
