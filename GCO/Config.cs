﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GCO.Features.ModdedMissionLogic;
using Newtonsoft.Json;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace GCO
{
    public class ConfigSettings
    {
        [JsonProperty("HPOnKillEnabled")]
        public bool HPOnKillEnabled { get; set; }

        [JsonProperty("CleaveEnabled")]
        public bool CleaveEnabled { get; set; }

        [JsonProperty("SimplifiedBanditLogicEnabled")]
        public bool SimplifiedBanditLogicEnabled { get; set; }

        [JsonProperty("HyperArmorEnabled")]
        public bool HyperArmorEnabled { get; set; }

    }

    public static class Config
    {
        public static ConfigSettings ConfigSettings { get; set; }

        private static readonly string ConfigFilePath =
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "GCOconfig.json");

        private static readonly bool configExists = File.Exists(ConfigFilePath);

        public static bool ConfigLoadedSuccessfully { get; set; }

        public static bool xorbarexCleaveExists { get; set; }

        public static void initConfig()
        {
            if (configExists)
            {
                try
                {
                    ConfigSettings = JsonConvert.DeserializeObject<ConfigSettings>(File.ReadAllText(ConfigFilePath));
                    ConfigLoadedSuccessfully = true;
                    return;
                }
                catch
                {
      
                }
            }

            ConfigLoadedSuccessfully = false;
            ConfigSettings = new ConfigSettings();

            ConfigSettings.CleaveEnabled = true;
            ConfigSettings.HPOnKillEnabled = true;
            ConfigSettings.HyperArmorEnabled = true;
            ConfigSettings.SimplifiedBanditLogicEnabled = true;

        }
    }
}
