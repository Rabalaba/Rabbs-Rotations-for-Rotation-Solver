using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbsRotations.Tank
{
    public class GNBRotation : GNB_Base
    {

        public override string GameVersion => "6.55";
        public override string RotationName => "Rabbs GNB";
        public override string Description => "PvP Rotation for GNB";
        public override CombatType Type => CombatType.PvP;
        public override bool ShowStatus => true;

        protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration();

        public static IBaseAction PVP_Gnashed { get; } = new BaseAction(AdjustId(ActionID.PvP_GnashingFang));
        public static IBaseAction PVP_Hypervelocity { get; } = new BaseAction(ActionID.PvP_HyperVelocity);
        public static IBaseAction PVP_EyeGouge { get; } = new BaseAction(ActionID.PvP_EyeGouge);
        public static IBaseAction PVP_AbdomenTear { get; } = new BaseAction(ActionID.PvP_AbdomenTear);
        public static IBaseAction PVP_JugularRip { get; } = new BaseAction(ActionID.PvP_JugularRip);
        public static IBaseAction PVP_SolidBarrel { get; } = new BaseAction(ActionID.PvP_SolidBarrel);
        public static IBaseAction PVP_BrutalShell { get; } = new BaseAction(ActionID.PvP_BrutalShell);
        public static IBaseAction PVP_KeenEdge { get; } = new BaseAction(ActionID.PvP_KeenEdge);
        public static IBaseAction PVP_GnashingFang { get; } = new BaseAction(ActionID.PvP_GnashingFang);
        public static IBaseAction PVP_SavageClaw { get; } = new BaseAction(ActionID.PvP_SavageClaw);
        public static IBaseAction PVP_WickedTalon { get; } = new BaseAction(ActionID.PvP_WickedTalon);
        public static IBaseAction PVP_DoubleDown { get; } = new BaseAction(ActionID.PvP_DoubleDown)
        {
            ActionCheck = (b, m) => HostileTargets.Where(a => a.YalmDistanceX < 5).Count() > 1
        };
        public static IBaseAction PVP_RoughDivide { get; } = new BaseAction(ActionID.PvP_RoughDivide);
        public static IBaseAction PVP_BlastingZone { get; } = new BaseAction(ActionID.PvP_BlastingZone);
        public static IBaseAction PVP_Nebula { get; } = new BaseAction(ActionID.PvP_Nebula);
        public static IBaseAction PVP_Auora { get; } = new BaseAction(ActionID.PvP_Aurora);
        public static IBaseAction PVP_JuntionCast { get; } = new BaseAction(ActionID.PvP_JunctionCast);
        public static IBaseAction PvP_DrawAndJunction { get; } = new BaseAction(ActionID.PvP_DrawAndJunction)
        {
            ChoiceTarget = (Targets, mustUse) =>
            {
                return Player;
            },
            ActionCheck = (b, m) => !Player.HasStatus(true, (StatusID)3044)
        };
        public static IBaseAction PvP_RelentlesRush { get; } = new BaseAction(ActionID.PvP_RelentlessRush)
        {
            ActionCheck = (b, m) => LimitBreakLevel >= 1 && HostileTargets.Where(a => a.YalmDistanceX < 5).Count() > 1
        };


        protected override bool GeneralGCD(out IAction act)
        {
            act = null;

            #region PvP
            uint gnashId = AdjustId(PVP_GnashingFang.ID);
            if (PvP_RelentlesRush.CanUse(out act, CanUseOption.MustUseEmpty) && gnashId == PVP_GnashingFang.ID) return true;
            if (PVP_DoubleDown.CanUse(out act, CanUseOption.MustUseEmpty) && gnashId == PVP_GnashingFang.ID) return true;
            if (PVP_SavageClaw.CanUse(out act, CanUseOption.MustUseEmpty) && gnashId == PVP_SavageClaw.ID) return true;
            if (PVP_WickedTalon.CanUse(out act, CanUseOption.MustUseEmpty) && gnashId == PVP_WickedTalon.ID) return true;
            if (PVP_GnashingFang.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
            
            
            if (PVP_SolidBarrel.CanUse(out act, CanUseOption.MustUse)) return true;
            if (PVP_BrutalShell.CanUse(out act, CanUseOption.MustUse)) return true;
            if (PVP_KeenEdge.CanUse(out act, CanUseOption.MustUse)) return true;


            return false;
            #endregion
        }

        protected override bool AttackAbility(out IAction act)
        {
            act = null;

            #region PvP
            uint junkId = AdjustId(PVP_JuntionCast.ID);
            if (PVP_EyeGouge.CanUse(out act, CanUseOption.MustUseEmpty) && Player.HasStatus(true, StatusID.PvP_ReadyToGouge)) return true;
            if (PVP_AbdomenTear.CanUse(out act, CanUseOption.MustUseEmpty) && Player.HasStatus(true, StatusID.PvP_ReadyToTear)) return true;
            if (PVP_JugularRip.CanUse(out act, CanUseOption.MustUseEmpty) && Player.HasStatus(true, StatusID.PvP_ReadyToRip)) return true;           
            if (PVP_Hypervelocity.CanUse(out act, CanUseOption.MustUseEmpty) && Player.HasStatus(true, StatusID.PvP_ReadyToBlast)) return true;
            if (PVP_BlastingZone.CanUse(out act, CanUseOption.MustUseEmpty) && junkId == PVP_BlastingZone.ID) return true;
            if (PVP_Nebula.CanUse(out act, CanUseOption.MustUseEmpty) && junkId == PVP_Nebula.ID) return true;
            if (PVP_Auora.CanUse(out act, CanUseOption.MustUseEmpty) && junkId == PVP_Auora.ID) return true;
            if (PVP_RoughDivide.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
            if (PvP_DrawAndJunction.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
            return false;
            #endregion
        }


    }
    }
