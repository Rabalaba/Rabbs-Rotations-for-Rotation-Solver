using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Utility.Signatures;
using ExCSS;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Gauge;
using RotationSolver.Basic.Rotations.Basic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using static DefaultRotations.Magical.BobRoss;
using static FFXIVClientStructs.FFXIV.Client.UI.Misc.DataCenterHelper;


namespace RabbsRotationsNET8.Magical;
[Rotation("Rabbs Blackest Mage for Level 100 Mages only", CombatType.PvE, GameVersion = "7.2")]
[SourceCode(Path = "main/BasicRotations/Magical/BLM_Beta.cs")]
[Api(4)]

public sealed class BLM_Gamma : BlackMageRotation
{
    #region Config Options
    [RotationConfig(CombatType.PvE, Name = "Use Opener (Level 100 only)")]
    public bool Useopener { get; set; } = false;

    [RotationConfig(CombatType.PvE, Name = "Use Countdown Ability")]
    public bool Usecountdown { get; set; } = false;

    [RotationConfig(CombatType.PvE, Name = "Which Opener to use")]
    public Openchoice Openerchoice { get; set; } = Openchoice.Standard;
    [Range(1, 2, ConfigUnitType.None, 1)]

    [RotationConfig(CombatType.PvE, Name = "Use Leylines in combat when standing still")]
    public bool LeylineMadness { get; set; } = false;

    [RotationConfig(CombatType.PvE, Name = "Use both stacks of Leylines automatically")]
    public bool Leyline2Madness { get; set; } = false;

    [RotationConfig(CombatType.PvE, Name = "Use Retrace when out of Leylines in combat and standing still")]
    public bool UseRetrace { get; set; } = false;

    [RotationConfig(CombatType.PvE, Name = "Use Gemdraught/Tincture/pot")]
    public bool UseMedicine { get; set; } = false;

    public enum Openchoice : byte
    {
        [Description("Standard 5+7 Opener")] Standard,
        [Description("Alternative Flare Opener")] AltFlare
    }
    #endregion

    #region Config Under Hood Stuff


    #endregion

    #region underhood stuff
    public bool TargetHasThunderDebuff => Target is not null && Target.HasStatus(true, StatusID.Thunder, StatusID.ThunderIi, StatusID.ThunderIii, StatusID.ThunderIv, StatusID.HighThunder_3872, StatusID.HighThunder);
    public bool ThunderBuffAboutToFallOff => Target is not null && TargetHasThunderDebuff && Target.StatusTime(true, StatusID.Thunder, StatusID.ThunderIi, StatusID.ThunderIii, StatusID.ThunderIv, StatusID.HighThunder_3872, StatusID.HighThunder) < 3;
    public bool ShouldThunder => Player.HasStatus(true, StatusID.Thunderhead) && (!TargetHasThunderDebuff || ThunderBuffAboutToFallOff);
    private Stopwatch? noHostilesTimer = null;

    public double GetTimeSinceNoHostilesInCombat()
    {
        if (InCombat && NumberOfAllHostilesInMaxRange == 0)
        {
            if (noHostilesTimer == null)
            {
                noHostilesTimer = Stopwatch.StartNew();
            }
            return noHostilesTimer.Elapsed.TotalSeconds;
        }
        else
        {
            noHostilesTimer?.Stop(); // Stop the timer if it's running
            noHostilesTimer = null;
            return 0.0;
        }
    }
    public bool WillBeAbleToFlareStarST
    {
        get
        {
            const int baseFireFourCost = 1600;
            const int fireFourCostWithHeart = 800;
            int soulDeficit = 6 - AstralSoulStacks;
            int discountedCasts = Math.Min(soulDeficit, UmbralHearts);
            int normalCasts = soulDeficit - discountedCasts;



            // If Manafont is available, we can likely cast enough spells to get 6 stacks
            // before running out of MP (given its significant MP recovery).
            if (!ManafontPvE.Cooldown.IsCoolingDown)
            {
                return true;
            }

            // If we already have 6 stacks, we can cast Flare Star (0 MP cost)
            if (AstralSoulStacks == 6)
            {
                return true;
            }
            // calculate the mp needed to get to 6 stacks
            int howMuchManaINeed = (discountedCasts * fireFourCostWithHeart) + (normalCasts * baseFireFourCost);

            if (CurrentMp > howMuchManaINeed) 
            { 
                return true; 
            }


            return false;
        }
    }
    public bool WillBeAbleToFlareStarMT
    {
        get
        {
            int soulDeficit = 6 - AstralSoulStacks;
            int flaresNeeded = (int)Math.Ceiling((double)soulDeficit / 3); // Flare grants 3 stacks

            // Manafont check still applies
            if (!ManafontPvE.Cooldown.IsCoolingDown || AstralSoulStacks == 6)
            {
                return true;
            }

            if (flaresNeeded > 2)
            { return false; }

            if (flaresNeeded == 2)
            {
                if (UmbralHearts > 0 && CurrentMp >= 2400)
                { 
                    return true; 
                }
            }
            if (flaresNeeded == 1)
                if (CurrentMp > 800)
                {
                    return true;
                }


            return false; // If we can afford all the needed Flare casts
        }
    }

    //public static IBaseAction RainbowPrePull { get; } = new BaseAction((ActionID)34688);

    public IBaseAction FreezePvE2 => _FreezePvECreator2.Value;

    private static void ModifyFreezePvE(ref ActionSetting setting)
    {
        //setting.ActionCheck = () => InUmbralIce && UmbralHearts == 0;
        setting.RotationCheck = () => InUmbralIce;
        setting.UnlockedByQuestID = 66611u;
        
        //setting.CreateConfig = () => new ActionConfig

    }

    private readonly Lazy<IBaseAction> _FreezePvECreator2 = new Lazy<IBaseAction>(delegate
    {
        IBaseAction action460 = new BaseAction(ActionID.FreezePvE);
        ActionSetting setting460 = action460.Setting;
        ModifyFreezePvE(ref setting460);
        action460.Setting = setting460;
        return action460;
    });

    public bool shouldTranspose
    {
        get
        {
            var recentActions = RecordActions.Take(2);
            if (InUmbralIce && recentActions.Any(x => x.Action.RowId == FreezePvE.ID))
            {
                return true;
            }
            if (GetTimeSinceNoHostilesInCombat() > 5f) //we are in combat but nothing to attack for 5 seconds
            {
                if (!IsParadoxActive)
                    { return true; }
                if (!HasThunder)
                { return true; }
            }
            if (InAstralFire)
            {
                if (!FlarePvE.CanUse(out _) && !DespairPvE.CanUse(out _) && !FlareStarPvE.CanUse(out _) && !FireIvPvE.CanUse(out _) && !ParadoxPvE.CanUse(out _) && ManafontPvE.Cooldown.IsCoolingDown)
                {
                    return true;
                }
            }
            if (InUmbralIce)
            {
                if (UmbralHearts == 3 && UmbralIceStacks == 3 && !IsParadoxActive)
                    { return true; }
            }

            return false;
        }
    }
    #endregion

    #region Countdown
    protected override IAction? CountDownAction(float remainTime)
    {
        IAction act;
        if (remainTime < FireIiiPvE.Info.CastTime + CountDownAhead && remainTime > 1)
        {
            if (FireIiiPvE.CanUse(out act)) return act;
        }
        if (remainTime < FireIiiPvE.Info.CastTime - CountDownAhead && IsMoving)
        {
            if (!NextGCDisInstant)
            {
                if (CanMakeInstant)
                {
                    if (SwiftcastPvE.CanUse(out act)) return act;
                    if (TriplecastPvE.CanUse(out act, usedUp: true)) return act;
                }
            }
        }
            return base.CountDownAction(remainTime);
    }
    #endregion

    #region Additional oGCD Logic

    [RotationDesc]
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (shouldTranspose)
        {
            if (TransposePvE.CanUse(out act, skipCastingCheck:true, skipComboCheck:true, skipAoeCheck:true, skipStatusProvideCheck:true, skipTargetStatusNeedCheck:true, skipTTKCheck:true, usedUp:true)) return true;
        }
        if (!ManafontPvE.Cooldown.IsCoolingDown && CurrentMp < 800 && AstralSoulStacks < 6 && InAstralFire)
        {
            if (ManafontPvE.CanUse(out act, skipCastingCheck: true, skipComboCheck: true, skipAoeCheck: true, skipStatusProvideCheck: true, skipTargetStatusNeedCheck: true, skipTTKCheck: true, usedUp: true)) return true;
        }

        return base.EmergencyAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.AetherialManipulationPvE)]
    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
    {
        if (AetherialManipulationPvE.CanUse(out act)) return true;
        return base.MoveForwardAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.BetweenTheLinesPvE)]
    protected override bool MoveBackAbility(IAction nextGCD, out IAction? act)
    {
        if (BetweenTheLinesPvE.CanUse(out act)) return true;
        return base.MoveBackAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.ManawardPvE)]
    protected sealed override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
    {
        if (ManawardPvE.CanUse(out act)) return true;
        return base.DefenseAreaAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.ManawardPvE, ActionID.AddlePvE)]
    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? act)
    {
        if (ManawardPvE.CanUse(out act)) return true;
        if (AddlePvE.CanUse(out act)) return true;
        return base.DefenseSingleAbility(nextGCD, out act);
    }
    #endregion

    #region oGCD Logic

    [RotationDesc(ActionID.TransposePvE, ActionID.LeyLinesPvE, ActionID.RetracePvE)]
    protected override bool GeneralAbility(IAction nextGCD, out IAction? act)
    {


        #region Opener
        if (Useopener && CombatTime < 30 && InCombat && !Player.HasStatus(true, StatusID.BrinkOfDeath, StatusID.Weakness))
        {
            if (AmplifierPvE.Cooldown.IsCoolingDown)
            {
                if (LeyLinesPvE.CanUse(out act)) return true;
            }
        }
        
        #endregion
        if (LeylineMadness && InCombat && HasHostilesInRange && LeyLinesPvE.CanUse(out act, usedUp: Leyline2Madness)) return true;
        if (!IsLastAbility(ActionID.LeyLinesPvE) && UseRetrace && InCombat && HasHostilesInRange && !Player.HasStatus(true, StatusID.CircleOfPower) && RetracePvE.CanUse(out act)) return true;

        return base.GeneralAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.RetracePvE, ActionID.SwiftcastPvE, ActionID.TriplecastPvE, ActionID.AmplifierPvE)]
    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {

        if (InCombat && HasHostilesInRange)
        {
            if (InUmbralIce)
            {
                if (IsLastAction(ActionID.TransposePvE))
                {
                    if (CanMakeInstant)
                    {
                        if (SwiftcastPvE.CanUse(out act)) return true;
                        if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                    }
                }
            }
            if (InAstralFire)
            {
                #region Opener
                if (Useopener && CombatTime < 30 && InCombat && !Player.HasStatus(true, StatusID.BrinkOfDeath, StatusID.Weakness))
                {
                    if (!NextGCDisInstant)
                    {
                        if (SwiftcastPvE.CanUse(out act)) return true;
                    }
                    if (!IsPolyglotStacksMaxed)
                    {
                        if (AmplifierPvE.CanUse(out act)) return true;
                    }
                    if (UseMedicine && UseBurstMedicine(out act)) return true;

                }
                #endregion
                
            }
            if (CombatTime > 30 || !Useopener)
            {
                if (!IsPolyglotStacksMaxed)
                {
                    if (AmplifierPvE.CanUse(out act)) return true;
                }
                if (UseMedicine && UseBurstMedicine(out act)) return true;
            }
                
        }

        return base.AttackAbility(nextGCD, out act);
    }
    #endregion

    #region GCD Logic

    protected override bool GeneralGCD(out IAction? act)
    {
        var isTargetBoss = CurrentTarget?.IsBossFromTTK() ?? false;
        var isTargetDying = CurrentTarget?.IsDying() ?? false;
        var recentActions = RecordActions.Take(4);
        if (IsLastAction(ActionID.FreezePvE))
        {
            if (FoulPvE.CanUse(out act)) return true;
            if (ThunderIiPvE.CanUse(out act)) return true;
        }
        
        if (IsPolyglotStacksMaxed)
        {
            if (FoulPvE.CanUse(out act)) return true;
            if (XenoglossyPvE.CanUse(out act)) return true;
        }
        if (!InAstralFire && !InUmbralIce && InCombat)
        {
            if (CanMakeInstant && !NextGCDisInstant && CurrentMp > BlizzardIiiPvE.Info.MPNeed)
            {
                if (SwiftcastPvE.CanUse(out act)) return true;
                if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
            }
            if (FireIiiPvE.CanUse(out act)) return true;
        }
        if (ThunderIiPvE.CanUse(out act) && ShouldThunder) return true;
        if (ThunderPvE.CanUse(out act) && ShouldThunder) return true;
        if (isTargetBoss && isTargetDying)
        {
            if (FoulPvE.CanUse(out act)) return true;
            if (XenoglossyPvE.CanUse(out act)) return true;
        }
        if (FreezePvE2.CanUse(out act) && UmbralHearts < 3) return true;
        if (FlareStarPvE.CanUse(out act)) return true;
        if (FlarePvE.CanUse(out act) && AstralSoulStacks > 3 && CurrentMp > 1200) return true;
        if (InUmbralIce && !recentActions.Any(x => x.Action.RowId == FreezePvE.ID))
        {
            
            if ((InCombat && GetTimeSinceNoHostilesInCombat() > 5f) || (!InCombat && TimeSinceLastAction.TotalSeconds > 4.5))
            {
                if (UmbralIceStacks < 3 || UmbralHearts < 3)
                {
                    if (UmbralSoulPvE.CanUse(out act)) return true;
                }   
            }
            if (FreezePvE.CanUse(out act)) return true;

            if (UmbralIceStacks < 3)
            {
                if (BlizzardIiiPvE.CanUse(out act)) return true;
            }
            if (UmbralHearts < 3 || (CurrentMp < 10000 && !IsLastAction(ActionID.BlizzardIvPvE)))
            {
                if (BlizzardIvPvE.CanUse(out act)) return true;
            }
            if (ParadoxPvEReady)
            {
                if (BlizzardPvE.CanUse(out act)) return true;
            }

        }
        if (InAstralFire)
        {
            if ((InCombat && GetTimeSinceNoHostilesInCombat() > 5f) || (!InCombat && TimeSinceLastAction.TotalSeconds > 4.5))
            {
                if (TransposePvE.CanUse(out act)) return true;
            }
            if (AstralFireStacks < 3 && (HasFire || ParadoxPvEReady))
            {
                if (HasFire)
                {
                    if (FireIiiPvE.CanUse(out act)) return true;
                }
                if (ParadoxPvEReady && !IsLastAbility(ActionID.FireIiiPvE))
                {
                    if (ParadoxPvE.CanUse(out act)) return true;
                }
            }
            // general polyglot stuff here for openers and cap prevention

            if (!ManafontPvE.Cooldown.IsCoolingDown && Openerchoice == Openchoice.Standard && CombatTime < 30)
            {
                if (CurrentMp < FireIvPvE.Info.MPNeed)
                {
                    if (FoulPvE.CanUse(out act)) return true;
                    if (XenoglossyPvE.CanUse(out act)) return true;
                }
            }
            if (!ManafontPvE.Cooldown.IsCoolingDown && Openerchoice == Openchoice.AltFlare && CombatTime < 30)
            {
                if (Player.HasStatus(true, StatusID.CircleOfPower))
                {
                    if (FoulPvE.CanUse(out act)) return true;
                    if (XenoglossyPvE.CanUse(out act)) return true;
                }
                if (AstralSoulStacks == 4 && !HasFire && !ParadoxPvEReady && CurrentMp == 1600)
                {
                    if (DespairPvE.CanUse(out act)) return true;
                }
            }
                /// setting rotation for in combat and planted, moving will be set seperately
                if (InCombat && (!IsMoving || NextGCDisInstant))
            {
                if (ParadoxPvE.CanUse(out act) && ManafontPvE.Cooldown.WillHaveOneChargeGCD(3)) return true;

                if (WillBeAbleToFlareStarMT || WillBeAbleToFlareStarST)
                {
                    if (FlareStarPvE.CanUse(out act)) return true;
                    if (FlarePvE.CanUse(out var flareAction))
                    {
                        if (WillBeAbleToFlareStarST && !WillBeAbleToFlareStarMT)
                        {
                            uint flareCost = FlarePvE.Info.MPNeed;
                            uint remainingMpAfterFlare = CurrentMp - flareCost;

                            const int baseFireFourCost = 1600;
                            const int fireFourCostWithHeart = 800;
                            int soulDeficitAfterFlare = 6 - (AstralSoulStacks + 3);
                            int discountedCastsAfterFlare = Math.Min(soulDeficitAfterFlare, UmbralHearts);
                            int normalCastsAfterFlare = soulDeficitAfterFlare - discountedCastsAfterFlare;

                            if (!ManafontPvE.Cooldown.IsCoolingDown || (AstralSoulStacks + 3) == 6 || (remainingMpAfterFlare > (discountedCastsAfterFlare * fireFourCostWithHeart) + (normalCastsAfterFlare * baseFireFourCost)))
                            {
                                act = flareAction;
                                return true;
                            }
                            else
                            {
                                act = null;
                                return false;
                            }
                        }
                        else
                        {
                            act = flareAction;
                            return true;
                        }
                    }
                    if (!WillBeAbleToFlareStarST)
                    {
                        if (FlarePvE.CanUse(out act, skipAoeCheck: true)) return true;
                    }
                    if (FireIvPvE.CanUse(out act)) return true;
                }
                if (IsParadoxActive)
                {
                    if (ParadoxPvE.CanUse(out act, skipStatusProvideCheck:true)) return true;
                }
                if (!IsParadoxActive || CurrentMp < 1600)
                {
                    if (DespairPvE.CanUse(out act)) return true;
                }
            }
            if (InCombat && IsMoving && !NextGCDisInstant && HasHostilesInRange)
            {
                //before wasting triplecast or swiftcast lets check for intant cast stuff, we like to save swiftcast for b3 to start ice phase.
                //first consider xenoglossy
                if (PolyglotStacks > 0)
                {
                    if (FoulPvE.CanUse(out act)) return true;
                    if (XenoglossyPvE.CanUse(out act)) return true;
                }

                // can we use a paradox instant here? general rule is if you do not have firestarter proc it can be used
                if (ParadoxPvEReady && !HasFire)
                {
                    if (ParadoxPvE.CanUse(out act)) return true;
                }
                
                //we can check for fire3 via firestarter
                if (HasFire)
                {
                    if (FireIiiPvE.CanUse(out act)) return true;
                }

                //lastly despair can be used only when flarestar is unusable this fire season
                if (!WillBeAbleToFlareStarMT && !WillBeAbleToFlareStarST)
                {
                    if (DespairPvE.CanUse(out act)) return true;
                }

                //if we reached here then flarestar is salvageable and we should use triplecast if available first and swiftcast second(saving for b3)
                if (CanMakeInstant)
                {
                    
                    if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;

                    if (SwiftcastPvE.CanUse(out act)) return true;
                }
            }

        }
        //fallback if here something is messed up

        return base.GeneralGCD(out act);
    }

    #endregion

    #region Black Magic

    #endregion
    public unsafe override void DisplayStatus()
    {
        //motif
        ImGui.Text("WillBeAbleToFlareStarMT " + WillBeAbleToFlareStarMT);
        ImGui.Text("WillBeAbleToFlareStarST " + WillBeAbleToFlareStarST);
        ImGui.Text("soulstack " + SoulStackCount);
        ImGui.Text("even minute " + IsWithinFirst15SecondsOfEvenMinute());
        ImGui.Text("Combat Time " + CombatTime);
        ImGui.Text("CombatNoEnemiesTimer " + GetTimeSinceNoHostilesInCombat());
        ImGui.Text("shouldTranspose " + shouldTranspose);
        ImGui.Text("RealAstralDefecit " + (ThisManyInstantCasts + AstralSoulStacks));
        ImGui.Text("Fire3 time " + FireIiiPvE.Info.CastTime);

        base.DisplayStatus();
    }
}