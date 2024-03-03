using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbsRotations.Magical
{
    public class BlmRotation : BLM_Base
    {

        public override string GameVersion => "6.55";
        public override string RotationName => "Rabbs BLM";
        public override string Description => "PvP Rabbs BLM";
        public override CombatType Type => CombatType.PvP;
        public override bool ShowStatus => true;

        protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
            .SetBool(CombatType.PvP, "EnableLB", true, "Do you want to enable Limit Break");

        public static IBaseAction PvP_AetherialManipulation2 { get; } = new BaseAction(ActionID.PvP_AetherialManipulation)
        {
            ChoiceTarget = (Targets, mustUse) =>
            {
                Targets = HostileTargets.Where(b => b.YalmDistanceX < 25).ToArray();

                if (Targets.Any())
                {
                    return Targets.OrderBy(ObjectHelper.GetHealthRatio).First();
                }

                return null;
            },
            ActionCheck = (b,m) => !PvP_Burst.IsCoolingDown && PvP_AetherialManipulation2.ChoiceTarget is not null
        };

        public static IBaseAction PvP_Burst2 { get; } = new BaseAction(ActionID.PvP_Burst)
        {
            ActionCheck = (b, m) => HostileTargets.Where(a => a.YalmDistanceX < 5).Any()
        };

        public static IBaseAction PvP_Paradox2 { get; } = new BaseAction(ActionID.PvP_Paradox)
        {
            ChoiceTarget = (Targets, mustUse) =>
            {
                Targets = HostileTargets.Where(b => b.YalmDistanceX < 25).ToArray();

                if (Targets.Any())
                {
                    return Targets.OrderBy(ObjectHelper.GetHealthRatio).First();
                }

                return null;
            },
            ActionCheck = (b, m) => PvP_Paradox2.Target.StatusStack(true, (StatusID)3216) < 3 && PvP_Paradox2.ChoiceTarget is not null
        };

        public static IBaseAction PvP_Superflare2 { get; } = new BaseAction(ActionID.PvP_Superflare)
        {
            ActionCheck = (b, m) => HostileTargets.Where(a => a.YalmDistanceX < 25 && a.StatusStack(true, (StatusID)3216) == 3).Any()
        };


        protected override bool GeneralGCD(out IAction act)
        {
            act = null;

            #region PvP
            if (PvP_Burst2.CanUse(out act, CanUseOption.MustUse)) return true;
            if (PvP_AetherialManipulation2.CanUse(out act, CanUseOption.MustUse) && PvP_AetherialManipulation2.Target.CurrentHp < 20000 && Player.CurrentHp > 20000) return true;
            if (PvP_SoulResonance.CanUse(out act, CanUseOption.MustUse) && Configs.GetBool("EnableLB") && HostileTarget && PvP_Fire.Target.CurrentHp < 40000 && PvP_SoulResonance.IsEnabled) return true;
            if (PvP_Paradox2.CanUse(out act, CanUseOption.MustUse)) return true;
            if (PvP_Superflare2.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
            if (PvP_Nightwing.CanUse(out act, CanUseOption.MustUse)) return true;
            if (PvP_Fire.CanUse(out act, CanUseOption.MustUse) && !IsMoving & !IsLastAbility(ActionID.PvP_AetherialManipulation) || (IsMoving && (Player.HasStatus(true, StatusID.PvP_UmbralIce3)|| Player.HasStatus(true, StatusID.PvP_UmbralIce2)))) return true;
            if (PvP_Blizzard.CanUse(out act, CanUseOption.MustUse) && !IsLastAbility(ActionID.PvP_AetherialManipulation)) return true;

            return false;
            #endregion
        }


    }
}
