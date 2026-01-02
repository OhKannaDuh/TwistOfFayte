using System;
using System.Collections.Generic;
using TwistOfFayte.Data.Fates;

namespace TwistOfFayte.Config;

[Serializable]
public class FateBlacklist
{
    public HashSet<ushort> Blacklist { get; set; } = [];

    internal bool Contains(ushort id)
    {
        return Blacklist.Contains(id);
    }

    internal bool Contains(Fate fate)
    {
        return Contains(fate.Id.Value);
    }

    internal void Toggle(ushort id)
    {
        if (Blacklist.Contains(id))
        {
            Blacklist.Remove(id);
        }
        else
        {
            Blacklist.Add(id);
        }
    }

    internal void Toggle(Fate fate)
    {
        Toggle(fate.Id.Value);
    }
}
