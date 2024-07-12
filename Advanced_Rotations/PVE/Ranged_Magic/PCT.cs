using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Data;
using System;
using static Lumina.Data.Parsing.Layer.LayerCommon;

namespace RabbsRotationsNET8.Magical;

[Rotation("BobRoss", CombatType.PvE, GameVersion = "7.0")]
[SourceCode(Path = "main/DefaultRotations/Magical/PCT_Default.cs")]
[Api(2)]
public sealed class PCT_Default : PictomancerRotation
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

        return base.EmergencyAbility(nextGCD, out act);
    }
    #endregion

    #region oGCD Logic
    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {



        if (Player.HasStatus(true, StatusID.StarryMuse))
        {
            if (Player.HasStatus(true, StatusID.SubtractiveSpectrum) && !Player.HasStatus(true, StatusID.SubtractivePalette))
            {
                if (SubtractivePalettePvE.CanUse(out act)) return true;
            }


            if (CreatureMotifDrawn)
            {
                if (PomMusePvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true, skipComboCheck: true, skipAoeCheck: true, usedUp: true) && CreatureMotifDrawn && LivingMusePvE.AdjustedID == PomMusePvE.ID) return true;
                if (WingedMusePvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true, skipComboCheck: true, skipAoeCheck: true, usedUp: true) && CreatureMotifDrawn && LivingMusePvE.AdjustedID == WingedMusePvE.ID) return true;
                if (ClawedMusePvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true, skipComboCheck: true, skipAoeCheck: true, usedUp: true) && CreatureMotifDrawn && LivingMusePvE.AdjustedID == ClawedMusePvE.ID) return true;
                if (FangedMusePvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true, skipComboCheck: true, skipAoeCheck: true, usedUp: true) && CreatureMotifDrawn && LivingMusePvE.AdjustedID == FangedMusePvE.ID) return true;
            }



        }

        if (!Player.HasStatus(true, StatusID.SubtractivePalette) && (PaletteGauge >= 50 || Player.HasStatus(true, StatusID.SubtractiveSpectrum)) && SubtractivePalettePvE.CanUse(out act)) return true;

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

        if (StarPrismPvE.CanUse(out act) && Player.HasStatus(true, StatusID.Starstruck)) return true;

        if (RainbowDripPvE.CanUse(out act) && Player.HasStatus(true, StatusID.RainbowBright)) return true;

        // white/black paint use while moving
        if (IsMoving)
        {
            if (HammerStampPvE.CanUse(out act, skipCastingCheck: true, skipAoeCheck: true) && Player.HasStatus(true, StatusID.HammerTime) && InCombat) return true;
            if (CometInBlackPvE.CanUse(out act, skipCastingCheck: true, skipAoeCheck: true) && Paint > 0 && Player.HasStatus(true, StatusID.MonochromeTones)) return true;
            if (HolyInWhitePvE.CanUse(out act, skipCastingCheck: true, skipAoeCheck: true) && Paint > 0) return true;
        }

        if (Player.HasStatus(true, StatusID.StarryMuse))
        {
            if (MadeenPortraitReady)
            {
                if (RetributionOfTheMadeenPvE.CanUse(out act)) return true;
            }
            if (MooglePortraitReady)
            {
                if (MogOfTheAgesPvE.CanUse(out act)) return true;
            }
            if (!Player.HasStatus(true, StatusID.HammerTime) && WeaponMotifDrawn && SteelMusePvE.Cooldown.HasOneCharge && Player.StatusTime(true, StatusID.StarryMuse) >= 15f)
            {
                if (StrikingMusePvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true, skipComboCheck: true, skipAoeCheck: true, usedUp: true)) return true;
            }
            if (Player.HasStatus(true, StatusID.HammerTime))
            {
                if (HammerStampPvE.CanUse(out act, skipCastingCheck: true, skipAoeCheck: true) && Player.HasStatus(true, StatusID.HammerTime) && InCombat) return true;
            }
            if (ThunderIiInMagentaPvE.CanUse(out act) && Player.HasStatus(true, StatusID.SubtractivePalette) && Player.HasStatus(true, StatusID.AetherhuesIi)) return true;
            if (StoneIiInYellowPvE.CanUse(out act) && Player.HasStatus(true, StatusID.SubtractivePalette) && Player.HasStatus(true, StatusID.Aetherhues)) return true;
            if (BlizzardIiInCyanPvE.CanUse(out act) && Player.HasStatus(true, StatusID.SubtractivePalette)) return true;
        }

        if (HammerStampPvE.CanUse(out act, skipCastingCheck: true, skipAoeCheck: true) && Player.HasStatus(true, StatusID.HammerTime) && InCombat) return true;

        if (!InCombat)
        {
            

        if (!CreatureMotifDrawn)
            {
                if (PomMotifPvE.CanUse(out act) && CreatureMotifPvE.AdjustedID == PomMotifPvE.ID) return true;
                if (WingMotifPvE.CanUse(out act) && CreatureMotifPvE.AdjustedID == WingMotifPvE.ID) return true;
                if (ClawMotifPvE.CanUse(out act) && CreatureMotifPvE.AdjustedID == ClawMotifPvE.ID) return true;
                if (MawMotifPvE.CanUse(out act) && CreatureMotifPvE.AdjustedID == MawMotifPvE.ID) return true;
            }
        if (!WeaponMotifDrawn) 
            { 
                if (HammerMotifPvE.CanUse(out act)) return true;
            }
        if (!LandscapeMotifDrawn) 
            { 
                if (StarrySkyMotifPvE.CanUse(out act)&&!Player.HasStatus(true, StatusID.Hyperphantasia)) return true;
            }

        if (RainbowDripPvE.CanUse(out act)) return true;
        
        }
        if (InCombat && (HasSwift || !HasHostilesInMaxRange) && (!CreatureMotifDrawn || (!WeaponMotifDrawn && !Player.HasStatus(true, StatusID.HammerTime)) || !LandscapeMotifDrawn))
        {

            if (!LandscapeMotifDrawn)
            {
                if (StarrySkyMotifPvE.CanUse(out act) && !Player.HasStatus(true, StatusID.Hyperphantasia)) return true;
            }
            if (!WeaponMotifDrawn && !Player.HasStatus(true, StatusID.HammerTime))
            {
                if (HammerMotifPvE.CanUse(out act)) return true;
            }
            if (!CreatureMotifDrawn)
            {
                if (PomMotifPvE.CanUse(out act) && CreatureMotifPvE.AdjustedID == PomMotifPvE.ID) return true;
                if (WingMotifPvE.CanUse(out act) && CreatureMotifPvE.AdjustedID == WingMotifPvE.ID) return true;
                if (ClawMotifPvE.CanUse(out act) && CreatureMotifPvE.AdjustedID == ClawMotifPvE.ID) return true;
                if (MawMotifPvE.CanUse(out act) && CreatureMotifPvE.AdjustedID == MawMotifPvE.ID) return true;
            }
        }

        if (!LandscapeMotifDrawn && ScenicMusePvE.Cooldown.RecastTimeRemainOneCharge <= LandscapeMotifPvE.Info.CastTime)
        {
            if (StarrySkyMotifPvE.CanUse(out act) && !Player.HasStatus(true, StatusID.Hyperphantasia)) return true;
        }
        if (!CreatureMotifDrawn && (LivingMusePvE.Cooldown.HasOneCharge || LivingMusePvE.Cooldown.RecastTimeRemainOneCharge <= CreatureMotifPvE.Info.CastTime))
        {
            if (PomMotifPvE.CanUse(out act) && CreatureMotifPvE.AdjustedID == PomMotifPvE.ID) return true;
            if (WingMotifPvE.CanUse(out act) && CreatureMotifPvE.AdjustedID == WingMotifPvE.ID) return true;
            if (ClawMotifPvE.CanUse(out act) && CreatureMotifPvE.AdjustedID == ClawMotifPvE.ID) return true;
            if (MawMotifPvE.CanUse(out act) && CreatureMotifPvE.AdjustedID == MawMotifPvE.ID) return true; ;
        }
        if (!WeaponMotifDrawn && !Player.HasStatus(true, StatusID.HammerTime) && (SteelMusePvE.Cooldown.HasOneCharge || SteelMusePvE.Cooldown.RecastTimeRemainOneCharge <= WeaponMotifPvE.Info.CastTime))
        {
            if (HammerMotifPvE.CanUse(out act)) return true;
        }

        if (InCombat)
        {

            if (ScenicMusePvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true, skipComboCheck: true, skipAoeCheck: true, usedUp: true) && LandscapeMotifDrawn) return true;
            if (MogOfTheAgesPvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true, skipComboCheck: true, skipAoeCheck: true, usedUp: true) && MooglePortraitReady) return true;
            if (StrikingMusePvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true, skipComboCheck: true, skipAoeCheck: true, usedUp: true) && WeaponMotifDrawn) return true;
            if (PomMusePvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true, skipComboCheck: true, skipAoeCheck: true, usedUp: true) && CreatureMotifDrawn && LivingMusePvE.AdjustedID == PomMusePvE.ID) return true;
            if (WingedMusePvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true, skipComboCheck: true, skipAoeCheck: true, usedUp: true) && CreatureMotifDrawn && LivingMusePvE.AdjustedID == WingedMusePvE.ID) return true;
            if (ClawedMusePvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true, skipComboCheck: true, skipAoeCheck: true, usedUp: true) && CreatureMotifDrawn && LivingMusePvE.AdjustedID == ClawedMusePvE.ID) return true;
            if (FangedMusePvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true, skipComboCheck: true, skipAoeCheck: true, usedUp: true) && CreatureMotifDrawn && LivingMusePvE.AdjustedID == FangedMusePvE.ID) return true;
        }




        

        //white paint over cap protection
        if (Paint == 5)
        {
            if (CometInBlackPvE.CanUse(out act, skipCastingCheck: true, skipAoeCheck: true) && Paint > 0 && Player.HasStatus(true, StatusID.MonochromeTones)) return true;
            if (HolyInWhitePvE.CanUse(out act, skipCastingCheck: true, skipAoeCheck: true) && Paint > 0) return true;
        }





        ///123 combo stuff with moving checks
        if (!IsMoving)
        {

            ///aoe
            ///

            if (ThunderIiInMagentaPvE.CanUse(out act) && Player.HasStatus(true, StatusID.SubtractivePalette) && Player.HasStatus(true, StatusID.AetherhuesIi)) return true;
            if (StoneIiInYellowPvE.CanUse(out act) && Player.HasStatus(true, StatusID.SubtractivePalette) && Player.HasStatus(true, StatusID.Aetherhues)) return true;
            if (BlizzardIiInCyanPvE.CanUse(out act) && Player.HasStatus(true, StatusID.SubtractivePalette)) return true;


            if (WaterIiInBluePvE.CanUse(out act) && Player.HasStatus(true, StatusID.AetherhuesIi)) return true;
            if (AeroIiInGreenPvE.CanUse(out act) && Player.HasStatus(true, StatusID.Aetherhues)) return true;
            if (FireIiInRedPvE.CanUse(out act)) return true;

            ///single target
            ///


            if (ThunderInMagentaPvE.CanUse(out act) && Player.HasStatus(true, StatusID.SubtractivePalette) && Player.HasStatus(true, StatusID.AetherhuesIi)) return true;
            if (StoneInYellowPvE.CanUse(out act) && Player.HasStatus(true, StatusID.SubtractivePalette) && Player.HasStatus(true, StatusID.Aetherhues)) return true;
            if (BlizzardInCyanPvE.CanUse(out act) && Player.HasStatus(true, StatusID.SubtractivePalette)) return true;


            if (WaterInBluePvE.CanUse(out act) && Player.HasStatus(true, StatusID.AetherhuesIi)) return true;
            if (AeroInGreenPvE.CanUse(out act) && Player.HasStatus(true, StatusID.Aetherhues)) return true;
            if (FireInRedPvE.CanUse(out act)) return true;

        }
        return base.GeneralGCD(out act);
    }

    private bool AttackGCD(out IAction? act, bool burst)
    {
        act = null;

        return false;
    }
    #endregion


}
