using Dalamud.Game;
using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Basic.Data;
using System;
using static FFXIVClientStructs.FFXIV.Client.Game.Control.GazeController;

namespace RabbsRotationsNET8.PVE.Tank;

[Rotation("Rabbs Gnb (Single Target Only for now)", CombatType.PvE, GameVersion = "6.58")]
[SourceCode(Path = "main/RabbsRotations/Tank/GNB.cs")]
[Api(1)]
public unsafe sealed class GNB_PVE : GunbreakerRotation
{

    public override bool CanHealSingleSpell => false;

    public override bool CanHealAreaSpell => false;

    private static bool NoMercy => Player.HasStatus(true, StatusID.NoMercy);
    public static int MaxCartridges => Player.Level >= 88 ? 3 : 2;
    public static bool IsOddMinute()
    {
        // Get whole minutes from CombatTime (assuming it represents seconds)
        int minutes = (int)Math.Floor(CombatTime / 60f);

        // Use modulo to check if minutes is odd (remainder 1)
        return minutes % 2 != 0;
    }
    public static bool TwoMinteWindow => CombatTime >= 118 && CombatTime < 135;
    public static float GetCooldownRemainingTime(IBaseAction baseAction) => baseAction.Cooldown.RecastTimeRemainOneCharge;
    public static bool IsOffCooldown(IBaseAction baseAction) => !baseAction.Cooldown.IsCoolingDown;
    public static bool IsOnCooldown(IBaseAction baseAction) => baseAction.Cooldown.IsCoolingDown;
    public static bool LevelChecked(IBaseAction baseAction) => baseAction.EnoughLevel;


    protected override IAction? CountDownAction(float remainTime)
    {
        if (remainTime <= 0.7 && LightningShotPvE.CanUse(out var act)) return act;
        return base.CountDownAction(remainTime);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {


        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool GeneralGCD(out IAction? act)
    {

        var aState = FFXIVClientStructs.FFXIV.Client.Game.UI.UIState.Instance()->PlayerState;
        var SkillSpeed = aState.Attributes[45];
        var gcdSpeed = SkillSpeed < 532 ? 250 : 247;
        bool slowSkS = gcdSpeed == 250;
        bool regularSkS = gcdSpeed == 247;

        if (NoMercyPvE.Cooldown.RecastTimeRemainOneCharge > 57 || NoMercy)
        {
            if (DoubleDownPvE.EnoughLevel)
            {
                if (regularSkS)
                {
                    if (Ammo >= 2 && !Player.HasStatus(true, StatusID.ReadyToRip) && AmmoComboStep >= 1)
                        if (DoubleDownPvE.CanUse(out act, skipAoeCheck: true)) return true;
                    if (DoubleDownPvE.Cooldown.IsCoolingDown)
                        if (SonicBreakPvE.CanUse(out act, skipAoeCheck: true)) return true;
                }

                if (slowSkS)
                {
                    if (!Player.HasStatus(true, StatusID.ReadyToRip) && AmmoComboStep >= 1)
                        if (SonicBreakPvE.CanUse(out act, skipAoeCheck: true)) return true;
                    if (Ammo >= 2 && SonicBreakPvE.Cooldown.IsCoolingDown)
                        if (DoubleDownPvE.CanUse(out act, skipAoeCheck: true)) return true;
                }
            }

            if (!DoubleDownPvE.EnoughLevel)
            {
                if (!Player.HasStatus(true, StatusID.ReadyToRip) && GnashingFangPvE.Cooldown.IsCoolingDown)
                    if (SonicBreakPvE.CanUse(out act, skipAoeCheck: true)) return true;
            }
        }
        //Pre Gnashing Fang stuff
        if (GnashingFangPvE.EnoughLevel)
        {
            bool activeNoMercy = NoMercyPvE.Cooldown.RecastTimeRemainOneCharge > 50 || NoMercy;
            if (GetCooldownRemainingTime(GnashingFangPvE) <= GetCooldownRemainingTime(KeenEdgePvE) && AmmoComboStep == 0 &&
                (Ammo == MaxCartridges && activeNoMercy && (!TwoMinteWindow && regularSkS || slowSkS) || //Regular 60 second GF/NM timing
                Ammo == MaxCartridges && activeNoMercy && TwoMinteWindow && GetCooldownRemainingTime(DoubleDownPvE) <= 1 && regularSkS || //2 min delay for regular SkS
                Ammo == 1 && NoMercy && GetCooldownRemainingTime(DoubleDownPvE) > 50 || //NMDDGF windows/Scuffed windows
                Ammo > 0 && GetCooldownRemainingTime(NoMercyPvE) > 17 && GetCooldownRemainingTime(NoMercyPvE) < 35 || //Regular 30 second window                                                                        
                Ammo == 1 && GetCooldownRemainingTime(NoMercyPvE) > 50 && (IsOffCooldown(BloodbathPvE) && LevelChecked(BloodbathPvE) || !LevelChecked(BloodbathPvE)))) //Opener Conditions
                if (GnashingFangPvE.CanUse(out act)) return true;
            if (AmmoComboStep is 1 or 2)
            {
                if (SavageClawPvE.CanUse(out act, skipComboCheck: true)) return true;
                if (WickedTalonPvE.CanUse(out act, skipComboCheck: true)) return true;
            }

        }
        if (NoMercy && AmmoComboStep == 0 && LevelChecked(BurstStrikePvE))
        {
            if (LevelChecked(HypervelocityPvE) && Player.HasStatus(true, StatusID.ReadyToBlast))
                if (HypervelocityPvE.CanUse(out act)) return true;
            if (Ammo != 0 && GetCooldownRemainingTime(GnashingFangPvE) > 4)
                if (BurstStrikePvE.CanUse(out act)) return true;
        }

        //final check if Burst Strike is used right before No Mercy ends
        if (LevelChecked(HypervelocityPvE) && Player.HasStatus(true, StatusID.ReadyToBlast))
            if (HypervelocityPvE.CanUse(out act)) return true;
        // Regular 1-2-3 combo with overcap feature

        if (BrutalShellPvE.CanUse(out act)) return true;
        if (LevelChecked(HypervelocityPvE) && Player.HasStatus(true, StatusID.ReadyToBlast))
            if (HypervelocityPvE.CanUse(out act)) return true;
        if (LevelChecked(BurstStrikePvE) && Ammo == MaxCartridges)
            if (BurstStrikePvE.CanUse(out act)) return true;
        if (SolidBarrelPvE.CanUse(out act)) return true;
        if (KeenEdgePvE.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        var aState = FFXIVClientStructs.FFXIV.Client.Game.UI.UIState.Instance()->PlayerState;
        var SkillSpeed = aState.Attributes[45];
        var gcdSpeed = SkillSpeed < 532 ? 250 : 247;
        bool slowSkS = gcdSpeed == 250;
        bool regularSkS = gcdSpeed == 247;
        if (!NoMercyPvE.Cooldown.IsCoolingDown)
        {
            if (BurstStrikePvE.EnoughLevel)
            {
                if (regularSkS)
                {
                    if (Ammo is 1 && CombatElapsedLess(30) && !BloodfestPvE.Cooldown.IsCoolingDown || //Opener Conditions
                       TwoMinteWindow && GetCooldownRemainingTime(DoubleDownPvE) < 4 || //2 min delay
                       !TwoMinteWindow && Ammo == MaxCartridges && GetCooldownRemainingTime(GnashingFangPvE) < 4) //Regular NMGF
                        if (NoMercyPvE.CanUse(out act)) return true;
                }

                if (slowSkS)
                {
                    if (CombatElapsedLess(30) && InCombat && Player.HasStatus(true, StatusID.BrutalShell) ||
                        Ammo == MaxCartridges ||
                        IsOddMinute() && Ammo == 2 && IsLastGCD(true, BurstStrikePvE))
                        if (NoMercyPvE.CanUse(out act)) return true;
                }
            }

            if (!BurstStrikePvE.EnoughLevel) //no cartridges unlocked
                if (NoMercyPvE.CanUse(out act)) return true;
        }



        if (Ammo is 0 && NoMercy)
        {
            if (regularSkS && GnashingFangPvE.Cooldown.IsCoolingDown || slowSkS && NoMercyPvE.Cooldown.IsCoolingDown)
                if (BloodfestPvE.CanUse(out act)) return true;
        }



        //Blasting Zone outside of NM
        if (!NoMercy && (GnashingFangPvE.Cooldown.IsCoolingDown && NoMercyPvE.Cooldown.RecastTimeRemainOneCharge > 17 || //Post Gnashing Fang
            !GnashingFangPvE.EnoughLevel)) //Pre Gnashing Fang
            if (DangerZonePvE.CanUse(out act)) return true;

        //Stops DZ Drift
        if (NoMercy && (SonicBreakPvE.Cooldown.IsCoolingDown && slowSkS || DoubleDownPvE.Cooldown.IsCoolingDown && regularSkS))
            if (DangerZonePvE.CanUse(out act)) return true;


        //Continuation
        if (EyeGougePvE.CanUse(out act)) return true;
        if (AbdomenTearPvE.CanUse(out act)) return true;
        if (JugularRipPvE.CanUse(out act)) return true;

        //60s weaves
        if (NoMercy)
        {
            //Post DD
            if (regularSkS && DoubleDownPvE.Cooldown.IsCoolingDown || slowSkS && SonicBreakPvE.Cooldown.IsCoolingDown)
            {
                if (DangerZonePvE.CanUse(out act)) return true;
                if (BowShockPvE.CanUse(out act, skipAoeCheck: true)) return true;
            }

            //Pre DD
            if (SonicBreakPvE.Cooldown.IsCoolingDown && !DoubleDownPvE.EnoughLevel)
            {
                if (BowShockPvE.CanUse(out act, skipAoeCheck: true)) return true;
                if (DangerZonePvE.CanUse(out act)) return true;
            }
        }


        //Rough Divide Feature
        if (!IsMoving && !Player.HasStatus(true, StatusID.ReadyToBlast))
        {
            if (HostileTarget.DistanceToPlayer() <= 3 && NoMercy && DangerZonePvE.Cooldown.IsCoolingDown && BowShockPvE.Cooldown.IsCoolingDown && DoubleDownPvE.Cooldown.IsCoolingDown)
                if (RoughDividePvE.CanUse(out act)) return true;
            if (HostileTarget.DistanceToPlayer() <= 3 && NoMercy && DangerZonePvE.Cooldown.IsCoolingDown && BowShockPvE.Cooldown.IsCoolingDown && DoubleDownPvE.Cooldown.IsCoolingDown && AmmoComboStep is 2)
                if (RoughDividePvE.CanUse(out act, usedUp: true)) return true;
        }

        // 60s window features
        if (NoMercyPvE.Cooldown.RecastTimeRemainOneCharge > 57 || NoMercy)
        {
            if (!DoubleDownPvE.EnoughLevel)
            {
                //sub level 54 functionality
                if (!SonicBreakPvE.EnoughLevel)
                    if (DangerZonePvE.CanUse(out act)) return true;
            }
        }

        return base.AttackAbility(nextGCD, out act);
    }

    protected override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
    {
        if (HeartOfLightPvE.CanUse(out act, onLastAbility: true)) return true;
        if (ReprisalPvE.CanUse(out act)) return true;
        return base.DefenseAreaAbility(nextGCD, out act);
    }

    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? act)
    {

        if (CamouflagePvE.CanUse(out act, onLastAbility: true)) return true;
        if (HeartOfStonePvE.CanUse(out act, onLastAbility: true)) return true;
        if ((!RampartPvE.Cooldown.IsCoolingDown || RampartPvE.Cooldown.ElapsedAfter(60)) && NebulaPvE.CanUse(out act)) return true;
        if (NebulaPvE.Cooldown.IsCoolingDown && NebulaPvE.Cooldown.ElapsedAfter(60) && RampartPvE.CanUse(out act)) return true;
        if (ReprisalPvE.CanUse(out act)) return true;

        return base.DefenseSingleAbility(nextGCD, out act);
    }

    protected override bool HealSingleAbility(IAction nextGCD, out IAction? act)
    {
        if (AuroraPvE.CanUse(out act, usedUp: true, onLastAbility: true)) return true;
        return base.HealSingleAbility(nextGCD, out act);
    }


}