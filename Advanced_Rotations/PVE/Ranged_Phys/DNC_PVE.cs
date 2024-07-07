using RotationSolver.Basic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static FFXIVClientStructs.FFXIV.Client.Game.Control.GazeController;

namespace RabbsRotationsNET8.PVE.Ranged_Phys;
[Rotation("Rabbs Dancer PVE & PVP", CombatType.Both, GameVersion = "6.58")]
[Api(1)]
[SourceCode(Path = "main/RabbsRotations/Ranged/DNC.cs")]



public unsafe sealed class DNC : DancerRotation
{

    protected override IAction? CountDownAction(float remainTime)
    {
        if (remainTime < 0.3)
            if (DanceFinishGCD(out var act)) return act;

        if (remainTime <= 15)
        {
            if (StandardStepPvE.CanUse(out var act, skipAoeCheck: true)) return act;
            if (ExecuteStepGCD(out act)) return act;
        }
        return base.CountDownAction(remainTime);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (IsDancing)
        {
            if (DanceFinishGCD(out act)) return true;
            if (ExecuteStepGCD(out act)) return true;
            return base.EmergencyAbility(nextGCD, out act);
        }

        if (TechnicalStepPvE.Cooldown.ElapsedAfter(115)
            && UseBurstMedicine(out act)) return true;

        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool GeneralGCD(out IAction? act)
    {
        var IsTargetDying = HostileTarget?.IsDying() ?? false;



        #region GCD
        if (IsDancing)
        {
            if (DanceFinishGCD(out act)) return true;
            if (ExecuteStepGCD(out act)) return true;
        }

        if (!Player.HasStatus(true, StatusID.ClosedPosition) && ClosedPositionPvE.CanUse(out act)) return true;

        // ST Standard Step (outside of burst)
        //if (!Player.HasStatus(true, StatusID.TechnicalFinish))
        //{
            //if (AllHostileTargets.Any(p => !p.IsDying() && p.DistanceToPlayer() < 10 && p.IsTargetable) &&
                //TechnicalStepPvE.Cooldown.RecastTimeRemainOneCharge > 5 &&
               // (!FlourishPvE.Cooldown.IsCoolingDown || FlourishPvE.Cooldown.RecastTimeRemainOneCharge > 5))
              //  if (StandardStepPvE.CanUse(out act)) return true;
     //   }

        // ST Technical Step
        //if (AllHostileTargets.Any(p => !p.IsDying() && p.DistanceToPlayer() < 10) &&
            //InCombat &&
            //!Player.HasStatus(true, StatusID.StandardStep))
            //if (TechnicalStepPvE.CanUse(out act)) return true;

        // ST Saber Dance
        if (TechnicalStepPvE.Cooldown.RecastTimeRemainOneCharge > 5 || !TechnicalStepPvE.Cooldown.IsCoolingDown)
        {
            if (Esprit >= 85 ||
                Player.HasStatus(true, StatusID.TechnicalFinish) && Esprit >= 50)
                if (SaberDancePvE.CanUse(out act, skipAoeCheck: true)) return true;
        }

        if (Player.HasStatus(true, StatusID.FlourishingStarfall))
            if (StarfallDancePvE.CanUse(out act, skipAoeCheck: true)) return true;

        if (Player.HasStatus(true, StatusID.FlourishingFinish))
            if (TillanaPvE.CanUse(out act, skipAoeCheck: true)) return true;

        // ST Standard Step (inside of burst)
       // if (AllHostileTargets.Any(p => !p.IsDying() && p.DistanceToPlayer() < 10) &&
           // Player.HasStatus(true, StatusID.TechnicalFinish))
       // {
         //   if (!IsTargetDying &&
          //      Player.StatusTime(true, StatusID.TechnicalFinish) > 5)
           //     if (StandardStepPvE.CanUse(out act)) return true;
       // }

        if (BloodshowerPvE.CanUse(out act)) return true;
        if (FountainfallPvE.CanUse(out act)) return true;
        if (RisingWindmillPvE.CanUse(out act)) return true;
        if (ReverseCascadePvE.CanUse(out act)) return true;
        if (BladeshowerPvE.CanUse(out act)) return true;
        if (WindmillPvE.CanUse(out act)) return true;
        if (FountainPvE.CanUse(out act)) return true;
        if (CascadePvE.CanUse(out act)) return true;
        #endregion

        return base.GeneralGCD(out act);
    }



    protected unsafe override bool AttackAbility(IAction nextGCD, out IAction? act)
    {

        if (IsDancing)
        {
            if (DanceFinishGCD(out act)) return true;
            if (ExecuteStepGCD(out act)) return true;
        }
        var IsTargetDying = HostileTarget?.IsDying() ?? false;

        if (Player.HasStatus(true, StatusID.TechnicalFinish) || !TechnicalStepPvE.EnoughLevel)
            if (DevilmentPvE.CanUse(out act)) return true;

        // ST Flourish
        if (!Player.HasStatus(true, StatusID.ThreefoldFanDance) && !Player.HasStatus(true, StatusID.FourfoldFanDance) &&
            !Player.HasStatus(true, StatusID.FlourishingSymmetry) && !Player.HasStatus(true, StatusID.FlourishingFlow))
            if (FlourishPvE.CanUse(out act)) return true;


        if (Player.HasStatus(true, StatusID.ThreefoldFanDance))
            if (FanDanceIiiPvE.CanUse(out act, skipAoeCheck: true)) return true;

        // FD1 HP% Dump
        if (IsTargetDying && Feathers > 0)
            if (FanDancePvE.CanUse(out act)) return true;

        // Burst FD1
        if (Player.HasStatus(true, StatusID.TechnicalFinish) && Feathers > 0)
            if (FanDancePvE.CanUse(out act)) return true;

        // FD1 Pooling
        if (Feathers > 3 &&
                (TechnicalStepPvE.Cooldown.RecastTimeRemainOneCharge > 2.5f || !TechnicalStepPvE.Cooldown.IsCoolingDown))
            if (FanDancePvE.CanUse(out act)) return true;

        // FD1 Non-pooling & under burst level
        if (!TechnicalStepPvE.EnoughLevel && Feathers > 0)
            if (FanDancePvE.CanUse(out act)) return true;
        if (Player.HasStatus(true, StatusID.FourfoldFanDance))
            if (FanDanceIvPvE.CanUse(out act, skipAoeCheck: true)) return true;

        return base.AttackAbility(nextGCD, out act);
    }
}
