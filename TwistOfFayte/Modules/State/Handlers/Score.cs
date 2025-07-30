﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TwistOfFayte.Modules.State.Handlers;

public class Score
{
    private readonly Dictionary<string, float> sources = [];

    public IReadOnlyDictionary<string, float> Sources {
        get => sources;
    }

    public float Value {
        get => Math.Max(sources.Values.Where(f => f != float.MaxValue && f != float.MinValue).Sum(), 0f);
    }

    public void Add(string source, float value)
    {
        sources.Add(source, value);
    }

    public void Clear()
    {
        sources.Clear();
    }

    public static implicit operator float(Score s)
    {
        return s.Value;
    }

    public override string ToString()
    {
        return Value.ToString("F2");
    }
}
