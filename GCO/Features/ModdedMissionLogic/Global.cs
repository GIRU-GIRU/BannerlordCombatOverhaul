using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace GCO.Features.ModdedMissionLogic
{
    public static class Global
    {

        private static MissionTime _playerAgentHyperarmorActiveTime;

        public static void ApplyHyperArmor()
        {
            _playerAgentHyperarmorActiveTime = MissionTime.SecondsFromNow(1f);
        }

        public static bool IsHyperArmorActive()
        {
            bool hyperArmorActive = !_playerAgentHyperarmorActiveTime.IsPast;

            return hyperArmorActive;
        }
    }
}
