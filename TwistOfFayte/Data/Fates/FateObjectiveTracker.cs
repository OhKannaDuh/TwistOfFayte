using System.Collections.Generic;
using System.Linq;

namespace TwistOfFayte.Data.Fates;

public sealed class FateObjectiveTracker(byte initial)
{
    private readonly byte initial = initial;

    private readonly List<byte> progressReports = [initial];

    private int stride;

    public int Stride
    {
        get => stride;
    }

    private byte LastProgress
    {
        get => progressReports[^1];
    }

    private byte RemainingProgress
    {
        get => (byte)(100 - LastProgress);
    }

    public int ObjectivesCompleted
    {
        get => stride == 0 ? 0 : LastProgress / stride;
    }

    public int ObjectivesRemaining
    {
        get => stride == 0 ? -1 : RemainingProgress / stride;
    }

    public int TotalObjectives
    {
        get => stride == 0 ? -1 : 100 / stride;
    }

    public void Observe(Fate fate)
    {
        var progress = fate.Progress;
        var last = LastProgress;
        if (progress <= last)
        {
            return;
        }

        progressReports.Add(progress);
        var strides = new List<byte>(progressReports.Count - 1);
        for (var i = 1; i < progressReports.Count; i++)
        {
            strides.Add((byte)(progressReports[i] - progressReports[i - 1]));
        }

        stride = strides.Min();
    }
}
