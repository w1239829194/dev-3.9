﻿using Spt.Custom.Airdrops.Models;
using Comfort.Common;
using EFT;
using EFT.Airdrop;
using Newtonsoft.Json;
using StayInTarkov.SptSupport.Airdrops.Models;
using StayInTarkov.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StayInTarkov.SptSupport.Airdrops.Utils
{
    public static class AirdropUtil
    {
        public static async Task<AirdropConfigModel> GetConfigFromServer()
        {
            string json = await SptBackendCommunication.Instance.GetJsonAsync("/singleplayer/airdrop/config");
            return JsonConvert.DeserializeObject<AirdropConfigModel>(json);
        }

        public static int ChanceToSpawn(GameWorld gameWorld, AirdropConfigModel config, bool isFlare)
        {
            // Flare summoned airdrops are guaranteed
            if (isFlare)
            {
                return 100;
            }

            string location = gameWorld.AllAlivePlayersList[0].Location;

            int result = 25;
            switch (location.ToLower())
            {
                case "bigmap":
                    {
                        result = config.AirdropChancePercent.Bigmap;
                        break;
                    }
                case "interchange":
                    {
                        result = config.AirdropChancePercent.Interchange;
                        break;
                    }
                case "rezervbase":
                    {
                        result = config.AirdropChancePercent.Reserve;
                        break;
                    }
                case "shoreline":
                    {
                        result = config.AirdropChancePercent.Shoreline;
                        break;
                    }
                case "woods":
                    {
                        result = config.AirdropChancePercent.Woods;
                        break;
                    }
                case "lighthouse":
                    {
                        result = config.AirdropChancePercent.Lighthouse;
                        break;
                    }
                case "tarkovstreets":
                    {
                        result = config.AirdropChancePercent.TarkovStreets;
                        break;
                    }
            }

            return result;
        }

        public static bool ShouldAirdropOccur(int dropChance, List<AirdropPoint> airdropPoints)
        {
            return airdropPoints.Count > 0 && Random.Range(0, 100) <= dropChance;
        }

        public static async Task<AirdropParametersModel> InitAirdropParams(GameWorld gameWorld, bool isFlare)
        {
            var serverConfig = await GetConfigFromServer();
            if (serverConfig == null)
                return new AirdropParametersModel() { Config = serverConfig, AirdropAvailable = false };

            if (serverConfig.AirdropChancePercent == null)
                return new AirdropParametersModel() { Config = serverConfig, AirdropAvailable = false };

            var allAirdropPoints = LocationScene.GetAll<AirdropPoint>().ToList();
            if (gameWorld == null)
                return new AirdropParametersModel() { Config = serverConfig, AirdropAvailable = false };

            if (gameWorld.RegisteredPlayers == null || !gameWorld.RegisteredPlayers.Any())
                return new AirdropParametersModel() { Config = serverConfig, AirdropAvailable = false };

            var playerPosition = gameWorld.RegisteredPlayers[0].Position;
            var flareAirdropPoints = new List<AirdropPoint>();
            var dropChance = ChanceToSpawn(gameWorld, serverConfig, isFlare);

            if (isFlare && allAirdropPoints.Count > 0)
            {
                foreach (AirdropPoint point in allAirdropPoints)
                {
                    if (Vector3.Distance(playerPosition, point.transform.position) <= 100f)
                    {
                        flareAirdropPoints.Add(point);
                    }
                }
            }

            if (flareAirdropPoints.Count == 0 && isFlare)
            {
                Debug.LogError($"[AKI-AIRDROPS]: Airdrop called in by flare, Unable to find an airdropPoint within 100m, defaulting to normal drop");
                flareAirdropPoints.Add(allAirdropPoints.OrderBy(_ => Guid.NewGuid()).FirstOrDefault());
            }

            return new AirdropParametersModel()
            {
                Config = serverConfig,
                AirdropAvailable = ShouldAirdropOccur(dropChance, allAirdropPoints),

                DistanceTraveled = 0f,
                DistanceToTravel = 8000f,
                Timer = 0,
                PlaneSpawned = false,
                BoxSpawned = false,

                DropHeight = Random.Range(serverConfig.PlaneMinFlyHeight, serverConfig.PlaneMaxFlyHeight),
                TimeToStart = isFlare
                    ? 5
                    : Random.Range(serverConfig.AirdropMinStartTimeSeconds, serverConfig.AirdropMaxStartTimeSeconds),

                RandomAirdropPoint = isFlare && allAirdropPoints.Count > 0
                    ? flareAirdropPoints.OrderBy(_ => Guid.NewGuid()).First().transform.position
                    : allAirdropPoints.OrderBy(_ => Guid.NewGuid()).First().transform.position,

                PlaneSpawnPoint = default(Vector3),
                PlaneLookAt = default(Vector3)
            };
        }
    }
}
