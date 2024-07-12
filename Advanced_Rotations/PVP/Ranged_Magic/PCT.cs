using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Data;
using System;
using static Lumina.Data.Parsing.Layer.LayerCommon;

namespace RabbsRotationsNET8.Magical;

[Rotation("HappyLittleAccidental PvP", CombatType.PvP, GameVersion = "7.0")]
[SourceCode(Path = "main/DefaultRotations/Magical/PCT_Default.cs")]
[Api(2)]
public sealed class PCT_Default_PvP : PictomancerRotation
{




    public override MedicineType MedicineType => MedicineType.Intelligence;
    #region Countdown logic
    // Defines logic for actions to take during the countdown before combat starts.
    protected override IAction? CountDownAction(float remainTime)
    {
        return base.CountDownAction(remainTime);
    }
    #endregion

    #region Emergency Logic
    // Determines emergency actions to take based on the next planned GCD action.
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
        if (!Player.HasStatus(true, StatusID.SubtractivePalette_4102) && !IsMoving)
        {
            if (SubtractivePalettePvP.CanUse(out act)) return true;
        }

        return base.EmergencyAbility(nextGCD, out act);
    }
    #endregion

    #region oGCD Logic
    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
       

        return base.AttackAbility(nextGCD, out act);
    }

    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
    {
        act = null;


        return base.MoveForwardAbility(nextGCD, out act);
    }
    #endregion

    #region GCD Logic
    protected override bool MoveForwardGCD(out IAction? act)
    {
        act = null;

        return base.MoveForwardGCD(out act);
    }

    protected override bool GeneralGCD(out IAction? act)
    {
        if (StarPrismPvP.CanUse(out act, skipAoeCheck:true) && Player.HasStatus(true, StatusID.Starstruck_4118)) return true;
        if (RetributionOfTheMadeenPvP.CanUse(out act, skipAoeCheck:true) && Player.HasStatus(true, StatusID.MadeenPortrait)) return true;
        if (MogOfTheAgesPvP.CanUse(out act, skipAoeCheck:true) && Player.HasStatus(true, StatusID.MooglePortrait)) return true;
        if (Player.HasStatus(true, StatusID.PomSketch) && !IsMoving)
        {
            if (PomMotifPvP.CanUse(out act)) return true;
        }
        if (Player.HasStatus(true,StatusID.PomMotif) && LivingMusePvP.Cooldown.HasOneCharge) 
        {
            if (PomMusePvP.CanUse(out act, skipAoeCheck: true, usedUp: true)) return true;
        }
        if (Player.HasStatus(true, StatusID.WingSketch) && !IsMoving)
        {
            if (WingMotifPvP.CanUse(out act)) return true;
        }
        if (Player.HasStatus(true, StatusID.WingMotif) && LivingMusePvP.Cooldown.HasOneCharge)
        {
            if (WingedMusePvP.CanUse(out act, skipAoeCheck: true, usedUp: true)) return true;
        }
        if (Player.HasStatus(true, StatusID.ClawSketch) && !IsMoving)
        {
            if (ClawMotifPvP.CanUse(out act)) return true;
        }
        if (Player.HasStatus(true, StatusID.ClawMotif) && LivingMusePvP.Cooldown.HasOneCharge)
        {
            if (ClawedMusePvP.CanUse(out act, skipAoeCheck: true, usedUp:true)) return true;
        }
        if (Player.HasStatus(true, StatusID.MawSketch) && !IsMoving)
        {
            if (MawMotifPvP.CanUse(out act)) return true;
        }
        if (Player.HasStatus(true, StatusID.MawMotif) && LivingMusePvP.Cooldown.HasOneCharge)
        {
            if (FangedMusePvP.CanUse(out act, skipAoeCheck: true, usedUp: true)) return true;
        }

        if (Player.HasStatus(true, StatusID.SubtractivePalette_4102))
        {
            if (CometInBlackPvP.CanUse(out act, skipAoeCheck: true, usedUp: true)) return true;
            if(ThunderInMagentaPvP.CanUse(out act) && Player.HasStatus(true, StatusID.AetherhuesIi_4101)) return true;
            if (StoneInYellowPvP.CanUse(out act) && Player.HasStatus(true, StatusID.Aetherhues_4100)) return true;
            if (BlizzardInCyanPvP.CanUse(out act)) return true;
        }

        if (HolyInWhitePvP.CanUse(out act, skipAoeCheck: true, usedUp: true)) return true;
        if (IsMoving && Player.HasStatus(true, StatusID.SubtractivePalette_4102) && HolyInWhitePvE.Cooldown.CurrentCharges == 0)
        {
            if (ReleaseSubtractivePalettePvP.CanUse(out act)) return true;
        }
        if (WaterInBluePvP.CanUse(out act) && Player.HasStatus(true, StatusID.AetherhuesIi_4101)) return true;
        if (AeroInGreenPvP.CanUse(out act) && Player.HasStatus(true, StatusID.Aetherhues_4100)) return true;
        if (FireInRedPvP.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }

    private bool AttackGCD(out IAction? act, bool burst)
    {
        act = null;

        return false;
    }
    #endregion


}
