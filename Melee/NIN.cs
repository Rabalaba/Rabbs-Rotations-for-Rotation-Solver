using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using Serilog;
using Serilog.Events;
using FFXIVClientStructs.FFXIV.Client.Game.Gauge;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Rotations;
using Svg;
using System;
using System.Formats.Tar;
using System.Reflection.Metadata.Ecma335;
using Dalamud.Logging;
using ImGuiNET;
using RotationSolver.Basic.Configuration;
using Dalamud.Game.ClientState.Objects.SubKinds;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Data.Parsing.Layer;
using static Dalamud.Interface.Utility.Raii.ImRaii;

namespace RabbsRotations.Melee;

[RotationDesc(ActionID.Mug)]
public sealed class NinRotation : NIN_Base
{
    public override string GameVersion => "6.55";
    public override string RotationName => "Rabbs NIN";   
    public override string Description => "PVP and PVE Ninhja w/ both PVE openers(Check config)"; 
    public override CombatType Type => CombatType.Both;



    private static INinAction _ninActionAim;
    private static bool InTrickAttack => TrickAttack.IsCoolingDown && !TrickAttack.ElapsedAfter(17);
    private static bool InMug => Mug.IsCoolingDown && !Mug.ElapsedAfter(19);
    private static bool NoNinjutsu => AdjustId(ActionID.Ninjutsu) is ActionID.Ninjutsu or ActionID.RabbitMedium;
    private static BaseAction FakeTrick { get; } = new(ActionID.SpinningEdge);
    private float TrickCool => TrickAttack.IsCoolingDown ? NextAbilityToNextGCD - FakeTrick.RecastTimeElapsed : 0;
    private static BaseAction ArmorCrush2 { get; } = new BaseAction(ActionID.ArmorCrush)
    {
        //EnemyPositional = (EnemyPositional a, bool n) => EnemyPositional.Flank
        ActionCheck = (BattleChara b, bool m) => HutonEndAfter(25f) && !HutonEndAfterGCD() && !InTrickAttack && !InMug && ArmorCrush2.EnemyPositional is EnemyPositional.Flank
    };

    public static IBaseAction PvP_SpinningEdge { get; } = new BaseAction(ActionID.PvP_Spinningedge);
    public static IBaseAction PvP_GustSlash { get; } = new BaseAction(ActionID.PvP_Gustslash);
    public static IBaseAction PvP_AeolianEdge { get; } = new BaseAction(ActionID.PvP_Aeolianedge);
    public static IBaseAction PvP_Shukuchi2 { get; } = new BaseAction(ActionID.PvP_Shukuchi);
    public static IBaseAction PvP_Bunshin2 { get; } = new BaseAction(ActionID.PvP_Bunshin);
    public static IBaseAction PvP_ForkedRaiju2 { get; } = new BaseAction(ActionID.PvP_Forkedraiju);





    #region Opener Related Properties
    private int Openerstep { get; set; }
    private bool OpenerHasFinished { get; set; }
    private bool OpenerHasFailed { get; set; }
    private bool OpenerActionsAvailable { get; set; }
    private bool OpenerInProgress { get; set; }
    private bool Flag { get; set; }
    #endregion

    #region Action Related Properties
    private bool WillhaveTrick{ get; set; }
    private bool InBurst { get; set; }
    public static object TargetAreaMove { get; private set; }

    #endregion

    #region Rotation Config
    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
        .SetBool(CombatType.PvE, "UseHide", true, "Use Hide")
        .SetBool(CombatType.PvE, "AutoUnhide", true, "Use Unhide")
        .SetCombo(CombatType.PvE, "RotationSelection", 1, "Select which Rotation to use.", "3rd GCD Trick, Standard", "4th GCD Trick - Standard");
    #endregion

    protected override IAction CountDownAction(float remainTime)
    {
        ActionID Adjsuted_ninjutsu = AdjustId(ActionID.Ninjutsu);
        if (remainTime < 0.7f)
        {
            if (Player.Level == 90)
            {
                Flag = false;
                OpenerHasFailed = false;
                Openerstep = 0;
                OpenerInProgress = true;
            }
            
            

            if (Adjsuted_ninjutsu == ActionID.Suiton && Suiton.CanUse(out var act, CanUseOption.EmptyOrSkipCombo)) return act;
        }

        if (remainTime < 6f)
        {
            if (Adjsuted_ninjutsu == ActionID.RabbitMedium)
            {
                if (RabbitMedium.CanUse(out var act)) return act;
            }
            if (Adjsuted_ninjutsu == ActionID.Raiton && WeaponRemain == WeaponElapsed)
            {
                if (Jin.CanUse(out var act, CanUseOption.MustUseEmpty)) return act;
            }

            if (Adjsuted_ninjutsu == ActionID.FumaShuriken && WeaponRemain == WeaponElapsed)
            {
                if (Chi.CanUse(out var act, CanUseOption.MustUseEmpty)) return act;
            }

            if (Adjsuted_ninjutsu == ActionID.Ninjutsu && WeaponRemain == WeaponElapsed)
            {
                if (Ten.CanUse(out var act, CanUseOption.MustUseEmpty)) return act;
            }


        }

        if (remainTime < 9 && !Player.HasStatus(true, StatusID.Ninjutsu))
        {
            if (Hide.CanUse(out var act)) return act;
        }

        if (remainTime < 11 && HutonTime == 0)
        {
            if (1 >= 1)
            {
                if (Adjsuted_ninjutsu == ActionID.Huton && Huton.CanUse(out var act, CanUseOption.EmptyOrSkipCombo)) return act;
            }
            if (Adjsuted_ninjutsu == ActionID.RabbitMedium)
            {
                if (RabbitMedium.CanUse(out var act)) return act;
            }


            if (Adjsuted_ninjutsu == ActionID.Raiton && WeaponRemain == WeaponElapsed)
            {
                if (Ten.CanUse(out var act, CanUseOption.MustUseEmpty)) return act;
            }

            if (Adjsuted_ninjutsu == ActionID.FumaShuriken && WeaponRemain == WeaponElapsed)
            {
                if (Chi.CanUse(out var act, CanUseOption.MustUseEmpty)) return act;
            }

            if (Adjsuted_ninjutsu == ActionID.Ninjutsu && WeaponRemain == WeaponElapsed)
            {
                if (Jin.CanUse(out var act, CanUseOption.MustUseEmpty)) return act;
            }


        }


        return base.CountDownAction(remainTime);
    }


    #region Ninjutsu
    private static void SetNinjutsu(INinAction act)
    {
        if (act == null || AdjustId(ActionID.Ninjutsu) == ActionID.RabbitMedium) return;
        if (_ninActionAim != null && IsLastAction(false, Ten, Jin, Chi, FumaShurikenTen, FumaShurikenJin)) return;
        if (_ninActionAim != act)
        {
            _ninActionAim = act;
        }
    }

    private static void ClearNinjutsu()
    {
        if (_ninActionAim != null)
        {
            _ninActionAim = null;
        }
    }

    private static bool ChoiceNinjutsu(out IAction act)
    {
        act = null;
        if (AdjustId(ActionID.Ninjutsu) != ActionID.Ninjutsu) return false;
        if (TimeSinceLastAction.TotalSeconds > 4.5) ClearNinjutsu();
        if (_ninActionAim != null && WeaponRemain < 0.2) return false;

        //Kassatsu
        if (Player.HasStatus(true, StatusID.Kassatsu))
        {
            if (GokaMekkyaku.CanUse(out _))
            {
                SetNinjutsu(GokaMekkyaku);
                return false;
            }
            if (HyoshoRanryu.CanUse(out _))
            {
                SetNinjutsu(HyoshoRanryu);
                return false;
            }

            if (Katon.CanUse(out _))
            {
                SetNinjutsu(Katon);
                return false;
            }

            if (Raiton.CanUse(out _))
            {
                SetNinjutsu(Raiton);
                return false;
            }
        }
        else
        {
            //Buff
            if (Huraijin.CanUse(out act)) return true;
            if (!HutonEndAfterGCD() && _ninActionAim?.ID == Huton.ID)
            {
                ClearNinjutsu();
                return false;
            }
            if (Ten.CanUse(out _, CanUseOption.EmptyOrSkipCombo)
               && (!InCombat || !Huraijin.EnoughLevel) && Huton.CanUse(out _)
               && !IsLastAction(false, Huton))
            {
                SetNinjutsu(Huton);
                return false;
            }

            //Aoe
            if (Katon.CanUse(out _))
            {
                if (!Player.HasStatus(true, StatusID.Doton) && !IsMoving && !TenChiJin.WillHaveOneCharge(10))
                    SetNinjutsu(Doton);
                else SetNinjutsu(Katon);
                return false;
            }

            //Vulnerable
            if (IsBurst && TrickAttack.WillHaveOneCharge(21) && Suiton.CanUse(out _))
            {
                SetNinjutsu(Suiton);
                return false;
            }

            //Single
            if (Ten.CanUse(out _, InTrickAttack && !Player.HasStatus(false, StatusID.RaijuReady) ? CanUseOption.EmptyOrSkipCombo : CanUseOption.None))
            {
                if (Raiton.CanUse(out _))
                {
                    SetNinjutsu(Raiton);
                    return false;
                }

                if (!Chi.EnoughLevel && FumaShuriken.CanUse(out _))
                {
                    SetNinjutsu(FumaShuriken);
                    return false;
                }
            }
        }

        if (IsLastAction(false, DotonChi, SuitonJin,
            RabbitMedium, FumaShuriken, Katon, Raiton,
            Hyoton, Huton, Doton, Suiton, GokaMekkyaku, HyoshoRanryu))
        {
            ClearNinjutsu();
        }
        return false;
    }

    private static bool DoNinjutsu(out IAction act)
    {
        act = null;

        //TenChiJin
        if (Player.HasStatus(true, StatusID.TenChiJin))
        {
            uint tenId = AdjustId(Ten.ID);
            uint chiId = AdjustId(Chi.ID);
            uint jinId = AdjustId(Jin.ID);

            //First
            if (tenId == FumaShurikenTen.ID
                && !IsLastAction(false, FumaShurikenJin, FumaShurikenTen))
            {
                //AOE
                if (Katon.CanUse(out _))
                {
                    if (FumaShurikenJin.CanUse(out act)) return true;
                }
                //Single
                if (FumaShurikenTen.CanUse(out act)) return true;
            }

            //Second
            else if (tenId == KatonTen.ID && !IsLastAction(false, KatonTen))
            {
                if (KatonTen.CanUse(out act, CanUseOption.MustUse)) return true;
            }
            //Others
            else if (chiId == RaitonChi.ID && !IsLastAction(false, RaitonChi))
            {
                if (RaitonChi.CanUse(out act, CanUseOption.MustUse)) return true;
            }
            else if (chiId == DotonChi.ID && !IsLastAction(false, DotonChi))
            {
                if (DotonChi.CanUse(out act, CanUseOption.MustUse)) return true;
            }
            else if (jinId == SuitonJin.ID && !IsLastAction(false, SuitonJin))
            {
                if (SuitonJin.CanUse(out act, CanUseOption.MustUse)) return true;
            }
        }

       //Keep Kassatsu in Burst.
        if (!Player.WillStatusEnd(3, false, StatusID.Kassatsu) 
            && Player.HasStatus(false, StatusID.Kassatsu) && !InTrickAttack) return false;
        if (_ninActionAim == null) return false;

        var id = AdjustId(ActionID.Ninjutsu);

        //Failed
        if ((uint)id == RabbitMedium.ID)
        {
            ClearNinjutsu();
            act = null;
            return false;
        }
        //First
        else if (id == ActionID.Ninjutsu)
        {
            //Can't use.
            if (!Player.HasStatus(true, StatusID.Kassatsu, StatusID.TenChiJin)
                && !Ten.CanUse(out _, CanUseOption.EmptyOrSkipCombo)
                && !IsLastAction(false, _ninActionAim.Ninjutsu[0]))
            {
                return false;
            }

            if (NextAbilityToNextGCD == 0)
            {
                act = new BaseAction((ActionID)_ninActionAim.Ninjutsu[0].AdjustedID, ActionOption.Friendly);
            }
            return true;


        }
        //Finished
        else if ((uint)id == _ninActionAim.ID)
        {
            if ( WeaponElapsed == WeaponRemain && _ninActionAim.CanUse(out act, CanUseOption.MustUse)) return true;
            if (_ninActionAim.ID == Doton.ID && !InCombat)
            {
                act = _ninActionAim;
                return true;
            }
        }
        //Second
        else if ((uint)id == FumaShuriken.ID)
        {
            if (_ninActionAim.Ninjutsu.Length > 1
                && !IsLastAction(false, _ninActionAim.Ninjutsu[1])
                && NextAbilityToNextGCD == 0)
            {
                act = new BaseAction((ActionID)_ninActionAim.Ninjutsu[1].AdjustedID, ActionOption.Friendly);
                return true;
            }
        }
        //Third
        else if ((uint)id == Katon.ID || (uint)id == Raiton.ID || (uint)id == Hyoton.ID)
        {
            if (_ninActionAim.Ninjutsu.Length > 2
                && !IsLastAction(false, _ninActionAim.Ninjutsu[2])
                && WeaponElapsed == WeaponRemain)
            {
                act = new BaseAction((ActionID)_ninActionAim.Ninjutsu[2].AdjustedID, ActionOption.Friendly);
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Opener Logic
    private bool Opener(out IAction act)
    {
        act = default(IAction);
        while (OpenerInProgress && (!OpenerHasFinished || !OpenerHasFailed))
        {
            if (TimeSinceLastAction.TotalSeconds > 2.5 && !Flag)
            {
                OpenerHasFailed = true;
                OpenerInProgress = false;
                Openerstep = 0;

                Flag = true;
            }
            if (Player.IsDead && !Flag)
            {
                OpenerHasFailed = true;
                OpenerInProgress = false;
                Openerstep = 0;

                Flag = true;
            }
            switch (Configs.GetCombo("RotationSelection"))
            {
                case 0:
                    switch (Openerstep)
                    {
                        case 0:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Kassatsu.ID, Kassatsu.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 1:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == SpinningEdge.ID, SpinningEdge.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 2:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == GustSlash.ID, GustSlash.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 3:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Mug.ID, Mug.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 4:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Bunshin.ID, Bunshin.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 5:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == PhantomKamaitachi.ID, PhantomKamaitachi.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 6:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == TrickAttack.ID, TrickAttack.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 7:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == AeolianEdge.ID, AeolianEdge.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 8:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == DreamWithinADream.ID, DreamWithinADream.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 9:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == 18805, Ten.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 10:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == 18807, Jin.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 11:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == 16942, HyoshoRanryu.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 12:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == 2259, Ten.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 13:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == 18806, Chi.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 14:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Raiton.ID, Raiton.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 15:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == TenChiJin.ID, TenChiJin.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 16:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == FumaShurikenTen.ID, FumaShurikenTen.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 17:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == RaitonChi.ID, RaitonChi.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 18:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == SuitonJin.ID, SuitonJin.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 19:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Meisui.ID, Meisui.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 20:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == FleetingRaiju.ID, FleetingRaiju.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 21:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Bhavacakra.ID, Bhavacakra.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 22:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == FleetingRaiju.ID, FleetingRaiju.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 23:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Bhavacakra.ID, Bhavacakra.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 24:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == 2259, Ten.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 25:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == 18806, Chi.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 26:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Raiton.ID, NIN_Base.Raiton.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 27:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == FleetingRaiju.ID, FleetingRaiju.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 28:
                            OpenerHasFinished = true;
                            OpenerInProgress = false;

                            break;
                    }
                    break;
                case 1: 
                    switch (Openerstep)
                    {
                        case 0:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Kassatsu.ID, Kassatsu.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 1:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == SpinningEdge.ID, SpinningEdge.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 2:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == GustSlash.ID, GustSlash.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 3:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Mug.ID, Mug.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 4:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Bunshin.ID, Bunshin.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 5:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == PhantomKamaitachi.ID, PhantomKamaitachi.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 6:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == AeolianEdge.ID, AeolianEdge.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 7:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == TrickAttack.ID, TrickAttack.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 8:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == DreamWithinADream.ID, DreamWithinADream.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 9:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == 18805, Ten.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 10:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == 18807, Jin.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 11:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == 16942, HyoshoRanryu.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 12:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == 2259, Ten.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 13:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == 18806, Chi.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 14:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Raiton.ID, Raiton.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 15:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == TenChiJin.ID, TenChiJin.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 16:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == FumaShurikenTen.ID, FumaShurikenTen.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 17:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == RaitonChi.ID, RaitonChi.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 18:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == SuitonJin.ID, SuitonJin.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 19:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Meisui.ID, Meisui.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 20:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == FleetingRaiju.ID, FleetingRaiju.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 21:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Bhavacakra.ID, Bhavacakra.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 22:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == FleetingRaiju.ID, FleetingRaiju.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 23:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Bhavacakra.ID, Bhavacakra.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 24:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == 2259, Ten.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 25:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == 18806, Chi.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 26:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == Raiton.ID, NIN_Base.Raiton.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 27:
                            return OpenerStep(RecordActions?.FirstOrDefault().Action.RowId == FleetingRaiju.ID, FleetingRaiju.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 28:
                            OpenerHasFinished = true;
                            OpenerInProgress = false;
                            break;
                    }
                    break;
            }
        }
        act = null;
        return false;
    }
    private bool OpenerStep(bool condition, bool result)
    {
        if (condition)
        {
            Openerstep++;
            return false;
        }
        return result;
    }
#endregion

    protected override bool GeneralGCD(out IAction act)
    {
        #region PvP
        if (Player.HasStatus(true, StatusID.PvP_Hidden))
        {
            if (PvP_SpinningEdge.CanUse(out act)) return true;
        }
        if (PvP_SeitonTenchu.CanUse(out act)) return true;
        if (Player.HasStatus(true, StatusID.PvP_ThreeMudra))
        {
            if (!Player.HasStatus(true, StatusID.PvP_SealedMeisui) && Player.CurrentHp < 49000)
            {
                if (PvP_Meisui.CanUse(out act)) return true;
            }
            if (!Player.HasStatus(true, StatusID.PvP_SealedDoton))
            {
                if (PvP_Doton.CanUse(out act)) return true;
            }
            if (!Player.HasStatus(true, StatusID.PvP_SealedGokaMekkyaku))
            {
                if (PvP_GokaMekkyaku.CanUse(out act)) return true;
            }
            if (!Player.HasStatus(true, StatusID.PvP_SealedHyoshoRanryu))
            {
                if (PvP_HyoshoRanryu.CanUse(out act)) return true;
            }
            if (!Player.HasStatus(true, StatusID.PvP_SeakedForkedRaiju))
            {
                if (PvP_ForkedRaiju2.CanUse(out act)) return true;
            }
            if (!Player.HasStatus(true, StatusID.PvP_SealedHuton))
            {
                if (PvP_Huton.CanUse(out act)) return true;
            }
        }
        if (PvP_FumaShuriken.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
        if (PvP_AeolianEdge.CanUse(out act)) return true;
        if (PvP_GustSlash.CanUse(out act)) return true;
        if (PvP_SpinningEdge.CanUse(out act)) return true;
        #endregion

        #region PvE


        if (OpenerInProgress)
        {
            return Opener(out act);
        }
        if (!OpenerInProgress /*|| OpenerHasFailed || OpenerHasFinished*/)
        {

            var hasRaijuReady = Player.HasStatus(true, StatusID.RaijuReady);

            if ((InTrickAttack || InMug) && NoNinjutsu && !hasRaijuReady
                && PhantomKamaitachi.CanUse(out act)) return true;

            if (ChoiceNinjutsu(out act)) return true;
            if ((!InCombat || !CombatElapsedLess(9)) && DoNinjutsu(out act)) return true;

            //No Ninjutsu
            if (NoNinjutsu)
            {
                if (!CombatElapsedLess(10) && FleetingRaiju.CanUse(out act)) return true;
                if (hasRaijuReady) return false;

                if (Huraijin.CanUse(out act)) return true;

                //AOE
                if (HakkeMujinsatsu.CanUse(out act)) return true;
                if (DeathBlossom.CanUse(out act)) return true;

                //Single
                if (ArmorCrush2.CanUse(out act)) return true;
                if (AeolianEdge.CanUse(out act)) return true;
                if (GustSlash.CanUse(out act)) return true;
                if (SpinningEdge.CanUse(out act)) return true;

                //Range
                if (IsMoveForward && MoveForwardAbility(out act)) return true;
                if (ThrowingDagger.CanUse(out act)) return true;
            }

            if (Configs.GetBool("AutoUnhide"))
            {
                StatusHelper.StatusOff(StatusID.Hidden);
            }
            if (!InCombat && _ninActionAim == null && Configs.GetBool("UseHide")
                && Ten.IsCoolingDown && Hide.CanUse(out act)) return true;
        }
        #endregion
        return base.GeneralGCD(out act);
        
    }

    [RotationDesc(ActionID.ForkedRaiju)]
    protected override bool MoveForwardGCD(out IAction act)
    {
        if (ForkedRaiju.CanUse(out act)) return true;
        return base.MoveForwardGCD(out act);
    }


    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {

        if (OpenerInProgress)
        {
            return Opener(out act);
        }
        if (!OpenerInProgress /*|| OpenerHasFailed || OpenerHasFinished*/)
        {
            if (AdjustId(ActionID.Ninjutsu) == ActionID.RabbitMedium)
            {
                if (RabbitMedium.CanUse(out act)) return true;
            }
            if (!NoNinjutsu || !InCombat) return base.EmergencyAbility(nextGCD, out act);

            if (Kassatsu.CanUse(out act)) return true;
            if (UseBurstMedicine(out act)) return true;

            if (IsBurst && !CombatElapsedLess(5) && Mug.CanUse(out act)) return true;

            //Use Suiton
            if (!CombatElapsedLess(6))
            {
                if (TrickAttack.CanUse(out act)) return true;
                if (TrickAttack.IsCoolingDown && !TrickAttack.WillHaveOneCharge(19)
                    && Meisui.CanUse(out act)) return true;
            }
        }


        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(out IAction act)
    {
        if (OpenerInProgress)
        {
            return Opener(out act);
        }
        if (!OpenerInProgress /*|| OpenerHasFailed || OpenerHasFinished*/)
        {
            act = null;
            if (!NoNinjutsu || !InCombat) return false;
            if (NextAbilityToNextGCD > 0.5f)
            {



                if (!IsMoving && InTrickAttack && !Ten.ElapsedAfter(30) && TenChiJin.CanUse(out act)) return true;

                if (!CombatElapsedLess(5) && Bunshin.CanUse(out act)) return true;

                if (InTrickAttack)
                {
                    if (!DreamWithinADream.EnoughLevel)
                    {
                        if (Assassinate.CanUse(out act)) return true;
                    }
                    else
                    {
                        if (DreamWithinADream.CanUse(out act)) return true;
                    }
                }

                if ((!InMug || InTrickAttack)
                    && (!Bunshin.WillHaveOneCharge(10) || Player.HasStatus(false, StatusID.PhantomKamaitachiReady) || Mug.WillHaveOneCharge(2)))
                {
                    if (HellfrogMedium.CanUse(out act)) return true;
                    if (Bhavacakra.CanUse(out act)) return true;
                }
                if (Ninki > 85 || (Ninki >= 50 & Meisui.CanUse(out _)))
                {
                    if (HellfrogMedium.CanUse(out act)) return true;
                    if (Bhavacakra.CanUse(out act)) return true;
                }
            }
        }
        /*unsafe
        {
            if (!PvP_Shukuchi2.IsCoolingDown && Target is not null)
            {
                var loc = (FFXIVClientStructs.FFXIV.Common.Math.Vector3)Target.Position;
                ActionManager.Instance()->UseActionLocation(ActionType.Action, PvP_Shukuchi2.ID, 3758096384, &loc);
            }
        }*/
        if (PvP_Shukuchi2.CanUse(out act)) return true;
        if (PvP_Mug.CanUse(out act)) return true;
        if (PvP_Bunshin2.CanUse(out act)) return true;
        if (PvP_ThreeMudra.CanUse(out act, CanUseOption.MustUseEmpty) && !Player.HasStatus(true,StatusID.PvP_ThreeMudra)) return true;
        return base.AttackAbility(out act);
    }
}