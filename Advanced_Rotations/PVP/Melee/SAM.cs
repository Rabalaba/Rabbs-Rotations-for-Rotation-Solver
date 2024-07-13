

using FFXIVClientStructs.FFXIV.Client.Game.UI;
using static FFXIVClientStructs.FFXIV.Client.UI.Misc.DataCenterHelper;

namespace DefaultRotations.Melee;

[Rotation("RabbsPvP", CombatType.PvP, GameVersion = "7.00", Description = "Beta Rotation")]
[SourceCode(Path = "main/DefaultRotations/PVPRotations/Tank/SAM_Default.PvP.cs")]
[Api(2)]
public sealed class SAM_DefaultPvP : SamuraiRotation
{
    [RotationConfig(CombatType.PvP, Name = "Sprint")]
    public bool UseSprintPvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "Recuperate")]
    public bool UseRecuperatePvP { get; set; } = false;

    [Range(1, 100, ConfigUnitType.Percent, 1)]
    [RotationConfig(CombatType.PvP, Name = "RecuperateHP%%?")]
    public int RCValue { get; set; } = 75;

    [RotationConfig(CombatType.PvP, Name = "Use Purify")]
    public bool UsePurifyPvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "Use Purify on Stun")]
    public bool Use1343PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "Use Purify on DeepFreeze")]
    public bool Use3219PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "Use Purify on HalfAsleep")]
    public bool Use3022PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "Use Purify on Sleep")]
    public bool Use1348PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "Use Purify on Bind")]
    public bool Use1345PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "Use Purify on Heavy")]
    public bool Use1344PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "Use Purify on Silence")]
    public bool Use1347PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool GuardCancel { get; set; } = false;

    public static IBaseAction Zantetsuken { get; } = new BaseAction((ActionID)29537);

    public static IBaseAction Dash { get; } = new BaseAction((ActionID)29532);

    private bool TryPurify(out IAction? action)
    {
        action = null;
        if (!UsePurifyPvP) return false;

        var purifyStatuses = new Dictionary<int, bool>
        {
            { 1343, Use1343PvP },
            { 3219, Use3219PvP },
            { 3022, Use3022PvP },
            { 1348, Use1348PvP },
            { 1345, Use1345PvP },
            { 1344, Use1344PvP },
            { 1347, Use1347PvP }
        };

        foreach (var status in purifyStatuses)
        {
            if (status.Value && Player.HasStatus(true, (StatusID)status.Key))
            {
                return PurifyPvP.CanUse(out action);
            }
        }

        return false;
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
        if (GuardCancel && Player.HasStatus(true, StatusID.Guard)) return false;
        if (TryPurify(out act)) return true;
        if (UseRecuperatePvP && Player.CurrentHp / Player.MaxHp * 100 < RCValue && RecuperatePvP.CanUse(out act)) return true;
        if (Dash.CanUse(out act, usedUp: true, skipAoeCheck: true, skipCastingCheck: true, skipComboCheck: true) && Dash.Target.Target?.DistanceToPlayer() > 5) return true;
        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
        if (GuardCancel && Player.HasStatus(true, StatusID.Guard)) return false;
        if (Dash.CanUse(out act, usedUp:true, skipAoeCheck:true, skipCastingCheck:true, skipComboCheck:true) && Dash.Target.Target?.DistanceToPlayer() > 5) return true;
        if (MineuchiPvP.CanUse(out act)) return true;
        if (MeikyoShisuiPvP.CanUse(out act)) return true;
        return base.AttackAbility(nextGCD, out act);
    }
    protected override bool GeneralAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
        if (GuardCancel && Player.HasStatus(true, StatusID.Guard)) return false;

        return base.GeneralAbility(nextGCD, out act);
    }
    protected unsafe override bool GeneralGCD(out IAction? act)
    {
        act = null;
        if (UIState.Instance()->LimitBreakController.CurrentUnits >= 4000)
        {
            if (HissatsuChitenPvP.CanUse(out act)) return true;
            if (Zantetsuken.CanUse(out act, skipAoeCheck:true) && (Zantetsuken.Target.Target?.HasStatus(true, StatusID.Kuzushi) ?? false)) return true;
        }
                // Early exits for Guard status or Sprint usage
                if (GuardCancel && Player.HasStatus(true, StatusID.Guard)) return false;
        if (!Player.HasStatus(true, StatusID.Guard) && UseSprintPvP && !Player.HasStatus(true, StatusID.Sprint) && !InCombat && SprintPvP.CanUse(out act)) return true;
        if (KaeshiNamikiriPvP.CanUse(out act, skipAoeCheck: true) && OgiNamikiriPvP.AdjustedID == KaeshiNamikiriPvP.ID) return true;
        if (OgiNamikiriPvP.CanUse(out act, skipAoeCheck:true)) return true;
        if (MidareSetsugekkaPvP.CanUse(out act, skipAoeCheck: true) && Player.HasStatus(true, StatusID.Midare)) return true;

        

        if (KashaPvP.CanUse(out act)) return true;
        if (GekkoPvP.CanUse(out act)) return true;
        if (YukikazePvP.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }


    public unsafe override void DisplayStatus()
    {
        //motif
        ImGui.Text("debuf " + UIState.Instance()->LimitBreakController.CurrentUnits.ToString());

        base.DisplayStatus();
    }
}
