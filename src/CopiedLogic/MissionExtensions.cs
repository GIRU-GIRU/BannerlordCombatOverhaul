using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace GCO.CopiedLogic
{
    internal static class MissionExtensions
    {
        internal static float ComputeRelativeSpeedDiffOfAgents(this Mission _, Agent agentA, Agent agentB)
        {
            Vec3 v = Vec3.Zero;
            if (agentA.MountAgent != null)
            {
                v = agentA.MountAgent.MovementVelocity.y * agentA.MountAgent.GetMovementDirection();
            }
            else
            {
                v.AsVec2 = agentA.MovementVelocity;
                v.RotateAboutZ(agentA.MovementDirectionAsAngle);
            }
            Vec3 v2 = Vec3.Zero;
            if (agentB.MountAgent != null)
            {
                v2 = agentB.MountAgent.MovementVelocity.y * agentB.MountAgent.GetMovementDirection();
            }
            else
            {
                v2.AsVec2 = agentB.MovementVelocity;
                v2.RotateAboutZ(agentB.MovementDirectionAsAngle);
            }
            return (v - v2).Length;

        }

        internal static float SpeedGraphFunction(this Mission _, float progress, StrikeType strikeType, Agent.UsageDirection attackDir)
        {
            bool isThrust = strikeType == StrikeType.Thrust;
            bool isAttackUp = attackDir == Agent.UsageDirection.AttackUp;
            ManagedParametersEnum managedParameterEnum;
            ManagedParametersEnum managedParameterEnum2;
            ManagedParametersEnum managedParameterEnum3;
            ManagedParametersEnum managedParameterEnum4;
            if (isThrust)
            {
                managedParameterEnum = ManagedParametersEnum.ThrustCombatSpeedGraphZeroProgressValue;
                managedParameterEnum2 = ManagedParametersEnum.ThrustCombatSpeedGraphFirstMaximumPoint;
                managedParameterEnum3 = ManagedParametersEnum.ThrustCombatSpeedGraphSecondMaximumPoint;
                managedParameterEnum4 = ManagedParametersEnum.ThrustCombatSpeedGraphOneProgressValue;
            }
            else if (isAttackUp)
            {
                managedParameterEnum = ManagedParametersEnum.OverSwingCombatSpeedGraphZeroProgressValue;
                managedParameterEnum2 = ManagedParametersEnum.OverSwingCombatSpeedGraphFirstMaximumPoint;
                managedParameterEnum3 = ManagedParametersEnum.OverSwingCombatSpeedGraphSecondMaximumPoint;
                managedParameterEnum4 = ManagedParametersEnum.OverSwingCombatSpeedGraphOneProgressValue;
            }
            else
            {
                managedParameterEnum = ManagedParametersEnum.SwingCombatSpeedGraphZeroProgressValue;
                managedParameterEnum2 = ManagedParametersEnum.SwingCombatSpeedGraphFirstMaximumPoint;
                managedParameterEnum3 = ManagedParametersEnum.SwingCombatSpeedGraphSecondMaximumPoint;
                managedParameterEnum4 = ManagedParametersEnum.SwingCombatSpeedGraphOneProgressValue;
            }
            float managedParameter = ManagedParameters.Instance.GetManagedParameter(managedParameterEnum);
            float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(managedParameterEnum2);
            float managedParameter3 = ManagedParameters.Instance.GetManagedParameter(managedParameterEnum3);
            float managedParameter4 = ManagedParameters.Instance.GetManagedParameter(managedParameterEnum4);
            float result;
            if (progress < managedParameter2)
            {
                result = (1f - managedParameter) / managedParameter2 * progress + managedParameter;
            }
            else if (managedParameter3 < progress)
            {
                result = (managedParameter4 - 1f) / (1f - managedParameter3) * (progress - managedParameter3) + 1f;
            }
            else
            {
                result = 1f;
            }
            return result;
        }
    }
}
