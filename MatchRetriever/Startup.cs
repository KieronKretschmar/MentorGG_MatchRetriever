using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EquipmentLib;
using MatchRetriever.Helpers;
using MatchRetriever.Misplays;
using MatchRetriever.ModelFactories;
using MatchRetriever.ModelFactories.DemoViewer;
using MatchRetriever.ModelFactories.GrenadesAndKills;
using MatchRetriever.ModelFactories.GrenadesAndKillsOverviews;
using MatchRetriever.Models.GrenadesAndKills;
using MatchRetriever.Models.GrenadesAndKillsOverviews;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZoneReader;

namespace MatchRetriever
{
    /// <summary>
    /// Requires env variables ["MYSQL_CONNECTION_STRING"]
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                // Set serializer to newtonsoft json
                .AddNewtonsoftJson(x => x.UseMemberCasing());

            services.AddLogging(services =>
            {
                services.AddConsole();
                services.AddDebug();
            });

            // if a connectionString is set use mysql, else use InMemory
            var connString = Configuration.GetValue<string>("MYSQL_CONNECTION_STRING");
            if (connString != null)
            {
                services.AddDbContext<Database.MatchContext>(o =>  o.UseLazyLoadingProxies().UseMySql(connString));
            }
            else
            {
                    services.AddDbContext<Database.MatchContext>((sp, options) =>
                    {
                        options.UseInMemoryDatabase(databaseName: "MyInMemoryDatabase");
                    });
            }

            #region Add ModelFactories for GrenadeAndKills
            // ModelFactories with dependencies ...
            services.AddScoped<IFireNadeModelFactory, FireNadeModelFactory>();
            services.AddScoped<IFlashModelFactory, FlashModelFactory>();
            services.AddScoped<IHeModelFactory, HeModelFactory>();
            services.AddScoped<ISmokeModelFactory, SmokeModelFactory>();
            services.AddScoped<IKillModelFactory, KillModelFactory>();
            // ... SampleFactories
            services.AddScoped<ISampleFactory<FireNadeSample>, FireNadeSampleFactory>();
            services.AddScoped<ISampleFactory<FlashSample>, FlashSampleFactory>();
            services.AddScoped<ISampleFactory<HeSample>, HeSampleFactory>();
            services.AddScoped<ISampleFactory<SmokeSample>, SmokeSampleFactory>();
            services.AddScoped<ISampleFactory<KillSample>, KillSampleFactory>();
            // ... LineupFactories
            services.AddScoped<ILineupPerformanceFactory<SmokeSample, SmokeLineupPerformance>, SmokeLineupModelFactory>();
            // ... ZoneFactories
            services.AddScoped<IZonePerformanceFactory<FireNadeSample, FireNadeZonePerformance>, FireNadeZoneModelFactory>();
            services.AddScoped<IZonePerformanceFactory<FlashSample, FlashZonePerformance>, FlashZoneModelFactory>();
            services.AddScoped<IZonePerformanceFactory<HeSample, HeZonePerformance>, HeZoneModelFactory>();
            services.AddScoped<IZonePerformanceFactory<KillSample, KillZonePerformance>, KillZoneModelFactory>();
            // ... FilterableZoneFactories
            services.AddScoped<IFilterableZoneModelFactory<KillSample, KillZonePerformance, KillFilterSetting>, KillFilterableZoneModelFactory>();


            //// Add OverviewModelFactories for GrenadeAndKills
            services.AddScoped<IOverviewModelFactory<FireNadeOverviewMapSummary>, FireNadeOverviewModelFactory>();
            services.AddScoped<IOverviewModelFactory<FlashOverviewMapSummary>, FlashesOverviewModelFactory>();
            services.AddScoped<IOverviewModelFactory<HeOverviewMapSummary>, HeOverviewModelFactory>();
            services.AddScoped<IOverviewModelFactory<SmokeOverviewMapSummary>, SmokeOverviewModelFactory>();
            services.AddScoped<IOverviewModelFactory<KillOverviewMapSummary>, KillOverviewModelFactory>();
            #endregion

            #region Add other ModelFactories
            services.AddScoped<IMatchSelectionModelFactory, MatchSelectionModelFactory>();
            services.AddScoped<IMatchesModelFactory, MatchesModelFactory>();
            services.AddScoped<IFriendsComparisonModelFactory, FriendsComparisonModelFactory>();
            services.AddScoped<IDemoViewerMatchModelFactory, DemoViewerMatchModelFactory>();
            services.AddScoped<IDemoViewerRoundModelFactory, DemoViewerRoundModelFactory>();
			services.AddSingleton<IZoneReader, FileReader>(services =>
             {
                 //TODO MANDATORY switch to env var
                 return new FileReader(@"C:\Users\Lasse\source\repos\MatchRetriever\ZoneReader\ZoneReader\resources\");
             }); 
           
			services.AddSingleton<IEquipmentProvider, EquipmentProvider>(services =>
             {
                 //TODO MANDATORY Remove hardcoded path
                 return new EquipmentProvider(services.GetRequiredService<ILogger<EquipmentProvider>>(), @"C:\Users\Lasse\source\repos\MatchRetriever\EquipmentLib\EquipmentLib\EquipmentData\");
             });

            #endregion

            #region Misplay detectors
            services.AddScoped<_detectorHelpers>();


            //Add new misplays here
            services.AddScoped<ISubdetector, BadBombDropDetector>();
            services.AddScoped<ISubdetector, SmokeFailDetector>();
            services.AddScoped<ISubdetector, ShotWhileMovingDetector>();
            services.AddScoped<ISubdetector, SelfFlashDetector>();
            services.AddScoped<ISubdetector, TeamFlashDetector>();
            services.AddScoped<ISubdetector, UnnecessaryReloadDetector>();

            services.AddScoped<IMisplayDetector, MisplayDetector>(services =>
            {
                return new MisplayDetector(services.GetServices<ISubdetector>().ToList());
            });

            services.AddScoped<IMisplayModelFactory, MisplayModelFactory>();
            #endregion

            // Add other services            
            services.AddSingleton<ISteamUserOperator>(services =>
            {
                return new MockSteamUserOperator();
                //return new SteamUserOperator(services.GetService<ILogger>(), Configuration.GetValue<string>("STEAMUSEROPERATOR_URI"));
            });

            // Enable versioning
            // See https://dotnetcoretutorials.com/2017/01/17/api-versioning-asp-net-core/
            services.AddMvc();
            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
