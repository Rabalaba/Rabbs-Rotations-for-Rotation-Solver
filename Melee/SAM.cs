using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Gauge;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Data.Parsing;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Data;
using Svg;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FFXIVClientStructs.FFXIV.Client.Game.Control.GazeController;
using static FFXIVClientStructs.FFXIV.Client.Graphics.Kernel.VertexShader;

namespace RabbsRotations.Melee
{
    public sealed class SAM_Default : SAM_Base
    {
        public override CombatType Type => CombatType.PvE;
        public override string GameVersion => "6.55";
        public override string RotationName => "Rabbs Samurai Testin";
        public override string Description => "PVE Samurai w/ option for opener Grade8 Stength Pot(Check options)";

        #region Rotation Config
        protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
            .SetCombo(CombatType.PvE, "RotationSelection", 1, "Select which Opener to use.", "With Medicine", "Without Medicine")
            .SetInt(CombatType.PvE, "KenkiwastePrevent", 50, "Use Kenki above.", min: 0, max: 85, speed: 5);
        #endregion

        #region Phase Logic
        #region Phase Related Properties
        private int Openerstep { get; set; }
        private bool OpenerHasFinished { get; set; }
        private bool OpenerHasFailed { get; set; }
        private bool OpenerActionsAvailable { get; set; }
        private bool OpenerInProgress { get; set; }
        private bool CooldownPhaseInProgressOdd { get; set; }
        private bool CooldownPhaseInProgressEven { get; set; }
        private int EvenCooldownPhasestep { get; set; }
        private int OddCooldownPhasestep { get; set; }
        private int OddFillerPhasestep { get; set; }
        private int EvenFillerPhasestep { get; set; }
        private bool CooldownPhaseEvenHasFinished { get; set; }
        private bool CooldownPhaseEvenHasFailed { get; set; }
        private bool CooldownPhaseOddHasFinished { get; set; }
        private bool CooldownPhaseOddHasFailed { get; set; }
        private bool OddMinuteBurstPhaseInProgress { get; set; }
        private int OddMinuteBurstPhasestep { get; set; }
        private bool OddMinuteBurstPhaseHasFinished { get; set; }
        private bool OddMinuteBurstPhaseHasFailed { get; set; }
        private bool EvenMinuteBurstPhaseInProgress { get; set; }
        private int EvenMinuteBurstPhasestep { get; set; }
        private bool EvenMinuteBurstPhaseHasFinished { get; set; }
        private bool EvenMinuteBurstPhaseHasFailed { get; set; }
        private bool OddFillerPhaseInProgress { get; set; }
        private bool EvenFillerPhaseInProgress { get; set; }
        private bool OddFillerPhaseHasFinished { get; set; }
        private bool OddFillerPhaseHasFailed { get; set; }
        private bool EvenFillerPhaseHasFinished { get; set; }
        private bool EvenFillerPhaseHasFailed { get; set; }
        private bool NoPhaseInProgess { get; set; }
        private bool OddBurstReady { get; set; }
        private bool EvenBurstReady { get; set; }
        private bool Flag { get; set; }
        private static BaseAction FakeSenei { get; } = new(ActionID.Ikishoten);
        private float SeneiCool => Ikishoten.IsCoolingDown ? 120 - FakeSenei.RecastTimeElapsed : 0;
        internal static bool inOpener = false;
        internal static bool inOddFiller = false;
        internal static bool inEvenFiller = false;
        internal static bool nonOpener = true;
        internal static bool hasDied = false;
        internal static bool fillerComplete = false;
        internal static bool fastFillerReady = false;
        //private static SAMGauge gauge { get; set; }
        private bool IsThereABoss = HostileTargets.Any(p => p.IsBossFromTTK()) || HostileTargets.Any(p => p.IsBossFromIcon());

        public static float GetDebuffRemainingTime(StatusID effectId)
        {
            // Define a small time step (e.g., 0.1 seconds)
            const float timeStep = 0.1f;

            // Maximum time to iterate (adjust based on expected effect duration)
            const float maxTime = 120.0f; // Adjust this value as needed

            // Iterate by time steps until WillStatusEnd returns true or maxTime is reached
            float currentTime = 0.0f;
            while (currentTime < maxTime && !HostileTarget.WillStatusEnd(currentTime, true, effectId))
            {
                currentTime += timeStep;
            }

            // If WillStatusEnd never returned true, reached maxTime
            if (currentTime >= maxTime)
            {
                return 0.0f; // Indicate effect might be permanent or not applied
            }

            // Return the closest whole second (rounded down)
            return currentTime;
        }
        #endregion

        #region Countdown Logic
        protected override IAction CountDownAction(float remainTime)
        {
            //var IsThereABoss = HostileTargets.Any(p => p.IsBossFromTTK()) || HostileTargets.Any(p => p.IsBossFromIcon());
            if (remainTime < 0.1f && IsThereABoss)
            {
                Flag = false;
                OpenerHasFailed = false;
                OpenerHasFinished = false;
                Openerstep = 0;
                OpenerInProgress = true;

            }

            if (remainTime < 0.2f)
            {
                if (Gekko.CanUse(out _, CanUseOption.MustUseEmpty)) return Gekko;
            }


            if (remainTime < 5f)
            {
                if (TrueNorth.CanUse(out _, CanUseOption.IgnoreClippingCheck)) return TrueNorth;
            }

            if (remainTime < 9f)
            {
                if (MeikyoShisui.CanUse(out _, CanUseOption.IgnoreClippingCheck)) return MeikyoShisui;
            }



            return base.CountDownAction(remainTime);
        }
        #endregion

        #region Opener Logic
        private bool Opener(out IAction act)
        {
            act = default(IAction);

            while (OpenerInProgress)
            {
                if (TimeSinceLastAction.TotalSeconds > 3.5f)
                {
                    OpenerInProgress = false;
                    Openerstep = 0;
                }
                if (Player.IsDead)
                {
                    OpenerInProgress = false;
                    Openerstep = 0;
                }
                switch (Configs.GetCombo("RotationSelection"))
                {
                    case 0:
                        switch (Openerstep)
                        {
                            case 0:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Gekko.ID, Gekko.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 1:
                                return OpenerStep(TinctureOfStrength8.IsCoolingDown, TinctureOfStrength8.Use());
                            case 2:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Kasha.ID, Kasha.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 3:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Ikishoten.ID, Ikishoten.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 4:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Yukikaze.ID, Yukikaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 5:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == MidareSetsugekka.ID, MidareSetsugekka.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 6:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == HissatsuSenei.ID, HissatsuSenei.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 7:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == KaeshiSetsugekka.ID, KaeshiSetsugekka.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 8:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == MeikyoShisui.ID, MeikyoShisui.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 9:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Gekko.ID, Gekko.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 10:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == HissatsuShinten.ID, HissatsuShinten.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 11:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Higanbana.ID, Higanbana.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 12:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == HissatsuShinten.ID, HissatsuShinten.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 13:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == OgiNamikiri.ID, OgiNamikiri.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 14:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Shoha.ID, Shoha.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 15:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == KaeshiNamikiri.ID, KaeshiNamikiri.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 16:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Kasha.ID, Kasha.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 17:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == HissatsuShinten.ID, HissatsuShinten.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 18:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Gekko.ID, Gekko.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 19:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == HissatsuGyoten.ID, HissatsuGyoten.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 20:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 21:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Yukikaze.ID, Yukikaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 22:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == HissatsuShinten.ID, HissatsuShinten.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 23:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == MidareSetsugekka.ID, MidareSetsugekka.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 24:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == KaeshiSetsugekka.ID, KaeshiSetsugekka.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 25:
                                OpenerHasFinished = true;
                                OpenerInProgress = false;
                                //EvenCooldownPhasestep = 0;
                                //CooldownPhaseInProgressEven = true;

                                break;
                        }
                        break;
                    case 1:
                        switch (Openerstep)
                        {
                            case 0:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Gekko.ID, Gekko.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 1:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Kasha.ID, Kasha.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 2:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Ikishoten.ID, Ikishoten.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 3:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Yukikaze.ID, Yukikaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 4:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == MidareSetsugekka.ID, MidareSetsugekka.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 5:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == HissatsuSenei.ID, HissatsuSenei.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 6:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == KaeshiSetsugekka.ID, KaeshiSetsugekka.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 7:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == MeikyoShisui.ID, MeikyoShisui.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 8:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Gekko.ID, Gekko.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 9:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == HissatsuShinten.ID, HissatsuShinten.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 10:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Higanbana.ID, Higanbana.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 11:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == HissatsuShinten.ID, HissatsuShinten.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 12:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == OgiNamikiri.ID, OgiNamikiri.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 13:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Shoha.ID, Shoha.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 14:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == KaeshiNamikiri.ID, KaeshiNamikiri.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 15:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Kasha.ID, Kasha.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 16:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == HissatsuShinten.ID, HissatsuShinten.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 17:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Gekko.ID, Gekko.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 18:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == HissatsuGyoten.ID, HissatsuGyoten.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 19:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 20:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Yukikaze.ID, Yukikaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 21:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == HissatsuShinten.ID, HissatsuShinten.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 22:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == MidareSetsugekka.ID, MidareSetsugekka.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 23:
                                return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == KaeshiSetsugekka.ID, KaeshiSetsugekka.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 24:
                                OpenerHasFinished = true;
                                OpenerInProgress = false;
                                //EvenCooldownPhasestep = 0;
                                //CooldownPhaseInProgressEven = true;

                                break;
                        }
                        break;
                }
            }
            act = null;
            return false;
        }
        private bool OpenerStep(bool condition, bool result)
        {
            if (condition)
            {
                Openerstep++;
                return false;
            }
            return result;
        }
        #endregion

        #region Cooldown Phase Logic
        private unsafe bool CoolddownPhaseOdd(out IAction act)
        {
            act = default(IAction);

            while (CooldownPhaseInProgressOdd)
            {
                if (TimeSinceLastAction.TotalSeconds > 3.5f || Player.IsDead)
                {
                    CooldownPhaseInProgressOdd = false;
                    OddCooldownPhasestep = 0;
                }

                switch (OddCooldownPhasestep)
                {
                    case 0:
                        return CooldownPhaseStepOdd(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 1:
                        return CooldownPhaseStepOdd(RecordActions?.FirstOrDefault().Action.RowId == Yukikaze.ID, Yukikaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 2:
                        return CooldownPhaseStepOdd(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 3:
                        return CooldownPhaseStepOdd(RecordActions?.FirstOrDefault().Action.RowId == Jinpu.ID, Jinpu.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 4:
                        return CooldownPhaseStepOdd(RecordActions?.FirstOrDefault().Action.RowId == Gekko.ID, Gekko.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 5:
                        return CooldownPhaseStepOdd(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 6:
                        return CooldownPhaseStepOdd(RecordActions?.FirstOrDefault().Action.RowId == Shifu.ID, Shifu.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 7:
                        return CooldownPhaseStepOdd(RecordActions?.FirstOrDefault().Action.RowId == Kasha.ID, Kasha.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 8:
                        return CooldownPhaseStepOdd(RecordActions?.FirstOrDefault().Action.RowId == MidareSetsugekka.ID, MidareSetsugekka.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 9:
                        return CooldownPhaseStepOdd(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 10:
                        return CooldownPhaseStepOdd(RecordActions?.FirstOrDefault().Action.RowId == Yukikaze.ID, Yukikaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 11:
                        return CooldownPhaseStepOdd(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 12:
                        return CooldownPhaseStepOdd(RecordActions?.FirstOrDefault().Action.RowId == Jinpu.ID, Jinpu.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 13:
                        return CooldownPhaseStepOdd(RecordActions?.FirstOrDefault().Action.RowId == Gekko.ID, Gekko.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 14:
                        return CooldownPhaseStepOdd(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 15:
                        return CooldownPhaseStepOdd(RecordActions?.FirstOrDefault().Action.RowId == Shifu.ID, Shifu.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 16:
                        return CooldownPhaseStepOdd(RecordActions?.FirstOrDefault().Action.RowId == Kasha.ID, Kasha.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 17:
                        CooldownPhaseOddHasFinished = true;
                        CooldownPhaseInProgressOdd = false;
                        EvenMinuteBurstPhasestep = 0;
                        EvenMinuteBurstPhaseInProgress = true;

                        break;
                }
            }
            act = null;
            return false;
        }

        private bool CooldownPhaseStepOdd(bool condition, bool result)
        {
            if (condition)
            {
                OddCooldownPhasestep++;
                return false;
            }
            return result;
        }

        private unsafe bool CoolddownPhaseEven(out IAction act)
        {
            act = default(IAction);

            while (CooldownPhaseInProgressEven)
            {
                if (TimeSinceLastAction.TotalSeconds > 3.5f || Player.IsDead)
                {
                    CooldownPhaseInProgressEven = false;
                    EvenCooldownPhasestep = 0;
                }
                switch (EvenCooldownPhasestep)
                {
                    case 0:
                        return CooldownPhaseStepEven(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 1:
                        return CooldownPhaseStepEven(RecordActions?.FirstOrDefault().Action.RowId == Yukikaze.ID, Yukikaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 2:
                        return CooldownPhaseStepEven(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 3:
                        return CooldownPhaseStepEven(RecordActions?.FirstOrDefault().Action.RowId == Jinpu.ID, Jinpu.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 4:
                        return CooldownPhaseStepEven(RecordActions?.FirstOrDefault().Action.RowId == Gekko.ID, Gekko.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 5:
                        return CooldownPhaseStepEven(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 6:
                        return CooldownPhaseStepEven(RecordActions?.FirstOrDefault().Action.RowId == Shifu.ID, Shifu.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 7:
                        return CooldownPhaseStepEven(RecordActions?.FirstOrDefault().Action.RowId == Kasha.ID, Kasha.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 8:
                        return CooldownPhaseStepEven(RecordActions?.FirstOrDefault().Action.RowId == MidareSetsugekka.ID, MidareSetsugekka.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 9:
                        return CooldownPhaseStepEven(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 10:
                        return CooldownPhaseStepEven(RecordActions?.FirstOrDefault().Action.RowId == Yukikaze.ID, Yukikaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 11:
                        return CooldownPhaseStepEven(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 12:
                        return CooldownPhaseStepEven(RecordActions?.FirstOrDefault().Action.RowId == Jinpu.ID, Jinpu.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 13:
                        return CooldownPhaseStepEven(RecordActions?.FirstOrDefault().Action.RowId == Gekko.ID, Gekko.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 14:
                        return CooldownPhaseStepEven(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 15:
                        return CooldownPhaseStepEven(RecordActions?.FirstOrDefault().Action.RowId == Shifu.ID, Shifu.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 16:
                        return CooldownPhaseStepEven(RecordActions?.FirstOrDefault().Action.RowId == Kasha.ID, Kasha.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 17:
                        CooldownPhaseInProgressEven = false;
                        OddMinuteBurstPhasestep = 0;
                        OddMinuteBurstPhaseInProgress = true;

                        break;
                }
            }
            act = null;
            return false;
        }
        private bool CooldownPhaseStepEven(bool condition, bool result)
        {
            if (condition)
            {
                EvenCooldownPhasestep++;
                return false;
            }
            return result;
        }
        #endregion

        #region Burst Phase Logic
        private unsafe bool OddMinuteBurstPhase(out IAction act)
        {
            act = default(IAction);

            while (OddMinuteBurstPhaseInProgress)
            {
                if (TimeSinceLastAction.TotalSeconds > 3.5f || Player.IsDead)
                {
                    OddMinuteBurstPhaseInProgress = false;
                    OddMinuteBurstPhasestep = 0;
                }
                switch (OddMinuteBurstPhasestep)
                {
                    case 0:
                        return OddminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == MidareSetsugekka.ID, MidareSetsugekka.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 1:
                        return OddminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == KaeshiSetsugekka.ID, KaeshiSetsugekka.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 2:
                        return OddminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == MeikyoShisui.ID, MeikyoShisui.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 3:
                        return OddminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Gekko.ID, Gekko.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 4:
                        return OddminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Higanbana.ID, Higanbana.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 5:
                        return OddminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Gekko.ID, Gekko.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 6:
                        return OddminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Kasha.ID, Kasha.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 7:
                        return OddminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 8:
                        return OddminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Yukikaze.ID, Yukikaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 9:
                        return OddminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == MidareSetsugekka.ID, MidareSetsugekka.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 10:
                        OddMinuteBurstPhaseHasFinished = true;
                        OddMinuteBurstPhaseInProgress = false;
                        OddFillerPhasestep = 0;
                        OddFillerPhaseInProgress = true;
                        break;

                }

            }
            act = null;
            return false;
        }

        private bool OddminutePhaseStep(bool condition, bool result)
        {
            if (condition)
            {
                OddMinuteBurstPhasestep++;
                return false;
            }
            return result;
        }

        private bool EvenMinuteBurstPhase(out IAction act)
        {
            act = default(IAction);

            while (EvenMinuteBurstPhaseInProgress)
            {
                if (TimeSinceLastAction.TotalSeconds > 3.5f || Player.IsDead)
                {
                    EvenMinuteBurstPhaseInProgress = false;
                    EvenMinuteBurstPhasestep = 0;
                }
                switch (EvenMinuteBurstPhasestep)
                {
                    case 0:
                        return EvenminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == MidareSetsugekka.ID, MidareSetsugekka.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 1:
                        return EvenminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == HissatsuSenei.ID, HissatsuSenei.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 2:
                        return EvenminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == KaeshiSetsugekka.ID, KaeshiSetsugekka.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 3:
                        return EvenminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == MeikyoShisui.ID, MeikyoShisui.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 4:
                        return EvenminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Gekko.ID, Gekko.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 5:
                        return EvenminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Ikishoten.ID, Ikishoten.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 6:
                        return EvenminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Higanbana.ID, Higanbana.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 7:
                        return EvenminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == OgiNamikiri.ID, OgiNamikiri.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 8:
                        return EvenminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == KaeshiNamikiri.ID, KaeshiNamikiri.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 9:
                        return EvenminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Kasha.ID, Kasha.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 10:
                        return EvenminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Gekko.ID, Gekko.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 11:
                        return EvenminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 12:
                        return EvenminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Yukikaze.ID, Yukikaze.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 13:
                        return EvenminutePhaseStep(RecordActions?.FirstOrDefault().Action.RowId == MidareSetsugekka.ID, MidareSetsugekka.CanUse(out act, CanUseOption.MustUseEmpty));
                    case 14:
                        EvenMinuteBurstPhaseInProgress = false;
                        EvenFillerPhasestep = 0;
                        EvenFillerPhaseInProgress = true;
                        break;

                }

            }
            act = null;
            return false;
        }
        private bool EvenminutePhaseStep(bool condition, bool result)
        {
            if (condition)
            {
                EvenMinuteBurstPhasestep++;
                return false;
            }
            return result;
        }
        #endregion

        #region Filler Phase Logic
        private unsafe bool OddFillerPhase(out IAction act)
        {
            act = default(IAction);
            var aState = FFXIVClientStructs.FFXIV.Client.Game.UI.UIState.Instance()->PlayerState;
            var SkillSpeed = aState.Attributes[45];
            var switchselect = SkillSpeed <= 648 ? 0 : SkillSpeed <= 1174 ? 1 : 2;

            while (OddFillerPhaseInProgress && (!OddFillerPhaseHasFinished || !OddFillerPhaseHasFailed))
            {
                if (TimeSinceLastAction.TotalSeconds > 3.5f || Player.IsDead)
                {
                    OddFillerPhaseHasFailed = true;
                    OddFillerPhaseInProgress = false;
                    OddFillerPhasestep = 0;
                }
                switch (switchselect)
                {
                    case 0:
                        switch (OddFillerPhasestep)
                        {
                            case 0:
                                return OddFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 1:
                                return OddFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Yukikaze.ID, Yukikaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 2:
                                return OddFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Hagakure.ID, Hagakure.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 3:
                                OddFillerPhaseHasFinished = true;
                                OddFillerPhaseInProgress = false;
                                //OddCooldownPhasestep = 0;
                                //CooldownPhaseInProgressOdd = true;
                                break;

                        }
                        break;
                    case 1:
                        switch (OddFillerPhasestep)
                        {
                            case 0:
                                return OddFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 1:
                                return OddFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Jinpu.ID, Jinpu.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 2:
                                return OddFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Gekko.ID, Gekko.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 3:
                                return OddFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Hagakure.ID, Hagakure.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 4:
                                OddFillerPhaseHasFinished = true;
                                OddFillerPhaseInProgress = false;
                                //OddCooldownPhasestep = 0;
                                //CooldownPhaseInProgressOdd = true;
                                break;

                        }
                        break;
                    case 2:
                        switch (OddFillerPhasestep)
                        {
                            case 0:
                                return OddFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 1:
                                return OddFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Yukikaze.ID, Yukikaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 2:
                                return OddFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Hagakure.ID, Hagakure.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 3:
                                return OddFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 4:
                                return OddFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Yukikaze.ID, Yukikaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 5:
                                return OddFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Hagakure.ID, Hagakure.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 6:
                                OddFillerPhaseHasFinished = true;
                                OddFillerPhaseInProgress = false;
                                //OddCooldownPhasestep = 0;
                                //CooldownPhaseInProgressOdd = true;
                                break;

                        }
                        break;
                }

            }
            act = null;
            return false;
        }
        private bool OddFillerPhaseStep(bool condition, bool result)
        {
            if (condition)
            {
                OddFillerPhasestep++;
                return false;
            }
            return result;
        }

        private unsafe bool EvenFillerPhase(out IAction act)
        {
            act = default(IAction);
            var aState = FFXIVClientStructs.FFXIV.Client.Game.UI.UIState.Instance()->PlayerState;
            var SkillSpeed = aState.Attributes[45];
            var switchselect = SkillSpeed <= 648 ? 0 : SkillSpeed <= 1174 ? 1 : 2;

            while (EvenFillerPhaseInProgress && (!EvenFillerPhaseHasFinished || !EvenFillerPhaseHasFailed))
            {
                if (TimeSinceLastAction.TotalSeconds > 3.5f || Player.IsDead)
                {
                    EvenFillerPhaseHasFailed = true;
                    EvenFillerPhaseInProgress = false;
                    EvenFillerPhasestep = 0;
                }
                switch (switchselect)
                {
                    case 0:
                        switch (EvenFillerPhasestep)
                        {
                            case 0:
                                return EvenFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 1:
                                return EvenFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Yukikaze.ID, Yukikaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 2:
                                return EvenFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Hagakure.ID, Hagakure.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 3:
                                EvenFillerPhaseHasFinished = true;
                                EvenFillerPhaseInProgress = false;
                                //EvenCooldownPhasestep = 0;
                                //CooldownPhaseInProgressEven = true;
                                break;

                        }
                        break;
                    case 1:
                        switch (EvenFillerPhasestep)
                        {
                            case 0:
                                return EvenFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 1:
                                return EvenFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Jinpu.ID, Jinpu.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 2:
                                return EvenFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Gekko.ID, Gekko.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 3:
                                return EvenFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Hagakure.ID, Hagakure.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 4:
                                EvenFillerPhaseHasFinished = true;
                                EvenFillerPhaseInProgress = false;
                                //EvenCooldownPhasestep = 0;
                                //CooldownPhaseInProgressEven = true;
                                break;

                        }
                        break;
                    case 2:
                        switch (EvenFillerPhasestep)
                        {
                            case 0:
                                return EvenFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 1:
                                return EvenFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Yukikaze.ID, Yukikaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 2:
                                return EvenFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Hagakure.ID, Hagakure.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 3:
                                return EvenFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Hakaze.ID, Hakaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 4:
                                return EvenFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Yukikaze.ID, Yukikaze.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 5:
                                return EvenFillerPhaseStep(RecordActions?.FirstOrDefault().Action.RowId == Hagakure.ID, Hagakure.CanUse(out act, CanUseOption.MustUseEmpty));
                            case 6:
                                EvenFillerPhaseHasFinished = true;
                                EvenFillerPhaseInProgress = false;
                                //EvenCooldownPhasestep = 0;
                                //CooldownPhaseInProgressEven = true;
                                break;

                        }
                        break;
                }

            }
            act = null;
            return false;
        }

        private bool EvenFillerPhaseStep(bool condition, bool result)
        {
            if (condition)
            {
                EvenFillerPhasestep++;
                return false;
            }
            return result;
        }
        #endregion

        #endregion

        protected unsafe override bool GeneralGCD(out IAction act)
        {

            act = null;
            var aState = FFXIVClientStructs.FFXIV.Client.Game.UI.UIState.Instance()->PlayerState;
            var SkillSpeed = aState.Attributes[45];
            uint SamFillerCombo = SkillSpeed <= 648 ? 2u : 3u;
            ActionID Adjsuted_Keishi = AdjustId(ActionID.TsubameGaeshi);
            ActionID Adjsuted_Namikiri = AdjustId(ActionID.OgiNamikiri);
            var IsTargetBoss = HostileTarget?.IsBossFromTTK() ?? false;
            var IsTargetDying = HostileTarget?.IsDying() ?? false;
            int GCDsUntilBurst(uint gcdCount = 0u, float offset = 0f)
            {
                int gcdsChecked = 0;
                while (!HissatsuSenei.WillHaveOneChargeGCD(gcdCount, offset))
                {
                    gcdCount++;
                    gcdsChecked++;
                }
                return gcdsChecked;  // Represents how many GCDs until a charge is available
            }

            #region PvP
            #endregion
            #region PvEPhases
            if (OpenerInProgress)
            {
                return Opener(out act);
            }
            if (EvenMinuteBurstPhaseInProgress)
            {
                return EvenMinuteBurstPhase(out act);
            }
            if (OddMinuteBurstPhaseInProgress)
            {
                return OddMinuteBurstPhase(out act);
            }
            if (CooldownPhaseInProgressOdd)
            {
                return CoolddownPhaseOdd(out act);
            }
            if (CooldownPhaseInProgressEven)
            {
                return CoolddownPhaseEven(out act);
            }
            if (EvenFillerPhaseInProgress)
            {
                return EvenFillerPhase(out act);
            }
            if (OddFillerPhaseInProgress)
            {
                return OddFillerPhase(out act);
            }
            /*
            if (IsThereABoss && Player.Level == 90)
            {
                //what of not in any phase => need to realign to proper phase
                if (!OpenerInProgress && !EvenMinuteBurstPhaseInProgress && !OddMinuteBurstPhaseInProgress && !CooldownPhaseInProgressOdd && !CooldownPhaseInProgressEven && !EvenFillerPhaseInProgress && !OddFillerPhaseInProgress && InCombat)
                {
                    NoPhaseInProgess = true;
                }

                if (NoPhaseInProgess) //problem here because we aren't attacking if phase not defined
                {
                    //lets align to burst its better that way, even burst has priority
                    if (HissatsuSenei.WillHaveOneChargeGCD(2) || !HissatsuSenei.IsCoolingDown)
                    {
                        //even burst nned to eat sen to realign higabana
                        if (SenCount == 3)
                        {
                            EvenMinuteBurstPhasestep = 0;
                            EvenMinuteBurstPhaseInProgress = true;
                        }
                        if (SenCount == 2 && HasSetsu && HasGetsu)
                        {
                            OddCooldownPhasestep= 14;
                            CooldownPhaseInProgressOdd= true;
                        }
                        if(SenCount == 1 && HasSetsu)
                        {
                            OddCooldownPhasestep = 11;
                            CooldownPhaseInProgressOdd = true;
                        }
                        OddCooldownPhasestep = 9;
                        CooldownPhaseInProgressOdd = true;

                    }
                    if (HissatsuSenei.WillHaveOneChargeGCD(19))
                    {
                        //odd cooldown
                        if (HissatsuSenei.WillHaveOneChargeGCD(10))
                        {
                            if (SenCount == 2 && HasSetsu && HasGetsu)
                            {
                                OddCooldownPhasestep = 14;
                                CooldownPhaseInProgressOdd = true;
                            }
                            if (SenCount == 1 && HasSetsu)
                            {
                                OddCooldownPhasestep = 11;
                                CooldownPhaseInProgressOdd = true;
                            }
                            OddCooldownPhasestep = 9;
                            CooldownPhaseInProgressOdd = true;
                        }
                        if (SenCount == 3)
                        {
                            EvenMinuteBurstPhasestep = 8;
                            EvenMinuteBurstPhaseInProgress = true;
                        }
                        if (SenCount == 2 && HasSetsu && HasGetsu)
                        {
                            OddCooldownPhasestep = 5;
                            CooldownPhaseInProgressOdd = true;
                        }
                        if (SenCount == 1 && HasSetsu)
                        {
                            OddCooldownPhasestep = 2;
                            CooldownPhaseInProgressOdd = true;
                        }
                        OddCooldownPhasestep = 0;
                        CooldownPhaseInProgressOdd = true;
                    }
                    if (HissatsuSenei.WillHaveOneChargeGCD(19+SamFillerCombo))
                    {
                        //odd filler
                        OddFillerPhasestep = 0;
                        OddFillerPhaseInProgress = true;
                    }
                    if (HissatsuSenei.WillHaveOneChargeGCD(28 + SamFillerCombo))
                    {
                        //odd burst
                        if (SenCount == 3)
                        {
                            OddMinuteBurstPhasestep = 0;
                            OddMinuteBurstPhaseInProgress = true;
                        }
                        if (SenCount == 2 && HasSetsu && HasGetsu)
                        {
                            EvenCooldownPhasestep = 14;
                            CooldownPhaseInProgressEven = true;
                        }
                        if (SenCount == 1 && HasSetsu)
                        {
                            EvenCooldownPhasestep = 11;
                            CooldownPhaseInProgressEven = true;
                        }
                        EvenCooldownPhasestep = 9;
                        CooldownPhaseInProgressEven = true;
                    }
                    if (HissatsuSenei.WillHaveOneChargeGCD(45 + SamFillerCombo))
                    {
                        //even cooldown
                        if (HissatsuSenei.WillHaveOneChargeGCD(36 + SamFillerCombo))
                        {
                            if (SenCount == 2 && HasSetsu && HasGetsu)
                            {
                                EvenCooldownPhasestep = 14;
                                CooldownPhaseInProgressEven = true;
                            }
                            if (SenCount == 1 && HasSetsu)
                            {
                                EvenCooldownPhasestep = 11;
                                CooldownPhaseInProgressEven = true;
                            }
                            EvenCooldownPhasestep = 9;
                            CooldownPhaseInProgressEven = true;
                        }
                        if (SenCount == 3)
                        {
                            EvenMinuteBurstPhasestep = 8;
                            EvenMinuteBurstPhaseInProgress = true;
                        }
                        if (SenCount == 2 && HasSetsu && HasGetsu)
                        {
                            EvenCooldownPhasestep = 5;
                            CooldownPhaseInProgressEven = true;
                        }
                        if (SenCount == 1 && HasSetsu)
                        {
                            EvenCooldownPhasestep = 2;
                            CooldownPhaseInProgressEven = true;
                        }
                        EvenCooldownPhasestep = 0;
                        CooldownPhaseInProgressEven = true;
                    }
                    if (HissatsuSenei.WillHaveOneChargeGCD(45 + 2*SamFillerCombo))
                    {
                        //even filler
                        EvenFillerPhasestep = 0;
                        EvenFillerPhaseInProgress = true;
                    }
                    if (HissatsuSenei.WillHaveOneChargeGCD(54 + 2 * SamFillerCombo))
                    {
                        //even burst but after Senei used
                        EvenFillerPhasestep = 0;
                        EvenFillerPhaseInProgress = true;
                    }
                    EvenFillerPhasestep = 0;
                    EvenFillerPhaseInProgress = true;
                }
            }
            
            
            */
            #endregion





            var meikyoBuff = Player.HasStatus(true, StatusID.MeikyoShisui);
            var ogiready = Player.HasStatus(true, StatusID.OgiNamikiriReady);
            var ogitime = Player.WillStatusEnd(5, true, StatusID.OgiNamikiriReady);
            bool inOddFiller = false;
            bool inEvenFiller = false;
            bool fillerComplete = false;
            bool fastFillerReady = false;
            bool hasDied = false;
            //bool evenMinute = CombatTime % 2 == 1;
            //bool oddMinute = CombatTime % 2 == 0;
            var meikyostacks = Player.StatusStack(true, StatusID.MeikyoShisui);
            var oneSeal = SenCount == 1;
            var twoSeal = SenCount == 2;
            var threeSeal = SenCount == 3;
            if (Player.HasStatus(true, StatusID.Weakness))
                hasDied = true;
            var SamAOEKenkiOvercapAmount = Configs.GetInt("KenkiwastePrevent");

            bool openerReady = MeikyoShisui.CurrentCharges == 1 && !HissatsuSenei.IsCoolingDown && !Ikishoten.IsCoolingDown && TsubameGaeshi.CurrentCharges == 2;


            #region PvEFreestyle
            #region AOE
            if (NumberOfAllHostilesInRange >= 3)
            {
                if (OgiNamikiri.EnoughLevel)
                {


                    if (KaeshiNamikiri.CanUse(out act, CanUseOption.MustUse)) return true;
                    if (OgiNamikiri.CanUse(out act, CanUseOption.MustUse)) return true;

                }

                if (TenkaGoken.EnoughLevel)
                {
                    if (!IsMoving)
                    {
                        if (TenkaGoken.CanUse(out act, CanUseOption.MustUse)) return true;
                        if (MidareSetsugekka.CanUse(out act, CanUseOption.MustUse)) return true;
                    }

                    if (TsubameGaeshi.EnoughLevel && TsubameGaeshi.CurrentCharges > 0)
                    {
                        if (KaeshiGoken.CanUse(out act, CanUseOption.MustUse)) return true;
                    }
                }

                if (meikyoBuff)
                {
                    if ((HasGetsu == false && HasFlower) || !HasMoon)
                    {
                        if (Mangetsu.CanUse(out act, CanUseOption.MustUse)) return true;
                    }

                    if ((HasKa == false && HasMoon) || !HasFlower)
                    {
                        if (Oka.CanUse(out act, CanUseOption.MustUse)) return true;
                    }
                }

                if (ActionManager.Instance()->Combo.Timer > 0)
                {
                    if (Mangetsu.EnoughLevel && (ActionManager.Instance()->Combo.Action == Fuko.ID || ActionManager.Instance()->Combo.Action == Fuga.ID))
                    {
                        if (HasGetsu == false || IsMoonTimeLessThanFlower || !HasMoon)
                        {
                            if (Mangetsu.CanUse(out act, CanUseOption.MustUse)) return true;
                        }

                        if (Oka.EnoughLevel && (HasKa == false || !IsMoonTimeLessThanFlower || !HasFlower))
                        {
                            if (Oka.CanUse(out act, CanUseOption.MustUse)) return true;
                        }
                    }
                }

                if (!Oka.EnoughLevel && Kasha.EnoughLevel)
                {

                    if (Kasha.CanUse(out act, CanUseOption.MustUse)) return true;
                    if (Shifu.CanUse(out act, CanUseOption.MustUse)) return true;
                    if (HasKa == false || !IsMoonTimeLessThanFlower || !HasFlower && Hakaze.EnoughLevel)
                    {
                        if (Hakaze.CanUse(out act, CanUseOption.MustUse)) return true;
                    }
                }

                if (Fuko.CanUse(out act)) return true;
                if (!Fuko.EnoughLevel && Fuga.CanUse(out act)) return true;
            }
            #endregion

            #region SingleTarget
            //filler stuff here needs more testing
            if (!hasDied && OgiNamikiri.EnoughLevel && CombatTime > 60)
            {
                bool oddMinute = SenCount == 0 && SeneiCool < 40 && SeneiCool > 20 && HostileTarget && HostileTarget.HasStatus(true, StatusID.Higanbana);
                bool evenMinute = SenCount == 0 && SeneiCool < 100 && SeneiCool > 80 && HostileTarget && HostileTarget.HasStatus(true,StatusID.Higanbana);
                bool fillerfinished = RecordActions?.FirstOrDefault().Action.RowId == Hagakure.ID;
                bool fillerready = RecordActions?.FirstOrDefault().Action.RowId == MidareSetsugekka.ID;

                if (evenMinute && !fillerfinished && fillerready)
                {
                    EvenFillerPhasestep = 0;
                    EvenFillerPhaseInProgress = true;
                }

                if (oddMinute && !fillerfinished && fillerready) 
                {
                    OddFillerPhasestep = 0;
                    OddFillerPhaseInProgress = true;

                }
                
            }
            //end filler

            // higabana drift fixer
            if (Player.Level == 90 && CombatTime > 45 && meikyoBuff && SenCount == 1 && HostileTarget && (HostileTarget.WillStatusEnd(20,true, StatusID.Higanbana) || !HostileTarget.HasStatus(true, StatusID.Higanbana)))
            {
                if (Higanbana.CanUse(out act)) return true;
            }
            if (!InCombat)
            {
                hasDied = false;
                nonOpener = true;
                inOpener = false;

                if (OgiNamikiri.EnoughLevel)
                {
                    if ((IsLastAction(true, MeikyoShisui) || meikyoBuff) && openerReady)
                    {
                        if (!inOpener)
                            inOpener = true;
                        nonOpener = false;
                    }

                    if (inOpener)
                    {
                        if (meikyostacks == 3 && (oneSeal || twoSeal || threeSeal) && Hagakure.EnoughLevel)
                        {
                            if (Hagakure.CanUse(out act)) return true;
                        }
                    }
                }
                //Prep for Opener
                if (meikyoBuff && !MeikyoShisui.IsCoolingDown && SenCount == 0 && Gekko.EnoughLevel)
                    if (Gekko.CanUse(out act)) return true;

                //Stops waste if you use Iaijutsu or Ogi and you've got a Kaeshi ready
                if (!inOpener)
                {
                    if (KaeshiNamikiri.CanUse(out act, CanUseOption.MustUse)) return true;
                    if (OgiNamikiri.CanUse(out act, CanUseOption.MustUse)) return true;

                    if (TsubameGaeshi.EnoughLevel && TsubameGaeshi.CurrentCharges > 0)

                    {
                        if (KaeshiGoken.CanUse(out act, CanUseOption.MustUse)) return true;
                        if (KaeshiSetsugekka.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
                    }
                }
            }

            if (Enpi.EnoughLevel && !inEvenFiller && !inOddFiller && HostileTarget?.DistanceToPlayer() > 3)
                if (Enpi.CanUse(out act)) return true;


            if (InCombat)
            {
                if (inOpener && OgiNamikiri.EnoughLevel && !hasDied && !nonOpener)
                {


                    //GCDs
                    if ((twoSeal && ActionManager.Instance()->Combo.Action == Yukikaze.ID) ||
                        (threeSeal && (meikyostacks == 1 || !ogiready)) ||
                        (oneSeal && !HostileTarget.HasStatus(true, StatusID.Higanbana) && TsubameGaeshi.CurrentCharges == 1) && !IsTargetDying)
                    {
                        if (MidareSetsugekka.CanUse(out act)) return true;
                        if (TenkaGoken.CanUse(out act)) return true;
                        if (Higanbana.CanUse(out act)) return true;

                    }

                    if (KaeshiNamikiri.CanUse(out act, CanUseOption.MustUse)) return true;
                    if (OgiNamikiri.CanUse(out act, CanUseOption.MustUse)) return true;
                    if (KaeshiSetsugekka.CanUse(out act, CanUseOption.MustUse)) return true;
                    if (KaeshiGoken.CanUse(out act, CanUseOption.MustUse)) return true;

                    //1-2-3 Logic
                    if (ActionManager.Instance()->Combo.Action == Hakaze.ID)
                    { if (Yukikaze.CanUse(out act)) return true; }

                    if (twoSeal && MeditationStacks == 0 && HostileTarget.HasStatus(true, StatusID.Higanbana))
                    { if (Hakaze.CanUse(out act)) return true; }

                    if (meikyostacks == 3)
                    { if (Gekko.CanUse(out act)) return true; }

                    if (meikyostacks == 2 && !ogiready)
                    { if (Kasha.CanUse(out act)) return true; }

                    if (meikyostacks == 1)
                    {
                        if (Ikishoten.WillHaveOneCharge(110))
                        { if (Yukikaze.CanUse(out act)) return true; }

                        if (MeditationStacks == 0 || !ogiready)
                        { if (Gekko.CanUse(out act)) return true; }
                    }

                    if (TsubameGaeshi.CurrentCharges == 0)
                        inOpener = false;

                    if ((ActionManager.Instance()->Combo.Action == Yukikaze.ID && oneSeal) || (ActionManager.Instance()->Combo.Action == Hakaze.ID && (threeSeal || HasSetsu)) || CombatTime > 40)
                    {
                        inOpener = false;
                        nonOpener = true;
                    }
                }

                if (!inOpener)
                {


                    //Death desync check
                    if (Player.HasStatus(true, StatusID.Weakness))
                        hasDied = true;


                    //Filler Features
                    

                    //Meikyo Waste Protection (Stops waste during even minute windows)
                    if (meikyoBuff && Player.WillStatusEnd(6, true, StatusID.MeikyoShisui) && ogiready)
                    {
                        if (!HasGetsu && Gekko.EnoughLevel)
                        { if (Gekko.CanUse(out act)) return true; }

                        if (!HasKa && Kasha.EnoughLevel)
                        { if (Kasha.CanUse(out act)) return true; }

                        if (!HasSetsu && Yukikaze.EnoughLevel)
                        { if (Yukikaze.CanUse(out act)) return true; }
                    }
                    // Iaijutsu Features
                    if (Higanbana.EnoughLevel)
                    {
                        if (KaeshiSetsugekka.IsEnabled && TsubameGaeshi.EnoughLevel && TsubameGaeshi.CurrentCharges > 0)
                        { if (KaeshiSetsugekka.CanUse(out act, CanUseOption.MustUseEmpty)) return true; }

                        if (!IsMoving)
                        {
                            if (((oneSeal || (oneSeal && meikyostacks == 2)) && HostileTargets.Any(p => p.WillStatusEnd(10, true, StatusID.Higanbana)) && !IsTargetDying ||
                                (twoSeal && !MidareSetsugekka.EnoughLevel) ||
                                (threeSeal && MidareSetsugekka.EnoughLevel)))
                            {
                                if (MidareSetsugekka.CanUse(out act)) return true;
                                if (TenkaGoken.CanUse(out act)) return true;
                                if (Higanbana.CanUse(out act)) return true;

                            }
                        }
                    }

                    //Ogi Namikiri Features
                    if (OgiNamikiri.EnoughLevel)
                    {
                        if ((!IsMoving && ogiready) || KaeshiNamikiri.IsEnabled)
                        {
                            if (hasDied || nonOpener || (meikyostacks is 1 or 2 && !HostileTargets.Any(p => p.WillStatusEnd(45, true, StatusID.Higanbana)) && meikyoBuff) || Ikishoten.WillHaveOneCharge(105))
                            {
                                if (KaeshiNamikiri.CanUse(out act, CanUseOption.MustUse)) return true;
                                if (OgiNamikiri.CanUse(out act, CanUseOption.MustUse)) return true;
                            }

                        }
                    }


                }
            }

            if (!inOpener)
            {
                if (meikyoBuff)
                {
                    if (!HasMoon || (!HasGetsu && HasFlower))
                    { if (Gekko.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true; }

                    if (((!HasKa && HasMoon) || !HasFlower))
                    { if (Kasha.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true; }

                    if (!HasSetsu)
                    { if (Yukikaze.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true; }
                }

                if (ActionManager.Instance()->Combo.Timer > 0)
                {
                    if (ActionManager.Instance()->Combo.Action == Hakaze.ID && Jinpu.EnoughLevel)
                    {
                        if (!HasSetsu && Yukikaze.EnoughLevel && HasMoon && HasFlower)
                        { if (Yukikaze.CanUse(out act)) return true; }
                        //IsMoonTimeLessThanFlower
                        if ((!Kasha.EnoughLevel && ((IsMoonTimeLessThanFlower) || !HasMoon)) ||
                           (Kasha.EnoughLevel && (!HasMoon || (HasFlower && !HasGetsu) || (threeSeal && (IsMoonTimeLessThanFlower)))))
                        { if (Jinpu.CanUse(out act)) return true; }

                        if (Shifu.EnoughLevel &&
                            ((!Kasha.EnoughLevel && ((!IsMoonTimeLessThanFlower) || !HasFlower)) ||
                            (Kasha.EnoughLevel && (!HasFlower || (HasMoon && !HasKa) || (threeSeal && (!IsMoonTimeLessThanFlower))))))
                        { if (Shifu.CanUse(out act)) return true; }
                    }

                    if (ActionManager.Instance()->Combo.Action == Jinpu.ID && Gekko.EnoughLevel)
                    { if (Gekko.CanUse(out act)) return true; }

                    if (ActionManager.Instance()->Combo.Action == Shifu.ID && Kasha.EnoughLevel)
                    { if (Kasha.CanUse(out act)) return true; }
                }
            }


            if (Hakaze.CanUse(out act)) return true;

            #endregion

            #endregion









            /*
            bool oddMinute = HostileTargets.Any(p => p.WillStatusEnd(48, true, StatusID.Higanbana)) && !HostileTargets.Any(p => p.WillStatusEnd(51, true, StatusID.Higanbana) && !HissatsuSenei.WillHaveOneCharge(45));
            bool evenMinute = HostileTargets.Any(p => p.WillStatusEnd(44, true, StatusID.Higanbana)) && !HostileTargets.Any(p => p.WillStatusEnd(47, true, StatusID.Higanbana) && !HissatsuSenei.WillHaveOneCharge(85));
            if (HostileTargets.Any(p => p.HasStatus(true, StatusID.Higanbana)) && oddMinute || evenMinute)
            {
                if (SamFillerCombo == 2)
                {
                    if (RecordActions?.FirstOrDefault().Action.RowId == Hagakure.ID)
                        fillerComplete = true;
                    if (SenCount > 0)
                    { if (Hagakure.CanUse(out act)) return true; }
                    if (Yukikaze.CanUse(out act)) return true;
                    if (Hakaze.CanUse(out act)) return true;
                }

                if (SamFillerCombo == 3)
                {
                    if (RecordActions?.FirstOrDefault().Action.RowId == Hagakure.ID)
                        fillerComplete = true;
                    if (SenCount > 0)
                    { if (Hagakure.CanUse(out act)) return true; }
                    if (Gekko.CanUse(out act)) return true;
                    if (Jinpu.CanUse(out act)) return true;
                    if (Hakaze.CanUse(out act)) return true;
                }
            }
            if (KaeshiNamikiri.CanUse(out act, CanUseOption.MustUse)) return true;
            if (KaeshiGoken.CanUse(out act, CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo)) return true;
            if (KaeshiSetsugekka.CanUse(out act, CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo)) return true;
            if (ogitime && OgiNamikiri.CanUse(out act, CanUseOption.MustUse)) return true;
            if ((!IsTargetBoss || (HostileTarget?.HasStatus(true, StatusID.Higanbana) ?? false)) && HasMoon && HasFlower && OgiNamikiri.CanUse(out act, CanUseOption.MustUse)) return true;
            if (SenCount == 1 && IsTargetBoss && !IsTargetDying)
            {
                if (HasMoon && HasFlower && Higanbana.CanUse(out act)) return true;
            }
            if (SenCount == 2)
            {
                if (TenkaGoken.CanUse(out act, !MidareSetsugekka.EnoughLevel ? CanUseOption.MustUse : CanUseOption.None)) return true;
            }
            if (SenCount == 3)
            {
                if (MidareSetsugekka.CanUse(out act)) return true;
            }
            if ((!HasMoon || IsMoonTimeLessThanFlower || !Oka.EnoughLevel) && Mangetsu.CanUse(out act, meikyoBuff && !HasGetsu ? CanUseOption.EmptyOrSkipCombo : CanUseOption.None)) return true;
            if ((!HasFlower || !IsMoonTimeLessThanFlower) && Oka.CanUse(out act, meikyoBuff && !HasKa ? CanUseOption.EmptyOrSkipCombo : CanUseOption.None)) return true;
            if (!HasSetsu && Yukikaze.CanUse(out act, meikyoBuff && HasGetsu && HasKa && !HasSetsu ? CanUseOption.EmptyOrSkipCombo : CanUseOption.None)) return true;
            if (Gekko.CanUse(out act, meikyoBuff && !HasGetsu ? CanUseOption.EmptyOrSkipCombo : CanUseOption.None)) return true;
            if (Kasha.CanUse(out act, meikyoBuff && !HasKa ? CanUseOption.EmptyOrSkipCombo : CanUseOption.None)) return true;
            if ((!HasMoon || IsMoonTimeLessThanFlower || !Shifu.EnoughLevel) && Jinpu.CanUse(out act)) return true;
            if ((!HasFlower || !IsMoonTimeLessThanFlower) && Shifu.CanUse(out act)) return true;
            if (!meikyoBuff)
            {
                if (Fuko.CanUse(out act)) return true;
                if (!Fuko.EnoughLevel && Fuga.CanUse(out act)) return true;
                if (Hakaze.CanUse(out act)) return true;
                if (Enpi.CanUse(out act)) return true;
            }
            */
            return false;

        }

        protected unsafe override bool AttackAbility(out IAction act)
        {

            act = null;
            ActionID Adjsuted_Keishi = AdjustId(ActionID.TsubameGaeshi);
            ActionID Adjsuted_Namikiri = AdjustId(ActionID.OgiNamikiri);
            var IsTargetBoss = HostileTarget?.IsBossFromTTK() ?? false;
            var IsTargetDying = HostileTarget?.IsDying() ?? false;
            var meikyoBuff = Player.HasStatus(true, StatusID.MeikyoShisui);
            bool hasDied = false;
            //bool evenMinute = CombatTime % 2 == 1;
            //bool oddMinute = CombatTime % 2 == 0;
            var meikyostacks = Player.StatusStack(true, StatusID.MeikyoShisui);
            var oneSeal = SenCount == 1;
            var twoSeal = SenCount == 2;
            var threeSeal = SenCount == 3;
            if (Player.HasStatus(true, StatusID.Weakness))
                hasDied = true;
            if (Player.HasStatus(true, StatusID.Weakness))
                hasDied = true;

            #region PvEPhases

            if (OpenerInProgress)
            {
                return Opener(out act);
            }
            if (EvenMinuteBurstPhaseInProgress)
            {
                return EvenMinuteBurstPhase(out act);
            }
            if (OddMinuteBurstPhaseInProgress)
            {
                return OddMinuteBurstPhase(out act);
            }
            if (CooldownPhaseInProgressOdd)
            {
                return CoolddownPhaseOdd(out act);
            }
            if (CooldownPhaseInProgressEven)
            {
                return CoolddownPhaseEven(out act);
            }
            #endregion
            //oGCDs
            if (RecordActions?.FirstOrDefault().Action.RowId == KaeshiSetsugekka.ID)
            { if (MeikyoShisui.CanUse(out act, CanUseOption.MustUseEmpty)) return true; }
            //Senei Features
            if (Player.Level == 90 && Kenki >= 25 && !HissatsuSenei.IsCoolingDown && RecordActions?.FirstOrDefault().Action.RowId == MidareSetsugekka.ID)
            {
                if (HissatsuGuren.CanUse(out act)) return true;
                if (HissatsuSenei.CanUse(out act)) return true;
            }
            if (Player.Level != 90 && Kenki >= 25 && !HissatsuSenei.IsCoolingDown)
            {
                if (HissatsuShinten.EnoughLevel && !HissatsuSenei.EnoughLevel)
                {
                    if (HissatsuKyuten.CanUse(out act)) return true;
                    if (HissatsuShinten.CanUse(out act)) return true;
                }

                if (HissatsuSenei.EnoughLevel)
                {

                    if (hasDied || nonOpener || Ikishoten.WillHaveOneCharge(110) || ((KaeshiSetsugekka.IsEnabled || SenCount == 0) && HostileTargets.Any(p => p.WillStatusEnd(10, true, StatusID.Higanbana))))
                    {
                        if (HissatsuGuren.CanUse(out act)) return true;
                        if (HissatsuSenei.CanUse(out act)) return true;
                    }

                }
            }

            if (KaeshiNamikiri.IsEnabled && MeditationStacks == 3)
            {
                if (Shoha2.CanUse(out act)) return true;
                if (Shoha.CanUse(out act)) return true;
            }

            if (twoSeal && MeditationStacks == 0 && Ikishoten.WillHaveOneCharge(110) && Ikishoten.IsCoolingDown && !HissatsuSenei.WillHaveOneCharge(10))
            {
                if (Kenki >= 25)
                {
                    if (HissatsuKyuten.CanUse(out act)) return true;
                    if (HissatsuShinten.CanUse(out act)) return true;
                }

            }

            if (Kenki >= 25 && !HissatsuSenei.WillHaveOneCharge(10))
            {
                if (oneSeal && meikyostacks == 0)
                {
                    if (HissatsuKyuten.CanUse(out act)) return true;
                    if (HissatsuShinten.CanUse(out act)) return true;
                }

                if (Player.Level != 90 && meikyostacks == 1 && !HissatsuSenei.IsCoolingDown && (KaeshiSetsugekka.IsEnabled || SenCount == 0))
                {
                    if (HissatsuGuren.CanUse(out act)) return true;
                    if (HissatsuSenei.CanUse(out act)) return true;
                }
            }

            if (Player.Level < 76 && SenCount == 0 && meikyostacks == 1 && TsubameGaeshi.CurrentCharges == 1 && !meikyoBuff)
            { if (MeikyoShisui.CanUse(out act, CanUseOption.MustUseEmpty)) return true; }

            if (Kenki >= 25 && Shoha.IsCoolingDown && !HissatsuSenei.WillHaveOneCharge(10))
            {
                if (HissatsuKyuten.CanUse(out act)) return true;
                if (HissatsuShinten.CanUse(out act)) return true;
            }


            //Meikyo Features
            if (Player.Level < 76 && !meikyoBuff && HasHostilesInRange && IsLastGCD(true, Yukikaze, Mangetsu, Oka) &&
            (!IsTargetBoss || (HostileTarget?.HasStatus(true, StatusID.Higanbana) ?? false) && !(HostileTarget?.WillStatusEnd(40, true, StatusID.Higanbana) ?? false) || !HasMoon && !HasFlower || IsTargetBoss && IsTargetDying))
            {
                if (MeikyoShisui.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
            }

            

            if (HissatsuShinten.EnoughLevel && Kenki >= 25 && !HissatsuSenei.WillHaveOneCharge(10))
            {
                if (HissatsuSenei.WillHaveOneCharge(110) || Kenki >= 50 || IsTargetDying)
                {
                    if (HissatsuKyuten.CanUse(out act)) return true;
                    if (HissatsuShinten.CanUse(out act)) return true;
                }
            }

            //Ikishoten Features
            if (Ikishoten.EnoughLevel)
            {
                //Dumps Kenki in preparation for Ikishoten
                if (Kenki > 50 && Ikishoten.WillHaveOneCharge(10))
                {
                    if (HissatsuKyuten.CanUse(out act)) return true;
                    if (HissatsuShinten.CanUse(out act)) return true;
                }

                if (Kenki <= 50 && !Ikishoten.IsCoolingDown && RecordActions?.FirstOrDefault().Action.RowId == Higanbana.ID)
                { if (Ikishoten.CanUse(out act)) return true; }
            }

            if (Shoha.EnoughLevel && MeditationStacks == 3)
            {
                if (Shoha2.CanUse(out act)) return true;
                if (Shoha.CanUse(out act)) return true;
            }
       




            /*
            if (Kenki <= 50 && Ikishoten.CanUse(out act)) return true;
            if ((HostileTarget?.HasStatus(true, StatusID.Higanbana) ?? false) && (HostileTarget?.WillStatusEnd(32, true, StatusID.Higanbana) ?? false) && !(HostileTarget?.WillStatusEnd(28, true, StatusID.Higanbana) ?? false) && SenCount == 1 && IsLastAction(true, Yukikaze) && !meikyoBuff)
            {
                if (Hagakure.CanUse(out act)) return true;
            }
            if (HasMoon && HasFlower)
            {
                if (HissatsuGuren.CanUse(out act, !HissatsuSenei.EnoughLevel ? CanUseOption.MustUse : CanUseOption.None)) return true;
                if (HissatsuSenei.CanUse(out act)) return true;
            }
            if (Shoha2.CanUse(out act)) return true;
            if (Shoha.CanUse(out act)) return true;
            if (Kenki >= 50 && Ikishoten.WillHaveOneCharge(10) || Kenki >= Configs.GetInt("addKenki") || IsTargetBoss && IsTargetDying)
            {
                if (HissatsuKyuten.CanUse(out act)) return true;
                if (HissatsuShinten.CanUse(out act)) return true;
            }

            */

            return base.AttackAbility(out act);
        }

        protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
        {
            var IsTargetBoss = HostileTarget?.IsBossFromTTK() ?? false;
            var IsTargetDying = HostileTarget?.IsDying() ?? false;


            /*

            if (!OpenerInProgress)
            {
                if (Shoha2.CanUse(out act)) return true;
                if (Shoha.CanUse(out act)) return true;
            }
            if (!OpenerInProgress && Kenki > 50)
            {
                if (HissatsuKyuten.CanUse(out act)) return true;
                if (HissatsuShinten.CanUse(out act)) return true;

            }
            if (!OpenerInProgress && HasHostilesInRange && IsLastGCD(true, Yukikaze, Mangetsu, Oka) &&
            (!IsTargetBoss || (HostileTarget?.HasStatus(true, StatusID.Higanbana) ?? false) && !(HostileTarget?.WillStatusEnd(40, true, StatusID.Higanbana) ?? false) || !HasMoon && !HasFlower || IsTargetBoss && IsTargetDying))
            {
                if (MeikyoShisui.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
            }
            */
            return base.EmergencyAbility(nextGCD, out act);
        }


    }
}
