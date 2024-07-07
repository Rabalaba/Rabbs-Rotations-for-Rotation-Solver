using System.ComponentModel;

namespace RabbsRotationsNET8.Magical;

[Rotation("Rabbs SMN", CombatType.PvE, GameVersion = "6.58")]
[SourceCode(Path = "main/DefaultRotations/Magical/SMN_Default.cs")]
[Api(1)]
public sealed class SMN_Default : SummonerRotation
{
    #region Config Options

    [RotationConfig(CombatType.PvE, Name = "Use Crimson Cyclone. Will use at any range, regardless of saftey use with caution.")]
    public bool AddCrimsonCyclone { get; set; } = true;


    [Range(1, 6, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvE, Name = "Egi Order:\n  1 = Titan->Garuda->Ifrit.\n  2 = Titan->Ifrit->Garuda.\n  3 = Garuda->Titan->Ifrit.\n  4 = Garuda->Ifrit->Titan.\n  5 = Ifrit->Titan->Garuda.\n  6 = Ifrit->Garuda->Titan")]
    public int EgiOrder { get; set; } = 1;


    public IBaseAction SummonTitan => _SummonTitan.Value;

    private readonly Lazy<IBaseAction> _SummonTitan = new Lazy<IBaseAction>(delegate
    {
        IBaseAction actionx = new BaseAction(ActionID.SummonTopazPvE);
        ActionSetting settingx = actionx.Setting;
        ModifySummonTitan(ref settingx);
        actionx.Setting = settingx;
        return actionx;
    });

    public static void ModifySummonTitan(ref ActionSetting setting)
    {
        setting.StatusProvide = new StatusID[1] { StatusID.TitansFavor };
        setting.RotationCheck = () => IsTitanReady && SummonTime <= WeaponRemain;
    }

    public IBaseAction SummonGaruda => _SummonGaruda.Value;

    private readonly Lazy<IBaseAction> _SummonGaruda = new Lazy<IBaseAction>(delegate
    {
        IBaseAction actionx1 = new BaseAction(ActionID.SummonEmeraldPvE);
        ActionSetting settingx1 = actionx1.Setting;
        ModifySummonGaruda(ref settingx1);
        actionx1.Setting = settingx1;
        return actionx1;
    });

    public static void ModifySummonGaruda(ref ActionSetting setting)
    {
        setting.StatusProvide = new StatusID[1] { StatusID.TitansFavor };
        setting.RotationCheck = () => IsGarudaReady && SummonTime <= WeaponRemain;
    }

    public IBaseAction SummonIfrit => _SummonIfrit.Value;

    private readonly Lazy<IBaseAction> _SummonIfrit = new Lazy<IBaseAction>(delegate
    {
        IBaseAction actionx2 = new BaseAction(ActionID.SummonRubyPvE);
        ActionSetting settingx2 = actionx2.Setting;
        ModifySummonIfrit(ref settingx2);
        actionx2.Setting = settingx2;
        return actionx2;
    });

    public static void ModifySummonIfrit(ref ActionSetting setting)
    {
        setting.StatusProvide = new StatusID[1] { StatusID.TitansFavor };
        setting.RotationCheck = () => IsIfritReady && SummonTime <= WeaponRemain;
    }


    #endregion

    #region Countdown Logic
    protected override IAction? CountDownAction(float remainTime)
    {
        if (SummonCarbunclePvE.CanUse(out var act)) return act;

        if (remainTime <= RuinPvE.Info.CastTime + CountDownAhead
            && RuinPvE.CanUse(out act)) return act;
        return base.CountDownAction(remainTime);
    }
    #endregion

    #region Move Logic
    [RotationDesc(ActionID.CrimsonCyclonePvE)]
    protected override bool MoveForwardGCD(out IAction? act)
    {
        if (CrimsonCyclonePvE.CanUse(out act, skipAoeCheck: true)) return true;
        return base.MoveForwardGCD(out act);
    }
    #endregion

    #region oGCD Logic
    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        if (IsBurst && !Player.HasStatus(false, StatusID.SearingLight))
        {
            if (SearingLightPvE.CanUse(out act, skipAoeCheck: true)) return true;
        }

        var IsTargetBoss = HostileTarget?.IsBossFromTTK() ?? false;
        var IsTargetDying = HostileTarget?.IsDying() ?? false;

        if ((InBahamut && SummonBahamutPvE.Cooldown.ElapsedOneChargeAfterGCD(3) || InPhoenix ||
            IsTargetBoss && IsTargetDying) && EnkindleBahamutPvE.CanUse(out act, skipAoeCheck: true)) return true;

        if ((SummonBahamutPvE.Cooldown.ElapsedOneChargeAfterGCD(3) || IsTargetBoss && IsTargetDying) && DeathflarePvE.CanUse(out act, skipAoeCheck: true)) return true;
        if (RekindlePvE.CanUse(out act, skipAoeCheck: true)) return true;
        if (MountainBusterPvE.CanUse(out act, skipAoeCheck: true)) return true;

        if ((Player.HasStatus(false, StatusID.SearingLight) && InBahamut && (SummonBahamutPvE.Cooldown.ElapsedOneChargeAfterGCD(3) || !EnergyDrainPvE.Cooldown.IsCoolingDown) || EnergyDrainPvE.Cooldown.RecastTimeRemainOneCharge < 5 ||
            !SearingLightPvE.EnoughLevel || IsTargetBoss && IsTargetDying) && PainflarePvE.CanUse(out act)) return true;

        if ((InBahamut || InSolarBahamut && Player.HasStatus(false, StatusID.SearingLight) && (SummonBahamutPvE.Cooldown.ElapsedOneChargeAfterGCD(4) || !EnergyDrainPvE.Cooldown.IsCoolingDown) || !SearingLightPvE.EnoughLevel || IsTargetBoss && IsTargetDying) && FesterPvE.CanUse(out act) || NecrotizePvE.CanUse(out act)) return true;

        if (EnergySiphonPvE.CanUse(out act)) return true;
        if (EnergyDrainPvE.CanUse(out act)) return true;

        return base.AttackAbility(nextGCD, out act);
    }
    #endregion

    #region GCD Logic
    protected override bool GeneralGCD(out IAction? act)
    {
        if (SummonCarbunclePvE.CanUse(out act)) return true;

        if (SlipstreamPvE.CanUse(out act, skipAoeCheck: true)) return true;

        if (CrimsonStrikePvE.CanUse(out act, skipAoeCheck: true)) return true;

        //AOE
        if (PreciousBrilliancePvE.CanUse(out act)) return true;
        //Single
        if (GemshinePvE.CanUse(out act)) return true;

        if (!IsMoving && AddCrimsonCyclone && CrimsonCyclonePvE.CanUse(out act, skipAoeCheck: true)) return true;

        if ((Player.HasStatus(false, StatusID.SearingLight) || SearingLightPvE.Cooldown.IsCoolingDown) && SummonBahamutPvE.CanUse(out act)) return true;

        if (!SummonBahamutPvE.EnoughLevel && HasHostilesInRange && AetherchargePvE.CanUse(out act)) return true;

        if (IsMoving && (Player.HasStatus(true, StatusID.GarudasFavor) || InIfrit)
            && !Player.HasStatus(true, StatusID.Swiftcast) && !InBahamut && !InPhoenix
            && RuinIvPvE.CanUse(out act, skipAoeCheck: true)) return true;
   
        switch (EgiOrder)
        {
            case 1:
                if (SummonTitan.CanUse(out act)) return true;
                if (SummonGaruda.CanUse(out act)) return true;
                if (SummonIfrit.CanUse(out act)) return true;
                break;

            case 2:
                if (SummonTitan.CanUse(out act)) return true;
                if (SummonGaruda.CanUse(out act)) return true;
                if (SummonIfrit.CanUse(out act)) return true;
                break;

            case 3:
                if (SummonGaruda.CanUse(out act)) return true;
                if (SummonTitan.CanUse(out act)) return true;
                if (SummonIfrit.CanUse(out act)) return true;
                break;
            case 4:
                if (SummonGaruda.CanUse(out act)) return true;
                if (SummonIfrit.CanUse(out act)) return true;
                if (SummonTitan.CanUse(out act)) return true;
                break;

            case 5:
                if (SummonIfrit.CanUse(out act)) return true;
                if (SummonTitan.CanUse(out act)) return true;
                if (SummonGaruda.CanUse(out act)) return true;
                break;

            case 6:
                if (SummonIfrit.CanUse(out act)) return true;
                if (SummonGaruda.CanUse(out act)) return true;
                if (SummonTitan.CanUse(out act)) return true;
                break;
        }
        
        if (SummonTimeEndAfterGCD() && AttunmentTimeEndAfterGCD() &&
            !Player.HasStatus(true, StatusID.Swiftcast) && !InBahamut && !InPhoenix &&
            RuinIvPvE.CanUse(out act, skipAoeCheck: true)) return true;

        if (OutburstPvE.CanUse(out act)) return true;

        if (RuinPvE.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }
    #endregion

    #region Extra Methods
    public override bool CanHealSingleSpell => false;

    #endregion
}