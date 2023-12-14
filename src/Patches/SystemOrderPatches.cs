using Game;
using Game.Common;
using HarmonyLib;
using WorkforceRealismEnhancement.Systems;

namespace WorkforceRealismEnhancement.Patches
{
    [HarmonyPatch(typeof(SystemOrder))]
    public static class SystemOrder_Patches
    {
        [HarmonyPatch(nameof(SystemOrder.Initialize))]
        [HarmonyPostfix]
        public static void Postfix(UpdateSystem updateSystem)
        {
            updateSystem?.UpdateAt<WorkforceRealistmEnhancementSystem>(SystemUpdatePhase.ModificationEnd);
        }
    }
}
