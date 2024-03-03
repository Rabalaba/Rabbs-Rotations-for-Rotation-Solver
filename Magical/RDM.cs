using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace RabbsRotations.Magical;

[RotationDesc(ActionID.Embolden)]
public sealed class RdmRotation : RDM_Base
{
    public override string GameVersion => "6.55";
    public override string RotationName => "Rabbs RDM";
    public override string Description => "PvP Rabbs BLM";
    public override CombatType Type => CombatType.PvP;


    static IBaseAction VerthunderStartUp { get; } = new BaseAction(ActionID.Verthunder);

    public static IBaseAction PvPCorpsACorps { get; } = new BaseAction(ActionID.PvP_Corpsacorps);

    public static IBaseAction PvPDisplacement { get; } = new BaseAction(ActionID.PvP_Displacement);

    public static IBaseAction PvPFrazzle { get; } = new BaseAction((ActionID)29698, ActionOption.Defense);

    public static IBaseAction PvPVerstone   { get; } = new BaseAction(ActionID.PvP_Verstone);

    public static IBaseAction PvPEnchanted_RiposeBlack { get; } = new BaseAction((ActionID)29692);

    public static IBaseAction PvPEnchanted_ZwerchhauBlack { get; } = new BaseAction((ActionID)29693);

    public static IBaseAction PvPEnchanted_RedoublementBlack { get; } = new BaseAction((ActionID)29694);

    public static IBaseAction PvPVerflare{ get; } = new BaseAction((ActionID)29688);

    public static IBaseAction PvPEnchanted_RiposeWhite { get; } = new BaseAction((ActionID)29689);

    public static IBaseAction PvPEnchanted_ZwerchhauWhite { get; } = new BaseAction((ActionID)29690);

    public static IBaseAction PvPEnchanted_RedoublementWhite { get; } = new BaseAction((ActionID)29691);

    public static IBaseAction PvPVerholy { get; } = new BaseAction(ActionID.PvP_Verholy);

    public static IBaseAction PvP_SouthernCross { get; } = new BaseAction(ActionID.PvP_SouthernCross)
    {
        ActionCheck = (BattleChara t, bool m) => RotationSolver.Basic.Rotations.CustomRotation.LimitBreakLevel >= 1
    };

    private static BaseAction PvP_Zantetsuken { get; } = new(ActionID.PvP_SouthernCross)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            Targets = Targets.Where(b => b.YalmDistanceX < 20 &&
            b.HasStatus(true, (StatusID)3241)).ToArray();

            if (Targets.Any())
            {
                return Targets.OrderBy(ObjectHelper.GetHealthRatio).First();
            }

            return null;
        },
        ActionCheck = (BattleChara b, bool m) => LimitBreakLevel >= 1
    };

    private static BaseAction PvP_MarksmansSpite { get; } = new(ActionID.PvP_MarksmansSpite)
    {

        ChoiceTarget = (Targets, mustUse) =>
        {
            Targets = Targets.Where(b => b.YalmDistanceX < 50 &&
            (b.CurrentHp + b.CurrentMp*6) <50000 &&
            b.HasStatus(false, (StatusID)3054)).ToArray();

            if (Targets.Any())
            {
                return Targets.OrderBy(ObjectHelper.GetHealthRatio).Last();
            }

            return null;
        },
        ActionCheck = (BattleChara b, bool m) => LimitBreakLevel >= 1
    };




    private static bool CanStartMeleeCombo
    {
        #region PvP

        #endregion

        #region PVE
        get
        {
            if (Player.HasStatus(true, StatusID.Manafication, StatusID.Embolden) ||
                BlackMana == 100 || WhiteMana == 100) return true;

            if (BlackMana == WhiteMana) return false;

            if (WhiteMana < BlackMana)
            {
                if (Player.HasStatus(true, StatusID.VerstoneReady)) return false;
            }
            else
            {
                if (Player.HasStatus(true, StatusID.VerfireReady)) return false;
            }

            if (Player.HasStatus(true, Vercure.StatusProvide)) return false;

            //Waiting for embolden.
            if (Embolden.EnoughLevel && Embolden.WillHaveOneChargeGCD(5)) return false;

            return true;
        }
        #endregion
    }

    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
        .SetBool(CombatType.PvE,"UseVercure", false, "Use Vercure for Dualcast when out of combat.")
        .SetBool(CombatType.PvE,"UseAcceleration", false, "Use Acceleration On Cooldown to avoid overstack");

    protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime < VerthunderStartUp.CastTime + CountDownAhead
            && VerthunderStartUp.CanUse(out var act, CanUseOption.EmptyOrSkipCombo)) return act;

        //Remove Swift
        StatusHelper.StatusOff(StatusID.DualCast);
        StatusHelper.StatusOff(StatusID.Acceleration);
        StatusHelper.StatusOff(StatusID.SwiftCast);

        return base.CountDownAction(remainTime);
    }

    protected override bool GeneralGCD(out IAction act)
    {
        #region pvp



        if (Player.Level > 90)
        {
            

            if (Player.HasStatus(true, (StatusID)3245))
            {
                if (Player.HasStatus(true, (StatusID)3233))
                {
                    
                }
                if (PvPEnchanted_RedoublementWhite.CanUse(out act)) return true;
                if (PvPEnchanted_ZwerchhauWhite.CanUse(out act)) return true;
                if (PvPEnchanted_RiposeWhite.CanUse(out act)) return true;


            }

            if (Player.HasStatus(true, (StatusID)3246))

            {

                if (Player.HasStatus(true, (StatusID)3233))
                {
                    
                }
                if (PvPEnchanted_RedoublementBlack.CanUse(out act)) return true;
                if (PvPEnchanted_ZwerchhauBlack.CanUse(out act)) return true;
                if (PvPEnchanted_RiposeBlack.CanUse(out act)) return true;

            }
            
            //
        }
        if (PvP_Zantetsuken.CanUse(out act)) return true;
        if (Player.HasStatus(true, StatusID.PvP_WhiteShift) && Player.HasStatus(true, StatusID.PvP_VermilionRadiance) && PvPVerholy.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
        if (Player.HasStatus(true, StatusID.PvP_BlackShift) && Player.HasStatus(true, StatusID.PvP_VermilionRadiance) && PvPVerflare.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
        if (Player.HasStatus(true, StatusID.PvP_BlackShift) ? PvPEnchanted_RedoublementBlack.CanUse(out act) : PvPEnchanted_RedoublementWhite.CanUse(out act)) return true;
        if (Player.HasStatus(true, StatusID.PvP_BlackShift) ? PvPEnchanted_ZwerchhauBlack.CanUse(out act) : PvPEnchanted_ZwerchhauWhite.CanUse(out act)) return true;
        if (Player.HasStatus(true, StatusID.PvP_BlackShift) ? PvPEnchanted_RiposeBlack.CanUse(out act) : PvPEnchanted_RiposeWhite.CanUse(out act)) return true;
        if (PvPVerstone.CanUse(out act)) return true;



        #endregion

       

        return base.GeneralGCD(out act);
    }

    protected override bool EmergencyGCD(out IAction act)
    {
        

        return base.EmergencyGCD(out act);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        act = null;


        
        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(out IAction act)
    {

        #region PvP
        var id = AdjustId(ActionID.PvP_Enchantedriposte);
        if (Player.HasStatus(false, StatusID.PvP_BlackShift) && PvPFrazzle.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        if (PvPCorpsACorps.CanUse(out act, CanUseOption.EmptyOrSkipCombo) && !PvPCorpsACorps.Target.HasStatus(true,(StatusID)3242) && !PvPEnchanted_RiposeWhite.IsCoolingDown) return true;
        if (PvPDisplacement.CanUse(out act, CanUseOption.EmptyOrSkipCombo) && Player.WillStatusEnd(0.5f,true,(StatusID)3243) && (id == ActionID.PvP_Verholy || id == ActionID.PvP_Verflare) )return true;

        #endregion

        

        return base.AttackAbility(out act);

    }
}