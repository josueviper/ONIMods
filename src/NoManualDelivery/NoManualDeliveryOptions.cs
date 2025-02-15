﻿using Newtonsoft.Json;
using SanchozzONIMods.Lib;
using PeterHan.PLib.Options;

namespace NoManualDelivery
{
    [JsonObject(MemberSerialization.OptIn)]
    [ConfigFile(IndentOutput: true)]
    [RestartRequired]
    internal sealed class NoManualDeliveryOptions : BaseOptions<NoManualDeliveryOptions>
    {
        [JsonProperty]
        [Option]
        public bool AllowAlwaysPickupEdible { get; set; } = true;

        [JsonProperty]
        [Option]
        public bool AllowTransferArmPickupGasLiquid { get; set; } = false;
    }
}
