namespace RabbsRotationsNET8.Healer;

[Rotation("Rabbs Ast", CombatType.PvE, GameVersion = "6.58")]
[SourceCode(Path = "main/DefaultRotations/Healer/AST_Default.cs")]
[Api(1)]
public sealed class AST_Default : AstrologianRotation
{
    #region Config Options
    [RotationConfig(CombatType.PvE, Name = "Use spells with cast times to heal. (Ignored if you are the only healer in party)")]
    public bool GCDHeal { get; set; } = false;

    [Range(4, 20, ConfigUnitType.Seconds)]
    [RotationConfig(CombatType.PvE, Name = "Use Earthly Star during countdown timer.")]
    public float UseEarthlyStarTime { get; set; } = 15;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvE, Name = "Minimum HP threshold party member needs to be to use Aspected Benefic")]
    public float AspectedBeneficHeal { get; set; } = 0.4f;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvE, Name = "Minimum HP threshold among party member needed to use Horoscope")]
    public float HoroscopeHeal { get; set; } = 0.3f;
    #endregion


    public static IBaseAction MinorArcanaPvE2 { get; } = new BaseAction((ActionID)37022);

    #region Countdown Logic
    protected override IAction? CountDownAction(float remainTime)
    {
        if (remainTime < MaleficPvE.Info.CastTime + CountDownAhead
            && MaleficPvE.CanUse(out var act)) return act;
        if (remainTime < 3 && UseBurstMedicine(out act)) return act;
        if (remainTime is < 4 and > 3 && AspectedBeneficPvE.CanUse(out act)) return act;
        if (remainTime < UseEarthlyStarTime
            && EarthlyStarPvE.CanUse(out act)) return act;
        if (remainTime < 30 && AstralDrawPvE.CanUse(out act)) return act;

        return base.CountDownAction(remainTime);
    }
    #endregion

    #region Defensive Logic
    [RotationDesc(ActionID.CelestialIntersectionPvE, ActionID.ExaltationPvE)]
    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? act)
    {
        if (CelestialIntersectionPvE.CanUse(out act, usedUp: true)) return true;
        if (ExaltationPvE.CanUse(out act)) return true;
        return base.DefenseSingleAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.MacrocosmosPvE)]
    protected override bool DefenseAreaGCD(out IAction? act)
    {
        act = null;
        if (MacrocosmosPvE.Cooldown.IsCoolingDown && !MacrocosmosPvE.Cooldown.WillHaveOneCharge(150)
            || CollectiveUnconsciousPvE.Cooldown.IsCoolingDown && !CollectiveUnconsciousPvE.Cooldown.WillHaveOneCharge(40)) return false;

        if (MacrocosmosPvE.CanUse(out act)) return true;
        return base.DefenseAreaGCD(out act);
    }

    [RotationDesc(ActionID.CollectiveUnconsciousPvE)]
    protected override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
        if (MacrocosmosPvE.Cooldown.IsCoolingDown && !MacrocosmosPvE.Cooldown.WillHaveOneCharge(150)
            || CollectiveUnconsciousPvE.Cooldown.IsCoolingDown && !CollectiveUnconsciousPvE.Cooldown.WillHaveOneCharge(40)) return false;

        if (CollectiveUnconsciousPvE.CanUse(out act)) return true;
        return base.DefenseAreaAbility(nextGCD, out act);
    }
    #endregion

    #region GCD Logic
    protected override bool GeneralGCD(out IAction? act)
    {
        if (GravityPvE.CanUse(out act)) return true;

        if (CombustPvE.CanUse(out act)) return true;
        if (MaleficPvE.CanUse(out act)) return true;
        if (CombustPvE.CanUse(out act, skipStatusProvideCheck: true)) return true;

        return base.GeneralGCD(out act);
    }

    [RotationDesc(ActionID.AspectedBeneficPvE, ActionID.BeneficIiPvE, ActionID.BeneficPvE)]
    protected override bool HealSingleGCD(out IAction? act)
    {
        if (AspectedBeneficPvE.CanUse(out act)
            && (IsMoving
            || AspectedBeneficPvE.Target.Target?.GetHealthRatio() > AspectedBeneficHeal)) return true;

        if (BeneficIiPvE.CanUse(out act)) return true;
        if (BeneficPvE.CanUse(out act)) return true;

        return base.HealSingleGCD(out act);
    }

    [RotationDesc(ActionID.AspectedHeliosPvE, ActionID.HeliosPvE)]
    protected override bool HealAreaGCD(out IAction? act)
    {
        if (AspectedHeliosPvE.CanUse(out act)) return true;
        if (HeliosPvE.CanUse(out act)) return true;
        return base.HealAreaGCD(out act);
    }
    #endregion

    #region oGCD Logic
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (base.EmergencyAbility(nextGCD, out act)) return true;

        if (!InCombat) return false;

        if (nextGCD.IsTheSameTo(true, AspectedHeliosPvE, HeliosPvE))
        {
            if (HoroscopePvE.CanUse(out act)) return true;
            if (NeutralSectPvE.CanUse(out act)) return true;
        }

        if (nextGCD.IsTheSameTo(true, BeneficPvE, BeneficIiPvE, AspectedBeneficPvE))
        {
            if (SynastryPvE.CanUse(out act)) return true;
        }
        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool GeneralAbility(IAction nextGCD, out IAction? act)
    {
        if (AstralDrawPvE.CanUse(out act)) return true;
        return base.GeneralAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        if (IsBurst && !IsMoving
            && DivinationPvE.CanUse(out act)) return true;

        if (MinorArcanaPvE2.CanUse(out act, usedUp: true)) return true;

        if (AstralDrawPvE.CanUse(out act, usedUp: IsBurst)) return true;

        if (InCombat)
        {
            if (IsMoving && LightspeedPvE.CanUse(out act)) return true;

            if (!IsMoving)
            {
                if (!Player.HasStatus(true, StatusID.EarthlyDominance, StatusID.GiantDominance))
                {
                    if (EarthlyStarPvE.CanUse(out act)) return true;
                }
            }

            if (DrawnCrownCard == CardType.LORD || MinorArcanaPvE2.Cooldown.WillHaveOneChargeGCD(1, 0))
            {
                if (MinorArcanaPvE2.CanUse(out act)) return true;
            }
        }


        //if (InCombat && PlayCard(out act)) return true;

        return base.AttackAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.EssentialDignityPvE, ActionID.CelestialIntersectionPvE, ActionID.CelestialOppositionPvE,
        ActionID.EarthlyStarPvE, ActionID.HoroscopePvE)]
    protected override bool HealSingleAbility(IAction nextGCD, out IAction? act)
    {
        if (EssentialDignityPvE.CanUse(out act)) return true;
        if (CelestialIntersectionPvE.CanUse(out act, usedUp: true)) return true;

        if (DrawnCrownCard == CardType.LADY
            && MinorArcanaPvE2.CanUse(out act)) return true;

        if (CelestialOppositionPvE.CanUse(out act)) return true;

        if (Player.HasStatus(true, StatusID.GiantDominance))
        {
            act = EarthlyStarPvE;
            return true;
        }

        if (!Player.HasStatus(true, StatusID.HoroscopeHelios, StatusID.Horoscope) && HoroscopePvE.CanUse(out act)) return true;

        if ((Player.HasStatus(true, StatusID.HoroscopeHelios)
            || PartyMembersMinHP < HoroscopeHeal)
            && HoroscopePvE.CanUse(out act)) return true;

        return base.HealSingleAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.CelestialOppositionPvE, ActionID.EarthlyStarPvE, ActionID.HoroscopePvE)]
    protected override bool HealAreaAbility(IAction nextGCD, out IAction? act)
    {
        if (CelestialOppositionPvE.CanUse(out act)) return true;

        if (Player.HasStatus(true, StatusID.GiantDominance))
        {
            act = EarthlyStarPvE;
            return true;
        }

        if (Player.HasStatus(true, StatusID.HoroscopeHelios) && HoroscopePvE.CanUse(out act)) return true;

        if (DrawnCrownCard == CardType.LADY && MinorArcanaPvE2.CanUse(out act)) return true;

        return base.HealAreaAbility(nextGCD, out act);
    }
    #endregion

    #region Extra Methods
    public override bool CanHealSingleSpell => base.CanHealSingleSpell && (GCDHeal || PartyMembers.GetJobCategory(JobRole.Healer).Count() < 2);
    public override bool CanHealAreaSpell => base.CanHealAreaSpell && (GCDHeal || PartyMembers.GetJobCategory(JobRole.Healer).Count() < 2);

    #endregion
}