
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Gauge;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using ImGuiNET;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using System.ComponentModel;
using static DefaultRotations.Magical.BobRoss;
using Lumina.Excel.Sheets;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using System;

namespace RabbsRotationsNET8.Magical;

[Rotation("Boring InfininePowerdox", CombatType.PvE, GameVersion = "7.11")]
[SourceCode(Path = "main/DefaultRotations/Magical/BLM_IP.cs")]
[Api(4)]
public class BLM_IP : BlackMageRotation
{
    #region Config Options
    public override MedicineType MedicineType => MedicineType.Intelligence;
    [RotationConfig(CombatType.PvE, Name = "Super Janky LeyLines")]
    public Lluse Llusage { get; set; } = Lluse.Never;
    [RotationConfig(CombatType.PvE, Name = "Use Potions")]
    public PotionUse PotUseage { get; set; } = PotionUse.Never;
    public enum PotionUse : byte
    {
        [Description("OnCooldown")] On_Cooldown,
        [Description("Never")] Never,
        [Description("Every2Minutes")] Every_2_Mins,
        [Description("WhenSomeoneelsesmarterthanmePots")] With_Others
    }
    public enum Lluse : byte
    {
        [Description("OnCooldown")] On_Cooldown,
        [Description("Never")] Never,
        [Description("Every2Minutes")] Every_2_Mins,
        [Description("WhenSomeoneelsesmarterthanmePots")] With_Others
    }

    #endregion

    public IBaseAction FixedUS { get; } = new BaseAction((ActionID)16506);

    public IBaseAction FixedMF { get; } = new BaseAction((ActionID)158);

    public IBaseAction FixedB4 { get; } = new BaseAction((ActionID)3576);

    public IBaseAction FixedLL { get; } = new BaseAction((ActionID)3573);

    public IBaseAction FixedB3 { get; } = new BaseAction((ActionID)154);

    public IBaseAction FixedHT { get; } = new BaseAction((ActionID)144);


    public IBaseItem Potion2 { get; } = new BaseItem(44165);

    //ActionManager.Instance()->UseAction(ActionType.Action, adjustedID, num, 0u, ActionManager.UseActionMode.None, 0u, null);


    bool IsTargetTargetable = HostileTarget?.IsTargetable ?? false;



    public static bool IsEvenMinute()
    {
        // Get whole minutes from CombatTime (assuming it represents seconds)
        int minutes = (int)Math.Floor(CombatTime / 60f);

        // Use modulo to check if minutes is even (remainder 0)
        return minutes % 2 == 0;
    }

    public static bool IsWithinFirst15SecondsOfEvenMinute()
    {
        int minutes = (int)Math.Floor(CombatTime / 60f);
        int secondsInCurrentMinute = (int)Math.Floor(CombatTime % 60f);

        return minutes % 2 == 0 && secondsInCurrentMinute >= 0 && secondsInCurrentMinute < 15;
    }


    public bool NextGCDisInstant => Player.HasStatus(true, StatusID.Triplecast, StatusID.Swiftcast);

    public bool CanMakeInstant => TriplecastPvE.Cooldown.CurrentCharges > 0 || !SwiftcastPvE.Cooldown.IsCoolingDown;

    public bool Someone_is_bustin_a_nut => PartyMembers.Any(m => m.HasStatus(true, StatusID.Brotherhood, StatusID.SearingLight, StatusID.BattleLitany, StatusID.ArcaneCircle, StatusID.MagesBallad, StatusID.TechnicalFinish, StatusID.Embolden));

    public int ThisManyInstantCasts => (TriplecastPvE.Cooldown.CurrentCharges *3)  + Player.StatusStack(true, StatusID.Triplecast) + SwiftcastPvE.Cooldown.CurrentCharges;

    public int AstralDefecit => ThisManyInstantCasts - AstralSoulStacks;

    protected override IAction? CountDownAction(float remainTime)
    {
        IAction act;
        if (remainTime < BlizzardIiiPvE.Info.CastTime + CountDownAhead)
        {
            if (BlizzardIiiPvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true, skipComboCheck: true, skipAoeCheck: true, usedUp: true)) return act;
        }
        return base.CountDownAction(remainTime);
    }

    protected override unsafe bool AttackAbility(IAction nextGCD, out IAction? act)
    {

        if (InCombat && IsTargetTargetable)
        {


            switch (PotUseage)
            {
                case PotionUse.On_Cooldown:

                        if (UseBurstMedicine(out act)) return true;
                    break;
                case PotionUse.Every_2_Mins:
                    if(UseBurstMedicine(out act) && IsWithinFirst15SecondsOfEvenMinute()) return true;
                    break;
                case PotionUse.With_Others:
                    if (UseBurstMedicine(out act) && PartyMembers.Any(m => m.HasStatus(true, StatusID.Medicated))) return true;
                    break;
                case PotionUse.Never:
                    break;
            }

            switch (Llusage)
            {
                case Lluse.On_Cooldown:
                    if (LeyLinesPvE.Cooldown.HasOneCharge && !Player.HasStatus(true, StatusID.LeyLines))
                    {
                        if (LeyLinesPvE.CanUse(out act, usedUp: true)) return true;
                        //ActionManager.Instance()->UseAction(ActionType.Action, LeyLinesPvE.AdjustedID);
                    }
                    break;
                case Lluse.Every_2_Mins:
                    if (LeyLinesPvE.Cooldown.HasOneCharge && IsWithinFirst15SecondsOfEvenMinute() && !Player.HasStatus(true, StatusID.LeyLines))
                    {
                        if (LeyLinesPvE.CanUse(out act, usedUp: true)) return true;
                        //ActionManager.Instance()->UseAction(ActionType.Action, LeyLinesPvE.AdjustedID);
                    }
                    break;
                case Lluse.With_Others:
                    if (LeyLinesPvE.Cooldown.HasOneCharge && Someone_is_bustin_a_nut && !Player.HasStatus(true, StatusID.LeyLines))
                    {
                        if (LeyLinesPvE.CanUse(out act, usedUp: true)) return true;
                        //ActionManager.Instance()->UseAction(ActionType.Action, LeyLinesPvE.AdjustedID);
                    }
                    break;
                case Lluse.Never:
                    break;
            }
            if (InAstralFire && CurrentMp < 800)
            {
                if (!ManafontPvE.Cooldown.IsCoolingDown)
                {
                    if (ManafontPvE.CanUse(out act)) return true;
                }
            }
            if (!IsPolyglotStacksMaxed)
            {
                if (AmplifierPvE.CanUse(out act)) return true;
            }
            if (CanMakeInstant && InUmbralIce && !IsParadoxActive)
            {
                if (SwiftcastPvE.CanUse(out act)) return true;
                if (TriplecastPvE.CanUse(out act, usedUp:true)) return true;
            }

        }

        

        return base.AttackAbility(nextGCD, out act);
    }

    protected unsafe override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        

        return base.EmergencyAbility(nextGCD, out act);
    }

    protected unsafe override bool GeneralGCD(out IAction? act)
    {
        if (IsPolyglotStacksMaxed || Someone_is_bustin_a_nut || Player.HasStatus(true, StatusID.LeyLines))
        {
            if (FoulPvE.CanUse(out act, usedUp:true)) return true;
            if (XenoglossyPvE.CanUse(out act, usedUp: true)) return true;
        }
        if (HostileTarget != null && (!HostileTarget.HasStatus(true, StatusID.Thunder, StatusID.ThunderIi, StatusID.ThunderIii, StatusID.ThunderIv, StatusID.HighThunder_3872, StatusID.HighThunder) || HostileTarget.StatusTime(true, StatusID.Thunder, StatusID.ThunderIi, StatusID.ThunderIii, StatusID.ThunderIv, StatusID.HighThunder_3872, StatusID.HighThunder) < 3) && Player.HasStatus(true, StatusID.Thunderhead))
        {
            ActionManager.Instance()->UseAction(ActionType.Action, 36986, HostileTarget.EntityId, 0, ActionManager.UseActionMode.Queue, 0, null);
        }
        if (ParadoxPvE.CanUse(out act)) return true;
        if (NextGCDisInstant && InUmbralIce)
        {
            if (UmbralIceStacks < 3)
            {
                if (BlizzardIiiPvE.CanUse(out act)) return true;
            }
            if (UmbralHearts < 3)
            {
                if (BlizzardIvPvE.CanUse(out act)) return true;
            }
        }
        if (Player.HasStatus(true, StatusID.Firestarter))
        {
            if (FireIiiPvE.CanUse(out act)) return true;
        }
        if (DespairPvE.CanUse(out act)) return true;
        if (AstralFireStacks == 3 || UmbralIceStacks == 3)
        {
            if (TransposePvE.CanUse(out act)) return true;
        }
        if (UmbralSoulPvE.CanUse(out act)) return true;
        return base.GeneralGCD(out act);
    }
    public unsafe override void DisplayStatus()
    {
        //motif
        ImGui.Text("insta " + ThisManyInstantCasts);
        ImGui.Text("InAstralFire " + InAstralFire);
        ImGui.Text("iAstralFireStacks " + AstralFireStacks);
        ImGui.Text("elementtime " + ElementTime);
        ImGui.Text("even minute " + IsWithinFirst15SecondsOfEvenMinute());
        ImGui.Text("Combat Time " + CombatTime);

        base.DisplayStatus();
    }


}
