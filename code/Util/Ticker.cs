using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;


public class Ticker
{
    public int TPS
    {
        get => _TPS;

        set
        {
            if(value <= 0.0)
                throw new ArgumentException("TPS should be above 0.");

            _TPS = value;
            TickDelay = 1.0 / value;
            TPSFrequencyFraction = (double)value / Stopwatch.Frequency;
        }
    }
    int _TPS;
    double TickDelay;
    private double TPSFrequencyFraction;
    double Delta = 0.0;
    double SkipTickThresold;
    /// <summary>
    /// Last time stamp processed in ticks.
    /// </summary>
    long LastStamp;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tps"></param>
    /// <param name="skipTickThresold"> How much extra tick after process is acceptable</param>
    /// <exception cref="ArgumentException"></exception>
    public Ticker(int tps, double skipTickThresold)
    {
        TPS = tps;
        if(skipTickThresold <= 0.0)
            throw new ArgumentException("Skip thresold should should be more than 0");
        SkipTickThresold = skipTickThresold;
        LastStamp = Stopwatch.GetTimestamp();
    }
    public bool ShouldExecute(out double elapsingTicks)
    {
        var curr_stamp = Stopwatch.GetTimestamp();
        var last_stamp = LastStamp;
        var stamp_to_delta_multiplier = TPSFrequencyFraction;
        var delta = Delta;

        delta += (double)(curr_stamp - last_stamp) * stamp_to_delta_multiplier;
        LastStamp = curr_stamp;
        
        bool should_process = delta >= 1.0;

        if(should_process)
        {
            delta -= 1.0;
            if(delta > SkipTickThresold)
            {
                elapsingTicks = 1.0 + delta;
                delta = 0.0;
            }
            else
                elapsingTicks = 1.0;
        }
        else
        {
            elapsingTicks = 0.0;
            double reverse_delta = 1.0 - delta;
            Thread.Sleep((int)(reverse_delta * (TickDelay * 1000)));
        }

        Delta = delta;

        return should_process;
    }
}