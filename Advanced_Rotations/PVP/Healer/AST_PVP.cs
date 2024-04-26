namespace RabbsRotations.Healer;

[Rotation("Rabbs AST PVP", CombatType.PvP, GameVersion = "6.58")]
public sealed class AST_PVP : AstrologianRotation
{
    [Range(4, 20, ConfigUnitType.Seconds)]
    [RotationConfig(CombatType.PvE, Name = "Use Earthly Star during countdown timer.")]
    public float UseEarthlyStarTime { get; set; } = 15;

    protected override IAction? CountDownAction(float remainTime)
    {
        

        return base.CountDownAction(remainTime);
    }


    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? act)
    {
        
        return base.DefenseSingleAbility(nextGCD, out act);
    }


    protected override bool DefenseAreaGCD(out IAction? act)
    {
        act = null;
       
        return base.DefenseAreaGCD(out act);
    }


    protected override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
       
        return base.DefenseAreaAbility(nextGCD, out act);
    }

    protected override bool GeneralGCD(out IAction? act)
    {
        

        return base.GeneralGCD(out act);
    }


    protected override bool HealAreaGCD(out IAction? act)
    {
       
        return base.HealAreaGCD(out act);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
       
        
        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool GeneralAbility(IAction nextGCD, out IAction? act)
    {

        return base.GeneralAbility(nextGCD, out act);
    }


    protected override bool HealSingleGCD(out IAction? act)
    {


        return base.HealSingleGCD(out act);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {


        return base.AttackAbility(nextGCD, out act);
    }


    protected override bool HealSingleAbility(IAction nextGCD, out IAction? act)
    {

        return base.HealSingleAbility(nextGCD, out act);
    }


    protected override bool HealAreaAbility(IAction nextGCD, out IAction? act)
    {


        return base.HealAreaAbility(nextGCD, out act);
    }
}
