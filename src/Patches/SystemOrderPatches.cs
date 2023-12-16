using Game;
using Game.Common;
using HarmonyLib;
using RealisticDensity.Systems;

namespace RealisticDensity.Patches
{
    [HarmonyPatch(typeof(SystemOrder))]
    public static class SystemOrder_Patches
    {
        [HarmonyPatch(nameof(SystemOrder.Initialize))]
        [HarmonyPostfix]
        public static void Postfix(UpdateSystem updateSystem)
        {
            updateSystem?.UpdateAt<RealisticDensitySystem>(SystemUpdatePhase.ModificationEnd);
        }
    }
}
