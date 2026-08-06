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
        //Init the APP
        await Init(args);

        //Start ticking
        ProcessArbiter = new Ticker(60, 1.0);
        double delta;
        while(Program.IsAlive)
        {
            if(ProcessArbiter.ShouldExecute(out delta))
                Tick(delta);
        }

        await App.StopAsync();
    }

    static async Task Init(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        builder.Services.AddEndpointsApiExplorer();

        //DB config//

        services.AddDbContext<ApplicationDBContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

        //Auth//
        services.AddAuthorization();

        services.AddIdentity<DBUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDBContext>();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "StockAuth";

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

        if(app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //This works because the thread is launched instead of blocking.
        app.StartAsync().Wait();
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
                if (ProcessArbiter.ShouldExecute(out double delta))
                    Tick(delta);
            }
        }
    }
}