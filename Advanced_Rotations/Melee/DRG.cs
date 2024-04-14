using RotationSolver.Basic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FFXIVClientStructs.FFXIV.Client.Game.Control.GazeController;

namespace RabbsRotations.Melee;
[Rotation("Rabbs Dragoon", CombatType.PvE, GameVersion = "6.58")]
[SourceCode(Path = "main/RabbsRotations/Melee/DRG.cs")]
public sealed class DRG : DragoonRotation
{
    #region Rotation Config

    #endregion
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
    public static bool HasEffect(StatusID status) => Player.HasStatus(true, status);
    public static IBaseAction SoloEye { get; } = new BaseAction((ActionID)7398);
    protected override IAction? CountDownAction(float remainTime)
    {
        return base.CountDownAction(remainTime);
    }
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        return base.EmergencyAbility(nextGCD, out act);
    }
    protected override bool GeneralGCD(out IAction? act)
    {

        //1-2-3 Combo
      
        if (CoerthanTormentPvE.CanUse(out act)) return true;

        if (SonicThrustPvE.CanUse(out act)) return true;

        if (DraconianFuryPvE.CanUse(out act)) return true;

        if (FangAndClawPvE.CanUse(out act)) return true;

        if (WheelingThrustPvE.CanUse(out act)) return true;

        if ((LevelChecked(ChaosThrustPvE) && HostileTarget != null && (!HostileTarget.HasStatus(true, StatusID.ChaoticSpring, StatusID.ChaosThrust) || HostileTarget.WillStatusEnd(6, true, StatusID.ChaoticSpring, StatusID.ChaosThrust))) || Player.WillStatusEnd(10, true, StatusID.PowerSurge, StatusID.PowerSurge_2720))
        {
            if (DisembowelPvE.CanUse(out act)) return true;

            if (ChaosThrustPvE.CanUse(out act)) return true;
        }

        if (VorpalThrustPvE.CanUse(out act)) return true;

        if (FullThrustPvE.CanUse(out act)) return true;


        if (TrueThrustPvE.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }
    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        if (WeaponRemain > 0.6)
        {
            if (Player.HasStatus(true, StatusID.PowerSurge, StatusID.PowerSurge_2720))
            {

                //Battle Litany Feature
                if (CombatElapsedLess(30) && HostileTarget != null && HostileTarget.HasStatus(true, StatusID.ChaoticSpring, StatusID.ChaosThrust))
                {
                    if (BattleLitanyPvE.CanUse(out act, skipAoeCheck: true)) return true;
                }

                if (CombatTime > 30)
                {
                    if (BattleLitanyPvE.CanUse(out act, skipAoeCheck: true)) return true;
                }

                //Lance Charge Feature
                if (LanceChargePvE.CanUse(out act, skipAoeCheck: true)) return true;

                //Dragon Sight Feature
                if (DragonSightPvE.CanUse(out act, skipAoeCheck: true)) return true;
                if (PartyMembers.Count() < 2 && SoloEye.CanUse(out act, skipStatusProvideCheck: true)) return true;

                //Life Surge Feature
                if (!HasEffect(StatusID.LifeSurge) &&
                    ((HasEffect(StatusID.RightEye) && HasEffect(StatusID.LanceCharge) && IsLastGCD(true, VorpalThrustPvE)) ||
                    (HasEffect(StatusID.LanceCharge) && IsLastGCD(true, VorpalThrustPvE)) ||
                    (HasEffect(StatusID.RightEye) && HasEffect(StatusID.LanceCharge) && (HasEffect(StatusID.WheelInMotion) || HasEffect(StatusID.FangAndClawBared))) ||
                    (IsOnCooldown(DragonSightPvE) && IsOnCooldown(LanceChargePvE) && IsLastGCD(true, VorpalThrustPvE))))
                    if (LifeSurgePvE.CanUse(out act, usedUp: true)) return true;
                //Wyrmwind Thrust Feature
                if (WyrmwindThrustPvE.CanUse(out act, skipAoeCheck: true)) return true;

                if (LevelChecked(BattleLitanyPvE) && CombatElapsedLess(30) && HostileTarget != null && Player.HasStatus(true, StatusID.BattleLitany) && !IsMoving && HostileTarget.DistanceToPlayer() <= 3)
                {
                    if (StardiverPvE.CanUse(out act, skipAoeCheck: true) && WeaponRemain > 1.5) return true;
                    if (NastrondPvE.CanUse(out act, skipAoeCheck: true)) return true;
                    if (GeirskogulPvE.CanUse(out act, skipAoeCheck: true)) return true;
                    if (HighJumpPvE.CanUse(out act, skipAoeCheck: true) && WeaponRemain > 0.8) return true;
                    if (MirageDivePvE.CanUse(out act, skipAoeCheck: true)) return true;
                    if (DragonfireDivePvE.CanUse(out act, skipAoeCheck: true) && WeaponRemain > 0.8) return true;
                    if (SpineshatterDivePvE.CanUse(out act, skipAoeCheck: true, usedUp: true) && WeaponRemain > 0.8) return true;
                }

                if (HostileTarget.DistanceToPlayer() <= 3 && !IsMoving && (CombatTime > 30 || !LevelChecked(BattleLitanyPvE)))
                {
                    if (StardiverPvE.CanUse(out act, skipAoeCheck: true) && WeaponRemain > 1.5) return true;
                    if (NastrondPvE.CanUse(out act, skipAoeCheck: true)) return true;
                    if (GeirskogulPvE.CanUse(out act, skipAoeCheck: true)) return true;
                    if (HighJumpPvE.CanUse(out act, skipAoeCheck: true) && WeaponRemain > 0.8) return true;
                    if (MirageDivePvE.CanUse(out act, skipAoeCheck: true)) return true;
                    if (DragonfireDivePvE.CanUse(out act, skipAoeCheck: true) && WeaponRemain > 0.8) return true;
                    if (SpineshatterDivePvE.CanUse(out act, skipAoeCheck: true, usedUp: true) && WeaponRemain > 0.8) return true;
                }
            }
        }

        return base.AttackAbility(nextGCD, out act);
    }
}

