
using System;
using System.Threading.Tasks;

public static class Program
{
    public static volatile bool IsAlive;
    public static async Task<int> Main(string[] args)
    {
        Console.WriteLine("Main executed.");
        IsAlive = true;

        await ServerBuild.Run(args);

        return 0;
    }
}