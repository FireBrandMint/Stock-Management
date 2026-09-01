using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


public static class ClientBuild
{
    private static Ticker ProcessArbiter = new Ticker(20, 1.0);
    public static int TPS => ProcessArbiter.TPS;
    public static async Task Run(string[] args)
    {
        Init();

        while(Program.IsAlive)
        {
            var se = await ProcessArbiter.ShouldExecute();
            if(se.can_run)
                Tick(se.elapsing_ticks);
        }
    }

    static async void Init()
    {
        
    }

    static void Tick(double delta)
    {
        
    }
}