﻿using Comfort.Common;
using EFT;
using HarmonyLib;
using System.Linq;
using System.Reflection;

namespace StayInTarkov.SptSupport.Singleplayer.Patches.RaidFix
{
    /// <summary>
    /// Credit: SPT-Spt team
    /// Link: https://dev.sp-tarkov.com/SPT-AKI/Modules/src/branch/master/project/Spt.SinglePlayer/Patches/RaidFix/LabsKeycardRemovalPatch.cs
    /// </summary>
    public class LabsKeycardRemovalPatch : ModulePatch
    {
        private const string LabsAccessCardTemplateId = "5c94bbff86f7747ee735c08f";

        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));
        }

        [PatchPostfix]
        private static void PatchPostfix()
        {
            var gameWorld = Singleton<GameWorld>.Instance;
            var player = gameWorld?.MainPlayer;

            if (gameWorld == null || player == null)
            {
                return;
            }

            if (gameWorld.MainPlayer.Location.ToLower() != "laboratory")
            {
                return;
            }

            var accessCardItem = player.Profile.Inventory.AllRealPlayerItems.FirstOrDefault(x => x.TemplateId == LabsAccessCardTemplateId);

            if (accessCardItem == null)
            {
                return;
            }

            var inventoryController = Traverse.Create(player).Field<InventoryControllerClass>("_inventoryController").Value;
            InteractionsHandlerClass.Remove(accessCardItem, inventoryController, false, true);
        }
    }
}