using EFT;
using StayInTarkov.Networking;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StayInTarkov.SptSupport.Custom
{
    /// <summary>
    /// Created by: SPT-Spt
    /// Link: https://dev.sp-tarkov.com/SPT-AKI/Modules/src/branch/master/project/Spt.Custom/Patches/BotDifficultyPatch.cs
    /// </summary>
    public class BotDifficultyPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            var methodName = "LoadDifficultyStringInternal";
            var flags = BindingFlags.Public | BindingFlags.Static;

            return StayInTarkovHelperConstants.EftTypes.Single(x => x.GetMethod(methodName, flags) != null)
                .GetMethod(methodName, flags);
        }

        [PatchPrefix]
        private static bool PatchPrefix(ref string __result, BotDifficulty botDifficulty, WildSpawnType role)
        {
            __result = SptBackendCommunication.Instance.GetJsonBLOCKING($"/singleplayer/settings/bot/difficulty/{role}/{botDifficulty}");
            return string.IsNullOrWhiteSpace(__result);
        }
    }
}
