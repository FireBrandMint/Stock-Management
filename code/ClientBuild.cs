using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


public static class ClientBuild
{
    private static Ticker ProcessArbiter = new Ticker(20, 1.0);
    public static int TPS => ProcessArbiter.TPS;
    public static async void Run(string[] args)
    {
        Init();

        double delta;
        while(Program.IsAlive)
        {
            if(ProcessArbiter.ShouldExecute(out delta))
                Tick(delta);
        }
    }

    static async void Init()
    {
        
    }

    static void Tick(double delta)
    {
        
    }
}