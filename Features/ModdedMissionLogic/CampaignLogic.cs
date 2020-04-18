using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace GCO.Features.ModdedMissionLogic
{
    public class CampaignLogic : CampaignBehaviorBase
    {

        //required to prevent crashing
        public override void RegisterEvents()
        {
            CampaignEvents.OnMissionStartedEvent.AddNonSerializedListener(this, new Action<IMission>(this.FindBattle));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        public void FindBattle(IMission misson)
        {
            Mission mission = (Mission)misson;
            bool isCombat = mission.CombatType == Mission.MissionCombatType.Combat;

            if (Config.ConfigSettings.HPOnKillEnabled)
            {
                if (Mission.Current.Scene != null)
                {
                    if (mission.IsFieldBattle && isCombat)
                    {
                        Mission.Current.AddMissionBehaviour(new HealthOnKill());
                    }
                }
            }
        }
    }
}
