using RotationSolver.Basic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static FFXIVClientStructs.FFXIV.Client.Game.Control.GazeController;

namespace RabbsRotations.Ranged;
[Rotation("Rabbs Dancer PVP", CombatType.PvP, GameVersion = "6.58")]
[Api(1)]
[SourceCode(Path = "main/RabbsRotations/Ranged/DNC.cs")]



public unsafe sealed class DNC_PVP : DancerRotation
{

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
        var IsTargetDying = HostileTarget?.IsDying() ?? false;

        #region pvp
        //if (AllHostileTargets.Any(p => p.DistanceToPlayer() < 5) && HoningDancePvP.CanUse(out act, skipAoeCheck:true)) return true;

        if (StarfallDancePvP.CanUse(out act)) return true;

        if (FountainPvP.CanUse(out act)) return true;

        if (CascadePvP.CanUse(out act)) return true;

        #endregion


        return base.GeneralGCD(out act);
    }



    protected unsafe override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        #region pvp

        //if (Player.MaxHp - Player.CurrentHp >= 10000)
        //{
            //if (CuringWaltzPvP.CanUse(out act)) return true;
        //}
        if (HasHostilesInRange && !Player.HasStatus(false, StatusID.FlourishingSaberDance, StatusID.EnAvant))
        if (EnAvantPvP.CanUse(out act, usedUp:true)) return true;

        if (FanDancePvP.CanUse(out act, skipAoeCheck:true,  usedUp:true)) return true;



        #endregion
       

        return base.AttackAbility(nextGCD, out act);
    }
}
