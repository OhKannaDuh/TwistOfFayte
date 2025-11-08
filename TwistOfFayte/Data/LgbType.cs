namespace TwistOfFayte.Data;

public enum LgbType
{
    Bg,
    PlanEvent,
    PlanLive,
    PlanMap,
    Planner,
    Sound,
    Vfx,
}

public static class LgbTypeEx
{
    public static string GetFileName(this LgbType type)
    {
        return $"{type.ToString().ToLower()}.lgb";
    }
}
