

public static class Program
{
    public static volatile bool IsAlive;
    public static async void Main(string[] args)
    {
        IsAlive = true;

        #if SERVER
            ServerBuild.Run(args);
        #elif CLIENT
            ClientBuild.Run(args);
        #endif
    }
}