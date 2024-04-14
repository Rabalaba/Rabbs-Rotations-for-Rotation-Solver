using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using RotationSolver.Basic.Actions;


namespace RabbsRotations.JobHelpers
{
    internal class NIN
    {
        internal class MudraCasting : NinjaRotation
        {

            ///<summary> Checks if the player is in a state to be able to cast a ninjitsu.</summary>
            public bool CanCast()
            {
                var gcd = WeaponTotal;

                if (gcd == 0.5) return true;

                if (TenPvE.Cooldown.CurrentCharges == 0 &&
                    !Player.HasStatus(true, StatusID.Mudra) &&
                    !Player.HasStatus(true, StatusID.Kassatsu))
                    return false;

                return true;
            }


            private MudraState currentMudra = MudraState.None;
            public MudraState CurrentMudra
            {
                get
                {
                    return currentMudra;
                }
                set
                {
                    if (value == MudraState.None)
                    {
                        justResetMudra = true;
                    }
                    else
                    {
                        justResetMudra = false;
                    }

                    currentMudra = value;
                }
            }

            ///<summary> Simple method of casting Fuma Shuriken.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public bool CastFumaShuriken(out uint actionID)
            {
                if (FumaShurikenPvE.EnoughLevel && CurrentMudra is MudraState.None or MudraState.CastingFumaShuriken)
                {
                    if (!CanCast())
                    {
                        CurrentMudra = MudraState.None;
                        actionID = 0;
                        return false;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == FumaShurikenPvE.ID)
                    {
                        actionID = AdjustId(NinjutsuPvE.ID);
                        return true;
                    }

                    actionID = AdjustId(TenPvE.ID);
                    CurrentMudra = MudraState.CastingFumaShuriken;
                    return true;
                }

                CurrentMudra = MudraState.None;
                actionID = 0;
                return false;
            }


            ///<summary> Simple method of casting Raiton.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public bool CastRaiton(out uint actionID)
            {
                if (RaitonPvE.EnoughLevel && CurrentMudra is MudraState.None or MudraState.CastingRaiton)
                {
                    if (!CanCast())
                    {
                        CurrentMudra = MudraState.None;
                        actionID = 0;
                        return false;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == FumaShurikenPvE.ID)
                    {
                        actionID = AdjustId(ChiPvE.ID);
                        return true;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == RaitonPvE.ID)
                    {
                        actionID = AdjustId(NinjutsuPvE.ID);
                        return true;
                    }

                    actionID = AdjustId(TenPvE.ID);
                    CurrentMudra = MudraState.CastingRaiton;
                    return true;
                }

                CurrentMudra = MudraState.None;
                actionID = 0;
                return false;
            }

            ///<summary> Simple method of casting Katon.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public bool CastKaton(out uint actionID)
            {
                if (KatonPvE.EnoughLevel && CurrentMudra is MudraState.None or MudraState.CastingKaton)
                {
                    if (!CanCast())
                    {
                        CurrentMudra = MudraState.None;
                        actionID = 0;
                        return false;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == FumaShurikenPvE.ID)
                    {
                        actionID = AdjustId(TenPvE.ID);
                        return true;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == KatonPvE.ID)
                    {
                        actionID = AdjustId(NinjutsuPvE.ID);
                        return true;
                    }

                    actionID = AdjustId(ChiPvE.ID);
                    CurrentMudra = MudraState.CastingKaton;
                    return true;
                }

                CurrentMudra = MudraState.None;
                actionID = 0;
                return false;
            }

            ///<summary> Simple method of casting Hyoton.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public bool CastHyoton(out uint actionID)
            {
                if (HyotonPvE.EnoughLevel && CurrentMudra is MudraState.None or MudraState.CastingHyoton)
                {
                    if (!CanCast() || Player.HasStatus(true, StatusID.Kassatsu))
                    {
                        CurrentMudra = MudraState.None;
                        actionID = 0;
                        return false;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == FumaShurikenPvE.ID)
                    {
                        actionID = AdjustId(JinPvE.ID);
                        return true;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == HyotonPvE.ID)
                    {
                        actionID = AdjustId(NinjutsuPvE.ID);
                        return true;
                    }

                    actionID = AdjustId(TenPvE.ID);
                    CurrentMudra = MudraState.CastingHyoton;
                    return true;
                }

                CurrentMudra = MudraState.None;
                actionID = 0;
                return false;
            }

            ///<summary> Simple method of casting Huton.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public bool CastHuton(out uint actionID)
            {
                if (HutonPvE.EnoughLevel && CurrentMudra is MudraState.None or MudraState.CastingHuton)
                {
                    if (!CanCast())
                    {
                        CurrentMudra = MudraState.None;
                        actionID = 0;
                        return false;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == FumaShurikenPvE.ID)
                    {
                        actionID = AdjustId(JinPvE.ID);
                        return true;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == HyotonPvE.ID)
                    {
                        actionID = AdjustId(TenPvE.ID);
                        return true;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == HutonPvE.ID)
                    {
                        actionID = AdjustId(NinjutsuPvE.ID);
                        return true;
                    }

                    actionID = AdjustId(ChiPvE.ID);
                    CurrentMudra = MudraState.CastingHuton;
                    return true;
                }

                CurrentMudra = MudraState.None;
                actionID = 0;
                return false;
            }

            ///<summary> Simple method of casting Doton.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public bool CastDoton(out uint actionID)
            {
                if (DotonPvE.EnoughLevel && CurrentMudra is MudraState.None or MudraState.CastingDoton)
                {
                    if (!CanCast())
                    {
                        CurrentMudra = MudraState.None;
                        actionID = 0;
                        return false;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == FumaShurikenPvE.ID)
                    {
                        actionID = AdjustId(JinPvE.ID);
                        return true;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == HyotonPvE.ID)
                    {
                        actionID = AdjustId(ChiPvE.ID);
                        return true;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == DotonPvE.ID)
                    {
                        actionID = AdjustId(NinjutsuPvE.ID);
                        return true;
                    }

                    actionID = AdjustId(TenPvE.ID);
                    CurrentMudra = MudraState.CastingDoton;
                    return true;
                }

                CurrentMudra = MudraState.None;
                actionID = 0;
                return false;
            }

            ///<summary> Simple method of casting Suiton.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public bool CastSuiton(out uint actionID)
            {
                if (SuitonPvE.EnoughLevel && CurrentMudra is MudraState.None or MudraState.CastingSuiton)
                {
                    if (!CanCast())
                    {
                        CurrentMudra = MudraState.None;
                        actionID = 0;
                        return false;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == FumaShurikenPvE.ID)
                    {
                        actionID = AdjustId(ChiPvE.ID);
                        return true;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == RaitonPvE.ID)
                    {
                        actionID = AdjustId(JinPvE.ID);
                        return true;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == SuitonPvE.ID)
                    {
                        actionID = AdjustId(NinjutsuPvE.ID);
                        return true;
                    }

                    actionID = AdjustId(TenPvE.ID);
                    CurrentMudra = MudraState.CastingSuiton;
                    return true;
                }

                CurrentMudra = MudraState.None;
                actionID = 0;
                return false;
            }

            ///<summary> Simple method of casting Goka Mekkyaku.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public bool CastGokaMekkyaku(out uint actionID)
            {
                if (GokaMekkyakuPvE.EnoughLevel && CurrentMudra is MudraState.None or MudraState.CastingGokaMekkyaku)
                {
                    if (!CanCast() || !Player.HasStatus(true, StatusID.Kassatsu))
                    {
                        CurrentMudra = MudraState.None;
                        actionID = 0;
                        return false;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == FumaShurikenPvE.ID)
                    {
                        actionID = AdjustId(TenPvE.ID);
                        return true;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == GokaMekkyakuPvE.ID)
                    {
                        actionID = AdjustId(NinjutsuPvE.ID);
                        return true;
                    }

                    actionID = AdjustId(ChiPvE.ID);
                    CurrentMudra = MudraState.CastingGokaMekkyaku;
                    return true;
                }

                CurrentMudra = MudraState.None;
                actionID = 0;
                return false;
            }

            ///<summary> Simple method of casting Hyosho Ranryu.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public bool CastHyoshoRanryu(out uint actionID)
            {
                if (HyoshoRanryuPvE.EnoughLevel && CurrentMudra is MudraState.None or MudraState.CastingHyoshoRanryu)
                {
                    if (!CanCast() || !Player.HasStatus(true, StatusID.Kassatsu))
                    {
                        CurrentMudra = MudraState.None;
                        actionID = 0;
                        return false;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == FumaShurikenPvE.ID)
                    {
                        actionID = JinPvE_18807.ID;
                        return true;
                    }

                    if (AdjustId(NinjutsuPvE.ID) == HyoshoRanryuPvE.ID)
                    {
                        actionID = HyoshoRanryuPvE.ID;
                        return true;
                    }

                    actionID = ChiPvE.ID;
                    CurrentMudra = MudraState.CastingHyoshoRanryu;
                    return true;
                }

                CurrentMudra = MudraState.None;
                actionID = 0;
                return false;
            }

            private bool justResetMudra = false;
            public bool ContinueCurrentMudra(ref uint actionID)
            {

                if ((RecordActions?.FirstOrDefault()?.Action.RowId == FumaShurikenPvE.ID ||
                    RecordActions?.FirstOrDefault()?.Action.RowId == KatonPvE.ID ||
                    RecordActions?.FirstOrDefault()?.Action.RowId == RaitonPvE.ID ||
                    RecordActions?.FirstOrDefault()?.Action.RowId == HyotonPvE.ID ||
                    RecordActions?.FirstOrDefault()?.Action.RowId == HutonPvE.ID ||
                    RecordActions?.FirstOrDefault()?.Action.RowId == DotonPvE.ID ||
                    RecordActions?.FirstOrDefault()?.Action.RowId == SuitonPvE.ID ||
                    RecordActions?.FirstOrDefault()?.Action.RowId == GokaMekkyakuPvE.ID ||
                    RecordActions?.FirstOrDefault()?.Action.RowId == HyotonPvE.ID) &&
                    !justResetMudra)
                    CurrentMudra = MudraState.None;


                return CurrentMudra switch
                {
                    MudraState.None => false,
                    MudraState.CastingFumaShuriken => CastFumaShuriken(out actionID),
                    MudraState.CastingKaton => CastKaton(out actionID),
                    MudraState.CastingRaiton => CastRaiton(out actionID),
                    MudraState.CastingHyoton => CastHyoton(out actionID),
                    MudraState.CastingHuton => CastHuton(out actionID),
                    MudraState.CastingDoton => CastDoton(out actionID),
                    MudraState.CastingSuiton => CastSuiton(out actionID),
                    MudraState.CastingGokaMekkyaku => CastGokaMekkyaku(out actionID),
                    MudraState.CastingHyoshoRanryu => CastHyoshoRanryu(out actionID),
                    _ => false,
                };
            }

            public enum MudraState
            {
                None,
                CastingFumaShuriken,
                CastingKaton,
                CastingRaiton,
                CastingHyoton,
                CastingHuton,
                CastingDoton,
                CastingSuiton,
                CastingGokaMekkyaku,
                CastingHyoshoRanryu

            }
        }

        internal class NINOpenerLogic : NinjaRotation
        {
            private bool HasCooldowns()
            {
                if (TenPvE.Cooldown.CurrentCharges < 1) return false;
                if (MugPvE.Cooldown.IsCoolingDown) return false;
                if (TenChiJinPvE.Cooldown.IsCoolingDown) return false;
                if (PhantomKamaitachiPvE.Cooldown.IsCoolingDown) return false;
                if (BunshinPvE.Cooldown.IsCoolingDown) return false;
                if (DreamWithinADreamPvE.Cooldown.IsCoolingDown) return false;
                if (KassatsuPvE.Cooldown.IsCoolingDown) return false;

                return true;
            }

            private static uint OpenerLevel => 90;

            public uint PrePullStep = 1;

            public uint OpenerStep = 1;

            public static bool LevelChecked => Player.Level >= OpenerLevel;

            public bool CanOpener => HasCooldowns() && LevelChecked;

            private OpenerState currentState = OpenerState.OpenerFinished;

            public OpenerState CurrentState
            {
                get
                {
                    return currentState;
                }
                set
                {
                    if (value != currentState)
                    {
                        if (value == OpenerState.PrePull) PrePullStep = 1;
                        if (value == OpenerState.InOpener) OpenerStep = 1;
                        if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener) { PrePullStep = 0; OpenerStep = 0; }

                        currentState = value;
                    }
                }
            }

            private bool DoPrePullSteps(out uint actionID, MudraCasting mudraState)
            {
                actionID = 0;
                if (!LevelChecked) return false;


                if (CanOpener && PrePullStep == 0 && !InCombat) { CurrentState = OpenerState.PrePull; }

                if (CurrentState == OpenerState.PrePull)
                {
                    if (TimeSinceLastAction.TotalSeconds > 5 && !InCombat)
                    {
                        mudraState.CastHuton(out actionID);
                        PrePullStep = 1;
                        return true;
                    }

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == HutonPvE.ID && PrePullStep == 1) PrePullStep++;
                    else if (PrePullStep == 1) mudraState.CastHuton(out actionID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == HidePvE.ID && PrePullStep == 2) PrePullStep++;
                    else if (PrePullStep == 2) { actionID = AdjustId(HidePvE.ID); }

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == SuitonPvE.ID && PrePullStep == 3) CurrentState = OpenerState.InOpener;
                    else if (PrePullStep == 3) mudraState.CastSuiton(out actionID);

                    //Failure states
                    if (PrePullStep is (1 or 2) && InCombat) { mudraState.CurrentMudra = MudraCasting.MudraState.None; ResetOpener(); }
                    actionID = 0;
                    return true;

                }

                PrePullStep = 0;
                actionID = 0;
                return false;
            }

            private bool DoOpener(out uint actionID, MudraCasting mudraState)
            {
                actionID = 0;
                if (!LevelChecked) return false;

                if (CurrentState == OpenerState.InOpener)
                {

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == KassatsuPvE.ID && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = AdjustId(KassatsuPvE.ID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == SpinningEdgePvE.ID && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = AdjustId(SpinningEdgePvE.ID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == GustSlashPvE.ID && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = AdjustId(GustSlashPvE.ID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == MugPvE.ID && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = AdjustId(MugPvE.ID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == BunshinPvE.ID && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = AdjustId(BunshinPvE.ID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == PhantomKamaitachiPvE.ID && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = AdjustId(PhantomKamaitachiPvE.ID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == TrickAttackPvE.ID && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = AdjustId(TrickAttackPvE.ID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == AeolianEdgePvE.ID && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8) actionID = AdjustId(AeolianEdgePvE.ID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == DreamWithinADreamPvE.ID && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = AdjustId(DreamWithinADreamPvE.ID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == HyoshoRanryuPvE.ID && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) mudraState.CastHyoshoRanryu(out actionID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == RaitonPvE.ID && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) mudraState.CastRaiton(out actionID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == TenChiJinPvE.ID && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = AdjustId(TenChiJinPvE.ID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == FumaShurikenPvE_18873.ID && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = AdjustId(TenPvE.ID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == RaitonPvE_18877.ID && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = AdjustId(ChiPvE.ID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == SuitonPvE_18881.ID && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = AdjustId(JinPvE.ID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == MeisuiPvE.ID && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = AdjustId(MeisuiPvE.ID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == FleetingRaijuPvE.ID && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = AdjustId(FleetingRaijuPvE.ID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == BhavacakraPvE.ID && OpenerStep == 18) OpenerStep++;
                    else if (OpenerStep == 18) actionID = AdjustId(BhavacakraPvE.ID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == FleetingRaijuPvE.ID && OpenerStep == 19) OpenerStep++;
                    else if (OpenerStep == 19) actionID = AdjustId(FleetingRaijuPvE.ID);

                    if (RecordActions?.FirstOrDefault()?.Action.RowId == BhavacakraPvE.ID && OpenerStep == 20) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == 20) actionID = AdjustId(BhavacakraPvE.ID);


                    //Failure states
                    if ((OpenerStep is 13 or 14 or 15 && IsMoving) ||
                        (OpenerStep is 7 && !Player.HasStatus(true, StatusID.Suiton)) ||
                        (OpenerStep is 18 or 20 && Ninki < 45) ||
                        (OpenerStep is 17 or 19 && !Player.HasStatus(true, StatusID.RaijuReady)) ||
                        (OpenerStep is 10 && !Player.HasStatus(true, StatusID.Kassatsu)))
                        ResetOpener();


                    return true;
                }

                return false;
            }

            private void ResetOpener()
            {
                CurrentState = OpenerState.FailedOpener;
            }

            private bool openerEventsSetup = false;

            public bool DoFullOpener(out uint actionID, MudraCasting mudraState)
            {
                actionID = 0;
                if (!LevelChecked) return false;

                if (!openerEventsSetup) { Condition.ConditionChange += CheckCombatStatus; openerEventsSetup = true; }

                if (CurrentState == OpenerState.PrePull || CurrentState == OpenerState.FailedOpener)
                    if (DoPrePullSteps(out actionID, mudraState)) return true;

                if (CurrentState == OpenerState.InOpener)
                    if (DoOpener(out actionID, mudraState)) return true;

                if (CurrentState == OpenerState.OpenerFinished && InCombat)
                    ResetOpener();

                return false;
            }

            internal void Dispose()
            {
                Condition.ConditionChange -= CheckCombatStatus;
            }

            private void CheckCombatStatus(ConditionFlag flag, bool value)
            {
                if (flag == ConditionFlag.InCombat && value == false) ResetOpener();
            }
        }
    }
}
