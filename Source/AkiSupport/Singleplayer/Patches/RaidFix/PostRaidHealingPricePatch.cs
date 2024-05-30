using EFT;
using HarmonyLib;
using StayInTarkov;
using System;
using System.Reflection;

namespace StayInTarkov.SptSupport.Singleplayer.Patches.RaidFix
{

    /// <summary>
    /// Credit: SPT-Spt team
    /// Link: https://dev.sp-tarkov.com/SPT-AKI/Modules/src/branch/master/project/Spt.SinglePlayer/Patches/RaidFix/PostRaidHealingPricePatch.cs
    /// </summary>
    public class PostRaidHealingPricePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Profile.TraderInfo), nameof(Profile.TraderInfo.UpdateLevel));
        }

        [PatchPrefix]
        protected static void PatchPrefix(Profile.TraderInfo __instance)
        {
            if (__instance.Settings == null)
            {
                return;
            }

            var loyaltyLevel = __instance.Settings.GetLoyaltyLevel(__instance);
            var loyaltyLevelSettings = __instance.Settings.GetLoyaltyLevelSettings(loyaltyLevel);

            if (loyaltyLevelSettings == null)
            {
                throw new IndexOutOfRangeException($"Loyalty level {loyaltyLevel} not found.");
            }

            // Backing field of the "CurrentLoyalty" property
            // Traverse.Create(__instance).Field("<CurrentLoyalty>k__BackingField").SetValue(loyaltyLevelSettings.Value);
            __instance.CurrentLoyalty = loyaltyLevelSettings.Value;
        }
    }
}