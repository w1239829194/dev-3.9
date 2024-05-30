﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace Spt.Custom.Airdrops.Models
{
    /// <summary>
    /// Created by: SPT-Spt team
    /// Link: https://dev.sp-tarkov.com/SPT-AKI/Modules/src/branch/master/project/Spt.Custom/Airdrops/Models
    /// </summary>
    public class AirdropLootResultModel
    {
        [JsonProperty("dropType")]
        public string DropType { get; set; }

        [JsonProperty("loot")]
        public IEnumerable<AirdropLootModel> Loot { get; set; }
    }


    public class AirdropLootModel
    {
        [JsonProperty("tpl")]
        public string Tpl { get; set; }

        [JsonProperty("isPreset")]
        public bool IsPreset { get; set; }

        [JsonProperty("stackCount")]
        public int StackCount { get; set; }

        [JsonProperty("id")]
        public string ID { get; set; }
    }
}