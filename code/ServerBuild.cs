using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


public static class ServerBuild
{
    public static WebApplication App = null!;
    public static int TPS => ProcessArbiter.TPS;
    private static Ticker ProcessArbiter = null!;

    public static async Task Run(string[] args)
    {
        Console.WriteLine("Starting endpoint aplication.");
        //Init the APP
        await Init(args);

        await ConsoleLoop();

        //Start ticking
        ProcessArbiter = new Ticker(60, 1.0);
        while(Program.IsAlive)
        {
            var se = await ProcessArbiter.ShouldExecute();
            if(se.can_run)
                Tick(se.elapsing_ticks);
        }

        await App.StopAsync();
    }

    private static async Task ConsoleLoop()
    {
        while (Program.IsAlive)
        {
            string? command = await ReadLineAsync();

            if (command == "stop")
                Program.IsAlive = false;
        }
    }

    private static Task<string?> ReadLineAsync()
    {
        return Task.Run(() => Console.ReadLine());
    }

    static async Task Init(string[] args)
    {
        //Console.WriteLine("Not blocked 1");
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        builder.Services.AddEndpointsApiExplorer();

        //DB config//

        services.AddDbContext<UserDBContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("Users")));

        services.AddDbContext<ProductDBContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("ProductRegistry")));

        services.AddDbContext<StockDBContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("Stock")));

        //Auth//
        services.AddAuthorization();

        services.AddIdentity<DBUser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 3;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
        }).AddEntityFrameworkStores<UserDBContext>();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "AuthStockApp";

            options.ExpireTimeSpan = TimeSpan.FromDays(7);

            options.SlidingExpiration = true;

            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };

            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });

        //Register controllers//
        services.AddControllers();
        
        //Build//
        var app = builder.Build();
        App = app;

        //Enable features//
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        //Console.WriteLine("Not blocked 2");
        
        //Register roles//
        using (var scope = app.Services.CreateScope())
        {
            var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var name in DBUser.Roles.Keys)
            {
                if (!await roles.RoleExistsAsync(name))
                    await roles.CreateAsync(new IdentityRole(name));
            }
        }

        //Console.WriteLine("Not blocked 3");

        if(app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //This works because the thread is launched instead of blocking.
        await app.StartAsync();

        //Console.WriteLine("Not blocked 4");

        Console.WriteLine("Loaded all!.");
    }

    static void Tick(double delta)
    {
        
    }

    public sealed class TickService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (Program.IsAlive)
            {
                var result = await ProcessArbiter.ShouldExecute();
                if (result.can_run)
                    Tick(result.elapsing_ticks);
            }
        }
    }
}