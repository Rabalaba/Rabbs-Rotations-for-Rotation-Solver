using FFXIVClientStructs.FFXIV.Client.UI;

namespace RabbsRotationsNET8.Magical;

[Rotation("JustAnotherMiqo", CombatType.PvE, GameVersion = "7.0")]
[SourceCode(Path = "main/DefaultRotations/Magical/PCT_Default.cs")]
[Api(1)]
public sealed class PCT_Default : PictomancerRotation
{



    public override MedicineType MedicineType => MedicineType.Intelligence;
    #region Countdown logic
    // Defines logic for actions to take during the countdown before combat starts.
    protected override IAction? CountDownAction(float remainTime)
    {
        return base.CountDownAction(remainTime);
    }
    #endregion

    #region Emergency Logic
    // Determines emergency actions to take based on the next planned GCD action.
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        act = null;

        return base.EmergencyAbility(nextGCD, out act);
    }
    #endregion

    #region oGCD Logic
    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {



        return base.AttackAbility(nextGCD, out act);
    }

    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
    {
        act = null;


        return base.MoveForwardAbility(nextGCD, out act);
    }
    #endregion

    #region GCD Logic
    protected override bool MoveForwardGCD(out IAction? act)
    {
        act = null;

        return base.MoveForwardGCD(out act);
    }

    protected override bool GeneralGCD(out IAction? act)
    {

        if (!InCombat)
        { 
        if (!CreatureMotifDrawn)
            {
                if (PomMotifPvE.CanUse(out act) && CreatureMotifPvE.AdjustedID == PomMotifPvE.ID) return true;
                if (WingMotifPvE.CanUse(out act) && CreatureMotifPvE.AdjustedID == WingMotifPvE.ID) return true;
            }
        if (!WeaponMotifDrawn) 
            { 
                
            }
        
        }

        if (HammerStampPvE.CanUse(out act, skipCastingCheck: true, skipAoeCheck: true) && Player.HasStatus(true, StatusID.HammerTime)) return true;

        if (Player.HasStatus(true, StatusID.SubtractivePalette))
        {
            //AOE
            if (ThunderIiInMagentaPvE.CanUse(out act, skipCastingCheck: true)) return true;
            if (StoneIiInYellowPvE.CanUse(out act, skipCastingCheck: true)) return true;
            if (BlizzardIiInCyanPvE.CanUse(out act, skipCastingCheck: true)) return true;

            //123
            if (ThunderInMagentaPvE.CanUse(out act, skipCastingCheck: true)) return true;
            if (StoneInYellowPvE.CanUse(out act, skipCastingCheck: true)) return true;
            if (BlizzardInCyanPvE.CanUse(out act, skipCastingCheck: true)) return true;

        }
        else
        {
            if (HolyInWhitePvE.CanUse(out act, skipCastingCheck: true, skipAoeCheck: true)) return true;
            //AOE
            if (WaterIiInBluePvE.CanUse(out act, skipCastingCheck: true)) return true;
            if (AeroIiInGreenPvE.CanUse(out act, skipCastingCheck: true)) return true;
            if (FireIiInRedPvE.CanUse(out act, skipCastingCheck: true)) return true;

            //123
            if (WaterInBluePvE.CanUse(out act, skipCastingCheck: true)) return true;
            if (AeroInGreenPvE.CanUse(out act, skipCastingCheck: true)) return true;
            if (FireInRedPvE.CanUse(out act, skipCastingCheck: true)) return true;
        }


        return base.GeneralGCD(out act);
    }

    private bool AttackGCD(out IAction? act, bool burst)
    {
        act = null;

        return false;
    }
    #endregion


}
