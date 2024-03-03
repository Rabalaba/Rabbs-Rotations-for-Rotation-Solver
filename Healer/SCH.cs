using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Types;

namespace RabbsRotations.Healer
{
    [RotationDesc(ActionID.ChainStratagem)]
    public sealed class SchRotation : SCH_Base
    {
        public override string GameVersion => "6.55";
        public override string RotationName => "Rabbs SCH";
        public override string Description => "PVP Rabbs SCH";
        public override CombatType Type => CombatType.PvP;

        public static IBaseAction PVP_Broil { get; } = new BaseAction(ActionID.PvP_Broil);

        public static IBaseAction PvP_Adloquilum { get; } = new BaseAction(ActionID.PvP_Adloquilum);

        public static IBaseAction PvP_DeploymentTactics { get; } = new BaseAction(ActionID.PvP_DeploymentTactics)
        {
            ChoiceTarget = (Targets, mustUse) =>
            {
                Targets = AllHostileTargets.Where(b => b.DistanceToPlayer() < 30 && b.HasStatus(true, StatusID.PvP_Biolysis)).ToArray();

                if (Targets.Any())
                {
                    return Targets.OrderBy(ObjectHelper.GetHealthRatio).First();
                }

                return null;
            }
        };

        public static IBaseAction PvP_Biolysis { get; } = new BaseAction(ActionID.PvP_Biolysis)
        {
            ChoiceTarget = (Targets, mustUse) =>
            {
                // Filter targets with Biolysis
                Targets = AllHostileTargets.Where(b => b.DistanceToPlayer() < 25).ToArray();

                if (!Targets.Any())
                {
                    return null;
                }

                // Calculate enemy density around each target
                var enemyDensity = new Dictionary<Dalamud.Game.ClientState.Objects.Types.BattleChara, int>();
                foreach (var target in Targets)
                {
                    enemyDensity[target] = AllHostileTargets.Where(other =>
                    {
                        var dx = other.Position.X - target.Position.X;
                        var dy = other.Position.Y - target.Position.Y;
                        var dz = other.Position.Z - target.Position.Z;
                        return Math.Sqrt(dx * dx + dy * dy + dz * dz) <= 5 && other != target;
                    }).Count();
                }

                // Find target with the most enemies around
                //var targetWithMostEnemies = enemyDensity.OrderByDescending(pair => pair.Value).FirstOrDefault().Key;

                //return targetWithMostEnemies;
                var highestDensity = enemyDensity.Max(pair => pair.Value);
                var tiedTargets = enemyDensity.Where(pair => pair.Value == highestDensity).Select(pair => pair.Key).ToArray();

                // Select target with lowest health among tied ones
                return tiedTargets.OrderByDescending(t => ObjectHelper.GetHealthRatio(t)).LastOrDefault();
            }
        };

        public static IBaseAction PvP_Expedient { get; } = new BaseAction(ActionID.PvP_Expedient)
        {
            ActionCheck = (b, m) =>
            {
                // Check if Biolysis is active and there's a target
                if (!b.HasStatus(true, StatusID.PvP_Biolysis) || b == null)
                {
                    return false; // Expedient not applicable
                }

                // Calculate enemy density around Biolysis target within 5 yalms
                var enemyDensity = AllHostileTargets.Count(other =>
                {
                    var dx = other.Position.X - b.Position.X;
                    var dy = other.Position.Y - b.Position.Y;
                    var dz = other.Position.Z - b.Position.Z;
                    return Math.Sqrt(dx * dx + dy * dy + dz * dz) <= 5 && other != b;
                });

                // Use Expedient if enemy density is >= 2
                return enemyDensity >= 2;
            }
        };

        public static IBaseAction PvP_Mummification { get; } = new BaseAction(ActionID.PvP_Mummification);


        public static IBaseAction PvP_SummonSeraph { get; } = new BaseAction(ActionID.PvP_SummonSeraph)
        {
            ChoiceTarget = (Targets, mustUse) =>
            {
                Targets = PartyMembers.Where(b => b.YalmDistanceX < 30).ToArray();

                if (Targets.Any())
                {
                    return Targets.OrderBy(ObjectHelper.GetHealthRatio).First();
                }

                return Player;
            },
            ActionCheck = (BattleChara b, bool m) => LimitBreakLevel >= 1 && PartyMembers.Count() > 1 && PartyMembers.Any((Func<BattleChara, bool>)(p => p.GetHealthRatio() <= 0.8))
        };

        public static IBaseAction PvP_Consolation { get; } = new BaseAction(ActionID.PvP_Consolation);


        protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
        {

            return base.EmergencyAbility(nextGCD, out act);
        }

        protected override bool GeneralGCD(out IAction act)
        {
            act = null;
            ///SummonEos.CanUse(out act) || Bio.CanUse(out act) || ArtOfWar.CanUse(out act) || Ruin.CanUse(out act) || Ruin2.CanUse(out act) || Bio.CanUse(out act, CanUseOption.MustUse)


            #region PvP
            if (PvP_SummonSeraph.CanUse(out act, CanUseOption.MustUseEmpty)) return true;

            if (PvP_Adloquilum.CanUse(out act)) return true;

            if (PvP_Mummification.CanUse(out act, CanUseOption.MustUseEmpty)) return true;

            if (PvP_Biolysis.CanUse(out act, CanUseOption.MustUseEmpty) && Player.HasStatus(true, StatusID.PvP_Recitation)) return true;

            if (PVP_Broil.CanUse(out act)) return true;

            #endregion

            return false;
        }

        

        protected override bool AttackAbility(out IAction act)
        {
            act = null;
            //if (PvP_Consolation.CanUse(out act)) return true;
            if (PvP_Expedient.CanUse(out act)) return true;
            if (PvP_DeploymentTactics.CanUse(out act)) return true;
            return false;
        }


    }
}
