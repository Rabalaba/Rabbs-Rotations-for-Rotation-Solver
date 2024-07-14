using FFXIVClientStructs.FFXIV.Client.Game;
using System.ComponentModel.DataAnnotations;

namespace RabbsRotationsNET8.PVE.Melee;

[Rotation("Rabbs Samurai", CombatType.PvE, GameVersion = "6.58")]
[SourceCode(Path = "main/RabbsRotations/Melee/SAM.cs")]
[Api(2)]
public sealed class SAM_PVE : SamuraiRotation
{

    [RotationSolver.Basic.Attributes.Range(25, 85, ConfigUnitType.None, 5)]
    public static IBaseAction MeikoPrePull { get; } = new BaseAction((ActionID)7499);
    private static bool HaveMeikyoShisui => Player.HasStatus(true, StatusID.MeikyoShisui);
    private static bool HaveTrueNorth => Player.HasStatus(true, StatusID.TrueNorth);
    private int FillerPhasestep { get; set; }
    private bool FillerPhaseInProgress { get; set; }
    public static bool IsOddMinute()
    {
        // Get whole minutes from CombatTime (assuming it represents seconds)
        int minutes = (int)Math.Floor(CombatTime / 60f);

        // Use modulo to check if minutes is odd (remainder 1)
        return minutes % 2 != 0;
    }


    protected unsafe override IAction? CountDownAction(float remainTime)
    {
        //var IsThereABoss = AllHostileTargets.Any(p => p.IsBossFromTTK()) || AllHostileTargets.Any(p => p.IsBossFromIcon());

        if (remainTime < 0.2)
        {
            if (GekkoPvE.CanUse(out var act, skipComboCheck: true))
            {
                return act;
            }
        }


        if (remainTime < 5 && !HaveTrueNorth)
        {
            if (TrueNorthPvE.CanUse(out var act)) return act;
        }

        if (remainTime < 8.5)
        {
            if (MeikyoShisuiPvE.CanUse(out var act)) return act;
        }



        return base.CountDownAction(remainTime);
    }

    protected override bool GeneralGCD(out IAction? act)
    {
        var IsTargetBoss = HostileTarget?.IsBossFromTTK() ?? false;
        var IsTargetDying = HostileTarget?.IsDying() ?? false;
        //int numberLNhigabana = AllHostileTargets.Where(m => m.HasStatus(true, StatusID.Higanbana)).Count(); 
        int currentMinute = (int)Math.Floor(CombatTime / 60f);
        int lastCombatMinute = currentMinute; // Initialize with current minute

        if (KaeshiNamikiriPvE.CanUse(out act, skipAoeCheck: true, usedUp: true)) return true;
        if (KaeshiGokenPvE.CanUse(out act, usedUp: true)) return true;
        if (KaeshiSetsugekkaPvE.CanUse(out act, skipAoeCheck: true, usedUp: true)) return true;
        if ((!IsTargetBoss || (HostileTarget?.HasStatus(true, StatusID.Higanbana) ?? false)) && HasMoon && HasFlower && !(HostileTarget?.WillStatusEnd(16, true, StatusID.Higanbana) ?? false)
            && OgiNamikiriPvE.CanUse(out act, skipAoeCheck: true)) return true;
        if (Player.HasStatus(true, StatusID.MeikyoShisui))
        {

            // Check for Higabna if Sen is 1 and target doesn't have Higanbana debuff, skip for first couple gcd in opener to get raid buffs first, maybe build logic for raid burst correction ie "IsBurst" check
            //if (SenCount == 1 && CombatTime > 5 && !IsTargetDying && AllHostileTargets != null && NumberOfHostilesInRange < 3 && numberLNhigabana < 3 && (!AllHostileTargets.Any(p => p.HasStatus(true, StatusID.Higanbana)) || AllHostileTargets.Any(p => p.WillStatusEnd(10, true, StatusID.Higanbana))))
            //{
                //if (HiganbanaPvE.CanUse(out act)) return true; // Only return true if Higanbana is usable and debuff applies
            //}

            if (SenCount == 2)
            {
                if (TenkaGokenPvE.CanUse(out act)) return true;
            }
            // Check for Midare if Sen is 3
            if (SenCount == 3)
            {
                if (MidareSetsugekkaPvE.CanUse(out act)) return true; // Only return true if Midare is usable
            }



            // Remaining Skills (Gekko > Kasha > Yukikaze)
            if (!HasGetsu) // Gekko can be used if not under Getsu buff, Mangetsu if aoe
            {
                if (IsMoonTimeLessThanFlower || !HasMoon) // Use Gekko if Moon uptime is less than flower of no moon
                {
                    if (MangetsuPvE.CanUse(out act, skipComboCheck: true)) return true;
                    if (GekkoPvE.CanUse(out act, skipComboCheck: true)) return true;
                }
            }

            // Use Oka/Kasha if no Ka buff and usable
            if (!HasKa)
            {
                if (OkaPvE.CanUse(out act, skipComboCheck: true)) return true;
                if (KashaPvE.CanUse(out act, skipComboCheck: true)) return true;
            }


            // Default Yukikaze if others are not usable
            if (YukikazePvE.CanUse(out act, skipComboCheck: true)) return true;
        }



        if (SenCount == 1 && (Player.HasStatus(true, StatusID.OgiNamikiriReady) || HostileTarget != null && HostileTarget.WillStatusEnd(10, true, StatusID.Higanbana)))
        {
            if (HasMoon && HasFlower && HiganbanaPvE.CanUse(out act, skipStatusProvideCheck: true)) return true;
        }
        if (SenCount == 2)
        {
            if (TenkaGokenPvE.CanUse(out act, skipAoeCheck: !MidareSetsugekkaPvE.EnoughLevel)) return true;
        }
        if (SenCount == 3)
        {
            if (MidareSetsugekkaPvE.CanUse(out act)) return true;
        }
        if ((!HasMoon || IsMoonTimeLessThanFlower || !OkaPvE.EnoughLevel) && MangetsuPvE.CanUse(out act, skipAoeCheck: HaveMeikyoShisui && !HasGetsu)) return true;
        if ((!HasFlower || !IsMoonTimeLessThanFlower) && OkaPvE.CanUse(out act, skipAoeCheck: HaveMeikyoShisui && !HasKa)) return true;
        if (!HasSetsu && YukikazePvE.CanUse(out act, skipAoeCheck: HaveMeikyoShisui && HasGetsu && HasKa && !HasSetsu)) return true;
        if (GekkoPvE.CanUse(out act, skipComboCheck: HaveMeikyoShisui && !HasGetsu)) return true;
        if (KashaPvE.CanUse(out act, skipComboCheck: HaveMeikyoShisui && !HasKa)) return true;
        if ((!HasMoon || IsMoonTimeLessThanFlower || !ShifuPvE.EnoughLevel) && JinpuPvE.CanUse(out act)) return true;
        if ((!HasFlower || !IsMoonTimeLessThanFlower) && ShifuPvE.CanUse(out act)) return true;
        if (FukoPvE.CanUse(out act)) return true;
        if (!FukoPvE.EnoughLevel && FugaPvE.CanUse(out act)) return true;
        if (HakazePvE.CanUse(out act)) return true;

        if (EnpiPvE.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }

    protected unsafe override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
        var IsTargetBoss = HostileTarget?.IsBossFromTTK() ?? false;
        var IsTargetDying = HostileTarget?.IsDying() ?? false;

        if (Kenki <= 50 && HasFlower && HasMoon && IkishotenPvE.CanUse(out act)) return true;
        var aState = FFXIVClientStructs.FFXIV.Client.Game.UI.UIState.Instance()->PlayerState;
        var SkillSpeed = aState.Attributes[45];
        var isOddMinute = IsOddMinute();
        if (IsBurst && UseBurstMedicine(out act)) return true;
        var fillerSelect = SkillSpeed <= 648 ? isOddMinute ? 2 : 0 : 1;

        if (fillerSelect == 2 && (HostileTarget?.WillStatusEnd(50, true, StatusID.Higanbana) ?? false) && !(HostileTarget?.WillStatusEnd(16, true, StatusID.Higanbana) ?? false) && SenCount == 1 && IsLastAction(true, YukikazePvE) && !HaveMeikyoShisui && !RecordActions[0..30].Any(x => x.Action.RowId == HagakurePvE.ID))
        {
            if (HagakurePvE.CanUse(out act)) return true;
        }
        if (KaeshiNamikiriPvE.EnoughLevel && !HaveMeikyoShisui && InCombat)
        {
            if (IsLastAbility(true, KaeshiSetsugekkaPvE) || !HasMoon || !HasFlower || SenCount == 0 && !KaeshiSetsugekkaPvE.IsEnabled || MeikyoShisuiPvE.Cooldown.CurrentCharges == 2)
            {
                if (MeikyoShisuiPvE.CanUse(out act, usedUp: true)) return true;
            }
        }

        if (!KaeshiNamikiriPvE.EnoughLevel && !HaveMeikyoShisui && InCombat)
        {
            if (!HasMoon || !HasFlower || SenCount == 0)
            {
                if (MeikyoShisuiPvE.CanUse(out act, usedUp: true)) return true;
            }
        }

        if (HasMoon && HasFlower && (IsBurst || SenCount == 0))
        {
            if (HissatsuGurenPvE.CanUse(out act, skipAoeCheck: !HissatsuSeneiPvE.EnoughLevel)) return true;
            if (HissatsuSeneiPvE.CanUse(out act)) return true;
        }
        if (CombatTime > 30 || IkishotenPvE.IsInCooldown)
        {
            if (ShohaPvE.CanUse(out act)) return true;
        }

        if (Kenki >= 50 && IkishotenPvE.Cooldown.WillHaveOneCharge(10) || Kenki >= 50 || IsTargetBoss && IsTargetDying)
        {
            if (HissatsuKyutenPvE.CanUse(out act)) return true;
            if (HissatsuShintenPvE.CanUse(out act)) return true;
        }
        if (Kenki < 25 && CombatTime < 40 && SenCount == 2 && IkishotenPvE.IsInCooldown && TsubamegaeshiPvE.Cooldown.HasOneCharge)
        {
            if (HissatsuGurenPvE.CanUse(out act)) return true;
        }

        return base.AttackAbility(nextGCD, out act);
    }
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        return base.EmergencyAbility(nextGCD, out act);
    }


}