using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using RotationSolver.Basic.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FFXIVClientStructs.FFXIV.Client.Game.Control.GazeController;
using static FFXIVClientStructs.FFXIV.Client.Graphics.Kernel.VertexShader;

namespace RabbsRotations.Melee
{
    public sealed class SAM_Default : SAM_Base
    {
        public override CombatType Type => CombatType.PVE;
        public override string GameVersion => "6.55";
        public override string RotationName => "Rabbs Samurai";
        public override string Description => "PVE Samurai w/ option for opener Grade8 Stength Pot(Check options)";

        #region Rotation Config
        protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
            .SetCombo(CombatType.PvE, "RotationSelection", 1, "Select which Opener to use.", "With Medicine", "Without Medicine");
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
        #endregion

        #region Countdown Logic
        protected override IAction CountDownAction(float remainTime)
        {
            if (remainTime < 0.1f)
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

            while (OpenerInProgress && (!OpenerHasFinished || !OpenerHasFailed))
            {
                if (TimeSinceLastAction.TotalSeconds > 3.5f)
                {
                    OpenerHasFailed = true;
                    OpenerInProgress = false;
                    Openerstep = 0;
                }
                if (Player.IsDead)
                {
                    OpenerHasFailed = true;
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
                                EvenCooldownPhasestep = 0;
                                CooldownPhaseInProgressEven = true;

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
                                EvenCooldownPhasestep = 0;
                                CooldownPhaseInProgressEven = true;

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

            while (CooldownPhaseInProgressOdd && (!CooldownPhaseOddHasFinished || !CooldownPhaseOddHasFailed))
            {
                if (TimeSinceLastAction.TotalSeconds > 3.5f || Player.IsDead)
                {
                    CooldownPhaseOddHasFailed = true;
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

            while (CooldownPhaseInProgressEven && (!CooldownPhaseEvenHasFinished || !CooldownPhaseEvenHasFailed))
            {
                if (TimeSinceLastAction.TotalSeconds > 3.5f || Player.IsDead)
                {
                    CooldownPhaseEvenHasFailed = true;
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
                        CooldownPhaseEvenHasFinished = true;
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

            while (OddMinuteBurstPhaseInProgress && (!OddMinuteBurstPhaseHasFinished || !OddMinuteBurstPhaseHasFailed))
            {
                if (TimeSinceLastAction.TotalSeconds > 3.5f || Player.IsDead)
                {
                    OddMinuteBurstPhaseHasFailed = true;
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

            while (EvenMinuteBurstPhaseInProgress && (!EvenMinuteBurstPhaseHasFinished || !EvenMinuteBurstPhaseHasFailed))
            {
                if (TimeSinceLastAction.TotalSeconds > 3.5f || Player.IsDead)
                {
                    EvenMinuteBurstPhaseHasFailed = true;
                    EvenMinuteBurstPhaseInProgress = false;
                    Openerstep = 0;
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
                        EvenMinuteBurstPhaseHasFinished = true;
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
                                OddCooldownPhasestep = 0;
                                CooldownPhaseInProgressOdd = true;
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
                                OddCooldownPhasestep = 0;
                                CooldownPhaseInProgressOdd = true;
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
                                OddCooldownPhasestep = 0;
                                CooldownPhaseInProgressOdd = true;
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
                                EvenCooldownPhasestep = 0;
                                CooldownPhaseInProgressEven = true;
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
                                EvenCooldownPhasestep = 0;
                                CooldownPhaseInProgressEven = true;
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
                                EvenCooldownPhasestep = 0;
                                CooldownPhaseInProgressEven = true;
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

        protected override bool GeneralGCD(out IAction act)
        {

            act = null;
            ActionID Adjsuted_Keishi = AdjustId(ActionID.TsubameGaeshi);
            var IsTargetBoss = HostileTarget?.IsBossFromTTK() ?? false;
            var IsTargetDying = HostileTarget?.IsDying() ?? false;
            int GCDsUntilBurst(uint gcdCount = 0u, float offset = 0f)
            {
                int gcdsChecked = 0;
                while (!TsubameGaeshi.WillHaveOneChargeGCD(gcdCount, offset))
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

            //what of not in any phase => need to realign to proper phase
            if (!OpenerInProgress && !EvenMinuteBurstPhaseInProgress && !OddMinuteBurstPhaseInProgress && !CooldownPhaseInProgressOdd && !CooldownPhaseInProgressEven && !EvenFillerPhaseInProgress && !OddFillerPhaseInProgress && InCombat)
            {
                NoPhaseInProgess = true;
            }

            if (NoPhaseInProgess) //problem here because we aren't attacking if phase not defined
            {

                if (GCDsUntilBurst() <= 1 && SenCount == 3) // our burst abilities are ready... trying to figure out odd minute vs even
                {
                    if (HissatsuSenei.WillHaveOneChargeGCD(1))
                    {
                        EvenMinuteBurstPhasestep = 0;
                        EvenMinuteBurstPhaseInProgress = true;
                    }
                    OddMinuteBurstPhasestep = 0;
                    OddMinuteBurstPhaseInProgress = true;
                }

                if (GCDsUntilBurst() <= 1 && SenCount < 3)  //burst is ready but we need sen first
                {
                    //Kasha combo(Ka) > Gekko(Getsu) combo > Yukikaze(Setsu) combo
                    if (!HasKa)
                    {
                        if (Kasha.CanUse(out act)) return true;
                        if (Shifu.CanUse(out act)) return true;
                        if (Hakaze.CanUse(out act)) return true;

                    }

                    if (!HasGetsu)
                    {
                        if (Gekko.CanUse(out act)) return true;
                        if (Jinpu.CanUse(out act)) return true;
                        if (Hakaze.CanUse(out act)) return true;
                    }

                    if (!HasSetsu)
                    {
                        if (Yukikaze.CanUse(out act)) return true;
                        if (Hakaze.CanUse(out act)) return true;
                    }
                    if (GCDsUntilBurst() > 1 && GCDsUntilBurst() <=18) //cooldown phase this will be true
                    {
                        if (!HissatsuSenei.WillHaveOneChargeGCD(22)) //odd burst this will be true if even burst is next one
                        {
                            if (GCDsUntilBurst() < 8)  //half way though cooldwon phase
                            {
                                if (SenCount == 3) //check if we can midare
                                {
                                    OddCooldownPhasestep = 8;  //the midare is step 8 (9th gcd since we count staring at 0
                                    CooldownPhaseInProgressOdd = true;
                                }
                                //get us to 3 sen to allign to midare halfway point
                                if (!HasKa)
                                {
                                    if (Kasha.CanUse(out act)) return true;
                                    if (Shifu.CanUse(out act)) return true;
                                    if (Hakaze.CanUse(out act)) return true;

                                }

                                if (!HasGetsu)
                                {
                                    if (Gekko.CanUse(out act)) return true;
                                    if (Jinpu.CanUse(out act)) return true;
                                    if (Hakaze.CanUse(out act)) return true;
                                }

                                if (!HasSetsu)
                                {
                                    if (Yukikaze.CanUse(out act)) return true;
                                    if (Hakaze.CanUse(out act)) return true;
                                }
                            }
                            if (GCDsUntilBurst() > 8 && SenCount !=3)  //cast gcds until halfway midare alignment
                            {
                                if (!HasKa)
                                {
                                    if (Kasha.CanUse(out act)) return true;
                                    if (Shifu.CanUse(out act)) return true;
                                    if (Hakaze.CanUse(out act)) return true;

                                }

                                if (!HasGetsu)
                                {
                                    if (Gekko.CanUse(out act)) return true;
                                    if (Jinpu.CanUse(out act)) return true;
                                    if (Hakaze.CanUse(out act)) return true;
                                }

                                if (!HasSetsu)
                                {
                                    if (Yukikaze.CanUse(out act)) return true;
                                    if (Hakaze.CanUse(out act)) return true;
                                }
                            }
                            if (GCDsUntilBurst() > 8 && SenCount == 3) //somehow we messed up but we have to midare to align now
                            {
                                if(MidareSetsugekka.CanUse(out act)) return true;  //consider using filler logic here to better align in future updates
                            }

                        }
                        if (HissatsuSenei.WillHaveOneChargeGCD(22)) //even burst this will be true if even burst is next one
                        {
                            if (GCDsUntilBurst() < 8)  //half way though cooldwon phase
                            {
                                if (SenCount == 3) //check if we can midare
                                {
                                    EvenCooldownPhasestep = 8;  //the midare is step 8 (9th gcd since we count staring at 0
                                    CooldownPhaseInProgressEven = true;
                                }
                                //get us to 3 sen to allign to midare halfway point
                                if (!HasKa)
                                {
                                    if (Kasha.CanUse(out act)) return true;
                                    if (Shifu.CanUse(out act)) return true;
                                    if (Hakaze.CanUse(out act)) return true;

                                }

                                if (!HasGetsu)
                                {
                                    if (Gekko.CanUse(out act)) return true;
                                    if (Jinpu.CanUse(out act)) return true;
                                    if (Hakaze.CanUse(out act)) return true;
                                }

                                if (!HasSetsu)
                                {
                                    if (Yukikaze.CanUse(out act)) return true;
                                    if (Hakaze.CanUse(out act)) return true;
                                }
                            }
                            if (GCDsUntilBurst() > 8 && SenCount != 3)  //cast gcds until halfway midare alignment
                            {
                                if (!HasKa)
                                {
                                    if (Kasha.CanUse(out act)) return true;
                                    if (Shifu.CanUse(out act)) return true;
                                    if (Hakaze.CanUse(out act)) return true;

                                }

                                if (!HasGetsu)
                                {
                                    if (Gekko.CanUse(out act)) return true;
                                    if (Jinpu.CanUse(out act)) return true;
                                    if (Hakaze.CanUse(out act)) return true;
                                }

                                if (!HasSetsu)
                                {
                                    if (Yukikaze.CanUse(out act)) return true;
                                    if (Hakaze.CanUse(out act)) return true;
                                }
                            }
                            if (GCDsUntilBurst() > 8 && SenCount == 3) //somehow we messed up but we have to midare to align now
                            {
                                if (MidareSetsugekka.CanUse(out act)) return true;  //consider using filler logic here to better align in future updates
                            }

                        }

                    }

                    //only other condition is filler phase
                    if (!HissatsuSenei.WillHaveOneChargeGCD(22)) // true for odd filler
                    {
                        OddFillerPhasestep = 0;
                        OddFillerPhaseInProgress = true;
                    }
                    EvenFillerPhasestep = 0;
                    EvenFillerPhaseInProgress = true;

                }
            }

            #endregion
                /*
                if (KaeshiNamikiri.CanUse(out act, CanUseOption.MustUse)) return true;
                if (KaeshiGoken.CanUse(out act, CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo)) return true;
                if (KaeshiSetsugekka.CanUse(out act, CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo)) return true;
                if ((!IsTargetBoss || (HostileTarget?.HasStatus(true, StatusID.Higanbana) ?? false)) && HasMoon && HasFlower
                    && OgiNamikiri.CanUse(out act, CanUseOption.MustUse)) return true;
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
                if ((!HasMoon || IsMoonTimeLessThanFlower || !Oka.EnoughLevel) && Mangetsu.CanUse(out act, Player.HasStatus(true, StatusID.MeikyoShisui) && !HasGetsu ? CanUseOption.EmptyOrSkipCombo : CanUseOption.None)) return true;
                if ((!HasFlower || !IsMoonTimeLessThanFlower) && Oka.CanUse(out act, Player.HasStatus(true, StatusID.MeikyoShisui) && !HasKa ? CanUseOption.EmptyOrSkipCombo : CanUseOption.None)) return true;
                if (!HasSetsu && Yukikaze.CanUse(out act, Player.HasStatus(true, StatusID.MeikyoShisui) && HasGetsu && HasKa && !HasSetsu ? CanUseOption.EmptyOrSkipCombo : CanUseOption.None)) return true;
                if (Gekko.CanUse(out act, Player.HasStatus(true, StatusID.MeikyoShisui) && !HasGetsu ? CanUseOption.EmptyOrSkipCombo : CanUseOption.None)) return true;
                if (Kasha.CanUse(out act, Player.HasStatus(true, StatusID.MeikyoShisui) && !HasKa ? CanUseOption.EmptyOrSkipCombo : CanUseOption.None)) return true;
                if ((!HasMoon || IsMoonTimeLessThanFlower || !Shifu.EnoughLevel) && Jinpu.CanUse(out act)) return true;
                if ((!HasFlower || !IsMoonTimeLessThanFlower) && Shifu.CanUse(out act)) return true;

                if (!Player.HasStatus(true, StatusID.MeikyoShisui))
                {
                    if (Fuko.CanUse(out act)) return true;
                    if (!Fuko.EnoughLevel && Fuga.CanUse(out act)) return true;
                    if (Hakaze.CanUse(out act)) return true;
                    if (Enpi.CanUse(out act)) return true;
                }*/
            return false;

        }

        protected override bool AttackAbility(out IAction act)
        {

            act = null;
            ActionID Adjsuted_Keishi = AdjustId(ActionID.TsubameGaeshi);
            var IsTargetBoss = HostileTarget?.IsBossFromTTK() ?? false;
            var IsTargetDying = HostileTarget?.IsDying() ?? false;

            if (!OpenerInProgress)
            {
                if (Shoha2.CanUse(out act)) return true;
                if (Shoha.CanUse(out act)) return true;
                if (Kenki >= 50)
                {
                    if (HissatsuKyuten.CanUse(out act)) return true;
                    if (HissatsuShinten.CanUse(out act)) return true;
                }
            }
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
            /*
            if (Kenki <= 50 && Ikishoten.CanUse(out act)) return true;
            if ((HostileTarget?.HasStatus(true, StatusID.Higanbana) ?? false) && (HostileTarget?.WillStatusEnd(32, true, StatusID.Higanbana) ?? false) && !(HostileTarget?.WillStatusEnd(28, true, StatusID.Higanbana) ?? false) && SenCount == 1 && IsLastAction(true, Yukikaze) && !Player.HasStatus(true, StatusID.MeikyoShisui))
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
            if (Kenki >= 50 && Ikishoten.WillHaveOneCharge(10) || IsTargetBoss && IsTargetDying)
            {
                if (HissatsuKyuten.CanUse(out act)) return true;
                if (HissatsuShinten.CanUse(out act)) return true;
            }
            */


            return base.AttackAbility(out act);
        }

        protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
        {

                return base.EmergencyAbility(nextGCD, out act);
        }


    }
}
