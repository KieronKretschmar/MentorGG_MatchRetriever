using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Database;
using EquipmentLib;
using MatchRetriever.Helpers;
using MatchRetriever.Middleware;
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
using Microsoft.OpenApi.Models;
using ZoneReader;

namespace MatchRetriever
{
    /// <summary>
    /// Requires env variables ["MYSQL_CONNECTION_STRING", "EQUIPMENT_CSV_DIRECTORY", "STEAMUSEROPERATOR_URI"]
    /// Optional env variables ["EQUIPMENT_ENDPOINT"]
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
                .AddNewtonsoftJson(x => 
                {
                    x.UseMemberCasing();
                    // Serialize longs (steamIds) as strings
                    x.SerializerSettings.Converters.Add(new LongToStringConverter());
                    x.SerializerSettings.Converters.Add(new PolygonConverter());
                });

            services.AddLogging(services =>
            {
                services.AddConsole(o =>
                {
                    o.TimestampFormat = "[yyyy-MM-dd HH:mm:ss zzz] ";
                });
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

                    Console.WriteLine("WARNING: Running InMemory Database!");
            }

            #region Read environment variables
            var STEAMUSEROPERATOR_URI = Configuration.GetValue<string>("STEAMUSEROPERATOR_URI");
            var EQUIPMENT_CSV_DIRECTORY = Configuration.GetValue<string>("EQUIPMENT_CSV_DIRECTORY");
            if(EQUIPMENT_CSV_DIRECTORY == null)
                throw new ArgumentNullException("The environment variable EQUIPMENT_CSV_DIRECTORY has not been set.");
            var EQUIPMENT_ENDPOINT = Configuration.GetValue<string>("EQUIPMENT_ENDPOINT");
            var ZONEREADER_RESOURCE_PATH = Configuration.GetValue<string>("ZONEREADER_RESOURCE_PATH");
            if (ZONEREADER_RESOURCE_PATH == null)
                throw new ArgumentNullException("The environment variable ZONEREADER_RESOURCE_PATH has not been set.");
            #endregion

            #region Swagger
            services.AddSwaggerGen(options =>
            {
                OpenApiInfo interface_info = new OpenApiInfo { Title = "MatchRetriever", Version = "v1", };
                options.SwaggerDoc("v1", interface_info);

                // Generate documentation based on the XML Comments provided.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                // Optionally, if installed, enable annotations
                options.EnableAnnotations();
            });
            #endregion


            #region Add ModelFactories for GrenadeAndKills
            // ModelFactories with dependencies ...
            services.AddTransient<IFireNadeModelFactory, FireNadeModelFactory>();
            services.AddTransient<IFlashModelFactory, FlashModelFactory>();
            services.AddTransient<IHeModelFactory, HeModelFactory>();
            services.AddTransient<ISmokeModelFactory, SmokeModelFactory>();
            services.AddTransient<IKillModelFactory, KillModelFactory>();
            // ... SampleFactories
            services.AddTransient<ISampleFactory<FireNadeSample>, FireNadeSampleFactory>();
            services.AddTransient<ISampleFactory<FlashSample>, FlashSampleFactory>();
            services.AddTransient<ISampleFactory<HeSample>, HeSampleFactory>();
            services.AddTransient<ISampleFactory<SmokeSample>, SmokeSampleFactory>();
            services.AddTransient<ISampleFactory<KillSample>, KillSampleFactory>();
            // ... LineupFactories
            services.AddTransient<ILineupPerformanceFactory<SmokeSample, SmokeLineupPerformance>, SmokeLineupModelFactory>();
            // ... ZoneFactories
            services.AddTransient<IZonePerformanceFactory<FireNadeSample, FireNadeZonePerformance>, FireNadeZoneModelFactory>();
            services.AddTransient<IZonePerformanceFactory<FlashSample, FlashZonePerformance>, FlashZoneModelFactory>();
            services.AddTransient<IZonePerformanceFactory<HeSample, HeZonePerformance>, HeZoneModelFactory>();
            services.AddTransient<IZonePerformanceFactory<KillSample, KillZonePerformance>, KillZoneModelFactory>();
            // ... FilterableZoneFactories
            services.AddTransient<IFilterableZoneModelFactory<KillSample, KillZonePerformance, KillFilterSetting>, KillFilterableZoneModelFactory>();

            // Add ImportantPositions
            services.AddTransient<IImportantPositionsModelFactory, ImportantPositionsModelFactory>();


            // Add OverviewModelFactories for GrenadeAndKills
            services.AddTransient<IOverviewModelFactory<FireNadeOverviewMapSummary>, FireNadeOverviewModelFactory>();
            services.AddTransient<IOverviewModelFactory<FlashOverviewMapSummary>, FlashesOverviewModelFactory>();
            services.AddTransient<IOverviewModelFactory<HeOverviewMapSummary>, HeOverviewModelFactory>();
            services.AddTransient<IOverviewModelFactory<SmokeOverviewMapSummary>, SmokeOverviewModelFactory>();
            services.AddTransient<IOverviewModelFactory<KillOverviewMapSummary>, KillOverviewModelFactory>();


            #endregion

            #region Add other ModelFactories
            services.AddTransient<IPlayerInfoModelFactory, PlayerInfoModelFactory>();
            services.AddTransient<IMatchSelectionModelFactory, MatchSelectionModelFactory>();
            services.AddTransient<IMatchesModelFactory, MatchesModelFactory>();
            services.AddTransient<IFriendsComparisonModelFactory, FriendsComparisonModelFactory>();
            services.AddTransient<IDemoViewerMatchModelFactory, DemoViewerMatchModelFactory>();
            services.AddTransient<IDemoViewerRoundModelFactory, DemoViewerRoundModelFactory>();
            services.AddTransient<IPlayerSummaryModelFactory, PlayerSummaryModelFactory>();
            #endregion

            #region Misplay detectors
            services.AddTransient<_detectorHelpers>();


            //Add new misplays here
            services.AddTransient<ISubdetector, BadBombDropDetector>();
            services.AddTransient<ISubdetector, SmokeFailDetector>();
            services.AddTransient<ISubdetector, ShotWhileMovingDetector>();
            services.AddTransient<ISubdetector, SelfFlashDetector>();
            services.AddTransient<ISubdetector, TeamFlashDetector>();
            services.AddTransient<ISubdetector, UnnecessaryReloadDetector>();

            services.AddTransient<IMisplayDetector, MisplayDetector>(services =>
            {
                return new MisplayDetector(services.GetServices<ISubdetector>().ToList());
            });

            services.AddTransient<IMisplayModelFactory, MisplayModelFactory>();
            #endregion

            #region Add Helper services          
            services.AddSingleton<ISteamUserOperator>(services =>
            {
                if (STEAMUSEROPERATOR_URI != null)
                {
                    return new SteamUserOperator(services.GetService<ILogger<SteamUserOperator>>(), Configuration.GetValue<string>("STEAMUSEROPERATOR_URI"));
                }
                else
                {
                    Console.WriteLine("STEAMUSEROPERATOR_URI not provided. Using Mock instead. This should not happen in production.");
                    return new MockSteamUserOperator();
                }
            });

            services.AddSingleton<IEquipmentProvider, EquipmentProvider>(services =>
            {
                return new EquipmentProvider(services.GetService<ILogger<EquipmentProvider>>(), EQUIPMENT_CSV_DIRECTORY, EQUIPMENT_ENDPOINT);
            });
            services.AddSingleton<IZoneReader, FileReader>(services =>
            {
                return new FileReader(services.GetService<ILogger<FileReader>>(), ZONEREADER_RESOURCE_PATH);
            });
            #endregion

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "swagger";
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "MatchRetriever");
            });
            #endregion
        }
    }
}
