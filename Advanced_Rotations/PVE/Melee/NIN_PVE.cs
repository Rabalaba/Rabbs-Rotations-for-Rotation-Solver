﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static FFXIVClientStructs.FFXIV.Client.Game.Control.GazeController;
using RabbsRotations.JobHelpers;
using static RabbsRotations.JobHelpers.NIN;
using RotationSolver.Basic.Data;
using static RabbsRotations.Job_Helpers.CustomComboFunctions;
using Lumina.Excel.GeneratedSheets;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.Game.Gauge;
using Svg.FilterEffects;
using Dalamud.Plugin.Services;

namespace RabbsRotationsNET8.PVE.Melee;
[Rotation("Rabbs Ninja", CombatType.PvE, GameVersion = "6.58")]
[Api(1)]
[SourceCode(Path = "main/RabbsRotations/Melee/NIN.cs")]

public unsafe sealed class NIN_PVE : NinjaRotation
{
    private static bool inMudraState => Player.HasStatus(true, StatusID.Mudra);

    public static IJobGauges JobGauges { get; private set; } = null!;
    //private static NinjaGauge* gauge = (NinjaGauge*)NIN_PVE.JobGauges.Get<NINGauge>().Address;
    //private static byte stacks = gauge->Kazematoi;
    private static readonly MudraCasting mudraCasting = new();
    private static bool inTCJ => Player.HasStatus(true, StatusID.TenChiJin);
    private static readonly MudraCasting mudraState = new();
    private bool InTrickAttack => TrickAttackPvE.Cooldown.IsCoolingDown && !TrickAttackPvE.Cooldown.ElapsedAfter(17);

    protected override IAction? CountDownAction(float remainTime)
    {
        return base.CountDownAction(remainTime);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {

        return base.EmergencyAbility(nextGCD, out act);
    }

    //public static byte stacks == NinjaGauge.Kazematoi;



    protected unsafe override bool GeneralGCD(out IAction? act)
    {
        bool setupSuitonWindow = GetCooldownRemainingTime(TrickAttackPvE) <= 10 && !Player.HasStatus(true, StatusID.Suiton);
        bool chargeCheck = GetRemainingCharges(TenPvE) == 2 || GetRemainingCharges(TenPvE) == 1 && TenPvE.Cooldown.RecastTimeRemainOneCharge < 3;
        bool inTrickBurstSaveWindow = GetCooldownRemainingTime(TrickAttackPvE) <= 15 && SuitonPvE.EnoughLevel;
        bool poolCharges = GetRemainingCharges(TenPvE) == 1 && TenPvE.Cooldown.RecastTimeRemainOneCharge < 2 || HostileTarget != null && HostileTarget.HasStatus(true, StatusID.TrickAttack);
        uint actionID = 0;

        //if (stacks != 0)
        //{
            //if (SpinningEdgePvE.CanUse(out act)) return true;
        //}

        /*
        if (inTCJ)
        {
            uint tenId = AdjustId(TenPvE.ID);
            uint chiId = AdjustId(ChiPvE.ID);
            uint jinId = AdjustId(JinPvE.ID);

            if (tenId == FumaShurikenPvE_18873.ID
                && !IsLastAction(false, FumaShurikenPvE_18875, FumaShurikenPvE_18873))
            {
                //AOE
                if (KatonPvE.CanUse(out _))
                {
                    if (FumaShurikenPvE_18875.CanUse(out act)) return true;
                }
                //Single
                if (FumaShurikenPvE_18873.CanUse(out act)) return true;
            }

            //Second
            else if (tenId == KatonPvE_18876.ID && !IsLastAction(false, KatonPvE_18876))
            {
                if (KatonPvE_18876.CanUse(out act, skipAoeCheck: true)) return true;
            }
            //Others
            else if (chiId == RaitonPvE_18877.ID && !IsLastAction(false, RaitonPvE_18877))
            {
                if (RaitonPvE_18877.CanUse(out act, skipAoeCheck: true)) return true;
            }
            else if (chiId == DotonPvE_18880.ID && !IsLastAction(false, DotonPvE_18880))
            {
                if (DotonPvE_18880.CanUse(out act, skipAoeCheck: true)) return true;
            }
            else if (jinId == SuitonPvE_18881.ID && !IsLastAction(false, SuitonPvE_18881))
            {
                if (SuitonPvE_18881.CanUse(out act, skipAoeCheck: true)) return true;
            }
        }
        if (Player.HasStatus(true, StatusID.Hidden))
        {
            StatusHelper.StatusOff(StatusID.Hidden);
        }
        if (!InCombat && TenPvE.Cooldown.IsCoolingDown && !inMudraState && HidePvE.CanUse(out act)) return true;
        if (AdjustId(ActionID.NinjutsuPvE) is ActionID.RabbitMediumPvE)
            if (RabbitMediumPvE.CanUse(out act)) return true;

        if (mudraState.CastGokaMekkyaku(out actionID))
        {
            IBaseAction Thisaction = new BaseAction((ActionID)actionID);
            if (actionID != 0)
            {
                if (Thisaction.CanUse(out act, skipAoeCheck: true, skipCastingCheck: true, skipComboCheck: true, skipStatusProvideCheck: true, usedUp: true)) return true;
            }
        }

        if (Player.HasStatus(true, StatusID.Kassatsu) && HostileTarget != null && HostileTarget.HasStatus(true, StatusID.TrickAttack) && MugPvE.Cooldown.IsCoolingDown)
            mudraState.CurrentMudra = MudraCasting.MudraState.CastingHyoshoRanryu;

        if (mudraState.CastHyoshoRanryu(out actionID))
        {
            IBaseAction Thisaction = new BaseAction((ActionID)actionID);
            if (actionID != 0)
            {
                if (Thisaction.CanUse(out act)) return true;
            }
        }
        if (mudraState.CurrentMudra != MudraCasting.MudraState.None && WeaponRemain == WeaponElapsed)
        {
            if (mudraState.ContinueCurrentMudra(ref actionID))
            {
                IBaseAction Thisaction = new BaseAction((ActionID)actionID);
                if (actionID != 0)
                {
                    if (Thisaction.CanUse(out act, skipAoeCheck: true, skipCastingCheck: true, skipComboCheck: true, skipStatusProvideCheck: true, usedUp: true)) return true;
                }
            }
        }

        if (InCombat && !HasHostilesInRange && HasHostilesInMaxRange)
        {
            if (Player.HasStatus(true, StatusID.PhantomKamaitachiReady) &&
                            (GetCooldownRemainingTime(TrickAttackPvE) > Player.StatusTime(true, StatusID.PhantomKamaitachiReady) && Player.StatusTime(true, StatusID.PhantomKamaitachiReady) < 5 || HostileTarget != null && HostileTarget.HasStatus(true, StatusID.TrickAttack) || Player.HasStatus(true, StatusID.Bunshin) && HostileTarget != null && HostileTarget.HasStatus(true, StatusID.Mug)) &&
                            PhantomKamaitachiPvE.EnoughLevel
                            && true)
                if (PhantomKamaitachiPvE.CanUse(out act)) return true;

            if (InCombat &&
                setupSuitonWindow &&
                TrickAttackPvE.EnoughLevel &&
                !Player.HasStatus(true, StatusID.Suiton) &&
                mudraState.CastSuiton(out actionID))
            {
                IBaseAction Thisaction = new BaseAction((ActionID)actionID);
                if (actionID != 0)
                {
                    if (Thisaction.CanUse(out act, skipAoeCheck: true, skipCastingCheck: true,  skipComboCheck: true, skipStatusProvideCheck: true, usedUp: true)) return true;
                }
            }
            
            if (true &&
                !inTrickBurstSaveWindow &&
                chargeCheck &&
                poolCharges &&
                mudraState.CastRaiton(out actionID))
            {
                IBaseAction Thisaction = new BaseAction((ActionID)actionID);
                if (actionID != 0)
                {
                    if (Thisaction.CanUse(out act, skipAoeCheck: true, skipCastingCheck: true,  skipComboCheck: true, skipStatusProvideCheck: true, usedUp: true)) return true;
                }
            }
            


            if (HostileTarget != null && !Player.HasStatus(true, StatusID.RaijuReady) && !inMudraState)
                if (ThrowingDaggerPvE.CanUse(out act)) return true;
        }

        if (NumberOfAllHostilesInRange >= 3)
        {
            if (RecordActions != null)
            {
                // Limit to the most recent 10 elements (or less if RecordActions has fewer elements)
                var recentActions = RecordActions.Take(10);

                if (!recentActions.Any(x => x.Action.RowId == DotonPvE.ID || x.Action.RowId == DotonPvE_18880.ID) && !Player.HasStatus(true, StatusID.Doton))
                {
                    if (mudraState.CastDoton(out actionID))
                    {
                        IBaseAction Thisaction = new BaseAction((ActionID)actionID);
                        if (actionID != 0)
                        {
                            if (Thisaction.CanUse(out act, skipAoeCheck: true, skipCastingCheck: true, skipComboCheck: true, skipStatusProvideCheck: true, usedUp: true)) return true;
                        }
                    }
                }
            }



            if (mudraState.CastKaton(out actionID))
            {
                IBaseAction Thisaction = new BaseAction((ActionID)actionID);
                if (actionID != 0)
                {
                    if (Thisaction.CanUse(out act, skipAoeCheck: true, skipCastingCheck: true, skipComboCheck: true, skipStatusProvideCheck: true, usedUp: true)) return true;
                }
            }
        }

        if (Player.HasStatus(true, StatusID.RaijuReady))
        {
            if (FleetingRaijuPvE.CanUse(out act)) return true;
        }

        if (Player.HasStatus(true, StatusID.PhantomKamaitachiReady) &&
            (GetCooldownRemainingTime(TrickAttackPvE) > Player.StatusTime(true, StatusID.PhantomKamaitachiReady) && Player.StatusTime(true, StatusID.PhantomKamaitachiReady) < 5 || HostileTarget != null && HostileTarget.HasStatus(true, StatusID.TrickAttack) || Player.HasStatus(true, StatusID.Bunshin) && HostileTarget != null && HostileTarget.HasStatus(true, StatusID.Mug)) &&
            PhantomKamaitachiPvE.EnoughLevel)
            if (PhantomKamaitachiPvE.CanUse(out act)) return true;



        if (!inTrickBurstSaveWindow && IsOnCooldown(MugPvE) && mudraState.CastHyoshoRanryu(out actionID))
        {
            IBaseAction Thisaction = new BaseAction((ActionID)actionID);
            if (actionID != 0)
            {
                if (Thisaction.CanUse(out act, skipAoeCheck: true, skipCastingCheck: true, skipComboCheck: true, skipStatusProvideCheck: true, usedUp: true)) return true;
            }
        }


        if (InCombat && setupSuitonWindow &&
            TrickAttackPvE.EnoughLevel &&
            !Player.HasStatus(true, StatusID.Suiton) &&
            mudraState.CastSuiton(out actionID))
        {
            IBaseAction Thisaction = new BaseAction((ActionID)actionID);
            if (actionID != 0)
            {
                if (Thisaction.CanUse(out act, skipAoeCheck: true, skipCastingCheck: true, skipComboCheck: true, skipStatusProvideCheck: true, usedUp: true)) return true;
            }
        }
        
        if (!inTrickBurstSaveWindow &&
                    chargeCheck &&
                    poolCharges &&
                    mudraState.CastRaiton(out actionID))
        {
            IBaseAction Thisaction = new BaseAction((ActionID)actionID);
            if (actionID != 0)
            {
                if (Thisaction.CanUse(out act, skipAoeCheck: true, skipCastingCheck: true, skipComboCheck: true, skipStatusProvideCheck: true, usedUp: true)) return true;
            }
        }

        if (InTrickAttack &&
                    mudraState.CastRaiton(out actionID))
        {
            IBaseAction Thisaction = new BaseAction((ActionID)actionID);
            if (actionID != 0)
            {
                if (Thisaction.CanUse(out act, skipAoeCheck: true, skipCastingCheck: true, skipComboCheck: true, skipStatusProvideCheck: true, usedUp: true)) return true;
            }
        }


        if (!RaitonPvE.EnoughLevel &&
                    chargeCheck &&
                    mudraState.CastFumaShuriken(out actionID))
        {
            IBaseAction Thisaction = new BaseAction((ActionID)actionID);
            if (actionID != 0)
            {
                if (Thisaction.CanUse(out act, skipAoeCheck: true, skipCastingCheck: true, skipComboCheck: true, skipStatusProvideCheck: true, usedUp: true)) return true;
            }
        }

        if (!inMudraState)
        {
            //AOE
            if (HakkeMujinsatsuPvE.CanUse(out act)) return true;
            if (DeathBlossomPvE.CanUse(out act)) return true;

            //Single
            if (ArmorCrushPvE.CanUse(out act)) return true;
            if (AeolianEdgePvE.CanUse(out act)) return true;
            if (GustSlashPvE.CanUse(out act)) return true;
            if (SpinningEdgePvE.CanUse(out act)) return true;
        }
        */
        return base.GeneralGCD(out act);
    }

    protected unsafe override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        bool setupKassatsuWindow = GetCooldownRemainingTime(TrickAttackPvE) <= 10 && Player.HasStatus(true, StatusID.Suiton);
        bool inTrickBurstSaveWindow = TrickAttackPvE.Cooldown.IsCoolingDown && TrickAttackPvE.Cooldown.WillHaveOneCharge(15);

        if (!inMudraState && !inTCJ)
        {
            if (Player.HasStatus(true, StatusID.Suiton) &&
                           GetCooldownRemainingTime(TrickAttackPvE) <= 3 &&
                           InCombat && CombatTime > 6 &&
                           IsOffCooldown(MugPvE) &&
                           MugPvE.EnoughLevel)
                if (MugPvE.CanUse(out act)) return true;

            if (Player.HasStatus(true, StatusID.Suiton) && IsOffCooldown(TrickAttackPvE) && InCombat && CombatTime > 8)
                if (TrickAttackPvE.CanUse(out act)) return true;

            if (Ninki >= 50)
                if (BunshinPvE.CanUse(out act)) return true;

            if (HostileTarget != null && HostileTarget.HasStatus(true, StatusID.TrickAttack_3254) || setupKassatsuWindow)
                if (KassatsuPvE.CanUse(out act)) return true;

            if (HostileTarget != null && HostileTarget.HasStatus(true, StatusID.TrickAttack) && Ninki >= 50 || Ninki == 100 && IsOnCooldown(MugPvE))
            {
                if (HellfrogMediumPvE.CanUse(out act)) return true;
                if (BhavacakraPvE.CanUse(out act)) return true;
            }

            if (InTrickAttack)
            {
                if (MugPvE.CanUse(out act)) return true;

                if (TenChiJinPvE.CanUse(out act)) return true;

                if (Player.HasStatus(true, StatusID.Suiton) && Ninki <= 50)
                    if (MeisuiPvE.CanUse(out act)) return true;

                if (Ninki >= 50)
                    if (HellfrogMediumPvE.CanUse(out act)) return true;

                if (Ninki >= 50)
                    if (BhavacakraPvE.CanUse(out act)) return true;

                if (AssassinatePvE.CanUse(out act)) return true;

            }

        }

        return base.AttackAbility(nextGCD, out act);
    }
}

