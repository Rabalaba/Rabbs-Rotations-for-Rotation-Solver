
namespace RabbsRotationsNET8.Magical;

[Rotation("Rabbs Infinite Power-dox", CombatType.PvE, GameVersion = "6.58")]
[SourceCode(Path = "main/DefaultRotations/Magical/BLM_Default.cs")]
[Api(4)]
public class BLM_Default : BlackMageRotation
{

    public  IBaseAction FixedUS { get; } = new BaseAction((ActionID)16506);

    public  IBaseAction FixedMF { get; } = new BaseAction((ActionID)158);

    [RotationConfig(CombatType.PvE, Name = "Use Transpose to Astral Fire before Paradox")]
    public bool UseTransposeForParadox { get; set; } = true;

    [RotationConfig(CombatType.PvE, Name = "Extend Astral Fire Time Safely")]
    public bool ExtendTimeSafely { get; set; } = false;

    [RotationConfig(CombatType.PvE, Name = @"Use ""Double Paradox"" rotation [N15]")]
    public bool UseN15 { get; set; } = false;

    public bool NextGCDisInstant => Player.HasStatus(true, StatusID.Triplecast, StatusID.Swiftcast);

    protected override IAction? CountDownAction(float remainTime)
    {
        IAction act;
        if (remainTime < 1)
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
            if (AstralSoulStacks == 6 && CurrentMp > 0)
            {
                if (DespairPvE.CanUse(out act)) return true;
            }
            if (TriplecastPvE.Cooldown.CurrentCharges == 2)
            {
                if (AstralSoulStacks == 6)
                {
                    if (TriplecastPvE.CanUse(out act)) return true;
                }

                if (InUmbralIce && UmbralHearts == 0)
                {
                    if (TriplecastPvE.CanUse(out act)) return true;
                }
            }
            if (TriplecastPvE.Cooldown.CurrentCharges > 0 && InAstralFire)
            {
                if (TriplecastPvE.CanUse(out act)) return true;
            }
            if (AstralSoulStacks == 6 && InAstralFire && !NextGCDisInstant)
            {
                if (SwiftcastPvE.CanUse(out act)) return true;
            }

            if (IsPolyglotStacksMaxed)
            {
                if (FoulPvE.CanUse(out act)) return true;
                if (XenoglossyPvE.CanUse(out act)) return true;
            }
            if (UmbralIceStacks == 3 && UmbralHearts == 3 && InUmbralIce)
            {
                if (TransposePvE.CanUse(out act)) return true;
            }


            if (InAstralFire && AstralFireStacks == 3 && !Player.HasStatus(true, StatusID.Firestarter))
            {
                if (TransposePvE.CanUse(out act)) return true;
            }

            //manafont can only be used in astral fire, it gives 3 umbral hearts. If we are at 2 or more our next umbral soul will kick off paradox
            if (InAstralFire && UmbralHearts < 2 && !NextGCDisInstant && AstralSoulStacks == 0)
            {
                if (FixedMF.CanUse(out act)) return true;
            }

            if (UmbralHearts < 2 && !NextGCDisInstant && AstralSoulStacks == 0)
            {
                if (SwiftcastPvE.CanUse(out act)) return true;
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
        
        if (IsParadoxActive)
        {
            if (ParadoxPvE.CanUse(out act)) return true;    
        }
        if (NextGCDisInstant)
        {
            if (InUmbralIce && UmbralHearts == 0)
            {
                if (BlizzardIvPvE.CanUse(out act)) return true;
            }
            if (AstralSoulStacks == 6)
            {
                if (FlareStarPvE.CanUse(out act)) return true;
            }
            if (InAstralFire && AstralSoulStacks < 6)
            {
                if (FlarePvE.CanUse(out act, skipAoeCheck:true)) return true;
            }

        }

        if (!InAstralFire && !InUmbralIce)
        {
            if (BlizzardIiiPvE.CanUse(out act)) return true;
        }

        if (HostileTarget != null && (!HostileTarget.HasStatus(true, StatusID.Thunder, StatusID.ThunderIi, StatusID.ThunderIii, StatusID.ThunderIv, StatusID.HighThunder_3872, StatusID.HighThunder) || HostileTarget.WillStatusEnd(3, true, StatusID.Thunder, StatusID.ThunderIi, StatusID.ThunderIii, StatusID.ThunderIv, StatusID.HighThunder_3872, StatusID.HighThunder)))
        {
            if (ThunderIiPvE.CanUse(out act)) return true;
            if (ThunderPvE.CanUse(out act)) return true;
        }

        if (InUmbralIce && (UmbralIceStacks < 3 || UmbralHearts < 3) && NextGCDisInstant)
        {
            if (BlizzardIvPvE.CanUse(out act)) return true;
        }

        if (InUmbralIce && (UmbralIceStacks <3 || UmbralHearts < 3) && !NextGCDisInstant)
        {
            if (FixedUS.CanUse(out act)) return true;
        }

        if (InAstralFire && Player.HasStatus(true, StatusID.Firestarter))
        {
            if (FireIiiPvE.CanUse(out act)) return true;
        }

        return base.GeneralGCD(out act);
    }

   

    [RotationDesc(ActionID.BetweenTheLinesPvE, ActionID.LeyLinesPvE)]
    protected override bool HealSingleAbility(IAction nextGCD, out IAction? act)
    {
        if (BetweenTheLinesPvE.CanUse(out act)) return true;
        if (LeyLinesPvE.CanUse(out act)) return true;

        return base.HealSingleAbility(nextGCD, out act);
    }
}
