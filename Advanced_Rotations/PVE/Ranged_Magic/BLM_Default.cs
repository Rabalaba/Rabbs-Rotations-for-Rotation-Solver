
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Gauge;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using ImGuiNET;
using RotationSolver.Basic.Helpers;

namespace RabbsRotationsNET8.Magical;

[Rotation("Infinite Power-dox2", CombatType.PvE, GameVersion = "7.11")]
[SourceCode(Path = "main/DefaultRotations/Magical/BLM_Default.cs")]
[Api(4)]
public class BLM_Default : BlackMageRotation
{

    public IBaseAction FixedUS { get; } = new BaseAction((ActionID)16506);

    public IBaseAction FixedMF { get; } = new BaseAction((ActionID)158);

    public IBaseAction FixedB4 { get; } = new BaseAction((ActionID)3576);

    public IBaseItem Potion2 { get; } = new BaseItem(44165);

    /// <summary>
    /// public class BaseItem : IBaseItem
    /// </summary>
    /// public class BaseAction : IBaseAction


    public bool NextGCDisInstant => Player.HasStatus(true, StatusID.Triplecast, StatusID.Swiftcast);

    public bool CanMakeInstant => TriplecastPvE.Cooldown.CurrentCharges > 0 || !SwiftcastPvE.Cooldown.IsCoolingDown;

    public int ThisManyInstantCasts => (TriplecastPvE.Cooldown.CurrentCharges *3)  + Player.StatusStack(true, StatusID.Triplecast) + SwiftcastPvE.Cooldown.CurrentCharges;

    public int AstralDefecit => ThisManyInstantCasts - AstralSoulStacks;

    protected override IAction? CountDownAction(float remainTime)
    {

        return base.CountDownAction(remainTime);
    }

    protected override unsafe bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        if (!Potion2.Cooldown.IsCoolingDown)
        {
            if (!Potion2.CanUse(out act)) return true;
        }

        if (LeyLinesPvE.Cooldown.HasOneCharge)
        {
            ActionManager.Instance()->UseAction(ActionType.Action, 3573);
        }

        if (ElementTime > 0 && ElementTimeEndAfter(1))
        {
            if (TransposePvE.CanUse(out act)) return true;
        }

            if (InCombat)
            {
            if (ThisManyInstantCasts > 6 && AstralSoulStacks < 6 && InAstralFire && ElementTime > 6 && Player.HasStatus(true, StatusID.Firestarter))
            {
                if (FixedMF.CanUse(out act)) return true;
            }
            if (ThisManyInstantCasts > 6 && AstralSoulStacks < 6 && InAstralFire && ElementTime > 6 && Player.HasStatus(true, StatusID.Firestarter) && Player.CurrentMp > 9000)
            {
                if (TriplecastPvE.CanUse(out act)) return true;
            }

            if (ThisManyInstantCasts > 3 && AstralSoulStacks == 3 && InAstralFire && ElementTime > 6)
            {
                if (TriplecastPvE.CanUse(out act, usedUp:true)) return true;
            }

            if (ThisManyInstantCasts == 1 && AstralSoulStacks == 6 && InAstralFire && ElementTime > 3)
            {
                if (SwiftcastPvE.CanUse(out act)) return true;
            }

            if (LeyLinesPvE.CanUse(out act)) return true;

            if (!IsPolyglotStacksMaxed)
                {
                    if (AmplifierPvE.CanUse(out act)) return true;
                }



            }
        

        return base.AttackAbility(nextGCD, out act);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {

        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool GeneralGCD(out IAction? act)
    {
        if (InAstralFire && ElementTime < 4 && Player.HasStatus(true, StatusID.Firestarter))
        {
                if (FireIiiPvE.CanUse(out act)) return true;
            
        }

            if (NextGCDisInstant)
        {

            if (AstralSoulStacks == 6)
            {
                if (FlareStarPvE.CanUse(out act)) return true;
            }
            if (FireIvPvE.CanUse(out act)) return true;
            
 
        }
        if (HostileTarget != null && (!HostileTarget.HasStatus(true, StatusID.Thunder, StatusID.ThunderIi, StatusID.ThunderIii, StatusID.ThunderIv, StatusID.HighThunder_3872, StatusID.HighThunder) || HostileTarget.StatusTime(true, StatusID.Thunder, StatusID.ThunderIi, StatusID.ThunderIii, StatusID.ThunderIv, StatusID.HighThunder_3872, StatusID.HighThunder) < 3))
        {
            if (ThunderPvE.CanUse(out act)) return true;
        }
        if (!NextGCDisInstant)
        {
            if (FoulPvE.CanUse(out act)) return true;
            if (XenoglossyPvE.CanUse(out act)) return true;
        }
        if (IsParadoxActive)
        {
            if (ParadoxPvE.CanUse(out act)) return true;
        }
        

        if (UmbralIceStacks == 3 && UmbralHearts == 3 && InUmbralIce)
        {
            if (TransposePvE.CanUse(out act)) return true;
        }

        if (InAstralFire && AstralFireStacks == 3 && !Player.HasStatus(true, StatusID.Firestarter) && CurrentMp < 800)
        {
            if (TransposePvE.CanUse(out act)) return true;
        }



        if (InAstralFire)
        {
            if (Player.HasStatus(true, StatusID.Firestarter))
            {
                if (FireIiiPvE.CanUse(out act)) return true;
            }
            if (CurrentMp >= 800)
            {
                if (DespairPvE.CanUse(out act)) return true;
            }
        }

        if (InUmbralIce)
        {
            if (FixedUS.CanUse(out act, skipCastingCheck: true)) return true;
        }

        if (!InUmbralIce && !InAstralFire)
        {
            if (NextGCDisInstant)
            {
                if (FireIiiPvE.CanUse(out act)) return true;
            }
        }

        if (BlizzardIiiPvE.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }
    public unsafe override void DisplayStatus()
    {
        //motif
        ImGui.Text("insta " + ThisManyInstantCasts);
        ImGui.Text("InAstralFire " + InAstralFire);
        ImGui.Text("iAstralFireStacks " + AstralFireStacks);
        ImGui.Text("elementtime " + ElementTime);

        base.DisplayStatus();
    }


}
