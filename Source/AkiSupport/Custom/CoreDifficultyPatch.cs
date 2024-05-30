using StayInTarkov.Networking;
using System.Linq;
using System.Reflection;

namespace StayInTarkov.SptSupport.Custom
{
    /// <summary>
    /// Created by: SPT-Spt
    /// Link: https://dev.sp-tarkov.com/SPT-AKI/Modules/src/branch/master/project/Spt.Custom/Patches/CoreDifficultyPatch.cs
    /// </summary>
    public class CoreDifficultyPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            var methodName = "LoadCoreByString";
            var flags = BindingFlags.Public | BindingFlags.Static;

            return StayInTarkovHelperConstants.EftTypes.Single(x => x.GetMethod(methodName, flags) != null)
                .GetMethod(methodName, flags);
        }

        [PatchPrefix]
        private static bool PatchPrefix(ref string __result)
        {
            __result = SptBackendCommunication.Instance.GetJsonBLOCKING("/singleplayer/settings/bot/difficulty/core/core");
            return string.IsNullOrWhiteSpace(__result);
        }
    }
}
