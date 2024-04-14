using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Gauge;
using RotationSolver.Basic.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static FFXIVClientStructs.FFXIV.Client.Game.Control.GazeController;

namespace RabbsRotations.Ranged;
[Rotation("Rabbs Machinist (DO NOT USE YET)", CombatType.PvE, GameVersion = "6.58")]
[SourceCode(Path = "main/RabbsRotations/Ranged/MCH.cs")]





public unsafe sealed class MCH : MachinistRotation
{
    public static float GetCooldownRemainingTime(IBaseAction baseAction) => baseAction.Cooldown.RecastTimeRemainOneCharge;
    public static bool IsOffCooldown(IBaseAction baseAction) => !baseAction.Cooldown.IsCoolingDown;
    public static bool IsOnCooldown(IBaseAction baseAction) => baseAction.Cooldown.IsCoolingDown;
    public static bool LevelChecked(IBaseAction baseAction) => baseAction.EnoughLevel;
    public static bool HasEffect(StatusID status) => Player.HasStatus(true, status);
    internal static bool inOpener = false;
    internal static bool readyOpener = false;
    internal static bool openerStarted = false;
    internal static byte step = 0;






    protected override IAction? CountDownAction(float remainTime)
    {
        var IsThereABoss = AllHostileTargets.Any(p => p.IsBossFromTTK()) || AllHostileTargets.Any(p => p.IsBossFromIcon());
        int medicaThreshold = PartyMembers.Where(o => o.DistanceToPlayer() < 20).Count();


        if (remainTime < 0.2)
        {
            
            if (IsOffCooldown(BarrelStabilizerPvE) && IsOffCooldown(AirAnchorPvE) && IsOffCooldown(WildfirePvE) && IsOffCooldown(ChainSawPvE) && IsOffCooldown(DrillPvE)
            && GaussRoundPvE.Cooldown.CurrentCharges == 3 && RicochetPvE.Cooldown.CurrentCharges == 3 && ReassemblePvE.Cooldown.CurrentCharges == 2
            && !InCombat && !inOpener && !openerStarted && Player.Level == 90)
            {
                readyOpener = true;
                inOpener = false;
                step = 0;
            }
            if ((readyOpener || openerStarted) && !inOpener) { openerStarted = true; } else { openerStarted = false; }
            if (HeatedSplitShotPvE.CanUse(out var act)) return act;
            
        }

        if (remainTime < 2.0 && IsThereABoss)
        {
            if (UseBurstMedicine(out var act, clippingCheck:false)) return act;
        }

        return base.CountDownAction(remainTime);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        /*
        if (ChainSawPvE.EnoughLevel && nextGCD.IsTheSameTo(true, ChainSawPvE))
        {
            if (ReassemblePvE.CanUse(out act, usedUp: true)) return true;
        }

        if (!DrillPvE.EnoughLevel && nextGCD.IsTheSameTo(true, CleanShotPvE)
            || nextGCD.IsTheSameTo(false, AirAnchorPvE, ChainSawPvE, DrillPvE))
        {
            if (ReassemblePvE.CanUse(out act, usedUp: true)) return true;
        }
        */
        return base.EmergencyAbility(nextGCD, out act);
    }

    protected unsafe override bool GeneralGCD(out IAction? act)
    {
        
        //opener
        if (openerStarted && IsLastGCD(true, HeatedSplitShotPvE)) { inOpener = true; openerStarted = false; readyOpener = false; }
        

        // Reset check for opener
        if (IsOffCooldown(BarrelStabilizerPvE) && IsOffCooldown(AirAnchorPvE) && IsOffCooldown(WildfirePvE) && IsOffCooldown(ChainSawPvE) && IsOffCooldown(DrillPvE)
            && GaussRoundPvE.Cooldown.CurrentCharges == 2 && RicochetPvE.Cooldown.CurrentCharges == 2 && ReassemblePvE.Cooldown.CurrentCharges == 2
            && !InCombat && !inOpener && !openerStarted && Player.Level == 90)
        {
            readyOpener = true;
            inOpener = false;
            step = 0;
        }
        else
        { readyOpener = false; }

        // Reset if opener is interrupted, requires step 0 and 1 to be explicit since the inCombat check can be slow
        if ((step == 0 && IsLastAbility(true, GaussRoundPvE))
            || (inOpener && step >= 1 && !InCombat)) inOpener = false;

        // Start Opener
        if (inOpener)
        {

            //we do it in steps to be able to control it
            if (step == 0)
            {
                if (IsLastAbility(true,GaussRoundPvE)) step++;
                else if(GaussRoundPvE.CanUse(out act)) return true;
            }
            /*
            if (step == 1)
            {
                if (IsOnCooldown(All.Swiftcast)) step++;
                else return All.Swiftcast;
            }

            if (step == 2)
            {
                if (GetRemainingCharges(Acceleration) < 2) step++;
                else return Acceleration;
            }

            if (step == 3)
            {
                if (lastComboMove == Verthunder3 && !HasEffect(Buffs.Acceleration)) step++;
                else return Verthunder3;
            }

            if (step == 4)
            {
                if (lastComboMove == Verthunder3 && !HasEffect(All.Buffs.Swiftcast)) step++;
                else return Verthunder3;
            }

            if (step == 5)
            {
                if (IsOnCooldown(Embolden)) step++;
                else return Embolden;
            }

            if (step == 6)
            {
                if (IsOnCooldown(Manafication)) step++;
                else return Manafication;
            }

            if (step == 7)
            {
                if (lastComboMove == Riposte) step++;
                else return EnchantedRiposte;
            }

            if (step == 8)
            {
                if (IsOnCooldown(Fleche)) step++;
                else return Fleche;
            }

            if (step == 9)
            {
                if (lastComboMove == Zwerchhau) step++;
                else return EnchantedZwerchhau;
            }

            if (step == 10)
            {
                if (IsOnCooldown(ContreSixte)) step++;
                else return ContreSixte;
            }

            if (step == 11)
            {
                if (lastComboMove == Redoublement || Gauge.ManaStacks == 3) step++;
                else return EnchantedRedoublement;
            }

            if (step == 12)
            {
                if (GetRemainingCharges(Corpsacorps) < 2) step++;
                else return Corpsacorps;
            }

            if (step == 13)
            {
                if (GetRemainingCharges(Engagement) < 2) step++;
                else return Engagement;
            }

            if (step == 14)
            {
                if (lastComboMove == Verholy) step++;
                else return Verholy;
            }

            if (step == 15)
            {
                if (GetRemainingCharges(Corpsacorps) < 1) step++;
                else return Corpsacorps;
            }

            if (step == 16)
            {
                if (GetRemainingCharges(Engagement) < 1) step++;
                else return Engagement;
            }

            if (step == 17)
            {
                if (lastComboMove == Scorch) step++;
                else return Scorch;
            }

            if (step == 18)
            {
                if (lastComboMove == Resolution) step++;
                else return Resolution;
            }
            */
            inOpener = false;
        }
        
        //1-2-3 Combo
        //aoe
        if (SpreadShotPvE.CanUse(out act)) return true;
        //Single
        if (CleanShotPvE.CanUse(out act)) return true;
        if (SlugShotPvE.CanUse(out act)) return true;
        if (SplitShotPvE.CanUse(out act)) return true;



        return base.GeneralGCD(out act);
    }

    protected unsafe override bool AttackAbility(IAction nextGCD, out IAction? act)
    { 
        return base.AttackAbility(nextGCD, out act);
    }
    
}