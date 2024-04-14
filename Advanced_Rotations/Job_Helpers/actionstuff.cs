namespace RabbsRotations.Job_Helpers
{
    internal abstract partial class CustomComboFunctions
    {
        public static bool ActionReady(IBaseAction id) => id.EnoughLevel && (id.Cooldown.HasOneCharge || id.Cooldown.RecastTimeRemainOneCharge <= 3);
        public static ushort GetRemainingCharges(IBaseAction id) => id.Cooldown.CurrentCharges;
        public static float GetCooldownRemainingTime(IBaseAction id) => id.Cooldown.RecastTimeRemainOneCharge;
        public static bool IsOnCooldown(IBaseAction id) => id.Cooldown.IsCoolingDown;
        public static bool IsOffCooldown(IBaseAction id) => id.Cooldown.HasOneCharge;

    }
}
