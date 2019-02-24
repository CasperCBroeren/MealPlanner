using Joonasw.AspNetCore.SecurityHeaders;
using MealPlanner.Data.Repositories;
using MealPlanner.Data.Repositories.Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Umi.Core;

namespace mealplanner
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true) 
                .AddEnvironmentVariables();

            if (!env.IsDevelopment())
            {
                builder.AddAzureKeyVault("https://maaltijdplanner.vault.azure.net");
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.TryAddTransient<ITagRepository, TagRepository>();
            services.TryAddTransient<IIngredientRepository, IngredientRepository>();
            services.TryAddTransient<IWeekplanningRepository, WeekplanningRepository>();
            services.TryAddTransient<IMealRepository, MealRepository>();
            services.TryAddTransient<IGroupRepository, GroupRepository>();
            services.TryAddTransient<IBoughtIngredientRepository, BoughtIngredientRepository>();
            // Umi for urls
            services.AddUmi();
            // Add framework services.
            services.AddMvc();

            services.AddAuthentication(options =>
           {
               options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
               options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

           })
               .AddJwtBearer(options =>
              {
                  options.RequireHttpsMetadata = false;
                  options.SaveToken = true;
                  options.TokenValidationParameters = new TokenValidationParameters()
                  {
                      ValidateIssuerSigningKey = true,
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["jwtSecret"])),
                      ValidateAudience = true,
                      ValidAudience = "MealPlanner",
                      ValidateIssuer = false,
                      ValidateLifetime = true
                      
                  };
              });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("GroupOnly", policy =>
                {
                    policy.RequireClaim("GroupId");
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                });
            });

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCsp(csp =>
            {
                csp.ByDefaultAllow.FromSelf();
                csp.AllowScripts.FromSelf().From("*.vo.msecnd.net").From("dc.services.visualstudio.com");
                csp.AllowConnections.ToSelf().To("dc.services.visualstudio.com");
                csp.AllowFonts.From("data:").FromSelf();
                csp.AllowImages.From("data:").FromSelf();
                csp.AllowStyles.From("'unsafe-inline'").FromSelf();
            });


            app.UseStaticFiles();

            app.UseUmi();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }

    }
}
