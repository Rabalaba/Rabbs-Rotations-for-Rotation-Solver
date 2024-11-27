
namespace RabbsRotationsNET8.Magical;

[Rotation("Infinite Power-dox", CombatType.PvE, GameVersion = "7.11")]
[SourceCode(Path = "main/DefaultRotations/Magical/BLM_Default.cs")]
[Api(4)]
public class BLM_Default : BlackMageRotation
{

    public IBaseAction FixedUS { get; } = new BaseAction((ActionID)16506);

    public IBaseAction FixedMF { get; } = new BaseAction((ActionID)158);

    public IBaseAction FixedB4 { get; } = new BaseAction((ActionID)3576);

    public IBaseAction FixedLL { get; } = new BaseAction((ActionID)3573);

    public bool NextGCDisInstant => Player.HasStatus(true, StatusID.Triplecast, StatusID.Swiftcast);

    public bool CanMakeInstant => TriplecastPvE.Cooldown.CurrentCharges > 0 || !SwiftcastPvE.Cooldown.IsCoolingDown;

    protected override IAction? CountDownAction(float remainTime)
    {
        IAction act;
        if (remainTime < 10)
        {
            if (TriplecastPvE.CanUse(out act, usedUp: true) && !NextGCDisInstant) return act;
        }
        if (remainTime < BlizzardIiiPvE.Info.CastTime + CountDownAhead)
        {
            if (BlizzardIiiPvE.CanUse(out act)) return act;
        }
        return base.CountDownAction(remainTime);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {

        if (InCombat)
        {

            if (!IsPolyglotStacksMaxed)
            {
                if (AmplifierPvE.CanUse(out act)) return true;
            }


            if (!NextGCDisInstant)
            {
                if (InAstralFire && !HasFire && CurrentMp == 0 && PolyglotStacks == 0)
                {
                    if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                    if (SwiftcastPvE.CanUse(out act)) return true;
                }
                if (InUmbralIce)
                {
                    if (UmbralIceStacks < 3)
                    {
                        if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                        if (SwiftcastPvE.CanUse(out act)) return true;
                    }
                    if (UmbralHearts < 3)
                    {
                        if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                        if (SwiftcastPvE.CanUse(out act)) return true;
                    }
                }
                if (!InUmbralIce && !InAstralFire)
                {
                    if (CanMakeInstant)
                    {
                        if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                        if (SwiftcastPvE.CanUse(out act)) return true;
                    }
                }
            }
        }


        return base.AttackAbility(nextGCD, out act);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (!NextGCDisInstant)
        {
            if (InAstralFire && !HasFire && CurrentMp == 0 && PolyglotStacks == 0)
            {
                if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                if (SwiftcastPvE.CanUse(out act)) return true;
            }
            if (InUmbralIce)
            {
                if (UmbralIceStacks < 3)
                {
                    if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                    if (SwiftcastPvE.CanUse(out act)) return true;
                }
                if (UmbralHearts < 3)
                {
                    if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                    if (SwiftcastPvE.CanUse(out act)) return true;
                }
            }
            if (!InUmbralIce && !InAstralFire)
            {
                if (CanMakeInstant)
                {
                    if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                    if (SwiftcastPvE.CanUse(out act)) return true;
                }
            }
        }

        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool GeneralGCD(out IAction? act)
    {

        if (!NextGCDisInstant && InCombat)
        {
            if (InAstralFire && !HasFire && CurrentMp == 0 && PolyglotStacks == 0)
            {
                if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                if (SwiftcastPvE.CanUse(out act)) return true;
            }
            if (InUmbralIce)
            {
                if (UmbralIceStacks < 3)
                {
                    if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                    if (SwiftcastPvE.CanUse(out act)) return true;
                }
                if (UmbralHearts < 3)
                {
                    if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                    if (SwiftcastPvE.CanUse(out act)) return true;
                }
            }
            if (!InUmbralIce && !InAstralFire)
            {
                if (CanMakeInstant)
                {
                    if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                    if (SwiftcastPvE.CanUse(out act)) return true;
                }
            }
        }
        if (NextGCDisInstant)
        {

            if (InUmbralIce)
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
        if (HostileTarget != null && (!HostileTarget.HasStatus(true, StatusID.Thunder, StatusID.ThunderIi, StatusID.ThunderIii, StatusID.ThunderIv, StatusID.HighThunder_3872, StatusID.HighThunder) || HostileTarget.WillStatusEnd(3, true, StatusID.Thunder, StatusID.ThunderIi, StatusID.ThunderIii, StatusID.ThunderIv, StatusID.HighThunder_3872, StatusID.HighThunder)))
        {
            if (ThunderPvE.CanUse(out act)) return true;
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

        if (ScathePvE.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }


}
