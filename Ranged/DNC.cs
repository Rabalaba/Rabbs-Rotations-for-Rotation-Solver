using Dalamud.Game.ClientState.Objects.Types;
using RotationSolver.Basic.Rotations;
using System;

namespace RabbsRotations.Ranged;

[SourceCode(Path = "main/DefaultRotations/Ranged/DNC_Default.cs")]
public sealed class DNC_Default : DNC_Base
{
    public override CombatType Type => CombatType.PvP;

    public override string GameVersion => "6.55";

    public override string RotationName => "PVP Dancer";

    private static BaseAction PVPStarFallDance { get; } = new BaseAction(ActionID.PvP_Starfalldance);

    private static BaseAction PVPHoningDance { get; } = new BaseAction(ActionID.PvP_Honingdance);

    private static BaseAction PVPSaberdance { get; } = new BaseAction(ActionID.PvP_Saberdance);

    private static BaseAction PVPFanDance { get; } = new BaseAction(ActionID.PvP_Fandance);

    private static BaseAction PVPCuringWaltz { get; } = new BaseAction(ActionID.PvP_Curingwaltz);

    private static BaseAction PVPEnAvant { get; } = new BaseAction(ActionID.PvP_Enavant)
    {
        ActionCheck = (BattleChara b, bool m) => !CustomRotation.Player.HasStatus(false, StatusID.PvP_FlourishingSaberDance, StatusID.PvP_EnAvant)
    };

    private static BaseAction PVPContradance { get; } = new BaseAction(ActionID.PvP_Contradance)
    { ActionCheck = (t, m) => RotationSolver.Basic.Rotations.CustomRotation.LimitBreakLevel >= 1 };

    private static BaseAction PVPReverseCascade { get; } = new BaseAction(ActionID.PvP_Reversecascade);

    private static BaseAction PVPFountainfall { get; } = new BaseAction(ActionID.PvP_Fountainfall);

    private static BaseAction PVPCascade { get; } = new BaseAction(ActionID.PvP_Cascade);

    private static BaseAction PVPFountain{ get; } = new BaseAction(ActionID.PvP_Fountain);




    protected override IAction CountDownAction(float remainTime)
    {
        //if(remainTime <= CountDownAhead)
        //{
        //    if(DanceFinishGCD(out))
        //}
        if (remainTime <= 15)
        {
            if (StandardStep.CanUse(out var act, CanUseOption.MustUse)) return act;
            if (ExecuteStepGCD(out act)) return act;
        }
        return base.CountDownAction(remainTime);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        if (IsDancing)
        {
            return base.EmergencyAbility(nextGCD, out act);
        }

        if (TechnicalStep.ElapsedAfter(115)
            && UseBurstMedicine(out act)) return true;

        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(out IAction act)
    {
        #region pvp

        if (Player.MaxHp - Player.CurrentHp >= 10000)
        {
            if (PVPCuringWaltz.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
        }

        if (PVPEnAvant.CanUse(out act, CanUseOption.MustUseEmpty)) return true;

        if (PVPFanDance.CanUse(out act, CanUseOption.MustUseEmpty)) return true;

        

        #endregion
        act = null;
        if (IsDancing) return false;

        if (Devilment.CanUse(out act))
        {
            if (IsBurst && !TechnicalStep.EnoughLevel) return true;

            if (Player.HasStatus(true, StatusID.TechnicalFinish)) return true;
        }

        if (UseClosedPosition(out act)) return true;

        if (Flourish.CanUse(out act)) return true;
        if (FanDance3.CanUse(out act, CanUseOption.MustUse)) return true;

        if (Player.HasStatus(true, StatusID.Devilment) || Feathers > 3 || !TechnicalStep.EnoughLevel)
        {
            if (FanDance2.CanUse(out act)) return true;
            if (FanDance.CanUse(out act)) return true;
        }

        if (FanDance4.CanUse(out act, CanUseOption.MustUse))
        {
            if (TechnicalStep.EnoughLevel && TechnicalStep.IsCoolingDown && TechnicalStep.WillHaveOneChargeGCD()) return false;
            return true;
        }

        return base.AttackAbility(out act);
    }

    protected override bool GeneralGCD(out IAction act)
    {
        #region pvp
        /*
        bool starfallDanceReady = !PVPStarFallDance.IsCoolingDown;
        bool starfallDance = Player.HasStatus(true, StatusID.PvP_StarfallDance);
        bool curingWaltzReady = !GetCooldown(CuringWaltz).IsCooldown;
        bool honingDanceReady = !GetCooldown(HoningDance).IsCooldown;
        var acclaimStacks = GetBuffStacks(Buffs.Acclaim);
        bool canWeave = CanWeave(Cascade);
        var distance = GetTargetDistance();
        var HPThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCPvP_WaltzThreshold);
        var HP = PlayerHealthPercentageHp();
        var orderbyresults5y = from o in Dalamud.Game.ClientState.Objects.ObjectTable.Where(o => o.ObjectKind is Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Player or Dalamud.Game.ClientState.Objects.Enums.ObjectKind.BattleNpc && ((BattleChara)o).YalmDistanceX <= 5 && ((BattleChara)o).CurrentHp > 0 && PartyTargetingService.CanUseActionOnGameObject(Cascade, (GameObject2*)o.Address))
                               orderby ((BattleChara)o).MaxHp - ((BattleChara)o).CurrentHp descending
                               select o;
        bool InMeleeRange2 = orderbyresults5y.FirstOrDefault() != null;
        var orderbyresults13y = from o in Service.ObjectTable.Where(o => o.ObjectKind is Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Player or Dalamud.Game.ClientState.Objects.Enums.ObjectKind.BattleNpc && ((BattleChara)o).YalmDistanceX <= 13 && ((BattleChara)o).CurrentHp > 0 && PartyTargetingService.CanUseActionOnGameObject(Cascade, (GameObject2*)o.Address))
                                orderby ((BattleChara)o).MaxHp - ((BattleChara)o).CurrentHp descending
                                select o;
        bool InMeleeRange3 = orderbyresults13y.FirstOrDefault() != null;
        var orderbyresults20y = from o in Service.ObjectTable.Where(o => o.ObjectKind is Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Player or Dalamud.Game.ClientState.Objects.Enums.ObjectKind.BattleNpc && ((BattleChara)o).YalmDistanceX <= 20 && ((BattleChara)o).CurrentHp > 0 && PartyTargetingService.CanUseActionOnGameObject(Cascade, (GameObject2*)o.Address))
                                orderby ((BattleChara)o).MaxHp - ((BattleChara)o).CurrentHp descending
                                select o;
        bool InMeleeRange4 = orderbyresults20y.FirstOrDefault() != null;

        // Honing Dance Option
        if (honingDanceReady && InMeleeRange2)
        {
            if (HasEffect(Buffs.Acclaim) && acclaimStacks < 4)
                return WHM.Assize;

            return HoningDance;
        }
        */
        if (PVPHoningDance.CanUse(out act, CanUseOption.MustUse)) return true;

        if (PVPStarFallDance.CanUse(out act, CanUseOption.MustUse)) return true;


        if (PVPFountain.CanUse(out act, CanUseOption.MustUse)) return true;
        if (PVPCascade.CanUse(out act, CanUseOption.MustUse)) return true;

        #endregion
        if (!InCombat && !Player.HasStatus(true, StatusID.ClosedPosition1) && ClosedPosition.CanUse(out act)) return true;

        //if (DanceFinishGCD(out act)) return true;
        if (ExecuteStepGCD(out act)) return true;

        if (IsBurst && InCombat && TechnicalStep.CanUse(out act, CanUseOption.MustUse)) return true;

        if (AttackGCD(out act, Player.HasStatus(true, StatusID.Devilment))) return true;

        return base.GeneralGCD(out act);
    }

    private static bool AttackGCD(out IAction act, bool breaking)
    {
        act = null;
        if (IsDancing) return false;

        if ((breaking || Esprit >= 85) && SaberDance.CanUse(out act, CanUseOption.MustUse)) return true;

        if (StarFallDance.CanUse(out act, CanUseOption.MustUse)) return true;

        if (Tillana.CanUse(out act, CanUseOption.MustUse)) return true;

        if (UseStandardStep(out act)) return true;

        if (BloodShower.CanUse(out act)) return true;
        if (FountainFall.CanUse(out act)) return true;

        if (RisingWindmill.CanUse(out act)) return true;
        if (ReverseCascade.CanUse(out act)) return true;

        if (BladeShower.CanUse(out act)) return true;
        if (Windmill.CanUse(out act)) return true;

        if (Fountain.CanUse(out act)) return true;
        if (Cascade.CanUse(out act)) return true;

        return false;
    }

    private static bool UseStandardStep(out IAction act)
    {
        if (!StandardStep.CanUse(out act, CanUseOption.MustUse)) return false;
        if (Player.WillStatusEndGCD(2, 0, true, StatusID.StandardFinish)) return true;

        if (!HasHostilesInRange) return false;

        if (TechnicalStep.EnoughLevel && (Player.HasStatus(true, StatusID.TechnicalFinish) || TechnicalStep.IsCoolingDown && TechnicalStep.WillHaveOneCharge(5))) return false;

        return true;
    }

    private static bool UseClosedPosition(out IAction act)
    {
        if (!ClosedPosition.CanUse(out act)) return false;

        if (InCombat && Player.HasStatus(true, StatusID.ClosedPosition1))
        {
            foreach (var friend in PartyMembers)
            {
                if (friend.HasStatus(true, StatusID.ClosedPosition2))
                {
                    if (ClosedPosition.Target != friend) return true;
                    break;
                }
            }
        }
        return false;
    }
}
