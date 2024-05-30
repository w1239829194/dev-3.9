﻿using BepInEx;
using BepInEx.Bootstrap;
using Comfort.Common;
using EFT;
using StayInTarkov.SptSupport.Singleplayer.Models.Healing;
using StayInTarkov.Coop.Components.CoopGameComponents;
using StayInTarkov.Health;
using StayInTarkov.Networking;
using System;
using System.Linq;
using System.Reflection;

namespace StayInTarkov.UI
{
    /// <summary>
    /// Original by SPT-Spt team https://dev.sp-tarkov.com/SPT-AKI/Modules/src/branch/master/project/Spt.SinglePlayer/Patches/Progression/OfflineSaveProfilePatch.cs
    /// Modified by Paulov to suit SIT.
    /// Description: OfflineSaveProfile runs when the RAID ends and saves the Active Profile to the Backend via a custom web call.
    /// </summary>
    public class OfflineSaveProfile : ModulePatch
    {
        public static MethodInfo GetMethod()
        {
            foreach (var method in ReflectionHelpers.GetAllMethodsForType(typeof(TarkovApplication)))
            {
                if (method.Name.StartsWith("method") &&
                    method.GetParameters().Length >= 3 &&
                    method.GetParameters()[0].Name == "profileId" &&
                    method.GetParameters()[1].Name == "savageProfile" &&
                    method.GetParameters()[2].Name == "location" &&
                    method.GetParameters().Any(x => x.Name == "result") &&
                    method.GetParameters()[method.GetParameters().Length - 1].Name == "timeHasComeScreenController"
                    )
                {
                    //Logger.Log(BepInEx.Logging.LogLevel.Info, method.Name);
                    return method;
                }
            }
            Logger.Log(BepInEx.Logging.LogLevel.Error, "OfflineSaveProfile::Method is not found!");

            return null;
        }

        protected override MethodBase GetTargetMethod()
        {
            return GetMethod();
        }

        private static ISession _backEndSession;
        public static ISession BackEndSession
        {
            get
            {
                if (_backEndSession == null)
                {
                    _backEndSession = Singleton<ClientApplication<ISession>>.Instance.GetClientBackEndSession();
                }

                return _backEndSession;
            }
        }

        [PatchPrefix]
        public static bool PatchPrefix(string profileId, RaidSettings ____raidSettings, TarkovApplication __instance, Result<ExitStatus, TimeSpan, object> result)
        {
            if (StayInTarkovPlugin.Instance.IsSptSinglePlayerLoaded())
            {
                Logger.LogDebug("SIT: Detected Spt SP Module. Ignoring SIT's Save Profile Method.");
                return true;
            }

            Logger.LogInfo("SIT: Saving Profile...");

            // Get scav or pmc profile based on IsScav value
            var profile = ____raidSettings.IsScav
                ? BackEndSession.ProfileOfPet
                : BackEndSession.Profile;

            var currentHealth = HealthListener.Instance.CurrentHealth;

            // Set PMCs half health for heal screen
            if (!____raidSettings.IsScav)
            {
                HealthListener.HealHalfHealth(HealthListener.Instance.MyHealthController, currentHealth.Health, EBodyPart.Head);
                HealthListener.HealHalfHealth(HealthListener.Instance.MyHealthController, currentHealth.Health, EBodyPart.Chest);
                HealthListener.HealHalfHealth(HealthListener.Instance.MyHealthController, currentHealth.Health, EBodyPart.Stomach);
                HealthListener.HealHalfHealth(HealthListener.Instance.MyHealthController, currentHealth.Health, EBodyPart.LeftArm);
                HealthListener.HealHalfHealth(HealthListener.Instance.MyHealthController, currentHealth.Health, EBodyPart.RightArm);
                HealthListener.HealHalfHealth(HealthListener.Instance.MyHealthController, currentHealth.Health, EBodyPart.LeftLeg);
                HealthListener.HealHalfHealth(HealthListener.Instance.MyHealthController, currentHealth.Health, EBodyPart.RightLeg);
                currentHealth = HealthListener.Instance.CurrentHealth;
            }

            SaveProfileProgress(result.Value0, profile, currentHealth, ____raidSettings.IsScav);

            HealthListener.Instance.MyHealthController = null;
            return true;
        }

        public static void SaveProfileProgress(ExitStatus exitStatus, Profile profileData, PlayerHealth currentHealth, bool isPlayerScav)
        {
            // "Disconnecting" from your game in Single Player shouldn't result in losing your gear. This is stupid.
            if (exitStatus == ExitStatus.Left)
                exitStatus = ExitStatus.Runner;

            SaveProfileRequest request = new()
            {
                exit = exitStatus.ToString().ToLower(),
                profile = profileData,
                health = currentHealth,
                isPlayerScav = isPlayerScav
            };

            var convertedJson = request.SITToJson();
            SptBackendCommunication.Instance.PostJsonBLOCKING("/raid/profile/save", convertedJson);
        }

        public class SaveProfileRequest
        {
            public string exit { get; set; }
            public Profile profile { get; set; }
            public bool isPlayerScav { get; set; }
            public PlayerHealth health { get; set; }
        }
    }
}
