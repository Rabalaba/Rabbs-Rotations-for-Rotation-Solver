using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;

namespace RabbsRotations.Healer;

[Rotation("Rabbs WHM PVP", CombatType.PvP, GameVersion = "6.58")]
public sealed class WHM_PVP :WhiteMageRotation
{
    public static IBaseAction AfflatusPurgationpPvP { get; } = new BaseAction((ActionID)29230);


    protected override bool GeneralGCD(out IAction? act)
    {
        var ShouldCure2 = AllianceMembers.Any(p => p.IsDying() && p.DistanceToPlayer() < 30);
        var targetable = HostileTarget != null && HostileTarget.ObjectKind is ObjectKind.Player && !HostileTarget.HasStatus(true, StatusID.Guard, StatusID.HallowedGround_1302, StatusID.UndeadRedemption);

        if (AfflatusPurgationpPvP.CanUse(out act, skipAoeCheck:true) && targetable && LimitBreakLevel > 0) return true;
        if (CureIiiPvP.CanUse(out act, skipAoeCheck:true)) return true;
        if (ShouldCure2)
            if (CureIiPvP.CanUse(out act)) return true;
        if (AfflatusMiseryPvP.CanUse(out act, skipAoeCheck:true) && targetable) return true;
        if (GlareIiiPvP.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }

    private bool UseLily(out IAction? act)
    {
        if (AfflatusRapturePvE.CanUse(out act, skipAoeCheck: true)) return true;
        if (AfflatusSolacePvE.CanUse(out act)) return true;
        return false;
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        var ShouldAquaveil = PartyMembers.Any(p => p.IsDying() && p.DistanceToPlayer() < 30);
        var targetable = HostileTarget != null && HostileTarget.ObjectKind is ObjectKind.Player && !HostileTarget.HasStatus(true, StatusID.Guard, StatusID.HallowedGround_1302, StatusID.UndeadRedemption);

        if (ShouldAquaveil)
            if (AquaveilPvP.CanUse(out act)) return true;
        if (MiracleOfNaturePvP.CanUse(out act) && targetable) return true;
        if (SeraphStrikePvP.CanUse(out act, skipAoeCheck:true) && targetable) return true;


        return base.AttackAbility(nextGCD, out act);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {

        return base.EmergencyAbility(nextGCD, out act);
    }


    protected override bool HealSingleGCD(out IAction? act)
    {
        if (CureIiPvP.CanUse(out act)) return true;

        return base.HealSingleGCD(out act);
    }


    protected override bool HealSingleAbility(IAction nextGCD, out IAction? act)
    {
        if (AquaveilPvP.CanUse(out act)) return true;
        return base.HealSingleAbility(nextGCD, out act);
    }


    protected override bool HealAreaGCD(out IAction? act)
    {

        if (CureIiiPvP.CanUse(out act, skipAoeCheck: true)) return true;
        return base.HealAreaGCD(out act);
    }


    protected override bool HealAreaAbility(IAction nextGCD, out IAction? act)
    {
        if (AquaveilPvP.CanUse(out act)) return true;
        return base.HealAreaAbility(nextGCD, out act);
    }


    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
        if (AquaveilPvP.CanUse(out act)) return true;
        return base.DefenseSingleAbility(nextGCD, out act);
    }


    protected override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
        if (AquaveilPvP.CanUse(out act)) return true;
        return base.DefenseAreaAbility(nextGCD, out act);
    }

    protected override IAction? CountDownAction(float remainTime)
    {

        return base.CountDownAction(remainTime);
    }


}
