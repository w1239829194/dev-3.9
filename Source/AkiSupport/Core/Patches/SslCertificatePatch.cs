using StayInTarkov;
using System.Linq;
using System.Reflection;
using UnityEngine.Networking;

namespace Spt.Core.Patches
{
    /// <summary>
    /// Created by: SPT-Spt
    /// Link: https://dev.sp-tarkov.com/SPT-AKI/Modules/src/branch/master/project/Spt.Core/Patches/SslCertificatePatch.cs
    /// Modified by: Paulov
    /// </summary>
    public class SslCertificatePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return StayInTarkovHelperConstants.EftTypes.Single(x => x.BaseType == typeof(CertificateHandler))
                .GetMethod("ValidateCertificate", StayInTarkovHelperConstants.PublicDeclaredFlags);
        }

        [PatchPrefix]
        private static bool PatchPrefix(ref bool __result)
        {
            __result = true;
            return false; // Skip origial
        }
    }
}
