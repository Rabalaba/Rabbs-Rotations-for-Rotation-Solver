
using Dalamud.Game.ClientState.JobGauge.Types;
using RotationSolver.Basic.Data;
using System;

namespace DefaultRotations.Melee;

[Rotation("Default", CombatType.PvE, GameVersion = "7.0")]
[SourceCode(Path = "main/DefaultRotations/Melee/VPR_Default.cs")]
[Api(2)]
public sealed class VPR_Default : ViperRotation
{
    private static bool HaveReawakend => Player.HasStatus(true, StatusID.Reawakened, StatusID.Reawakened_4094);
    private static bool HaveSwiftScaled => Player.HasStatus(true, StatusID.Swiftscaled, StatusID.Swiftscaled_4121);
    private static bool HaveHuntersInstinct => Player.HasStatus(true, StatusID.HuntersInstinct, StatusID.HuntersInstinct_4120);
    private static bool HaveHuntersVenom => Player.HasStatus(true, StatusID.HuntersVenom);
    private static bool HaveSwiftVenom => Player.HasStatus(true, StatusID.SwiftskinsVenom);
    private static bool HaveFellHuntersVenom => Player.HasStatus(true, StatusID.FellhuntersVenom);
    private static bool HaveFellskintVenom => Player.HasStatus(true, StatusID.FellskinsVenom);
    private static bool HavePoisedFang => Player.HasStatus(true, StatusID.PoisedForTwinfang);
    private static bool HavePoisedBlood => Player.HasStatus(true, StatusID.PoisedForTwinblood);
    private static bool HaveFlankingVenom => Player.HasStatus(true, StatusID.FlankstungVenom, StatusID.FlanksbaneVenom);
    private static bool HaveHindVenom => Player.HasStatus(true, StatusID.HindsbaneVenom, StatusID.HindstungVenom);
    private static bool HaveGrimHuntersVenom => Player.HasStatus(true, StatusID.GrimhuntersVenom);
    private static bool HaveGrimSkinVenom => Player.HasStatus(true, StatusID.GrimskinsVenom);
    private static int MyGeneration => EnhancedSerpentsLineageTrait.EnoughLevel ? 6-AnguineTribute:5-AnguineTribute;
    public static IBaseAction ThisCoil { get; } = new BaseAction((ActionID)34645);
    public static IBaseAction UnCoilCoil { get; } = new BaseAction((ActionID)34633);
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
        uint SerpentsTailId = AdjustId(SerpentsTailPvE.ID);
        uint ComboMark = AdjustId(SteelFangsPvE.ID);
        int ComboMark2 = ComboMark == SteelFangsPvE.ID ? 1 : ComboMark == HuntersStingPvE.ID ? 2 : 3;
        act = null;
        if (SerpentsIrePvE.CanUse(out act) && InCombat && RattlingCoilStacks <= 2 && !HaveReawakend) return true;
        if (TwinfangThreshPvE.CanUse(out act, skipComboCheck: true, skipCastingCheck: true, skipAoeCheck: true, skipStatusProvideCheck: true) && HaveFellHuntersVenom) return true;
        if (TwinbloodThreshPvE.CanUse(out act, skipComboCheck: true, skipCastingCheck: true, skipAoeCheck: true, skipStatusProvideCheck: true) && HaveFellskintVenom) return true;
        if (TwinfangBitePvE.CanUse(out act) && HaveHuntersVenom) return true;
        if (TwinbloodBitePvE.CanUse(out act) && HaveSwiftVenom) return true;
        if (UncoiledTwinfangPvE.CanUse(out act, skipComboCheck: true, skipCastingCheck: true, skipAoeCheck: true, skipStatusProvideCheck: true) && HavePoisedFang) return true;
        if (ThisCoil.CanUse(out act, skipComboCheck: true, skipCastingCheck: true, skipAoeCheck: true, skipStatusProvideCheck: true) && HavePoisedBlood) return true;
        if (LastLashPvE.CanUse(out act, skipComboCheck: true, skipCastingCheck: true, skipAoeCheck: true, skipStatusProvideCheck: true) && SerpentsTailId == LastLashPvE.ID) return true;
        if (DeathRattlePvE.CanUse(out act, skipComboCheck: true, skipCastingCheck: true, skipAoeCheck: true, skipStatusProvideCheck: true) && SerpentsTailId == DeathRattlePvE.ID) return true;
        if (FirstLegacyPvE.CanUse(out act, skipComboCheck: true, skipCastingCheck: true, skipAoeCheck: true, skipStatusProvideCheck: true) && SerpentsTailId == FirstLegacyPvE.ID) return true;
        if (SecondLegacyPvE.CanUse(out act, skipComboCheck: true, skipCastingCheck: true, skipAoeCheck: true, skipStatusProvideCheck: true) && SerpentsTailId == SecondLegacyPvE.ID) return true;
        if (ThirdLegacyPvE.CanUse(out act, skipComboCheck: true, skipCastingCheck: true, skipAoeCheck: true, skipStatusProvideCheck: true) && SerpentsTailId == ThirdLegacyPvE.ID) return true;
        if (FourthLegacyPvE.CanUse(out act, skipComboCheck: true, skipCastingCheck: true, skipAoeCheck: true, skipStatusProvideCheck: true) && SerpentsTailId == FourthLegacyPvE.ID) return true;
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
        uint ComboMark = AdjustId(SteelFangsPvE.ID);
        int ComboMark2 = ComboMark == SteelFangsPvE.ID ? 1 : ComboMark == HuntersStingPvE.ID ? 2 : 3;
        uint ComboMark3 = AdjustId(SteelMawPvE.ID);
        int ComboMark4 = ComboMark3 == SteelMawPvE.ID ? 1 : ComboMark == HuntersBitePvE.ID ? 2 : 3;
        act = null;
        if (MyGeneration is 1)
        {
            if (SteelFangsPvE.CanUse(out act)) return true;
        }
        if (MyGeneration is 2)
        {
            if (DreadFangsPvE.CanUse(out act)) return true;
        }
        if (MyGeneration is 3)
        {
            if (HuntersCoilPvE.CanUse(out act, skipComboCheck:true) && DreadCombo is (DreadCombo)9) return true;
        }
        if (MyGeneration is 4)
        {
            if (SwiftskinsCoilPvE.CanUse(out act, skipComboCheck: true) && DreadCombo is (DreadCombo)10) return true;
        }
        if (MyGeneration is 5)
        {
            if (OuroborosPvE.CanUse(out act, skipComboCheck:true, skipCastingCheck:true, skipAoeCheck:true, skipStatusProvideCheck:true)) return true;
        }


        //Overcap protection
        if ((DreadwinderPvE.Cooldown.CurrentCharges > 0 || !SerpentsIrePvE.Cooldown.IsCoolingDown) &&
            ((RattlingCoilStacks is 3 && EnhancedVipersRattleTrait.EnoughLevel) ||
            (RattlingCoilStacks is 2 && !EnhancedVipersRattleTrait.EnoughLevel)))
        {
            if (UnCoilCoil.CanUse(out act, skipComboCheck: true, skipCastingCheck: true, skipAoeCheck: true, skipStatusProvideCheck: true)) return true;
        }


        if (RattlingCoilStacks > 0  && DreadCombo is (DreadCombo)0 && !Player.HasStatus(true, StatusID.ReadyToReawaken) &&
            !HaveSwiftVenom && !HaveHuntersVenom &&
            HaveSwiftScaled && HaveHuntersInstinct)
        {
            if (UnCoilCoil.CanUse(out act, skipComboCheck: true, skipCastingCheck: true, skipAoeCheck: true, skipStatusProvideCheck: true)) return true;
        }



        if (HuntersDenPvE.CanUse(out act, skipComboCheck: true, skipCastingCheck: true, skipAoeCheck: true, skipStatusProvideCheck: true) && DreadCombo == DreadCombo.PitOfDread) return true;
        if (SwiftskinsDenPvE.CanUse(out act, skipComboCheck: true, skipCastingCheck: true, skipAoeCheck: true, skipStatusProvideCheck: true) && DreadCombo == DreadCombo.HuntersDen) return true;
        if (HuntersCoilPvE.CanUse(out act, skipComboCheck: true) && DreadCombo == DreadCombo.Dreadwinder) return true;
        if (SwiftskinsCoilPvE.CanUse(out act, skipComboCheck: true) && DreadCombo == DreadCombo.HuntersCoil) return true;

        if (ComboMark4 == 2)
        {
            if (SteelMawPvE.CanUse(out act) && HaveGrimHuntersVenom) return true;
            if (DreadMawPvE.CanUse(out act) && HaveGrimSkinVenom) return true;
        }

        if (ComboMark2 == 2)
        {

            if (SteelFangsPvE.CanUse(out act) && HaveFlankingVenom) return true;
            if (DreadFangsPvE.CanUse(out act) && HaveHindVenom) return true;
        }

        if (ComboMark4 == 3)
        {
            if (SteelMawPvE.CanUse(out act) && HaveGrimHuntersVenom) return true;
            if (DreadMawPvE.CanUse(out act) && HaveGrimSkinVenom) return true;
        }
        if (ComboMark2 == 3)
        {
            if ((Player.HasStatus(true, StatusID.FlankstungVenom) || Player.HasStatus(true, StatusID.HindstungVenom)))
            {
                if (SteelMawPvE.CanUse(out act)) return true;
                if (SteelFangsPvE.CanUse(out act)) return true;
            }

            if ((Player.HasStatus(true, StatusID.FlanksbaneVenom) || Player.HasStatus(true, StatusID.HindsbaneVenom)))
            {
                if (DreadMawPvE.CanUse(out act)) return true;
                if (DreadFangsPvE.CanUse(out act)) return true;
            }
        }
        if (ComboMark2 == 1)
        {

        //Reawakend Usage
        if ((SerpentOffering >= 50|| Player.HasStatus(true, StatusID.ReadyToReawaken)) &&
            HaveSwiftScaled &&
            HaveHuntersInstinct &&
            (HostileTarget?.HasStatus(true, StatusID.NoxiousGnash, StatusID.NoxiousGnash_4099) ?? false) &&
            !HaveHuntersVenom && !HaveSwiftVenom &&
            !HavePoisedBlood && !HavePoisedFang)

        {
            if (ReawakenPvE.CanUse(out act, skipComboCheck: true, skipCastingCheck: true, skipAoeCheck: true, skipStatusProvideCheck: true)) return true;
        }


            //Dreadwinder Usage
            if (PitOfDreadPvE.CanUse(out act, usedUp: true) && DreadCombo is (DreadCombo)0) return true;
            //Dreadwinder Usage
            if (DreadwinderPvE.CanUse(out act, usedUp: true) && DreadCombo is (DreadCombo)0) return true;
        }

        if (!HostileTarget?.HasStatus(true, StatusID.NoxiousGnash, StatusID.NoxiousGnash_4099) ?? false || (HostileTarget?.WillStatusEnd(20, true, StatusID.NoxiousGnash, StatusID.NoxiousGnash_4099) ?? false))
        {
            if (DreadMawPvE.CanUse(out act)) return true;
            if (DreadFangsPvE.CanUse(out act)) return true;
        }
        else
        {
            if (SteelMawPvE.CanUse(out act)) return true;
            if (SteelFangsPvE.CanUse(out act)) return true;
        }
        if (DreadMawPvE.CanUse(out act)) return true;
        if (DreadFangsPvE.CanUse(out act)) return true;
        if (WrithingSnapPvE.CanUse(out act)) return true;

            return base.GeneralGCD(out act);
    }

    private bool AttackGCD(out IAction? act, bool burst)
    {
        act = null;

        return false;
    }
    #endregion

    #region Extra Methods
    #endregion
}
