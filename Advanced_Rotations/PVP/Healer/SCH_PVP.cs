using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using RotationSolver.Basic.Helpers;
namespace RabbsRotations.Healer;

[Rotation("Rabbs SCH PVP", CombatType.PvP, GameVersion = "6.58")]
[Api(1)]
public sealed class SCH_PVP : ScholarRotation
{

    public static BaseAction DeployThis { get; } = new BaseAction((ActionID)29234);

    public static void SetDeployThis()
    {
        DeployThis.Setting.TargetType = TargetType.BeAttacked; // Assuming appropriate access
        DeployThis.Setting.TargetStatusNeed = [StatusID.Biolysis_3089];
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {


        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool GeneralGCD(out IAction? act)
    {
        var targetable = HostileTarget != null && HostileTarget.ObjectKind is ObjectKind.Player && !HostileTarget.HasStatus(true, StatusID.Guard, StatusID.HallowedGround_1302, StatusID.UndeadRedemption);
        var ShouldAdlo = AllianceMembers.Any(p => (p.CurrentHp / p.MaxHp < 0.75) && p.DistanceToPlayer() < 30);

        if (ShouldAdlo)
            if (AdloquiumPvP.CanUse(out act, usedUp:true) && AdloquiumPvP.Target.Target != null && !AdloquiumPvP.Target.Target.HasStatus(true, StatusID.Galvanize_3087)) return true;

        if (targetable && DeploymentTacticsPvP.Cooldown.HasOneCharge && (Player.HasStatus(true, StatusID.Recitation_3094) || ExpedientPvP.Cooldown.RecastTimeRemainOneCharge > 10))
        {
            if (BiolysisPvP.CanUse(out act)) return true;
        }

        if(BroilIvPvP.CanUse(out act)) return true;

       
        return base.GeneralGCD(out act);
    }


    protected override bool HealSingleGCD(out IAction? act)
    {


        return base.HealSingleGCD(out act);
    }

    protected override bool HealSingleAbility(IAction nextGCD, out IAction? act)
    {


        return base.HealSingleAbility(nextGCD, out act);
    }


    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? act)
    {

        return base.DefenseSingleAbility(nextGCD, out act);
    }


    protected override bool HealAreaGCD(out IAction? act)
    {


        return base.HealAreaGCD(out act);
    }



    protected override bool HealAreaAbility(IAction nextGCD, out IAction? act)
    {


        return base.HealAreaAbility(nextGCD, out act);
    }


    protected override bool DefenseAreaGCD(out IAction? act)
    {

        return base.DefenseAreaGCD(out act);
    }

    protected override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
    {

        return base.DefenseAreaAbility(nextGCD, out act);
    }


    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        var targetable = HostileTarget != null && HostileTarget.ObjectKind is ObjectKind.Player && !HostileTarget.HasStatus(true, StatusID.Guard, StatusID.HallowedGround_1302, StatusID.UndeadRedemption);
        var CanSpread = AllHostileTargets.Any(p => !p.IsDying() && p.DistanceToPlayer() < 30);
        DeployThis.Target = BroilIvPvP.Target;
        if (targetable)
        {
            if (!BiolysisPvP.Cooldown.IsCoolingDown && !Player.HasStatus(true, StatusID.Recitation_3094))
            {
                if (ExpedientPvP.CanUse(out act, skipAoeCheck:true)) return true;
            }
            if (DeploymentTacticsPvP.CanUse(out act, skipClippingCheck:true, skipAoeCheck:true) && DeploymentTacticsPvP.Target.Target != null && DeploymentTacticsPvP.Target.Target.HasStatus(true,StatusID.Biolysis_3089)) return true;
            if (MummificationPvP.CanUse(out act, skipAoeCheck:true)) return true;
        }


        return base.AttackAbility(nextGCD, out act);
    }

    protected override IAction? CountDownAction(float remainTime)
    {

        return base.CountDownAction(remainTime);
    }
}